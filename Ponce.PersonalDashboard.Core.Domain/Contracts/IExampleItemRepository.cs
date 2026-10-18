using Ponce.PersonalDashboard.Core.Domain.Entities.Example;

namespace Ponce.PersonalDashboard.Core.Domain.Contracts
{
    /// <summary>Data access for example items.</summary>
    public interface IExampleItemRepository : IRepository
    {
        IQueryable<ExampleItem> QueryExampleItems();
        Task<ExampleItem?> GetExampleItemById(Guid exampleItemId, CancellationToken ct = default);
        Task SaveExampleItem(ExampleItem exampleItem, CancellationToken ct = default);
    }
}
