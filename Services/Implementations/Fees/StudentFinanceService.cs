using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class StudentFinanceService : IStudentFinanceService
{
    private readonly IStudentFinanceRepository _repository;

    public StudentFinanceService(IStudentFinanceRepository repository)
    {
        _repository = repository;
    }

    public async Task<PagedResult<StudentInvoiceDto>> GetInvoicesPagedAsync(
        int studentId, int page, int pageSize, string? search, int? status, CancellationToken ct)
    {
        var (items, total) = await _repository.GetInvoicesPagedAsync(studentId, page, pageSize, search, status, ct);
        return new PagedResult<StudentInvoiceDto>
        {
            Items = items.AsReadOnly(),
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<PagedResult<StudentPaymentDto>> GetPaymentsPagedAsync(
        int studentId, int page, int pageSize, string? search, CancellationToken ct)
    {
        var (items, total) = await _repository.GetPaymentsPagedAsync(studentId, page, pageSize, search, ct);
        return new PagedResult<StudentPaymentDto>
        {
            Items = items.AsReadOnly(),
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<PagedResult<StudentLedgerEntryDto>> GetLedgerPagedAsync(
        int studentId, int page, int pageSize, string? search, CancellationToken ct)
    {
        var (items, total) = await _repository.GetLedgerPagedAsync(studentId, page, pageSize, search, ct);
        return new PagedResult<StudentLedgerEntryDto>
        {
            Items = items.AsReadOnly(),
            Page = page,
            PageSize = pageSize,
            TotalItems = total
        };
    }

    public async Task<(decimal TotalInvoiced, decimal TotalPaid, decimal TotalDue)> GetFinanceSummaryAsync(int studentId, CancellationToken ct)
    {
        var (invoiced, paid) = await _repository.GetFinanceSummaryAsync(studentId, ct);
        return (invoiced, paid, invoiced - paid);
    }

    public async Task<StudentPaymentDto?> GetLastPaymentAsync(int studentId, CancellationToken ct)
    {
        return await _repository.GetLastPaymentAsync(studentId, ct);
    }
}