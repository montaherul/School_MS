using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class MonitoringService : IMonitoringService
{
    private readonly ISchoolPayRepository _repo;
    private readonly ILogger<MonitoringService> _logger;

    public MonitoringService(ISchoolPayRepository repo, ILogger<MonitoringService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SchoolPayMonitoringDto> GetMonitoringDataAsync(CancellationToken ct = default)
        => await _repo.GetMonitoringDataAsync(ct);
}
