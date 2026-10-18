namespace Ponce.PersonalDashboard.Core.API.Endpoints.Overview
{
    /// <summary>Read-only summary across all features for the overview page.</summary>
    public static class OverviewEndpointGroup
    {
        public static RouteGroupBuilder MapOverviewEndpoints(this RouteGroupBuilder appEndpoints)
        {
            var overviewEndpointGroup = appEndpoints.MapGroup("overview").WithTags("Overview");
            overviewEndpointGroup
                .MapGetOverview();
            return appEndpoints;
        }
    }
}
