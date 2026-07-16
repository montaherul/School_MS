using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Entities.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class WebhookService : IWebhookService
{
    private readonly ISchoolPayRepository _repository;
    private readonly GatewayFactory _gatewayFactory;
    private readonly ILogger<WebhookService> _logger;

    public WebhookService(
        ISchoolPayRepository repository,
        GatewayFactory gatewayFactory,
        ILogger<WebhookService> logger)
    {
        _repository = repository;
        _gatewayFactory = gatewayFactory;
        _logger = logger;
    }

    public async Task<bool> ProcessWebhookAsync(
        string providerCode,
        string? transactionReference,
        string? providerEventType,
        string rawPayload,
        CancellationToken ct = default)
    {
        var provider = await _repository.GetProviderEntityByCodeAsync(providerCode, ct);
        if (provider == null)
        {
            _logger.LogWarning("Webhook received for unknown provider: {Provider}", providerCode);
            return false;
        }

        var webhook = new PaymentGatewayWebhook
        {
            PaymentProviderId = provider.Id,
            TransactionReference = transactionReference,
            ProviderEventType = providerEventType,
            RawPayload = rawPayload,
            Status = SchoolPayWebhookStatus.Received,
            ReceivedAt = DateTime.UtcNow
        };
        await _repository.CreateWebhookAsync(webhook, ct);

        var gatewayProvider = _gatewayFactory.GetProvider(providerCode);
        if (gatewayProvider == null)
        {
            webhook.Status = SchoolPayWebhookStatus.Failed;
            webhook.ErrorMessage = $"No gateway implementation for {providerCode}";
            await _repository.UpdateWebhookAsync(webhook, ct);
            return false;
        }

        webhook.Status = SchoolPayWebhookStatus.Processing;
        await _repository.UpdateWebhookAsync(webhook, ct);

        try
        {
            var ipnResult = await gatewayProvider.ProcessIpnAsync(
                null, transactionReference, null, "VALID", ct);

            webhook.Status = ipnResult.Success ? SchoolPayWebhookStatus.Processed : SchoolPayWebhookStatus.Failed;
            webhook.ErrorMessage = ipnResult.Success ? null : ipnResult.ErrorMessage;
            webhook.ProcessedAt = DateTime.UtcNow;
            await _repository.UpdateWebhookAsync(webhook, ct);
            return ipnResult.Success;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Webhook processing failed for provider {Provider}", providerCode);
            webhook.Status = SchoolPayWebhookStatus.Failed;
            webhook.ErrorMessage = ex.Message;
            webhook.ProcessedAt = DateTime.UtcNow;
            await _repository.UpdateWebhookAsync(webhook, ct);
            return false;
        }
    }

    public async Task<List<SchoolPayWebhookDto>> GetRecentWebhooksAsync(int count = 50, CancellationToken ct = default)
        => await _repository.GetRecentWebhooksAsync(count, ct);

    public async Task<bool> RetryWebhookAsync(int webhookId, CancellationToken ct = default)
    {
        var webhook = await _repository.GetWebhookEntityByIdAsync(webhookId, ct);
        if (webhook == null) return false;

        var provider = await _repository.GetProviderEntityByIdAsync(webhook.PaymentProviderId, ct);
        if (provider == null) return false;

        var gatewayProvider = _gatewayFactory.GetProvider(provider.Code);
        if (gatewayProvider == null) return false;

        webhook.AttemptCount++;
        webhook.Status = SchoolPayWebhookStatus.Processing;

        try
        {
            var ipnResult = await gatewayProvider.ProcessIpnAsync(
                null, webhook.TransactionReference, null, "VALID", ct);
            webhook.Status = ipnResult.Success ? SchoolPayWebhookStatus.Processed : SchoolPayWebhookStatus.Failed;
            webhook.ErrorMessage = ipnResult.Success ? null : ipnResult.ErrorMessage;
            webhook.ProcessedAt = DateTime.UtcNow;
            await _repository.UpdateWebhookAsync(webhook, ct);
            return ipnResult.Success;
        }
        catch (Exception ex)
        {
            webhook.Status = SchoolPayWebhookStatus.Failed;
            webhook.ErrorMessage = ex.Message;
            webhook.ProcessedAt = DateTime.UtcNow;
            await _repository.UpdateWebhookAsync(webhook, ct);
            return false;
        }
    }
}
