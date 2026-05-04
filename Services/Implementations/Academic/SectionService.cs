using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SectionService : ISectionService
{
    private readonly SchoolDbContext _db;

    public SectionService(SchoolDbContext db) { _db = db; }

    public async Task<PagedResult<SectionListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 5, 100); var term = search?.Trim();
        var query = _db.Sections.Where(x => !x.IsDeleted);
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new SectionListItemDto {
            Id = x.Id,SchoolClassId = x.SchoolClassId,Name = x.Name,        }).ToListAsync(cancellationToken);
        return new PagedResult<SectionListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<SectionUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Sections.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SectionUpsertDto { Id = entity.Id,SchoolClassId = entity.SchoolClassId,Name = entity.Name,        };
    }

    public async Task<int> CreateAsync(SectionUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new Section { CreatedBy = createdBy,SchoolClassId = dto.SchoolClassId,Name = dto.Name,        };
        _db.Sections.Add(entity); await _db.SaveChangesAsync(cancellationToken); return entity.Id;
    }

    public async Task UpdateAsync(SectionUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Sections.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Section not found.");
        entity.SchoolClassId = dto.SchoolClassId;
        entity.Name = dto.Name;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Sections.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Section not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }
}

