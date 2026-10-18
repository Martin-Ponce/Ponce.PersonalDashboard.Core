using Ponce.PersonalDashboard.Core.Domain.Entities.JobApplication;
using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;

namespace Ponce.PersonalDashboard.Core.Domain.Contracts
{
    /// <summary>Data access for job applications.</summary>
    public interface IJobApplicationRepository : IRepository
    {
        /// <summary>Optional date range: from is inclusive, to is exclusive.</summary>
        IQueryable<JobApplication> QueryJobApplications(Guid userId, DateOnly? from = null, DateOnly? to = null);
        /// <summary>Count per status. Statuses with none are skipped.</summary>
        Task<Dictionary<JobApplicationStatus, int>> CountByStatus(Guid userId, CancellationToken ct = default);
        Task<int> CountAppliedSince(Guid userId, DateOnly from, CancellationToken ct = default);
        Task<List<JobApplication>> GetMostRecent(Guid userId, int limit, CancellationToken ct = default);
        Task<List<(JobApplicationStatus Status, string? City, WorkMode WorkMode)>> GetStatusesAndCities(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default);
        Task<JobApplication?> GetJobApplicationById(Guid jobApplicationId, Guid userId, CancellationToken ct = default);
        Task SaveJobApplication(JobApplication jobApplication, CancellationToken ct = default);
        void DeleteJobApplication(JobApplication jobApplication);
    }
}
