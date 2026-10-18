using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Goals
{
    public static class DeleteGoal
    {
        public const string ENDPOINT_NAME = "Delete goal";

        public static RouteGroupBuilder MapDeleteGoal(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapDelete("{dailyGoalId:guid}",
                async ([FromRoute] Guid dailyGoalId, GoalService goalService, CancellationToken ct) =>
                {
                    await goalService.DeleteDailyGoal(dailyGoalId, ct);
                    return Results.NoContent();
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Deletes a daily goal together with its weekly goal and its check-in history.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
