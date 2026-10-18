namespace Ponce.PersonalDashboard.Core.Application.Contracts
{
    /// <summary>The logged-in user for the current request, every service scopes its data by this.</summary>
    public interface ICurrentUser
    {
        /// <summary>The token's sub claim, same id in the identity provider and in this app.</summary>
        Guid UserId { get; }
    }
}
