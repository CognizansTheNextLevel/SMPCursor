using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Text.Json;
using ClientTrackPro.Core.Models.Platform;
using ClientTrackPro.Core.Interfaces.Services;
using ClientTrackPro.Core.Interfaces.Repositories;
using ClientTrackPro.Core.Configuration;

namespace ClientTrackPro.Core.Services
{
    public class PlatformService : IPlatformService
    {
        private readonly IPlatformConnectionRepository _connectionRepository;
        private readonly AppSettings _appSettings;

        public PlatformService(
            IPlatformConnectionRepository connectionRepository,
            AppSettings appSettings)
        {
            _connectionRepository = connectionRepository;
            _appSettings = appSettings;
        }

        public async Task<PlatformConnection> ConnectPlatformAsync(Guid userId, PlatformType platform, string authCode)
        {
            var (accessToken, refreshToken, expiry) = await GetPlatformTokensAsync(platform, authCode);
            var platformUser = await GetPlatformUserInfoAsync(platform, accessToken);

            var connection = new PlatformConnection
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                Platform = platform,
                PlatformUserId = platformUser.Id,
                PlatformUsername = platformUser.Username,
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                TokenExpiry = expiry,
                IsPrimary = false,
                IsActive = true,
                ConnectedAt = DateTime.UtcNow,
                SyncStatus = "Connected"
            };

            await _connectionRepository.AddAsync(connection);
            return connection;
        }

        public async Task<bool> DisconnectPlatformAsync(Guid userId, Guid connectionId)
        {
            var connection = await _connectionRepository.GetByIdAsync(connectionId);
            if (connection == null || connection.UserId != userId)
                return false;

            return await _connectionRepository.DeactivateConnectionAsync(connectionId);
        }

        public async Task<PlatformConnection> GetConnectionByIdAsync(Guid userId, Guid connectionId)
        {
            var connection = await _connectionRepository.GetByIdAsync(connectionId);
            return connection?.UserId == userId ? connection : null;
        }

        public async Task<List<PlatformConnection>> GetUserConnectionsAsync(Guid userId)
        {
            return await _connectionRepository.GetByUserIdAsync(userId);
        }

        public async Task<bool> RefreshTokenAsync(Guid userId, Guid connectionId)
        {
            var connection = await _connectionRepository.GetByIdAsync(connectionId);
            if (connection == null || connection.UserId != userId)
                return false;

            var (accessToken, refreshToken, expiry) = await RefreshPlatformTokenAsync(
                connection.Platform, 
                connection.RefreshToken);

            return await _connectionRepository.UpdateTokensAsync(
                connectionId, 
                accessToken, 
                refreshToken, 
                expiry);
        }

        public async Task<bool> SetPrimaryPlatformAsync(Guid userId, Guid connectionId)
        {
            var connection = await _connectionRepository.GetByIdAsync(connectionId);
            if (connection == null || connection.UserId != userId)
                return false;

            return await _connectionRepository.SetPrimaryAsync(userId, connectionId);
        }

        public async Task<bool> SyncPlatformDataAsync(Guid userId, Guid connectionId)
        {
            var connection = await _connectionRepository.GetByIdAsync(connectionId);
            if (connection == null || connection.UserId != userId)
                return false;

            try
            {
                // Implement platform-specific sync logic here
                await _connectionRepository.UpdateSyncStatusAsync(connectionId, "Synced");
                await _connectionRepository.UpdateAsync(connection);
                return true;
            }
            catch
            {
                await _connectionRepository.UpdateSyncStatusAsync(connectionId, "Failed");
                return false;
            }
        }

        public async Task<bool> ValidateConnectionAsync(Guid userId, Guid connectionId)
        {
            var connection = await _connectionRepository.GetByIdAsync(connectionId);
            if (connection == null || connection.UserId != userId)
                return false;

            if (connection.TokenExpiry <= DateTime.UtcNow)
            {
                return await RefreshTokenAsync(userId, connectionId);
            }

            return true;
        }

        public string GetAuthUrlAsync(PlatformType platform)
        {
            return platform switch
            {
                PlatformType.Twitch => GetTwitchAuthUrl(),
                PlatformType.YouTube => GetYouTubeAuthUrl(),
                PlatformType.Facebook => GetFacebookAuthUrl(),
                PlatformType.TikTok => GetTikTokAuthUrl(),
                PlatformType.Instagram => GetInstagramAuthUrl(),
                _ => throw new NotSupportedException($"Platform {platform} is not supported")
            };
        }

        public async Task<bool> UpdatePlatformSettingsAsync(Guid userId, Guid connectionId, Dictionary<string, object> settings)
        {
            var connection = await _connectionRepository.GetByIdAsync(connectionId);
            if (connection == null || connection.UserId != userId)
                return false;

            var settingsJson = JsonSerializer.Serialize(settings);
            return await _connectionRepository.UpdatePlatformSettingsAsync(connectionId, settingsJson);
        }

        public async Task<Dictionary<string, object>> GetPlatformSettingsAsync(Guid userId, Guid connectionId)
        {
            var connection = await _connectionRepository.GetByIdAsync(connectionId);
            if (connection == null || connection.UserId != userId)
                return null;

            return JsonSerializer.Deserialize<Dictionary<string, object>>(connection.SyncStatus);
        }

        private async Task<(string accessToken, string refreshToken, DateTime? expiry)> GetPlatformTokensAsync(
            PlatformType platform, 
            string authCode)
        {
            // Implement platform-specific OAuth token exchange
            // This would typically involve making HTTP requests to the platform's OAuth endpoints
            throw new NotImplementedException();
        }

        private async Task<(string accessToken, string refreshToken, DateTime? expiry)> RefreshPlatformTokenAsync(
            PlatformType platform, 
            string refreshToken)
        {
            // Implement platform-specific token refresh
            // This would typically involve making HTTP requests to the platform's OAuth endpoints
            throw new NotImplementedException();
        }

        private async Task<PlatformUserInfo> GetPlatformUserInfoAsync(
            PlatformType platform, 
            string accessToken)
        {
            // Implement platform-specific user info retrieval
            // This would typically involve making HTTP requests to the platform's API
            throw new NotImplementedException();
        }

        private string GetTwitchAuthUrl()
        {
            var settings = _appSettings.Platforms.Twitch;
            return $"https://id.twitch.tv/oauth2/authorize" +
                   $"?client_id={settings.ClientId}" +
                   $"&redirect_uri={Uri.EscapeDataString(settings.RedirectUri)}" +
                   $"&response_type=code" +
                   $"&scope=user:read:email+channel:read:subscriptions";
        }

        private string GetYouTubeAuthUrl()
        {
            var settings = _appSettings.Platforms.YouTube;
            return $"https://accounts.google.com/o/oauth2/v2/auth" +
                   $"?client_id={settings.ClientId}" +
                   $"&redirect_uri={Uri.EscapeDataString(settings.RedirectUri)}" +
                   $"&response_type=code" +
                   $"&scope=https://www.googleapis.com/auth/youtube.readonly";
        }

        private string GetFacebookAuthUrl()
        {
            var settings = _appSettings.Platforms.Facebook;
            return $"https://www.facebook.com/v12.0/dialog/oauth" +
                   $"?client_id={settings.AppId}" +
                   $"&redirect_uri={Uri.EscapeDataString(settings.RedirectUri)}" +
                   $"&response_type=code" +
                   $"&scope=email+public_profile";
        }

        private string GetTikTokAuthUrl()
        {
            var settings = _appSettings.Platforms.TikTok;
            return $"https://www.tiktok.com/auth/authorize" +
                   $"?client_id={settings.ClientId}" +
                   $"&redirect_uri={Uri.EscapeDataString(settings.RedirectUri)}" +
                   $"&response_type=code" +
                   $"&scope=user.info.basic";
        }

        private string GetInstagramAuthUrl()
        {
            var settings = _appSettings.Platforms.Instagram;
            return $"https://api.instagram.com/oauth/authorize" +
                   $"?client_id={settings.ClientId}" +
                   $"&redirect_uri={Uri.EscapeDataString(settings.RedirectUri)}" +
                   $"&response_type=code" +
                   $"&scope=user_profile";
        }
    }

    public class PlatformUserInfo
    {
        public string Id { get; set; }
        public string Username { get; set; }
    }
} 