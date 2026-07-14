using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Accounting;

public interface IBankService
{
    Task<PagedResult<BankBookEntryDto>> GetBankBookAsync(int? accountId, int? bankType, DateTime? from, DateTime? to, int? periodId, int page, int pageSize, CancellationToken ct = default);
    Task<BankBookSummaryDto> GetSummaryAsync(int? accountId, int? bankType, DateTime? from, DateTime? to, int? periodId, CancellationToken ct = default);
    Task<int> CreateTransactionAsync(BankTransactionDto dto, string createdBy, CancellationToken ct = default);
    Task ReconcileAsync(string transactionIds, string reconciledBy, CancellationToken ct = default);
    Task<List<BankReconciliationDto>> GetUnreconciledAsync(int? accountId, CancellationToken ct = default);
}
