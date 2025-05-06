using System;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Account;

namespace ClientTrackPro.Core.Interfaces.Services
{
    public interface IFinancialSecurityService
    {
        // Transaction security
        Task<(bool success, string message)> ValidateTransactionAsync(PaymentRecord payment);
        Task<(bool success, string message)> ProcessSecurePaymentAsync(PaymentRecord payment);
        Task<(bool success, string message)> RefundTransactionAsync(string paymentId);
        
        // Fraud detection
        Task<bool> IsTransactionSuspiciousAsync(PaymentRecord payment);
        Task<bool> IsUserSuspiciousAsync(Guid userId);
        Task<bool> IsIPAddressSuspiciousAsync(string ipAddress);
        
        // Payment verification
        Task<bool> VerifyPaymentMethodAsync(string paymentMethod, string userId);
        Task<bool> ValidatePaymentDetailsAsync(PaymentRecord payment);
        
        // Compliance
        Task<bool> IsTransactionCompliantAsync(PaymentRecord payment);
        Task<string> GenerateTransactionReportAsync(string paymentId);
        Task<string> GenerateComplianceReportAsync(DateTime startDate, DateTime endDate);
        
        // Risk assessment
        Task<decimal> CalculateTransactionRiskAsync(PaymentRecord payment);
        Task<decimal> CalculateUserRiskScoreAsync(Guid userId);
        
        // Monitoring
        Task LogTransactionAttemptAsync(PaymentRecord payment);
        Task LogFraudAttemptAsync(string paymentId, string reason);
        Task<string> GetTransactionHistoryAsync(Guid userId);
    }
} 