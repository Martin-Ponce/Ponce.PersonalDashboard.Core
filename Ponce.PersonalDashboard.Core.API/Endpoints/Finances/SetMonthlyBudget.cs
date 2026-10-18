using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.Finances;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Finances
{
    public static class SetMonthlyBudget
    {
        public const string ENDPOINT_NAME = "Set monthly budget";

        public static RouteGroupBuilder MapSetMonthlyBudget(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPut("summary/budget",
                async ([FromQuery] short month, [FromQuery] short year, [FromBody] SetMonthlyBudgetRequest request, FinancesService financesService, CancellationToken ct) =>
                {
                    var response = await financesService.SetMonthlyBudget(month, year, request, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<MonthlySummaryResponse>(StatusCodes.Status200OK)
            .WithDescription("Sets the month's budget (0 removes it) and returns the updated monthly summary.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
