using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Account;
using ClientTrackPro.Core.Interfaces.Services;
using ClientTrackPro.Core.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace ClientTrackPro.Core.Services
{
    public class AdminService : IAdminService
    {
        private readonly IUserRepository _userRepository;
        private readonly IAdminRepository _adminRepository;
        private readonly ISubscriptionGiftCodeRepository _giftCodeRepository;
        private readonly IPaymentRecordRepository _paymentRepository;

        public AdminService(
            IUserRepository userRepository,
            IAdminRepository adminRepository,
            ISubscriptionGiftCodeRepository giftCodeRepository,
            IPaymentRecordRepository paymentRepository)
        {
            _userRepository = userRepository;
            _adminRepository = adminRepository;
            _giftCodeRepository = giftCodeRepository;
            _paymentRepository = paymentRepository;
        }

        public async Task<AdminUser> CreateAdminUserAsync(Guid userId, string role)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                throw new ArgumentException("User not found");

            var adminUser = new AdminUser
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Role = role,
                IsActive = true,
                CreatedAt = DateTime.UtcNow
            };

            return await _adminRepository.CreateAsync(adminUser);
        }

        public async Task<AdminUser> GetAdminUserAsync(Guid userId)
        {
            return await _adminRepository.GetByUserIdAsync(userId);
        }

        public async Task<bool> IsAdminAsync(Guid userId)
        {
            var admin = await GetAdminUserAsync(userId);
            return admin != null && admin.IsActive;
        }

        public async Task<bool> IsSuperAdminAsync(Guid userId)
        {
            var admin = await GetAdminUserAsync(userId);
            return admin != null && admin.IsActive && admin.Role == "SuperAdmin";
        }

        public async Task<List<User>> GetAllUsersAsync()
        {
            return await _userRepository.GetAllAsync();
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<bool> UpdateUserSubscriptionAsync(Guid userId, string subscriptionTier, int durationInDays)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.SubscriptionTier = subscriptionTier;
            user.SubscriptionExpiry = DateTime.UtcNow.AddDays(durationInDays);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> DeactivateUserAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return false;

            user.IsActive = false;
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<SubscriptionGiftCode> CreateGiftCodeAsync(string subscriptionTier, int durationInDays, DateTime expiresAt, Guid adminId)
        {
            var code = GenerateUniqueGiftCode();
            var giftCode = new SubscriptionGiftCode
            {
                Id = Guid.NewGuid(),
                Code = code,
                SubscriptionTier = subscriptionTier,
                DurationInDays = durationInDays,
                IsUsed = false,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = expiresAt,
                CreatedByAdminId = adminId
            };

            return await _giftCodeRepository.CreateAsync(giftCode);
        }

        public async Task<List<SubscriptionGiftCode>> GetAllGiftCodesAsync()
        {
            return await _giftCodeRepository.GetAllAsync();
        }

        public async Task<SubscriptionGiftCode> GetGiftCodeByCodeAsync(string code)
        {
            return await _giftCodeRepository.GetByCodeAsync(code);
        }

        public async Task<bool> DeactivateGiftCodeAsync(string code)
        {
            var giftCode = await _giftCodeRepository.GetByCodeAsync(code);
            if (giftCode == null)
                return false;

            giftCode.IsUsed = true;
            await _giftCodeRepository.UpdateAsync(giftCode);
            return true;
        }

        public async Task<PaymentRecord> CreatePaymentRecordAsync(PaymentRecord payment)
        {
            payment.Id = Guid.NewGuid();
            payment.PaymentDate = DateTime.UtcNow;
            return await _paymentRepository.CreateAsync(payment);
        }

        public async Task<List<PaymentRecord>> GetPaymentRecordsAsync(DateTime startDate, DateTime endDate)
        {
            return await _paymentRepository.GetByDateRangeAsync(startDate, endDate);
        }

        public async Task<decimal> GetTotalRevenueAsync(DateTime startDate, DateTime endDate)
        {
            var payments = await _paymentRepository.GetByDateRangeAsync(startDate, endDate);
            return payments.Where(p => p.Status == "Completed")
                          .Sum(p => p.Amount);
        }

        public async Task<Dictionary<string, int>> GetSubscriptionStatsAsync()
        {
            var users = await _userRepository.GetAllAsync();
            return users.GroupBy(u => u.SubscriptionTier)
                       .ToDictionary(g => g.Key, g => g.Count());
        }

        private string GenerateUniqueGiftCode()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            var random = new Random();
            var code = new string(Enumerable.Repeat(chars, 8)
                .Select(s => s[random.Next(s.Length)]).ToArray());
            return code;
        }
    }
} 