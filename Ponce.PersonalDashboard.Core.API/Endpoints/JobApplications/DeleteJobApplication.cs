using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.JobApplications
{
    public static class DeleteJobApplication
    {
        public const string ENDPOINT_NAME = "Delete job application";

        public static RouteGroupBuilder MapDeleteJobApplication(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapDelete("{jobApplicationId:guid}",
                async ([FromRoute] Guid jobApplicationId, JobApplicationService jobApplicationService, CancellationToken ct) =>
                {
                    await jobApplicationService.DeleteJobApplication(jobApplicationId, ct);
                    return Results.NoContent();
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces(StatusCodes.Status204NoContent)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Deletes a job application.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
