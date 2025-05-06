using System;
using System.Threading.Tasks;
using System.Net.Mail;
using ClientTrackPro.Core.Interfaces.Services;
using ClientTrackPro.Core.Configuration;

namespace ClientTrackPro.Core.Services
{
    public class EmailService : IEmailService
    {
        private readonly AppSettings _appSettings;

        public EmailService(AppSettings appSettings)
        {
            _appSettings = appSettings;
        }

        public async Task<bool> SendVerificationEmailAsync(string email, string verificationCode)
        {
            var subject = "Verify Your Email Address";
            var body = $@"
                <h2>Welcome to ClientTrackPro!</h2>
                <p>Please verify your email address by entering the following code:</p>
                <h1 style='font-size: 24px; color: #007bff;'>{verificationCode}</h1>
                <p>This code will expire in {_appSettings.Security.VerificationCodeExpiryInMinutes} minutes.</p>
                <p>If you didn't request this verification, please ignore this email.</p>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> SendPasswordResetEmailAsync(string email, string resetCode)
        {
            var subject = "Reset Your Password";
            var body = $@"
                <h2>Password Reset Request</h2>
                <p>You have requested to reset your password. Use the following code to proceed:</p>
                <h1 style='font-size: 24px; color: #007bff;'>{resetCode}</h1>
                <p>This code will expire in {_appSettings.Security.PasswordResetExpiryInMinutes} minutes.</p>
                <p>If you didn't request this reset, please ignore this email and ensure your account is secure.</p>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> SendSubscriptionConfirmationAsync(string email, string subscriptionTier)
        {
            var subject = "Subscription Confirmation";
            var body = $@"
                <h2>Thank You for Your Subscription!</h2>
                <p>Your subscription to the {subscriptionTier} tier has been confirmed.</p>
                <p>You now have access to all features included in your subscription plan.</p>
                <p>If you have any questions, please don't hesitate to contact our support team.</p>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string username)
        {
            var subject = "Welcome to ClientTrackPro!";
            var body = $@"
                <h2>Welcome to ClientTrackPro, {username}!</h2>
                <p>We're excited to have you on board. Here's what you can do next:</p>
                <ul>
                    <li>Complete your profile</li>
                    <li>Connect your streaming platforms</li>
                    <li>Upload your brand assets</li>
                    <li>Explore our features</li>
                </ul>
                <p>If you need any help, our support team is always here for you.</p>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> SendNotificationEmailAsync(string email, string subject, string message)
        {
            var body = $@"
                <h2>{subject}</h2>
                <div>{message}</div>
                <p>This is an automated message from ClientTrackPro.</p>";

            return await SendEmailAsync(email, subject, body);
        }

        public async Task<bool> ValidateEmailAsync(string email)
        {
            try
            {
                var addr = new MailAddress(email);
                return addr.Address == email;
            }
            catch
            {
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(string to, string subject, string body)
        {
            try
            {
                using var client = new SmtpClient(_appSettings.Email.SmtpServer, _appSettings.Email.SmtpPort)
                {
                    EnableSsl = true,
                    Credentials = new System.Net.NetworkCredential(
                        _appSettings.Email.SmtpUsername,
                        _appSettings.Email.SmtpPassword)
                };

                using var message = new MailMessage
                {
                    From = new MailAddress(_appSettings.Email.FromEmail, _appSettings.Email.FromName),
                    Subject = subject,
                    Body = body,
                    IsBodyHtml = true
                };

                message.To.Add(to);

                await client.SendMailAsync(message);
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception
                return false;
            }
        }
    }
} 