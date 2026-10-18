using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers.Goals;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;

namespace Ponce.PersonalDashboard.Core.Domain.Validators.Entities
{
    public static class GoalValidator
    {
        public static string ValidateGoalTitle(this string? title) =>
            title?.Trim().ValidateLength(
                GoalLimits.MAX_LENGTH_TITLE,
                StringValidator.ExceptionWhenOutOfRangeArguments(nameof(title), GoalLimits.MAX_LENGTH_TITLE),
                GoalLimits.MIN_LENGTH_TITLE)
            ?? throw StringValidator.ExceptionWhenNullArgument(nameof(title));

        public static decimal ValidateGoalAmount(this decimal amount)
        {
            if (amount <= 0 || amount > GoalLimits.MAX_AMOUNT)
                throw ValidationException.OutOfRange($"Argument '{nameof(amount)}' must be greater than 0 and at most {GoalLimits.MAX_AMOUNT}");
            return amount;
        }

        public static GoalUnit ValidateGoalUnit(this GoalUnit unit) =>
            Enum.IsDefined(unit) ? unit : throw ValidationException.NotSupported($"Goal unit '{unit}' is not supported");

        /// <summary>At least one valid day. Drops duplicates and sorts Monday first.</summary>
        public static DayOfWeek[] ValidatePlannedDays(this IEnumerable<DayOfWeek>? plannedDays)
        {
            var days = plannedDays?.Distinct().ToArray() ?? [];
            if (days.Length == 0)
                throw ValidationException.NullOrEmpty($"Argument '{nameof(plannedDays)}' must have at least one day");
            var invalid = days.Where(day => !Enum.IsDefined(day)).ToArray();
            if (invalid.Length > 0)
                throw ValidationException.NotSupported($"Day '{invalid[0]}' is not supported");
            return [.. days.OrderBy(day => (7 + (day - GoalLimits.WEEK_START)) % 7)];
        }

        public static string? ValidateWeeklyGoalTitle(this string? title) =>
            title.ValidateOptionalText(nameof(title), GoalLimits.MAX_LENGTH_TITLE);

        public static int ValidateWeeklyGoalTargetCount(this int targetCount)
        {
            if (targetCount < GoalLimits.MIN_WEEKLY_TARGET || targetCount > GoalLimits.MAX_WEEKLY_TARGET)
                throw ValidationException.OutOfRange($"Argument '{nameof(targetCount)}' must be between {GoalLimits.MIN_WEEKLY_TARGET} and {GoalLimits.MAX_WEEKLY_TARGET} (a daily goal can be checked at most once per day)");
            return targetCount;
        }

        /// <summary>No check-ins before the goal existed or after today, with a day of slack each side for time zones.</summary>
        public static DateOnly ValidateCheckInDate(this DateOnly date, DateTime goalCreatedAtUtc)
        {
            var minDate = DateOnly.FromDateTime(goalCreatedAtUtc).AddDays(-1);
            var maxDate = DateOnly.FromDateTime(DateTime.UtcNow).AddDays(1);
            if (date < minDate || date > maxDate)
                throw ValidationException.OutOfRange($"Argument '{nameof(date)}' must be between {minDate:yyyy-MM-dd} and {maxDate:yyyy-MM-dd}");
            return date;
        }

        private static string? ValidateOptionalText(this string? value, string argumentName, int maxLength)
        {
            if (string.IsNullOrWhiteSpace(value))
                return null;
            return value.Trim().ValidateLength(
                maxLength,
                StringValidator.ExceptionWhenOutOfRangeArguments(argumentName, maxLength));
        }
    }
}
