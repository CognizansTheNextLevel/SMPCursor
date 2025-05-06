using System;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Account;
using ClientTrackPro.Core.Interfaces.Services;
using ClientTrackPro.Core.Interfaces.Repositories;
using System.Security.Cryptography;
using System.Text;

namespace ClientTrackPro.Core.Services
{
    public class AccountService : IAccountService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IPayPalService _payPalService;
        private readonly AppSettings _appSettings;

        public AccountService(
            IUserRepository userRepository,
            IEmailService emailService,
            IPayPalService payPalService,
            AppSettings appSettings)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _payPalService = payPalService;
            _appSettings = appSettings;
        }

        public async Task<User> CreateAccountAsync(string email, string username, string password)
        {
            if (await _userRepository.IsEmailUniqueAsync(email) == false)
                throw new InvalidOperationException("Email already exists");

            if (await _userRepository.IsUsernameUniqueAsync(username) == false)
                throw new InvalidOperationException("Username already exists");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                PasswordHash = HashPassword(password),
                EmailVerified = false,
                VerificationCode = GenerateVerificationCode(),
                VerificationExpiry = DateTime.UtcNow.AddMinutes(_appSettings.Security.VerificationCodeExpiryInMinutes),
                SubscriptionTier = SubscriptionTier.Free,
                CreatedAt = DateTime.UtcNow,
                IsActive = true
            };

            await _userRepository.AddAsync(user);
            await _emailService.SendVerificationEmailAsync(email, user.VerificationCode);

            return user;
        }

        public async Task<bool> VerifyEmailAsync(string email, string verificationCode)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            if (user.VerificationCode != verificationCode || 
                user.VerificationExpiry < DateTime.UtcNow)
                return false;

            return await _userRepository.UpdateVerificationStatusAsync(user.Id, true);
        }

        public async Task<bool> ResendVerificationCodeAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            user.VerificationCode = GenerateVerificationCode();
            user.VerificationExpiry = DateTime.UtcNow.AddMinutes(_appSettings.Security.VerificationCodeExpiryInMinutes);

            await _userRepository.UpdateAsync(user);
            return await _emailService.SendVerificationEmailAsync(email, user.VerificationCode);
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<bool> UpdateSubscriptionAsync(Guid userId, SubscriptionTier tier)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            return await _userRepository.UpdateSubscriptionAsync(userId, tier, null);
        }

        public async Task<bool> ProcessPayPalPaymentAsync(Guid userId, string paymentId, SubscriptionTier tier)
        {
            if (!await _payPalService.ValidatePaymentAsync(paymentId))
                return false;

            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            var success = await _userRepository.UpdateSubscriptionAsync(
                userId, 
                tier, 
                DateTime.UtcNow.AddYears(1));

            if (success)
            {
                await _emailService.SendSubscriptionConfirmationAsync(
                    user.Email, 
                    tier.ToString());
            }

            return success;
        }

        public async Task<bool> UpdateUserProfileAsync(User user)
        {
            var existingUser = await _userRepository.GetByIdAsync(user.Id);
            if (existingUser == null) return false;

            // Only allow updating specific fields
            existingUser.Username = user.Username;
            // Add other updatable fields here

            await _userRepository.UpdateAsync(existingUser);
            return true;
        }

        public async Task<bool> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null) return false;

            if (user.PasswordHash != HashPassword(currentPassword))
                return false;

            user.PasswordHash = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);
            return true;
        }

        public async Task<bool> RequestPasswordResetAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            var resetCode = GenerateVerificationCode();
            user.VerificationCode = resetCode;
            user.VerificationExpiry = DateTime.UtcNow.AddMinutes(_appSettings.Security.PasswordResetExpiryInMinutes);

            await _userRepository.UpdateAsync(user);
            return await _emailService.SendPasswordResetEmailAsync(email, resetCode);
        }

        public async Task<bool> ResetPasswordAsync(string email, string resetCode, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null) return false;

            if (user.VerificationCode != resetCode || 
                user.VerificationExpiry < DateTime.UtcNow)
                return false;

            user.PasswordHash = HashPassword(newPassword);
            user.VerificationCode = null;
            user.VerificationExpiry = null;

            await _userRepository.UpdateAsync(user);
            return true;
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private string GenerateVerificationCode()
        {
            return Guid.NewGuid().ToString("N").Substring(0, 6).ToUpper();
        }
    }
} 