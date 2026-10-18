using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;
using Ponce.PersonalDashboard.Core.Infrastructure.Contexts;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Repositories
{
    public class GoalRepository(AppDbContext context, ILogger<GoalRepository> logger)
        : Repository<AppDbContext>(context), IGoalRepository
    {
        private readonly ILogger<GoalRepository> _logger = logger;

        public IQueryable<DailyGoal> QueryDailyGoals(Guid userId)
        {
            _logger.LogDebug("Querying daily goals");
            return _context.DailyGoals.Where(x => x.UserId == userId);
        }

        public async Task<DailyGoal?> GetDailyGoalById(Guid dailyGoalId, Guid userId, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching daily goal with id {DailyGoalId}", dailyGoalId);
            return await _context.DailyGoals
                .Include(x => x.WeeklyGoal)
                .FirstOrDefaultAsync(x => x.Id == dailyGoalId && x.UserId == userId, ct);
        }

        public async Task SaveDailyGoal(DailyGoal dailyGoal, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving daily goal with id {DailyGoalId}", dailyGoal.Id);
            await _context.DailyGoals.AddAsync(dailyGoal, ct);
        }

        public void DeleteDailyGoal(DailyGoal dailyGoal)
        {
            _logger.LogDebug("Deleting daily goal with id {DailyGoalId}", dailyGoal.Id);
            _context.DailyGoals.Remove(dailyGoal);
        }

        public async Task<GoalCheckIn?> GetCheckIn(Guid dailyGoalId, DateOnly date, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching check-in of daily goal {DailyGoalId} for {Date}", dailyGoalId, date);
            return await _context.GoalCheckIns.FirstOrDefaultAsync(x => x.DailyGoalId == dailyGoalId && x.Date == date, ct);
        }

        public async Task<List<DateOnly>> GetCheckInDates(Guid dailyGoalId, DateOnly from, DateOnly to, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching check-in dates of daily goal {DailyGoalId} between {From} and {To}", dailyGoalId, from, to);
            return await _context.GoalCheckIns
                .Where(x => x.DailyGoalId == dailyGoalId && x.Date >= from && x.Date <= to)
                .OrderBy(x => x.Date)
                .Select(x => x.Date)
                .ToListAsync(ct);
        }

        public async Task SaveCheckIn(GoalCheckIn checkIn, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving check-in of daily goal {DailyGoalId} for {Date}", checkIn.DailyGoalId, checkIn.Date);
            await _context.GoalCheckIns.AddAsync(checkIn, ct);
        }

        public void DeleteCheckIn(GoalCheckIn checkIn)
        {
            _logger.LogDebug("Deleting check-in of daily goal {DailyGoalId} for {Date}", checkIn.DailyGoalId, checkIn.Date);
            _context.GoalCheckIns.Remove(checkIn);
        }
    }
}
