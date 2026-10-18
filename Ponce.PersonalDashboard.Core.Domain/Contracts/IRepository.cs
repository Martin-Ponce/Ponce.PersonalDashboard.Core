using System.Data;

namespace Ponce.PersonalDashboard.Core.Domain.Contracts
{
    public interface IRepository
    {
        Task SaveChanges(CancellationToken ct = default);
        IDbTransaction BeginTransaction();
        Task CloseTransaction();
    }
}
