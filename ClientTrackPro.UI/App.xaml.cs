using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using ClientTrackPro.Core.Data;
using ClientTrackPro.Core.Interfaces.Services;
using ClientTrackPro.Core.Interfaces.Repositories;
using ClientTrackPro.Core.Services;
using ClientTrackPro.Core.Repositories;
using ClientTrackPro.Core.Configuration;

namespace ClientTrackPro.UI
{
    public partial class App : Application
    {
        public static IServiceProvider ServiceProvider { get; private set; }

        public App()
        {
            InitializeComponent();

            var services = new ServiceCollection();
            ConfigureServices(services);
            ServiceProvider = services.BuildServiceProvider();

            MainPage = new AppShell();
        }

        private void ConfigureServices(IServiceCollection services)
        {
            // Configuration
            var appSettings = new AppSettings
            {
                Email = new EmailSettings
                {
                    SmtpServer = "smtp.gmail.com",
                    SmtpPort = 587,
                    SmtpUsername = "your-email@gmail.com",
                    SmtpPassword = "your-app-password",
                    FromEmail = "noreply@clienttrackpro.com",
                    FromName = "ClientTrackPro"
                },
                PayPal = new PayPalSettings
                {
                    ClientId = "your-paypal-client-id",
                    ClientSecret = "your-paypal-client-secret",
                    BusinessEmail = "your-paypal-business-email",
                    UseSandbox = true
                },
                Storage = new StorageSettings
                {
                    ConnectionString = "your-storage-connection-string",
                    ContainerName = "brand-assets",
                    BaseUrl = "https://your-storage-account.blob.core.windows.net"
                },
                Security = new SecuritySettings
                {
                    JwtSecret = "your-jwt-secret-key",
                    JwtExpiryInMinutes = 60,
                    VerificationCodeExpiryInMinutes = 30,
                    PasswordResetExpiryInMinutes = 30
                },
                Platforms = new PlatformSettings
                {
                    Twitch = new PlatformConfig
                    {
                        ClientId = "your-twitch-client-id",
                        ClientSecret = "your-twitch-client-secret",
                        RedirectUri = "https://clienttrackpro.com/auth/twitch/callback"
                    },
                    YouTube = new PlatformConfig
                    {
                        ClientId = "your-youtube-client-id",
                        ClientSecret = "your-youtube-client-secret",
                        RedirectUri = "https://clienttrackpro.com/auth/youtube/callback"
                    },
                    Facebook = new PlatformConfig
                    {
                        ClientId = "your-facebook-client-id",
                        ClientSecret = "your-facebook-client-secret",
                        RedirectUri = "https://clienttrackpro.com/auth/facebook/callback"
                    },
                    TikTok = new PlatformConfig
                    {
                        ClientId = "your-tiktok-client-id",
                        ClientSecret = "your-tiktok-client-secret",
                        RedirectUri = "https://clienttrackpro.com/auth/tiktok/callback"
                    },
                    Instagram = new PlatformConfig
                    {
                        ClientId = "your-instagram-client-id",
                        ClientSecret = "your-instagram-client-secret",
                        RedirectUri = "https://clienttrackpro.com/auth/instagram/callback"
                    }
                }
            };
            services.AddSingleton(appSettings);

            // Database
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer("Server=(localdb)\\mssqllocaldb;Database=ClientTrackPro;Trusted_Connection=True;MultipleActiveResultSets=true"));

            // Repositories
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IAdminRepository, AdminRepository>();
            services.AddScoped<ISubscriptionGiftCodeRepository, SubscriptionGiftCodeRepository>();
            services.AddScoped<IPaymentRecordRepository, PaymentRecordRepository>();
            services.AddScoped<IBrandAssetRepository, BrandAssetRepository>();
            services.AddScoped<IPlatformConnectionRepository, PlatformConnectionRepository>();

            // Services
            services.AddScoped<IAccountService, AccountService>();
            services.AddScoped<IAdminService, AdminService>();
            services.AddScoped<IBrandService, BrandService>();
            services.AddScoped<IPlatformService, PlatformService>();
            services.AddScoped<IEmailService, EmailService>();
            services.AddScoped<IPayPalService, PayPalService>();
            services.AddScoped<IAuthService, AuthService>();

            // HTTP Client for OAuth
            services.AddHttpClient();

            // ViewModels
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<AdminViewModel>();
            services.AddTransient<StreamingViewModel>();
            services.AddTransient<ChatViewModel>();
            services.AddTransient<MediaViewModel>();
            services.AddTransient<AnalyticsViewModel>();
            services.AddTransient<SettingsViewModel>();
        }
    }
} 