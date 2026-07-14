using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Accounting;

namespace SchoolManagementSystem.Services.Implementations.Accounting;

public class LedgerService : ILedgerService
{
    private readonly ILedgerRepository _repo;

    public LedgerService(ILedgerRepository repo) { _repo = repo; }

    public async Task<PagedResult<GeneralLedgerEntryDto>> GetGeneralLedgerAsync(int? accountId, DateTime? from, DateTime? to, int? periodId, int page, int pageSize, CancellationToken ct)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, total) = await _repo.GetGeneralLedgerAsync(accountId, from, to, periodId, page, pageSize, ct);
        return new PagedResult<GeneralLedgerEntryDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = total };
    }

    public async Task<TrialBalanceResultDto> GetTrialBalanceAsync(DateTime? asOfDate, int? periodId, CancellationToken ct)
    {
        var entries = await _repo.GetTrialBalanceAsync(asOfDate, periodId, ct);
        return new TrialBalanceResultDto { Entries = entries };
    }

    public async Task<IncomeStatementDto> GetIncomeStatementAsync(DateTime from, DateTime to, int? periodId, CancellationToken ct)
    {
        var (incomes, expenses) = await _repo.GetIncomeStatementAsync(from, to, periodId, ct);
        return new IncomeStatementDto
        {
            Incomes = incomes,
            Expenses = expenses,
            PeriodName = $"{from:dd MMM yyyy} - {to:dd MMM yyyy}"
        };
    }

    public async Task<BalanceSheetDto> GetBalanceSheetAsync(DateTime? asOfDate, int? periodId, CancellationToken ct)
    {
        var (assets, liabilities, equity, netIncome) = await _repo.GetBalanceSheetAsync(asOfDate, periodId, ct);

        if (netIncome != 0)
        {
            equity.Add(new FinancialStatementLine
            {
                AccountId = 0,
                AccountCode = "NET",
                AccountName = netIncome > 0 ? "Net Profit" : "Net Loss",
                Amount = Math.Abs(netIncome)
            });
        }

        return new BalanceSheetDto
        {
            Assets = assets,
            Liabilities = liabilities,
            Equity = equity,
            PeriodName = asOfDate?.ToString("dd MMM yyyy") ?? DateTime.UtcNow.ToString("dd MMM yyyy")
        };
    }

    public Task<List<MonthlyIncomeSummaryDto>> GetMonthlyIncomeSummaryAsync(int year, int? periodId, CancellationToken ct)
        => _repo.GetMonthlyIncomeSummaryAsync(year, periodId, ct);
}
