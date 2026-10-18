namespace Ponce.PersonalDashboard.Core.API.Dtos.Configurations
{
    public class GeneralLoggerConfiguration
    {
        public bool LogHttpRequests { get; set; }
        public required DbLoggerConfiguration Database { get; set; }
    }
}
