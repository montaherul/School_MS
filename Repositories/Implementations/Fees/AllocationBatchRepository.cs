using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Implementations.Fees;

public class AllocationBatchRepository : IAllocationBatchRepository
{
    private readonly SchoolDbContext _db;

    public AllocationBatchRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<AllocationEngineResultDto> RunBatchAllocationAsync(CancellationToken ct = default)
    {
        var result = new AllocationEngineResultDto();

        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_AutoAllocatePayment";
        command.CommandType = CommandType.StoredProcedure;

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                result.PaymentsProcessed = reader.GetInt32(0);
                result.AllocationsCreated = reader.GetInt32(1);
                result.TotalAllocated = reader.GetDecimal(2);
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }

        return result;
    }
}
