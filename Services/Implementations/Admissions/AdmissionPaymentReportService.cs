using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Services.Interfaces.Admissions;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class AdmissionPaymentReportService : IAdmissionPaymentReportService
{
    private readonly IAdmissionPaymentReportRepository _repo;
    private readonly ILogger<AdmissionPaymentReportService> _logger;

    public AdmissionPaymentReportService(IAdmissionPaymentReportRepository repo, ILogger<AdmissionPaymentReportService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<(List<AdmissionDailyCollectionDto> Items, int TotalRecords)> GetDailyCollectionAsync(DateTime date, int page, int pageSize, CancellationToken ct)
    {
        return await _repo.GetDailyCollectionAsync(date, page, pageSize, ct);
    }

    public async Task<(List<AdmissionMonthlyCollectionDto> Items, AdmissionMonthlySummaryDto Summary)> GetMonthlyCollectionAsync(int year, int month, CancellationToken ct)
    {
        return await _repo.GetMonthlyCollectionAsync(year, month, ct);
    }

    public async Task<AdmissionRevenueReportDto> GetRevenueReportAsync(DateTime from, DateTime to, CancellationToken ct)
    {
        return await _repo.GetRevenueReportAsync(from, to, ct);
    }

    public async Task<(List<AdmissionPaymentRegisterDto> Items, int TotalRecords)> GetPaymentRegisterAsync(DateTime? from, DateTime? to, string? paymentMethod, int page, int pageSize, CancellationToken ct)
    {
        return await _repo.GetPaymentRegisterAsync(from, to, paymentMethod, page, pageSize, ct);
    }

    public async Task<(List<AdmissionRefundReportDto> Items, int TotalRecords)> GetRefundReportAsync(DateTime? from, DateTime? to, int page, int pageSize, CancellationToken ct)
    {
        return await _repo.GetRefundReportAsync(from, to, page, pageSize, ct);
    }

    public async Task<AdmissionPaymentDashboardDto> GetDashboardAsync(CancellationToken ct)
    {
        return await _repo.GetDashboardAsync(ct);
    }
}
