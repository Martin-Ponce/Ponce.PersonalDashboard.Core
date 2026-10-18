using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Goals;
using Ponce.PersonalDashboard.Core.Application.Services;
using Ponce.PersonalDashboard.Core.Domain.Helpers;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Goals
{
    public static class GetGoalById
    {
        public const string ENDPOINT_NAME = "Get goal by id";

        public static RouteGroupBuilder MapGetGoalById(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("{dailyGoalId:guid}",
                async ([FromRoute] Guid dailyGoalId, [FromQuery] DateOnly? date, GoalService goalService, CancellationToken ct) =>
                {
                    var response = await goalService.GetGoal(dailyGoalId, date ?? DateUtil.TodayUtc(), ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<GoalResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Gets a daily goal with its status on the given date (default: today in UTC) and its weekly progress.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
