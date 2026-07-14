using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Interfaces.Accounting;

public interface IBankTransactionRepository : IBaseRepository<BankTransaction>
{
    Task<(List<BankBookEntryDto> Items, int TotalRecords)> GetBankBookAsync(int? accountId, int? bankType, DateTime? from, DateTime? to, int? periodId, int page, int pageSize, CancellationToken ct);
    Task<BankBookSummaryDto> GetBankBookSummaryAsync(int? accountId, int? bankType, DateTime? from, DateTime? to, int? periodId, CancellationToken ct);
    Task<List<BankReconciliationDto>> GetUnreconciledAsync(int? accountId, CancellationToken ct);
    Task ReconcileTransactionsAsync(string transactionIds, string reconciledBy, CancellationToken ct = default);
}
