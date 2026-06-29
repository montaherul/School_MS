using SchoolManagementSystem.Models.DTOs.Admission;

namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface IAdmissionDashboardService
{
    Task<AdmissionDashboardDto> GetDashboardAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default);
    Task<AdmissionRegisterReportDto> GetRegisterReportAsync(AdmissionReportRequest request, CancellationToken ct = default);
    Task<List<TrendAnalysisDto>> GetTrendAnalysisAsync(DateTime? dateFrom = null, DateTime? dateTo = null, string? groupBy = "Month", CancellationToken ct = default);
    Task<ConversionFunnelDto> GetConversionFunnelAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default);
    Task<List<ClassDemandDto>> GetClassDemandAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default);
    Task<RevenueReportDto> GetRevenueReportAsync(DateTime? dateFrom = null, DateTime? dateTo = null, CancellationToken ct = default);
}
