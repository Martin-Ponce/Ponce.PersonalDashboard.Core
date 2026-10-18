using Microsoft.AspNetCore.Mvc;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Request.Users;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Users;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Users
{
    public static class UpdateCurrentUser
    {
        public const string ENDPOINT_NAME = "Update current user";

        public static RouteGroupBuilder MapUpdateCurrentUser(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapPut("",
                async ([FromBody] UpdateUserRequest request, UserService userService, CancellationToken ct) =>
                {
                    var response = await userService.UpdateCurrentUser(request, ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<GetUserResponse>(StatusCodes.Status200OK)
            .WithDescription("Updates what this app owns of the signed-in user's profile (the display name). The username is managed in the identity provider.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
