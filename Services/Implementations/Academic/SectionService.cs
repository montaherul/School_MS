using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces;
using System.Data;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SectionService : ISectionService
{
    private readonly IUnitOfWork _unitOfWork;

    public SectionService(IUnitOfWork unitOfWork) { _unitOfWork = unitOfWork; }

    public async Task<PagedResult<SectionListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        // For simplicity, I'll use the generic repository Query() for now, 
        // but real apps should use repositories for SP calls as shown in AdmissionRepository.
        
        var query = _unitOfWork.Repository<Section>().Query()
            .Include(s => s.SchoolClass)
            .Include(s => s.ParentSection)
            .Where(s => !s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            var lower = search.ToLower();
            query = query.Where(s => s.Name.ToLower().Contains(lower) || s.SchoolClass!.Name.ToLower().Contains(lower));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query
            .OrderBy(s => s.SchoolClass!.Name).ThenBy(s => s.Name)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(s => new SectionListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                SchoolClassId = s.SchoolClassId,
                ClassName = s.SchoolClass!.Name,
                ParentSectionId = s.ParentSectionId,
                GroupName = s.ParentSection != null ? s.ParentSection.Name : null
            })
            .ToListAsync(cancellationToken);

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

    public async Task<IEnumerable<SectionOptionDto>> GetByClassIdAsync(int classId, CancellationToken ct = default)
    {
        // Fetch leaf sections (sections that are NOT groups, or are groups themselves if they have no children?)
        // In this system, sections can have ParentSectionId.
        // We want sections that students can actually be in.
        
        var sections = await _unitOfWork.Repository<Section>().Query()
            .Where(s => s.SchoolClassId == classId && !s.IsDeleted)
            .Include(s => s.ParentSection)
            .ToListAsync(ct);

        // Filter for leaf sections: those that are NOT parents of any other active section in this class
        var parentIds = sections.Where(s => s.ParentSectionId != null).Select(s => s.ParentSectionId).Distinct().ToList();
        var leafSections = sections.Where(s => !parentIds.Contains(s.Id)).ToList();

        var studentRepo = _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>();
        
        var result = new List<SectionOptionDto>();
        foreach (var s in leafSections)
        {
            var studentCount = await studentRepo.CountAsync(st => st.SectionId == s.Id && !st.IsDeleted, ct);
            result.Add(new SectionOptionDto
            {
                Id = s.Id,
                Name = s.Name,
                GroupName = s.ParentSection?.Name ?? "General",
                StudentCount = studentCount,
                Capacity = s.Capacity
            });
        }
        return result;
    }

    public async Task<IEnumerable<SectionListItemDto>> GetGroupsByClassIdAsync(int classId, CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<Section>().Query()
            .Where(s => s.SchoolClassId == classId && s.ParentSectionId == null && !s.IsDeleted)
            .Select(s => new SectionListItemDto
            {
                Id = s.Id,
                Name = s.Name,
                StudentGroupId = s.StudentGroupId
            })
            .ToListAsync(ct);
    }

    public async Task<int> CreateAjaxAsync(int classId, string name, int? parentId, string createdBy, CancellationToken ct = default)
    {
        var section = new Section { SchoolClassId = classId, Name = name, ParentSectionId = parentId, CreatedBy = createdBy, CreatedAt = DateTime.UtcNow, Capacity = 50 };
        await _unitOfWork.Repository<Section>().AddAsync(section, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        if (parentId.HasValue)
        {
            var csRepo = _unitOfWork.Repository<ClassSubject>();
            var parentSubjects = await csRepo.ListAsync(cs => cs.SectionId == parentId.Value && !cs.IsDeleted);
            if (parentSubjects.Any())
            {
                var newSubjects = parentSubjects.Select(ps => new ClassSubject { SchoolClassId = ps.SchoolClassId, SubjectId = ps.SubjectId, SectionId = section.Id, CreatedBy = createdBy, CreatedAt = DateTime.UtcNow }).ToList();
                foreach (var ns in newSubjects) await csRepo.AddAsync(ns, ct);
                await _unitOfWork.SaveChangesAsync(ct);
            }
        }
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

        var allSections = new List<SectionAdmissionInfo>();
        foreach (var section in sections)
        {
            var studentCount = await studentRepo.CountAsync(st => st.SectionId == section.Id && !st.IsDeleted && st.Status == StudentStatus.Active, ct);
            allSections.Add(new SectionAdmissionInfo
            {
                Id = section.Id,
                Name = section.Name,
                Capacity = section.Capacity,
                ParentSectionId = section.ParentSectionId,
                ParentName = section.ParentSection != null ? section.ParentSection.Name : null,
                StudentCount = studentCount
            });
        }

        var hasChildren = sections.Any(s => s.ParentSectionId != null);
        if (hasChildren)
        {
            return allSections.Where(s => s.ParentSectionId != null).Select(s => new
            {
                id = s.Id, name = s.Name, displayName = $"{s.Name} ({s.StudentCount}/{s.Capacity}){(s.StudentCount >= s.Capacity ? " - FULL" : "")}",
                groupName = s.ParentName ?? "", parentSectionId = s.ParentSectionId, studentCount = s.StudentCount, capacity = s.Capacity, isFull = s.StudentCount >= s.Capacity
            }).ToList();
        }
        return allSections.Select(s => new
        {
            id = s.Id, name = s.Name, displayName = $"{s.Name} ({s.StudentCount}/{s.Capacity}){(s.StudentCount >= s.Capacity ? " - FULL" : "")}",
            groupName = "", studentCount = s.StudentCount, capacity = s.Capacity, isFull = s.StudentCount >= s.Capacity
        }).ToList();
    }

    private sealed class SectionAdmissionInfo
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int Capacity { get; set; }
        public int? ParentSectionId { get; set; }
        public string? ParentName { get; set; }
        public int StudentCount { get; set; }
    }

    public async Task<IEnumerable<dynamic>> GetAvailableClassesAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<SchoolClass>().Query()
            .Where(c => !c.IsDeleted && _unitOfWork.Repository<Section>().Query().Any(s => s.SchoolClassId == c.Id && !s.IsDeleted))
            .OrderBy(c => c.SortOrder)
            .Select(c => (dynamic)new { Id = c.Id, Name = c.Name, IsGroupBased = c.IsGroupBased })
            .ToListAsync(ct);
    }
}

