namespace Ponce.PersonalDashboard.Core.API.Endpoints.JobApplications
{
    public static class JobApplicationEndpointGroup
    {
        public static RouteGroupBuilder MapJobApplicationEndpoints(this RouteGroupBuilder appEndpoints)
        {
            var jobApplicationEndpointGroup = appEndpoints.MapGroup("jobapplications").WithTags("JobApplications");
            jobApplicationEndpointGroup
                .MapGetJobApplications()
                .MapGetJobApplicationsSummary()
                .MapGetJobApplicationById()
                .MapSaveJobApplication()
                .MapUpdateJobApplication()
                .MapDeleteJobApplication();
            return appEndpoints;
        }
    }
}
