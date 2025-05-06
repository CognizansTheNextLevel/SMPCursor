using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Brand;

namespace ClientTrackPro.Core.Interfaces.Services
{
    public interface IBrandService
    {
        Task<BrandAsset> UploadAssetAsync(Guid userId, BrandAsset asset, byte[] fileData);
        Task<bool> DeleteAssetAsync(Guid userId, Guid assetId);
        Task<BrandAsset> GetAssetByIdAsync(Guid userId, Guid assetId);
        Task<List<BrandAsset>> GetAssetsByTypeAsync(Guid userId, AssetType type);
        Task<List<BrandAsset>> GetAllAssetsAsync(Guid userId);
        Task<bool> UpdateAssetAsync(Guid userId, BrandAsset asset);
        Task<bool> SyncAssetsToPlatformAsync(Guid userId, string platformId);
        Task<bool> SyncAssetsToAllPlatformsAsync(Guid userId);
        Task<string> GenerateThumbnailAsync(Guid userId, Guid assetId);
        Task<bool> ValidateAssetAsync(BrandAsset asset);
    }
} 