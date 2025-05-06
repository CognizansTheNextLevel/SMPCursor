using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Brand;

namespace ClientTrackPro.Core.Interfaces.Repositories
{
    public interface IBrandAssetRepository : IRepository<BrandAsset>
    {
        Task<List<BrandAsset>> GetByUserIdAsync(Guid userId);
        Task<List<BrandAsset>> GetByTypeAsync(Guid userId, AssetType type);
        Task<bool> UpdateSyncStatusAsync(Guid assetId, string status);
        Task<bool> UpdateThumbnailAsync(Guid assetId, string thumbnailUrl);
        Task<bool> DeactivateAssetAsync(Guid assetId);
        Task<List<BrandAsset>> GetActiveAssetsAsync(Guid userId);
        Task<bool> UpdatePlatformSyncStatusAsync(Guid assetId, string platformId, string status);
    }
} 