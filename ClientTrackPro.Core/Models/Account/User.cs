using System;
using System.Collections.Generic;

namespace ClientTrackPro.Core.Models.Account
{
    public class User
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public bool IsActive { get; set; }
        public bool IsEmailVerified { get; set; }
        public string VerificationCode { get; set; }
        public DateTime? VerificationCodeExpiry { get; set; }
        public string PasswordResetToken { get; set; }
        public DateTime? PasswordResetExpiry { get; set; }
        public bool TwoFactorEnabled { get; set; }
        public string TwoFactorSecret { get; set; }
        public string SubscriptionTier { get; set; }
        public DateTime? SubscriptionExpiry { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
        public int FailedLoginAttempts { get; set; }
        public DateTime? LockoutEnd { get; set; }

        // Privacy fields
        public bool DataProcessingConsent { get; set; }
        public bool MarketingConsent { get; set; }
        public bool ThirdPartyConsent { get; set; }
        public DateTime? LastConsentUpdate { get; set; }
        public string DataProcessingPurpose { get; set; }
        public bool IsDataAnonymized { get; set; }
        public DateTime? DataRetentionExpiry { get; set; }
        public string PrivacyPreferences { get; set; }

        // Financial security fields
        public decimal RiskScore { get; set; }
        public int SuspiciousActivityCount { get; set; }
        public DateTime? LastRiskAssessment { get; set; }
        public string PaymentVerificationStatus { get; set; }
        public List<string> BlockedPaymentMethods { get; set; }
        public string TransactionLimits { get; set; }
        public bool IsHighRiskUser { get; set; }
        public string ComplianceStatus { get; set; }
        public DateTime? LastComplianceCheck { get; set; }
    }

    public enum SubscriptionTier
    {
        Free,
        Streamer,
        Mogul
    }
} 