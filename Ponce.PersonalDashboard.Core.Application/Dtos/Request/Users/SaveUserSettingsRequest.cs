namespace Ponce.PersonalDashboard.Core.Application.Dtos.Request.Users
{
    /// <summary>The frontend's whole settings object. Everything's required so a missing field gets rejected instead of defaulted.</summary>
    public class SaveUserSettingsRequest
    {
        public required string Theme { get; set; }
        public required string Language { get; set; }
        public required string DateFormat { get; set; }
        public required string Currency { get; set; }
        public required bool ShowDecimals { get; set; }
        public required bool AutoTimeZone { get; set; }
        public required string TimeZone { get; set; }
        public required string ProfilePicture { get; set; }
        public required OverviewWidgetsRequest OverviewWidgets { get; set; }
    }

    public class OverviewWidgetsRequest
    {
        public required bool Summary { get; set; }
        public required bool Spending { get; set; }
        public required bool Dates { get; set; }
        public required bool Applications { get; set; }
    }
}
