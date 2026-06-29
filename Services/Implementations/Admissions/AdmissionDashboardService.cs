using Microsoft.Extensions.Caching.Memory;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Services.Interfaces.Admissions;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class AdmissionDashboardService : IAdmissionDashboardService
{
    private readonly IAdmissionDashboardRepository _dashboardRepository;
    private readonly IMemoryCache _cache;
    private const string CacheKey = "AdmissionDashboard";

    public AdmissionDashboardService(
        IAdmissionDashboardRepository dashboardRepository,
        IMemoryCache cache)
    {
        _dashboardRepository = dashboardRepository;
        _cache = cache;
    }

    public async Task<AdmissionDashboardDto> GetDashboardAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        var cacheKey = $"{CacheKey}_{dateFrom:yyyyMMdd}_{dateTo:yyyyMMdd}";

        if (_cache.TryGetValue(cacheKey, out AdmissionDashboardDto? cached) && cached != null)
            return cached;

        var data = await _dashboardRepository.GetDashboardDataAsync(dateFrom, dateTo, ct);

        _cache.Set(cacheKey, data, TimeSpan.FromMinutes(5));

        return data;
    }

    public async Task<AdmissionRegisterReportDto> GetRegisterReportAsync(AdmissionReportRequest request, CancellationToken ct = default)
    {
        return await _dashboardRepository.GetRegisterReportAsync(request, ct);
    }

    public async Task<List<TrendAnalysisDto>> GetTrendAnalysisAsync(DateTime? dateFrom = null, DateTime? dateTo = null, string? groupBy = "Month", CancellationToken ct = default)
    {
        return await _dashboardRepository.GetTrendAnalysisAsync(dateFrom, dateTo, groupBy, ct);
    }

    public async Task<ConversionFunnelDto> GetConversionFunnelAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        return await _dashboardRepository.GetConversionFunnelAsync(dateFrom, dateTo, ct);
    }

    public async Task<List<ClassDemandDto>> GetClassDemandAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        return await _dashboardRepository.GetClassDemandAsync(dateFrom, dateTo, ct);
    }

    public async Task<RevenueReportDto> GetRevenueReportAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default)
    {
        return await _dashboardRepository.GetRevenueReportAsync(dateFrom, dateTo, ct);
    }
}
