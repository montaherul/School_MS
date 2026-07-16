namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IWebhookSignatureValidator
{
    bool ValidateSignature(string payload, string signature, string secretKey);
    string GenerateSignature(string payload, string secretKey);
}
