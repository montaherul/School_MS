using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IHealthMonitorService
{
    Task<List<SchoolPayHealthStatusDto>> GetLatestHealthStatusAsync(CancellationToken ct = default);
    Task<List<SchoolPayHealthHistoryDto>> GetHealthHistoryAsync(int providerId, int days = 30, CancellationToken ct = default);
    Task<bool> CheckProviderHealthAsync(int providerId, CancellationToken ct = default);
    Task RunAllHealthChecksAsync(CancellationToken ct = default);
}
