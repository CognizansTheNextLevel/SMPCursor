using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Account;

namespace ClientTrackPro.Core.Interfaces.Services
{
    public interface IPayPalService
    {
        Task<string> CreateSubscriptionAsync(SubscriptionTier tier, bool isAnnual);
        Task<bool> ProcessPaymentAsync(string paymentId);
        Task<bool> CancelSubscriptionAsync(string subscriptionId);
        Task<bool> ValidatePaymentAsync(string paymentId);
        Task<decimal> GetSubscriptionPriceAsync(SubscriptionTier tier, bool isAnnual);
        Task<bool> RefundPaymentAsync(string paymentId, decimal amount);
        Task<bool> UpdateSubscriptionAsync(string subscriptionId, SubscriptionTier newTier);
    }
} 