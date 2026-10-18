using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.JobApplication;
using Ponce.PersonalDashboard.Core.Domain.Helpers.JobApplication;
using Ponce.PersonalDashboard.Core.Infrastructure.Contexts;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Repositories
{
    public class JobApplicationRepository(AppDbContext context, ILogger<JobApplicationRepository> logger)
        : Repository<AppDbContext>(context), IJobApplicationRepository
    {
        private readonly ILogger<JobApplicationRepository> _logger = logger;

        public IQueryable<JobApplication> QueryJobApplications(Guid userId, DateOnly? from = null, DateOnly? to = null)
        {
            _logger.LogDebug("Querying job applications");
            var query = _context.JobApplications.Where(x => x.UserId == userId);
            // Date range instead of year/month parts so the (UserId, DateApplied) index gets used.
            if (from is not null)
                query = query.Where(x => x.DateApplied >= from);
            if (to is not null)
                query = query.Where(x => x.DateApplied < to);
            return query;
        }

        public async Task<Dictionary<JobApplicationStatus, int>> CountByStatus(Guid userId, CancellationToken ct = default)
        {
            _logger.LogDebug("Counting job applications per status");
            return await _context.JobApplications
                .Where(x => x.UserId == userId)
                .GroupBy(x => x.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Status, x => x.Count, ct);
        }

        public async Task<int> CountAppliedSince(Guid userId, DateOnly from, CancellationToken ct = default)
        {
            _logger.LogDebug("Counting job applications sent since {From}", from);
            return await _context.JobApplications.CountAsync(x => x.UserId == userId && x.DateApplied >= from, ct);
        }

        public async Task<List<JobApplication>> GetMostRecent(Guid userId, int limit, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching the {Limit} most recent job applications", limit);
            return await _context.JobApplications
                .Where(x => x.UserId == userId)
                .OrderByDescending(x => x.DateApplied).ThenByDescending(x => x.UpdatedAtUtc)
                .Take(limit)
                .ToListAsync(ct);
        }

        public async Task<List<(JobApplicationStatus Status, string? City, WorkMode WorkMode)>> GetStatusesAndCities(Guid userId, DateOnly from, DateOnly to, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching statuses and cities of job applications from {From} to {To}", from, to);
            var rows = await QueryJobApplications(userId, from, to)
                .Select(x => new { x.Status, x.City, x.WorkMode })
                .ToListAsync(ct);
            return [.. rows.Select(x => (x.Status, x.City, x.WorkMode))];
        }

        public async Task<JobApplication?> GetJobApplicationById(Guid jobApplicationId, Guid userId, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching job application with id {JobApplicationId}", jobApplicationId);
            return await _context.JobApplications.FirstOrDefaultAsync(x => x.Id == jobApplicationId && x.UserId == userId, ct);
        }

        public async Task SaveJobApplication(JobApplication jobApplication, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving job application with id {JobApplicationId}", jobApplication.Id);
            await _context.JobApplications.AddAsync(jobApplication, ct);
        }

        public void DeleteJobApplication(JobApplication jobApplication)
        {
            _logger.LogDebug("Deleting job application with id {JobApplicationId}", jobApplication.Id);
            _context.JobApplications.Remove(jobApplication);
        }
    }
}
