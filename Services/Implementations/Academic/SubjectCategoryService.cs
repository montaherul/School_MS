using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SubjectCategoryService : ISubjectCategoryService
{
    private readonly IUnitOfWork _uow;

    public SubjectCategoryService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<PagedResult<SubjectCategoryListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _uow.Repository<SubjectCategory>().Query()
            .Where(x => !x.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Name.Contains(search) || x.Code.Contains(search));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderBy(x => x.DisplayOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtoItems = items.Select(x => new SubjectCategoryListItemDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            Description = x.Description,
            DisplayOrder = x.DisplayOrder,
            IsActive = x.IsActive,
            TotalRecords = totalCount
        }).ToList();

        return new PagedResult<SubjectCategoryListItemDto>
        {
            Items = dtoItems,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<SubjectCategoryUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<SubjectCategory>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SubjectCategoryUpsertDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(SubjectCategoryUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new SubjectCategory
        {
            Name = dto.Name.Trim(),
            Code = dto.Code.Trim().ToUpperInvariant(),
            Description = dto.Description?.Trim(),
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            CreatedBy = createdBy
        };
        await _uow.Repository<SubjectCategory>().AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(SubjectCategoryUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<SubjectCategory>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("SubjectCategory not found.");
        entity.Name = dto.Name.Trim();
        entity.Code = dto.Code.Trim().ToUpperInvariant();
        entity.Description = dto.Description?.Trim();
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<SubjectCategory>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("SubjectCategory not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
