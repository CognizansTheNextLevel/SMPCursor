using ClientTrackPro.Core.Models;
using ClientTrackPro.Core.Interfaces.Services;
using System.Windows.Input;

namespace ClientTrackPro.UI.ViewModels
{
    public class MainViewModel : BaseViewModel
    {
        private readonly IUserService _userService;
        private User _currentUser;
        private bool _showSubscriptionBanner;

        public MainViewModel(IUserService userService)
        {
            _userService = userService;
            LoadUserData();
        }

        public User CurrentUser
        {
            get => _currentUser;
            set => SetProperty(ref _currentUser, value);
        }

        public bool ShowSubscriptionBanner
        {
            get => _showSubscriptionBanner;
            set => SetProperty(ref _showSubscriptionBanner, value);
        }

        private async void LoadUserData()
        {
            try
            {
                var userId = await SecureStorage.GetAsync("userId");
                if (!string.IsNullOrEmpty(userId))
                {
                    CurrentUser = await _userService.GetUserByIdAsync(Guid.Parse(userId));
                    ShowSubscriptionBanner = CurrentUser.SubscriptionTier == SubscriptionTier.Free;
                }
            }
            catch (Exception ex)
            {
                // Handle error
                await Application.Current.MainPage.DisplayAlert("Error", "Failed to load user data", "OK");
            }
        }

        public bool CanAccessFeature(string featureName)
        {
            if (CurrentUser == null) return false;

            return featureName switch
            {
                "Dashboard" => true, // Available to all tiers
                "Streaming" => CurrentUser.SubscriptionTier >= SubscriptionTier.Streamer,
                "Chat" => CurrentUser.SubscriptionTier >= SubscriptionTier.Streamer,
                "Media" => CurrentUser.SubscriptionTier >= SubscriptionTier.Streamer,
                "Analytics" => CurrentUser.SubscriptionTier >= SubscriptionTier.Mogul,
                _ => false
            };
        }

        public ICommand RefreshCommand => new Command(async () =>
        {
            await LoadUserData();
        });
    }
} 