using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClientTrackPro.Core.Models.Platform;
using ClientTrackPro.Core.Interfaces.Repositories;

namespace ClientTrackPro.Core.Repositories
{
    public class PlatformConnectionRepository : BaseRepository<PlatformConnection>, IPlatformConnectionRepository
    {
        public PlatformConnectionRepository(DbContext context) : base(context)
        {
        }

        public async Task<List<PlatformConnection>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(c => c.UserId == userId && c.IsActive)
                .OrderByDescending(c => c.ConnectedAt)
                .ToListAsync();
        }

        public async Task<PlatformConnection> GetByPlatformAsync(Guid userId, PlatformType platform)
        {
            return await _dbSet
                .FirstOrDefaultAsync(c => 
                    c.UserId == userId && 
                    c.Platform == platform && 
                    c.IsActive);
        }

        public async Task<bool> UpdateTokensAsync(Guid connectionId, string accessToken, string refreshToken, DateTime? expiry)
        {
            var connection = await GetByIdAsync(connectionId);
            if (connection == null)
                return false;

            connection.AccessToken = accessToken;
            connection.RefreshToken = refreshToken;
            connection.TokenExpiry = expiry;
            connection.LastSyncedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> SetPrimaryAsync(Guid userId, Guid connectionId)
        {
            // First, unset any existing primary connection
            var existingPrimary = await _dbSet
                .FirstOrDefaultAsync(c => c.UserId == userId && c.IsPrimary);

            if (existingPrimary != null)
            {
                existingPrimary.IsPrimary = false;
                await _context.SaveChangesAsync();
            }

            // Set the new primary connection
            var connection = await GetByIdAsync(connectionId);
            if (connection == null || connection.UserId != userId)
                return false;

            connection.IsPrimary = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateSyncStatusAsync(Guid connectionId, string status)
        {
            var connection = await GetByIdAsync(connectionId);
            if (connection == null)
                return false;

            connection.SyncStatus = status;
            connection.LastSyncedAt = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<PlatformConnection>> GetActiveConnectionsAsync(Guid userId)
        {
            return await _dbSet
                .Where(c => c.UserId == userId && c.IsActive)
                .OrderByDescending(c => c.ConnectedAt)
                .ToListAsync();
        }

        public async Task<bool> DeactivateConnectionAsync(Guid connectionId)
        {
            var connection = await GetByIdAsync(connectionId);
            if (connection == null)
                return false;

            connection.IsActive = false;
            connection.AccessToken = null;
            connection.RefreshToken = null;
            connection.TokenExpiry = null;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdatePlatformSettingsAsync(Guid connectionId, string settings)
        {
            var connection = await GetByIdAsync(connectionId);
            if (connection == null)
                return false;

            connection.SyncStatus = settings;
            await _context.SaveChangesAsync();
            return true;
        }
    }
} 