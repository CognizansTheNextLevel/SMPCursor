using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClientTrackPro.Core.Models.Account;
using ClientTrackPro.Core.Interfaces.Repositories;

namespace ClientTrackPro.Core.Repositories
{
    public class UserRepository : BaseRepository<User>, IUserRepository
    {
        public UserRepository(DbContext context) : base(context)
        {
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User> GetByUsernameAsync(string username)
        {
            return await _dbSet.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<bool> UpdateVerificationStatusAsync(Guid userId, bool isVerified)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.EmailVerified = isVerified;
            user.VerificationCode = null;
            user.VerificationExpiry = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateSubscriptionAsync(Guid userId, SubscriptionTier tier, DateTime? expiry)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.SubscriptionTier = tier;
            user.SubscriptionExpiry = expiry;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateLastLoginAsync(Guid userId)
        {
            var user = await GetByIdAsync(userId);
            if (user == null)
                return false;

            user.LastLoginAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsUsernameUniqueAsync(string username)
        {
            return !await _dbSet.AnyAsync(u => u.Username == username);
        }
    }
} 