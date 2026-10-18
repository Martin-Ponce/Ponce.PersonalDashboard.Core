using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Overview;
using Ponce.PersonalDashboard.Core.Application.Services;
using Ponce.PersonalDashboard.Core.Domain.Helpers;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Overview
{
    public static class GetOverview
    {
        public const string ENDPOINT_NAME = "Get overview";

        public static RouteGroupBuilder MapGetOverview(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("",
                async ([FromQuery] DateOnly? date, OverviewService overviewService, CancellationToken ct) =>
                {
                    var response = await overviewService.GetOverview(date ?? DateUtil.TodayUtc(), ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<OverviewResponse>(StatusCodes.Status200OK)
            .WithDescription("Gets everything the overview page shows in one call, for the month and week that contain the given date (yyyy-MM-dd, the client's local date; default: today in UTC): balance, this month's totals, budget and expenses per category, the last 6 months' expenses, the next 5 scheduled dates, and job application counts plus the 5 most recent.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
