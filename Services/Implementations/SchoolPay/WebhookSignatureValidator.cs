using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class WebhookSignatureValidator : IWebhookSignatureValidator
{
    private readonly ILogger<WebhookSignatureValidator> _logger;

    public WebhookSignatureValidator(ILogger<WebhookSignatureValidator> logger)
    {
        _logger = logger;
    }

    public bool ValidateSignature(string payload, string signature, string secretKey)
    {
        if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(secretKey))
            return false;

        try
        {
            var expected = GenerateSignature(payload, secretKey);
            var result = CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(expected),
                Encoding.UTF8.GetBytes(signature));
            return result;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Signature validation failed");
            return false;
        }
    }

    public string GenerateSignature(string payload, string secretKey)
    {
        using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(secretKey));
        var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(payload));
        return Convert.ToHexString(hash).ToLowerInvariant();
    }
}
