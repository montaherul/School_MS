using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces;

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
            .Include(x => x.ClassSubjectGroups)
                .ThenInclude(csg => csg.StudentGroup)
            .Where(x => !x.IsDeleted && !x.SchoolClass!.IsDeleted && !x.Subject!.IsDeleted);

        if (classId.HasValue && classId > 0)
            query = query.Where(x => x.SchoolClassId == classId.Value);

        if (!string.IsNullOrEmpty(groupName))
        {
            if (groupName.Equals("General", StringComparison.OrdinalIgnoreCase))
                query = query.Where(x => string.IsNullOrEmpty(x.GroupName) || x.GroupName == "General");
            else
                query = query.Where(x => x.GroupName == groupName);
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
                SelectedGroupIds = x.ClassSubjectGroups.Where(csg => !csg.IsDeleted).Select(csg => csg.StudentGroupId).ToList(),
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
            .Include(x => x.ClassSubjectGroups)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

        if (entity is null) return null;

        return new ClassSubjectUpsertDto
        {
            Id = entity.Id,
            SchoolClassId = entity.SchoolClassId,
            SubjectId = entity.SubjectId,
            SelectedGroupIds = entity.ClassSubjectGroups.Where(csg => !csg.IsDeleted).Select(csg => csg.StudentGroupId).ToList(),
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

        var groupRepo = _unitOfWork.Repository<StudentGroup>();
        var csgRepo = _unitOfWork.Repository<ClassSubjectGroup>();

        if (dto.Id > 0)
        {
            var entity = await repo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
                ?? throw new InvalidOperationException("Class-Subject mapping not found.");

            entity.GroupName = groupName;
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

            // Sync junction table: remove old links, add selected
            var existingGroups = await csgRepo.Query().Where(csg => csg.ClassSubjectId == entity.Id && !csg.IsDeleted).ToListAsync(ct);
            foreach (var old in existingGroups)
            {
                old.IsDeleted = true;
                old.UpdatedBy = userId;
                old.UpdatedAt = DateTime.UtcNow;
            }
            foreach (var gid in dto.SelectedGroupIds)
            {
                if (!existingGroups.Any(csg => csg.StudentGroupId == gid))
                {
                    await csgRepo.AddAsync(new ClassSubjectGroup { ClassSubjectId = entity.Id, StudentGroupId = gid, CreatedBy = userId }, ct);
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
            return entity.Id;
        }
        else
        {
            var duplicate = await repo.AnyAsync(x =>
                x.SchoolClassId == dto.SchoolClassId &&
                x.SubjectId == dto.SubjectId &&
                !x.IsDeleted, ct);

            if (duplicate)
                throw new InvalidOperationException("This subject is already mapped to the selected class.");

            var subject = await _unitOfWork.Repository<Subject>().FirstOrDefaultAsync(x => x.Id == dto.SubjectId && !x.IsDeleted, ct)
                ?? throw new InvalidOperationException("Subject not found.");

            var entity = new ClassSubject
            {
                SchoolClassId = dto.SchoolClassId,
                SubjectId = dto.SubjectId,
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

            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                await repo.AddAsync(entity, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                // Create junction entries for selected groups
                foreach (var gid in dto.SelectedGroupIds)
                {
                    await csgRepo.AddAsync(new ClassSubjectGroup { ClassSubjectId = entity.Id, StudentGroupId = gid, CreatedBy = userId }, ct);
                }
                if (dto.SelectedGroupIds.Count > 0)
                    await _unitOfWork.SaveChangesAsync(ct);
            }, ct);

            return entity.Id;
        }
    }

    public async Task SaveAssignmentsAsync(ClassSubjectAssignmentDto dto, string userId, CancellationToken ct = default)
    {
        var repo = _unitOfWork.Repository<ClassSubject>();
        var groupName = string.IsNullOrEmpty(dto.GroupName) ? "General" : dto.GroupName;

        await ValidateGroupForClassAsync(dto.SchoolClassId, groupName, ct);

        var groupRepo = _unitOfWork.Repository<StudentGroup>();
        var csgRepo = _unitOfWork.Repository<ClassSubjectGroup>();

        int? resolvedGroupId = null;
        if (!string.IsNullOrEmpty(groupName) && !groupName.Equals("General", StringComparison.OrdinalIgnoreCase))
        {
            var grp = await groupRepo.FirstOrDefaultAsync(x => x.Name.Trim().ToUpper() == groupName.Trim().ToUpper() && !x.IsDeleted, ct);
            resolvedGroupId = grp?.Id;
        }

        // Batch: fetch existing mappings once
        var existingMappings = await repo.Query()
            .Where(x => x.SchoolClassId == dto.SchoolClassId && dto.SubjectIds.Contains(x.SubjectId))
            .ToListAsync(ct);
        var existingLookup = existingMappings.ToDictionary(x => x.SubjectId);

        // Batch: fetch referenced subjects once
        var neededSubjects = dto.SubjectIds.Except(existingLookup.Keys).ToList();
        var subjects = neededSubjects.Count > 0
            ? await _unitOfWork.Repository<Subject>().Query()
                .Where(x => neededSubjects.Contains(x.Id) && !x.IsDeleted)
                .ToListAsync(ct)
            : [];
        var subjectLookup = subjects.ToDictionary(x => x.Id);

        // Batch: fetch existing junctions for existing mappings once
        var existingMappingIds = existingMappings.Select(x => x.Id).ToList();
        var existingJunctionLookup = existingMappingIds.Count > 0
            ? (await csgRepo.Query()
                .Where(csg => existingMappingIds.Contains(csg.ClassSubjectId) && !csg.IsDeleted)
                .ToListAsync(ct))
                .GroupBy(csg => csg.ClassSubjectId)
                .ToDictionary(g => g.Key, g => g.ToList())
            : [];

        var newMappings = new List<ClassSubject>();
        var newJunctions = new List<ClassSubjectGroup>();

        foreach (var subId in dto.SubjectIds)
        {
            if (existingLookup.TryGetValue(subId, out var existing))
            {
                if (existing.IsDeleted)
                {
                    existing.IsDeleted = false;
                    existing.IsActive = true;
                    existing.UpdatedBy = userId;
                    existing.UpdatedAt = DateTime.UtcNow;
                }

                // Ensure junction entry exists for this group
                if (resolvedGroupId.HasValue)
                {
                    var existingJunctions = existingJunctionLookup.GetValueOrDefault(existing.Id);
                    var hasJunction = existingJunctions?.Any(csg => csg.StudentGroupId == resolvedGroupId.Value) ?? false;
                    if (!hasJunction)
                    {
                        newJunctions.Add(new ClassSubjectGroup { ClassSubjectId = existing.Id, StudentGroupId = resolvedGroupId.Value, CreatedBy = userId });
                    }
                }
            }
            else if (subjectLookup.TryGetValue(subId, out var subject))
            {
                newMappings.Add(new ClassSubject
                {
                    SchoolClassId = dto.SchoolClassId,
                    SubjectId = subId,
                    GroupName = groupName,
                    FullMarks = dto.FullMarks,
                    PassMarks = dto.PassMarks,
                    IsOptional = subject.IsOptional,
                    IsMandatory = !subject.IsOptional,
                    IsReligionSubject = subject.IsReligionSubject,
                    ReligionType = subject.ReligionType,
                    IsActive = true,
                    CreatedBy = userId
                });
            }
        }

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            if (newMappings.Count > 0)
            {
                await repo.AddRangeAsync(newMappings, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                // Create junction entries for newly created mappings
                if (resolvedGroupId.HasValue)
                {
                    foreach (var m in newMappings)
                    {
                        newJunctions.Add(new ClassSubjectGroup { ClassSubjectId = m.Id, StudentGroupId = resolvedGroupId.Value, CreatedBy = userId });
                    }
                }
            }

            if (newJunctions.Count > 0)
            {
                await csgRepo.AddRangeAsync(newJunctions, ct);
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }, ct);
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

        // Soft-delete junction records too
        var csgRepo = _unitOfWork.Repository<ClassSubjectGroup>();
        var groups = await csgRepo.Query().Where(csg => csg.ClassSubjectId == id && !csg.IsDeleted).ToListAsync(ct);
        foreach (var g in groups)
        {
            g.IsDeleted = true;
            g.UpdatedBy = userId;
            g.UpdatedAt = DateTime.UtcNow;
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task SeedMappingsAsync(CancellationToken ct = default)
    {
        await _unitOfWork.Repository<ClassSubject>().ExecuteStoredProcAsync<object>("sp_SeedClassSubjectMappings_BD", Array.Empty<object>());
    }

    public async Task<IEnumerable<SubjectListItemDto>> GetUnmappedSubjectsAsync(int classId, string? groupName, CancellationToken ct = default)
    {
        var mappedIds = await _unitOfWork.Repository<ClassSubject>().Query().AsNoTracking()
            .Where(x => x.SchoolClassId == classId && !x.IsDeleted)
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
