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

<<<<<<< HEAD
    public async Task<PagedResult<SubjectListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<Subject>().Query().Where(x => !x.IsDeleted);
        if (!string.IsNullOrEmpty(search))
        {
            var lower = search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(lower) || x.Code.ToLower().Contains(lower));
        }

=======
    public async Task<PagedResult<SubjectListItemDto>> GetPagedAsync(int page, int pageSize, string? search, string? group = null, string? status = null, CancellationToken cancellationToken = default)
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
            {
                query = query.Where(x => string.IsNullOrEmpty(x.SubjectGroup) || x.SubjectGroup == "General");
            }
            else
            {
                query = query.Where(x => x.SubjectGroup == group);
            }
        }

        if (!string.IsNullOrEmpty(status))
        {
            if (status.Equals("active", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => x.IsActive);
            }
            else if (status.Equals("inactive", StringComparison.OrdinalIgnoreCase))
            {
                query = query.Where(x => !x.IsActive);
            }
        }

>>>>>>> d8b24e6 (attendece and website curtomize)
        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new SubjectListItemDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
<<<<<<< HEAD
                IsReligionSubject = x.IsReligionSubject,
                ReligionType = x.ReligionType
=======
                NameBn = x.NameBn,
                SubjectGroup = x.SubjectGroup,
                IsReligionSubject = x.IsReligionSubject,
                ReligionType = x.ReligionType,
                IsOptional = x.IsOptional,
                IsPractical = x.IsPractical,
                IsActive = x.IsActive
>>>>>>> d8b24e6 (attendece and website curtomize)
            }).ToListAsync(cancellationToken);

        return new PagedResult<SubjectListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<SubjectUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Subject>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
<<<<<<< HEAD
        return new SubjectUpsertDto { Id = entity.Id, Code = entity.Code, Name = entity.Name, IsReligionSubject = entity.IsReligionSubject, ReligionType = entity.ReligionType };
=======
        return new SubjectUpsertDto 
        { 
            Id = entity.Id, 
            Code = entity.Code, 
            Name = entity.Name, 
            NameBn = entity.NameBn,
            SubjectGroup = entity.SubjectGroup,
            IsReligionSubject = entity.IsReligionSubject, 
            ReligionType = entity.ReligionType,
            IsOptional = entity.IsOptional,
            IsPractical = entity.IsPractical,
            IsActive = entity.IsActive
        };
>>>>>>> d8b24e6 (attendece and website curtomize)
    }

    public async Task<int> CreateAsync(SubjectUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<Subject>();
        if (await repo.AnyAsync(x => x.Code.ToUpper() == dto.Code.Trim().ToUpper() && !x.IsDeleted, cancellationToken))
            throw new InvalidOperationException("Subject code already exists");

<<<<<<< HEAD
        var entity = new Subject { Code = dto.Code.Trim().ToUpper(), Name = dto.Name.Trim(), IsReligionSubject = dto.IsReligionSubject, ReligionType = dto.ReligionType, CreatedBy = createdBy };
=======
        var entity = new Subject 
        { 
            Code = dto.Code.Trim().ToUpper(), 
            Name = dto.Name.Trim(), 
            NameBn = dto.NameBn.Trim(),
            SubjectGroup = dto.SubjectGroup,
            IsReligionSubject = dto.IsReligionSubject, 
            ReligionType = dto.ReligionType, 
            IsOptional = dto.IsOptional,
            IsMandatory = !dto.IsOptional,
            IsPractical = dto.IsPractical,
            IsActive = dto.IsActive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
>>>>>>> d8b24e6 (attendece and website curtomize)
        await repo.AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(SubjectUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<Subject>();
        var entity = await repo.FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Subject not found.");
        
        if (await repo.AnyAsync(x => x.Id != dto.Id && x.Code.ToUpper() == dto.Code.Trim().ToUpper() && !x.IsDeleted, cancellationToken))
            throw new InvalidOperationException("Subject code already exists");

<<<<<<< HEAD
        entity.Code = dto.Code.Trim().ToUpper(); entity.Name = dto.Name.Trim(); entity.IsReligionSubject = dto.IsReligionSubject; entity.ReligionType = dto.ReligionType;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
=======
        entity.Code = dto.Code.Trim().ToUpper(); 
        entity.Name = dto.Name.Trim(); 
        entity.NameBn = dto.NameBn.Trim();
        entity.SubjectGroup = dto.SubjectGroup;
        entity.IsReligionSubject = dto.IsReligionSubject; 
        entity.ReligionType = dto.ReligionType;
        entity.IsOptional = dto.IsOptional;
        entity.IsMandatory = !dto.IsOptional;
        entity.IsPractical = dto.IsPractical;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy; 
        entity.UpdatedAt = DateTime.UtcNow;
>>>>>>> d8b24e6 (attendece and website curtomize)
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Subject>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Subject not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
<<<<<<< HEAD
=======

>>>>>>> d8b24e6 (attendece and website curtomize)
    public async Task<IDictionary<string?, List<SubjectListItemDto>>> GetGroupedSubjectsAsync(CancellationToken ct = default)
    {
        var subjects = await _unitOfWork.Repository<Subject>().Query()
            .AsNoTracking()
            .Where(s => !s.IsDeleted)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Name)
            .Select(s => new SubjectListItemDto
            {
                Id = s.Id,
                Code = s.Code,
                Name = s.Name,
<<<<<<< HEAD
                SubjectGroup = s.SubjectGroup,
                IsReligionSubject = s.IsReligionSubject,
                ReligionType = s.ReligionType
=======
                NameBn = s.NameBn,
                SubjectGroup = s.SubjectGroup,
                IsReligionSubject = s.IsReligionSubject,
                ReligionType = s.ReligionType,
                IsOptional = s.IsOptional,
                IsPractical = s.IsPractical,
                IsActive = s.IsActive
>>>>>>> d8b24e6 (attendece and website curtomize)
            })
            .ToListAsync(ct);

        return subjects
<<<<<<< HEAD
            .GroupBy(s => s.SubjectGroup)
=======
            .GroupBy(s => s.SubjectGroup ?? "General")
>>>>>>> d8b24e6 (attendece and website curtomize)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}

