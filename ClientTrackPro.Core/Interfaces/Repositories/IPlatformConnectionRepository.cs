using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Platform;

namespace ClientTrackPro.Core.Interfaces.Repositories
{
    public interface IPlatformConnectionRepository : IRepository<PlatformConnection>
    {
        Task<List<PlatformConnection>> GetByUserIdAsync(Guid userId);
        Task<PlatformConnection> GetByPlatformAsync(Guid userId, PlatformType platform);
        Task<bool> UpdateTokensAsync(Guid connectionId, string accessToken, string refreshToken, DateTime? expiry);
        Task<bool> SetPrimaryAsync(Guid userId, Guid connectionId);
        Task<bool> UpdateSyncStatusAsync(Guid connectionId, string status);
        Task<List<PlatformConnection>> GetActiveConnectionsAsync(Guid userId);
        Task<bool> DeactivateConnectionAsync(Guid connectionId);
        Task<bool> UpdatePlatformSettingsAsync(Guid connectionId, string settings);
    }
} 