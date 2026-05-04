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

    public async Task<PagedResult<SubjectListItemDto>> GetPagedAsync(
       int page,
       int pageSize,
       string? search,
       CancellationToken cancellationToken = default)
    {
        // ✅ Pagination safety
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);

        var term = search?.Trim();

        // ✅ Base query (No Tracking for performance)
        var query = _db.Subjects
            .AsNoTracking()
            .Where(x => !x.IsDeleted);

        // ✅ Improved search (Code + Name)
        if (!string.IsNullOrWhiteSpace(term))
        {
            query = query.Where(x =>
                x.Code.Contains(term) ||
                x.Name.Contains(term)
            );
        }

        // ✅ Total count
        var total = await query.CountAsync(cancellationToken);

        // ✅ Data fetch
        var items = await query
            .OrderByDescending(x => x.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new SubjectListItemDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<SubjectListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<SubjectUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Subjects.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SubjectUpsertDto { Id = entity.Id,Code = entity.Code,Name = entity.Name,        };
    }

    public async Task<int> CreateAsync(
     SubjectUpsertDto dto,
     string createdBy,
     CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException("Subject code is required");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Subject name is required");

        // 🔥 Normalize input
        var code = dto.Code.Trim().ToUpper();
        var name = dto.Name.Trim();

        // 🔥 Case-insensitive duplicate check
        var exists = await _db.Subjects.AnyAsync(
            x => !x.IsDeleted && x.Code.ToUpper() == code,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException("Subject code already exists");

        var entity = new Subject
        {
            Code = code,
            Name = name,
            CreatedBy = createdBy
        };

        _db.Subjects.Add(entity);
        await _db.SaveChangesAsync(cancellationToken);

        return entity.Id;
    }
    public async Task UpdateAsync(
     SubjectUpsertDto dto,
     string updatedBy,
     CancellationToken cancellationToken = default)
    {
        if (dto == null)
            throw new ArgumentNullException(nameof(dto));

        if (string.IsNullOrWhiteSpace(dto.Code))
            throw new ArgumentException("Subject code is required");

        if (string.IsNullOrWhiteSpace(dto.Name))
            throw new ArgumentException("Subject name is required");

        var entity = await _db.Subjects
            .FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Subject not found.");

        // 🔥 Normalize
        var code = dto.Code.Trim().ToUpper();
        var name = dto.Name.Trim();

        // 🔥 Duplicate check (exclude current record)
        var exists = await _db.Subjects.AnyAsync(
            x => x.Id != dto.Id &&
                 !x.IsDeleted &&
                 x.Code.ToUpper() == code,
            cancellationToken);

        if (exists)
            throw new InvalidOperationException("Subject code already exists");

        entity.Code = code;
        entity.Name = name;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }
    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _db.Subjects.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Subject not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow; await _db.SaveChangesAsync(cancellationToken);
    }
}

