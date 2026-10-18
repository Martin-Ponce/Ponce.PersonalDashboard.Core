using Ponce.PersonalDashboard.Core.Domain.Helpers.Goals;

namespace Ponce.PersonalDashboard.Core.Application.Dtos.Response.Goals
{
    /// <summary>A daily goal, whether it's checked on Date and its weekly progress.</summary>
    public class GoalResponse
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public decimal Amount { get; set; }
        public GoalUnit Unit { get; set; }
        /// <summary>Monday first.</summary>
        public required DayOfWeek[] PlannedDays { get; set; }
        /// <summary>Can't be checked for days before this.</summary>
        public DateTime CreatedAtUtc { get; set; }
        public DateOnly Date { get; set; }
        public bool Checked { get; set; }
        /// <summary>Check-ins of Date's week (Monday to Sunday), oldest first.</summary>
        public required List<DateOnly> CheckedDates { get; set; }
        public WeeklyGoalProgressResponse? WeeklyGoal { get; set; }
    }

    public class WeeklyGoalProgressResponse
    {
        public Guid Id { get; set; }
        public required string Title { get; set; }
        public int TargetCount { get; set; }
        /// <summary>Days the daily goal was checked this week.</summary>
        public int Progress { get; set; }
        public bool Completed => Progress >= TargetCount;
        public DateOnly WeekStart { get; set; }
        public DateOnly WeekEnd { get; set; }
    }
}
