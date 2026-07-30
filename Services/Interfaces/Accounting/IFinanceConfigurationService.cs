using SchoolManagementSystem.Models.DTOs.Accounting;

namespace SchoolManagementSystem.Services.Interfaces.Accounting;

public interface IFinanceConfigurationService
{
    Task<List<FinanceSettingDto>> GetAllSettingsAsync(string? category = null, CancellationToken ct = default);
    Task<FinanceSettingDto?> GetSettingAsync(string key, CancellationToken ct = default);
    Task<string> GetSettingValueAsync(string key, string defaultValue = "", CancellationToken ct = default);
    Task SetSettingAsync(string key, string value, string? description, string category, string createdBy, CancellationToken ct = default);
    Task DeleteSettingAsync(int id, string updatedBy, CancellationToken ct = default);
    Task<List<AccountMappingDto>> GetAllMappingsAsync(CancellationToken ct = default);
    Task<AccountMappingDto?> GetMappingByTransactionTypeAsync(string transactionType, CancellationToken ct = default);
    Task SaveMappingAsync(AccountMappingUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task DeleteMappingAsync(int id, string updatedBy, CancellationToken ct = default);
    Task<FiscalSettingDto> GetFiscalSettingsAsync(CancellationToken ct = default);
}
