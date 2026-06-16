using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class ClassSubjectMappingService : IClassSubjectMappingService
{
    private readonly IUnitOfWork _unitOfWork;

    public ClassSubjectMappingService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<PagedResult<ClassSubjectListItemDto>> GetPagedAsync(
        int page,
        int pageSize,
        int? classId,
        string? groupName,
        string? search,
        CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<ClassSubject>().Query().AsNoTracking()
            .Include(x => x.SchoolClass)
            .Include(x => x.Subject)
            .Include(x => x.StudentGroup)
            .Where(x => !x.IsDeleted && !x.SchoolClass!.IsDeleted && !x.Subject!.IsDeleted);

        if (classId.HasValue && classId > 0)
            query = query.Where(x => x.SchoolClassId == classId.Value);

        if (!string.IsNullOrEmpty(groupName))
        {
            if (groupName.Equals("General", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => string.IsNullOrEmpty(x.GroupName) || x.GroupName == "General");
            else if (groupName.Equals("BusinessStudies", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => x.GroupName == "BusinessStudies" || x.GroupName == "Business Studies" || (x.StudentGroup != null && (x.StudentGroup.Name == "BusinessStudies" || x.StudentGroup.Name == "Business Studies")));
            else
                query = query.Where(x => x.GroupName == groupName || (x.StudentGroup != null && x.StudentGroup.Name == groupName));
        }

        if (!string.IsNullOrEmpty(search))
        {
            var lower = search.ToLower();
            query = query.Where(x =>
                x.SchoolClass!.Name.ToLower().Contains(lower) ||
                x.Subject!.Name.ToLower().Contains(lower) ||
                x.Subject!.NameBn.ToLower().Contains(lower) ||
                x.Subject!.Code.ToLower().Contains(lower) ||
                (x.GroupName != null && x.GroupName.ToLower().Contains(lower)));
        }

        var totalCount = await query.CountAsync(ct);

        var items = await query
            .OrderBy(x => x.SchoolClass!.SortOrder)
            .ThenBy(x => x.DisplayOrder)
            .ThenBy(x => x.Subject!.Code)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(x => new ClassSubjectListItemDto
            {
                Id = x.Id,
                SchoolClassId = x.SchoolClassId,
                SchoolClassName = x.SchoolClass != null ? x.SchoolClass.Name : string.Empty,
                SubjectId = x.SubjectId,
                SubjectCode = x.Subject != null ? x.Subject.Code : string.Empty,
                SubjectNameEn = x.Subject != null ? x.Subject.Name : string.Empty,
                SubjectNameBn = x.Subject != null ? x.Subject.NameBn : string.Empty,
                StudentGroupId = x.StudentGroupId,
                GroupName = x.GroupName,
                FullMarks = x.FullMarks,
                PassMarks = x.PassMarks,
                IsMandatory = x.IsMandatory,
                IsOptional = x.IsOptional,
                IsReligionSubject = x.IsReligionSubject,
                ReligionType = x.ReligionType,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            })
            .ToListAsync(ct);

        return new PagedResult<ClassSubjectListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<ClassSubjectUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        var entity = await _unitOfWork.Repository<ClassSubject>().Query().AsNoTracking()
            .Include(x => x.SchoolClass)
            .Include(x => x.Subject)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

        if (entity is null) return null;

        return new ClassSubjectUpsertDto
        {
            Id = entity.Id,
            SchoolClassId = entity.SchoolClassId,
            SubjectId = entity.SubjectId,
            StudentGroupId = entity.StudentGroupId,
            GroupName = entity.GroupName,
            FullMarks = entity.FullMarks,
            PassMarks = entity.PassMarks,
            DisplayOrder = entity.DisplayOrder,
            IsMandatory = entity.IsMandatory,
            IsOptional = entity.IsOptional,
            IsReligionSubject = entity.IsReligionSubject,
            ReligionType = entity.ReligionType,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateOrUpdateAsync(ClassSubjectUpsertDto dto, string userId, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<ClassSubject>();
        var groupName = string.IsNullOrEmpty(dto.GroupName) ? "General" : dto.GroupName;

        await ValidateGroupForClassAsync(dto.SchoolClassId, groupName, ct);

        int? resolvedGroupId = null;
        if (!string.IsNullOrEmpty(groupName) && !groupName.Equals("General", StringComparison.OrdinalIgnoreCase))
        {
            var grp = await _unitOfWork.Repository<StudentGroup>().FirstOrDefaultAsync(x => x.Name.Trim().ToUpper() == groupName.Trim().ToUpper() && !x.IsDeleted, ct);
            resolvedGroupId = grp?.Id;
        }

        if (dto.Id > 0)
        {
            var entity = await repo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
                ?? throw new InvalidOperationException("Class-Subject mapping not found.");

            entity.GroupName = groupName;
            entity.StudentGroupId = resolvedGroupId ?? dto.StudentGroupId;
            entity.FullMarks = dto.FullMarks;
            entity.PassMarks = dto.PassMarks;
            entity.DisplayOrder = dto.DisplayOrder;
            entity.IsOptional = dto.IsOptional;
            entity.IsMandatory = !dto.IsOptional;
            entity.IsReligionSubject = dto.IsReligionSubject;
            entity.ReligionType = dto.IsReligionSubject ? dto.ReligionType : null;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = userId;
            entity.UpdatedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync(ct);
            return entity.Id;
        }
        else
        {
            var duplicate = await repo.AnyAsync(x =>
                x.SchoolClassId == dto.SchoolClassId &&
                x.SubjectId == dto.SubjectId &&
                x.GroupName == groupName &&
                !x.IsDeleted, ct);

            if (duplicate)
                throw new InvalidOperationException("This subject is already mapped to the selected class and stream.");

            var subject = await _unitOfWork.Repository<Subject>().FirstOrDefaultAsync(x => x.Id == dto.SubjectId && !x.IsDeleted, ct)
                ?? throw new InvalidOperationException("Subject not found.");

            var entity = new ClassSubject
            {
                SchoolClassId = dto.SchoolClassId,
                SubjectId = dto.SubjectId,
                StudentGroupId = resolvedGroupId ?? dto.StudentGroupId,
                GroupName = groupName,
                FullMarks = dto.FullMarks,
                PassMarks = dto.PassMarks,
                DisplayOrder = dto.DisplayOrder,
                IsOptional = dto.IsOptional,
                IsMandatory = !dto.IsOptional,
                IsReligionSubject = dto.IsReligionSubject || subject.IsReligionSubject,
                ReligionType = dto.IsReligionSubject ? dto.ReligionType : subject.ReligionType,
                IsActive = dto.IsActive,
                CreatedBy = userId
            };

            await repo.AddAsync(entity, ct);
            await _unitOfWork.SaveChangesAsync(ct);
            return entity.Id;
        }
    }

    public async Task SaveAssignmentsAsync(ClassSubjectAssignmentDto dto, string userId, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<ClassSubject>();
        var groupName = string.IsNullOrEmpty(dto.GroupName) ? "General" : dto.GroupName;

        await ValidateGroupForClassAsync(dto.SchoolClassId, groupName, ct);

        int? resolvedGroupId = null;
        if (!string.IsNullOrEmpty(groupName) && !groupName.Equals("General", StringComparison.OrdinalIgnoreCase))
        {
            var grp = await _unitOfWork.Repository<StudentGroup>().FirstOrDefaultAsync(x => x.Name.Trim().ToUpper() == groupName.Trim().ToUpper() && !x.IsDeleted, ct);
            resolvedGroupId = grp?.Id;
        }

        foreach (var subId in dto.SubjectIds)
        {
            var existing = await repo.FirstOrDefaultAsync(x =>
                x.SchoolClassId == dto.SchoolClassId &&
                x.SubjectId == subId &&
                x.GroupName == groupName, ct);

            if (existing != null)
            {
                if (existing.IsDeleted)
                {
                    existing.IsDeleted = false;
                    existing.IsActive = true;
                    existing.UpdatedBy = userId;
                    existing.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                var subject = await _unitOfWork.Repository<Subject>().FirstOrDefaultAsync(x => x.Id == subId && !x.IsDeleted, ct);
                if (subject == null) continue;

                var mapping = new ClassSubject
                {
                    SchoolClassId = dto.SchoolClassId,
                    SubjectId = subId,
                    GroupName = groupName,
                    StudentGroupId = resolvedGroupId,
                    FullMarks = dto.FullMarks,
                    PassMarks = dto.PassMarks,
                    IsOptional = subject.IsOptional,
                    IsMandatory = !subject.IsOptional,
                    IsReligionSubject = subject.IsReligionSubject,
                    ReligionType = subject.ReligionType,
                    IsActive = true,
                    CreatedBy = userId
                };
                await repo.AddAsync(mapping, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    private async Task ValidateGroupForClassAsync(int classId, string groupName, CancellationToken ct)
    {
        var schoolClass = await _unitOfWork.Repository<SchoolClass>().FirstOrDefaultAsync(x => x.Id == classId && !x.IsDeleted, ct);
        if (schoolClass == null) throw new InvalidOperationException("Class not found.");

        var isSecondary = schoolClass.SortOrder >= 9;
        var allowed = isSecondary
            ? new[] { "General", "Science", "BusinessStudies", "Humanities" }
            : new[] { "General" };

        if (!allowed.Contains(groupName, StringComparer.OrdinalIgnoreCase))
        {
            var msg = isSecondary
                ? $"Class {schoolClass.SortOrder} allows groups: General, Science, BusinessStudies, Humanities."
                : $"Class {schoolClass.SortOrder} only allows General group.";
            throw new InvalidOperationException($"Invalid group '{groupName}' for {schoolClass.Name}. {msg}");
        }
    }

    public async Task DeleteAsync(int id, string userId, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<ClassSubject>();
        var entity = await repo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Class-Subject mapping not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = userId;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<SubjectListItemDto>> GetUnmappedSubjectsAsync(int classId, string? groupName, CancellationToken ct = default)
    {
        var mappedIds = await _unitOfWork.Repository<ClassSubject>().Query().AsNoTracking()
            .Where(x => x.SchoolClassId == classId && x.GroupName == groupName && !x.IsDeleted)
            .Select(x => x.SubjectId)
            .ToListAsync(ct);

        return await _unitOfWork.Repository<Subject>().Query().AsNoTracking()
            .Where(s => !s.IsDeleted && !mappedIds.Contains(s.Id) && s.IsActive)
            .OrderBy(s => s.Code)
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
    }
}
