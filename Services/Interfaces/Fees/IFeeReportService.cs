using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IFeeReportService
{
    Task<PagedResult<StudentLedgerReportDto>> GetStudentLedgerReportAsync(int studentId, int page, int pageSize);
    Task<PagedResult<DailyCollectionReportDto>> GetDailyCollectionReportAsync(DateOnly date, int page, int pageSize);
    Task<PagedResult<MonthlyCollectionReportDto>> GetMonthlyCollectionReportAsync(int year, int page, int pageSize);
    Task<PagedResult<DueReportDto>> GetDueReportAsync(int page, int pageSize, int classId = 0);
    Task<PagedResult<DiscountReportDto>> GetDiscountReportAsync(int page, int pageSize);
    Task<PagedResult<WaiverReportDto>> GetWaiverReportAsync(int page, int pageSize);
    Task<PagedResult<RefundReportDto>> GetRefundReportAsync(int page, int pageSize);
    Task<PagedResult<ClassCollectionSummaryDto>> GetClassCollectionSummaryAsync(int academicYearId, int page, int pageSize);
    Task<CashBookResultDto> GetCashBookAsync(DateOnly fromDate, DateOnly toDate, int? academicYearId = null);
    Task<List<AcademicYearOptionDto>> GetAcademicYearOptionsAsync();
    Task<byte[]> ExportToExcelAsync<T>(List<T> data, string reportName);
    Task<byte[]> ExportToPdfAsync<T>(List<T> data, string reportName, string htmlTemplate);
}
