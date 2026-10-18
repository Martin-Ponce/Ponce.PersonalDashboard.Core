using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;

namespace Ponce.PersonalDashboard.Core.Application.Dtos.Response.JobApplications
{
    public class GetJobApplicationsResponse
    {
        public Guid Id { get; set; }
        public required string CompanyName { get; set; }
        public DateOnly DateApplied { get; set; }
        public decimal? Salary { get; set; }
        public required string JobTitle { get; set; }
        /// <summary>Null for remote jobs or when unknown.</summary>
        public string? City { get; set; }
        public WorkMode WorkMode { get; set; }
        public JobApplicationStatus Status { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
    }
}
