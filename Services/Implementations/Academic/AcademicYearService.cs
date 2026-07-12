using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class AcademicYearService : IAcademicYearService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ICalendarGenerationService _calendarGen;
    private readonly IAcademicYearRepository _repo;
    private readonly IMemoryCache _cache;
    private const string AllYearsCacheKey = "AcademicYear_All";

    public AcademicYearService(IUnitOfWork unitOfWork, ICalendarGenerationService calendarGen, IAcademicYearRepository repo, IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _calendarGen = calendarGen;
        _repo = repo;
        _cache = cache;
    }

    public async Task<PagedResult<AcademicYearListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken cancellationToken = default)
    {
        var spResults = await _repo.GetListSpAsync(page, pageSize, search);
        if (spResults.Count == 0)
            return new PagedResult<AcademicYearListItemDto> { Items = [], Page = page, PageSize = pageSize, TotalItems = 0 };

        var totalCount = spResults[0].TotalRecords;
        var items = spResults.Select(x => new AcademicYearListItemDto
        {
            Id = x.Id,
            Name = x.Name,
            Code = x.Code,
            StartsOn = x.StartsOn.ToString("yyyy-MM-dd"),
            EndsOn = x.EndsOn.ToString("yyyy-MM-dd"),
            IsActive = x.IsActive,
            IsCurrent = x.IsCurrent,
            IsLocked = x.IsLocked,
            Status = x.Status,
            TotalRecords = totalCount
        }).ToList();

        return new PagedResult<AcademicYearListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<AcademicYear?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _repo.GetByIdAsync(id, ct);
    }

    public async Task<AcademicYear?> GetActiveYearAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<AcademicYear>().Query().AsNoTracking()
            .FirstOrDefaultAsync(x => x.IsActive && !x.IsDeleted, ct);
    }

    public async Task<IReadOnlyList<AcademicYear>> GetAllYearsAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(AllYearsCacheKey, out IReadOnlyList<AcademicYear>? cached))
            return cached!;

        var result = await _unitOfWork.Repository<AcademicYear>().Query().AsNoTracking()
            .Where(x => !x.IsDeleted)
            .ToListAsync(ct);

        _cache.Set(AllYearsCacheKey, result, TimeSpan.FromMinutes(30));
        return result;
    }

    public async Task<AcademicYearUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<AcademicYear>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
        if (entity is null) return null;
        return new AcademicYearUpsertDto { Id = entity.Id, Name = entity.Name, Code = entity.Code, StartsOn = entity.StartsOn, EndsOn = entity.EndsOn, IsActive = entity.IsActive, IsCurrent = entity.IsCurrent, IsLocked = entity.IsLocked, Status = entity.Status };
    }

    public async Task<int> CreateAsync(AcademicYearUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var entity = new AcademicYear { Name = dto.Name.Trim(), Code = dto.Code?.Trim() ?? "", StartsOn = dto.StartsOn, EndsOn = dto.EndsOn, IsActive = dto.IsActive, IsCurrent = dto.IsCurrent, IsLocked = dto.IsLocked, Status = dto.Status ?? "Active", CreatedBy = createdBy };
        await _unitOfWork.Repository<AcademicYear>().AddAsync(entity, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try
        {
            await _calendarGen.GenerateYearAsync(entity.Id, entity.StartsOn.Year, cancellationToken);
        }
        catch
        {
            // Calendar generation is best-effort; don't block year creation
        }

        _cache.Remove(AllYearsCacheKey);
        return entity.Id;
    }

    public async Task UpdateAsync(AcademicYearUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<AcademicYear>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Academic Year not found.");
        entity.Name = dto.Name.Trim(); entity.Code = dto.Code?.Trim() ?? ""; entity.StartsOn = dto.StartsOn; entity.EndsOn = dto.EndsOn; entity.IsActive = dto.IsActive; entity.IsCurrent = dto.IsCurrent; entity.IsLocked = dto.IsLocked; entity.Status = dto.Status ?? "Active";
        entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _cache.Remove(AllYearsCacheKey);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var entity = await _unitOfWork.Repository<AcademicYear>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Academic Year not found.");
        entity.IsDeleted = true; entity.UpdatedBy = updatedBy; entity.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
        _cache.Remove(AllYearsCacheKey);
    }
}

