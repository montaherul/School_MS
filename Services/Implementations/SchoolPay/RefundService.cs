using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Entities.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class RefundService : IRefundService
{
    private readonly ISchoolPayRepository _repository;
    private readonly GatewayFactory _gatewayFactory;
    private readonly ILogger<RefundService> _logger;

    public RefundService(
        ISchoolPayRepository repository,
        GatewayFactory gatewayFactory,
        ILogger<RefundService> logger)
    {
        _repository = repository;
        _gatewayFactory = gatewayFactory;
        _logger = logger;
    }

    public async Task<SchoolPayRefundDto?> ProcessRefundAsync(
        int transactionId,
        decimal amount,
        string? reason,
        string processedBy,
        CancellationToken ct = default)
    {
        var transaction = await _repository.GetTransactionEntityByIdAsync(transactionId, ct);
        if (transaction == null)
        {
            _logger.LogWarning("Refund requested for non-existent transaction {Id}", transactionId);
            return null;
        }

        if (transaction.Status != SchoolPayTransactionStatus.Completed)
        {
            _logger.LogWarning("Refund requested for non-completed transaction {Ref}", transaction.TransactionReference);
            return null;
        }

        var gatewayProvider = _gatewayFactory.GetProvider(transaction.PaymentProvider?.Code ?? "");
        if (gatewayProvider == null)
        {
            _logger.LogWarning("No gateway provider for refund: {Provider}", transaction.PaymentProvider?.Code);
            return null;
        }

        var refund = new PaymentGatewayRefund
        {
            PaymentGatewayTransactionId = transactionId,
            RefundReference = $"RFND{transactionId:D6}{DateTime.UtcNow:yyyyMMddHHmmss}",
            RefundAmount = amount,
            Reason = reason,
            Status = RefundStatus.Requested,
            ProcessedBy = processedBy,
            CreatedBy = processedBy,
            CreatedAt = DateTime.UtcNow
        };

        if (transaction.PaymentProvider?.SupportsRefund == true)
        {
            var providerRefundId = transaction.ProviderTransactionId;
            var refundResult = await gatewayProvider.ProcessRefundAsync(
                providerRefundId ?? "", amount, reason, ct);

            refund.Status = refundResult ? RefundStatus.Processed : RefundStatus.Failed;
            refund.ProviderRefundId = refundResult ? providerRefundId : null;
            if (refundResult) refund.RefundedAt = DateTime.UtcNow;
        }
        else
        {
            refund.Status = RefundStatus.Approved;
        }

        await _repository.CreateRefundAsync(refund, ct);

        return new SchoolPayRefundDto
        {
            Id = refund.Id,
            TransactionReference = transaction.TransactionReference,
            RefundReference = refund.RefundReference,
            ProviderRefundId = refund.ProviderRefundId,
            RefundAmount = refund.RefundAmount,
            Reason = refund.Reason,
            Status = refund.Status,
            RefundedAt = refund.RefundedAt
        };
    }

    public async Task<List<SchoolPayRefundDto>> GetRefundsAsync(CancellationToken ct = default)
        => await _repository.GetRefundsAsync(ct);
}
