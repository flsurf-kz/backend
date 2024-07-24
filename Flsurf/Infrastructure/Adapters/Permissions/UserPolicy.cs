using Flsurf.Application.Common.Interfaces;
using Flsurf.Domain.User.Entities;
using Flsurf.Presentation.Web.Services;
using Microsoft.EntityFrameworkCore;
using SpiceDb;

namespace Flsurf.Infrastructure.Adapters.Permissions
{
    public class UserPolicy
    {
        private IApplicationDbContext _context;
        private IUser _currentUser;
        private UserEntity? _cachedCurrentUser;

        public UserPolicy(IApplicationDbContext dbContext, IUser currentUser)
        {
            _currentUser = currentUser;
            _context = dbContext;
        }
        // Fuck me! 
        public async Task<UserEntity> GetCurrentUser()
        {
            if (_currentUser.Id == null)
                throw new AccessDenied("Unauthorized");
            if (_cachedCurrentUser == null)
            {
                _cachedCurrentUser = await GetUserById((Guid)_currentUser.Id);

                if (_cachedCurrentUser == null || _cachedCurrentUser.Blocked)
                {
                    throw new AccessDenied("User is not authorized");
                }
            }
            return _cachedCurrentUser;
        }

        private async Task<UserEntity> GetUserById(Guid userId)
        {
            var user = await _context.Users
                .Include(x => x.Warnings)
                .FirstOrDefaultAsync(x => x.Id == userId);

            Guard.Against.NotFound(userId, user);

            return user;
        }
    }
}
