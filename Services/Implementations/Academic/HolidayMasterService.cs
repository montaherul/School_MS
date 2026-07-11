using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Text;

namespace SchoolManagementSystem.Services.Implementations.Academic;

public class HolidayMasterService : IHolidayMasterService
{
    private readonly IUnitOfWork _uow;

    public HolidayMasterService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<PagedResult<HolidayMasterDto>> GetPagedAsync(int page, int pageSize, string? search, string? type, string? religion, CancellationToken ct = default)
    {
        var query = _uow.Repository<HolidayMaster>().Query().AsNoTracking().Where(x => !x.IsDeleted);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(x => x.Name.Contains(search) || (x.NameBn != null && x.NameBn.Contains(search)));

        if (!string.IsNullOrEmpty(type))
            query = query.Where(x => x.HolidayType == type);

        if (!string.IsNullOrEmpty(religion))
            query = query.Where(x => x.Religion == religion);

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(x => x.HolidayDate).ThenBy(x => x.DisplayOrder)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(x => new HolidayMasterDto
            {
                Id = x.Id,
                Name = x.Name,
                NameBn = x.NameBn,
                HolidayType = x.HolidayType,
                HolidayDate = x.HolidayDate,
                IsRecurring = x.IsRecurring,
                Religion = x.Religion,
                CountryCode = x.CountryCode,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            }).ToListAsync(ct);

        return new PagedResult<HolidayMasterDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<HolidayMasterDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<HolidayMaster>().QueryNoTracking().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity == null) return null;

        return new HolidayMasterDto
        {
            Id = entity.Id,
            Name = entity.Name,
            NameBn = entity.NameBn,
            HolidayType = entity.HolidayType,
            HolidayDate = entity.HolidayDate,
            IsRecurring = entity.IsRecurring,
            Religion = entity.Religion,
            CountryCode = entity.CountryCode,
            Description = entity.Description,
            DisplayOrder = entity.DisplayOrder,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(HolidayMasterUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        var entity = new HolidayMaster
        {
            Name = dto.Name.Trim(),
            NameBn = dto.NameBn?.Trim(),
            HolidayType = dto.HolidayType,
            HolidayDate = dto.HolidayDate,
            IsRecurring = dto.IsRecurring,
            Religion = dto.Religion,
            CountryCode = dto.CountryCode ?? "BD",
            Description = dto.Description?.Trim(),
            DisplayOrder = dto.DisplayOrder,
            IsActive = dto.IsActive,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };

        await _uow.Repository<HolidayMaster>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(HolidayMasterUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<HolidayMaster>().Query().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Holiday not found.");

        entity.Name = dto.Name.Trim();
        entity.NameBn = dto.NameBn?.Trim();
        entity.HolidayType = dto.HolidayType;
        entity.HolidayDate = dto.HolidayDate;
        entity.IsRecurring = dto.IsRecurring;
        entity.Religion = dto.Religion;
        entity.CountryCode = dto.CountryCode ?? "BD";
        entity.Description = dto.Description?.Trim();
        entity.DisplayOrder = dto.DisplayOrder;
        entity.IsActive = dto.IsActive;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;

        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<HolidayMaster>().Query().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Holiday not found.");

        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task BulkActivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        if (ids == null || ids.Count == 0) return;
        await _uow.Repository<HolidayMaster>().Query()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsActive, true)
                .SetProperty(x => x.UpdatedBy, updatedBy)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow), ct);
    }

    public async Task BulkDeactivateAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        if (ids == null || ids.Count == 0) return;
        await _uow.Repository<HolidayMaster>().Query()
            .Where(x => ids.Contains(x.Id) && !x.IsDeleted)
            .ExecuteUpdateAsync(setters => setters
                .SetProperty(x => x.IsActive, false)
                .SetProperty(x => x.UpdatedBy, updatedBy)
                .SetProperty(x => x.UpdatedAt, DateTime.UtcNow), ct);
    }

    public async Task ActivateAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<HolidayMaster>().Query().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Holiday not found.");
        entity.IsActive = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeactivateAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<HolidayMaster>().Query().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Holiday not found.");
        entity.IsActive = false;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<int> ImportAsync(List<HolidayMasterUpsertDto> holidays, string createdBy, CancellationToken ct = default)
    {
        var existingItems = await _uow.Repository<HolidayMaster>().QueryNoTracking()
            .Where(x => !x.IsDeleted)
            .Select(x => new { x.Name, x.HolidayDate })
            .ToListAsync(ct);
        var existingSet = new HashSet<(string Name, DateOnly HolidayDate)>(
            existingItems.Select(x => (x.Name, x.HolidayDate)));

        var toAdd = new List<HolidayMaster>();
        foreach (var dto in holidays)
        {
            if (existingSet.Contains((dto.Name.Trim(), dto.HolidayDate))) continue;

            toAdd.Add(new HolidayMaster
            {
                Name = dto.Name.Trim(),
                NameBn = dto.NameBn?.Trim(),
                HolidayType = dto.HolidayType,
                HolidayDate = dto.HolidayDate,
                IsRecurring = dto.IsRecurring,
                Religion = dto.Religion,
                CountryCode = dto.CountryCode ?? "BD",
                Description = dto.Description?.Trim(),
                DisplayOrder = dto.DisplayOrder,
                IsActive = dto.IsActive,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            });
        }

        if (toAdd.Count > 0)
        {
            await _uow.Repository<HolidayMaster>().AddRangeAsync(toAdd, ct);
            await _uow.SaveChangesAsync(ct);
        }

        return toAdd.Count;
    }

    public async Task<byte[]> ExportAsync(CancellationToken ct = default)
    {
        var holidays = await _uow.Repository<HolidayMaster>().QueryNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.HolidayDate)
            .ToListAsync(ct);

        var sb = new StringBuilder();
        sb.AppendLine("Name,NameBn,HolidayType,HolidayDate,IsRecurring,Religion,CountryCode,Description,DisplayOrder,IsActive");

        foreach (var h in holidays)
        {
            sb.AppendLine($"\"{h.Name}\",\"{h.NameBn}\",\"{h.HolidayType}\",{h.HolidayDate:yyyy-MM-dd},{h.IsRecurring},\"{h.Religion}\",\"{h.CountryCode}\",\"{h.Description}\",{h.DisplayOrder},{h.IsActive}");
        }

        return Encoding.UTF8.GetBytes(sb.ToString());
    }

    public async Task<int> GenerateBangladeshHolidaysAsync(int year, string createdBy, CancellationToken ct)
    {
        var holidays = HolidayProvider.GetAllAnnualHolidays(year);
        var dtoList = holidays.Select(h => new HolidayMasterUpsertDto
        {
            Name = h.Name,
            NameBn = h.NameBn,
            HolidayType = h.HolidayType,
            HolidayDate = h.HolidayDate,
            IsRecurring = h.IsRecurring,
            Religion = h.Religion,
            CountryCode = "BD",
            Description = h.Description,
            DisplayOrder = h.DisplayOrder,
            IsActive = true
        }).ToList();

        return await ImportAsync(dtoList, createdBy, ct);
    }

    public async Task<List<HolidayMasterDto>> GetAllAsync(CancellationToken ct = default)
    {
        return await _uow.Repository<HolidayMaster>().QueryNoTracking()
            .Where(x => !x.IsDeleted && x.IsActive)
            .OrderBy(x => x.HolidayDate).ThenBy(x => x.DisplayOrder)
            .Select(x => new HolidayMasterDto
            {
                Id = x.Id,
                Name = x.Name,
                NameBn = x.NameBn,
                HolidayType = x.HolidayType,
                HolidayDate = x.HolidayDate,
                IsRecurring = x.IsRecurring,
                Religion = x.Religion,
                CountryCode = x.CountryCode,
                Description = x.Description,
                DisplayOrder = x.DisplayOrder,
                IsActive = x.IsActive
            }).ToListAsync(ct);
    }
}
