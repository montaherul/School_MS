using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SchoolClassService : ISchoolClassService
{
    private readonly IUnitOfWork _unitOfWork;

    public SchoolClassService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<PagedResult<SchoolClassListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<SchoolClass>().Query()
            .Where(c => !c.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(c => c.Name.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(c => c.SortOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new SchoolClassListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                SortOrder = c.SortOrder
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SchoolClassListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<SchoolClassUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<SchoolClass>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SchoolClassUpsertDto { Id = entity.Id, Name = entity.Name, SortOrder = entity.SortOrder };
    }

    public async Task<int> CreateAsync(SchoolClassUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new SchoolClass { CreatedBy = createdBy, Name = dto.Name, SortOrder = dto.SortOrder };
        await _unitOfWork.Repository<SchoolClass>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(SchoolClassUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<SchoolClass>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("SchoolClass not found.");
        entity.Name = dto.Name;
        entity.SortOrder = dto.SortOrder;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<SchoolClass>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("SchoolClass not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<SchoolClassListItemDto>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Repository<SchoolClass>().Query()
            .Where(c => !c.IsDeleted)
            .OrderBy(c => c.SortOrder)
            .Select(c => new SchoolClassListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                SortOrder = c.SortOrder
            })
            .ToListAsync(cancellationToken);
    }
}

