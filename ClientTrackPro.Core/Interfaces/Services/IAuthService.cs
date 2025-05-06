using System;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Account;

namespace ClientTrackPro.Core.Interfaces.Services
{
    public interface IAuthService
    {
        // Traditional login
        Task<(bool success, string token, string message)> LoginAsync(string email, string password);
        Task<(bool success, string message)> RegisterAsync(string email, string username, string password);
        
        // OAuth login
        Task<(bool success, string token, string message)> LoginWithGoogleAsync(string googleToken);
        Task<(bool success, string token, string message)> LoginWithPlatformAsync(string platform, string platformToken);
        
        // Email verification
        Task<(bool success, string message)> SendVerificationEmailAsync(string email);
        Task<(bool success, string message)> VerifyEmailAsync(string email, string otp);
        Task<(bool success, string message)> ResendVerificationEmailAsync(string email);
        
        // Password management
        Task<(bool success, string message)> ForgotPasswordAsync(string email);
        Task<(bool success, string message)> ResetPasswordAsync(string email, string token, string newPassword);
        Task<(bool success, string message)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword);
        
        // Security features
        Task<(bool success, string message)> EnableTwoFactorAsync(Guid userId);
        Task<(bool success, string message)> DisableTwoFactorAsync(Guid userId);
        Task<(bool success, string message)> VerifyTwoFactorAsync(Guid userId, string code);
        
        // Session management
        Task<bool> ValidateTokenAsync(string token);
        Task<bool> RevokeTokenAsync(string token);
        Task<bool> IsEmailVerifiedAsync(string email);
        
        // User info
        Task<User> GetUserByEmailAsync(string email);
        Task<User> GetUserByIdAsync(Guid userId);
    }
} 