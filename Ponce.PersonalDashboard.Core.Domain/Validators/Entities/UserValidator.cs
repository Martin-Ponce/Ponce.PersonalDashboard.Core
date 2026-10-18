using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers.Users;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants.Values;

namespace Ponce.PersonalDashboard.Core.Domain.Validators.Entities
{
    public static class UserValidator
    {
        public static Guid ValidateUserId(this Guid userId) =>
            userId != Guid.Empty ? userId : throw ValidationException.NullOrEmpty($"Argument '{nameof(userId)}' cannot be empty");

        public static string ValidateUserName(this string? userName) =>
            userName?.Trim().ValidateLength(
                UserLimits.MAX_LENGTH_USER_NAME,
                StringValidator.ExceptionWhenOutOfRangeArguments(nameof(userName), UserLimits.MAX_LENGTH_USER_NAME),
                UserLimits.MIN_LENGTH_USER_NAME)
            ?? throw StringValidator.ExceptionWhenNullArgument(nameof(userName));

        public static string? ValidateDisplayName(this string? displayName) =>
            displayName.ValidateOptionalText(nameof(displayName), UserLimits.MAX_LENGTH_DISPLAY_NAME);

        public static string ValidateTheme(this string? theme) =>
            theme.ValidateOption(nameof(theme), UserSettingsOptions.THEMES);

        public static string ValidateLanguage(this string? language) =>
            language.ValidateOption(nameof(language), UserSettingsOptions.LANGUAGES);

        public static string ValidateDateFormat(this string? dateFormat) =>
            dateFormat.ValidateOption(nameof(dateFormat), UserSettingsOptions.DATE_FORMATS);

        public static string ValidateCurrency(this string? currency) =>
            currency.ValidateOption(nameof(currency), UserSettingsOptions.CURRENCIES);

        public static string ValidateProfilePicture(this string? profilePicture) =>
            profilePicture.ValidateOption(nameof(profilePicture), UserSettingsOptions.PROFILE_PICTURES);

        /// <summary>Any IANA zone, checked by format so it doesn't depend on the server's time zone database.</summary>
        public static string ValidateTimeZone(this string? timeZone) =>
            timeZone?.Trim()
                .ValidateLength(
                    UserLimits.MAX_LENGTH_TIME_ZONE,
                    StringValidator.ExceptionWhenOutOfRangeArguments(nameof(timeZone), UserLimits.MAX_LENGTH_TIME_ZONE),
                    1)
                .ValidateRegex(
                    UserLimits.TIME_ZONE_PATTERN,
                    ValidationException.InvalidFormat($"Argument '{nameof(timeZone)}' must be an IANA time zone, e.g. 'Europe/Madrid'"))
            ?? throw StringValidator.ExceptionWhenNullArgument(nameof(timeZone));

        // Case-sensitive match against the frontend's options.
        private static string ValidateOption(this string? value, string argumentName, string[] options)
        {
            if (string.IsNullOrEmpty(value))
                throw StringValidator.ExceptionWhenNullArgument(argumentName);
            if (!options.Contains(value))
                throw ValidationException.NotSupported($"Argument '{argumentName}' must be one of: {string.Join(", ", options)}");
            return value;
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
