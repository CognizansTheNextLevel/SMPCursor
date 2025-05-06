using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.IO;
using ClientTrackPro.Core.Models.Brand;
using ClientTrackPro.Core.Interfaces.Services;
using ClientTrackPro.Core.Interfaces.Repositories;
using ClientTrackPro.Core.Configuration;

namespace ClientTrackPro.Core.Services
{
    public class BrandService : IBrandService
    {
        private readonly IBrandAssetRepository _assetRepository;
        private readonly IPlatformService _platformService;
        private readonly AppSettings _appSettings;

        public BrandService(
            IBrandAssetRepository assetRepository,
            IPlatformService platformService,
            AppSettings appSettings)
        {
            _assetRepository = assetRepository;
            _platformService = platformService;
            _appSettings = appSettings;
        }

        public async Task<BrandAsset> UploadAssetAsync(Guid userId, BrandAsset asset, byte[] fileData)
        {
            if (!await ValidateAssetAsync(asset))
                throw new InvalidOperationException("Invalid asset data");

            // Generate unique filename
            var fileExtension = Path.GetExtension(asset.FileUrl);
            var fileName = $"{Guid.NewGuid()}{fileExtension}";
            var fileUrl = await UploadToStorageAsync(fileName, fileData);

            asset.Id = Guid.NewGuid();
            asset.UserId = userId;
            asset.FileUrl = fileUrl;
            asset.CreatedAt = DateTime.UtcNow;
            asset.IsActive = true;

            await _assetRepository.AddAsync(asset);

            // Generate thumbnail if needed
            if (asset.Type == AssetType.VideoClip || asset.Type == AssetType.ProfilePicture)
            {
                asset.ThumbnailUrl = await GenerateThumbnailAsync(userId, asset.Id);
                await _assetRepository.UpdateAsync(asset);
            }

            return asset;
        }

        public async Task<bool> DeleteAssetAsync(Guid userId, Guid assetId)
        {
            var asset = await _assetRepository.GetByIdAsync(assetId);
            if (asset == null || asset.UserId != userId)
                return false;

            return await _assetRepository.DeactivateAssetAsync(assetId);
        }

        public async Task<BrandAsset> GetAssetByIdAsync(Guid userId, Guid assetId)
        {
            var asset = await _assetRepository.GetByIdAsync(assetId);
            return asset?.UserId == userId ? asset : null;
        }

        public async Task<List<BrandAsset>> GetAssetsByTypeAsync(Guid userId, AssetType type)
        {
            return await _assetRepository.GetByTypeAsync(userId, type);
        }

        public async Task<List<BrandAsset>> GetAllAssetsAsync(Guid userId)
        {
            return await _assetRepository.GetByUserIdAsync(userId);
        }

        public async Task<bool> UpdateAssetAsync(Guid userId, BrandAsset asset)
        {
            var existingAsset = await _assetRepository.GetByIdAsync(asset.Id);
            if (existingAsset == null || existingAsset.UserId != userId)
                return false;

            existingAsset.Name = asset.Name;
            existingAsset.Description = asset.Description;
            existingAsset.LastUpdated = DateTime.UtcNow;

            await _assetRepository.UpdateAsync(existingAsset);
            return true;
        }

        public async Task<bool> SyncAssetsToPlatformAsync(Guid userId, string platformId)
        {
            var assets = await _assetRepository.GetActiveAssetsAsync(userId);
            var success = true;

            foreach (var asset in assets)
            {
                try
                {
                    // Implement platform-specific sync logic here
                    await _assetRepository.UpdatePlatformSyncStatusAsync(asset.Id, platformId, "Synced");
                }
                catch
                {
                    await _assetRepository.UpdatePlatformSyncStatusAsync(asset.Id, platformId, "Failed");
                    success = false;
                }
            }

            return success;
        }

        public async Task<bool> SyncAssetsToAllPlatformsAsync(Guid userId)
        {
            var connections = await _platformService.GetUserConnectionsAsync(userId);
            var success = true;

            foreach (var connection in connections)
            {
                if (!await SyncAssetsToPlatformAsync(userId, connection.Id.ToString()))
                    success = false;
            }

            return success;
        }

        public async Task<string> GenerateThumbnailAsync(Guid userId, Guid assetId)
        {
            var asset = await _assetRepository.GetByIdAsync(assetId);
            if (asset == null || asset.UserId != userId)
                return null;

            // Implement thumbnail generation logic here
            // This would typically involve using a media processing library
            var thumbnailUrl = $"{_appSettings.Storage.BaseUrl}/thumbnails/{assetId}.jpg";
            
            await _assetRepository.UpdateThumbnailAsync(assetId, thumbnailUrl);
            return thumbnailUrl;
        }

        public async Task<bool> ValidateAssetAsync(BrandAsset asset)
        {
            if (string.IsNullOrEmpty(asset.Name))
                return false;

            if (asset.Type == AssetType.Other && string.IsNullOrEmpty(asset.Description))
                return false;

            // Add more validation rules as needed
            return true;
        }

        private async Task<string> UploadToStorageAsync(string fileName, byte[] fileData)
        {
            // Implement file upload logic here
            // This would typically involve using Azure Blob Storage or similar
            return $"{_appSettings.Storage.BaseUrl}/{_appSettings.Storage.ContainerName}/{fileName}";
        }
    }
} 