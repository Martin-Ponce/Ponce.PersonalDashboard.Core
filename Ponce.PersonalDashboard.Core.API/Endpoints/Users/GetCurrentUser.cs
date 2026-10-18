using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Application.Dtos.Response.Users;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Endpoints.Users
{
    public static class GetCurrentUser
    {
        public const string ENDPOINT_NAME = "Get current user";

        public static RouteGroupBuilder MapGetCurrentUser(this RouteGroupBuilder endpointGroup)
        {
            endpointGroup.MapGet("",
                async (UserService userService, CancellationToken ct) =>
                {
                    var response = await userService.GetCurrentUser(ct);
                    return Results.Ok(response);
                }
            )
            .HasApiVersion(ApiConstants.VERSION_1)
            .Produces<GetUserResponse>(StatusCodes.Status200OK)
            .WithDescription("Gets the signed-in user's profile. The user is created automatically on their first request with the identity provider's username, which is refreshed on every request.")
            .WithSummary(ENDPOINT_NAME)
            .WithName(ENDPOINT_NAME);
            return endpointGroup;
        }
    }
}
