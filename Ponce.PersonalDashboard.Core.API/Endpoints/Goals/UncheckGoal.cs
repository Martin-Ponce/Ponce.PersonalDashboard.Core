using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Goals;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Goals
{
    public static class UncheckGoal
    {
        public const string ENDPOINT_NAME = "Uncheck goal";

        public static RouteGroupBuilder MapUncheckGoal(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapDelete("{dailyGoalId:guid}/checkins/{date}",
                async ([FromRoute] Guid dailyGoalId, [FromRoute] DateOnly date, GoalService goalService, CancellationToken ct) =>
                {
                    var response = await goalService.UncheckDailyGoal(dailyGoalId, date, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<GoalResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Removes the check of a daily goal on the given date (yyyy-MM-dd), subtracting 1 from that week's weekly goal progress. Unchecking a day that wasn't checked changes nothing.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
