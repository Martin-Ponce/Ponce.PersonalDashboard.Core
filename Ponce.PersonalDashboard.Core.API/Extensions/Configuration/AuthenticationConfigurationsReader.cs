using Ponce.PersonalDashboard.Core.API.Dtos.Configurations;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Validators;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.API.Extensions.Configuration
{
    public static partial class ConfigurationExtensions
    {
        public static AuthenticationConfiguration GetAuthenticationConfiguration(this IConfiguration configuration)
        {
            var authority = configuration["Authentication:Authority"].TryOverwriteWithEnviromentValue("AUTH_AUTHORITY");
            authority.ValidateNull(ConfigurationException.MissingConfiguration(
                ExceptionSources.APPSETTINGS,
                "The 'Authentication:Authority' configuration (the identity provider base URL) is required and was not found."
            ));
            if (!Uri.TryCreate(authority, UriKind.Absolute, out _))
                throw ConfigurationException.InvalidConfiguration(
                    ExceptionSources.APPSETTINGS,
                    "The 'Authentication:Authority' configuration must be an absolute URL."
                );

            var requireHttpsMetadataValue = configuration["Authentication:RequireHttpsMetadata"].TryOverwriteWithEnviromentValue("AUTH_REQUIRE_HTTPS_METADATA");
            var requireHttpsMetadata = true; // Secure by default
            if (!string.IsNullOrWhiteSpace(requireHttpsMetadataValue) && !bool.TryParse(requireHttpsMetadataValue, out requireHttpsMetadata))
                throw ConfigurationException.InvalidConfiguration(
                    ExceptionSources.APPSETTINGS,
                    "The 'Authentication:RequireHttpsMetadata' configuration must be true or false."
                );

            var useDevelopmentUserValue = configuration["Authentication:UseDevelopmentUser"].TryOverwriteWithEnviromentValue("AUTH_USE_DEVELOPMENT_USER");
            var useDevelopmentUser = false;
            if (!string.IsNullOrWhiteSpace(useDevelopmentUserValue) && !bool.TryParse(useDevelopmentUserValue, out useDevelopmentUser))
                throw ConfigurationException.InvalidConfiguration(
                    ExceptionSources.APPSETTINGS,
                    "The 'Authentication:UseDevelopmentUser' configuration must be true or false."
                );

            var internalAuthority = configuration["Authentication:InternalAuthority"].TryOverwriteWithEnviromentValue("AUTH_INTERNAL_AUTHORITY");
            if (string.IsNullOrWhiteSpace(internalAuthority))
                internalAuthority = null;
            else if (!Uri.TryCreate(internalAuthority, UriKind.Absolute, out _))
                throw ConfigurationException.InvalidConfiguration(
                    ExceptionSources.APPSETTINGS,
                    "The 'Authentication:InternalAuthority' configuration must be an absolute URL."
                );

            return new AuthenticationConfiguration(authority, requireHttpsMetadata, useDevelopmentUser, internalAuthority);
        }
    }
}
