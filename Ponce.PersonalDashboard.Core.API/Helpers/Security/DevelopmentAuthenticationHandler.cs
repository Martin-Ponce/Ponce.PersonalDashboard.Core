using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using System.Text.Encodings.Web;

namespace Ponce.PersonalDashboard.Core.API.Helpers.Security
{
    /// <summary>Dev-only auth that signs every request in as a fixed local user, so you can work without the identity provider.</summary>
    public class DevelopmentAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder)
        : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder)
    {
        public const string SCHEME = "Development";
        public const string USER_NAME = "dev";
        public static readonly Guid USER_ID = new("00000000-0000-0000-0000-000000000001");

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var identity = new ClaimsIdentity(
                [new Claim(IdentityClaims.SUBJECT, USER_ID.ToString()), new Claim(IdentityClaims.NAME, USER_NAME)],
                SCHEME,
                IdentityClaims.NAME,
                IdentityClaims.ROLE);
            var ticket = new AuthenticationTicket(new ClaimsPrincipal(identity), SCHEME);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
