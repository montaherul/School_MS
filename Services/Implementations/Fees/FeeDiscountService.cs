using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeDiscountService : IFeeDiscountService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeeDiscountRepository _repository;

    public FeeDiscountService(IUnitOfWork unitOfWork, IFeeDiscountRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<PagedResult<FeeDiscountListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _repository.GetListByStoredProcedureAsync(page, pageSize, search, cancellationToken);
        return new PagedResult<FeeDiscountListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<FeeDiscountUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeDiscount>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new FeeDiscountUpsertDto { Id = entity.Id, Name = entity.Name, Description = entity.Description, DiscountType = (int)entity.DiscountType, Value = entity.Value, SchoolClassId = entity.SchoolClassId, FeeCategoryId = entity.FeeCategoryId, FeeStructureId = entity.FeeStructureId, IsActive = entity.IsActive, ValidFrom = entity.ValidFrom, ValidTo = entity.ValidTo };
    }

    public async Task<int> CreateAsync(FeeDiscountUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new FeeDiscount { CreatedBy = createdBy, Name = dto.Name, Description = dto.Description, DiscountType = (FeeDiscountType)dto.DiscountType, Value = dto.Value, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId, FeeStructureId = dto.FeeStructureId, IsActive = dto.IsActive, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo };
        await _unitOfWork.Repository<FeeDiscount>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var ledger = new FeeLedger
        {
            StudentId = 0,
            FeeInvoiceId = null,
            TransactionType = FeeLedgerType.Discount,
            Debit = 0,
            Credit = dto.Value,
            Balance = -dto.Value,
            Description = $"Discount created: {dto.Name}",
            TransactionDate = DateTime.UtcNow,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
        await _unitOfWork.Repository<FeeLedger>().AddAsync(ledger, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }

    public async Task UpdateAsync(FeeDiscountUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeDiscount>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeDiscount not found.");
        entity.Name = dto.Name; entity.Description = dto.Description; entity.DiscountType = (FeeDiscountType)dto.DiscountType;
        entity.Value = dto.Value; entity.SchoolClassId = dto.SchoolClassId; entity.FeeCategoryId = dto.FeeCategoryId;
        entity.FeeStructureId = dto.FeeStructureId; entity.IsActive = dto.IsActive; entity.ValidFrom = dto.ValidFrom; entity.ValidTo = dto.ValidTo;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeDiscount>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeDiscount>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeDiscount not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeDiscount>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeDiscount>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeDiscount not found or not deleted.");
        entity.IsDeleted = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeDiscount>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
