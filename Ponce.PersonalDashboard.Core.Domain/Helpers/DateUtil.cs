namespace Ponce.PersonalDashboard.Core.Domain.Helpers
{
    public static class DateUtil
    {
        /// <summary>First day of the week this date falls in.</summary>
        public static DateOnly StartOfWeek(this DateOnly date, DayOfWeek weekStart = DomainConstants.Values.GoalLimits.WEEK_START)
        {
            var daysSinceWeekStart = (7 + (date.DayOfWeek - weekStart)) % 7;
            return date.AddDays(-daysSinceWeekStart);
        }

        public static DateOnly EndOfWeek(this DateOnly date, DayOfWeek weekStart = DomainConstants.Values.GoalLimits.WEEK_START) =>
            date.StartOfWeek(weekStart).AddDays(6);

        public static DateOnly TodayUtc() => DateOnly.FromDateTime(DateTime.UtcNow);
    }
}
