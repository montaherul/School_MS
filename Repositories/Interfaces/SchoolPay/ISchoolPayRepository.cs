using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Entities.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolPayPaymentMethod = SchoolManagementSystem.Models.Entities.SchoolPay.PaymentMethod;

namespace SchoolManagementSystem.Repositories.Interfaces.SchoolPay;

public interface ISchoolPayRepository
{
    // ── Provider Management ──
    Task<List<SchoolPayProviderListDto>> GetAllProvidersAsync(CancellationToken ct = default);
    Task<SchoolPayProviderDto?> GetProviderByIdAsync(int id, CancellationToken ct = default);
    Task<PaymentProvider?> GetProviderEntityByIdAsync(int id, CancellationToken ct = default);
    Task<PaymentProvider?> GetProviderEntityByCodeAsync(string code, CancellationToken ct = default);
    Task<int> CreateProviderAsync(PaymentProvider provider, List<PaymentProviderConfiguration> configs, CancellationToken ct = default);
    Task UpdateProviderAsync(PaymentProvider provider, CancellationToken ct = default);
    Task UpsertProviderConfigurationAsync(int providerId, string key, string value, string updatedBy, CancellationToken ct = default);
    Task<List<SchoolPayProviderDto>> GetActiveProviderDtosAsync(CancellationToken ct = default);
    Task<List<SchoolPayProviderMethodDto>> GetPaymentMethodsAsync(int providerId, CancellationToken ct = default);
    Task<List<SchoolPayProviderMethodDto>> GetAllPaymentMethodsAsync(CancellationToken ct = default);
    Task<SchoolPayPaymentMethod?> GetPaymentMethodEntityByIdAsync(int id, CancellationToken ct = default);
    Task<SchoolPayPaymentMethod?> GetPaymentMethodEntityByCodeAsync(string code, CancellationToken ct = default);
    Task<int> CreatePaymentMethodAsync(SchoolPayPaymentMethod method, CancellationToken ct = default);
    Task UpdatePaymentMethodAsync(SchoolPayPaymentMethod method, CancellationToken ct = default);
    Task<bool> TogglePaymentMethodActiveAsync(int id, bool isActive, CancellationToken ct = default);
    Task<bool> UpdatePaymentMethodOrderAsync(int id, int displayOrder, CancellationToken ct = default);

    // ── Transactions ──
    Task<List<SchoolPayTransactionDto>> GetTransactionsPagedAsync(int page, int pageSize, string? status = null, string? providerCode = null, CancellationToken ct = default);
    Task<int> GetTransactionCountAsync(string? status = null, string? providerCode = null, CancellationToken ct = default);
    Task<PaymentGatewayTransaction?> GetTransactionEntityByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateTransactionAsync(PaymentGatewayTransaction entity, CancellationToken ct = default);

    // ── Webhooks ──
    Task<int> CreateWebhookAsync(PaymentGatewayWebhook webhook, CancellationToken ct = default);
    Task UpdateWebhookAsync(PaymentGatewayWebhook webhook, CancellationToken ct = default);
    Task<List<SchoolPayWebhookDto>> GetRecentWebhooksAsync(int count = 50, CancellationToken ct = default);
    Task<PaymentGatewayWebhook?> GetWebhookEntityByIdAsync(int id, CancellationToken ct = default);

    // ── Settlements ──
    Task<List<SchoolPaySettlementDto>> GetSettlementsAsync(CancellationToken ct = default);
    Task<SchoolPaySettlementDto?> GetSettlementByIdAsync(int id, CancellationToken ct = default);
    Task<PaymentGatewaySettlement?> GetSettlementEntityByIdAsync(int id, CancellationToken ct = default);
    Task UpdateSettlementAsync(PaymentGatewaySettlement settlement, CancellationToken ct = default);
    Task<int> CreateSettlementAsync(PaymentGatewaySettlement entity, CancellationToken ct = default);

    // ── Refunds ──
    Task<int> CreateRefundAsync(PaymentGatewayRefund refund, CancellationToken ct = default);
    Task<List<SchoolPayRefundDto>> GetRefundsAsync(CancellationToken ct = default);

    // ── Dashboard ──
    Task<SchoolPayDashboardDto> GetDashboardDataAsync(CancellationToken ct = default);

    // ── Audit ──
    Task<bool> LogAuditEventAsync(int? transactionId, string eventType, string? eventData, string? performedBy, string? ipAddress, CancellationToken ct = default);

    // ── Health ──
    Task RecordHealthCheckAsync(int providerId, ProviderHealthStatus status, int responseTimeMs, decimal successRate, int totalRequests, int failedRequests, string? lastError, CancellationToken ct = default);

    // ═══════════════════════════════════════════
    //  SP-02: HEALTH MONITOR
    // ═══════════════════════════════════════════
    Task<List<SchoolPayHealthStatusDto>> GetLatestHealthStatusAsync(CancellationToken ct = default);
    Task<List<SchoolPayHealthHistoryDto>> GetHealthHistoryAsync(int providerId, int days = 30, CancellationToken ct = default);

    // ═══════════════════════════════════════════
    //  SP-02: ROUTING RULES
    // ═══════════════════════════════════════════
    Task<List<SchoolPayRouteRuleDto>> GetAllRouteRulesAsync(CancellationToken ct = default);
    Task<PaymentRouteRule?> GetRouteRuleEntityByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateRouteRuleAsync(PaymentRouteRule rule, CancellationToken ct = default);
    Task UpdateRouteRuleAsync(PaymentRouteRule rule, CancellationToken ct = default);
    Task DeleteRouteRuleAsync(int id, CancellationToken ct = default);
    Task<List<SchoolPayRouteRuleDto>> GetActiveRouteRulesForAmountAsync(decimal amount, string? feeType, CancellationToken ct = default);

    // ═══════════════════════════════════════════
    //  SP-02: WEBHOOK QUEUE
    // ═══════════════════════════════════════════
    Task<List<PaymentGatewayWebhook>> GetPendingWebhooksForRetryAsync(int maxRetries = 3, int batchSize = 10, CancellationToken ct = default);

    // ═══════════════════════════════════════════
    //  SP-02: RECONCILIATION
    // ═══════════════════════════════════════════
    Task<List<PaymentGatewayTransaction>> GetTransactionsByDateRangeAsync(DateTime from, DateTime to, int? providerId, CancellationToken ct = default);
    Task<List<SchoolPayReconciliationResultDto>> GetReconciliationDataAsync(int settlementId, CancellationToken ct = default);

    // ═══════════════════════════════════════════
    //  SP-02: ANALYTICS
    // ═══════════════════════════════════════════
    Task<SchoolPayAnalyticsDto> GetAnalyticsAsync(int days = 30, CancellationToken ct = default);

    // ═══════════════════════════════════════════
    //  SP-03: OPERATIONS CENTER
    // ═══════════════════════════════════════════
    Task<SchoolPayOperationsDto> GetOperationsDataAsync(CancellationToken ct = default);

    // ═══════════════════════════════════════════
    //  SP-03: DEAD LETTER QUEUE
    // ═══════════════════════════════════════════
    Task<List<SchoolPayDeadLetterDto>> GetDeadLetterItemsAsync(CancellationToken ct = default);
    Task<PaymentGatewayWebhook?> GetDeadLetterEntityByIdAsync(int id, CancellationToken ct = default);
    Task ReprocessDeadLetterAsync(int id, CancellationToken ct = default);
    Task IgnoreDeadLetterAsync(int id, CancellationToken ct = default);
    Task MoveToDeadLetterAsync(int webhookId, CancellationToken ct = default);

    // ═══════════════════════════════════════════
    //  SP-03: PAYMENT TIMELINE
    // ═══════════════════════════════════════════
    Task<List<SchoolPayTimelineEntryDto>> GetTransactionTimelineAsync(int transactionId, CancellationToken ct = default);
    Task<SchoolPayTimelineDto?> GetFullTimelineAsync(int transactionId, CancellationToken ct = default);

    // ═══════════════════════════════════════════
    //  SP-04: SECURITY AUDIT
    // ═══════════════════════════════════════════
    Task<List<SchoolPaySecurityAuditEntryDto>> GetSecurityAuditLogAsync(int? providerId = null, int days = 30, CancellationToken ct = default);
    Task LogSecurityEventAsync(PaymentSecurityEventType eventType, string? details, string? performedBy, string? ipAddress, CancellationToken ct = default);

    // ═══════════════════════════════════════════
    //  SP-04: MONITORING
    // ═══════════════════════════════════════════
    Task<SchoolPayMonitoringDto> GetMonitoringDataAsync(CancellationToken ct = default);

    // ═══════════════════════════════════════════
    //  SP-04: SECRET STORE (for encrypted configs)
    // ═══════════════════════════════════════════
    Task<PaymentProviderConfiguration?> GetConfigByKeyAsync(int providerId, string key, CancellationToken ct = default);
    Task UpdateConfigValueAsync(int providerId, string key, string encryptedValue, string updatedBy, CancellationToken ct = default);
    Task<List<SchoolPaySecretKeyDto>> GetSecretKeysAsync(int providerId, CancellationToken ct = default);
}
