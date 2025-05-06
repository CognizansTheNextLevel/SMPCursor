using System.Threading.Tasks;

namespace ClientTrackPro.Core.Interfaces.Services
{
    public interface IEmailService
    {
        Task<bool> SendVerificationEmailAsync(string email, string verificationCode);
        Task<bool> SendPasswordResetEmailAsync(string email, string resetCode);
        Task<bool> SendSubscriptionConfirmationAsync(string email, string subscriptionTier);
        Task<bool> SendWelcomeEmailAsync(string email, string username);
        Task<bool> SendNotificationEmailAsync(string email, string subject, string message);
        Task<bool> ValidateEmailAsync(string email);
    }
} 