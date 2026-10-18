using Ponce.PersonalDashboard.Core.API.Dtos.Configurations;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Serilog.Events;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.API.Extensions.Configuration
{
    public static partial class ConfigurationExtensions
    {
        private static DbLoggerConfiguration GetDbLoggerConfiguration(this IConfiguration configuration, Infrastructure.Configurations.DatabaseConnectionConfiguration databaseConfiguration)
        {
            var enabled = configuration["Logger:Database:Enabled"].TryOverwriteWithEnviromentValue("LOGGER_DATABASE_ENABLED")
                .ToBool(ConfigurationException.InvalidConfiguration(ExceptionSources.APPSETTINGS, "The 'Logger:Database:Enabled' value must be a valid boolean"));
            var minimumLevel = ParseLogEventLevel(configuration["Logger:Database:MinimumLevel"].TryOverwriteWithEnviromentValue("LOGGER_DATABASE_MINIMUM_LEVEL"));
            return new DbLoggerConfiguration(enabled, databaseConfiguration.Provider, databaseConfiguration.ConnectionString, minimumLevel);
        }

        private static LogEventLevel ParseLogEventLevel(string? minimumLevelString)
        {
            if (string.IsNullOrEmpty(minimumLevelString))
            {
                throw ConfigurationException.InvalidConfiguration(
                    ExceptionSources.APPSETTINGS,
                    "The minimum log level cannot be empty"
                );
            }
            if (!Enum.TryParse<LogEventLevel>(minimumLevelString, true, out var result))
            {
                throw ConfigurationException.InvalidConfiguration(
                    ExceptionSources.APPSETTINGS,
                    $"The minimum log level '{minimumLevelString}' is not valid"
                );
            }
            return result;
        }

        private static bool GetLogHttpRequests(this IConfiguration configuration)
        {
            var logHttpRequests = configuration["Logger:LogHttpRequests"].TryOverwriteWithEnviromentValue("LOG_HTTP");
            return logHttpRequests.ToBool(ConfigurationException.InvalidConfiguration(
                    ExceptionSources.APPSETTINGS,
                    $"The 'LogHttpRequests' value must be a valid boolean, but '{logHttpRequests}' was received"
                ));
        }

        private static GeneralLoggerConfiguration GetGeneralLoggerConfiguration(this IConfiguration configuration, Infrastructure.Configurations.DatabaseConnectionConfiguration databaseConfiguration)
        {
            return new GeneralLoggerConfiguration()
            {
                LogHttpRequests = configuration.GetLogHttpRequests(),
                Database = configuration.GetDbLoggerConfiguration(databaseConfiguration),
            };
        }
    }
}
