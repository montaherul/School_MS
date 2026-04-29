using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SubjectService : ISubjectService
{
    private readonly SchoolDbContext _db;

    public SubjectService(SchoolDbContext db) { _db = db; }

    public async Task<PagedResult<SubjectListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1); pageSize = Math.Clamp(pageSize, 5, 100); var term = search?.Trim();
        var query = _db.Subjects.Where(x => !x.IsDeleted); query = query.Where(x => term == null || x.Code.Contains(term));
        var total = await query.CountAsync(cancellationToken);
        var items = await query.OrderByDescending(x => x.Id).Skip((page - 1) * pageSize).Take(pageSize).Select(x => new SubjectListItemDto {
            Id = x.Id,Code = x.Code,Name = x.Name,        }).ToListAsync(cancellationToken);
        return new PagedResult<SubjectListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<SubjectUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Subjects.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SubjectUpsertDto { Id = entity.Id,Code = entity.Code,Name = entity.Name,        };
    }

    public async Task<int> CreateAsync(SubjectUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new Subject { CreatedBy = createdBy,Code = dto.Code,Name = dto.Name,        };
        _db.Subjects.Add(entity); await _db.SaveChangesAsync(cancellationToken); return entity.Id;
    }

    public async Task UpdateAsync(SubjectUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Subjects.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Subject not found.");
        entity.Code = dto.Code;
        entity.Name = dto.Name;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Subjects.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Subject not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }
}

