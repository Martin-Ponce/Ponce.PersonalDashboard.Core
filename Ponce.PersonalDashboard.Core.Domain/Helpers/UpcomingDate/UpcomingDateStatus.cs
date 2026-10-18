namespace Ponce.PersonalDashboard.Core.Domain.Helpers.UpcomingDate
{
    /// <summary>Whether an upcoming date is still pending or done. Stored as the char's integer code.</summary>
    public enum UpcomingDateStatus
    {
        Scheduled = 'S',
        Completed = 'C'
    }
}
