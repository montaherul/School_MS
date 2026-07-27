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

    private async Task<AutoBillingResultDto> ExecuteBillingSpAsync(string spName, Dictionary<string, object> parameters, CancellationToken ct)
    {
        var result = new AutoBillingResultDto();
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = spName;
        command.CommandType = CommandType.StoredProcedure;
        foreach (var p in parameters)
            command.Parameters.Add(new SqlParameter(p.Key, p.Value));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                result.InvoicesGenerated = reader.IsDBNull(reader.GetOrdinal("GeneratedCount")) ? 0 : Convert.ToInt32(reader["GeneratedCount"]);
                result.StudentsBilled = reader.IsDBNull(reader.GetOrdinal("StudentsProcessed")) ? 0 : Convert.ToInt32(reader["StudentsProcessed"]);
                result.TotalAmount = reader.IsDBNull(reader.GetOrdinal("TotalAmount")) ? 0 : Convert.ToDecimal(reader["TotalAmount"]);
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
        return result;
    }

    public async Task<AutoBillingResultDto> GenerateMonthlyInvoicesAsync(int academicYearId, int dueDay = 10, int batchSize = 500, CancellationToken ct = default)
    {
        return await ExecuteBillingSpAsync("sp_GenerateMonthlyInvoices", new()
        {
            ["@AcademicYearId"] = academicYearId,
            ["@DueDay"] = dueDay,
            ["@BatchSize"] = batchSize
        }, ct);
    }

    public async Task<AutoBillingResultDto> GenerateOneTimeFeeInvoicesAsync(int academicYearId, int dueDay = 30, int batchSize = 500, CancellationToken ct = default)
    {
        return await ExecuteBillingSpAsync("sp_GenerateOneTimeFeeInvoices", new()
        {
            ["@AcademicYearId"] = academicYearId,
            ["@DueDay"] = dueDay,
            ["@BatchSize"] = batchSize
        }, ct);
    }

    public async Task<AutoBillingResultDto> GenerateExamFeeInvoicesAsync(int academicYearId, string examName = "Term Exam", int dueDay = 15, int batchSize = 500, CancellationToken ct = default)
    {
        return await ExecuteBillingSpAsync("sp_GenerateExamFeeInvoices", new()
        {
            ["@AcademicYearId"] = academicYearId,
            ["@ExamName"] = examName,
            ["@DueDay"] = dueDay,
            ["@BatchSize"] = batchSize
        }, ct);
    }
}
