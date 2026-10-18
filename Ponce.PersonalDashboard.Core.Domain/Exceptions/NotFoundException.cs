namespace Ponce.PersonalDashboard.Core.Domain.Exceptions
{
    /// <summary>The thing you asked for doesn't exist (HTTP 404).</summary>
    public class NotFoundException : AppException
    {
        public NotFoundException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
            : base(code, message, args) { }
    }
}
