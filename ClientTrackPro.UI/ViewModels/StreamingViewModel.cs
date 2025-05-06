using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientTrackPro.Core.Models.Platform;
using ClientTrackPro.Core.Interfaces.Services;

namespace ClientTrackPro.UI.ViewModels
{
    public class StreamingViewModel : BaseViewModel
    {
        private readonly IPlatformService _platformService;
        private bool _isStreaming;
        private string _streamTitle;
        private string _selectedCategory;
        private string _selectedPlatform;
        private int _currentViewers;
        private int _chatMessages;
        private TimeSpan _streamDuration;
        private int _newFollowers;
        private string _chatMessage;

        public bool IsStreaming
        {
            get => _isStreaming;
            set => SetProperty(ref _isStreaming, value);
        }

        public string StreamTitle
        {
            get => _streamTitle;
            set => SetProperty(ref _streamTitle, value);
        }

        public string SelectedCategory
        {
            get => _selectedCategory;
            set => SetProperty(ref _selectedCategory, value);
        }

        public string SelectedPlatform
        {
            get => _selectedPlatform;
            set => SetProperty(ref _selectedPlatform, value);
        }

        public int CurrentViewers
        {
            get => _currentViewers;
            set => SetProperty(ref _currentViewers, value);
        }

        public int ChatMessages
        {
            get => _chatMessages;
            set => SetProperty(ref _chatMessages, value);
        }

        public TimeSpan StreamDuration
        {
            get => _streamDuration;
            set => SetProperty(ref _streamDuration, value);
        }

        public int NewFollowers
        {
            get => _newFollowers;
            set => SetProperty(ref _newFollowers, value);
        }

        public string ChatMessage
        {
            get => _chatMessage;
            set => SetProperty(ref _chatMessage, value);
        }

        public ObservableCollection<string> Categories { get; } = new();
        public ObservableCollection<string> Platforms { get; } = new();
        public ObservableCollection<ChatMessageViewModel> ChatMessagesList { get; } = new();
        public ObservableCollection<StreamAlertViewModel> StreamAlerts { get; } = new();

        public ICommand StartStreamCommand { get; }
        public ICommand EndStreamCommand { get; }
        public ICommand SendChatMessageCommand { get; }

        public StreamingViewModel(IPlatformService platformService)
        {
            _platformService = platformService;

            StartStreamCommand = new Command(async () => await StartStream());
            EndStreamCommand = new Command(async () => await EndStream());
            SendChatMessageCommand = new Command(async () => await SendChatMessage());

            LoadCategories();
            LoadPlatforms();
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await LoadStreamData();
        }

        private void LoadCategories()
        {
            Categories.Clear();
            Categories.Add("Gaming");
            Categories.Add("Just Chatting");
            Categories.Add("Music");
            Categories.Add("Art");
            Categories.Add("Education");
            Categories.Add("Other");
        }

        private void LoadPlatforms()
        {
            Platforms.Clear();
            Platforms.Add("Twitch");
            Platforms.Add("YouTube");
            Platforms.Add("Facebook");
            Platforms.Add("TikTok");
        }

        private async Task LoadStreamData()
        {
            try
            {
                var connections = await _platformService.GetUserConnectionsAsync(CurrentUserId);
                // Load stream data from connected platforms
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to load stream data", ex.Message);
            }
        }

        private async Task StartStream()
        {
            if (string.IsNullOrWhiteSpace(StreamTitle))
            {
                await ShowErrorAsync("Error", "Please enter a stream title");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedCategory))
            {
                await ShowErrorAsync("Error", "Please select a category");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedPlatform))
            {
                await ShowErrorAsync("Error", "Please select a platform");
                return;
            }

            try
            {
                // Implement stream start logic
                IsStreaming = true;
                StreamDuration = TimeSpan.Zero;
                StartStreamTimer();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to start stream", ex.Message);
            }
        }

        private async Task EndStream()
        {
            try
            {
                // Implement stream end logic
                IsStreaming = false;
                StopStreamTimer();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to end stream", ex.Message);
            }
        }

        private async Task SendChatMessage()
        {
            if (string.IsNullOrWhiteSpace(ChatMessage))
                return;

            try
            {
                // Implement chat message sending logic
                ChatMessagesList.Add(new ChatMessageViewModel
                {
                    Username = "You",
                    Message = ChatMessage,
                    UserAvatar = "user_avatar.png"
                });

                ChatMessage = string.Empty;
                ChatMessages++;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to send message", ex.Message);
            }
        }

        private void StartStreamTimer()
        {
            // Implement stream duration timer
        }

        private void StopStreamTimer()
        {
            // Implement stream duration timer stop
        }
    }

    public class ChatMessageViewModel
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public string UserAvatar { get; set; }
    }

    public class StreamAlertViewModel
    {
        public string Title { get; set; }
        public string Description { get; set; }
        public string AlertIcon { get; set; }
    }
} 