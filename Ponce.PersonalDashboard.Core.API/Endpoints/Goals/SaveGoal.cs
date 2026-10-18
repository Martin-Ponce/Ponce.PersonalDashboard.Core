using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.Goals;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Goals;
using Ponce.PersonalDashboard.Core.Application.Services;
using Ponce.PersonalDashboard.Core.Domain.Helpers;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Goals
{
    public static class SaveGoal
    {
        public const string ENDPOINT_NAME = "Create goal";

        public static RouteGroupBuilder MapSaveGoal(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPost("",
                async ([FromBody] SaveDailyGoalRequest request, [FromQuery] DateOnly? date, GoalService goalService, CancellationToken ct) =>
                {
                    var response = await goalService.SaveDailyGoal(request, date ?? DateUtil.TodayUtc(), ct);
                    return Results.Created($"goals/{response.Id}", response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<GoalResponse>(StatusCodes.Status201Created)
            .WithDescription("Creates a daily goal and, optionally, the weekly goal linked to it.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
