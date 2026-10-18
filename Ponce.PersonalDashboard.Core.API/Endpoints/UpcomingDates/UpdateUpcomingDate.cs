using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.UpcomingDates;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.UpcomingDates;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.UpcomingDates
{
    public static class UpdateUpcomingDate
    {
        public const string ENDPOINT_NAME = "Update upcoming date";

        public static RouteGroupBuilder MapUpdateUpcomingDate(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPut("{upcomingDateId:guid}",
                async ([FromRoute] Guid upcomingDateId, [FromBody] SaveUpcomingDateRequest request, UpcomingDateService upcomingDateService, CancellationToken ct) =>
                {
                    var response = await upcomingDateService.UpdateUpcomingDate(upcomingDateId, request, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<SaveUpcomingDateResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Updates every field of an upcoming date, including its status.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
