using Ponce.PersonalDashboard.Core.Domain.Helpers.Goals;

namespace Ponce.PersonalDashboard.Core.Application.Dtos.Request.Goals
{
    /// <summary>Used to create and update a daily goal. On update a null WeeklyGoal removes the weekly goal.</summary>
    public class SaveDailyGoalRequest
    {
        public required string Title { get; set; }
        /// <summary>What one completion is worth, e.g. 30 minutes.</summary>
        public decimal Amount { get; set; }
        public GoalUnit Unit { get; set; } = GoalUnit.Times;
        /// <summary>At least one.</summary>
        public required DayOfWeek[] PlannedDays { get; set; }
        public SaveWeeklyGoalRequest? WeeklyGoal { get; set; }
    }

    public class SaveWeeklyGoalRequest
    {
        /// <summary>Falls back to the daily goal's title when null.</summary>
        public string? Title { get; set; }
        /// <summary>Days per week it has to be checked (1-7).</summary>
        public int TargetCount { get; set; }
    }
}
