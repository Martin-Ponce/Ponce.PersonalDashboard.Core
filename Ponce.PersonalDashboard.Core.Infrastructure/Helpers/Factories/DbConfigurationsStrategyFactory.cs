using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Infrastructure.Configurations;
using Ponce.PersonalDashboard.Core.Infrastructure.Helpers.Strategies;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Helpers.Factories
{
    /// <summary>Picks the mapping strategy for the configured database provider.</summary>
    internal static class DbConfigurationsStrategyFactory
    {
        public static IDbConfigurationsStrategy GetStrategy(string provider) => provider switch
        {
            DatabaseProviders.SQL_SERVER => new SqlServerConfigurationsStrategy(),
            DatabaseProviders.POSTGRES => new PostgresConfigurationsStrategy(),
            DatabaseProviders.SQLITE => new SqliteConfigurationsStrategy(),
            _ => throw ConfigurationException.InvalidConfiguration(
                ExceptionSources.APPSETTINGS,
                $"Database provider '{provider}' is not supported. Use one of: {DatabaseProviders.SQL_SERVER}, {DatabaseProviders.POSTGRES}, {DatabaseProviders.SQLITE}."
            )
        };
    }
}
