using System.Data;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Finance;
using SchoolManagementSystem.Repositories.Interfaces.Finance;

namespace SchoolManagementSystem.Repositories.Implementations.Finance;

public class CashFlowRepository : BaseRepository<object>, ICashFlowRepository
{
    public CashFlowRepository(SchoolDbContext db) : base(db) { }

    public async Task<CashFlowStatementDto> GetCashFlowStatementAsync(int year, int? month = null, int? periodType = 3, CancellationToken ct = default)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_GetCashFlowStatement";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@Year", year);
        AddParameter(cmd, "@Month", month ?? 0);
        AddParameter(cmd, "@PeriodType", periodType ?? 3);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var rdr = await cmd.ExecuteReaderAsync(ct);
        var dto = new CashFlowStatementDto();
        if (await rdr.ReadAsync(ct))
        {
            dto.AsOfDate = GetNullableDateTime(rdr, "AsOfDate");
            dto.PeriodName = GetString(rdr, "PeriodName");
            dto.NetCashFlow = GetDecimal(rdr, "NetCashFlow");
            dto.OpeningBalance = GetDecimal(rdr, "OpeningBalance");
            dto.ClosingBalance = GetDecimal(rdr, "ClosingBalance");
        }
        if (await rdr.NextResultAsync(ct))
        {
            dto.OperatingActivities.SectionName = "Operating Activities";
            while (await rdr.ReadAsync(ct))
                dto.OperatingActivities.Lines.Add(new CashFlowLineDto
                {
                    Label = GetString(rdr, "Label"),
                    Amount = GetDecimal(rdr, "Amount"),
                    IsTotal = GetBoolean(rdr, "IsTotal")
                });
            dto.OperatingActivities.Total = dto.OperatingActivities.Lines.Where(x => x.IsTotal).Sum(x => x.Amount);
        }
        if (await rdr.NextResultAsync(ct))
        {
            dto.InvestingActivities.SectionName = "Investing Activities";
            while (await rdr.ReadAsync(ct))
                dto.InvestingActivities.Lines.Add(new CashFlowLineDto
                {
                    Label = GetString(rdr, "Label"),
                    Amount = GetDecimal(rdr, "Amount"),
                    IsTotal = GetBoolean(rdr, "IsTotal")
                });
            dto.InvestingActivities.Total = dto.InvestingActivities.Lines.Where(x => x.IsTotal).Sum(x => x.Amount);
        }
        if (await rdr.NextResultAsync(ct))
        {
            dto.FinancingActivities.SectionName = "Financing Activities";
            while (await rdr.ReadAsync(ct))
                dto.FinancingActivities.Lines.Add(new CashFlowLineDto
                {
                    Label = GetString(rdr, "Label"),
                    Amount = GetDecimal(rdr, "Amount"),
                    IsTotal = GetBoolean(rdr, "IsTotal")
                });
            dto.FinancingActivities.Total = dto.FinancingActivities.Lines.Where(x => x.IsTotal).Sum(x => x.Amount);
        }
        return dto;
    }
}
