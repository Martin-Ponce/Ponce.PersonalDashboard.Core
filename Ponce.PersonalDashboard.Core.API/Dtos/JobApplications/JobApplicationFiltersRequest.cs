using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;

namespace Ponce.PersonalDashboard.Core.API.Dtos.JobApplications
{
    /// <summary>Extra filters for the job applications list. Empty means any, repeat a param for several values.</summary>
    public class JobApplicationFiltersRequest
    {
        [FromQuery]
        public short? Year { get; set; }
        /// <summary>1-12, used together with <see cref="Year"/>.</summary>
        [FromQuery]
        public short? Month { get; set; }
        [FromQuery]
        public JobApplicationStatus[]? Statuses { get; set; }
        [FromQuery]
        public WorkMode[]? WorkModes { get; set; }
        /// <summary><see cref="Application.Services.JobApplicationService.REMOTE_CITY"/> matches remote jobs and jobs without a city.</summary>
        [FromQuery]
        public string[]? Cities { get; set; }
    }
}
