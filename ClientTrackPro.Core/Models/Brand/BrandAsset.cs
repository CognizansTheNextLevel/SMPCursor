using System;

namespace ClientTrackPro.Core.Models.Brand
{
    public class BrandAsset
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public AssetType Type { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string FileUrl { get; set; }
        public string ThumbnailUrl { get; set; }
        public string PlatformSyncStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? LastUpdated { get; set; }
        public bool IsActive { get; set; }
    }

    public enum AssetType
    {
        Logo,
        Banner,
        ProfilePicture,
        VideoClip,
        Overlay,
        Alert,
        Emote,
        Badge,
        Other
    }
} 