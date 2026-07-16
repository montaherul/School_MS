using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IWebhookService
{
    Task<bool> ProcessWebhookAsync(
        string providerCode,
        string? transactionReference,
        string? providerEventType,
        string rawPayload,
        CancellationToken ct = default);
    Task<List<SchoolPayWebhookDto>> GetRecentWebhooksAsync(int count = 50, CancellationToken ct = default);
    Task<bool> RetryWebhookAsync(int webhookId, CancellationToken ct = default);
}
