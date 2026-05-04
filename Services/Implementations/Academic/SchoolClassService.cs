using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SchoolClassService : ISchoolClassService
{
    private readonly SchoolDbContext _db;

    public SchoolClassService(SchoolDbContext db) { _db = db; }

    public async Task<PagedResult<SchoolClassListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 5, 100); var term = search?.Trim();
        var query = _db.Classes.Where(x => !x.IsDeleted); query = query.Where(x => term == null || x.Name.Contains(term));
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new SchoolClassListItemDto {
            Id = x.Id,Name = x.Name,SortOrder = x.SortOrder,        }).ToListAsync(cancellationToken);
        return new PagedResult<SchoolClassListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<SchoolClassUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Classes.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SchoolClassUpsertDto { Id = entity.Id,Name = entity.Name,SortOrder = entity.SortOrder,        };
    }

    public async Task<int> CreateAsync(SchoolClassUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new SchoolClass { CreatedBy = createdBy,Name = dto.Name,SortOrder = dto.SortOrder,        };
        _db.Classes.Add(entity); await _db.SaveChangesAsync(cancellationToken); return entity.Id;
    }

    public async Task UpdateAsync(SchoolClassUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Classes.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("SchoolClass not found.");
        entity.Name = dto.Name;
        entity.SortOrder = dto.SortOrder;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Classes.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("SchoolClass not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }
}

