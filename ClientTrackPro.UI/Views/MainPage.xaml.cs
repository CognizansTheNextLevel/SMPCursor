using ClientTrackPro.UI.ViewModels;

namespace ClientTrackPro.UI.Views
{
    public partial class MainPage : ContentPage
    {
        private readonly MainViewModel _viewModel;

        public MainPage(MainViewModel viewModel)
        {
            InitializeComponent();
            _viewModel = viewModel;
            BindingContext = _viewModel;
        }

        private async void OnDashboardClicked(object sender, EventArgs e)
        {
            if (!_viewModel.CanAccessFeature("Dashboard"))
            {
                await ShowUpgradePrompt();
                return;
            }
            await Navigation.PushAsync(new DashboardPage());
        }

        private async void OnStreamingClicked(object sender, EventArgs e)
        {
            if (!_viewModel.CanAccessFeature("Streaming"))
            {
                await ShowUpgradePrompt();
                return;
            }
            await Navigation.PushAsync(new StreamingPage());
        }

        private async void OnChatClicked(object sender, EventArgs e)
        {
            if (!_viewModel.CanAccessFeature("Chat"))
            {
                await ShowUpgradePrompt();
                return;
            }
            await Navigation.PushAsync(new ChatPage());
        }

        private async void OnMediaClicked(object sender, EventArgs e)
        {
            if (!_viewModel.CanAccessFeature("Media"))
            {
                await ShowUpgradePrompt();
                return;
            }
            await Navigation.PushAsync(new MediaPage());
        }

        private async void OnAnalyticsClicked(object sender, EventArgs e)
        {
            if (!_viewModel.CanAccessFeature("Analytics"))
            {
                await ShowUpgradePrompt();
                return;
            }
            await Navigation.PushAsync(new AnalyticsPage());
        }

        private async void OnSettingsClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SettingsPage());
        }

        private async void OnAccountClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new AccountPage());
        }

        private async void OnUpgradeClicked(object sender, EventArgs e)
        {
            await Navigation.PushAsync(new SubscriptionPage());
        }

        private async Task ShowUpgradePrompt()
        {
            await DisplayAlert(
                "Feature Locked",
                "This feature is only available in the Streamer or Mogul subscription tiers. Would you like to upgrade?",
                "Upgrade Now",
                "Maybe Later"
            );
        }
    }
} 