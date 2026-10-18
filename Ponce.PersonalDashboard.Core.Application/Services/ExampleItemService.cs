using Mapster;
using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.ExampleItems;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.ExampleItems;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.Example;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.Application.Services
{
    /// <summary>Service for the example items.</summary>
    public class ExampleItemService(IExampleItemRepository exampleItemRepository, ILogger<ExampleItemService> logger)
    {
        private readonly IExampleItemRepository _exampleItemRepository = exampleItemRepository;
        private readonly ILogger<ExampleItemService> _logger = logger;

        public IQueryable<GetExampleItemsResponse> GetExampleItems() =>
            _exampleItemRepository.QueryExampleItems().ProjectToType<GetExampleItemsResponse>();

        public async Task<GetExampleItemResponse> GetExampleItem(Guid exampleItemId, CancellationToken ct = default)
        {
            var exampleItem = await GetExampleItemOrThrow(exampleItemId, ct);
            return exampleItem.Adapt<GetExampleItemResponse>();
        }

        public async Task<SaveExampleItemResponse> SaveExampleItem(SaveExampleItemRequest request, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving example item {ExampleItemName}", request.Name);
            var exampleItem = new ExampleItem(request.Name, request.Description);
            await _exampleItemRepository.SaveExampleItem(exampleItem, ct);
            await _exampleItemRepository.SaveChanges(ct);
            _logger.LogInformation("Example item {ExampleItemId} saved successfully", exampleItem.Id);
            return exampleItem.Adapt<SaveExampleItemResponse>();
        }

        private async Task<ExampleItem> GetExampleItemOrThrow(Guid exampleItemId, CancellationToken ct) =>
            await _exampleItemRepository.GetExampleItemById(exampleItemId, ct) ??
                throw new NotFoundException(
                    CustomExceptionCodes.EXAMPLEITEM_NOT_FOUND,
                    $"ExampleItem with id {exampleItemId} doesn't exist",
                    new Dictionary<string, object> { { "ExampleItemId", exampleItemId } });
    }
}
