namespace Ponce.PersonalDashboard.Core.API.Dtos.Configurations
{
    /// <summary>How we validate access tokens from the identity provider.</summary>
    public class AuthenticationConfiguration(string authority, bool requireHttpsMetadata, bool useDevelopmentUser = false, string? internalAuthority = null)
    {
        /// <summary>Identity provider base URL, has to match its issuer exactly.</summary>
        public string Authority { get; } = authority;
        /// <summary>False only for local dev against an identity provider over plain http.</summary>
        public bool RequireHttpsMetadata { get; } = requireHttpsMetadata;
        /// <summary>Skips token validation and signs everyone in as a fixed local user. Only honored in Development.</summary>
        public bool UseDevelopmentUser { get; } = useDevelopmentUser;
        /// <summary>Where this API reaches the identity provider when it differs from <see cref="Authority"/> (e.g. inside Docker).</summary>
        public string? InternalAuthority { get; } = internalAuthority;
    }
}
