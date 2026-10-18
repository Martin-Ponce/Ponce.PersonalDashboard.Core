using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.Users;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Users;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Users
{
    public static class SaveUserSettings
    {
        public const string ENDPOINT_NAME = "Save user settings";

        public static RouteGroupBuilder MapSaveUserSettings(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPut("settings",
                async ([FromBody] SaveUserSettingsRequest request, UserService userService, CancellationToken ct) =>
                {
                    var response = await userService.SaveSettings(request, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<UserSettingsResponse>(StatusCodes.Status200OK)
            .WithDescription("Replaces the signed-in user's settings with the frontend's whole settings object. Every property is required; theme, language, date format, currency and profile picture must be one of the values the frontend offers, and the time zone an IANA id.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
