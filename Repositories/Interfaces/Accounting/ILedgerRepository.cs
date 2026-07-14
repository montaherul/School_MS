using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Repositories.Interfaces;

namespace SchoolManagementSystem.Repositories.Interfaces.Accounting;

public interface ILedgerRepository
{
    Task<(List<GeneralLedgerEntryDto> Items, int TotalRecords)> GetGeneralLedgerAsync(int? accountId, DateTime? from, DateTime? to, int? periodId, int page, int pageSize, CancellationToken ct);
    Task<List<TrialBalanceDto>> GetTrialBalanceAsync(DateTime? asOfDate, int? periodId, CancellationToken ct);
    Task<(List<FinancialStatementLine> Incomes, List<FinancialStatementLine> Expenses)> GetIncomeStatementAsync(DateTime from, DateTime to, int? periodId, CancellationToken ct);
    Task<(List<FinancialStatementLine> Assets, List<FinancialStatementLine> Liabilities, List<FinancialStatementLine> Equity, decimal NetIncome)> GetBalanceSheetAsync(DateTime? asOfDate, int? periodId, CancellationToken ct);
    Task<List<MonthlyIncomeSummaryDto>> GetMonthlyIncomeSummaryAsync(int year, int? periodId, CancellationToken ct);
}
