using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class StudentGroupService : IStudentGroupService
{
    private readonly IUnitOfWork _uow;

    public StudentGroupService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<StudentGroupListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var query = _uow.Repository<StudentGroup>().Query().AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var term = search.Trim().ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(term) || x.Code.ToLower().Contains(term));
        }

        var total = await query.CountAsync(ct);
        var items = await query
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new StudentGroupListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Description = x.Description,
                MinClass = x.MinClass,
                MaxClass = x.MaxClass,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive,
                TotalRecords = total
            })
            .ToListAsync(ct);

        return items;
    }

    public async Task<StudentGroupUpsertDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<StudentGroup>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity is null) return null;

        return new StudentGroupUpsertDto
        {
            Id = entity.Id,
            Name = entity.Name,
            Code = entity.Code,
            Description = entity.Description,
            MinClass = entity.MinClass,
            MaxClass = entity.MaxClass,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(StudentGroupUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        var entity = new StudentGroup
        {
            Name = dto.Name,
            Code = dto.Code,
            Description = dto.Description,
            MinClass = dto.MinClass,
            MaxClass = dto.MaxClass,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<StudentGroup>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(StudentGroupUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<StudentGroup>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Student group not found.");

        entity.Name = dto.Name;
        entity.Code = dto.Code;
        entity.Description = dto.Description;
        entity.MinClass = dto.MinClass;
        entity.MaxClass = dto.MaxClass;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<StudentGroup>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Student group not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<List<StudentGroupListItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _uow.Repository<StudentGroup>().Query().AsNoTracking()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.DisplayOrder)
            .ThenBy(x => x.Name)
            .Select(x => new StudentGroupListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                Code = x.Code,
                Description = x.Description,
                MinClass = x.MinClass,
                MaxClass = x.MaxClass,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            })
            .ToListAsync(ct);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId, CancellationToken ct = default)
    {
        return !await _uow.Repository<StudentGroup>().AnyAsync(
            x => x.Code == code && !x.IsDeleted && (excludeId == null || x.Id != excludeId.Value), ct);
    }
}
