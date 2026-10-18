using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.UpcomingDates;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.UpcomingDates
{
    public static class GetUpcomingDateById
    {
        public const string ENDPOINT_NAME = "Get upcoming date by id";

        public static RouteGroupBuilder MapGetUpcomingDateById(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("{upcomingDateId:guid}",
                async ([FromRoute] Guid upcomingDateId, UpcomingDateService upcomingDateService, CancellationToken ct) =>
                {
                    var response = await upcomingDateService.GetUpcomingDate(upcomingDateId, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<GetUpcomingDateResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Gets a specific upcoming date by id.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
