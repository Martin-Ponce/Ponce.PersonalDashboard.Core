using Ponce.PersonalDashboard.Core.API.Helpers.Security;
using Ponce.PersonalDashboard.Core.Application.Contracts;
using Ponce.PersonalDashboard.Core.Application.Services;

namespace Ponce.PersonalDashboard.Core.API.Middlewares
{
    /// <summary>Makes sure the signed-in user exists in our database (created on first request) with an up to date username.</summary>
    public class UserSynchronizationMiddleware(RequestDelegate next)
    {
        private readonly RequestDelegate _next = next;

        public async Task InvokeAsync(HttpContext context, UserService userService, ICurrentUser currentUser)
        {
            if (context.User.Identity?.IsAuthenticated == true)
            {
                var userId = currentUser.UserId;
                // The IdP always sends "name", the id is just a fallback.
                var userName = context.User.FindFirst(IdentityClaims.NAME)?.Value ?? userId.ToString();
                await userService.SyncUser(userId, userName, context.RequestAborted);
            }
            await _next(context);
        }
    }
}
