using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SchoolClassService : ISchoolClassService
{
    private readonly IUnitOfWork _unitOfWork;

    public SchoolClassService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<PagedResult<SchoolClassListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<SchoolClass>();
        var query = repo.Query().Where(c => !c.IsDeleted && c.IsActive);

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(c => c.Name.Contains(search) || c.NameBn.Contains(search) || c.Code.Contains(search));

        var totalCount = await query.CountAsync(ct);
        var items = await query
            .OrderBy(c => c.SortOrder)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(c => new SchoolClassListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                NameBn = c.NameBn,
                Code = c.Code,
                SortOrder = c.SortOrder,
                Capacity = c.Capacity,
                IsGroupBased = c.IsGroupBased,
                IsActive = c.IsActive,
                SectionCount = c.Sections.Count(s => !s.IsDeleted)
            })
            .ToListAsync(ct);

        return new PagedResult<SchoolClassListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<SchoolClassUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<SchoolClass>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity is null) return null;
        return new SchoolClassUpsertDto
        {
            Id = entity.Id,
            Name = entity.Name,
            NameBn = entity.NameBn,
            Code = entity.Code,
            SortOrder = entity.SortOrder,
            Capacity = entity.Capacity,
            Description = entity.Description,
            IsGroupBased = entity.IsGroupBased,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(SchoolClassUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        if (!await IsNameUniqueAsync(dto.Name, null, ct))
            throw new InvalidOperationException("A class with this name already exists.");
        if (!string.IsNullOrWhiteSpace(dto.Code) && !await IsCodeUniqueAsync(dto.Code, null, ct))
            throw new InvalidOperationException("A class with this code already exists.");

        var entity = new SchoolClass
        {
            CreatedBy = createdBy,
            Name = dto.Name,
            NameBn = dto.NameBn,
            Code = dto.Code,
            SortOrder = dto.SortOrder,
            Capacity = dto.Capacity,
            Description = dto.Description,
            IsGroupBased = dto.IsGroupBased,
            IsActive = dto.IsActive
        };
        await _unitOfWork.Repository<SchoolClass>().AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(SchoolClassUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        if (!await IsNameUniqueAsync(dto.Name, dto.Id, ct))
            throw new InvalidOperationException("A class with this name already exists.");
        if (!string.IsNullOrWhiteSpace(dto.Code) && !await IsCodeUniqueAsync(dto.Code, dto.Id, ct))
            throw new InvalidOperationException("A class with this code already exists.");

        var entity = await _unitOfWork.Repository<SchoolClass>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("SchoolClass not found.");

        entity.Name = dto.Name;
        entity.NameBn = dto.NameBn;
        entity.Code = dto.Code;
        entity.SortOrder = dto.SortOrder;
        entity.Capacity = dto.Capacity;
        entity.Description = dto.Description;
        entity.IsGroupBased = dto.IsGroupBased;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var canDelete = await CanDeleteAsync(id, ct);
        if (!canDelete)
            throw new InvalidOperationException("Cannot delete class: it has sections, students, or subject mappings.");

        var entity = await _unitOfWork.Repository<SchoolClass>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("SchoolClass not found.");
        entity.IsDeleted = true;
        entity.IsActive = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<SchoolClassListItemDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<SchoolClass>().Query()
            .Where(c => !c.IsDeleted && c.IsActive)
            .OrderBy(c => c.SortOrder)
            .Select(c => new SchoolClassListItemDto
            {
                Id = c.Id,
                Name = c.Name,
                NameBn = c.NameBn,
                Code = c.Code,
                SortOrder = c.SortOrder,
                Capacity = c.Capacity,
                IsGroupBased = c.IsGroupBased,
                IsActive = c.IsActive
            })
            .ToListAsync(ct);
    }

    public async Task<SchoolClassListItemDto> CloneAsync(int id, string createdBy, CancellationToken ct = default)
    {
        var source = await _unitOfWork.Repository<SchoolClass>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Source class not found.");

        var clone = new SchoolClass
        {
            CreatedBy = createdBy,
            Name = source.Name + " (Clone)",
            NameBn = source.NameBn,
            Code = source.Code + "_CLONE",
            SortOrder = source.SortOrder + 1,
            Capacity = source.Capacity,
            Description = source.Description,
            IsGroupBased = source.IsGroupBased,
            IsActive = false
        };
        await _unitOfWork.Repository<SchoolClass>().AddAsync(clone, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return new SchoolClassListItemDto
        {
            Id = clone.Id,
            Name = clone.Name,
            NameBn = clone.NameBn,
            Code = clone.Code,
            SortOrder = clone.SortOrder,
            Capacity = clone.Capacity,
            IsGroupBased = clone.IsGroupBased,
            IsActive = clone.IsActive
        };
    }

    public async Task ArchiveAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<SchoolClass>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("SchoolClass not found.");
        entity.IsActive = false;
        entity.ArchivedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<SchoolClass>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("SchoolClass not found.");
        entity.IsActive = true;
        entity.ArchivedAt = null;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ToggleActiveAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<SchoolClass>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("SchoolClass not found.");
        entity.IsActive = !entity.IsActive;
        if (!entity.IsActive) entity.ArchivedAt = DateTime.UtcNow;
        else entity.ArchivedAt = null;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<bool> IsNameUniqueAsync(string name, int? excludeId = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<SchoolClass>().Query().Where(c => !c.IsDeleted && c.Name == name);
        if (excludeId.HasValue) query = query.Where(c => c.Id != excludeId.Value);
        return !await query.AnyAsync(ct);
    }

    public async Task<bool> IsCodeUniqueAsync(string code, int? excludeId = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<SchoolClass>().Query().Where(c => !c.IsDeleted && c.Code == code);
        if (excludeId.HasValue) query = query.Where(c => c.Id != excludeId.Value);
        return !await query.AnyAsync(ct);
    }

    public async Task<bool> CanDeleteAsync(int id, CancellationToken ct = default)
    {
        var sectionRepo = _unitOfWork.Repository<Section>();
        var hasSections = await sectionRepo.Query().AnyAsync(s => s.SchoolClassId == id && !s.IsDeleted, ct);
        if (hasSections) return false;

        var mappingRepo = _unitOfWork.Repository<ClassSubject>();
        var hasMappings = await mappingRepo.Query().AnyAsync(m => m.SchoolClassId == id && !m.IsDeleted, ct);
        if (hasMappings) return false;

        var studentRepo = _unitOfWork.Repository<Student>();
        var hasStudents = await studentRepo.Query().AnyAsync(st => st.ClassId == id && !st.IsDeleted, ct);
        if (hasStudents) return false;

        return true;
    }
}
