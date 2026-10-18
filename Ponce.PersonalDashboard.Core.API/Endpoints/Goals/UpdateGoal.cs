using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.Goals;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Goals;
using Ponce.PersonalDashboard.Core.Application.Services;
using Ponce.PersonalDashboard.Core.Domain.Helpers;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Goals
{
    public static class UpdateGoal
    {
        public const string ENDPOINT_NAME = "Update goal";

        public static RouteGroupBuilder MapUpdateGoal(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPut("{dailyGoalId:guid}",
                async ([FromRoute] Guid dailyGoalId, [FromBody] SaveDailyGoalRequest request, [FromQuery] DateOnly? date, GoalService goalService, CancellationToken ct) =>
                {
                    var response = await goalService.UpdateDailyGoal(dailyGoalId, request, date ?? DateUtil.TodayUtc(), ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<GoalResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Replaces a daily goal's title, amount, unit and planned days, and its weekly goal: sending weeklyGoal creates or updates it, sending it as null removes it. Check-ins are kept.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
