using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.UpcomingDate;
using Ponce.PersonalDashboard.Core.Domain.Helpers.UpcomingDate;
using Ponce.PersonalDashboard.Core.Infrastructure.Contexts;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Repositories
{
    public class UpcomingDateRepository(AppDbContext context, ILogger<UpcomingDateRepository> logger)
        : Repository<AppDbContext>(context), IUpcomingDateRepository
    {
        private readonly ILogger<UpcomingDateRepository> _logger = logger;

        public IQueryable<UpcomingDate> QueryUpcomingDates(Guid userId)
        {
            _logger.LogDebug("Querying upcoming dates");
            return _context.UpcomingDates.Where(x => x.UserId == userId);
        }

        public async Task<List<UpcomingDate>> GetNextScheduled(Guid userId, DateOnly from, int limit, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching the next {Limit} scheduled dates from {From}", limit, from);
            return await _context.UpcomingDates
                .Where(x => x.UserId == userId && x.Status == UpcomingDateStatus.Scheduled && x.Date >= from)
                .OrderBy(x => x.Date).ThenBy(x => x.Time == null).ThenBy(x => x.Time).ThenBy(x => x.Title)
                .Take(limit)
                .ToListAsync(ct);
        }

        public async Task<UpcomingDate?> GetUpcomingDateById(Guid upcomingDateId, Guid userId, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching upcoming date with id {UpcomingDateId}", upcomingDateId);
            return await _context.UpcomingDates.FirstOrDefaultAsync(x => x.Id == upcomingDateId && x.UserId == userId, ct);
        }

        public async Task SaveUpcomingDate(UpcomingDate upcomingDate, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving upcoming date with id {UpcomingDateId}", upcomingDate.Id);
            await _context.UpcomingDates.AddAsync(upcomingDate, ct);
        }

        public void DeleteUpcomingDate(UpcomingDate upcomingDate)
        {
            _logger.LogDebug("Deleting upcoming date with id {UpcomingDateId}", upcomingDate.Id);
            _context.UpcomingDates.Remove(upcomingDate);
        }
    }
}
