using System;
using System.Threading.Tasks;
using ClientTrackPro.Core.Models.Account;

namespace ClientTrackPro.Core.Interfaces.Services
{
    public interface IDataProtectionService
    {
        // Data encryption
        Task<string> EncryptSensitiveDataAsync(string data);
        Task<string> DecryptSensitiveDataAsync(string encryptedData);
        
        // Data anonymization
        Task<string> AnonymizeUserDataAsync(User user);
        Task<string> PseudonymizeUserDataAsync(User user);
        
        // Data retention
        Task<bool> DeleteUserDataAsync(Guid userId);
        Task<bool> ArchiveUserDataAsync(Guid userId);
        
        // Data export
        Task<string> ExportUserDataAsync(Guid userId);
        Task<string> GeneratePrivacyReportAsync(Guid userId);
        
        // Consent management
        Task<bool> UpdateUserConsentAsync(Guid userId, string consentType, bool granted);
        Task<bool> HasUserConsentAsync(Guid userId, string consentType);
        
        // Data access logs
        Task LogDataAccessAsync(Guid userId, string accessType, string purpose);
        Task<string> GetDataAccessLogsAsync(Guid userId);
    }
} 