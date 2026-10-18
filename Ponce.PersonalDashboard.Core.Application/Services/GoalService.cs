using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Application.Contracts;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.Goals;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Goals;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.Application.Services
{
    /// <summary>Daily goals and their weekly goal. Weekly progress is never stored, it's just the check-ins of the week.</summary>
    public class GoalService(IGoalRepository goalRepository, ICurrentUser currentUser, ILogger<GoalService> logger)
    {
        private readonly IGoalRepository _goalRepository = goalRepository;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly ILogger<GoalService> _logger = logger;

        /// <summary>Every daily goal with its status on that date and that week's progress.</summary>
        public IQueryable<GoalResponse> GetGoals(DateOnly date)
        {
            var weekStart = date.StartOfWeek();
            var weekEnd = date.EndOfWeek();
            return _goalRepository.QueryDailyGoals(_currentUser.UserId).Select(x => new GoalResponse
            {
                Id = x.Id,
                Title = x.Title,
                Amount = x.Amount,
                Unit = x.Unit,
                PlannedDays = x.PlannedDays,
                CreatedAtUtc = x.CreatedAtUtc,
                Date = date,
                Checked = x.CheckIns.Any(c => c.Date == date),
                CheckedDates = x.CheckIns
                    .Where(c => c.Date >= weekStart && c.Date <= weekEnd)
                    .OrderBy(c => c.Date)
                    .Select(c => c.Date)
                    .ToList(),
                WeeklyGoal = x.WeeklyGoal == null ? null : new WeeklyGoalProgressResponse
                {
                    Id = x.WeeklyGoal.Id,
                    Title = x.WeeklyGoal.Title ?? x.Title,
                    TargetCount = x.WeeklyGoal.TargetCount,
                    Progress = x.CheckIns.Count(c => c.Date >= weekStart && c.Date <= weekEnd),
                    WeekStart = weekStart,
                    WeekEnd = weekEnd
                }
            });
        }

        public async Task<GoalResponse> GetGoal(Guid dailyGoalId, DateOnly date, CancellationToken ct = default)
        {
            var dailyGoal = await GetDailyGoalOrThrow(dailyGoalId, ct);
            return await ToGoalResponse(dailyGoal, date, ct);
        }

        public async Task<GoalResponse> SaveDailyGoal(SaveDailyGoalRequest request, DateOnly date, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving daily goal {Title}", request.Title);
            var dailyGoal = new DailyGoal(_currentUser.UserId, request.Title, request.Amount, request.Unit, request.PlannedDays);
            if (request.WeeklyGoal is not null)
                dailyGoal.SetWeeklyGoal(request.WeeklyGoal.Title, request.WeeklyGoal.TargetCount);
            await _goalRepository.SaveDailyGoal(dailyGoal, ct);
            await _goalRepository.SaveChanges(ct);
            _logger.LogInformation("Daily goal {DailyGoalId} saved successfully", dailyGoal.Id);
            return await ToGoalResponse(dailyGoal, date, ct);
        }

        public async Task<GoalResponse> UpdateDailyGoal(Guid dailyGoalId, SaveDailyGoalRequest request, DateOnly date, CancellationToken ct = default)
        {
            var dailyGoal = await GetDailyGoalOrThrow(dailyGoalId, ct);
            dailyGoal.Update(request.Title, request.Amount, request.Unit, request.PlannedDays);
            if (request.WeeklyGoal is null)
                dailyGoal.RemoveWeeklyGoal();
            else
                dailyGoal.SetWeeklyGoal(request.WeeklyGoal.Title, request.WeeklyGoal.TargetCount);
            await _goalRepository.SaveChanges(ct);
            _logger.LogInformation("Daily goal {DailyGoalId} updated successfully", dailyGoal.Id);
            return await ToGoalResponse(dailyGoal, date, ct);
        }

        public async Task DeleteDailyGoal(Guid dailyGoalId, CancellationToken ct = default)
        {
            var dailyGoal = await GetDailyGoalOrThrow(dailyGoalId, ct);
            _goalRepository.DeleteDailyGoal(dailyGoal);
            await _goalRepository.SaveChanges(ct);
            _logger.LogInformation("Daily goal {DailyGoalId} deleted successfully", dailyGoalId);
        }

        public async Task<GoalResponse> CheckDailyGoal(Guid dailyGoalId, DateOnly date, CancellationToken ct = default)
        {
            var dailyGoal = await GetDailyGoalOrThrow(dailyGoalId, ct);
            if (await _goalRepository.GetCheckIn(dailyGoalId, date, ct) is null)
            {
                await _goalRepository.SaveCheckIn(new GoalCheckIn(dailyGoal, date), ct);
                await _goalRepository.SaveChanges(ct);
                _logger.LogInformation("Daily goal {DailyGoalId} checked for {Date}", dailyGoalId, date);
            }
            return await ToGoalResponse(dailyGoal, date, ct);
        }

        public async Task<GoalResponse> UncheckDailyGoal(Guid dailyGoalId, DateOnly date, CancellationToken ct = default)
        {
            var dailyGoal = await GetDailyGoalOrThrow(dailyGoalId, ct);
            var checkIn = await _goalRepository.GetCheckIn(dailyGoalId, date, ct);
            if (checkIn is not null)
            {
                _goalRepository.DeleteCheckIn(checkIn);
                await _goalRepository.SaveChanges(ct);
                _logger.LogInformation("Daily goal {DailyGoalId} unchecked for {Date}", dailyGoalId, date);
            }
            return await ToGoalResponse(dailyGoal, date, ct);
        }

        // Same shape as the GetGoals projection, but from the loaded goal.
        private async Task<GoalResponse> ToGoalResponse(DailyGoal dailyGoal, DateOnly date, CancellationToken ct)
        {
            var weekStart = date.StartOfWeek();
            var weekEnd = date.EndOfWeek();
            var checkedDates = await _goalRepository.GetCheckInDates(dailyGoal.Id, weekStart, weekEnd, ct);
            return new GoalResponse
            {
                Id = dailyGoal.Id,
                Title = dailyGoal.Title,
                Amount = dailyGoal.Amount,
                Unit = dailyGoal.Unit,
                PlannedDays = dailyGoal.PlannedDays,
                CreatedAtUtc = dailyGoal.CreatedAtUtc,
                Date = date,
                Checked = checkedDates.Contains(date),
                CheckedDates = checkedDates,
                WeeklyGoal = dailyGoal.WeeklyGoal is null ? null : new WeeklyGoalProgressResponse
                {
                    Id = dailyGoal.WeeklyGoal.Id,
                    Title = dailyGoal.WeeklyGoal.Title ?? dailyGoal.Title,
                    TargetCount = dailyGoal.WeeklyGoal.TargetCount,
                    Progress = checkedDates.Count,
                    WeekStart = weekStart,
                    WeekEnd = weekEnd
                }
            };
        }

        private async Task<DailyGoal> GetDailyGoalOrThrow(Guid dailyGoalId, CancellationToken ct) =>
            await _goalRepository.GetDailyGoalById(dailyGoalId, _currentUser.UserId, ct) ??
                throw new NotFoundException(
                    CustomExceptionCodes.DAILYGOAL_NOT_FOUND,
                    $"DailyGoal with id {dailyGoalId} doesn't exist",
                    new Dictionary<string, object> { { "DailyGoalId", dailyGoalId } });
    }
}
