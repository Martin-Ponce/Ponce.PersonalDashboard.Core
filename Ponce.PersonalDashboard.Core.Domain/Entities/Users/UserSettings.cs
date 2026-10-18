using Ponce.PersonalDashboard.Core.Domain.Entities.Base;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;
using static Ponce.PersonalDashboard.Core.Domain.Helpers.Users.UserSettingsOptions;

namespace Ponce.PersonalDashboard.Core.Domain.Entities.Users
{
    /// <summary>A user's settings (theme, formats, time zone, widgets), one per user. Users who never saved any get CreateDefault.</summary>
    public class UserSettings : Entity
    {
        public Guid UserId { get; private set; }
        public string Theme { get; private set; } = default!;
        public string Language { get; private set; } = default!;
        public string DateFormat { get; private set; } = default!;
        /// <summary>ISO 4217 code.</summary>
        public string Currency { get; private set; } = default!;
        public bool ShowDecimals { get; private set; }
        /// <summary>When true, the device's time zone is used and TimeZone is ignored.</summary>
        public bool AutoTimeZone { get; private set; }
        /// <summary>IANA time zone, e.g. "Europe/Madrid".</summary>
        public string TimeZone { get; private set; } = default!;
        /// <summary>One of <see cref="UserSettingsOptions.PROFILE_PICTURES"/>.</summary>
        public string ProfilePicture { get; private set; } = default!;
        public bool ShowSummaryWidget { get; private set; }
        public bool ShowSpendingWidget { get; private set; }
        public bool ShowDatesWidget { get; private set; }
        public bool ShowApplicationsWidget { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        private UserSettings() { } // Required by EF Core

        public UserSettings(
            Guid userId, string theme, string language, string dateFormat, string currency, bool showDecimals,
            bool autoTimeZone, string timeZone, string profilePicture, bool showSummaryWidget, bool showSpendingWidget,
            bool showDatesWidget, bool showApplicationsWidget)
        {
            UserId = userId.ValidateUserId();
            Update(theme, language, dateFormat, currency, showDecimals, autoTimeZone, timeZone, profilePicture,
                showSummaryWidget, showSpendingWidget, showDatesWidget, showApplicationsWidget);
        }

        public static UserSettings CreateDefault(Guid userId) => new(
            userId, DEFAULT_THEME, DEFAULT_LANGUAGE, DEFAULT_DATE_FORMAT, DEFAULT_CURRENCY, DEFAULT_SHOW_DECIMALS,
            DEFAULT_AUTO_TIME_ZONE, DEFAULT_TIME_ZONE, DEFAULT_PROFILE_PICTURE, DEFAULT_WIDGET_VISIBLE, DEFAULT_WIDGET_VISIBLE,
            DEFAULT_WIDGET_VISIBLE, DEFAULT_WIDGET_VISIBLE);

        public void Update(
            string theme, string language, string dateFormat, string currency, bool showDecimals,
            bool autoTimeZone, string timeZone, string profilePicture, bool showSummaryWidget, bool showSpendingWidget,
            bool showDatesWidget, bool showApplicationsWidget)
        {
            Theme = theme.ValidateTheme();
            Language = language.ValidateLanguage();
            DateFormat = dateFormat.ValidateDateFormat();
            Currency = currency.ValidateCurrency();
            ShowDecimals = showDecimals;
            AutoTimeZone = autoTimeZone;
            TimeZone = timeZone.ValidateTimeZone();
            ProfilePicture = profilePicture.ValidateProfilePicture();
            ShowSummaryWidget = showSummaryWidget;
            ShowSpendingWidget = showSpendingWidget;
            ShowDatesWidget = showDatesWidget;
            ShowApplicationsWidget = showApplicationsWidget;
            UpdatedAtUtc = DateTime.UtcNow;
        }
    }
}
