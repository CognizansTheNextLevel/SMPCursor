using System;

namespace ClientTrackPro.Core.Models.Account
{
    public class AdminUser
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public User User { get; set; }
        public string Role { get; set; } // SuperAdmin, Admin, Moderator
        public bool IsActive { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastLoginAt { get; set; }
    }
} 