using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;

namespace Ponce.PersonalDashboard.Core.Application.Dtos.Response.JobApplications
{
    /// <summary>Job application counts for a month, ignores the list filters.</summary>
    public class JobApplicationsSummaryResponse
    {
        public short Year { get; set; }
        /// <summary>1-12</summary>
        public short Month { get; set; }
        public int Total { get; set; }
        /// <summary>Every status is there, 0 when none.</summary>
        public required Dictionary<JobApplicationStatus, int> StatusCounts { get; set; }
        /// <summary>Sorted, remote jobs and jobs without a city go under <see cref="Services.JobApplicationService.REMOTE_CITY"/>.</summary>
        public required List<string> Cities { get; set; }
    }
}
