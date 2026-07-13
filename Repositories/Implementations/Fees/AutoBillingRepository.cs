using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Implementations.Fees;

public class AutoBillingRepository : IAutoBillingRepository
{
    private readonly SchoolDbContext _db;

    public AutoBillingRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<AutoBillingResultDto> GenerateMonthlyInvoicesAsync(int academicYearId, int dueDay = 10, int batchSize = 500, CancellationToken ct = default)
    {
        var result = new AutoBillingResultDto();
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GenerateMonthlyInvoices";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@AcademicYearId", academicYearId));
        command.Parameters.Add(new SqlParameter("@DueDay", dueDay));
        command.Parameters.Add(new SqlParameter("@BatchSize", batchSize));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                result.InvoicesGenerated = reader.IsDBNull(reader.GetOrdinal("GeneratedCount")) ? 0 : Convert.ToInt32(reader["GeneratedCount"]);
                result.StudentsBilled = result.InvoicesGenerated;
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
        return result;
    }
}
