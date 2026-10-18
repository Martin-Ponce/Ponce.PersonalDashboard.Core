using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Finances
{
    public static class GetBalance
    {
        public const string ENDPOINT_NAME = "Get balance";

        public static RouteGroupBuilder MapGetBalance(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("balance",
                async (FinancesService financesService, CancellationToken ct) =>
                {
                    var response = await financesService.GetBalance(ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<BalanceResponse>(StatusCodes.Status200OK)
            .WithDescription("Gets the signed-in user's total balance: the opening balance plus income minus expenses of every month from the opening balance's as-of month up to the current one. Calculated on every read, never stored.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
