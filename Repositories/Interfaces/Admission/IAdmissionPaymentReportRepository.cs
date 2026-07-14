using SchoolManagementSystem.Models.DTOs.Admission;

namespace SchoolManagementSystem.Repositories.Interfaces.Admission;

public interface IAdmissionPaymentReportRepository
{
    Task<(List<AdmissionDailyCollectionDto> Items, int TotalRecords)> GetDailyCollectionAsync(DateTime date, int page, int pageSize, CancellationToken ct);
    Task<(List<AdmissionMonthlyCollectionDto> Items, AdmissionMonthlySummaryDto Summary)> GetMonthlyCollectionAsync(int year, int month, CancellationToken ct);
    Task<AdmissionRevenueReportDto> GetRevenueReportAsync(DateTime from, DateTime to, CancellationToken ct);
    Task<(List<AdmissionPaymentRegisterDto> Items, int TotalRecords)> GetPaymentRegisterAsync(DateTime? from, DateTime? to, string? paymentMethod, int page, int pageSize, CancellationToken ct);
    Task<(List<AdmissionRefundReportDto> Items, int TotalRecords)> GetRefundReportAsync(DateTime? from, DateTime? to, int page, int pageSize, CancellationToken ct);
    Task<AdmissionPaymentDashboardDto> GetDashboardAsync(CancellationToken ct);
}
