using Ponce.PersonalDashboard.Core.Domain.Entities.Users;

namespace Ponce.PersonalDashboard.Core.Domain.Contracts
{
    /// <summary>Data access for users and their settings.</summary>
    public interface IUserRepository : IRepository
    {
        Task<User?> GetUserById(Guid userId, CancellationToken ct = default);
        /// <summary>Returns false instead of throwing if the user already exists (two first requests racing).</summary>
        Task<bool> TryInsertUser(User user, CancellationToken ct = default);

        /// <summary>Null if they never saved any.</summary>
        Task<UserSettings?> GetUserSettings(Guid userId, CancellationToken ct = default);
        Task SaveUserSettings(UserSettings settings, CancellationToken ct = default);
    }
}
