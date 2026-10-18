using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Users;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Users
{
    public static class GetUserSettings
    {
        public const string ENDPOINT_NAME = "Get user settings";

        public static RouteGroupBuilder MapGetUserSettings(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("settings",
                async (UserService userService, CancellationToken ct) =>
                {
                    var response = await userService.GetSettings(ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<UserSettingsResponse>(StatusCodes.Status200OK)
            .WithDescription("Gets the signed-in user's settings (theme, language, formats, time zone, profile picture and overview widgets). Returns the defaults if they never saved any.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
