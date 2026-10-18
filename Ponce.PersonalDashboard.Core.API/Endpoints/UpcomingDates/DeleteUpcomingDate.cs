using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.UpcomingDates
{
    public static class DeleteUpcomingDate
    {
        public const string ENDPOINT_NAME = "Delete upcoming date";

        public static RouteGroupBuilder MapDeleteUpcomingDate(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapDelete("{upcomingDateId:guid}",
                async ([FromRoute] Guid upcomingDateId, UpcomingDateService upcomingDateService, CancellationToken ct) =>
                {
                    await upcomingDateService.DeleteUpcomingDate(upcomingDateId, ct);
                    return Results.NoContent();
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Deletes an upcoming date.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
