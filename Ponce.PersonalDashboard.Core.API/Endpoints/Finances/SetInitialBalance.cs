using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.Finances;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Finances
{
    public static class SetInitialBalance
    {
        public const string ENDPOINT_NAME = "Set initial balance";

        public static RouteGroupBuilder MapSetInitialBalance(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPut("balance/initial",
                async ([FromBody] SetInitialBalanceRequest request, FinancesService financesService, CancellationToken ct) =>
                {
                    var response = await financesService.SetInitialBalance(request, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<BalanceResponse>(StatusCodes.Status200OK)
            .WithDescription("Sets the signed-in user's opening balance (can be negative). Optionally send asOfMonth and asOfYear together: the balance is then the one at the start of that month (not after the current one), and only that month and later ones are added to it. Returns the recalculated balance.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
