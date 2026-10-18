using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using System.Data;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Repositories
{
    /// <summary>Base repository every feature repository derives from.</summary>
    public class Repository<TContext>(TContext context) : IRepository where TContext : DbContext
    {
        protected readonly TContext _context = context;

        public virtual async Task SaveChanges(CancellationToken ct = default)
        {
            await _context.SaveChangesAsync(ct);
        }

        public IDbTransaction BeginTransaction()
        {
            var transaction = _context.Database.BeginTransaction();
            return transaction.GetDbTransaction();
        }

        public async Task CloseTransaction()
        {
            if (_context.Database.CurrentTransaction != null)
                await _context.Database.CurrentTransaction.DisposeAsync();
        }
    }
}
