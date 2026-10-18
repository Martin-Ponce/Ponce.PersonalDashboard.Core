using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Goals;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Goals
{
    public static class CheckGoal
    {
        public const string ENDPOINT_NAME = "Check goal";

        public static RouteGroupBuilder MapCheckGoal(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPut("{dailyGoalId:guid}/checkins/{date}",
                async ([FromRoute] Guid dailyGoalId, [FromRoute] DateOnly date, GoalService goalService, CancellationToken ct) =>
                {
                    var response = await goalService.CheckDailyGoal(dailyGoalId, date, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<GoalResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Marks a daily goal as done on the given date (yyyy-MM-dd, the client's local date), adding 1 to that week's weekly goal progress. Checking an already checked day changes nothing.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
