namespace Ponce.PersonalDashboard.Core.Application.Dtos.Response.Users
{
    /// <summary>Same shape as the frontend's settings, so it can be used as-is.</summary>
    public class UserSettingsResponse
    {
        public required string Theme { get; set; }
        public required string Language { get; set; }
        public required string DateFormat { get; set; }
        public required string Currency { get; set; }
        public bool ShowDecimals { get; set; }
        public bool AutoTimeZone { get; set; }
        public required string TimeZone { get; set; }
        public required string ProfilePicture { get; set; }
        public required OverviewWidgetsResponse OverviewWidgets { get; set; }
    }

    public class OverviewWidgetsResponse
    {
        public bool Summary { get; set; }
        public bool Spending { get; set; }
        public bool Dates { get; set; }
        public bool Applications { get; set; }
    }
}
