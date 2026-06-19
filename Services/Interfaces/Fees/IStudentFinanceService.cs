using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Dashboard;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IStudentFinanceService
{
    Task<PagedResult<StudentInvoiceDto>> GetInvoicesPagedAsync(int studentId, int page, int pageSize, string? search, int? status, CancellationToken ct);
    Task<PagedResult<StudentPaymentDto>> GetPaymentsPagedAsync(int studentId, int page, int pageSize, string? search, CancellationToken ct);
    Task<PagedResult<StudentLedgerEntryDto>> GetLedgerPagedAsync(int studentId, int page, int pageSize, string? search, CancellationToken ct);
    Task<(decimal TotalInvoiced, decimal TotalPaid, decimal TotalDue)> GetFinanceSummaryAsync(int studentId, CancellationToken ct);
    Task<StudentPaymentDto?> GetLastPaymentAsync(int studentId, CancellationToken ct);
}