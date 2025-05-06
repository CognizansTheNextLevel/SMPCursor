using System;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Account;

namespace ClientTrackPro.Core.Interfaces.Services
{
    public interface IAccountService
    {
        Task<User> CreateAccountAsync(string email, string username, string password);
        Task<bool> VerifyEmailAsync(string email, string verificationCode);
        Task<bool> ResendVerificationCodeAsync(string email);
        Task<User> GetUserByIdAsync(Guid userId);
        Task<User> GetUserByEmailAsync(string email);
        Task<bool> UpdateSubscriptionAsync(Guid userId, SubscriptionTier tier);
        Task<bool> ProcessPayPalPaymentAsync(Guid userId, string paymentId, SubscriptionTier tier);
        Task<bool> UpdateUserProfileAsync(User user);
        Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        Task<bool> RequestPasswordResetAsync(string email);
        Task<bool> ResetPasswordAsync(string email, string resetCode, string newPassword);
    }
} 