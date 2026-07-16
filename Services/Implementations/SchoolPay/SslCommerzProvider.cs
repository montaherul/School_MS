using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class SslCommerzProvider : IPaymentGatewayProvider
{
    private readonly IPaymentGatewayService _sslCommerzGatewayService;
    private readonly ILogger<SslCommerzProvider> _logger;

    public string ProviderCode => "SSLCOMMERZ";
    public string ProviderName => "SSLCommerz";
    public bool IsAvailable => true;

    public SslCommerzProvider(
        IPaymentGatewayService sslCommerzGatewayService,
        ILogger<SslCommerzProvider> logger)
    {
        _sslCommerzGatewayService = sslCommerzGatewayService;
        _logger = logger;
    }

    public async Task<SchoolPayInitResult> InitiatePaymentAsync(
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
        CancellationToken ct = default)
    {
        var sslCardType = MapMethodToSslCardType(preferredPaymentMethod);
        var result = await _sslCommerzGatewayService.InitiatePaymentAsync(onlinePaymentRequestId, sslCardType, ct);
        if (result == null)
        {
            return new SchoolPayInitResult { Success = false, ErrorMessage = "Gateway initialization failed" };
        }
        return new SchoolPayInitResult
        {
            Success = result.status == "SUCCESS",
            GatewayPageUrl = result.GatewayPageURL,
            TransactionReference = result.tran_id,
            SessionKey = result.sessionkey,
            ErrorMessage = result.status != "SUCCESS" ? result.failedreason : null
        };
    }

    public async Task<SchoolPayVerifyResult> ValidateTransactionAsync(string providerTransactionId, CancellationToken ct = default)
    {
        var result = await _sslCommerzGatewayService.ValidateTransactionAsync(providerTransactionId, ct);
        if (result == null)
        {
            return new SchoolPayVerifyResult { Success = false, ErrorMessage = "Validation failed" };
        }
        return new SchoolPayVerifyResult
        {
            Success = result.status == "VALID" || result.status == "VALIDATED",
            TransactionStatus = (result.status == "VALID" || result.status == "VALIDATED")
                ? SchoolPayTransactionStatus.Completed
                : SchoolPayTransactionStatus.Failed,
            ProviderTransactionId = result.tran_id,
            BankTransactionId = result.bank_tran_id,
            CardType = result.card_type,
            Amount = result.amount,
            Currency = result.currency,
            RiskLevel = result.risk_level,
            ErrorMessage = result.status != "VALID" && result.status != "VALIDATED" ? "Validation returned non-valid status" : null
        };
    }

    public async Task<SchoolPayIpnResult> ProcessIpnAsync(
        string? bankTransactionId,
        string? providerTransactionId,
        string? validationId,
        string status,
        CancellationToken ct = default)
    {
        if (string.IsNullOrEmpty(providerTransactionId))
        {
            return new SchoolPayIpnResult { Success = false, ErrorMessage = "Missing transaction ID" };
        }
        var success = await _sslCommerzGatewayService.ProcessIpnAsync(
            bankTransactionId, providerTransactionId, validationId, status, ct);
        return new SchoolPayIpnResult { Success = success };
    }

    public Task<bool> ProcessRefundAsync(string providerTransactionId, decimal amount, string? reason, CancellationToken ct = default)
    {
        _logger.LogInformation("SSLCommerz refund requested for {TranId}: amount={Amount}", providerTransactionId, amount);
        return Task.FromResult(false);
    }

    public Task<ProviderHealthStatus> CheckHealthAsync(CancellationToken ct = default)
    {
        return Task.FromResult(ProviderHealthStatus.Healthy);
    }

    private static string? MapMethodToSslCardType(string? methodCode)
    {
        if (string.IsNullOrEmpty(methodCode)) return null;
        return methodCode.ToLowerInvariant() switch
        {
            "bkash" => "bkash",
            "nagad" => "nagad",
            "rocket" => "rocket",
            "visa" => "visa",
            "mastercard" => "mastercard",
            "amex" => "amex",
            "internetbanking" or "ibanking" => "internetbanking",
            "mobilebanking" or "mfs" => "mobilebanking",
            _ => null
        };
    }
}
