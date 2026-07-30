using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Accounting;

public class FinanceConfigurationService : IFinanceConfigurationService
{
    private readonly IUnitOfWork _uow;

    public FinanceConfigurationService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<List<FinanceSettingDto>> GetAllSettingsAsync(string? category = null, CancellationToken ct = default)
    {
        var query = _uow.Repository<FinanceSetting>().ListAsync(s => !s.IsDeleted, ct);
        var settings = await query;

        if (!string.IsNullOrEmpty(category))
            settings = settings.Where(s => s.Category == category).ToList();

        return settings.Select(s => new FinanceSettingDto
        {
            Id = s.Id,
            Key = s.Key,
            Value = s.Value,
            Description = s.Description,
            Category = s.Category
        }).ToList();
    }

    public async Task<FinanceSettingDto?> GetSettingAsync(string key, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<FinanceSetting>()
            .FirstOrDefaultAsync(s => s.Key == key && !s.IsDeleted, ct);
        if (entity == null) return null;
        return new FinanceSettingDto
        {
            Id = entity.Id,
            Key = entity.Key,
            Value = entity.Value,
            Description = entity.Description,
            Category = entity.Category
        };
    }

    public async Task<string> GetSettingValueAsync(string key, string defaultValue = "", CancellationToken ct = default)
    {
        var setting = await GetSettingAsync(key, ct);
        return setting?.Value ?? defaultValue;
    }

    public async Task SetSettingAsync(string key, string value, string? description, string category, string createdBy, CancellationToken ct = default)
    {
        var existing = await _uow.Repository<FinanceSetting>()
            .FirstOrDefaultAsync(s => s.Key == key && !s.IsDeleted, ct);
        if (existing != null)
        {
            existing.Value = value;
            existing.Description = description ?? existing.Description;
            existing.Category = category;
            existing.UpdatedBy = createdBy;
            existing.UpdatedAt = DateTime.UtcNow;
            _uow.Repository<FinanceSetting>().Update(existing);
        }
        else
        {
            var setting = new FinanceSetting
            {
                Key = key,
                Value = value,
                Description = description,
                Category = category,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Repository<FinanceSetting>().AddAsync(setting, ct);
        }
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteSettingAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<FinanceSetting>()
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct)
            ?? throw new InvalidOperationException("Finance setting not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<FinanceSetting>().Update(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<List<AccountMappingDto>> GetAllMappingsAsync(CancellationToken ct = default)
    {
        var mappings = await _uow.Repository<AccountMapping>()
            .ListAsync(m => !m.IsDeleted, ct);

        var result = new List<AccountMappingDto>();
        foreach (var m in mappings)
        {
            var debitAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == m.DebitAccountCode && !a.IsDeleted, ct);
            var creditAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == m.CreditAccountCode && !a.IsDeleted, ct);

            result.Add(new AccountMappingDto
            {
                Id = m.Id,
                TransactionType = m.TransactionType,
                DebitAccountCode = m.DebitAccountCode,
                DebitAccountName = debitAccount?.AccountName ?? "Unknown",
                CreditAccountCode = m.CreditAccountCode,
                CreditAccountName = creditAccount?.AccountName ?? "Unknown",
                Description = m.Description,
                IsActive = m.IsActive
            });
        }
        return result;
    }

    public async Task<AccountMappingDto?> GetMappingByTransactionTypeAsync(string transactionType, CancellationToken ct = default)
    {
        var m = await _uow.Repository<AccountMapping>()
            .FirstOrDefaultAsync(x => x.TransactionType == transactionType && x.IsActive && !x.IsDeleted, ct);
        if (m == null) return null;

        var debitAccount = await _uow.Repository<ChartOfAccount>()
            .FirstOrDefaultAsync(a => a.AccountCode == m.DebitAccountCode && !a.IsDeleted, ct);
        var creditAccount = await _uow.Repository<ChartOfAccount>()
            .FirstOrDefaultAsync(a => a.AccountCode == m.CreditAccountCode && !a.IsDeleted, ct);

        return new AccountMappingDto
        {
            Id = m.Id,
            TransactionType = m.TransactionType,
            DebitAccountCode = m.DebitAccountCode,
            DebitAccountName = debitAccount?.AccountName ?? "Unknown",
            CreditAccountCode = m.CreditAccountCode,
            CreditAccountName = creditAccount?.AccountName ?? "Unknown",
            Description = m.Description,
            IsActive = m.IsActive
        };
    }

    public async Task SaveMappingAsync(AccountMappingUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        if (dto.Id > 0)
        {
            var entity = await _uow.Repository<AccountMapping>()
                .FirstOrDefaultAsync(m => m.Id == dto.Id && !m.IsDeleted, ct)
                ?? throw new InvalidOperationException("Account mapping not found.");
            entity.TransactionType = dto.TransactionType;
            entity.DebitAccountCode = dto.DebitAccountCode;
            entity.CreditAccountCode = dto.CreditAccountCode;
            entity.Description = dto.Description;
            entity.IsActive = dto.IsActive;
            entity.UpdatedBy = createdBy;
            entity.UpdatedAt = DateTime.UtcNow;
            _uow.Repository<AccountMapping>().Update(entity);
        }
        else
        {
            var entity = new AccountMapping
            {
                TransactionType = dto.TransactionType,
                DebitAccountCode = dto.DebitAccountCode,
                CreditAccountCode = dto.CreditAccountCode,
                Description = dto.Description,
                IsActive = dto.IsActive,
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Repository<AccountMapping>().AddAsync(entity, ct);
        }
        await _uow.SaveChangesAsync(ct);
    }

    public async Task DeleteMappingAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var entity = await _uow.Repository<AccountMapping>()
            .FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, ct)
            ?? throw new InvalidOperationException("Account mapping not found.");
        entity.IsDeleted = true;
        entity.UpdatedBy = updatedBy;
        entity.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<AccountMapping>().Update(entity);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<FiscalSettingDto> GetFiscalSettingsAsync(CancellationToken ct = default)
    {
        return new FiscalSettingDto
        {
            FiscalYearStart = await GetSettingValueAsync("FiscalYearStart", "01-01", ct),
            FiscalYearEnd = await GetSettingValueAsync("FiscalYearEnd", "12-31", ct),
            AutoCreatePeriods = await GetSettingValueAsync("AutoCreatePeriods", "true", ct) == "true",
            GracePeriodDays = int.Parse(await GetSettingValueAsync("GracePeriodDays", "30", ct)),
            WriteOffThreshold = decimal.Parse(await GetSettingValueAsync("WriteOffThreshold", "1.00", ct)),
            DefaultDueDay = int.Parse(await GetSettingValueAsync("DefaultDueDay", "10", ct)),
            MinPaymentPercentage = decimal.Parse(await GetSettingValueAsync("MinPaymentPercentage", "0", ct)),
            EnforcePeriodClosing = await GetSettingValueAsync("EnforcePeriodClosing", "true", ct) == "true"
        };
    }
}
