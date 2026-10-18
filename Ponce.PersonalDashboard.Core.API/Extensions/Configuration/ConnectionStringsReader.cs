using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Infrastructure.Configurations;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.API.Extensions.Configuration
{
    public static partial class ConfigurationExtensions
    {
        private static string GetConnectionStringValue(this IConfiguration configuration, string sectionName)
        {
            // Empty counts as missing since appsettings.json ships it blank.
            var connectionString = configuration[$"DatabaseConnections:{sectionName}:ConnectionString"].TryOverwriteWithEnviromentValue($"{sectionName}_CONNECTION_STRING");
            if (string.IsNullOrWhiteSpace(connectionString))
                throw ConfigurationException.MissingConfiguration(
                    ExceptionSources.APPSETTINGS,
                    $"Missing connection string '{sectionName}': set it with user secrets or the {sectionName.ToUpperInvariant()}_CONNECTION_STRING environment variable"
                );
            return connectionString;
        }

        private static DatabaseConnectionConfiguration GetConnectionConfiguration(this IConfiguration configuration, string sectionName)
        {
            var connectionString = configuration.GetConnectionStringValue(sectionName);
            var provider = configuration[$"DatabaseConnections:{sectionName}:Provider"].TryOverwriteWithEnviromentValue($"{sectionName}_DATABASE_PROVIDER") ??
                throw ConfigurationException.MissingConfiguration(ExceptionSources.APPSETTINGS, "Missing database provider configuration");
            return new DatabaseConnectionConfiguration(provider, connectionString);
        }
    }
}
