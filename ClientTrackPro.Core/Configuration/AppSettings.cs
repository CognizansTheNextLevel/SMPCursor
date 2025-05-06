namespace ClientTrackPro.Core.Configuration
{
    public class AppSettings
    {
        public EmailSettings Email { get; set; }
        public PayPalSettings PayPal { get; set; }
        public StorageSettings Storage { get; set; }
        public SecuritySettings Security { get; set; }
        public PlatformSettings Platforms { get; set; }
    }

    public class EmailSettings
    {
        public string SmtpServer { get; set; }
        public int SmtpPort { get; set; }
        public string SmtpUsername { get; set; }
        public string SmtpPassword { get; set; }
        public string FromEmail { get; set; }
        public string FromName { get; set; }
    }

    public class PayPalSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string BusinessEmail { get; set; }
        public bool UseSandbox { get; set; }
    }

    public class StorageSettings
    {
        public string ConnectionString { get; set; }
        public string ContainerName { get; set; }
        public string BaseUrl { get; set; }
    }

    public class SecuritySettings
    {
        public string JwtSecret { get; set; }
        public int JwtExpiryInMinutes { get; set; }
        public int VerificationCodeExpiryInMinutes { get; set; }
        public int PasswordResetExpiryInMinutes { get; set; }
    }

    public class PlatformSettings
    {
        public TwitchSettings Twitch { get; set; }
        public YouTubeSettings YouTube { get; set; }
        public FacebookSettings Facebook { get; set; }
        public TikTokSettings TikTok { get; set; }
        public InstagramSettings Instagram { get; set; }
    }

    public class TwitchSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }

    public class YouTubeSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }

    public class FacebookSettings
    {
        public string AppId { get; set; }
        public string AppSecret { get; set; }
        public string RedirectUri { get; set; }
    }

    public class TikTokSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }

    public class InstagramSettings
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
        public string RedirectUri { get; set; }
    }
} 