using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IFeeReportRepository
{
    Task<(List<StudentLedgerReportDto> items, int total)> GetStudentLedgerReportAsync(int studentId, int page, int pageSize);
    Task<(List<DailyCollectionReportDto> items, int total)> GetDailyCollectionReportAsync(DateOnly date, int page, int pageSize);
    Task<(List<MonthlyCollectionReportDto> items, int total)> GetMonthlyCollectionReportAsync(int year, int page, int pageSize);
    Task<(List<DueReportDto> items, int total)> GetDueReportAsync(int page, int pageSize, int classId = 0);
    Task<(List<DiscountReportDto> items, int total)> GetDiscountReportAsync(int page, int pageSize);
    Task<(List<WaiverReportDto> items, int total)> GetWaiverReportAsync(int page, int pageSize);
    Task<(List<RefundReportDto> items, int total)> GetRefundReportAsync(int page, int pageSize);
    Task<(List<ClassCollectionSummaryDto> items, int total)> GetClassCollectionSummaryAsync(int academicYearId, int page, int pageSize);
}
