using Ponce.PersonalDashboard.Core.API.Dtos.Pagination;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.ExampleItems;
using Ponce.PersonalDashboard.Core.Application.Services;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.ExampleItems
{
    public static class GetExampleItems
    {
        public const string ENDPOINT_NAME = "Get example items";

        public static RouteGroupBuilder MapGetExampleItems(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("",
                async (HttpContext context, ExampleItemService exampleItemService, [AsParameters] PaginationRequest request) =>
                {
                    var exampleItems = exampleItemService.GetExampleItems();
                    var response = await Operation(exampleItems, request);
                    context.AddPaginationHeader(response);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<IEnumerable<GetExampleItemsResponse>>(StatusCodes.Status200OK)
            .WithDescription("Gets the list of example items in the system.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }

        private static IReadOnlyDictionary<string, IOrderedQueryable<GetExampleItemsResponse>> GetExampleItemsSorts(IQueryable<GetExampleItemsResponse> query) => new Dictionary<string, IOrderedQueryable<GetExampleItemsResponse>>()
        {
            { "name", query.OrderBy(a => a.Name) },
            { "name_desc", query.OrderByDescending(a => a.Name) },
        };

        private static async Task<PagedList<GetExampleItemsResponse>> Operation(IQueryable<GetExampleItemsResponse> query, PaginationRequest paginationRequest)
        {
            if (!string.IsNullOrEmpty(paginationRequest.Filter))
            {
                var filter = paginationRequest.Filter.ToLower();
                query = query.Where(a => a.Name.ToLower().Contains(filter));
            }
            if (!string.IsNullOrEmpty(paginationRequest.Sort))
            {
                if (!GetExampleItemsSorts(query).TryGetValue(paginationRequest.Sort, out var sortedQuery))
                {
                    throw ValidationException.NotSupported("An unsupported sort parameter was received");
                }
                query = sortedQuery;
            }
            return await PagedList<GetExampleItemsResponse>.ToPagedList(query, paginationRequest);
        }
    }
}
