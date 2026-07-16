using System.ComponentModel.DataAnnotations;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.SchoolPay;

[Index(nameof(Code), IsUnique = true)]
public class PaymentProvider : BaseEntity
{
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    [MaxLength(200)]
    public string? LogoUrl { get; set; }

    public ProviderStatus Status { get; set; } = ProviderStatus.Active;

    public int Priority { get; set; }

    public bool IsSandbox { get; set; } = true;

    public bool SupportsRefund { get; set; }

    public bool SupportsSettlement { get; set; }

    public int MaxRetryAttempts { get; set; } = 3;

    [MaxLength(500)]
    public string? SupportedCurrencies { get; set; }

    [MaxLength(200)]
    public string? ClassName { get; set; }

    public bool IsActive { get; set; } = true;
}

public class PaymentProviderConfiguration : BaseEntity
{
    public int PaymentProviderId { get; set; }
    public PaymentProvider? PaymentProvider { get; set; }

    [MaxLength(100)]
    public string Key { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string Value { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsEncrypted { get; set; }

    public bool IsActive { get; set; } = true;
}

[Index(nameof(Code), IsUnique = true)]
public class PaymentMethod : BaseEntity
{
    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? LogoUrl { get; set; }

    public int PaymentProviderId { get; set; }
    public PaymentProvider? PaymentProvider { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }

    public bool IsDefault { get; set; }

    public bool IsRecommended { get; set; }

    public bool IsPopular { get; set; }

    public int PopularityRank { get; set; }

    [MaxLength(20)]
    public string? BackgroundColor { get; set; }

    [MaxLength(20)]
    public string? TextColor { get; set; }

    [MaxLength(100)]
    public string? Icon { get; set; }

    [MaxLength(100)]
    public string? CssClass { get; set; }
}

[Index(nameof(TransactionReference))]
[Index(nameof(ProviderTransactionId))]
public class PaymentGatewayTransaction : BaseEntity
{
    public int PaymentProviderId { get; set; }
    public PaymentProvider? PaymentProvider { get; set; }

    public int? PaymentMethodId { get; set; }
    public PaymentMethod? PaymentMethod { get; set; }

    public int? OnlinePaymentRequestId { get; set; }

    [MaxLength(100)]
    public string TransactionReference { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ProviderTransactionId { get; set; }

    [MaxLength(100)]
    public string? BankTransactionId { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "BDT";

    public decimal Amount { get; set; }

    public decimal? FeeAmount { get; set; }

    public SchoolPayTransactionStatus Status { get; set; } = SchoolPayTransactionStatus.Pending;

    [MaxLength(500)]
    public string? StatusMessage { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    public int AttemptCount { get; set; }

    [MaxLength(8000)]
    public string? RequestPayload { get; set; }

    [MaxLength(8000)]
    public string? ResponsePayload { get; set; }

    [MaxLength(8000)]
    public string? CallbackPayload { get; set; }

    public DateTime? InitiatedAt { get; set; }

    public DateTime? CompletedAt { get; set; }

    public DateTime? ExpiresAt { get; set; }

    public bool IsActive { get; set; } = true;
}

public class PaymentGatewayWebhook : BaseEntity
{
    public int PaymentProviderId { get; set; }
    public PaymentProvider? PaymentProvider { get; set; }

    public int? PaymentGatewayTransactionId { get; set; }
    public PaymentGatewayTransaction? PaymentGatewayTransaction { get; set; }

    [MaxLength(100)]
    public string? TransactionReference { get; set; }

    [MaxLength(100)]
    public string? ProviderEventType { get; set; }

    [MaxLength(2000)]
    public string? RawPayload { get; set; }

    public SchoolPayWebhookStatus Status { get; set; } = SchoolPayWebhookStatus.Received;

    [MaxLength(500)]
    public string? ErrorMessage { get; set; }

    public int AttemptCount { get; set; }

    public DateTime? ReceivedAt { get; set; }

    public DateTime? ProcessedAt { get; set; }
}

public class PaymentGatewaySettlement : BaseEntity
{
    public int PaymentProviderId { get; set; }
    public PaymentProvider? PaymentProvider { get; set; }

    [MaxLength(100)]
    public string SettlementReference { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ProviderSettlementId { get; set; }

    public decimal Amount { get; set; }

    [MaxLength(10)]
    public string Currency { get; set; } = "BDT";

    public SettlementStatus Status { get; set; } = SettlementStatus.Pending;

    public DateTime? SettlementDate { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    [MaxLength(8000)]
    public string? SettlementData { get; set; }
}

public class PaymentGatewayRefund : BaseEntity
{
    public int PaymentGatewayTransactionId { get; set; }
    public PaymentGatewayTransaction? PaymentGatewayTransaction { get; set; }

    [MaxLength(100)]
    public string RefundReference { get; set; } = string.Empty;

    [MaxLength(100)]
    public string? ProviderRefundId { get; set; }

    public decimal RefundAmount { get; set; }

    [MaxLength(500)]
    public string? Reason { get; set; }

    public RefundStatus Status { get; set; } = RefundStatus.Requested;

    public DateTime? RefundedAt { get; set; }

    [MaxLength(64)]
    public string? ProcessedBy { get; set; }
}

public class PaymentGatewayAudit : BaseEntity
{
    public int? PaymentGatewayTransactionId { get; set; }

    [MaxLength(50)]
    public string EventType { get; set; } = string.Empty;

    [MaxLength(2000)]
    public string? EventData { get; set; }

    [MaxLength(64)]
    public string? PerformedBy { get; set; }

    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;

    [MaxLength(100)]
    public string? IpAddress { get; set; }
}

public class PaymentRouteRule : BaseEntity
{
    public int PaymentProviderId { get; set; }
    public PaymentProvider? PaymentProvider { get; set; }

    [MaxLength(100)]
    public string RuleName { get; set; } = string.Empty;

    public PaymentRoutePriority Priority { get; set; } = PaymentRoutePriority.Primary;

    public decimal? MinAmount { get; set; }

    public decimal? MaxAmount { get; set; }

    [MaxLength(50)]
    public string? FeeType { get; set; }

    [MaxLength(500)]
    public string? ConditionExpression { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; }
}

public class PaymentGatewayHealth : BaseEntity
{
    public int PaymentProviderId { get; set; }
    public PaymentProvider? PaymentProvider { get; set; }

    public ProviderHealthStatus Status { get; set; } = ProviderHealthStatus.Unknown;

    public int ResponseTimeMs { get; set; }

    public decimal SuccessRate { get; set; }

    public int TotalRequests { get; set; }

    public int FailedRequests { get; set; }

    [MaxLength(1000)]
    public string? LastError { get; set; }

    public DateTime? LastCheckedAt { get; set; }

    public DateTime? LastSuccessAt { get; set; }
}

public class PaymentGatewaySecurityEvent : BaseEntity
{
    public int? PaymentProviderId { get; set; }

    public PaymentSecurityEventType EventType { get; set; }

    [MaxLength(2000)]
    public string? Details { get; set; }

    [MaxLength(64)]
    public string? PerformedBy { get; set; }

    [MaxLength(100)]
    public string? IpAddress { get; set; }

    [MaxLength(64)]
    public string? Severity { get; set; }

    [MaxLength(100)]
    public string? EventSource { get; set; }

    [MaxLength(100)]
    public string? MachineName { get; set; }

    [MaxLength(100)]
    public string? SessionId { get; set; }

    [MaxLength(100)]
    public string? GatewayTransactionId { get; set; }

    [MaxLength(100)]
    public string? CorrelationId { get; set; }

    [MaxLength(100)]
    public string? RequestId { get; set; }

    [MaxLength(500)]
    public string? UserAgent { get; set; }

    public DateTime PerformedAt { get; set; } = DateTime.UtcNow;
}
