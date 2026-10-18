namespace Ponce.PersonalDashboard.Core.API.Helpers.Security
{
    /// <summary>Claim names in the identity provider's tokens, read as-is since MapInboundClaims is off.</summary>
    public static class IdentityClaims
    {
        /// <summary>The user's id in the identity provider (a Guid), also used as our user id.</summary>
        public const string SUBJECT = "sub";
        /// <summary>The username. There's no email claim.</summary>
        public const string NAME = "name";
        public const string ROLE = "role";
    }
}
