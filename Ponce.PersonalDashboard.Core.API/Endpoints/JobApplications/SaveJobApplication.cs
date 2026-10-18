using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.JobApplications;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.JobApplications;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.JobApplications
{
    public static class SaveJobApplication
    {
        public const string ENDPOINT_NAME = "Create job application";

        public static RouteGroupBuilder MapSaveJobApplication(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPost("",
                async ([FromBody] SaveJobApplicationRequest request, JobApplicationService jobApplicationService, CancellationToken ct) =>
                {
                    var response = await jobApplicationService.SaveJobApplication(request, ct);
                    return Results.Created($"jobapplications/{response.Id}", response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<SaveJobApplicationResponse>(StatusCodes.Status201Created)
            .WithDescription("Creates a new job application.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
