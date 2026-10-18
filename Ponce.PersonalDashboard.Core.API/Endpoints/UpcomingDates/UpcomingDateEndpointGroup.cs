namespace Ponce.PersonalDashboard.Core.API.Endpoints.UpcomingDates
{
    public static class UpcomingDateEndpointGroup
    {
        public static RouteGroupBuilder MapUpcomingDateEndpoints(this RouteGroupBuilder appEndpoints)
        {
            var upcomingDateEndpointGroup = appEndpoints.MapGroup("upcomingdates").WithTags("UpcomingDates");
            upcomingDateEndpointGroup
                .MapGetUpcomingDates()
                .MapGetUpcomingDateById()
                .MapSaveUpcomingDate()
                .MapUpdateUpcomingDate()
                .MapDeleteUpcomingDate();
            return appEndpoints;
        }
    }
}
