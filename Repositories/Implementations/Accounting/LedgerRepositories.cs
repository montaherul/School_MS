using System.Data;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;

namespace SchoolManagementSystem.Repositories.Implementations.Accounting;

public class LedgerRepository : BaseRepository<object>, ILedgerRepository
{
    public LedgerRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<GeneralLedgerEntryDto> Items, int TotalRecords)> GetGeneralLedgerAsync(int? accountId, DateTime? from, DateTime? to, int? periodId, int page, int pageSize, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetGeneralLedger";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@AccountId", accountId);
        AddParameter(cmd, "@FromDate", from);
        AddParameter(cmd, "@ToDate", to);
        AddParameter(cmd, "@FinancialPeriodId", periodId);
        AddParameter(cmd, "@PageNumber", page);
        AddParameter(cmd, "@PageSize", pageSize);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<GeneralLedgerEntryDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new GeneralLedgerEntryDto
            {
                Id = GetInt32(reader, "Id"),
                AccountId = GetInt32(reader, "AccountId"),
                AccountCode = GetString(reader, "AccountCode"),
                AccountName = GetString(reader, "AccountName"),
                EntryDate = GetDateTime(reader, "EntryDate"),
                JournalNo = GetNullableString(reader, "JournalNo"),
                Description = GetNullableString(reader, "Description"),
                DebitAmount = GetDecimal(reader, "DebitAmount"),
                CreditAmount = GetDecimal(reader, "CreditAmount"),
                RunningBalance = GetDecimal(reader, "RunningBalance"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }

    public async Task<List<TrialBalanceDto>> GetTrialBalanceAsync(DateTime? asOfDate, int? periodId, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetTrialBalance";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@AsOfDate", asOfDate);
        AddParameter(cmd, "@FinancialPeriodId", periodId);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<TrialBalanceDto>();
        while (await reader.ReadAsync(ct))
        {
            var closingBal = GetDecimal(reader, "ClosingBalance");
            items.Add(new TrialBalanceDto
            {
                AccountId = GetInt32(reader, "AccountId"),
                AccountCode = GetString(reader, "AccountCode"),
                AccountName = GetString(reader, "AccountName"),
                AccountType = GetString(reader, "AccountType"),
                OpeningDebit = GetDecimal(reader, "OpeningDebit"),
                OpeningCredit = GetDecimal(reader, "OpeningCredit"),
                Debit = GetDecimal(reader, "TotalDebit"),
                Credit = GetDecimal(reader, "TotalCredit"),
                ClosingDebit = closingBal > 0 ? closingBal : 0,
                ClosingCredit = closingBal < 0 ? Math.Abs(closingBal) : 0
            });
        }
        return items;
    }

    public async Task<(List<FinancialStatementLine> Incomes, List<FinancialStatementLine> Expenses)> GetIncomeStatementAsync(DateTime from, DateTime to, int? periodId, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetIncomeStatement";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@FromDate", from);
        AddParameter(cmd, "@ToDate", to);
        AddParameter(cmd, "@FinancialPeriodId", periodId);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);

        var incomes = new List<FinancialStatementLine>();
        while (await reader.ReadAsync(ct))
        {
            incomes.Add(new FinancialStatementLine
            {
                AccountId = GetInt32(reader, "AccountId"),
                AccountCode = GetString(reader, "AccountCode"),
                AccountName = GetString(reader, "AccountName"),
                Amount = GetDecimal(reader, "Amount")
            });
        }

        // Move to next result set
        await reader.NextResultAsync(ct);

        var expenses = new List<FinancialStatementLine>();
        while (await reader.ReadAsync(ct))
        {
            expenses.Add(new FinancialStatementLine
            {
                AccountId = GetInt32(reader, "AccountId"),
                AccountCode = GetString(reader, "AccountCode"),
                AccountName = GetString(reader, "AccountName"),
                Amount = GetDecimal(reader, "Amount")
            });
        }

        return (incomes, expenses);
    }

    public async Task<(List<FinancialStatementLine> Assets, List<FinancialStatementLine> Liabilities, List<FinancialStatementLine> Equity, decimal NetIncome)> GetBalanceSheetAsync(DateTime? asOfDate, int? periodId, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetBalanceSheet";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@AsOfDate", asOfDate ?? DateTime.UtcNow);
        AddParameter(cmd, "@FinancialPeriodId", periodId);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);

        var assets = new List<FinancialStatementLine>();
        while (await reader.ReadAsync(ct))
        {
            assets.Add(new FinancialStatementLine
            {
                AccountId = GetInt32(reader, "AccountId"),
                AccountCode = GetString(reader, "AccountCode"),
                AccountName = GetString(reader, "AccountName"),
                Amount = GetDecimal(reader, "Amount")
            });
        }

        await reader.NextResultAsync(ct);

        var liabilities = new List<FinancialStatementLine>();
        while (await reader.ReadAsync(ct))
        {
            liabilities.Add(new FinancialStatementLine
            {
                AccountId = GetInt32(reader, "AccountId"),
                AccountCode = GetString(reader, "AccountCode"),
                AccountName = GetString(reader, "AccountName"),
                Amount = GetDecimal(reader, "Amount")
            });
        }

        await reader.NextResultAsync(ct);

        var equity = new List<FinancialStatementLine>();
        while (await reader.ReadAsync(ct))
        {
            equity.Add(new FinancialStatementLine
            {
                AccountId = GetInt32(reader, "AccountId"),
                AccountCode = GetString(reader, "AccountCode"),
                AccountName = GetString(reader, "AccountName"),
                Amount = GetDecimal(reader, "Amount")
            });
        }

        await reader.NextResultAsync(ct);

        var netIncome = 0m;
        if (await reader.ReadAsync(ct))
        {
            netIncome = GetDecimal(reader, "Amount");
        }

        return (assets, liabilities, equity, netIncome);
    }

    public async Task<List<MonthlyIncomeSummaryDto>> GetMonthlyIncomeSummaryAsync(int year, int? periodId, CancellationToken ct)
    {
        var query = from gl in _db.GeneralLedgerEntries
                    join a in _db.ChartOfAccounts on gl.AccountId equals a.Id
                    where !gl.IsDeleted && !a.IsDeleted && a.IsActive
                        && gl.EntryDate.Year == year
                        && (periodId == null || gl.FinancialPeriodId == periodId)
                    group new { gl, a } by new { gl.EntryDate.Year, gl.EntryDate.Month, a.AccountType } into g
                    select new
                    {
                        g.Key.Year,
                        g.Key.Month,
                        g.Key.AccountType,
                        Amount = g.Sum(x => x.a.AccountType == (Models.Enums.AccountType)3
                            ? x.gl.CreditAmount - x.gl.DebitAmount
                            : x.gl.DebitAmount - x.gl.CreditAmount)
                    };

        var raw = await query.ToListAsync(ct);

        return raw.GroupBy(x => new { x.Year, x.Month })
            .Select(g => new MonthlyIncomeSummaryDto
            {
                Month = new DateTime(g.Key.Year, g.Key.Month, 1).ToString("MMMM"),
                Year = g.Key.Year,
                TotalIncome = g.Where(x => (int)x.AccountType == 3).Sum(x => Math.Abs(x.Amount)),
                TotalExpense = g.Where(x => (int)x.AccountType == 4).Sum(x => Math.Abs(x.Amount))
            })
            .OrderBy(m => DateTime.ParseExact(m.Month, "MMMM", System.Globalization.CultureInfo.InvariantCulture).Month)
            .ToList();
    }
}
