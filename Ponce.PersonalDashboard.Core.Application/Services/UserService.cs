using Mapster;
using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Application.Contracts;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.Users;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Users;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.Application.Services
{
    /// <summary>Keeps the local user in sync with the identity provider, plus display name and settings.</summary>
    public class UserService(IUserRepository userRepository, ICurrentUser currentUser, ILogger<UserService> logger)
    {
        private readonly IUserRepository _userRepository = userRepository;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly ILogger<UserService> _logger = logger;

        /// <summary>Creates the user on their first request and refreshes the username if it changed. Runs on every authenticated request.</summary>
        public async Task SyncUser(Guid userId, string userName, CancellationToken ct = default)
        {
            var user = await _userRepository.GetUserById(userId, ct);
            if (user is null)
            {
                if (await _userRepository.TryInsertUser(new User(userId, userName), ct))
                    _logger.LogInformation("User {UserId} registered on first request", userId);
                return;
            }
            if (user.SyncIdentity(userName))
            {
                await _userRepository.SaveChanges(ct);
                _logger.LogInformation("User {UserId} updated from the identity provider", userId);
            }
        }

        public async Task<GetUserResponse> GetCurrentUser(CancellationToken ct = default) =>
            (await GetCurrentUserOrThrow(ct)).Adapt<GetUserResponse>();

        public async Task<GetUserResponse> UpdateCurrentUser(UpdateUserRequest request, CancellationToken ct = default)
        {
            var user = await GetCurrentUserOrThrow(ct);
            user.SetDisplayName(request.DisplayName);
            await _userRepository.SaveChanges(ct);
            _logger.LogInformation("User {UserId} updated", user.Id);
            return user.Adapt<GetUserResponse>();
        }

        public async Task<UserSettingsResponse> GetSettings(CancellationToken ct = default)
        {
            var settings = await _userRepository.GetUserSettings(_currentUser.UserId, ct) ?? UserSettings.CreateDefault(_currentUser.UserId);
            return ToResponse(settings);
        }

        public async Task<UserSettingsResponse> SaveSettings(SaveUserSettingsRequest request, CancellationToken ct = default)
        {
            var settings = await _userRepository.GetUserSettings(_currentUser.UserId, ct);
            if (settings is null)
            {
                settings = new UserSettings(
                    _currentUser.UserId, request.Theme, request.Language, request.DateFormat, request.Currency,
                    request.ShowDecimals, request.AutoTimeZone, request.TimeZone, request.ProfilePicture, request.OverviewWidgets.Summary,
                    request.OverviewWidgets.Spending, request.OverviewWidgets.Dates, request.OverviewWidgets.Applications);
                await _userRepository.SaveUserSettings(settings, ct);
            }
            else
            {
                settings.Update(
                    request.Theme, request.Language, request.DateFormat, request.Currency, request.ShowDecimals,
                    request.AutoTimeZone, request.TimeZone, request.ProfilePicture, request.OverviewWidgets.Summary, request.OverviewWidgets.Spending,
                    request.OverviewWidgets.Dates, request.OverviewWidgets.Applications);
            }
            await _userRepository.SaveChanges(ct);
            _logger.LogInformation("Settings of user {UserId} saved", _currentUser.UserId);
            return ToResponse(settings);
        }

        private static UserSettingsResponse ToResponse(UserSettings settings) => new()
        {
            Theme = settings.Theme,
            Language = settings.Language,
            DateFormat = settings.DateFormat,
            Currency = settings.Currency,
            ShowDecimals = settings.ShowDecimals,
            AutoTimeZone = settings.AutoTimeZone,
            TimeZone = settings.TimeZone,
            ProfilePicture = settings.ProfilePicture,
            OverviewWidgets = new OverviewWidgetsResponse
            {
                Summary = settings.ShowSummaryWidget,
                Spending = settings.ShowSpendingWidget,
                Dates = settings.ShowDatesWidget,
                Applications = settings.ShowApplicationsWidget
            }
        };

        // Sync runs before every endpoint, so a missing user means it got skipped.
        private async Task<User> GetCurrentUserOrThrow(CancellationToken ct) =>
            await _userRepository.GetUserById(_currentUser.UserId, ct) ??
                throw new NotFoundException(
                    CustomExceptionCodes.USER_NOT_FOUND,
                    $"User with id {_currentUser.UserId} doesn't exist",
                    new Dictionary<string, object> { { "UserId", _currentUser.UserId } });
    }
}
