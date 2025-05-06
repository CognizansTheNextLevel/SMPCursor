using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Platform;

namespace ClientTrackPro.Core.Interfaces.Services
{
    public interface IPlatformService
    {
        Task<PlatformConnection> ConnectPlatformAsync(Guid userId, PlatformType platform, string authCode);
        Task<bool> DisconnectPlatformAsync(Guid userId, Guid connectionId);
        Task<PlatformConnection> GetConnectionByIdAsync(Guid userId, Guid connectionId);
        Task<List<PlatformConnection>> GetUserConnectionsAsync(Guid userId);
        Task<bool> RefreshTokenAsync(Guid userId, Guid connectionId);
        Task<bool> SetPrimaryPlatformAsync(Guid userId, Guid connectionId);
        Task<bool> SyncPlatformDataAsync(Guid userId, Guid connectionId);
        Task<bool> ValidateConnectionAsync(Guid userId, Guid connectionId);
        Task<string> GetAuthUrlAsync(PlatformType platform);
        Task<bool> UpdatePlatformSettingsAsync(Guid userId, Guid connectionId, Dictionary<string, object> settings);
        Task<Dictionary<string, object>> GetPlatformSettingsAsync(Guid userId, Guid connectionId);
    }
} 