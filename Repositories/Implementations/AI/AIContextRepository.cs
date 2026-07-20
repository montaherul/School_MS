using System.Data;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Repositories.Interfaces.AI;

namespace SchoolManagementSystem.Repositories.Implementations.AI;

public class AIContextRepository : IAIContextRepository
{
    private readonly SchoolDbContext _db;

    public AIContextRepository(SchoolDbContext db) { _db = db; }

    public async Task<AiContextDto?> GetStudentContextAsync(int studentId, CancellationToken ct)
    {
        using var cmd = _db.Database.GetDbConnection().CreateCommand();
        cmd.CommandText = "sp_AIContext_GetStudentContext";
        cmd.CommandType = CommandType.StoredProcedure;
        AddParameter(cmd, "@StudentId", studentId);

        await using var lease = await OpenConnectionAsync(cmd.Connection!, ct);
        using var reader = await cmd.ExecuteReaderAsync(ct);

        AiContextDto? context = null;
        var subjects = new List<string>();

        // Result 1: Student info
        if (await reader.ReadAsync(ct))
        {
            context = new AiContextDto
            {
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                StudentNo = GetString(reader, "StudentNo"),
                ClassName = GetString(reader, "ClassName"),
                SectionName = GetString(reader, "SectionName"),
                GroupName = GetNullableString(reader, "GroupName")
            };
        }

        if (context is null) return null;

        // Result 2: School name
        if (await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
        {
            context.SchoolName = GetString(reader, "SchoolName");
        }

        // Result 3: Academic year
        if (await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
        {
            context.AcademicYear = GetString(reader, "AcademicYearName");
        }

        // Result 4: Subjects
        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                subjects.Add(GetString(reader, "SubjectName"));
            }
        }

        context.Subjects = subjects;
        return context;
    }

    private static async Task<IAsyncDisposable> OpenConnectionAsync(DbConnection connection, CancellationToken ct)
    {
        var wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed) await connection.OpenAsync(ct);
        return new ConnectionLease(connection, wasClosed);
    }

    private static void AddParameter(DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    private static string GetString(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? string.Empty : Convert.ToString(reader[name]) ?? string.Empty;
    private static string? GetNullableString(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToString(reader[name]);
    private static int GetInt32(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : Convert.ToInt32(reader[name]);

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly DbConnection _connection;
        private readonly bool _closeOnDispose;
        public ConnectionLease(DbConnection connection, bool closeOnDispose) { _connection = connection; _closeOnDispose = closeOnDispose; }
        public async ValueTask DisposeAsync() { if (_closeOnDispose) await _connection.CloseAsync(); }
    }
}
