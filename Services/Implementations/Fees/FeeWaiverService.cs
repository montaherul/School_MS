using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeWaiverService : IFeeWaiverService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeeWaiverRepository _repository;
    private readonly IAuditLogService _audit;

    public FeeWaiverService(IUnitOfWork unitOfWork, IFeeWaiverRepository repository, IAuditLogService audit)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
        _audit = audit;
    }

    public async Task<PagedResult<FeeWaiverListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _repository.GetListByStoredProcedureAsync(page, pageSize, search, studentId, cancellationToken);
        return new PagedResult<FeeWaiverListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<FeeWaiverUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeWaiver>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new FeeWaiverUpsertDto { Id = entity.Id, StudentId = entity.StudentId, FeeInvoiceId = entity.FeeInvoiceId, FeeCategoryId = entity.FeeCategoryId, FeeStructureId = entity.FeeStructureId, WaiverType = (int)entity.WaiverType, WaiverValue = entity.WaiverValue, WaiverAmount = entity.WaiverAmount, Reason = entity.Reason, IsApproved = entity.IsApproved, ValidFrom = entity.ValidFrom, ValidTo = entity.ValidTo };
    }

    public async Task<int> CreateAsync(FeeWaiverUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new FeeWaiver { CreatedBy = createdBy, StudentId = dto.StudentId, FeeInvoiceId = dto.FeeInvoiceId, FeeCategoryId = dto.FeeCategoryId, FeeStructureId = dto.FeeStructureId, WaiverType = (FeeDiscountType)dto.WaiverType, WaiverValue = dto.WaiverValue, WaiverAmount = dto.WaiverAmount, Reason = dto.Reason, IsApproved = dto.IsApproved, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo };

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.Repository<FeeWaiver>().AddAsync(entity, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            if (dto.IsApproved)
                await WriteLedgerForWaiverAsync(dto.StudentId, dto.FeeInvoiceId, entity.Id, dto.WaiverAmount, "Waiver applied", createdBy, cancellationToken);
        }, cancellationToken);

        await _audit.LogAsync("FeeWaivers", "Create", $"Waiver {entity.Id} recorded for student {dto.StudentId}, amount {dto.WaiverAmount}, approved={dto.IsApproved}", createdBy, cancellationToken: cancellationToken);

        return entity.Id;
    }

    public async Task UpdateAsync(FeeWaiverUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeWaiver>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeWaiver not found.");

        var wasApproved = entity.IsApproved;
        var oldAmount = entity.WaiverAmount;

        entity.StudentId = dto.StudentId; entity.FeeInvoiceId = dto.FeeInvoiceId; entity.FeeCategoryId = dto.FeeCategoryId;
        entity.FeeStructureId = dto.FeeStructureId; entity.WaiverType = (FeeDiscountType)dto.WaiverType;
        entity.WaiverValue = dto.WaiverValue; entity.WaiverAmount = dto.WaiverAmount; entity.Reason = dto.Reason;
        entity.IsApproved = dto.IsApproved; entity.ValidFrom = dto.ValidFrom; entity.ValidTo = dto.ValidTo;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeWaiver>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (dto.IsApproved)
        {
            if (wasApproved)
            {
                var oldLedgers = await _unitOfWork.Repository<FeeLedger>().ListAsync(x => x.FeeWaiverId == entity.Id && !x.IsDeleted, cancellationToken);
                foreach (var old in oldLedgers) { old.IsDeleted = true; _unitOfWork.Repository<FeeLedger>().Update(old); }
            }
            await WriteLedgerForWaiverAsync(dto.StudentId, dto.FeeInvoiceId, entity.Id, dto.WaiverAmount, wasApproved ? "Waiver updated" : "Waiver approved", updatedBy, cancellationToken);
        }
    }

    private async Task WriteLedgerForWaiverAsync(int studentId, int? feeInvoiceId, int waiverId, decimal amount, string description, string createdBy, CancellationToken cancellationToken)
    {
        var ledger = new FeeLedger
        {
            StudentId = studentId,
            FeeInvoiceId = feeInvoiceId,
            FeeWaiverId = waiverId,
            TransactionType = FeeLedgerType.Waiver,
            Debit = 0,
            Credit = amount,
            Balance = -amount,
            Description = description,
            TransactionDate = DateTime.UtcNow,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<FeeLedger>().AddAsync(ledger, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ApproveAsync(int id, string approvedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeWaiver>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Waiver not found.");

        if (entity.IsApproved) return;

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            entity.IsApproved = true;
            entity.ApprovedBy = approvedBy;
            entity.ApprovedAt = DateTime.UtcNow;
            entity.UpdatedBy = approvedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            _unitOfWork.Repository<FeeWaiver>().Update(entity);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await WriteLedgerForWaiverAsync(entity.StudentId, entity.FeeInvoiceId, entity.Id, entity.WaiverAmount, $"Waiver approved: {entity.Reason}", approvedBy, cancellationToken);
        }, cancellationToken);

        await _audit.LogAsync("FeeWaivers", "Approve", $"Waiver {id} approved, amount {entity.WaiverAmount}", approvedBy, cancellationToken: cancellationToken);
    }

    public async Task RejectAsync(int id, string rejectedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeWaiver>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Waiver not found.");

        var wasApproved = entity.IsApproved;

        entity.IsApproved = false;
        entity.ApprovedBy = null;
        entity.ApprovedAt = null;
        entity.RejectedBy = rejectedBy;
        entity.RejectedAt = DateTime.UtcNow;
        entity.UpdatedBy = rejectedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeWaiver>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (wasApproved)
        {
            var reversal = new FeeLedger
            {
                StudentId = entity.StudentId,
                FeeInvoiceId = entity.FeeInvoiceId,
                FeeWaiverId = entity.Id,
                TransactionType = FeeLedgerType.Adjustment,
                Debit = entity.WaiverAmount,
                Credit = 0,
                Balance = entity.WaiverAmount,
                Description = $"Waiver rejected (reversal): {entity.Reason}",
                TransactionDate = DateTime.UtcNow,
                CreatedBy = rejectedBy,
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.Repository<FeeLedger>().AddAsync(reversal, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeWaiver>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeWaiver not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeWaiver>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeWaiver>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeWaiver not found or not deleted.");
        entity.IsDeleted = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeWaiver>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
