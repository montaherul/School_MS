using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IProviderManagementService
{
    Task<List<SchoolPayProviderListDto>> GetAllProvidersAsync(CancellationToken ct = default);
    Task<SchoolPayProviderDto?> GetProviderByIdAsync(int id, CancellationToken ct = default);
    Task<SchoolPayProviderDto?> GetProviderByCodeAsync(string code, CancellationToken ct = default);
    Task<int> CreateProviderAsync(SchoolPayProviderUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task<bool> UpdateProviderAsync(int id, SchoolPayProviderUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task<bool> ToggleProviderStatusAsync(int id, bool isActive, string updatedBy, CancellationToken ct = default);
    Task<bool> DeleteProviderAsync(int id, CancellationToken ct = default);
    Task<bool> UpdateProviderPriorityAsync(int id, int priority, CancellationToken ct = default);
    Task<bool> ToggleSandboxModeAsync(int id, bool isSandbox, CancellationToken ct = default);
    Task<List<SchoolPayProviderDto>> GetActiveProvidersForCheckoutAsync(decimal amount, string? feeType = null, CancellationToken ct = default);
}
