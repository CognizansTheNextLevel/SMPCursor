using System;
using System.Threading.Tasks;
using ClientTrackPro.UI.Views;
using ClientTrackPro.Core.Interfaces.Services;

namespace ClientTrackPro.UI
{
    public partial class AppShell : Shell
    {
        private readonly IAdminService _adminService;
        private bool _isAdmin;

        public bool IsAdmin
        {
            get => _isAdmin;
            set
            {
                _isAdmin = value;
                OnPropertyChanged();
            }
        }

        public AppShell(IAdminService adminService)
        {
            InitializeComponent();
            _adminService = adminService;

            // Register routes
            Routing.RegisterRoute("account", typeof(AccountPage));
            Routing.RegisterRoute("subscription", typeof(SubscriptionPage));
            Routing.RegisterRoute("platforms", typeof(PlatformsPage));
            Routing.RegisterRoute("brand", typeof(BrandPage));
            Routing.RegisterRoute("analytics", typeof(AnalyticsPage));
            Routing.RegisterRoute("settings", typeof(SettingsPage));
            Routing.RegisterRoute("admin", typeof(AdminPage));

            // Check admin status
            CheckAdminStatus();
        }

        private async void CheckAdminStatus()
        {
            try
            {
                // Get current user ID from your authentication system
                var currentUserId = Guid.Parse("your-current-user-id");
                IsAdmin = await _adminService.IsAdminAsync(currentUserId);
            }
            catch (Exception ex)
            {
                // Handle error
                Console.WriteLine($"Error checking admin status: {ex.Message}");
                IsAdmin = false;
            }
        }
    }
} 