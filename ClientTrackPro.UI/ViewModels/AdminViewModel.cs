using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientTrackPro.Core.Models.Account;
using ClientTrackPro.Core.Interfaces.Services;

namespace ClientTrackPro.UI.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private readonly IAdminService _adminService;
        private decimal _totalRevenue;
        private int _activeUsers;
        private int _activeGiftCodes;

        public decimal TotalRevenue
        {
            get => _totalRevenue;
            set => SetProperty(ref _totalRevenue, value);
        }

        public int ActiveUsers
        {
            get => _activeUsers;
            set => SetProperty(ref _activeUsers, value);
        }

        public int ActiveGiftCodes
        {
            get => _activeGiftCodes;
            set => SetProperty(ref _activeGiftCodes, value);
        }

        public ObservableCollection<User> Users { get; } = new();
        public ObservableCollection<SubscriptionGiftCode> GiftCodes { get; } = new();
        public ObservableCollection<PaymentRecord> PaymentRecords { get; } = new();

        public ICommand CreateGiftCodeCommand { get; }
        public ICommand ViewReportsCommand { get; }
        public ICommand UpdateSubscriptionCommand { get; }
        public ICommand DeactivateUserCommand { get; }
        public ICommand DeactivateGiftCodeCommand { get; }

        public AdminViewModel(IAdminService adminService)
        {
            _adminService = adminService;

            CreateGiftCodeCommand = new Command(async () => await CreateGiftCode());
            ViewReportsCommand = new Command(async () => await ViewReports());
            UpdateSubscriptionCommand = new Command<Guid>(async (userId) => await UpdateSubscription(userId));
            DeactivateUserCommand = new Command<Guid>(async (userId) => await DeactivateUser(userId));
            DeactivateGiftCodeCommand = new Command<string>(async (code) => await DeactivateGiftCode(code));
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await LoadAdminData();
        }

        private async Task LoadAdminData()
        {
            try
            {
                // Load users
                var users = await _adminService.GetAllUsersAsync();
                Users.Clear();
                foreach (var user in users)
                {
                    Users.Add(user);
                }

                // Load gift codes
                var giftCodes = await _adminService.GetAllGiftCodesAsync();
                GiftCodes.Clear();
                foreach (var code in giftCodes)
                {
                    GiftCodes.Add(code);
                }

                // Load payment records
                var startDate = DateTime.UtcNow.AddMonths(-1);
                var endDate = DateTime.UtcNow;
                var payments = await _adminService.GetPaymentRecordsAsync(startDate, endDate);
                PaymentRecords.Clear();
                foreach (var payment in payments)
                {
                    PaymentRecords.Add(payment);
                }

                // Load statistics
                TotalRevenue = await _adminService.GetTotalRevenueAsync(startDate, endDate);
                ActiveUsers = Users.Count;
                ActiveGiftCodes = GiftCodes.Count;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to load admin data", ex.Message);
            }
        }

        private async Task CreateGiftCode()
        {
            try
            {
                // Show dialog to get gift code details
                var subscriptionTier = "Premium"; // Get from dialog
                var durationInDays = 30; // Get from dialog
                var expiresAt = DateTime.UtcNow.AddMonths(1); // Get from dialog

                var giftCode = await _adminService.CreateGiftCodeAsync(
                    subscriptionTier,
                    durationInDays,
                    expiresAt,
                    CurrentUserId);

                GiftCodes.Add(giftCode);
                ActiveGiftCodes++;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to create gift code", ex.Message);
            }
        }

        private async Task ViewReports()
        {
            try
            {
                // Implement reports view logic
                await Shell.Current.GoToAsync("//reports");
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to view reports", ex.Message);
            }
        }

        private async Task UpdateSubscription(Guid userId)
        {
            try
            {
                // Show dialog to get subscription details
                var subscriptionTier = "Premium"; // Get from dialog
                var durationInDays = 30; // Get from dialog

                var success = await _adminService.UpdateUserSubscriptionAsync(
                    userId,
                    subscriptionTier,
                    durationInDays);

                if (success)
                {
                    await LoadAdminData();
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to update subscription", ex.Message);
            }
        }

        private async Task DeactivateUser(Guid userId)
        {
            try
            {
                var success = await _adminService.DeactivateUserAsync(userId);
                if (success)
                {
                    await LoadAdminData();
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to deactivate user", ex.Message);
            }
        }

        private async Task DeactivateGiftCode(string code)
        {
            try
            {
                var success = await _adminService.DeactivateGiftCodeAsync(code);
                if (success)
                {
                    await LoadAdminData();
                }
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to deactivate gift code", ex.Message);
            }
        }
    }
} 