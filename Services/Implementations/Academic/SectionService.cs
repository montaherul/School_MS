using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces;
using System.Data;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SectionService : ISectionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ISectionRepository _repo;

    public SectionService(IUnitOfWork unitOfWork, ISectionRepository repo)
    {
        _unitOfWork = unitOfWork;
        _repo = repo;
    }

    public async Task<PagedResult<SectionListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var spResults = await _repo.GetListSpAsync(page, pageSize, search);
        if (spResults.Count == 0)
            return new PagedResult<SectionListItemDto> { Items = [], Page = page, PageSize = pageSize, TotalItems = 0 };

        var totalCount = spResults[0].TotalRecords;
        var ids = spResults.Select(x => x.Id).ToList();

        var sectionEntities = await _unitOfWork.Repository<Section>().Query().AsNoTracking()
            .Where(s => ids.Contains(s.Id))
            .Select(s => new { s.Id, s.ParentSectionId, ParentName = s.ParentSection != null ? s.ParentSection.Name : (string?)null })
            .ToListAsync(cancellationToken);
        var parentLookup = sectionEntities.ToDictionary(x => x.Id);

        var items = spResults.Select(x =>
        {
            var entity = parentLookup.GetValueOrDefault(x.Id);
            return new SectionListItemDto
            {
                Id = x.Id,
                Name = x.Name,
                SchoolClassId = x.SchoolClassId,
                ClassName = x.ClassName,
                StudentCount = x.StudentCount,
                ParentSectionId = entity?.ParentSectionId,
                GroupName = entity?.ParentName
            };
        }).ToList();

        return new PagedResult<SectionListItemDto>
        {
            Items = items,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<SectionUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Section>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SectionUpsertDto
        {
            Id = entity.Id,
            SchoolClassId = entity.SchoolClassId,
            Name = entity.Name,
            ParentSectionId = entity.ParentSectionId
        };
    }

    public async Task<int> CreateAsync(SectionUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new Section
        {
            CreatedBy = createdBy,
            SchoolClassId = dto.SchoolClassId,
            Name = dto.Name,
            ParentSectionId = dto.ParentSectionId
        };
        await _unitOfWork.Repository<Section>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(SectionUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Section>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Section not found.");
        entity.SchoolClassId = dto.SchoolClassId;
        entity.Name = dto.Name;
        entity.ParentSectionId = dto.ParentSectionId;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Section>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Section not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<IEnumerable<SectionOptionDto>> GetByClassIdAsync(int classId, int? studentGroupId = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<Section>().Query()
            .Where(s => s.SchoolClassId == classId && !s.IsDeleted);

        if (studentGroupId.HasValue)
            query = query.Where(s => s.StudentGroupId == studentGroupId.Value);

        var sections = await query
            .Include(s => s.ParentSection)
            .ToListAsync(ct);

        // Filter for leaf sections: those that are NOT parents of any other active section in this class
        var parentIds = sections.Where(s => s.ParentSectionId != null).Select(s => s.ParentSectionId).Distinct().ToList();
        var leafSections = sections.Where(s => !parentIds.Contains(s.Id)).ToList();

        var studentRepo = _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>();

        var sectionIds = leafSections.Select(s => s.Id).ToList();
        var counts = sectionIds.Count > 0
            ? await studentRepo.Query()
                .Where(st => sectionIds.Contains(st.SectionId) && !st.IsDeleted)
                .GroupBy(st => st.SectionId)
                .Select(g => new { SectionId = g.Key, Count = g.Count() })
                .ToListAsync(ct)
            : [];
        var countDict = counts.ToDictionary(c => c.SectionId, c => c.Count);

        return leafSections.Select(s => new SectionOptionDto
        {
            Id = s.Id,
            Name = s.Name,
            GroupName = s.ParentSection?.Name ?? "General",
            StudentCount = countDict.GetValueOrDefault(s.Id, 0),
            Capacity = s.Capacity
        }).ToList();
    }

    public async Task<IEnumerable<SectionListItemDto>> GetGroupsByClassIdAsync(int classId, CancellationToken ct = default)
    {
        var sections = await _unitOfWork.Repository<Section>().Query()
            .Include(s => s.StudentGroup)
            .Where(s => s.SchoolClassId == classId && s.ParentSectionId == null && !s.IsDeleted)
            .Select(s => new SectionListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                StudentGroupId = s.StudentGroupId,
                GroupName = s.StudentGroup != null ? s.StudentGroup.Name : null
            })
            .ToListAsync(ct);

        var displayKey = new Func<SectionListItemDto, string>(s => s.GroupName ?? s.Name);

        return sections
            .GroupBy(s => displayKey(s))
            .Select(g => g.First())
            .OrderBy(s => displayKey(s))
            .ToList();
    }

    public async Task<IEnumerable<SectionListItemDto>> GetStudentGroupsByClassIdAsync(int classId, CancellationToken ct = default)
    {
        var schoolClass = await _unitOfWork.Repository<SchoolClass>().Query()
            .Where(c => c.Id == classId && !c.IsDeleted)
            .FirstOrDefaultAsync(ct);

        if (schoolClass == null)
            return [];

        var groups = await _unitOfWork.Repository<StudentGroup>().Query()
            .Where(g => g.IsActive && !g.IsDeleted
                && g.MinClass <= schoolClass.SortOrder
                && g.MaxClass >= schoolClass.SortOrder)
            .OrderBy(g => g.DisplayOrder)
            .ToListAsync(ct);

        return groups.Select(g => new SectionListItemDto
        {
            Id = g.Id,
            Name = g.Name,
            StudentGroupId = g.Id
        }).ToList();
    }

    public async Task<int> CreateAjaxAsync(int classId, string name, int? parentId, string createdBy, CancellationToken ct = default)
    {
        int? studentGroupId = null;
        if (parentId.HasValue)
        {
            var parent = await _unitOfWork.Repository<Section>().GetByIdAsync(parentId.Value);
            studentGroupId = parent?.StudentGroupId;
        }
        var section = new Section { SchoolClassId = classId, Name = name, ParentSectionId = parentId, StudentGroupId = studentGroupId, CreatedBy = createdBy, CreatedAt = DateTime.UtcNow, Capacity = 50 };

        await _unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await _unitOfWork.Repository<Section>().AddAsync(section, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            if (parentId.HasValue)
            {
                var csRepo = _unitOfWork.Repository<ClassSubject>();
                var parentSubjects = await csRepo.ListAsync(cs => cs.SectionId == parentId.Value && !cs.IsDeleted);
                if (parentSubjects.Any())
                {
                    var newSubjects = parentSubjects.Select(ps => new ClassSubject { SchoolClassId = ps.SchoolClassId, SubjectId = ps.SubjectId, SectionId = section.Id, CreatedBy = createdBy, CreatedAt = DateTime.UtcNow }).ToList();
                    await csRepo.AddRangeAsync(newSubjects, ct);
                    await _unitOfWork.SaveChangesAsync(ct);
                }
            }
        }, ct);

        return section.Id;
    }

    public async Task<IEnumerable<object>> GetAdmissionSectionsAsync(int classId, CancellationToken ct = default)
    {
        var sections = await _unitOfWork.Repository<Section>().Query()
            .AsNoTracking()
            .Where(s => s.SchoolClassId == classId && !s.IsDeleted)
            .Include(s => s.ParentSection)
            .ToListAsync(ct);

        var studentRepo = _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>();

        var sectionIds = sections.Select(s => s.Id).ToList();
        var counts = sectionIds.Count > 0
            ? await studentRepo.Query()
                .Where(st => sectionIds.Contains(st.SectionId) && !st.IsDeleted && st.Status == StudentStatus.Active)
                .GroupBy(st => st.SectionId)
                .Select(g => new { SectionId = g.Key, Count = g.Count() })
                .ToListAsync(ct)
            : [];
        var countDict = counts.ToDictionary(c => c.SectionId, c => c.Count);

        var allSections = sections.Select(section => new SectionAdmissionInfo
        {
            Id = section.Id,
            Name = section.Name,
            Capacity = section.Capacity,
            ParentSectionId = section.ParentSectionId,
            ParentName = section.ParentSection?.Name,
            StudentGroupId = section.StudentGroupId,
            StudentCount = countDict.GetValueOrDefault(section.Id, 0)
        }).ToList();

        var hasChildren = sections.Any(s => s.ParentSectionId != null);
        if (hasChildren)
        {
            return allSections.Where(s => s.ParentSectionId != null).Select(s => new
            {
                id = s.Id, name = s.Name, displayName = $"{s.Name} ({s.StudentCount}/{s.Capacity}){(s.StudentCount >= s.Capacity ? " - FULL" : "")}",
                groupName = s.ParentName ?? "", parentSectionId = s.ParentSectionId, studentGroupId = s.StudentGroupId,
                studentCount = s.StudentCount, capacity = s.Capacity, isFull = s.StudentCount >= s.Capacity
            }).ToList();
        }
        return allSections.Select(s => new
        {
            id = s.Id, name = s.Name, displayName = $"{s.Name} ({s.StudentCount}/{s.Capacity}){(s.StudentCount >= s.Capacity ? " - FULL" : "")}",
            groupName = "", studentGroupId = (int?)null,
            studentCount = s.StudentCount, capacity = s.Capacity, isFull = s.StudentCount >= s.Capacity
        }).ToList();
    }

    private sealed class SectionAdmissionInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int? ParentSectionId { get; set; }
        public string? ParentName { get; set; }
        public int? StudentGroupId { get; set; }
        public int StudentCount { get; set; }
    }

    public async Task<IEnumerable<SectionOptionDto>> GetSectionsByClassWithFilterAsync(int classId, bool isStaff, List<int>? assignedSectionIds, int? studentGroupId, CancellationToken ct)
    {
        var sections = await GetByClassIdAsync(classId, studentGroupId, ct);

        if (!isStaff && assignedSectionIds != null)
        {
            sections = sections.Where(s => assignedSectionIds.Contains(s.Id)).ToList();
        }

        return sections;
    }

    public async Task AssignStudentToSectionAsync(int studentId, int sectionId, CancellationToken ct = default)
    {
        await _unitOfWork.Repository<Section>().ExecuteStoredProcAsync<object>(
            "sp_AssignStudentToSection", studentId, sectionId);
    }

    public async Task<IEnumerable<dynamic>> GetAvailableClassesAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<SchoolClass>().Query()
            .Where(c => !c.IsDeleted && _unitOfWork.Repository<Section>().Query().Any(s => s.SchoolClassId == c.Id && !s.IsDeleted))
            .OrderBy(c => c.SortOrder)
            .Select(c => (dynamic)new { Id = c.Id, Name = c.Name, SortOrder = c.SortOrder, IsGroupBased = c.IsGroupBased })
            .ToListAsync(ct);
    }
}

