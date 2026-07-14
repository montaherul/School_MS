using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Implementations.Accounting;

public class FinancialPeriodService : IFinancialPeriodService
{
    private readonly IUnitOfWork _uow;
    private readonly IFinancialPeriodRepository _repo;

    public FinancialPeriodService(IUnitOfWork uow, IFinancialPeriodRepository repo)
    {
        _uow = uow;
        _repo = repo;
    }

    public async Task<PagedResult<FinancialPeriodListItemDto>> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, total) = await _repo.GetPagedAsync(page, pageSize, search, ct);
        return new PagedResult<FinancialPeriodListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<FinancialPeriodUpsertDto?> GetForEditAsync(int id, CancellationToken ct)
    {
        var entity = await _uow.Repository<FinancialPeriod>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity is null) return null;
        return new FinancialPeriodUpsertDto
        {
            Id = entity.Id,
            Name = entity.Name,
            StartDate = entity.StartDate,
            EndDate = entity.EndDate,
            IsActive = entity.IsActive
        };
    }

    public async Task<int> CreateAsync(FinancialPeriodUpsertDto dto, string createdBy, CancellationToken ct)
    {
        var entity = new FinancialPeriod
        {
            CreatedBy = createdBy,
            Name = dto.Name,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = FinancialPeriodStatus.Open,
            IsActive = dto.IsActive
        };
        await _uow.Repository<FinancialPeriod>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(FinancialPeriodUpsertDto dto, string updatedBy, CancellationToken ct)
    {
        var entity = await _uow.Repository<FinancialPeriod>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct);
        if (entity is null) return;
        entity.Name = dto.Name;
        entity.StartDate = dto.StartDate;
        entity.EndDate = dto.EndDate;
        entity.IsActive = dto.IsActive;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;
        _uow.Repository<FinancialPeriod>().Update(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct)
    {
        var entity = await _uow.Repository<FinancialPeriod>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity is null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;
        _uow.Repository<FinancialPeriod>().Update(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task ClosePeriodAsync(int id, string closedBy, CancellationToken ct)
    {
        await _repo.CloseFinancialPeriodAsync(id, closedBy, ct);
    }

    public async Task<List<SelectListItem>> GetPeriodSelectListAsync(bool activeOnly, CancellationToken ct)
    {
        var periods = await _uow.Repository<FinancialPeriod>()
            .ListAsync(p => !p.IsDeleted && (!activeOnly || p.IsActive), ct);
        return periods
            .OrderByDescending(p => p.StartDate)
            .Select(p => new SelectListItem
            {
                Value = p.Id.ToString(),
                Text = $"{p.Name} ({p.StartDate:yyyy-MM-dd} - {p.EndDate:yyyy-MM-dd})"
            })
            .ToList();
    }
}
