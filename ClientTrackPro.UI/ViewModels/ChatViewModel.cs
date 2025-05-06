using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using ClientTrackPro.Core.Models.Platform;
using ClientTrackPro.Core.Interfaces.Services;

namespace ClientTrackPro.UI.ViewModels
{
    public class ChatViewModel : BaseViewModel
    {
        private readonly IPlatformService _platformService;
        private bool _isChatting;
        private string _selectedPlatform;
        private string _selectedChatRoom;
        private string _chatMessage;

        public bool IsChatting
        {
            get => _isChatting;
            set => SetProperty(ref _isChatting, value);
        }

        public string SelectedPlatform
        {
            get => _selectedPlatform;
            set
            {
                if (SetProperty(ref _selectedPlatform, value))
                {
                    LoadChatRooms();
                }
            }
        }

        public string SelectedChatRoom
        {
            get => _selectedChatRoom;
            set => SetProperty(ref _selectedChatRoom, value);
        }

        public string ChatMessage
        {
            get => _chatMessage;
            set => SetProperty(ref _chatMessage, value);
        }

        public ObservableCollection<string> Platforms { get; } = new();
        public ObservableCollection<string> ChatRooms { get; } = new();
        public ObservableCollection<ChatMessageViewModel> ChatMessages { get; } = new();
        public ObservableCollection<ChatParticipantViewModel> Participants { get; } = new();

        public ICommand StartChatCommand { get; }
        public ICommand EndChatCommand { get; }
        public ICommand SendMessageCommand { get; }
        public ICommand ShowEmojiCommand { get; }

        public ChatViewModel(IPlatformService platformService)
        {
            _platformService = platformService;

            StartChatCommand = new Command(async () => await StartChat());
            EndChatCommand = new Command(async () => await EndChat());
            SendMessageCommand = new Command(async () => await SendMessage());
            ShowEmojiCommand = new Command(async () => await ShowEmojiPicker());

            LoadPlatforms();
        }

        public override async Task InitializeAsync()
        {
            await base.InitializeAsync();
            await LoadChatData();
        }

        private void LoadPlatforms()
        {
            Platforms.Clear();
            Platforms.Add("Twitch");
            Platforms.Add("YouTube");
            Platforms.Add("Facebook");
            Platforms.Add("TikTok");
        }

        private void LoadChatRooms()
        {
            ChatRooms.Clear();
            if (string.IsNullOrEmpty(SelectedPlatform))
                return;

            // Load chat rooms based on selected platform
            switch (SelectedPlatform)
            {
                case "Twitch":
                    ChatRooms.Add("General");
                    ChatRooms.Add("Gaming");
                    ChatRooms.Add("Just Chatting");
                    break;
                case "YouTube":
                    ChatRooms.Add("Live Chat");
                    ChatRooms.Add("Community");
                    break;
                case "Facebook":
                    ChatRooms.Add("Live Comments");
                    ChatRooms.Add("Group Chat");
                    break;
                case "TikTok":
                    ChatRooms.Add("Live Comments");
                    ChatRooms.Add("Duet Chat");
                    break;
            }
        }

        private async Task LoadChatData()
        {
            try
            {
                var connections = await _platformService.GetUserConnectionsAsync(CurrentUserId);
                // Load chat data from connected platforms
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to load chat data", ex.Message);
            }
        }

        private async Task StartChat()
        {
            if (string.IsNullOrWhiteSpace(SelectedPlatform))
            {
                await ShowErrorAsync("Error", "Please select a platform");
                return;
            }

            if (string.IsNullOrWhiteSpace(SelectedChatRoom))
            {
                await ShowErrorAsync("Error", "Please select a chat room");
                return;
            }

            try
            {
                // Implement chat start logic
                IsChatting = true;
                ChatMessages.Clear();
                Participants.Clear();

                // Add sample participants
                Participants.Add(new ChatParticipantViewModel
                {
                    Username = "You",
                    Avatar = "user_avatar.png"
                });
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to start chat", ex.Message);
            }
        }

        private async Task EndChat()
        {
            try
            {
                // Implement chat end logic
                IsChatting = false;
                ChatMessages.Clear();
                Participants.Clear();
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to end chat", ex.Message);
            }
        }

        private async Task SendMessage()
        {
            if (string.IsNullOrWhiteSpace(ChatMessage))
                return;

            try
            {
                // Implement message sending logic
                ChatMessages.Add(new ChatMessageViewModel
                {
                    Username = "You",
                    Message = ChatMessage,
                    UserAvatar = "user_avatar.png",
                    Timestamp = DateTime.Now
                });

                ChatMessage = string.Empty;
            }
            catch (Exception ex)
            {
                await ShowErrorAsync("Failed to send message", ex.Message);
            }
        }

        private async Task ShowEmojiPicker()
        {
            // Implement emoji picker logic
            await Task.CompletedTask;
        }
    }

    public class ChatMessageViewModel
    {
        public string Username { get; set; }
        public string Message { get; set; }
        public string UserAvatar { get; set; }
        public DateTime Timestamp { get; set; }
    }

    public class ChatParticipantViewModel
    {
        public string Username { get; set; }
        public string Avatar { get; set; }
    }
} 