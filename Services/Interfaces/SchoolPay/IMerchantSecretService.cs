using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IMerchantSecretService
{
    Task<List<SchoolPaySecretKeyDto>> GetSecretsAsync(int providerId, CancellationToken ct = default);
    Task RotateSecretAsync(int providerId, string keyName, string newValue, string rotatedBy, CancellationToken ct = default);
    Task<string?> DecryptSecretAsync(int providerId, string keyName, CancellationToken ct = default);
    Task<string> EncryptValueAsync(string plainText);
    string DecryptValue(string cipherText);
}
