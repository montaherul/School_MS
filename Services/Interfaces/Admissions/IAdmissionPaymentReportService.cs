using SchoolManagementSystem.Models.DTOs.Admission;

namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface IAdmissionPaymentReportService
{
    Task<(List<AdmissionDailyCollectionDto> Items, int TotalRecords)> GetDailyCollectionAsync(DateTime date, int page, int pageSize, CancellationToken ct = default);
    Task<(List<AdmissionMonthlyCollectionDto> Items, AdmissionMonthlySummaryDto Summary)> GetMonthlyCollectionAsync(int year, int month, CancellationToken ct = default);
    Task<AdmissionRevenueReportDto> GetRevenueReportAsync(DateTime from, DateTime to, CancellationToken ct = default);
    Task<(List<AdmissionPaymentRegisterDto> Items, int TotalRecords)> GetPaymentRegisterAsync(DateTime? from, DateTime? to, string? paymentMethod, int page, int pageSize, CancellationToken ct = default);
    Task<(List<AdmissionRefundReportDto> Items, int TotalRecords)> GetRefundReportAsync(DateTime? from, DateTime? to, int page, int pageSize, CancellationToken ct = default);
    Task<AdmissionPaymentDashboardDto> GetDashboardAsync(CancellationToken ct = default);
}
