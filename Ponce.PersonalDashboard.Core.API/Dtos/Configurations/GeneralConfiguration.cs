using Ponce.PersonalDashboard.Core.Infrastructure.Configurations;

namespace Ponce.PersonalDashboard.Core.API.Dtos.Configurations
{
    public class GeneralConfiguration
    {
        public required DatabaseConnectionConfiguration Database { get; set; }
        public required GeneralLoggerConfiguration Logger { get; set; }
        public required TelemetryConfiguration Telemetry { get; set; }
        public required CacheConfiguration Cache { get; set; }
        public required CorsConfiguration Cors { get; set; }
        public required AuthenticationConfiguration Authentication { get; set; }
        public required int RateLimiterMaxCalls { get; set; }
    }
}
