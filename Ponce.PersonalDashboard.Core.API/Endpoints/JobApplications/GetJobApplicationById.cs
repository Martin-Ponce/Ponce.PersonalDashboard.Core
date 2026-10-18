using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.JobApplications;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.JobApplications
{
    public static class GetJobApplicationById
    {
        public const string ENDPOINT_NAME = "Get job application by id";

        public static RouteGroupBuilder MapGetJobApplicationById(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("{jobApplicationId:guid}",
                async ([FromRoute] Guid jobApplicationId, JobApplicationService jobApplicationService, CancellationToken ct) =>
                {
                    var response = await jobApplicationService.GetJobApplication(jobApplicationId, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<GetJobApplicationResponse>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithDescription("Gets a specific job application by id.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
