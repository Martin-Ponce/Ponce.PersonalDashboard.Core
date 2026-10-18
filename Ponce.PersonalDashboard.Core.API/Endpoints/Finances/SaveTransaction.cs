using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.Finances;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Finances
{
    public static class SaveTransaction
    {
        public const string ENDPOINT_NAME = "Create transaction";

        public static RouteGroupBuilder MapSaveTransaction(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPost("transactions",
                async ([FromBody] SaveTransactionRequest request, FinancesService financesService, CancellationToken ct) =>
                {
                    var response = await financesService.SaveTransaction(request, ct);
                    return Results.Created($"finances/transactions/{response.Transaction.Id}", response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<SaveTransactionResponse>(StatusCodes.Status201Created)
            .WithDescription("Creates a new transaction and returns it with the updated net total of its month.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
