using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers.UpcomingDate;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;

namespace Ponce.PersonalDashboard.Core.Domain.Validators.Entities
{
    public static class UpcomingDateValidator
    {
        public static DateOnly ValidateUpcomingDate(this DateOnly date)
        {
            if (date < UpcomingDateLimits.MIN_DATE || date > UpcomingDateLimits.MAX_DATE)
                throw ValidationException.OutOfRange($"Argument '{nameof(date)}' must be between {UpcomingDateLimits.MIN_DATE:yyyy-MM-dd} and {UpcomingDateLimits.MAX_DATE:yyyy-MM-dd}");
            return date;
        }

        public static string ValidateUpcomingDateTitle(this string? title) =>
            title?.Trim().ValidateLength(
                UpcomingDateLimits.MAX_LENGTH_TITLE,
                StringValidator.ExceptionWhenOutOfRangeArguments(nameof(title), UpcomingDateLimits.MAX_LENGTH_TITLE),
                UpcomingDateLimits.MIN_LENGTH_TITLE)
            ?? throw StringValidator.ExceptionWhenNullArgument(nameof(title));

        public static string? ValidateUpcomingDateDescription(this string? description)
        {
            if (string.IsNullOrWhiteSpace(description))
                return null;
            return description.Trim().ValidateLength(
                UpcomingDateLimits.MAX_LENGTH_DESCRIPTION,
                StringValidator.ExceptionWhenOutOfRangeArguments(nameof(description), UpcomingDateLimits.MAX_LENGTH_DESCRIPTION));
        }

        public static UpcomingDateStatus ValidateStatus(this UpcomingDateStatus status) =>
            Enum.IsDefined(status) ? status : throw ValidationException.NotSupported($"Upcoming date status '{status}' is not supported");
    }
}
