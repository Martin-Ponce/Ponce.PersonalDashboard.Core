using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.UpcomingDates;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.UpcomingDates;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.UpcomingDates
{
    public static class SaveUpcomingDate
    {
        public const string ENDPOINT_NAME = "Create upcoming date";

        public static RouteGroupBuilder MapSaveUpcomingDate(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPost("",
                async ([FromBody] SaveUpcomingDateRequest request, UpcomingDateService upcomingDateService, CancellationToken ct) =>
                {
                    var response = await upcomingDateService.SaveUpcomingDate(request, ct);
                    return Results.Created($"upcomingdates/{response.Id}", response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<SaveUpcomingDateResponse>(StatusCodes.Status201Created)
            .WithDescription("Creates a new upcoming date.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
