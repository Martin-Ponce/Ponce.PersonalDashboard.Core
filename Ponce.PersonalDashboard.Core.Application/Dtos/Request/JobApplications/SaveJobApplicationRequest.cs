using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;

namespace Ponce.PersonalDashboard.Core.Application.Dtos.Request.JobApplications
{
    /// <summary>Used to create and update a job application.</summary>
    public class SaveJobApplicationRequest
    {
        public required string CompanyName { get; set; }
        public DateOnly DateApplied { get; set; }
        public decimal? Salary { get; set; }
        public required string JobTitle { get; set; }
        /// <summary>Ignored for remote jobs.</summary>
        public string? City { get; set; }
        public WorkMode WorkMode { get; set; } = WorkMode.OnSite;
        public JobApplicationStatus Status { get; set; } = JobApplicationStatus.Applied;
    }
}
