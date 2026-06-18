using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeCategoryService : IFeeCategoryService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeeCategoryRepository _categoryRepository;

    public FeeCategoryService(IUnitOfWork unitOfWork, IFeeCategoryRepository categoryRepository)
    {
        _unitOfWork = unitOfWork;
        _categoryRepository = categoryRepository;
    }

    public async Task<PagedResult<FeeCategoryListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _categoryRepository.GetListByStoredProcedureAsync(page, pageSize, search, cancellationToken);
        return new PagedResult<FeeCategoryListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<FeeCategoryUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeCategory>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new FeeCategoryUpsertDto { Id = entity.Id, Name = entity.Name, Description = entity.Description, DisplayOrder = entity.DisplayOrder, IsActive = entity.IsActive };
    }

    public async Task<int> CreateAsync(FeeCategoryUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new FeeCategory { CreatedBy = createdBy, Name = dto.Name, Description = dto.Description, DisplayOrder = dto.DisplayOrder, IsActive = dto.IsActive };
        await _unitOfWork.Repository<FeeCategory>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(FeeCategoryUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeCategory>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeCategory not found.");
        entity.Name = dto.Name; entity.Description = dto.Description; entity.DisplayOrder = dto.DisplayOrder; entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeCategory>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeCategory>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeCategory not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeCategory>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeCategory>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeCategory not found or not deleted.");
        entity.IsDeleted = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeCategory>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
