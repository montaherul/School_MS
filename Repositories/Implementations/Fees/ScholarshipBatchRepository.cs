using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Implementations.Fees;

public class ScholarshipBatchRepository : IScholarshipBatchRepository
{
    private readonly SchoolDbContext _db;

    public ScholarshipBatchRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<ScholarshipEngineResultDto> ApplyScholarshipsAsync(CancellationToken ct = default)
    {
        var result = new ScholarshipEngineResultDto();

        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_ApplyScholarships";
        command.CommandType = CommandType.StoredProcedure;

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                result.StudentsProcessed = reader.GetInt32(0);
                result.ScholarshipsApplied = reader.GetInt32(1);
                result.TotalDiscountAmount = reader.GetDecimal(2);
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }

        return result;
    }
}
