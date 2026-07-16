using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class SandboxProvider : IPaymentGatewayProvider
{
    private readonly ISchoolPayRepository _repo;
    private readonly ILogger<SandboxProvider> _logger;
    private static readonly Random _rng = new();

    public string ProviderCode => "SANDBOX";
    public string ProviderName => "Sandbox Simulator";
    public bool IsAvailable => true;

    public SandboxProvider(ISchoolPayRepository repo, ILogger<SandboxProvider> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SchoolPayInitResult> InitiatePaymentAsync(
        int onlinePaymentRequestId, decimal amount, string transactionReference,
        string? description, string? customerName, string? customerEmail, string? customerPhone,
        string? successUrl, string? failUrl, string? cancelUrl, string? ipnUrl,
        string? preferredPaymentMethod = null,
        CancellationToken ct = default)
    {
        _logger.LogInformation("Sandbox: InitiatePayment {TxnRef} amount {Amount}", transactionReference, amount);

        var txn = new SchoolManagementSystem.Models.Entities.SchoolPay.PaymentGatewayTransaction
        {
            PaymentProviderId = 0,
            TransactionReference = transactionReference,
            Amount = amount,
            Currency = "BDT",
            Status = SchoolPayTransactionStatus.Processing,
            InitiatedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow,
            IsActive = true
        };
        await _repo.CreateTransactionAsync(txn, ct);

        return new SchoolPayInitResult
        {
            Success = true,
            TransactionReference = transactionReference,
            GatewayPageUrl = $"/SchoolPay/Sandbox/Gateway?txnRef={transactionReference}&amount={amount}&successUrl={successUrl}",
            SessionKey = Guid.NewGuid().ToString("N")
        };
    }

    public async Task<SchoolPayVerifyResult> ValidateTransactionAsync(string providerTransactionId, CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        return new SchoolPayVerifyResult
        {
            Success = true,
            TransactionStatus = SchoolPayTransactionStatus.Completed,
            ProviderTransactionId = providerTransactionId,
            BankTransactionId = $"BANK_{_rng.Next(100000, 999999)}",
            Amount = 0,
            Currency = "BDT"
        };
    }

    public async Task<SchoolPayIpnResult> ProcessIpnAsync(
        string? bankTransactionId, string? providerTransactionId, string? validationId, string status,
        CancellationToken ct = default)
    {
        await Task.Delay(100, ct);
        return new SchoolPayIpnResult
        {
            Success = status == "success",
            ErrorMessage = status == "success" ? null : "Sandbox simulated IPN failure"
        };
    }

    public async Task<bool> ProcessRefundAsync(string providerTransactionId, decimal amount, string? reason, CancellationToken ct = default)
    {
        await Task.Delay(200, ct);
        _logger.LogInformation("Sandbox: Refund {TxnId} amount {Amount}", providerTransactionId, amount);
        return true;
    }

    public async Task<ProviderHealthStatus> CheckHealthAsync(CancellationToken ct = default)
    {
        await Task.Delay(50, ct);
        return ProviderHealthStatus.Healthy;
    }
}
