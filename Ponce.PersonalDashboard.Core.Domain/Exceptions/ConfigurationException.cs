namespace Ponce.PersonalDashboard.Core.Domain.Exceptions
{
    /// <summary>A config value is missing or invalid. Thrown at startup.</summary>
    public class ConfigurationException : AppException
    {
        public static readonly int MISSING_CONFIGURATION = 1;
        public static readonly int INVALID_CONFIGURATION = 2;

        public static ConfigurationException MissingConfiguration(string source, string message) =>
            new(MISSING_CONFIGURATION, message, new Dictionary<string, object> { { "Source", source } });
        public static ConfigurationException InvalidConfiguration(string source, string message) =>
            new(INVALID_CONFIGURATION, message, new Dictionary<string, object> { { "Source", source } });

        public ConfigurationException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
            : base(code, message, args) { }
    }
}
