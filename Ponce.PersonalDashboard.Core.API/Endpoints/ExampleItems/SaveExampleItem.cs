using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.ExampleItems;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.ExampleItems;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.ExampleItems
{
    public static class SaveExampleItem
    {
        public const string ENDPOINT_NAME = "Create example item";

        public static RouteGroupBuilder MapSaveExampleItem(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPost("",
                async ([FromBody] SaveExampleItemRequest request, ExampleItemService exampleItemService, CancellationToken ct) =>
                {
                    var response = await exampleItemService.SaveExampleItem(request, ct);
                    return Results.Created($"exampleitems/{response.Id}", response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<SaveExampleItemResponse>(StatusCodes.Status201Created)
            .WithDescription("Creates a new example item.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
