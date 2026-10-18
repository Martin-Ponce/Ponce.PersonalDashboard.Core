using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.Example;
using Ponce.PersonalDashboard.Core.Infrastructure.Contexts;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Repositories
{
    public class ExampleItemRepository(AppDbContext context, ILogger<ExampleItemRepository> logger)
        : Repository<AppDbContext>(context), IExampleItemRepository
    {
        private readonly ILogger<ExampleItemRepository> _logger = logger;

        public IQueryable<ExampleItem> QueryExampleItems()
        {
            _logger.LogDebug("Querying example items");
            return _context.ExampleItems;
        }

        public async Task<ExampleItem?> GetExampleItemById(Guid exampleItemId, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching example item with id {ExampleItemId}", exampleItemId);
            return await _context.ExampleItems.FirstOrDefaultAsync(x => x.Id == exampleItemId, ct);
        }

        public async Task SaveExampleItem(ExampleItem exampleItem, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving example item with id {ExampleItemId}", exampleItem.Id);
            await _context.ExampleItems.AddAsync(exampleItem, ct);
        }
    }
}
