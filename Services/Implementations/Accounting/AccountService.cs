using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Accounting;

public class ChartOfAccountService : IChartOfAccountService
{
    private readonly IUnitOfWork _uow;
    private readonly IChartOfAccountRepository _repo;

    public ChartOfAccountService(IUnitOfWork uow, IChartOfAccountRepository repo)
    {
        _uow = uow;
        _repo = repo;
    }

    public async Task<PagedResult<AccountListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? accountType, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, total) = await _repo.GetPagedAsync(page, pageSize, search, accountType, ct);
        return new PagedResult<AccountListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<AccountUpsertDto?> GetForEditAsync(int id, CancellationToken ct)
    {
        var entity = await _uow.Repository<ChartOfAccount>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity is null) return null;
        return new AccountUpsertDto
        {
            Id = entity.Id,
            AccountCode = entity.AccountCode,
            AccountName = entity.AccountName,
            Description = entity.Description,
            AccountType = entity.AccountType,
            ParentAccountId = entity.ParentAccountId,
            IsActive = entity.IsActive,
            OpeningBalance = entity.OpeningBalance,
            DisplayOrder = entity.DisplayOrder
        };
    }

    public async Task<int> CreateAsync(AccountUpsertDto dto, string createdBy, CancellationToken ct)
    {
        var entity = new ChartOfAccount
        {
            CreatedBy = createdBy,
            AccountCode = dto.AccountCode,
            AccountName = dto.AccountName,
            Description = dto.Description,
            AccountType = dto.AccountType,
            ParentAccountId = dto.ParentAccountId,
            IsActive = dto.IsActive,
            OpeningBalance = dto.OpeningBalance,
            DisplayOrder = dto.DisplayOrder
        };
        await _uow.Repository<ChartOfAccount>().AddAsync(entity, ct);
        await _uow.SaveChangesAsync(ct);
        return entity.Id;
    }

    public async Task UpdateAsync(AccountUpsertDto dto, string updatedBy, CancellationToken ct)
    {
        var entity = await _uow.Repository<ChartOfAccount>().FirstOrDefaultAsync(x => x.Id == dto.Id && !x.IsDeleted, ct);
        if (entity is null) return;
        entity.AccountCode = dto.AccountCode;
        entity.AccountName = dto.AccountName;
        entity.Description = dto.Description;
        entity.AccountType = dto.AccountType;
        entity.ParentAccountId = dto.ParentAccountId;
        entity.IsActive = dto.IsActive;
        entity.OpeningBalance = dto.OpeningBalance;
        entity.DisplayOrder = dto.DisplayOrder;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;
        _uow.Repository<ChartOfAccount>().Update(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct)
    {
        var entity = await _uow.Repository<ChartOfAccount>().FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
        if (entity is null) return;
        entity.IsDeleted = true;
        entity.UpdatedAt = DateTime.UtcNow;
        entity.UpdatedBy = updatedBy;
        _uow.Repository<ChartOfAccount>().Update(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public Task<List<AccountTreeDto>> GetTreeAsync(CancellationToken ct) => _repo.GetTreeAsync(ct);
    public Task<string> GenerateAccountCodeAsync(int accountType, CancellationToken ct) => _repo.GenerateAccountCodeAsync(accountType, ct);

    public async Task<List<SelectListItem>> GetAccountSelectListAsync(CancellationToken ct)
    {
        var accounts = await _uow.Repository<ChartOfAccount>()
            .ListAsync(a => !a.IsDeleted, ct);
        return accounts
            .OrderBy(a => a.AccountCode)
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.AccountCode} - {a.AccountName}"
            })
            .ToList();
    }

    public async Task<List<SelectListItem>> GetActiveAccountSelectListAsync(CancellationToken ct)
    {
        var accounts = await _uow.Repository<ChartOfAccount>()
            .ListAsync(a => a.IsActive && !a.IsDeleted, ct);
        return accounts
            .OrderBy(a => a.AccountCode)
            .Select(a => new SelectListItem
            {
                Value = a.Id.ToString(),
                Text = $"{a.AccountCode} - {a.AccountName}"
            })
            .ToList();
    }
}
