using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Finances;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Finances
{
    public static class GetTransactionById
    {
        public const string ENDPOINT_NAME = "Get transaction by id";

        public static RouteGroupBuilder MapGetTransactionById(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("transactions/{transactionId:guid}",
                async ([FromRoute] Guid transactionId, FinancesService financesService, CancellationToken ct) =>
                {
                    var response = await financesService.GetTransaction(transactionId, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<GetTransactionResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Gets a specific transaction by id.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
