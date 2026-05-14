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

    public async Task<PagedResult<SubjectListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _unitOfWork.Repository<Subject>().Query().Where(x => !x.IsDeleted);
        if (!string.IsNullOrEmpty(search))
        {
            var lower = search.ToLower();
            query = query.Where(x => x.Name.ToLower().Contains(lower) || x.Code.ToLower().Contains(lower));
        }

        var totalCount = await query.CountAsync(cancellationToken);
        var items = await query.OrderBy(x => x.Code)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new SubjectListItemDto
            {
                Id = x.Id,
                Code = x.Code,
                Name = x.Name,
                IsReligionSubject = x.IsReligionSubject,
                ReligionType = x.ReligionType
            }).ToListAsync(cancellationToken);

        return new PagedResult<SubjectListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<SubjectUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Subject>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SubjectUpsertDto { Id = entity.Id, Code = entity.Code, Name = entity.Name, IsReligionSubject = entity.IsReligionSubject, ReligionType = entity.ReligionType };
    }

    public async Task<int> CreateAsync(SubjectUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var repo = _unitOfWork.Repository<Subject>();
        if (await repo.AnyAsync(x => x.Code.ToUpper() == dto.Code.Trim().ToUpper() && !x.IsDeleted, cancellationToken))
            throw new InvalidOperationException("Subject code already exists");

        var entity = new Subject { Code = dto.Code.Trim().ToUpper(), Name = dto.Name.Trim(), IsReligionSubject = dto.IsReligionSubject, ReligionType = dto.ReligionType, CreatedBy = createdBy };
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

        entity.Code = dto.Code.Trim().ToUpper(); entity.Name = dto.Name.Trim(); entity.IsReligionSubject = dto.IsReligionSubject; entity.ReligionType = dto.ReligionType;
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<Subject>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken) ?? throw new InvalidOperationException("Subject not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
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
                SubjectGroup = s.SubjectGroup,
                IsReligionSubject = s.IsReligionSubject,
                ReligionType = s.ReligionType
            })
            .ToListAsync(ct);

        return subjects
            .GroupBy(s => s.SubjectGroup)
            .OrderBy(g => g.Key)
            .ToDictionary(g => g.Key, g => g.ToList());
    }
}

