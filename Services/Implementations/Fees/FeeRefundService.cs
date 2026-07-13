using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeRefundService : IFeeRefundService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeeRefundRepository _repository;
    private readonly IAuditLogService _audit;

    public FeeRefundService(IUnitOfWork unitOfWork, IFeeRefundRepository repository, IAuditLogService audit)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _audit = audit;
    }

    public async Task<PagedResult<FeeRefundListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _repository.GetListByStoredProcedureAsync(page, pageSize, search, cancellationToken);
        return new PagedResult<FeeRefundListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<FeeRefundUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeRefund>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new FeeRefundUpsertDto { Id = entity.Id, FeePaymentId = entity.FeePaymentId, RefundAmount = entity.RefundAmount, RefundMethod = (int)entity.RefundMethod, ReferenceNo = entity.ReferenceNo, Reason = entity.Reason, IsApproved = entity.IsApproved, RefundDate = entity.RefundDate };
    }

    public async Task<int> CreateAsync(FeeRefundUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var payment = await _unitOfWork.Repository<Payment>().FirstOrDefaultAsync(x => x.Id == dto.FeePaymentId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Payment not found.");

        var entity = new FeeRefund { CreatedBy = createdBy, FeePaymentId = dto.FeePaymentId, RefundAmount = dto.RefundAmount, RefundMethod = (PaymentMethod)dto.RefundMethod, ReferenceNo = dto.ReferenceNo, Reason = dto.Reason, IsApproved = false, RefundDate = dto.RefundDate };

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.Repository<FeeRefund>().AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        await _audit.LogAsync("FeeRefunds", "Create", $"Refund {entity.Id} recorded for payment {dto.FeePaymentId}, amount {dto.RefundAmount}, approved={entity.IsApproved}", createdBy, cancellationToken: cancellationToken);

        return entity.Id;
    }

    public async Task ApproveAsync(int id, string approvedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeRefund>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Refund not found.");

        if (entity.IsApproved) return;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            entity.IsApproved = true;
            entity.ApprovedBy = approvedBy;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.UpdatedBy = approvedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<FeeRefund>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            var payment = await _unitOfWork.Repository<Payment>().FirstOrDefaultAsync(x => x.Id == entity.FeePaymentId && !x.IsDeleted, cancellationToken);
            var invoice = payment != null ? await _unitOfWork.Repository<FeeInvoice>().FirstOrDefaultAsync(x => x.Id == payment.FeeInvoiceId && !x.IsDeleted, cancellationToken) : null;

            var ledger = new FeeLedger
            {
                StudentId = invoice?.StudentId ?? 0,
                FeeInvoiceId = invoice?.Id,
                FeePaymentId = entity.FeePaymentId,
                FeeRefundId = entity.Id,
                TransactionType = FeeLedgerType.Refund,
                Debit = entity.RefundAmount,
                Credit = 0,
                Balance = entity.RefundAmount,
                Description = $"Refund approved: {entity.Reason}",
                TransactionDate = DateTime.UtcNow,
                CreatedBy = approvedBy,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<FeeLedger>().AddAsync(ledger, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        await _audit.LogAsync("FeeRefunds", "Approve", $"Refund {id} approved, amount {entity.RefundAmount}", approvedBy, cancellationToken: cancellationToken);
    }

    public async Task RejectAsync(int id, string rejectedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeRefund>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Refund not found.");

        entity.IsApproved = false;
        entity.ApprovedBy = null;
        entity.ApprovedAt = null;
        entity.RejectedBy = rejectedBy;
        entity.RejectedAt = DateTime.UtcNow;
        entity.UpdatedBy = rejectedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeRefund>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _audit.LogAsync("FeeRefunds", "Reject", $"Refund {id} rejected", rejectedBy, cancellationToken: cancellationToken);
    }

    public async Task UpdateAsync(FeeRefundUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeRefund>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeRefund not found.");
        entity.FeePaymentId = dto.FeePaymentId; entity.RefundAmount = dto.RefundAmount; entity.RefundMethod = (PaymentMethod)dto.RefundMethod;
        entity.ReferenceNo = dto.ReferenceNo; entity.Reason = dto.Reason; entity.IsApproved = dto.IsApproved; entity.RefundDate = dto.RefundDate;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeRefund>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeRefund>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeRefund not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeRefund>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeRefund>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeRefund not found or not deleted.");
        entity.IsDeleted = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeRefund>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
