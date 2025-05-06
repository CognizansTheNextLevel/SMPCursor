using System;

namespace ClientTrackPro.Core.Models.Account
{
    public class SubscriptionGiftCode
    {
        public Guid Id { get; set; }
        public string Code { get; set; }
        public string SubscriptionTier { get; set; }
        public int DurationInDays { get; set; }
        public bool IsUsed { get; set; }
        public Guid? UsedByUserId { get; set; }
        public User UsedByUser { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime ExpiresAt { get; set; }
        public DateTime? UsedAt { get; set; }
        public Guid CreatedByAdminId { get; set; }
        public AdminUser CreatedByAdmin { get; set; }
    }
} 