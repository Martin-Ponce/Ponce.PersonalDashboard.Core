using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.Finances;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Finances
{
    public static class UpdateTransaction
    {
        public const string ENDPOINT_NAME = "Update transaction";

        public static RouteGroupBuilder MapUpdateTransaction(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPut("transactions/{transactionId:guid}",
                async ([FromRoute] Guid transactionId, [FromBody] UpdateTransactionRequest request, FinancesService financesService, CancellationToken ct) =>
                {
                    var response = await financesService.UpdateTransaction(transactionId, request, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<SaveTransactionResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Updates the type, category, description and amount of a transaction and returns it with the updated net total of its month.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
