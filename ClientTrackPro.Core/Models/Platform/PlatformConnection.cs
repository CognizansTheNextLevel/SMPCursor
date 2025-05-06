using System;

namespace ClientTrackPro.Core.Models.Platform
{
    public class PlatformConnection
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public PlatformType Platform { get; set; }
        public string PlatformUserId { get; set; }
        public string PlatformUsername { get; set; }
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }
        public DateTime? TokenExpiry { get; set; }
        public bool IsPrimary { get; set; }
        public bool IsActive { get; set; }
        public DateTime ConnectedAt { get; set; }
        public DateTime? LastSyncedAt { get; set; }
        public string SyncStatus { get; set; }
    }

    public enum PlatformType
    {
        Twitch,
        YouTube,
        Facebook,
        TikTok,
        Instagram,
        Kick,
        LinkedIn,
        Vimeo,
        Rumble,
        SOOP,
        CHZZK,
        Trovo,
        Dlive
    }
} 