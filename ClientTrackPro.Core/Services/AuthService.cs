using System;
using System.Threading.Tasks;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Cryptography;
using Microsoft.Extensions.Options;
using ClientTrackPro.Core.Models.Account;
using ClientTrackPro.Core.Interfaces.Services;
using ClientTrackPro.Core.Interfaces.Repositories;
using ClientTrackPro.Core.Configuration;
using Google.Apis.Auth;
using System.Net.Http;
using System.Text.Json;

namespace ClientTrackPro.Core.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IPlatformService _platformService;
        private readonly AppSettings _appSettings;
        private readonly HttpClient _httpClient;
        private readonly Random _random;

        public AuthService(
            IUserRepository userRepository,
            IEmailService emailService,
            IPlatformService platformService,
            IOptions<AppSettings> appSettings,
            HttpClient httpClient)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _platformService = platformService;
            _appSettings = appSettings.Value;
            _httpClient = httpClient;
            _random = new Random();
        }

        public async Task<(bool success, string token, string message)> LoginAsync(string email, string password)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return (false, null, "Invalid email or password");

            if (!user.IsEmailVerified)
                return (false, null, "Please verify your email first");

            if (!VerifyPassword(password, user.PasswordHash))
                return (false, null, "Invalid email or password");

            if (!user.IsActive)
                return (false, null, "Account is deactivated");

            var token = GenerateJwtToken(user);
            return (true, token, "Login successful");
        }

        public async Task<(bool success, string message)> RegisterAsync(string email, string username, string password)
        {
            if (await _userRepository.GetByEmailAsync(email) != null)
                return (false, "Email already registered");

            if (await _userRepository.GetByUsernameAsync(username) != null)
                return (false, "Username already taken");

            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = email,
                Username = username,
                PasswordHash = HashPassword(password),
                IsActive = true,
                IsEmailVerified = false,
                CreatedAt = DateTime.UtcNow
            };

            await _userRepository.CreateAsync(user);
            await SendVerificationEmailAsync(email);

            return (true, "Registration successful. Please check your email for verification.");
        }

        public async Task<(bool success, string token, string message)> LoginWithGoogleAsync(string googleToken)
        {
            try
            {
                var settings = new GoogleJsonWebSignature.ValidationSettings()
                {
                    Audience = new[] { _appSettings.Google.ClientId }
                };

                var payload = await GoogleJsonWebSignature.ValidateAsync(googleToken, settings);
                var user = await _userRepository.GetByEmailAsync(payload.Email);

                if (user == null)
                {
                    // Create new user from Google account
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = payload.Email,
                        Username = payload.Email.Split('@')[0],
                        IsActive = true,
                        IsEmailVerified = true, // Google emails are pre-verified
                        CreatedAt = DateTime.UtcNow
                    };
                    await _userRepository.CreateAsync(user);
                }

                var token = GenerateJwtToken(user);
                return (true, token, "Login successful");
            }
            catch (Exception ex)
            {
                return (false, null, $"Google authentication failed: {ex.Message}");
            }
        }

        public async Task<(bool success, string token, string message)> LoginWithPlatformAsync(string platform, string platformToken)
        {
            try
            {
                var platformUser = await _platformService.ValidatePlatformTokenAsync(platform, platformToken);
                if (platformUser == null)
                    return (false, null, "Invalid platform token");

                var user = await _userRepository.GetByEmailAsync(platformUser.Email);
                if (user == null)
                {
                    // Create new user from platform account
                    user = new User
                    {
                        Id = Guid.NewGuid(),
                        Email = platformUser.Email,
                        Username = platformUser.Username,
                        IsActive = true,
                        IsEmailVerified = true, // Platform accounts are pre-verified
                        CreatedAt = DateTime.UtcNow
                    };
                    await _userRepository.CreateAsync(user);
                }

                var token = GenerateJwtToken(user);
                return (true, token, "Login successful");
            }
            catch (Exception ex)
            {
                return (false, null, $"Platform authentication failed: {ex.Message}");
            }
        }

        public async Task<(bool success, string message)> SendVerificationEmailAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return (false, "User not found");

            var otp = GenerateOTP();
            user.VerificationCode = otp;
            user.VerificationCodeExpiry = DateTime.UtcNow.AddMinutes(_appSettings.Security.VerificationCodeExpiryInMinutes);
            await _userRepository.UpdateAsync(user);

            await _emailService.SendVerificationEmailAsync(email, otp);
            return (true, "Verification email sent");
        }

        public async Task<(bool success, string message)> VerifyEmailAsync(string email, string otp)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return (false, "User not found");

            if (user.VerificationCode != otp)
                return (false, "Invalid verification code");

            if (user.VerificationCodeExpiry < DateTime.UtcNow)
                return (false, "Verification code expired");

            user.IsEmailVerified = true;
            user.VerificationCode = null;
            user.VerificationCodeExpiry = null;
            await _userRepository.UpdateAsync(user);

            return (true, "Email verified successfully");
        }

        public async Task<(bool success, string message)> ResendVerificationEmailAsync(string email)
        {
            return await SendVerificationEmailAsync(email);
        }

        public async Task<(bool success, string message)> ForgotPasswordAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return (false, "User not found");

            var token = GeneratePasswordResetToken();
            user.PasswordResetToken = token;
            user.PasswordResetExpiry = DateTime.UtcNow.AddMinutes(_appSettings.Security.PasswordResetExpiryInMinutes);
            await _userRepository.UpdateAsync(user);

            await _emailService.SendPasswordResetEmailAsync(email, token);
            return (true, "Password reset email sent");
        }

        public async Task<(bool success, string message)> ResetPasswordAsync(string email, string token, string newPassword)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            if (user == null)
                return (false, "User not found");

            if (user.PasswordResetToken != token)
                return (false, "Invalid reset token");

            if (user.PasswordResetExpiry < DateTime.UtcNow)
                return (false, "Reset token expired");

            user.PasswordHash = HashPassword(newPassword);
            user.PasswordResetToken = null;
            user.PasswordResetExpiry = null;
            await _userRepository.UpdateAsync(user);

            return (true, "Password reset successful");
        }

        public async Task<(bool success, string message)> ChangePasswordAsync(Guid userId, string currentPassword, string newPassword)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            if (!VerifyPassword(currentPassword, user.PasswordHash))
                return (false, "Current password is incorrect");

            user.PasswordHash = HashPassword(newPassword);
            await _userRepository.UpdateAsync(user);

            return (true, "Password changed successfully");
        }

        public async Task<(bool success, string message)> EnableTwoFactorAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            // Implement 2FA setup logic here
            return (true, "Two-factor authentication enabled");
        }

        public async Task<(bool success, string message)> DisableTwoFactorAsync(Guid userId)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            // Implement 2FA disable logic here
            return (true, "Two-factor authentication disabled");
        }

        public async Task<(bool success, string message)> VerifyTwoFactorAsync(Guid userId, string code)
        {
            var user = await _userRepository.GetByIdAsync(userId);
            if (user == null)
                return (false, "User not found");

            // Implement 2FA verification logic here
            return (true, "Two-factor authentication verified");
        }

        public async Task<bool> ValidateTokenAsync(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var key = Encoding.ASCII.GetBytes(_appSettings.Security.JwtSecret);
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                return true;
            }
            catch
            {
                return false;
            }
        }

        public async Task<bool> RevokeTokenAsync(string token)
        {
            // Implement token revocation logic here
            return true;
        }

        public async Task<bool> IsEmailVerifiedAsync(string email)
        {
            var user = await _userRepository.GetByEmailAsync(email);
            return user?.IsEmailVerified ?? false;
        }

        public async Task<User> GetUserByEmailAsync(string email)
        {
            return await _userRepository.GetByEmailAsync(email);
        }

        public async Task<User> GetUserByIdAsync(Guid userId)
        {
            return await _userRepository.GetByIdAsync(userId);
        }

        private string GenerateJwtToken(User user)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Security.JwtSecret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                    new Claim(ClaimTypes.Email, user.Email),
                    new Claim(ClaimTypes.Name, user.Username)
                }),
                Expires = DateTime.UtcNow.AddMinutes(_appSettings.Security.JwtExpiryInMinutes),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key),
                    SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        private string HashPassword(string password)
        {
            using var sha256 = SHA256.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(hashedBytes);
        }

        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }

        private string GenerateOTP()
        {
            return _random.Next(100000, 999999).ToString();
        }

        private string GeneratePasswordResetToken()
        {
            return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
        }
    }
} 