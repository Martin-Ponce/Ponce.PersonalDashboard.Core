using Ponce.PersonalDashboard.Core.Application.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;

namespace Ponce.PersonalDashboard.Core.API.Helpers.Security
{
    /// <summary>The current user, read from the request's access token.</summary>
    public class CurrentUser(IHttpContextAccessor httpContextAccessor) : ICurrentUser
    {
        private readonly IHttpContextAccessor _httpContextAccessor = httpContextAccessor;

        public Guid UserId
        {
            get
            {
                var subject = _httpContextAccessor.HttpContext?.User.FindFirst(IdentityClaims.SUBJECT)?.Value;
                if (Guid.TryParse(subject, out var userId) && userId != Guid.Empty)
                    return userId;
                throw new UnauthorizedAppException(
                    UnauthorizedAppException.NOT_AUTHENTICATED,
                    $"The access token has no valid '{IdentityClaims.SUBJECT}' claim");
            }
        }
    }
}
