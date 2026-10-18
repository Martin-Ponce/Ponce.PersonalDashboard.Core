using Mapster;
using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Application.Contracts;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.JobApplications;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.JobApplications;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.JobApplication;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.Application.Services
{
    /// <summary>Job applications plus a monthly summary.</summary>
    public class JobApplicationService(IJobApplicationRepository jobApplicationRepository, ICurrentUser currentUser, ILogger<JobApplicationService> logger)
    {
        private readonly IJobApplicationRepository _jobApplicationRepository = jobApplicationRepository;
        private readonly ICurrentUser _currentUser = currentUser;
        private readonly ILogger<JobApplicationService> _logger = logger;

        /// <summary>Groups remote jobs and jobs without a city.</summary>
        public const string REMOTE_CITY = "Remote";

        public IQueryable<GetJobApplicationsResponse> GetJobApplications(short? year = null, short? month = null)
        {
            var range = MonthRange(year, month);
            return _jobApplicationRepository
                .QueryJobApplications(_currentUser.UserId, range?.From, range?.To)
                .ProjectToType<GetJobApplicationsResponse>();
        }

        /// <summary>The month's total, count per status and cities.</summary>
        public async Task<JobApplicationsSummaryResponse> GetMonthlySummary(short year, short month, CancellationToken ct = default)
        {
            var (from, to) = MonthRange(year, month)!.Value;
            var applications = await _jobApplicationRepository.GetStatusesAndCities(_currentUser.UserId, from, to, ct);
            return new JobApplicationsSummaryResponse
            {
                Year = year,
                Month = month,
                Total = applications.Count,
                StatusCounts = Enum.GetValues<JobApplicationStatus>()
                    .ToDictionary(status => status, status => applications.Count(x => x.Status == status)),
                Cities = [.. applications
                    .Select(x => x.WorkMode == WorkMode.Remote || x.City is null ? REMOTE_CITY : x.City)
                    .Distinct()
                    .Order(StringComparer.CurrentCultureIgnoreCase)]
            };
        }

        public async Task<GetJobApplicationResponse> GetJobApplication(Guid jobApplicationId, CancellationToken ct = default)
        {
            var jobApplication = await GetJobApplicationOrThrow(jobApplicationId, ct);
            return jobApplication.Adapt<GetJobApplicationResponse>();
        }

        public async Task<SaveJobApplicationResponse> SaveJobApplication(SaveJobApplicationRequest request, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving job application for {CompanyName}", request.CompanyName);
            var jobApplication = new JobApplication(
                _currentUser.UserId,
                request.CompanyName,
                request.DateApplied,
                request.Salary,
                request.JobTitle,
                request.City,
                request.WorkMode,
                request.Status);
            await _jobApplicationRepository.SaveJobApplication(jobApplication, ct);
            await _jobApplicationRepository.SaveChanges(ct);
            _logger.LogInformation("Job application {JobApplicationId} saved successfully", jobApplication.Id);
            return jobApplication.Adapt<SaveJobApplicationResponse>();
        }

        public async Task<SaveJobApplicationResponse> UpdateJobApplication(Guid jobApplicationId, SaveJobApplicationRequest request, CancellationToken ct = default)
        {
            var jobApplication = await GetJobApplicationOrThrow(jobApplicationId, ct);
            jobApplication.Update(
                request.CompanyName,
                request.DateApplied,
                request.Salary,
                request.JobTitle,
                request.City,
                request.WorkMode,
                request.Status);
            await _jobApplicationRepository.SaveChanges(ct);
            _logger.LogInformation("Job application {JobApplicationId} updated successfully", jobApplication.Id);
            return jobApplication.Adapt<SaveJobApplicationResponse>();
        }

        public async Task DeleteJobApplication(Guid jobApplicationId, CancellationToken ct = default)
        {
            var jobApplication = await GetJobApplicationOrThrow(jobApplicationId, ct);
            _jobApplicationRepository.DeleteJobApplication(jobApplication);
            await _jobApplicationRepository.SaveChanges(ct);
            _logger.LogInformation("Job application {JobApplicationId} deleted successfully", jobApplicationId);
        }

        /// <summary>First day of the month and of the next one, null when neither year nor month is given.</summary>
        private static (DateOnly From, DateOnly To)? MonthRange(short? year, short? month)
        {
            if (year is null && month is null)
                return null;
            if (year is null || month is null)
                throw ValidationException.NullOrEmpty("Arguments 'year' and 'month' must be sent together");
            var from = new DateOnly(year.Value.ValidateYear(), month.Value.ValidateMonth(), 1);
            return (from, from.AddMonths(1));
        }

        private async Task<JobApplication> GetJobApplicationOrThrow(Guid jobApplicationId, CancellationToken ct) =>
            await _jobApplicationRepository.GetJobApplicationById(jobApplicationId, _currentUser.UserId, ct) ??
                throw new NotFoundException(
                    CustomExceptionCodes.JOBAPPLICATION_NOT_FOUND,
                    $"JobApplication with id {jobApplicationId} doesn't exist",
                    new Dictionary<string, object> { { "JobApplicationId", jobApplicationId } });
    }
}
