using Ponce.PersonalDashboard.Core.Domain.Entities.Goals;

namespace Ponce.PersonalDashboard.Core.Domain.Contracts
{
    /// <summary>Data access for daily goals, weekly goals and check-ins.</summary>
    public interface IGoalRepository : IRepository
    {
        IQueryable<DailyGoal> QueryDailyGoals(Guid userId);
        /// <summary>Also loads the weekly goal, but not the check-ins.</summary>
        Task<DailyGoal?> GetDailyGoalById(Guid dailyGoalId, Guid userId, CancellationToken ct = default);
        Task SaveDailyGoal(DailyGoal dailyGoal, CancellationToken ct = default);
        /// <summary>Also deletes its weekly goal and check-ins.</summary>
        void DeleteDailyGoal(DailyGoal dailyGoal);

        Task<GoalCheckIn?> GetCheckIn(Guid dailyGoalId, DateOnly date, CancellationToken ct = default);
        /// <summary>Checked days in [from, to], oldest first.</summary>
        Task<List<DateOnly>> GetCheckInDates(Guid dailyGoalId, DateOnly from, DateOnly to, CancellationToken ct = default);
        Task SaveCheckIn(GoalCheckIn checkIn, CancellationToken ct = default);
        void DeleteCheckIn(GoalCheckIn checkIn);
    }
}
