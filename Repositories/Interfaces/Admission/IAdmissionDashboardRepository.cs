using SchoolManagementSystem.Models.DTOs.Admission;

namespace SchoolManagementSystem.Repositories.Interfaces.Admission;

public interface IAdmissionDashboardRepository
{
    Task<AdmissionDashboardDto> GetDashboardDataAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default);
    Task<AdmissionRegisterReportDto> GetRegisterReportAsync(AdmissionReportRequest request, CancellationToken ct = default);
    Task<List<TrendAnalysisDto>> GetTrendAnalysisAsync(DateTime? dateFrom = null, DateTime? dateTo = null, string? groupBy = "Month", CancellationToken ct = default);
    Task<ConversionFunnelDto> GetConversionFunnelAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default);
    Task<List<ClassDemandDto>> GetClassDemandAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default);
    Task<RevenueReportDto> GetRevenueReportAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default);
}
