using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Accounting;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Repositories.Implementations;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Repositories.Implementations.Accounting;

public class JournalEntryRepository : BaseRepository<JournalEntry>, IJournalEntryRepository
{
    public JournalEntryRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<JournalEntryListItemDto> Items, int TotalRecords)> GetPagedAsync(int page, int pageSize, string? search, int? entryType, CancellationToken ct)
    {
        var query = _db.JournalEntries.Where(j => !j.IsDeleted);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(j => j.JournalNo.Contains(search) || (j.Description != null && j.Description.Contains(search)));

        if (entryType.HasValue)
            query = query.Where(j => j.EntryType == (JournalEntryType)entryType.Value);

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(j => j.EntryDate)
            .ThenByDescending(j => j.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(j => new JournalEntryListItemDto
            {
                Id = j.Id,
                JournalNo = j.JournalNo,
                EntryDate = j.EntryDate,
                EntryType = j.EntryType.ToString(),
                Description = j.Description,
                IsPosted = j.IsPosted,
                TotalRecords = total
            })
            .ToListAsync(ct);

        foreach (var item in items)
        {
            var lines = await _db.JournalEntryLines
                .Where(l => l.JournalEntryId == item.Id && !l.IsDeleted)
                .ToListAsync(ct);
            item.TotalDebit = lines.Where(l => l.LineType == JournalLineType.Debit).Sum(l => l.Amount);
            item.TotalCredit = lines.Where(l => l.LineType == JournalLineType.Credit).Sum(l => l.Amount);
        }

        return (items, total);
    }

    public async Task<JournalEntryDetailDto?> GetDetailAsync(int id, CancellationToken ct)
    {
        var entry = await _db.JournalEntries
            .Where(j => j.Id == id && !j.IsDeleted)
            .Select(j => new JournalEntryDetailDto
            {
                Id = j.Id,
                JournalNo = j.JournalNo,
                EntryDate = j.EntryDate,
                EntryType = j.EntryType.ToString(),
                Description = j.Description,
                FinancialPeriodId = j.FinancialPeriodId,
                IsPosted = j.IsPosted,
                PostedAt = j.PostedAt,
                PostedBy = j.PostedBy,
                CreatedBy = j.CreatedBy,
                CreatedAt = j.CreatedAt
            })
            .FirstOrDefaultAsync(ct);

        if (entry == null) return null;

        if (entry.FinancialPeriodId.HasValue)
        {
            var period = await _db.FinancialPeriods
                .Where(p => p.Id == entry.FinancialPeriodId.Value)
                .Select(p => p.Name)
                .FirstOrDefaultAsync(ct);
            entry.FinancialPeriodName = period;
        }

        entry.Lines = await _db.JournalEntryLines
            .Where(l => l.JournalEntryId == id && !l.IsDeleted)
            .Join(_db.ChartOfAccounts.Where(a => !a.IsDeleted),
                l => l.AccountId, a => a.Id,
                (l, a) => new JournalEntryLineDetailDto
                {
                    Id = l.Id,
                    AccountId = l.AccountId,
                    AccountCode = a.AccountCode,
                    AccountName = a.AccountName,
                    LineType = l.LineType == JournalLineType.Debit ? "Debit" : "Credit",
                    Amount = l.Amount,
                    Narration = l.Narration
                })
            .OrderBy(l => l.Id)
            .ToListAsync(ct);

        return entry;
    }

    public async Task<string> GenerateJournalNoAsync(DateTime entryDate, CancellationToken ct)
    {
        var prefix = $"JV-{entryDate:yyyyMM}-";
        var maxNo = await _db.JournalEntries
            .Where(j => j.JournalNo.StartsWith(prefix) && !j.IsDeleted)
            .OrderByDescending(j => j.JournalNo)
            .Select(j => j.JournalNo)
            .FirstOrDefaultAsync(ct);

        if (string.IsNullOrEmpty(maxNo))
            return prefix + "001";

        var parts = maxNo.Split('-');
        if (parts.Length == 3 && int.TryParse(parts[2], out var num))
            return $"{prefix}{(num + 1):D3}";

        return prefix + "001";
    }

    public async Task PostJournalEntryAsync(int journalEntryId, string postedBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_PostJournalEntry";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@JournalEntryId", journalEntryId));
        cmd.Parameters.Add(new SqlParameter("@PostedBy", postedBy));

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
