using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface IMonitoringService
{
    Task<SchoolPayMonitoringDto> GetMonitoringDataAsync(CancellationToken ct = default);
}
