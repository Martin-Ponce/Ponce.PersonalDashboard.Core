namespace Ponce.PersonalDashboard.Core.Domain.Helpers.Users
{
    /// <summary>Options and defaults for the settings page. Keep these in sync with the frontend's constants.</summary>
    public static class UserSettingsOptions
    {
        public static readonly string[] THEMES = ["sky", "night"];
        public static readonly string[] LANGUAGES = ["en", "es", "sv"];
        public static readonly string[] DATE_FORMATS = ["DD/MM/YYYY", "MM/DD/YYYY", "YYYY-MM-DD"];
        /// <summary>ISO 4217 codes.</summary>
        public static readonly string[] CURRENCIES = ["EUR", "USD", "GBP", "MXN", "JPY", "CHF"];
        /// <summary>"initials" shows the user's initials, the rest are avatar images.</summary>
        public static readonly string[] PROFILE_PICTURES = ["initials", "bubbles", "leaf", "sun", "waves", "mountains", "moon", "flower", "planet"];

        public const string DEFAULT_THEME = "sky";
        public const string DEFAULT_LANGUAGE = "en";
        public const string DEFAULT_DATE_FORMAT = "DD/MM/YYYY";
        public const string DEFAULT_CURRENCY = "EUR";
        public const string DEFAULT_PROFILE_PICTURE = "initials";
        public const bool DEFAULT_SHOW_DECIMALS = true;
        public const bool DEFAULT_AUTO_TIME_ZONE = true;
        /// <summary>First zone in the frontend's TIME_ZONES list.</summary>
        public const string DEFAULT_TIME_ZONE = "Europe/Madrid";
        public const bool DEFAULT_WIDGET_VISIBLE = true;
    }
}
