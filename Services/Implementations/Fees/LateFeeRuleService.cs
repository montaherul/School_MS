using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class LateFeeRuleService : ILateFeeRuleService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILateFeeRuleRepository _repository;

    public LateFeeRuleService(IUnitOfWork unitOfWork, ILateFeeRuleRepository repository)
    {
        _unitOfWork = unitOfWork;
        _repository = repository;
    }

    public async Task<PagedResult<LateFeeRuleListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _repository.GetListByStoredProcedureAsync(page, pageSize, search, cancellationToken);
        return new PagedResult<LateFeeRuleListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<LateFeeRuleUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<LateFeeRule>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new LateFeeRuleUpsertDto { Id = entity.Id, Name = entity.Name, GraceDays = entity.GraceDays, FeeType = (int)entity.FeeType, FeeValue = entity.FeeValue, MaxFee = entity.MaxFee, SchoolClassId = entity.SchoolClassId, FeeCategoryId = entity.FeeCategoryId, IsActive = entity.IsActive };
    }

    public async Task<int> CreateAsync(LateFeeRuleUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new LateFeeRule { CreatedBy = createdBy, Name = dto.Name, GraceDays = dto.GraceDays, FeeType = (FeeDiscountType)dto.FeeType, FeeValue = dto.FeeValue, MaxFee = dto.MaxFee, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId, IsActive = dto.IsActive };
        await _unitOfWork.Repository<LateFeeRule>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(LateFeeRuleUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<LateFeeRule>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("LateFeeRule not found.");
        entity.Name = dto.Name; entity.GraceDays = dto.GraceDays; entity.FeeType = (FeeDiscountType)dto.FeeType;
        entity.FeeValue = dto.FeeValue; entity.MaxFee = dto.MaxFee; entity.SchoolClassId = dto.SchoolClassId; entity.FeeCategoryId = dto.FeeCategoryId; entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<LateFeeRule>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<LateFeeRule>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("LateFeeRule not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<LateFeeRule>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<LateFeeRule>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("LateFeeRule not found or not deleted.");
        entity.IsDeleted = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<LateFeeRule>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
