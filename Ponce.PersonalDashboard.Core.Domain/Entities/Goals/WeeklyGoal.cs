using Ponce.PersonalDashboard.Core.Domain.Entities.Base;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;

namespace Ponce.PersonalDashboard.Core.Domain.Entities.Goals
{
    /// <summary>Weekly target for a daily goal, like 3 check-ins a week. Only changed through its daily goal.</summary>
    public class WeeklyGoal : Entity
    {
        public Guid DailyGoalId { get; private set; }
        /// <summary>Null means the daily goal's title is used.</summary>
        public string? Title { get; private set; }
        /// <summary>Check-ins needed in a week.</summary>
        public int TargetCount { get; private set; }

        private WeeklyGoal() { } // Required by EF Core

        internal WeeklyGoal(Guid dailyGoalId, string? title, int targetCount)
        {
            DailyGoalId = dailyGoalId;
            Update(title, targetCount);
        }

        internal void Update(string? title, int targetCount)
        {
            Title = title.ValidateWeeklyGoalTitle();
            TargetCount = targetCount.ValidateWeeklyGoalTargetCount();
        }
    }
}
