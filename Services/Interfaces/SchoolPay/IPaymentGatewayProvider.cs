using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IPaymentGatewayProvider
{
    string ProviderCode { get; }
    string ProviderName { get; }
    bool IsAvailable { get; }

    Task<SchoolPayInitResult> InitiatePaymentAsync(
        int onlinePaymentRequestId,
        decimal amount,
        string transactionReference,
        string? description,
        string? customerName,
        string? customerEmail,
        string? customerPhone,
        string? successUrl,
        string? failUrl,
        string? cancelUrl,
        string? ipnUrl,
        string? preferredPaymentMethod = null,
        CancellationToken ct = default);

    Task<SchoolPayVerifyResult> ValidateTransactionAsync(
        string providerTransactionId,
        CancellationToken ct = default);

    Task<SchoolPayIpnResult> ProcessIpnAsync(
        string? bankTransactionId,
        string? providerTransactionId,
        string? validationId,
        string status,
        CancellationToken ct = default);

    Task<bool> ProcessRefundAsync(
        string providerTransactionId,
        decimal amount,
        string? reason,
        CancellationToken ct = default);

    Task<ProviderHealthStatus> CheckHealthAsync(CancellationToken ct = default);
}
