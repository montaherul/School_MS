using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeTypeService : IFeeTypeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeeTypeRepository _typeRepository;

    public FeeTypeService(IUnitOfWork unitOfWork, IFeeTypeRepository typeRepository)
    {
        _unitOfWork = unitOfWork;
        _typeRepository = typeRepository;
    }

    public async Task<PagedResult<FeeTypeListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _typeRepository.GetListByStoredProcedureAsync(page, pageSize, search, cancellationToken);
        return new PagedResult<FeeTypeListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<FeeTypeUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeType>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new FeeTypeUpsertDto { Id = entity.Id, Name = entity.Name, Description = entity.Description, DisplayOrder = entity.DisplayOrder, IsActive = entity.IsActive };
    }

    public async Task<int> CreateAsync(FeeTypeUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new FeeType { CreatedBy = createdBy, Name = dto.Name, Description = dto.Description, DisplayOrder = dto.DisplayOrder, IsActive = dto.IsActive };
        await _unitOfWork.Repository<FeeType>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(FeeTypeUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeType>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeType not found.");
        entity.Name = dto.Name; entity.Description = dto.Description; entity.DisplayOrder = dto.DisplayOrder; entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeType>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeType>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeType not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeType>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeType>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeType not found or not deleted.");
        entity.IsDeleted = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeType>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
