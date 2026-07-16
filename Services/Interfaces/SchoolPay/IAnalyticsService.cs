using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IAnalyticsService
{
    Task<SchoolPayAnalyticsDto> GetAnalyticsAsync(int days = 30, CancellationToken ct = default);
    Task<SchoolPayAnalyticsDto> GetFilteredAnalyticsAsync(SchoolPayAnalyticsFilterDto filter, CancellationToken ct = default);
}
