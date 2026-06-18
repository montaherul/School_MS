using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeStructureService : IFeeStructureService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IFeeStructureRepository _structureRepository;

    public FeeStructureService(IUnitOfWork unitOfWork, IFeeStructureRepository structureRepository)
    {
        _unitOfWork = unitOfWork;
        _structureRepository = structureRepository;
    }

    public async Task<PagedResult<FeeStructureListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? schoolClassId = null, int? feeCategoryId = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _structureRepository.GetListByStoredProcedureAsync(page, pageSize, search, schoolClassId, feeCategoryId, cancellationToken);
        return new PagedResult<FeeStructureListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<FeeStructureUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeStructure>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new FeeStructureUpsertDto
        {
            Id = entity.Id, SchoolClassId = entity.SchoolClassId, FeeCategoryId = entity.FeeCategoryId,
            AcademicYearId = entity.AcademicYearId, FeeName = entity.FeeName, Description = entity.Description,
            Amount = entity.Amount, IsRecurring = entity.IsRecurring, Frequency = (int)entity.Frequency,
            DueDay = entity.DueDay, IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(FeeStructureUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new FeeStructure
        {
            CreatedBy = createdBy, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId,
            AcademicYearId = dto.AcademicYearId, FeeName = dto.FeeName, Description = dto.Description,
            Amount = dto.Amount, IsRecurring = dto.IsRecurring, Frequency = (FeeFrequency)dto.Frequency,
            DueDay = dto.DueDay, IsActive = dto.IsActive
        };
        await _unitOfWork.Repository<FeeStructure>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(FeeStructureUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeStructure>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeStructure not found.");
        entity.SchoolClassId = dto.SchoolClassId; entity.FeeCategoryId = dto.FeeCategoryId; entity.AcademicYearId = dto.AcademicYearId;
        entity.FeeName = dto.FeeName; entity.Description = dto.Description; entity.Amount = dto.Amount;
        entity.IsRecurring = dto.IsRecurring; entity.Frequency = (FeeFrequency)dto.Frequency; entity.DueDay = dto.DueDay; entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeStructure>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeStructure>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeStructure not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeStructure>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<FeeStructure>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeStructure not found or not deleted.");
        entity.IsDeleted = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<FeeStructure>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
