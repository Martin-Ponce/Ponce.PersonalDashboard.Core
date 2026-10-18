namespace Ponce.PersonalDashboard.Core.Domain.Exceptions
{
    /// <summary>Base for all app exceptions: a numeric Code plus optional Args. The error middleware maps it to an HTTP status.</summary>
    public abstract class AppException : Exception
    {
        public int Code { get; }
        public IReadOnlyDictionary<string, object>? Args { get; }

        protected AppException(int code, string message, IReadOnlyDictionary<string, object>? args = null)
            : base(message)
        {
            Code = code;
            Args = args;
        }
    }
}
