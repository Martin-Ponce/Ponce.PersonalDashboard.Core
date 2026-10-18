using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Finances
{
    public static class GetMonthlySummary
    {
        public const string ENDPOINT_NAME = "Get monthly summary";

        public static RouteGroupBuilder MapGetMonthlySummary(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("summary",
                async ([FromQuery] short month, [FromQuery] short year, FinancesService financesService, CancellationToken ct) =>
                {
                    var response = await financesService.GetMonthlySummary(month, year, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<MonthlySummaryResponse>(StatusCodes.Status200OK)
            .WithDescription("Gets the month's total income, total expenses, net balance and budget (with what's left of it and whether it was exceeded).")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
