using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Repositories.Implementations.Fees;

public class AutoFeeAssignmentRepository : IAutoFeeAssignmentRepository
{
    private readonly SchoolDbContext _db;

    public AutoFeeAssignmentRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<AutoAssignmentResultDto> AssignFeeStructureAsync(int studentId, int academicYearId, CancellationToken ct = default)
    {
        var result = new AutoAssignmentResultDto();
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_AutoAssignStudentFeeStructure";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@StudentId", studentId));
        command.Parameters.Add(new SqlParameter("@AcademicYearId", academicYearId));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            var rows = await command.ExecuteNonQueryAsync(ct);
            result.AssignmentsCreated = rows;
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
        return result;
    }

    public async Task<FeeMigrationResultDto> MigrateFeeStructureAsync(int studentId, int oldClassId, int newClassId, int academicYearId, CancellationToken ct = default)
    {
        var result = new FeeMigrationResultDto();
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_MigrateStudentFeeStructure";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@StudentId", studentId));
        command.Parameters.Add(new SqlParameter("@OldClassId", oldClassId));
        command.Parameters.Add(new SqlParameter("@NewClassId", newClassId));
        command.Parameters.Add(new SqlParameter("@AcademicYearId", academicYearId));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                result.OldAssignmentsDeactivated = reader.IsDBNull(reader.GetOrdinal("Deactivated")) ? 0 : Convert.ToInt32(reader["Deactivated"]);
                result.NewAssignmentsCreated = reader.IsDBNull(reader.GetOrdinal("Created")) ? 0 : Convert.ToInt32(reader["Created"]);
            }
            result.OldAssignmentsDeactivated = result.OldAssignmentsDeactivated > 0 || result.NewAssignmentsCreated > 0 ? result.OldAssignmentsDeactivated : 0;
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
        return result;
    }

    public async Task<FeeCopyResultDto> CopyFeeStructureForAcademicYearAsync(int fromAcademicYearId, int toAcademicYearId, CancellationToken ct = default)
    {
        var result = new FeeCopyResultDto();
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_CopyFeeStructureForAcademicYear";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@FromAcademicYearId", fromAcademicYearId));
        command.Parameters.Add(new SqlParameter("@ToAcademicYearId", toAcademicYearId));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            if (await reader.ReadAsync(ct))
            {
                result.StructuresCopied = reader.IsDBNull(reader.GetOrdinal("CopiedCount")) ? 0 : Convert.ToInt32(reader["CopiedCount"]);
            }
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
        return result;
    }
}
