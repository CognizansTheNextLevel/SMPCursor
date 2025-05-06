using System;

namespace ClientTrackPro.Core.Models.Account
{
    public class PaymentRecord
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public string PaymentMethod { get; set; }
        public string SubscriptionTier { get; set; }
        public int DurationInDays { get; set; }
        public DateTime PaymentDate { get; set; }
        public string Status { get; set; } // Completed, Pending, Failed, Refunded
        public string TransactionType { get; set; } // Subscription, GiftCode, Refund
        public string Notes { get; set; }
    }
} 