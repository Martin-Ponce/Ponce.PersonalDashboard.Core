using Ponce.PersonalDashboard.Core.API.Dtos.Configurations;
using Ponce.PersonalDashboard.Core.API.Helpers;
using Ponce.PersonalDashboard.Core.Domain.Exceptions;
using Ponce.PersonalDashboard.Core.Domain.Validators;
using static Ponce.PersonalDashboard.Core.Domain.DomainConstants;

namespace Ponce.PersonalDashboard.Core.API.Extensions.Configuration
{
    public static partial class ConfigurationExtensions
    {
        private const string DATABASE_CONTEXT_CONNECTION_STRING_NAME = "Database";

        public static GeneralConfiguration GetConfiguration(this IConfiguration configuration)
        {
            var databaseConfiguration = configuration.GetConnectionConfiguration(DATABASE_CONTEXT_CONNECTION_STRING_NAME);
            return new GeneralConfiguration()
            {
                Database = databaseConfiguration,
                Logger = configuration.GetGeneralLoggerConfiguration(databaseConfiguration),
                RateLimiterMaxCalls = configuration.GetRateLimiterMaxCalls(),
                Telemetry = configuration.GetTelemetryConfiguration(),
                Cache = configuration.GetCacheConfiguration(),
                Cors = configuration.GetCorsConfiguration(),
                Authentication = configuration.GetAuthenticationConfiguration(),
            };
        }

        private static int GetRateLimiterMaxCalls(this IConfiguration configuration)
        {
            var value = configuration["MaxCallsPerMinute"].TryOverwriteWithEnviromentValue("MAX_CALLS");
            value.ValidateNull(ConfigurationException.MissingConfiguration(
                    ExceptionSources.APPSETTINGS,
                    "Missing maximum amount of calls"
                ));
            if (!int.TryParse(value, out var result))
                throw ConfigurationException.InvalidConfiguration(
                    ExceptionSources.APPSETTINGS,
                    "The maximum amount of calls has an invalid value"
                );
            return result;
        }
    }
}
