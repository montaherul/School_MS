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

public class FinancialPeriodRepository : BaseRepository<FinancialPeriod>, IFinancialPeriodRepository
{
    public FinancialPeriodRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<FinancialPeriodListItemDto> Items, int TotalRecords)> GetPagedAsync(int page, int pageSize, string? search, CancellationToken ct)
    {
        var query = _db.FinancialPeriods.Where(p => !p.IsDeleted);

        if (!string.IsNullOrEmpty(search))
            query = query.Where(p => p.Name.Contains(search));

        var total = await query.CountAsync(ct);

        var items = await query
            .OrderByDescending(p => p.StartDate)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(p => new FinancialPeriodListItemDto
            {
                Id = p.Id,
                Name = p.Name,
                StartDate = p.StartDate,
                EndDate = p.EndDate,
                Status = p.Status.ToString(),
                IsActive = p.IsActive,
                TotalRecords = total
            })
            .ToListAsync(ct);

        return (items, total);
    }

    public async Task<FinancialPeriod?> GetActivePeriodAsync(CancellationToken ct)
    {
        return await _db.FinancialPeriods
            .Where(p => !p.IsDeleted && p.IsActive && p.Status == FinancialPeriodStatus.Open)
            .OrderByDescending(p => p.StartDate)
            .FirstOrDefaultAsync(ct);
    }

    public async Task CloseFinancialPeriodAsync(int financialPeriodId, string closedBy, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_CloseFinancialPeriod";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@FinancialPeriodId", financialPeriodId));
        cmd.Parameters.Add(new SqlParameter("@ClosedBy", closedBy));

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        await cmd.ExecuteNonQueryAsync(ct);
    }
}
