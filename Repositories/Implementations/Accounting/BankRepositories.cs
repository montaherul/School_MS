using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;

namespace SchoolManagementSystem.Repositories.Implementations.Accounting;

public class BankTransactionRepository : BaseRepository<BankTransaction>, IBankTransactionRepository
{
    public BankTransactionRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<BankBookEntryDto> Items, int TotalRecords)> GetBankBookAsync(int? accountId, int? bankType, DateTime? from, DateTime? to, int? periodId, int page, int pageSize, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetBankBook";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@AccountId", accountId);
        AddParameter(cmd, "@BankAccountType", bankType);
        AddParameter(cmd, "@FromDate", from);
        AddParameter(cmd, "@ToDate", to);
        AddParameter(cmd, "@FinancialPeriodId", periodId);
        AddParameter(cmd, "@PageNumber", page);
        AddParameter(cmd, "@PageSize", pageSize);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<BankBookEntryDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new BankBookEntryDto
            {
                Id = GetInt32(reader, "Id"),
                AccountId = GetInt32(reader, "AccountId"),
                AccountName = GetString(reader, "AccountName"),
                BankAccountType = GetString(reader, "BankAccountType"),
                TransactionDate = GetDateTime(reader, "TransactionDate"),
                TransactionType = GetString(reader, "TransactionType"),
                Amount = GetDecimal(reader, "Amount"),
                ReferenceNo = GetNullableString(reader, "ReferenceNo"),
                ChequeNo = GetNullableString(reader, "ChequeNo"),
                Description = GetNullableString(reader, "Description"),
                CounterParty = GetNullableString(reader, "CounterParty"),
                IsReconciled = GetBoolean(reader, "IsReconciled"),
                RunningBalance = GetDecimal(reader, "RunningBalance"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }

    public async Task<BankBookSummaryDto> GetBankBookSummaryAsync(int? accountId, int? bankType, DateTime? from, DateTime? to, int? periodId, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetBankBook";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@AccountId", accountId);
        AddParameter(cmd, "@BankAccountType", bankType);
        AddParameter(cmd, "@FromDate", from);
        AddParameter(cmd, "@ToDate", to);
        AddParameter(cmd, "@FinancialPeriodId", periodId);
        AddParameter(cmd, "@PageNumber", 1);
        AddParameter(cmd, "@PageSize", 1);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);

        // Read first result set to advance to second
        using var reader = await cmd.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct)) { }

        // Read summary from second result set
        await reader.NextResultAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            return new BankBookSummaryDto
            {
                OpeningBalance = GetDecimal(reader, "OpeningBalance"),
                TotalDeposits = GetDecimal(reader, "TotalDeposits"),
                TotalWithdrawals = GetDecimal(reader, "TotalWithdrawals"),
                ClosingBalance = GetDecimal(reader, "ClosingBalance"),
                UnclearedBalance = GetDecimal(reader, "UnclearedBalance")
            };
        }

        return new BankBookSummaryDto();
    }

    public async Task<List<BankReconciliationDto>> GetUnreconciledAsync(int? accountId, CancellationToken ct)
    {
        var query = _db.BankTransactions
            .Where(b => !b.IsDeleted && !b.IsReconciled)
            .AsQueryable();

        if (accountId.HasValue)
            query = query.Where(b => b.AccountId == accountId.Value);

        return await query
            .OrderBy(b => b.TransactionDate)
            .Select(b => new BankReconciliationDto
            {
                TransactionId = b.Id,
                TransactionDate = b.TransactionDate,
                Description = b.Description ?? "",
                ReferenceNo = b.ReferenceNo ?? "",
                Amount = b.Amount,
                TransactionType = b.TransactionType.ToString(),
                IsReconciled = b.IsReconciled
            })
            .ToListAsync(ct);
    }

    public async Task ReconcileTransactionsAsync(string transactionIds, string reconciledBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_ReconcileBankTransactions";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@TransactionIds", transactionIds));
        cmd.Parameters.Add(new SqlParameter("@ReconciledBy", reconciledBy));

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
