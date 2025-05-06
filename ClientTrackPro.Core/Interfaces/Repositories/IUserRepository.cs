using System;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Account;

namespace ClientTrackPro.Core.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> GetByUsernameAsync(string username);
        Task<bool> UpdateVerificationStatusAsync(Guid userId, bool isVerified);
        Task<bool> UpdateSubscriptionAsync(Guid userId, SubscriptionTier tier, DateTime? expiry);
        Task<bool> UpdateLastLoginAsync(Guid userId);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsUsernameUniqueAsync(string username);
    }
} 