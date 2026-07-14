using System.Data;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;

namespace SchoolManagementSystem.Repositories.Implementations.Accounting;

public class ChartOfAccountRepository : BaseRepository<ChartOfAccount>, IChartOfAccountRepository
{
    public ChartOfAccountRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<AccountListItemDto> Items, int TotalRecords)> GetPagedAsync(int page, int pageSize, string? search, int? accountType, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetAccountsPaged";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@PageNumber", page);
        AddParameter(cmd, "@PageSize", pageSize);
        AddParameter(cmd, "@SearchTerm", search);
        AddParameter(cmd, "@AccountType", accountType);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);
        var items = new List<AccountListItemDto>();
        while (await reader.ReadAsync(ct))
        {
            items.Add(new AccountListItemDto
            {
                Id = GetInt32(reader, "Id"),
                AccountCode = GetString(reader, "AccountCode"),
                AccountName = GetString(reader, "AccountName"),
                Description = GetNullableString(reader, "Description"),
                AccountType = GetString(reader, "AccountType"),
                ParentAccount = GetNullableString(reader, "ParentAccount"),
                IsActive = GetBoolean(reader, "IsActive"),
                OpeningBalance = GetDecimal(reader, "OpeningBalance"),
                DisplayOrder = GetInt32(reader, "DisplayOrder"),
                TotalRecords = GetInt32(reader, "TotalRecords")
            });
        }
        return (items, items.FirstOrDefault()?.TotalRecords ?? 0);
    }

    public async Task<List<AccountTreeDto>> GetTreeAsync(CancellationToken ct)
    {
        var accounts = await _db.ChartOfAccounts
            .Where(a => !a.IsDeleted && a.IsActive)
            .OrderBy(a => a.AccountType)
            .ThenBy(a => a.DisplayOrder)
            .ThenBy(a => a.AccountCode)
            .ToListAsync(ct);

        var typeNames = new[] { "", "Asset", "Liability", "Income", "Expense", "Equity" };

        var roots = accounts
            .Where(a => a.ParentAccountId == null)
            .Select(a => MapToTree(a, accounts, typeNames))
            .ToList();

        return roots;
    }

    private static AccountTreeDto MapToTree(ChartOfAccount acc, List<ChartOfAccount> all, string[] typeNames)
    {
        return new AccountTreeDto
        {
            Id = acc.Id,
            AccountCode = acc.AccountCode,
            AccountName = acc.AccountName,
            AccountType = typeNames[(int)acc.AccountType],
            Children = all.Where(c => c.ParentAccountId == acc.Id)
                .Select(c => MapToTree(c, all, typeNames))
                .ToList()
        };
    }

    public async Task<string> GenerateAccountCodeAsync(int accountType, CancellationToken ct)
    {
        var prefix = accountType switch
        {
            1 => "1",
            2 => "2",
            3 => "3",
            4 => "4",
            5 => "5",
            _ => "0"
        };

        var maxCode = await _db.ChartOfAccounts
            .Where(a => a.AccountCode.StartsWith(prefix) && !a.IsDeleted)
            .OrderByDescending(a => a.AccountCode)
            .Select(a => a.AccountCode)
            .FirstOrDefaultAsync(ct);

        if (string.IsNullOrEmpty(maxCode))
            return prefix + "-001";

        var parts = maxCode.Split('-');
        if (parts.Length == 2 && int.TryParse(parts[1], out var num))
            return $"{prefix}-{(num + 1):D3}";

        return prefix + "-001";
    }
}
