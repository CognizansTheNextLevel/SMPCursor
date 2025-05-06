using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Account;

namespace ClientTrackPro.Core.Interfaces.Services
{
    public interface IAdminService
    {
        Task<AdminUser> CreateAdminUserAsync(Guid userId, string role);
        Task<AdminUser> GetAdminUserAsync(Guid userId);
        Task<bool> IsAdminAsync(Guid userId);
        Task<bool> IsSuperAdminAsync(Guid userId);
        
        Task<List<User>> GetAllUsersAsync();
        Task<User> GetUserByIdAsync(Guid userId);
        Task<bool> UpdateUserSubscriptionAsync(Guid userId, string subscriptionTier, int durationInDays);
        Task<bool> DeactivateUserAsync(Guid userId);
        
        Task<SubscriptionGiftCode> CreateGiftCodeAsync(string subscriptionTier, int durationInDays, DateTime expiresAt, Guid adminId);
        Task<List<SubscriptionGiftCode>> GetAllGiftCodesAsync();
        Task<SubscriptionGiftCode> GetGiftCodeByCodeAsync(string code);
        Task<bool> DeactivateGiftCodeAsync(string code);
        
        Task<PaymentRecord> CreatePaymentRecordAsync(PaymentRecord payment);
        Task<List<PaymentRecord>> GetPaymentRecordsAsync(DateTime startDate, DateTime endDate);
        Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate);
        Task<Dictionary<string, int>> GetSubscriptionStatsAsync();
    }
} 