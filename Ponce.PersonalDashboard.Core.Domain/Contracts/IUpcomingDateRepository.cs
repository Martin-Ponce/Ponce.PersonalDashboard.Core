using Ponce.PersonalDashboard.Core.Domain.Entities.UpcomingDate;

namespace Ponce.PersonalDashboard.Core.Domain.Contracts
{
    /// <summary>Data access for upcoming dates.</summary>
    public interface IUpcomingDateRepository : IRepository
    {
        IQueryable<UpcomingDate> QueryUpcomingDates(Guid userId);
        /// <summary>Soonest first, all-day ones after the timed ones on the same day.</summary>
        Task<List<UpcomingDate>> GetNextScheduled(Guid userId, DateOnly from, int limit, CancellationToken ct = default);
        Task<UpcomingDate?> GetUpcomingDateById(Guid upcomingDateId, Guid userId, CancellationToken ct = default);
        Task SaveUpcomingDate(UpcomingDate upcomingDate, CancellationToken ct = default);
        void DeleteUpcomingDate(UpcomingDate upcomingDate);
    }
}
