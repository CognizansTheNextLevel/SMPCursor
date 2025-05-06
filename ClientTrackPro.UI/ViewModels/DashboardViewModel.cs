using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientTrackPro.Core.Models.Account;
using ClientTrackPro.Core.Models.Platform;
using ClientTrackPro.Core.Interfaces.Services;

namespace ClientTrackPro.UI.ViewModels
{
    public class DashboardViewModel : BaseViewModel
    {
        private readonly IAccountService _accountService;
        private readonly IPlatformService _platformService;
        private readonly IBrandService _brandService;

        public string WelcomeMessage { get; private set; }
        public string LastLoginMessage { get; private set; }
        public int TotalFollowers { get; private set; }
        public int ActivePlatforms { get; private set; }
        public int TotalAssets { get; private set; }
        public string SubscriptionTier { get; private set; }

        public ObservableCollection<PlatformConnectionViewModel> PlatformConnections { get; } = new();
        public ObservableCollection<ActivityViewModel> RecentActivities { get; } = new();

        public ICommand ConnectPlatformCommand { get; }
        public ICommand UploadAssetCommand { get; }
        public ICommand ViewAnalyticsCommand { get; }
        public ICommand OpenSettingsCommand { get; }

        public DashboardViewModel(
            IAccountService accountService,
            IPlatformService platformService,
            IBrandService brandService)
        {
            _accountService = accountService;
            _platformService = platformService;
            _brandService = brandService;

            ConnectPlatformCommand = new Command(async () => await ConnectPlatform());
            UploadAssetCommand = new Command(async () => await UploadAsset());
            ViewAnalyticsCommand = new Command(async () => await ViewAnalytics());
            OpenSettingsCommand = new Command(async () => await OpenSettings());
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await LoadDashboardData();
        }

        private async Task LoadDashboardData()
        {
            try
            {
                var user = await _accountService.GetUserByIdAsync(CurrentUserId);
                if (user == null) return;

                WelcomeMessage = $"Welcome back, {user.Username}!";
                LastLoginMessage = $"Last login: {user.LastLoginAt:g}";
                SubscriptionTier = user.SubscriptionTier.ToString();

                var connections = await _platformService.GetUserConnectionsAsync(CurrentUserId);
                ActivePlatforms = connections.Count;
                PlatformConnections.Clear();
                foreach (var connection in connections)
                {
                    PlatformConnections.Add(new PlatformConnectionViewModel(connection));
                }

                var assets = await _brandService.GetAllAssetsAsync(CurrentUserId);
                TotalAssets = assets.Count;

                // Load recent activities
                await LoadRecentActivities();
            }
            catch (Exception ex)
            {
                // Handle error
                await ShowErrorAsync("Failed to load dashboard data", ex.Message);
            }
        }

        private async Task LoadRecentActivities()
        {
            // Implement activity loading logic
            RecentActivities.Clear();
            // Add sample activities for now
            RecentActivities.Add(new ActivityViewModel
            {
                Title = "New Follower",
                Description = "You gained 10 new followers",
                Timestamp = DateTime.Now.AddHours(-1),
                ActivityIcon = "follower_icon.png"
            });
        }

        private async Task ConnectPlatform()
        {
            // Implement platform connection logic
            await Shell.Current.GoToAsync("//platforms");
        }

        private async Task UploadAsset()
        {
            // Implement asset upload logic
            await Shell.Current.GoToAsync("//brand");
        }

        private async Task ViewAnalytics()
        {
            // Implement analytics view logic
            await Shell.Current.GoToAsync("//analytics");
        }

        private async Task OpenSettings()
        {
            // Implement settings view logic
            await Shell.Current.GoToAsync("//settings");
        }
    }

    public class PlatformConnectionViewModel
    {
        public string PlatformName { get; }
        public string PlatformIcon { get; }
        public string Status { get; }
        public Color StatusColor { get; }

        public PlatformConnectionViewModel(PlatformConnection connection)
        {
            PlatformName = connection.Platform.ToString();
            PlatformIcon = $"{connection.Platform.ToString().ToLower()}_icon.png";
            Status = connection.SyncStatus;
            StatusColor = connection.IsActive ? Colors.Green : Colors.Red;
        }
    }

    public class ActivityViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public DateTime Timestamp { get; set; }
        public string ActivityIcon { get; set; }
    }
} 