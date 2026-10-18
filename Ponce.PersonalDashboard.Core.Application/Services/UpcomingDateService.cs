using Mapster;
using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Application.Contracts;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.UpcomingDates;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.UpcomingDates;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.UpcomingDate;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers.UpcomingDate;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.Application.Services
{
    /// <summary>CRUD for upcoming dates.</summary>
    public class UpcomingDateService(IUpcomingDateRepository upcomingDateRepository, ICurrentUser currentUser, ILogger<UpcomingDateService> logger)
    {
        private readonly IUpcomingDateRepository _upcomingDateRepository = upcomingDateRepository;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly ILogger<UpcomingDateService> _logger = logger;

        public IQueryable<GetUpcomingDatesResponse> GetUpcomingDates(UpcomingDateStatus? status = null, DateOnly? from = null, DateOnly? to = null)
        {
            var query = _upcomingDateRepository.QueryUpcomingDates(_currentUser.UserId);
            if (status.HasValue)
                query = query.Where(x => x.Status == status.Value);
            if (from.HasValue)
                query = query.Where(x => x.Date >= from.Value);
            if (to.HasValue)
                query = query.Where(x => x.Date <= to.Value);
            return query.ProjectToType<GetUpcomingDatesResponse>();
        }

        public async Task<GetUpcomingDateResponse> GetUpcomingDate(Guid upcomingDateId, CancellationToken ct = default)
        {
            var upcomingDate = await GetUpcomingDateOrThrow(upcomingDateId, ct);
            return upcomingDate.Adapt<GetUpcomingDateResponse>();
        }

        public async Task<SaveUpcomingDateResponse> SaveUpcomingDate(SaveUpcomingDateRequest request, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving upcoming date {Title}", request.Title);
            var upcomingDate = new UpcomingDate(
                _currentUser.UserId,
                request.Date,
                request.Time,
                request.Title,
                request.Description,
                request.Status);
            await _upcomingDateRepository.SaveUpcomingDate(upcomingDate, ct);
            await _upcomingDateRepository.SaveChanges(ct);
            _logger.LogInformation("Upcoming date {UpcomingDateId} saved successfully", upcomingDate.Id);
            return upcomingDate.Adapt<SaveUpcomingDateResponse>();
        }

        public async Task<SaveUpcomingDateResponse> UpdateUpcomingDate(Guid upcomingDateId, SaveUpcomingDateRequest request, CancellationToken ct = default)
        {
            var upcomingDate = await GetUpcomingDateOrThrow(upcomingDateId, ct);
            upcomingDate.Update(
                request.Date,
                request.Time,
                request.Title,
                request.Description,
                request.Status);
            await _upcomingDateRepository.SaveChanges(ct);
            _logger.LogInformation("Upcoming date {UpcomingDateId} updated successfully", upcomingDate.Id);
            return upcomingDate.Adapt<SaveUpcomingDateResponse>();
        }

        public async Task DeleteUpcomingDate(Guid upcomingDateId, CancellationToken ct = default)
        {
            var upcomingDate = await GetUpcomingDateOrThrow(upcomingDateId, ct);
            _upcomingDateRepository.DeleteUpcomingDate(upcomingDate);
            await _upcomingDateRepository.SaveChanges(ct);
            _logger.LogInformation("Upcoming date {UpcomingDateId} deleted successfully", upcomingDateId);
        }

        private async Task<UpcomingDate> GetUpcomingDateOrThrow(Guid upcomingDateId, CancellationToken ct) =>
            await _upcomingDateRepository.GetUpcomingDateById(upcomingDateId, _currentUser.UserId, ct) ??
                throw new NotFoundException(
                    CustomExceptionCodes.UPCOMINGDATE_NOT_FOUND,
                    $"UpcomingDate with id {upcomingDateId} doesn't exist",
                    new Dictionary<string, object> { { "UpcomingDateId", upcomingDateId } });
    }
}
