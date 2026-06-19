using SchoolManagementSystem.Models.DTOs.Dashboard;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IStudentFinanceRepository
{
    Task<(List<StudentInvoiceDto> Items, int TotalRecords)> GetInvoicesPagedAsync(int studentId, int page, int pageSize, string? search, int? status, CancellationToken ct);
    Task<(List<StudentPaymentDto> Items, int TotalRecords)> GetPaymentsPagedAsync(int studentId, int page, int pageSize, string? search, CancellationToken ct);
    Task<(List<StudentLedgerEntryDto> Items, int TotalRecords)> GetLedgerPagedAsync(int studentId, int page, int pageSize, string? search, CancellationToken ct);
    Task<(decimal TotalInvoiced, decimal TotalPaid)> GetFinanceSummaryAsync(int studentId, CancellationToken ct);
    Task<StudentPaymentDto?> GetLastPaymentAsync(int studentId, CancellationToken ct);
}