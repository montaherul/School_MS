using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SchoolShiftService : ISchoolShiftService
{
    private readonly IUnitOfWork _uow;

    public SchoolShiftService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<PagedResult<SchoolShiftListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _uow.Repository<SchoolShift>().Query()
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

        var dtoItems = items.Select(x => new SchoolShiftListItemDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            StartTime = x.StartTime.ToString(@"hh\:mm"),
            EndTime = x.EndTime.ToString(@"hh\:mm"),
            DisplayOrder = x.DisplayOrder,
            IsActive = x.IsActive,
            TotalRecords = totalCount
        }).ToList();

        return new PagedResult<SchoolShiftListItemDto>
        {
            Items = dtoItems,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<SchoolShiftUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<SchoolShift>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SchoolShiftUpsertDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            StartTime = entity.StartTime,
            EndTime = entity.EndTime,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(SchoolShiftUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new SchoolShift
        {
            Name = dto.Name.Trim(),
            Code = dto.Code.Trim().ToUpperInvariant(),
            StartTime = dto.StartTime,
            EndTime = dto.EndTime,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            CreatedBy = createdBy
        };
        await _uow.Repository<SchoolShift>().AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(SchoolShiftUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<SchoolShift>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("School Shift not found.");
        entity.Name = dto.Name.Trim();
        entity.Code = dto.Code.Trim().ToUpperInvariant();
        entity.StartTime = dto.StartTime;
        entity.EndTime = dto.EndTime;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<object>> GetAllActiveAsync(CancellationToken cancellationToken = default)
    {
        return await _uow.Repository<SchoolShift>().Query()
            .Where(x => !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.DisplayOrder)
            .Select(x => new { x.Id, x.Name, x.Code })
            .ToListAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<SchoolShift>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("School Shift not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
