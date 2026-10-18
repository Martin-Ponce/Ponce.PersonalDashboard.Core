using Ponce.PersonalDashboard.Core.Domain.Entities.Base;
using Ponce.PersonalDashboard.Core.Domain.Validators.Entities;

namespace Ponce.PersonalDashboard.Core.Domain.Entities.Users
{
    /// <summary>Local copy of an identity provider user. The Id is the IdP's sub claim.</summary>
    public class User : AggregateRoot
    {
        /// <summary>Comes from the IdP's name claim, can't be edited here.</summary>
        public string UserName { get; private set; } = default!;
        /// <summary>Null means UserName is shown.</summary>
        public string? DisplayName { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }

        private User() { } // Required by EF Core

        public User(Guid identityProviderId, string userName)
        {
            Id = identityProviderId.ValidateUserId();
            CreatedAtUtc = DateTime.UtcNow;
            SyncIdentity(userName);
        }

        /// <summary>Updates the username from the token. Returns true if it changed.</summary>
        public bool SyncIdentity(string userName)
        {
            var validatedUserName = userName.ValidateUserName();
            if (validatedUserName == UserName)
                return false;
            UserName = validatedUserName;
            return true;
        }

        public void SetDisplayName(string? displayName) => DisplayName = displayName.ValidateDisplayName();
    }
}
