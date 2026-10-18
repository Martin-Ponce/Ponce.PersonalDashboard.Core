namespace Ponce.PersonalDashboard.Core.Domain.Exceptions
{
    /// <summary>The operation isn't allowed right now (HTTP 400).</summary>
    public class InvalidOperationAppException : AppException
    {
        public InvalidOperationAppException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
            : base(code, message, args) { }
    }
}
