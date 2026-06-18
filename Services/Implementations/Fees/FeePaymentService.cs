using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeePaymentService : IFeePaymentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeePaymentRepository _repository;

    public FeePaymentService(IUnitOfWork unitOfWork, IFeePaymentRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<PagedResult<FeePaymentListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? feeInvoiceId = null, int? paymentMethod = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _repository.GetListByStoredProcedureAsync(page, pageSize, search, feeInvoiceId, paymentMethod, cancellationToken);
        return new PagedResult<FeePaymentListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<FeePaymentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Payment>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new FeePaymentUpsertDto { Id = entity.Id, FeeInvoiceId = entity.FeeInvoiceId, Amount = entity.Amount, LateFee = entity.LateFee, DiscountAmount = entity.DiscountAmount, Method = (int)entity.Method, ReferenceNo = entity.ReferenceNo, PaidAt = entity.PaidAt, Remarks = entity.Remarks };
    }

    public async Task<int> CreateAsync(FeePaymentUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        if (dto.Amount <= 0)
            throw new InvalidOperationException("Payment amount must be greater than zero.");

        Payment entity = null!;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var invoice = await _unitOfWork.Repository<FeeInvoice>().FirstOrDefaultAsync(x => x.Id == dto.FeeInvoiceId && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Invoice not found.");

            var remainingBalance = invoice.TotalAmount - invoice.PaidAmount;
            if (dto.Amount > remainingBalance)
                throw new InvalidOperationException("Payment amount exceeds outstanding invoice balance.");

            entity = new Payment { CreatedBy = createdBy, FeeInvoiceId = dto.FeeInvoiceId, Amount = dto.Amount, LateFee = dto.LateFee, DiscountAmount = dto.DiscountAmount, Method = (PaymentMethod)dto.Method, ReferenceNo = dto.ReferenceNo, PaidAt = dto.PaidAt, Remarks = dto.Remarks };
            await _unitOfWork.Repository<Payment>().AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await RecalculateInvoiceAsync(dto.FeeInvoiceId, cancellationToken);
            await WriteLedgerEntryAsync(invoice.StudentId, dto.FeeInvoiceId, entity.Id, FeeLedgerType.Payment, 0, dto.Amount, $"Payment received: {dto.ReferenceNo ?? "N/A"}", cancellationToken);
        }, cancellationToken);

        return entity.Id;
    }

    public async Task UpdateAsync(FeePaymentUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        if (dto.Amount <= 0)
            throw new InvalidOperationException("Payment amount must be greater than zero.");

        var entity = await _unitOfWork.Repository<Payment>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Payment not found.");

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            var invoice = await _unitOfWork.Repository<FeeInvoice>().FirstOrDefaultAsync(x => x.Id == entity.FeeInvoiceId && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Invoice not found.");

            var otherPaymentsTotal = (await _unitOfWork.Repository<Payment>().ListAsync(x => x.FeeInvoiceId == entity.FeeInvoiceId && !x.IsDeleted && x.Id != entity.Id, cancellationToken)).Sum(p => p.Amount);
            var remainingBalance = invoice.TotalAmount - otherPaymentsTotal;
            if (dto.Amount > remainingBalance)
                throw new InvalidOperationException("Payment amount exceeds outstanding invoice balance.");

            // Soft-delete old ledger entries for this payment
            var oldLedgers = await _unitOfWork.Repository<FeeLedger>().ListAsync(x => x.FeePaymentId == entity.Id && !x.IsDeleted, cancellationToken);
            foreach (var old in oldLedgers)
            {
                old.IsDeleted = true;
                _unitOfWork.Repository<FeeLedger>().Update(old);
            }

            entity.FeeInvoiceId = dto.FeeInvoiceId; entity.Amount = dto.Amount; entity.LateFee = dto.LateFee;
            entity.DiscountAmount = dto.DiscountAmount; entity.Method = (PaymentMethod)dto.Method;
            entity.ReferenceNo = dto.ReferenceNo; entity.PaidAt = dto.PaidAt; entity.Remarks = dto.Remarks;
            entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<Payment>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await RecalculateInvoiceAsync(dto.FeeInvoiceId, cancellationToken);
            await WriteLedgerEntryAsync(invoice.StudentId, dto.FeeInvoiceId, entity.Id, FeeLedgerType.Payment, 0, dto.Amount, $"Payment updated: {dto.ReferenceNo ?? "N/A"}", cancellationToken);
        }, cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Payment>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Payment not found.");
        var invoiceId = entity.FeeInvoiceId;
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            _unitOfWork.Repository<Payment>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await RecalculateInvoiceAsync(invoiceId, cancellationToken);

            var invoice = await _unitOfWork.Repository<FeeInvoice>().FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, cancellationToken);
            if (invoice is not null)
                await WriteLedgerEntryAsync(invoice.StudentId, invoiceId, id, FeeLedgerType.Payment, entity.Amount, 0, $"Payment deleted (reversal): {entity.ReferenceNo ?? "N/A"}", cancellationToken);
        }, cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Payment>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Payment not found or not deleted.");
        entity.IsDeleted = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Payment>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    private async Task WriteLedgerEntryAsync(int studentId, int? feeInvoiceId, int? feePaymentId, FeeLedgerType type, decimal debit, decimal credit, string? description, CancellationToken cancellationToken)
    {
        var entry = new FeeLedger
        {
            StudentId = studentId,
            FeeInvoiceId = feeInvoiceId,
            FeePaymentId = feePaymentId,
            TransactionType = type,
            Debit = debit,
            Credit = credit,
            Balance = debit - credit,
            Description = description,
            TransactionDate = DateTime.UtcNow,
            CreatedBy = "system",
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<FeeLedger>().AddAsync(entry, cancellationToken);
    }

    private async Task RecalculateInvoiceAsync(int invoiceId, CancellationToken cancellationToken)
    {
        var invoice = await _unitOfWork.Repository<FeeInvoice>().FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, cancellationToken);
        if (invoice is null) return;

        var payments = await _unitOfWork.Repository<Payment>().ListAsync(x => x.FeeInvoiceId == invoiceId && !x.IsDeleted, cancellationToken);
        var totalPaid = payments.Sum(p => p.Amount);

        invoice.PaidAmount = totalPaid;
        invoice.UpdatedAt = DateTime.UtcNow;

        var dueAmount = invoice.TotalAmount - invoice.PaidAmount;
        if (dueAmount <= 0)
            invoice.Status = PaymentStatus.Paid;
        else if (totalPaid > 0)
            invoice.Status = PaymentStatus.Partial;
        else
            invoice.Status = PaymentStatus.Unpaid;

        _unitOfWork.Repository<FeeInvoice>().Update(invoice);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
