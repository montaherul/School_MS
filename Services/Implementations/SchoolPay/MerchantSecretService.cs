using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class MerchantSecretService : IMerchantSecretService
{
    private readonly ISchoolPayRepository _repo;
    private readonly ISecurityAuditService _audit;
    private readonly ILogger<MerchantSecretService> _logger;
    private static readonly byte[] Key = Encoding.UTF8.GetBytes("Sch00lP@yS3cr3tK3y!2024#AES256!!"[..32]);

    public MerchantSecretService(ISchoolPayRepository repo, ISecurityAuditService audit, ILogger<MerchantSecretService> logger)
    {
        _repo = repo;
        _audit = audit;
        _logger = logger;
    }

    public async Task<List<SchoolPaySecretKeyDto>> GetSecretsAsync(int providerId, CancellationToken ct = default)
        => await _repo.GetSecretKeysAsync(providerId, ct);

    public async Task RotateSecretAsync(int providerId, string keyName, string newValue, string rotatedBy, CancellationToken ct = default)
    {
        var encrypted = await EncryptValueAsync(newValue);
        await _repo.UpdateConfigValueAsync(providerId, keyName, encrypted, rotatedBy, ct);

        await _audit.LogSecurityEventAsync(
            PaymentSecurityEventType.SecretRotated,
            $"Secret '{keyName}' rotated for provider {providerId}",
            rotatedBy, null, ct);

        _logger.LogInformation("Secret {KeyName} rotated for provider {ProviderId} by {User}", keyName, providerId, rotatedBy);
    }

    public async Task<string?> DecryptSecretAsync(int providerId, string keyName, CancellationToken ct = default)
    {
        var config = await _repo.GetConfigByKeyAsync(providerId, keyName, ct);
        if (config == null) return null;
        return DecryptValue(config.Value);
    }

    public async Task<string> EncryptValueAsync(string plainText)
    {
        return await Task.Run(() => EncryptValue(plainText));
    }

    public string DecryptValue(string cipherText)
    {
        try
        {
            var combined = Convert.FromBase64String(cipherText);
            using var aes = Aes.Create();
            aes.Key = Key;
            var iv = combined[..16];
            var cipher = combined[16..];
            aes.IV = iv;
            using var decryptor = aes.CreateDecryptor();
            var result = decryptor.TransformFinalBlock(cipher, 0, cipher.Length);
            return Encoding.UTF8.GetString(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Decryption failed");
            throw new InvalidOperationException("Failed to decrypt secret", ex);
        }
    }

    private string EncryptValue(string plainText)
    {
        using var aes = Aes.Create();
        aes.Key = Key;
        aes.GenerateIV();
        using var encryptor = aes.CreateEncryptor();
        var plainBytes = Encoding.UTF8.GetBytes(plainText);
        var cipher = encryptor.TransformFinalBlock(plainBytes, 0, plainBytes.Length);
        var combined = new byte[aes.IV.Length + cipher.Length];
        Buffer.BlockCopy(aes.IV, 0, combined, 0, aes.IV.Length);
        Buffer.BlockCopy(cipher, 0, combined, aes.IV.Length, cipher.Length);
        return Convert.ToBase64String(combined);
    }
}
