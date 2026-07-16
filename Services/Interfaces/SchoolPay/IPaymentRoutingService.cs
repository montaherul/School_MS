using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IPaymentRoutingService
{
    Task<List<SchoolPayRouteRuleDto>> GetAllRulesAsync(CancellationToken ct = default);
    Task<SchoolPayRouteRuleDto?> GetRuleByIdAsync(int id, CancellationToken ct = default);
    Task<int> CreateRuleAsync(SchoolPayRouteRuleUpsertDto dto, string createdBy, CancellationToken ct = default);
    Task UpdateRuleAsync(int id, SchoolPayRouteRuleUpsertDto dto, string updatedBy, CancellationToken ct = default);
    Task DeleteRuleAsync(int id, CancellationToken ct = default);
    Task<int?> ResolveProviderAsync(decimal amount, string? feeType = null, CancellationToken ct = default);
    Task ToggleRuleActiveAsync(int id, bool isActive, CancellationToken ct = default);
}
