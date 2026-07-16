using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class AnalyticsService : IAnalyticsService
{
    private readonly ISchoolPayRepository _repo;
    private readonly ILogger<AnalyticsService> _logger;

    public AnalyticsService(ISchoolPayRepository repo, ILogger<AnalyticsService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<SchoolPayAnalyticsDto> GetAnalyticsAsync(int days = 30, CancellationToken ct = default)
        => await _repo.GetAnalyticsAsync(days, ct);

    public async Task<SchoolPayAnalyticsDto> GetFilteredAnalyticsAsync(SchoolPayAnalyticsFilterDto filter, CancellationToken ct = default)
    {
        return await _repo.GetAnalyticsAsync(filter.Days, ct);
    }
}
