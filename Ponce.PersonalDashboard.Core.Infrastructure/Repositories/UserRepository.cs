using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Ponce.PersonalDashboard.Core.Domain.Contracts;
using Ponce.PersonalDashboard.Core.Domain.Entities.Users;
using Ponce.PersonalDashboard.Core.Infrastructure.Contexts;

namespace Ponce.PersonalDashboard.Core.Infrastructure.Repositories
{
    public class UserRepository(AppDbContext context, ILogger<UserRepository> logger)
        : Repository<AppDbContext>(context), IUserRepository
    {
        private readonly ILogger<UserRepository> _logger = logger;

        public async Task<User?> GetUserById(Guid userId, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching user with id {UserId}", userId);
            return await _context.Users.FirstOrDefaultAsync(x => x.Id == userId, ct);
        }

        public async Task<bool> TryInsertUser(User user, CancellationToken ct = default)
        {
            _logger.LogDebug("Inserting user with id {UserId}", user.Id);
            var entry = await _context.Users.AddAsync(user, ct);
            try
            {
                await _context.SaveChangesAsync(ct);
                return true;
            }
            catch (DbUpdateException)
            {
                // Detach ours so the context stays usable.
                entry.State = EntityState.Detached;
                // Someone else inserted the same user first, that's fine. Anything else rethrows.
                if (!await _context.Users.AsNoTracking().AnyAsync(x => x.Id == user.Id, ct))
                    throw;
                _logger.LogDebug("User with id {UserId} was inserted concurrently", user.Id);
                return false;
            }
        }

        public async Task<UserSettings?> GetUserSettings(Guid userId, CancellationToken ct = default)
        {
            _logger.LogDebug("Fetching settings of user {UserId}", userId);
            return await _context.UserSettings.FirstOrDefaultAsync(x => x.UserId == userId, ct);
        }

        public async Task SaveUserSettings(UserSettings settings, CancellationToken ct = default)
        {
            _logger.LogDebug("Saving settings of user {UserId}", settings.UserId);
            await _context.UserSettings.AddAsync(settings, ct);
        }
    }
}
