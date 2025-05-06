using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using ClientTrackPro.Core.Models.Brand;
using ClientTrackPro.Core.Interfaces.Repositories;

namespace ClientTrackPro.Core.Repositories
{
    public class BrandAssetRepository : BaseRepository<BrandAsset>, IBrandAssetRepository
    {
        public BrandAssetRepository(DbContext context) : base(context)
        {
        }

        public async Task<List<BrandAsset>> GetByUserIdAsync(Guid userId)
        {
            return await _dbSet
                .Where(a => a.UserId == userId && a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<List<BrandAsset>> GetByTypeAsync(Guid userId, AssetType type)
        {
            return await _dbSet
                .Where(a => a.UserId == userId && a.Type == type && a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdateSyncStatusAsync(Guid assetId, string status)
        {
            var asset = await GetByIdAsync(assetId);
            if (asset == null)
                return false;

            asset.PlatformSyncStatus = status;
            asset.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateThumbnailAsync(Guid assetId, string thumbnailUrl)
        {
            var asset = await GetByIdAsync(assetId);
            if (asset == null)
                return false;

            asset.ThumbnailUrl = thumbnailUrl;
            asset.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeactivateAssetAsync(Guid assetId)
        {
            var asset = await GetByIdAsync(assetId);
            if (asset == null)
                return false;

            asset.IsActive = false;
            asset.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<List<BrandAsset>> GetActiveAssetsAsync(Guid userId)
        {
            return await _dbSet
                .Where(a => a.UserId == userId && a.IsActive)
                .OrderByDescending(a => a.CreatedAt)
                .ToListAsync();
        }

        public async Task<bool> UpdatePlatformSyncStatusAsync(Guid assetId, string platformId, string status)
        {
            var asset = await GetByIdAsync(assetId);
            if (asset == null)
                return false;

            // Update the sync status for the specific platform
            // This could be implemented as a JSON field or a separate table
            asset.PlatformSyncStatus = $"{platformId}:{status}";
            asset.LastUpdated = DateTime.UtcNow;

            await _context.SaveChangesAsync();
            return true;
        }
    }
} 