namespace Ponce.PersonalDashboard.Core.API.Endpoints.Users
{
    /// <summary>The signed-in user's profile and settings, always taken from the token, never from the URL.</summary>
    public static class UserEndpointGroup
    {
        public static RouteGroupBuilder MapUserEndpoints(this RouteGroupBuilder appEndpoints)
        {
            var userEndpointGroup = appEndpoints.MapGroup("users/me").WithTags("Users");
            userEndpointGroup
                .MapGetCurrentUser()
                .MapUpdateCurrentUser()
                .MapGetUserSettings()
                .MapSaveUserSettings();
            return appEndpoints;
        }
    }
}
