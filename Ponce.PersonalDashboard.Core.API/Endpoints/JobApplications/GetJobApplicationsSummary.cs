using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.JobApplications;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.JobApplications
{
    public static class GetJobApplicationsSummary
    {
        public const string ENDPOINT_NAME = "Get job applications monthly summary";

        public static RouteGroupBuilder MapGetJobApplicationsSummary(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("summary",
                async ([FromQuery] short year, [FromQuery] short month, JobApplicationService jobApplicationService, CancellationToken ct) =>
                {
                    var response = await jobApplicationService.GetMonthlySummary(year, month, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<JobApplicationsSummaryResponse>(StatusCodes.Status200OK)
            .WithDescription("Gets how many applications were sent in the month, how many are at each status and the cities they're in (remote jobs and jobs without a city under \"Remote\").")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
