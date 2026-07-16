using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IFailoverService
{
    Task<List<SchoolPayFailoverStatusDto>> GetFailoverStatusAsync(CancellationToken ct = default);
    Task<int?> ResolveWithFailoverAsync(decimal amount, string? feeType = null, CancellationToken ct = default);
    Task<bool> IsProviderAvailableAsync(int providerId, CancellationToken ct = default);
}
