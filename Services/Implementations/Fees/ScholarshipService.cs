using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class ScholarshipService : IScholarshipService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IScholarshipRepository _scholarshipRepository;

    public ScholarshipService(IUnitOfWork unitOfWork, IScholarshipRepository scholarshipRepository)
    {
        _unitOfWork = unitOfWork;
        _scholarshipRepository = scholarshipRepository;
    }

    public async Task<PagedResult<ScholarshipListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _scholarshipRepository.GetListByStoredProcedureAsync(page, pageSize, search, cancellationToken);
        return new PagedResult<ScholarshipListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<ScholarshipUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Scholarship>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new ScholarshipUpsertDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Description = entity.Description,
            DiscountType = entity.DiscountType,
            Value = entity.Value,
            SchoolClassId = entity.SchoolClassId,
            FeeCategoryId = entity.FeeCategoryId,
            FeeTypeId = entity.FeeTypeId,
            IsActive = entity.IsActive,
            ValidFrom = entity.ValidFrom,
            ValidTo = entity.ValidTo
        };
    }

    public async Task<int> CreateAsync(ScholarshipUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new Scholarship
        {
            CreatedBy = createdBy,
            Name = dto.Name,
            Description = dto.Description,
            DiscountType = dto.DiscountType,
            Value = dto.Value,
            SchoolClassId = dto.SchoolClassId,
            FeeCategoryId = dto.FeeCategoryId,
            FeeTypeId = dto.FeeTypeId,
            IsActive = dto.IsActive,
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo
        };
        await _unitOfWork.Repository<Scholarship>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(ScholarshipUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Scholarship>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Scholarship not found.");
        entity.Name = dto.Name;
        entity.Description = dto.Description;
        entity.DiscountType = dto.DiscountType;
        entity.Value = dto.Value;
        entity.SchoolClassId = dto.SchoolClassId;
        entity.FeeCategoryId = dto.FeeCategoryId;
        entity.FeeTypeId = dto.FeeTypeId;
        entity.IsActive = dto.IsActive;
        entity.ValidFrom = dto.ValidFrom;
        entity.ValidTo = dto.ValidTo;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Scholarship>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Scholarship>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Scholarship not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Scholarship>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Scholarship>().FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Scholarship not found or not deleted.");
        entity.IsDeleted = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _unitOfWork.Repository<Scholarship>().Update(entity);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
