using Ponce.PersonalDashboard.Core.Domain.Entities.Base;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;

namespace Ponce.PersonalDashboard.Core.Domain.Entities.Goals
{
    /// <summary>A daily goal marked as done on a given day (one per goal per day).</summary>
    public class GoalCheckIn : Entity
    {
        public Guid DailyGoalId { get; private set; }
        public DateOnly Date { get; private set; }
        public DateTime CheckedAtUtc { get; private set; }

        private GoalCheckIn() { } // Required by EF Core

        public GoalCheckIn(DailyGoal dailyGoal, DateOnly date)
        {
            DailyGoalId = dailyGoal.Id;
            Date = date.ValidateCheckInDate(dailyGoal.CreatedAtUtc);
            CheckedAtUtc = DateTime.UtcNow;
        }
    }
}
