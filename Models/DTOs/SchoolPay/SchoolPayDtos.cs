using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.SchoolPay;

public class SchoolPayCheckoutRequestDto
{
    public string InvoiceNo { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string? Description { get; set; }
    public string? ReturnUrl { get; set; }
    public string? CancelUrl { get; set; }
}

public class SchoolPayCheckoutResponseDto
{
    public bool Success { get; set; }
    public string? CheckoutUrl { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public int PaymentGatewayTransactionId { get; set; }
    public string? ErrorMessage { get; set; }
}

public class SchoolPayProviderDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public string? Description { get; set; }
    public bool IsActive { get; set; }
    public ProviderStatus Status { get; set; }
    public int Priority { get; set; }
    public List<SchoolPayProviderMethodDto> Methods { get; set; } = new();
}

public class SchoolPayProviderMethodDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public int PaymentProviderId { get; set; }
    public string? ProviderName { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; }
    public bool IsDefault { get; set; }
    public bool IsRecommended { get; set; }
    public bool IsPopular { get; set; }
    public int PopularityRank { get; set; }
    public string? BackgroundColor { get; set; }
    public string? TextColor { get; set; }
    public string? Icon { get; set; }
    public string? CssClass { get; set; }
}

public class SchoolPayMethodUpsertDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public int PaymentProviderId { get; set; }
    public int DisplayOrder { get; set; }
    public bool IsDefault { get; set; }
    public bool IsRecommended { get; set; }
    public bool IsPopular { get; set; }
    public int PopularityRank { get; set; }
    public string? BackgroundColor { get; set; }
    public string? TextColor { get; set; }
    public string? Icon { get; set; }
    public string? CssClass { get; set; }
}

public class SchoolPayTransactionDto
{
    public int Id { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string? MethodName { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public string? ProviderTransactionId { get; set; }
    public string? BankTransactionId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BDT";
    public decimal? FeeAmount { get; set; }
    public SchoolPayTransactionStatus Status { get; set; }
    public string? StatusMessage { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? InitiatedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
}

public class SchoolPayProviderListDto
{
    public int Id { get; set; }
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? LogoUrl { get; set; }
    public ProviderStatus Status { get; set; }
    public bool IsSandbox { get; set; }
    public int Priority { get; set; }
    public bool IsActive { get; set; }
    public int MethodCount { get; set; }
    public ProviderHealthStatus? HealthStatus { get; set; }
}

public class SchoolPayProviderUpsertDto
{
    public string Code { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? LogoUrl { get; set; }
    public bool IsSandbox { get; set; } = true;
    public bool SupportsRefund { get; set; }
    public bool SupportsSettlement { get; set; }
    public int MaxRetryAttempts { get; set; } = 3;
    public string? SupportedCurrencies { get; set; }
    public string? ClassName { get; set; }
    public int Priority { get; set; }
    public Dictionary<string, string> Configurations { get; set; } = new();
}

public class SchoolPayWebhookDto
{
    public int Id { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public string? ProviderEventType { get; set; }
    public SchoolPayWebhookStatus Status { get; set; }
    public string? ErrorMessage { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DateTime? ProcessedAt { get; set; }
}

public class SchoolPaySettlementDto
{
    public int Id { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string SettlementReference { get; set; } = string.Empty;
    public string? ProviderSettlementId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = "BDT";
    public SettlementStatus Status { get; set; }
    public DateTime? SettlementDate { get; set; }
    public string? Remarks { get; set; }
}

public class SchoolPayRefundDto
{
    public int Id { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public string RefundReference { get; set; } = string.Empty;
    public string? ProviderRefundId { get; set; }
    public decimal RefundAmount { get; set; }
    public string? Reason { get; set; }
    public RefundStatus Status { get; set; }
    public DateTime? RefundedAt { get; set; }
}

public class SchoolPayInitResult
{
    public bool Success { get; set; }
    public string? GatewayPageUrl { get; set; }
    public string? TransactionReference { get; set; }
    public string? SessionKey { get; set; }
    public string? ErrorMessage { get; set; }
}

public class SchoolPayVerifyResult
{
    public bool Success { get; set; }
    public SchoolPayTransactionStatus TransactionStatus { get; set; }
    public string? ProviderTransactionId { get; set; }
    public string? BankTransactionId { get; set; }
    public string? CardType { get; set; }
    public decimal? Amount { get; set; }
    public string? Currency { get; set; }
    public string? RiskLevel { get; set; }
    public string? ErrorMessage { get; set; }
}

public class SchoolPayIpnResult
{
    public bool Success { get; set; }
    public int? OnlinePaymentRequestId { get; set; }
    public int? PaymentId { get; set; }
    public string? ErrorMessage { get; set; }
}

public class SchoolPayDashboardDto
{
    public int TotalTransactions { get; set; }
    public int CompletedTransactions { get; set; }
    public int FailedTransactions { get; set; }
    public int PendingTransactions { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal TotalFeeAmount { get; set; }
    public int ActiveProviders { get; set; }
    public int HealthyProviders { get; set; }
    public List<SchoolPayRecentTransactionDto> RecentTransactions { get; set; } = new();
}

public class SchoolPayRecentTransactionDto
{
    public string TransactionReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public SchoolPayTransactionStatus Status { get; set; }
    public DateTime? InitiatedAt { get; set; }
}

public class SchoolPayCheckoutModel
{
    public int InvoiceId { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public int StudentId { get; set; }
    public decimal Amount { get; set; }
    public List<SchoolPayProviderDto> Providers { get; set; } = new();
    public List<SchoolPayProviderMethodDto> PaymentMethods { get; set; } = new();
}

// ═══════════════════════════════════════════
//  PHASE SP-02 DTOs
// ═══════════════════════════════════════════

public class SchoolPayHealthStatusDto
{
    public int ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public ProviderHealthStatus Status { get; set; }
    public int ResponseTimeMs { get; set; }
    public decimal SuccessRate { get; set; }
    public int TotalRequests { get; set; }
    public int FailedRequests { get; set; }
    public string? LastError { get; set; }
    public DateTime? LastCheckedAt { get; set; }
    public DateTime? LastSuccessAt { get; set; }
}

public class SchoolPayHealthHistoryDto
{
    public int Id { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public ProviderHealthStatus Status { get; set; }
    public int ResponseTimeMs { get; set; }
    public string? LastError { get; set; }
    public DateTime? LastCheckedAt { get; set; }
}

public class SchoolPayRouteRuleDto
{
    public int Id { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public int PaymentProviderId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public PaymentRoutePriority Priority { get; set; }
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? FeeType { get; set; }
    public string? ConditionExpression { get; set; }
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
}

public class SchoolPayRouteRuleUpsertDto
{
    public int PaymentProviderId { get; set; }
    public string RuleName { get; set; } = string.Empty;
    public PaymentRoutePriority Priority { get; set; } = PaymentRoutePriority.Secondary;
    public decimal? MinAmount { get; set; }
    public decimal? MaxAmount { get; set; }
    public string? FeeType { get; set; }
    public string? ConditionExpression { get; set; }
    public bool IsActive { get; set; } = true;
    public int DisplayOrder { get; set; }
}

public class SchoolPayReconciliationResultDto
{
    public int SettlementId { get; set; }
    public string SettlementReference { get; set; } = string.Empty;
    public string? ProviderName { get; set; }
    public decimal SettlementAmount { get; set; }
    public decimal MatchedAmount { get; set; }
    public decimal Difference { get; set; }
    public int MatchedTransactionCount { get; set; }
    public int UnmatchedTransactionCount { get; set; }
    public List<SchoolPayReconciliationLineDto> Transactions { get; set; } = new();
}

public class SchoolPayReconciliationLineDto
{
    public int TransactionId { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public SchoolPayTransactionStatus Status { get; set; }
    public DateTime? CompletedAt { get; set; }
    public bool IsMatched { get; set; }
}

public class SchoolPayAnalyticsDto
{
    public List<SchoolPayVolumeDayDto> VolumeByDay { get; set; } = new();
    public List<SchoolPayProviderPerformanceDto> ProviderPerformance { get; set; } = new();
    public List<SchoolPaySuccessRateTrendDto> SuccessRateTrend { get; set; } = new();
    public SchoolPayAnalyticsSummaryDto Summary { get; set; } = new();
}

public class SchoolPayVolumeDayDto
{
    public string Date { get; set; } = string.Empty;
    public int TransactionCount { get; set; }
    public decimal TotalAmount { get; set; }
}

public class SchoolPayProviderPerformanceDto
{
    public string ProviderName { get; set; } = string.Empty;
    public int TotalTransactions { get; set; }
    public int SuccessfulTransactions { get; set; }
    public int FailedTransactions { get; set; }
    public decimal SuccessRate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal AvgResponseTimeMs { get; set; }
}

public class SchoolPaySuccessRateTrendDto
{
    public string Date { get; set; } = string.Empty;
    public decimal SuccessRate { get; set; }
    public decimal TotalAmount { get; set; }
}

public class SchoolPayAnalyticsSummaryDto
{
    public int TotalTransactions { get; set; }
    public int SuccessfulTransactions { get; set; }
    public int FailedTransactions { get; set; }
    public decimal TotalRevenue { get; set; }
    public decimal AverageTransactionValue { get; set; }
    public decimal OverallSuccessRate { get; set; }
    public int ActiveProviders { get; set; }
}

public class SchoolPayAnalyticsFilterDto
{
    public int Days { get; set; } = 30;
    public int? ProviderId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

// ═══════════════════════════════════════════
//  SP-03: OPERATIONS CENTER
// ═══════════════════════════════════════════

public class SchoolPayOperationsDto
{
    public int PendingPayments { get; set; }
    public int GatewayPending { get; set; }
    public int WebhookQueueSize { get; set; }
    public int SettlementPending { get; set; }
    public int FailedPayments { get; set; }
    public int RefundPending { get; set; }
    public int DisputedSettlements { get; set; }
    public int DeadLetterCount { get; set; }
    public int HealthyProviders { get; set; }
    public int DegradedProviders { get; set; }
    public int UnhealthyProviders { get; set; }
    public List<SchoolPayAlertDto> Alerts { get; set; } = new();
    public List<SchoolPayRecentTransactionDto> RecentTransactions { get; set; } = new();
}

public class SchoolPayAlertDto
{
    public string Severity { get; set; } = "info";
    public string Message { get; set; } = string.Empty;
    public string? ActionUrl { get; set; }
    public string? ActionText { get; set; }
    public DateTime Timestamp { get; set; }
}

// ═══════════════════════════════════════════
//  SP-03: DEAD LETTER QUEUE
// ═══════════════════════════════════════════

public class SchoolPayDeadLetterDto
{
    public int Id { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public string? ProviderEventType { get; set; }
    public SchoolPayWebhookStatus Status { get; set; }
    public string? RawPayload { get; set; }
    public string? ErrorMessage { get; set; }
    public int AttemptCount { get; set; }
    public DateTime? ReceivedAt { get; set; }
    public DateTime? LastAttemptAt { get; set; }
}

// ═══════════════════════════════════════════
//  SP-03: PROVIDER FAILOVER
// ═══════════════════════════════════════════

public class SchoolPayFailoverStatusDto
{
    public int ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public bool IsHealthy { get; set; }
    public bool IsPrimary { get; set; }
    public int Priority { get; set; }
    public ProviderHealthStatus HealthStatus { get; set; }
    public string? LastError { get; set; }
    public DateTime? LastCheckedAt { get; set; }
}

// ═══════════════════════════════════════════
//  SP-03: PAYMENT TIMELINE
// ═══════════════════════════════════════════

public class SchoolPayTimelineEntryDto
{
    public int Id { get; set; }
    public string EventType { get; set; } = string.Empty;
    public string? EventData { get; set; }
    public string? PerformedBy { get; set; }
    public DateTime PerformedAt { get; set; }
    public string? IpAddress { get; set; }
}

public class SchoolPayTimelineDto
{
    public int TransactionId { get; set; }
    public string TransactionReference { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public SchoolPayTransactionStatus Status { get; set; }
    public List<SchoolPayTimelineEntryDto> Entries { get; set; } = new();
}

// ═══════════════════════════════════════════
//  SP-04: SECURITY
// ═══════════════════════════════════════════

public class SchoolPaySecurityAuditEntryDto
{
    public int Id { get; set; }
    public int EventType { get; set; }
    public string EventTypeName { get; set; } = string.Empty;
    public string? PerformedBy { get; set; }
    public string? IpAddress { get; set; }
    public string? Details { get; set; }
    public DateTime PerformedAt { get; set; }
}

public class SchoolPaySecretKeyDto
{
    public int Id { get; set; }
    public string KeyName { get; set; } = string.Empty;
    public string? KeyPreview { get; set; }
    public DateTime? LastRotatedAt { get; set; }
    public bool IsActive { get; set; }
    public string? Version { get; set; }
}

// ═══════════════════════════════════════════
//  SP-04: MONITORING
// ═══════════════════════════════════════════

public class SchoolPayMonitoringDto
{
    public List<SchoolPayProviderUptimeDto> ProviderUptimes { get; set; } = new();
    public List<SchoolPayWebhookLatencyDto> WebhookLatencies { get; set; } = new();
    public SchoolPayQueueMetricsDto QueueMetrics { get; set; } = new();
    public SchoolPayTrendDto Trends { get; set; } = new();
}

public class SchoolPayProviderUptimeDto
{
    public string ProviderName { get; set; } = string.Empty;
    public double UptimePercentage { get; set; }
    public int TotalChecks { get; set; }
    public int SuccessfulChecks { get; set; }
    public double AvgResponseTimeMs { get; set; }
}

public class SchoolPayWebhookLatencyDto
{
    public string Date { get; set; } = string.Empty;
    public double AvgProcessingTimeMs { get; set; }
    public int TotalProcessed { get; set; }
}

public class SchoolPayQueueMetricsDto
{
    public int WebhookQueueDepth { get; set; }
    public int DlqDepth { get; set; }
    public int PendingSettlements { get; set; }
    public int PendingRefunds { get; set; }
}

public class SchoolPayTrendDto
{
    public double SuccessRate24h { get; set; }
    public double SuccessRate7d { get; set; }
    public long TotalTransactions24h { get; set; }
    public decimal TotalVolume24h { get; set; }
}

// ═══════════════════════════════════════════
//  SP-04: EVENT BUS
// ═══════════════════════════════════════════

public class SchoolPayPaymentEvent
{
    public string EventId { get; set; } = Guid.NewGuid().ToString("N");
    public string EventType { get; set; } = string.Empty;
    public string? TransactionReference { get; set; }
    public int? TransactionId { get; set; }
    public decimal? Amount { get; set; }
    public string? ProviderCode { get; set; }
    public DateTime Timestamp { get; set; } = DateTime.UtcNow;
}

// ═══════════════════════════════════════════
//  SP-04: SANDBOX
// ═══════════════════════════════════════════

public class SchoolPaySandboxTestRequest
{
    public string Scenario { get; set; } = "success";
    public decimal Amount { get; set; } = 100;
    public int? InvoiceId { get; set; }
}

public class SchoolPaySandboxTestResult
{
    public bool Success { get; set; }
    public string? TransactionReference { get; set; }
    public string? GatewayPageUrl { get; set; }
    public string? Message { get; set; }
}
