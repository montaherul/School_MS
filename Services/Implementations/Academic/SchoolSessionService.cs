using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class SchoolSessionService : ISchoolSessionService
{
    private readonly IUnitOfWork _uow;

    public SchoolSessionService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<PagedResult<SchoolSessionListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var query = _uow.Repository<SchoolSession>().Query()
            .Include(x => x.AcademicYear)
            .Where(x => !x.IsDeleted)
            .AsNoTracking();

        if (!string.IsNullOrWhiteSpace(search))
            query = query.Where(x => x.Name.Contains(search));

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(x => x.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        var dtoItems = items.Select(x => new SchoolSessionListItemDto
        {
            Id = x.Id,
            Name = x.Name,
            StartDate = x.StartDate.ToString("yyyy-MM-dd"),
            EndDate = x.EndDate.ToString("yyyy-MM-dd"),
            IsCurrent = x.IsCurrent,
            IsActive = x.IsActive,
            TotalRecords = totalCount
        }).ToList();

        return new PagedResult<SchoolSessionListItemDto>
        {
            Items = dtoItems,
            Page = page,
            PageSize = pageSize,
            TotalItems = totalCount
        };
    }

    public async Task<SchoolSessionUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<SchoolSession>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new SchoolSessionUpsertDto
        {
            Id = entity.Id,
            AcademicYearId = entity.AcademicYearId,
            Name = entity.Name,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsCurrent = entity.IsCurrent,
            IsActive = entity.IsActive
        };
    }

    public async Task<List<SchoolSessionListItemDto>> GetActiveSessionsAsync(CancellationToken ct = default)
    {
        var entities = await _uow.Repository<SchoolSession>().Query()
            .Where(x => !x.IsDeleted && x.IsActive)
            .OrderByDescending(x => x.StartDate)
            .AsNoTracking()
            .ToListAsync(ct);

        return entities.Select(e => new SchoolSessionListItemDto
        {
            Id = e.Id,
            Name = e.Name,
            StartDate = e.StartDate.ToString("yyyy-MM-dd"),
            EndDate = e.EndDate.ToString("yyyy-MM-dd"),
            IsCurrent = e.IsCurrent,
            IsActive = e.IsActive
        }).ToList();
    }

    public async Task<int> CreateAsync(SchoolSessionUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new SchoolSession
        {
            AcademicYearId = dto.AcademicYearId,
            Name = dto.Name.Trim(),
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            IsCurrent = dto.IsCurrent,
            IsActive = dto.IsActive,
            CreatedBy = createdBy
        };
        await _uow.Repository<SchoolSession>().AddAsync(entity, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);
        return entity.Id;
    }

    public async Task UpdateAsync(SchoolSessionUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<SchoolSession>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("School Session not found.");
        entity.AcademicYearId = dto.AcademicYearId;
        entity.Name = dto.Name.Trim();
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.IsCurrent = dto.IsCurrent;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _uow.Repository<SchoolSession>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("School Session not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(cancellationToken);
    }
}
