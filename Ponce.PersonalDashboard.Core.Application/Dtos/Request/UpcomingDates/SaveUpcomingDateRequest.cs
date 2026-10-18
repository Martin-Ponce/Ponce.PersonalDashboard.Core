using Ponce.PersonalDashboard.Core.Domain.Helpers.UpcomingDate;

namespace Ponce.PersonalDashboard.Core.Application.Dtos.Request.UpcomingDates
{
    /// <summary>Used to create and update an upcoming date.</summary>
    public class SaveUpcomingDateRequest
    {
        public DateOnly Date { get; set; }
        /// <summary>Null for an all-day event.</summary>
        public TimeOnly? Time { get; set; }
        public required string Title { get; set; }
        public string? Description { get; set; }
        public UpcomingDateStatus Status { get; set; } = UpcomingDateStatus.Scheduled;
    }
}
