using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class AllocationEngineService : IAllocationEngineService
{
    private readonly IAllocationBatchRepository _batchRepository;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _audit;

    public AllocationEngineService(
        IAllocationBatchRepository batchRepository,
        IUnitOfWork unitOfWork,
        IAuditLogService audit)
    {
        _batchRepository = batchRepository;
        _unitOfWork = unitOfWork;
        _audit = audit;
    }

    public async Task<AllocationEngineResultDto> RunAsync(CancellationToken ct = default)
    {
        var result = await _batchRepository.RunBatchAllocationAsync(ct);

        await _audit.LogAsync("AllocationEngine", "Run",
            $"Allocation engine run: {result.AllocationsCreated} allocation(s) across {result.PaymentsProcessed} payment(s), total {result.TotalAllocated}", "system", cancellationToken: ct);

        return result;
    }

    public async Task<AllocationEngineResultDto> AllocateForPaymentAsync(int paymentId, CancellationToken ct = default)
    {
        var result = new AllocationEngineResultDto();

        var payment = await _unitOfWork.Repository<Payment>()
            .FirstOrDefaultAsync(p => p.Id == paymentId && !p.IsDeleted, ct);
        if (payment == null)
        {
            result.Errors.Add($"Payment #{paymentId} not found.");
            return result;
        }

        var existingAllocations = await _unitOfWork.Repository<PaymentAllocation>()
            .ListAsync(a => a.PaymentId == payment.Id && !a.IsDeleted, ct);
        var alreadyAllocated = existingAllocations.Sum(a => a.AllocatedAmount);
        var remaining = payment.Amount - alreadyAllocated;

        if (remaining <= 0) return result;

        var studentId = payment.FeeInvoice?.StudentId ?? 0;
        var openInvoices = await _unitOfWork.Repository<FeeInvoice>().ListAsync(
            i => i.StudentId == studentId
                 && !i.IsDeleted
                 && (i.Status == PaymentStatus.Issued || i.Status == PaymentStatus.Partial)
                 && i.TotalAmount > i.PaidAmount, ct);

        var totalDue = openInvoices.Sum(i => i.TotalAmount - i.PaidAmount);
        if (totalDue <= 0) return result;

        foreach (var invoice in openInvoices)
        {
            var invoiceDue = invoice.TotalAmount - invoice.PaidAmount;
            var proportion = invoiceDue / totalDue;
            var allocationAmount = Math.Round(remaining * proportion, 2);

            if (allocationAmount <= 0) continue;

            var allocation = new PaymentAllocation
            {
                PaymentId = payment.Id,
                FeeInvoiceId = invoice.Id,
                AllocatedAmount = allocationAmount,
                Remarks = $"Auto-allocated from Payment #{payment.Id}",
                CreatedBy = "system",
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<PaymentAllocation>().AddAsync(allocation, ct);

            invoice.PaidAmount += allocationAmount;
            if (invoice.PaidAmount >= invoice.TotalAmount)
                invoice.Status = PaymentStatus.Paid;
            else if (invoice.PaidAmount > 0)
                invoice.Status = PaymentStatus.Partial;
            invoice.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<FeeInvoice>().Update(invoice);

            result.AllocationsCreated++;
            result.TotalAllocated += allocationAmount;
        }

        result.PaymentsProcessed = 1;
        await _unitOfWork.SaveChangesAsync(ct);

        return result;
    }
}
