using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeInvoiceItemService : IFeeInvoiceItemService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeeInvoiceItemRepository _repository;

    public FeeInvoiceItemService(IUnitOfWork unitOfWork, IFeeInvoiceItemRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<PagedResult<FeeInvoiceItemListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? feeInvoiceId = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _repository.GetListByStoredProcedureAsync(page, pageSize, search, feeInvoiceId, cancellationToken);
        return new PagedResult<FeeInvoiceItemListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<FeeInvoiceItemUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeInvoiceItem>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new FeeInvoiceItemUpsertDto { Id = entity.Id, FeeInvoiceId = entity.FeeInvoiceId, FeeStructureId = entity.FeeStructureId, FeeCategoryId = entity.FeeCategoryId, Description = entity.Description, Amount = entity.Amount, DiscountAmount = entity.DiscountAmount, NetAmount = entity.NetAmount };
    }

    public async Task<int> CreateAsync(FeeInvoiceItemUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new FeeInvoiceItem { CreatedBy = createdBy, FeeInvoiceId = dto.FeeInvoiceId, FeeStructureId = dto.FeeStructureId, FeeCategoryId = dto.FeeCategoryId, Description = dto.Description, Amount = dto.Amount, DiscountAmount = dto.DiscountAmount, NetAmount = dto.NetAmount };
        await _unitOfWork.Repository<FeeInvoiceItem>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(FeeInvoiceItemUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeInvoiceItem>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeInvoiceItem not found.");
        entity.FeeInvoiceId = dto.FeeInvoiceId; entity.FeeStructureId = dto.FeeStructureId; entity.FeeCategoryId = dto.FeeCategoryId;
        entity.Description = dto.Description; entity.Amount = dto.Amount; entity.DiscountAmount = dto.DiscountAmount; entity.NetAmount = dto.NetAmount;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeInvoiceItem>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeInvoiceItem>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeInvoiceItem not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeInvoiceItem>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
