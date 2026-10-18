using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.JobApplications;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.JobApplications;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.JobApplications
{
    public static class UpdateJobApplication
    {
        public const string ENDPOINT_NAME = "Update job application";

        public static RouteGroupBuilder MapUpdateJobApplication(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPut("{jobApplicationId:guid}",
                async ([FromRoute] Guid jobApplicationId, [FromBody] SaveJobApplicationRequest request, JobApplicationService jobApplicationService, CancellationToken ct) =>
                {
                    var response = await jobApplicationService.UpdateJobApplication(jobApplicationId, request, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<SaveJobApplicationResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Updates every field of a job application, including its status.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
