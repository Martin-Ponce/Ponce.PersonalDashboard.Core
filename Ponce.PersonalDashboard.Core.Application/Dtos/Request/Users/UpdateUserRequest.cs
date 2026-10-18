namespace Ponce.PersonalDashboard.Core.Application.Dtos.Request.Users
{
    /// <summary>Just what this app owns, the username comes from the identity provider.</summary>
    public class UpdateUserRequest
    {
        /// <summary>Null or empty clears it.</summary>
        public string? DisplayName { get; set; }
    }
}
