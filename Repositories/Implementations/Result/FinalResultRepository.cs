using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using System.Data;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class FinalResultRepository : BaseRepository<FinalResult>, IFinalResultRepository
{
    public FinalResultRepository(SchoolDbContext db) : base(db) { }

    public async Task CalculateFinalPositionsBySpAsync(int academicYearId, CancellationToken ct = default)
    {
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_CalculateFinalPositions]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@AcademicYearId", academicYearId);
        await command.ExecuteNonQueryAsync(ct);
    }
}
