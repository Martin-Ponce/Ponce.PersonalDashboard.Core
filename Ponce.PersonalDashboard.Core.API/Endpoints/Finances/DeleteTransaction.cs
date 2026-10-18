using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Finances
{
    public static class DeleteTransaction
    {
        public const string ENDPOINT_NAME = "Delete transaction";

        public static RouteGroupBuilder MapDeleteTransaction(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapDelete("transactions/{transactionId:guid}",
                async ([FromRoute] Guid transactionId, FinancesService financesService, CancellationToken ct) =>
                {
                    await financesService.DeleteTransaction(transactionId, ct);
                    return Results.NoContent();
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Deletes a transaction.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
