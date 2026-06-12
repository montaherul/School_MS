using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SubjectService : ISubjectService
{
    private readonly IUnitOfWork _unitOfWork;

    public SubjectService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<PagedResult<SubjectListItemDto>> GetPagedAsync(int page, int pageSize, string? search, string? group = null, string? status = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<Subject>().Query().Where(x => !x.IsDeleted);

        if (!string.IsNullOrEmpty(search))
        {
            var lower = search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(lower) || x.NameBn.ToLower().Contains(lower) || x.Code.ToLower().Contains(lower));
        }

        if (!string.IsNullOrEmpty(group))
        {
            if (group.Equals("General", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => string.IsNullOrEmpty(x.SubjectGroup) || x.SubjectGroup == "General");
            else
                query = query.Where(x => x.SubjectGroup == group);
        }


        if (!string.IsNullOrEmpty(status))
        {
            if (status.Equals("active", StringComparison.OrdinalIgnoreCase)) query = query.Where(x => x.IsActive);
            else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase)) query = query.Where(x => !x.IsActive);
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new SubjectListItemDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                NameBn = x.NameBn,
                ShortName = x.ShortName,
                Category = x.Category,
                SubjectGroup = x.SubjectGroup,
                IsReligionSubject = x.IsReligionSubject,
                ReligionType = x.ReligionType,
                IsOptional = x.IsOptional,
                IsPractical = x.IsPractical,
                DefaultFullMarks = x.DefaultFullMarks,
                DefaultPassMarks = x.DefaultPassMarks,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            }).ToListAsync(ct);

        return new PagedResult<SubjectListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<SubjectUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<Subject>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity is null) return null;
        return new SubjectUpsertDto
        {
            Id = entity.Id,
            Code = entity.Code,
            Name = entity.Name,
            NameBn = entity.NameBn,
            ShortName = entity.ShortName,
            Category = entity.Category,
            SubjectGroup = entity.SubjectGroup,
            IsReligionSubject = entity.IsReligionSubject,
            ReligionType = entity.ReligionType,
            IsOptional = entity.IsOptional,
            IsPractical = entity.IsPractical,
            DefaultFullMarks = entity.DefaultFullMarks,
            DefaultPassMarks = entity.DefaultPassMarks,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(SubjectUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Subject>();
        if (await repo.AnyAsync(x => x.Code.ToUpper() == dto.Code.Trim().ToUpper() && !x.IsDeleted, ct))
            throw new InvalidOperationException("Subject code already exists");

        var entity = new Subject
        {
            Code = dto.Code.Trim().ToUpper(),
            Name = dto.Name.Trim(),
            NameBn = dto.NameBn.Trim(),
            ShortName = dto.ShortName?.Trim() ?? "",
            Category = dto.Category?.Trim() ?? "",
            SubjectGroup = dto.SubjectGroup ?? string.Empty,
            IsReligionSubject = dto.IsReligionSubject,
            ReligionType = dto.ReligionType,
            IsOptional = dto.IsOptional,
            IsMandatory = !dto.IsOptional,
            IsPractical = dto.IsPractical,
            DefaultFullMarks = dto.DefaultFullMarks,
            DefaultPassMarks = dto.DefaultPassMarks,
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            CreatedBy = createdBy
        };
        await repo.AddAsync(entity, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(SubjectUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Subject>();
        var entity = await repo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Subject not found.");

        if (await repo.AnyAsync(x => x.Id != dto.Id && x.Code.ToUpper() == dto.Code.Trim().ToUpper() && !x.IsDeleted, ct))
            throw new InvalidOperationException("Subject code already exists");

        entity.Code = dto.Code.Trim().ToUpper();
        entity.Name = dto.Name.Trim();
        entity.NameBn = dto.NameBn.Trim();
        entity.ShortName = dto.ShortName?.Trim() ?? "";
        entity.Category = dto.Category?.Trim() ?? "";
        entity.SubjectGroup = dto.SubjectGroup ?? string.Empty;
        entity.IsReligionSubject = dto.IsReligionSubject;
        entity.ReligionType = dto.ReligionType;
        entity.IsOptional = dto.IsOptional;
        entity.IsMandatory = !dto.IsOptional;
        entity.IsPractical = dto.IsPractical;
        entity.DefaultFullMarks = dto.DefaultFullMarks;
        entity.DefaultPassMarks = dto.DefaultPassMarks;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<Subject>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Subject not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IDictionary<string, List<SubjectListItemDto>>> GetGroupedSubjectsAsync(CancellationToken ct = default)
    {
        var subjects = await _unitOfWork.Repository<Subject>().Query()
            .AsNoTracking()
            .Where(s => !s.IsDeleted && s.IsActive)
            .OrderBy(s => s.DisplayOrder).ThenBy(s => s.Name)
            .Select(s => new SubjectListItemDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
                NameBn = s.NameBn,
                ShortName = s.ShortName,
                Category = s.Category,
                SubjectGroup = s.SubjectGroup,
                IsReligionSubject = s.IsReligionSubject,
                ReligionType = s.ReligionType,
                IsOptional = s.IsOptional,
                IsPractical = s.IsPractical,
                DefaultFullMarks = s.DefaultFullMarks,
                DefaultPassMarks = s.DefaultPassMarks,
                DisplayOrder = s.DisplayOrder,
                IsActive = s.IsActive
            })
            .ToListAsync(ct);

        return subjects
            .GroupBy(s => string.IsNullOrEmpty(s.SubjectGroup) ? "General" : s.SubjectGroup)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());
    }

    public async Task ToggleActiveAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<Subject>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Subject not found.");
        entity.IsActive = !entity.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task BulkActivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Subject>();
        var entities = await repo.Query().Where(x => ids.Contains(x.Id) && !x.IsDeleted).ToListAsync(ct);
        foreach (var e in entities) { e.IsActive = true; e.UpdatedBy = updatedBy; e.UpdatedAt = DateTime.UtcNow; }
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task BulkDeactivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Subject>();
        var entities = await repo.Query().Where(x => ids.Contains(x.Id) && !x.IsDeleted).ToListAsync(ct);
        foreach (var e in entities) { e.IsActive = false; e.UpdatedBy = updatedBy; e.UpdatedAt = DateTime.UtcNow; }
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task BulkImportAsync(List<SubjectUpsertDto> dtos, string createdBy, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<Subject>();
        foreach (var dto in dtos)
        {
            var exists = await repo.AnyAsync(x => x.Code.ToUpper() == dto.Code.Trim().ToUpper() && !x.IsDeleted, ct);
            if (exists) continue;

            var entity = new Subject
            {
                Code = dto.Code.Trim().ToUpper(),
                Name = dto.Name.Trim(),
                NameBn = dto.NameBn.Trim(),
                ShortName = dto.ShortName?.Trim() ?? "",
                Category = dto.Category?.Trim() ?? "",
                SubjectGroup = dto.SubjectGroup ?? string.Empty,
                IsReligionSubject = dto.IsReligionSubject,
                ReligionType = dto.ReligionType,
                IsOptional = dto.IsOptional,
                IsMandatory = !dto.IsOptional,
                IsPractical = dto.IsPractical,
                DefaultFullMarks = dto.DefaultFullMarks,
                DefaultPassMarks = dto.DefaultPassMarks,
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                CreatedBy = createdBy
            };
            await repo.AddAsync(entity, ct);
        }
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<List<SubjectListItemDto>> BulkExportAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<Subject>().Query()
            .Where(x => !x.IsDeleted)
            .OrderBy(x => x.Code)
            .Select(x => new SubjectListItemDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                NameBn = x.NameBn,
                ShortName = x.ShortName,
                Category = x.Category,
                SubjectGroup = x.SubjectGroup,
                IsReligionSubject = x.IsReligionSubject,
                ReligionType = x.ReligionType,
                IsOptional = x.IsOptional,
                IsPractical = x.IsPractical,
                DefaultFullMarks = x.DefaultFullMarks,
                DefaultPassMarks = x.DefaultPassMarks,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            })
            .ToListAsync(ct);
    }
}
