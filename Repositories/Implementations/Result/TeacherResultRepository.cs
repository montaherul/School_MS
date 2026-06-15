using System.Data;
using System.Data.Common;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Implementations.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class TeacherResultRepository : ITeacherResultRepository
{
    private readonly SchoolDbContext _context;

    public TeacherResultRepository(SchoolDbContext context)
    {
        _context = context;
    }

    public async Task<List<TeacherAssignedExamDto>> GetTeacherAssignedExamsAsync(int teacherId, int? academicYearId, CancellationToken ct = default)
    {
        var items = new List<TeacherAssignedExamDto>();
        var connection = _context.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetTeacherAssignedExams]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@TeacherId", teacherId);
        AddParameter(command, "@AcademicYearId", academicYearId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            items.Add(new TeacherAssignedExamDto
            {
                ExamId = GetInt32(reader, "ExamId"),
                ExamName = GetString(reader, "ExamName"),
                Term = GetString(reader, "Term"),
                StartsOn = DateOnly.FromDateTime(GetDateTime(reader, "StartsOn")),
                EndsOn = DateOnly.FromDateTime(GetDateTime(reader, "EndsOn")),
                Status = GetInt32(reader, "Status"),
                AcademicYearId = GetInt32(reader, "AcademicYearId"),
                AcademicYearName = GetString(reader, "AcademicYearName")
            });
        }
        return items;
    }

    public async Task<List<TeacherAssignedSubjectDto>> GetTeacherAssignedSubjectsAsync(int teacherId, int classId, int? sectionId, int? groupId, CancellationToken ct = default)
    {
        var items = new List<TeacherAssignedSubjectDto>();
        var connection = _context.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetTeacherAssignedSubjects]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@TeacherId", teacherId);
        AddParameter(command, "@ClassId", classId);
        AddParameter(command, "@SectionId", sectionId);
        AddParameter(command, "@GroupId", groupId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            items.Add(new TeacherAssignedSubjectDto
            {
                SubjectId = GetInt32(reader, "SubjectId"),
                SubjectName = GetString(reader, "SubjectName"),
                SubjectCode = GetString(reader, "SubjectCode"),
                ClassId = GetInt32(reader, "ClassId"),
                ClassName = GetString(reader, "ClassName"),
                SectionId = GetInt32(reader, "SectionId"),
                SectionName = GetString(reader, "SectionName"),
                GroupId = GetNullableInt32(reader, "GroupId"),
                GroupName = GetString(reader, "GroupName")
            });
        }
        return items;
    }

    public async Task<TeacherMarksEntrySheetDto> GetTeacherMarksEntrySheetAsync(int teacherId, int examId, int classId, int sectionId, int subjectId, int? groupId, CancellationToken ct = default)
    {
        var result = new TeacherMarksEntrySheetDto();
        var connection = _context.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetTeacherMarksEntrySheet]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@TeacherId", teacherId);
        AddParameter(command, "@ExamId", examId);
        AddParameter(command, "@ClassId", classId);
        AddParameter(command, "@SectionId", sectionId);
        AddParameter(command, "@SubjectId", subjectId);
        AddParameter(command, "@GroupId", groupId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            result.Authorized = GetBoolean(reader, "Authorized");
        }

        if (result.Authorized && await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                result.Students.Add(new TeacherMarksEntryStudentDto
                {
                    StudentId = GetInt32(reader, "StudentId"),
                    StudentNo = GetString(reader, "StudentNo"),
                    StudentName = GetString(reader, "StudentName"),
                    RollNumber = GetString(reader, "RollNumber"),
                    ClassName = GetString(reader, "ClassName"),
                    SectionName = GetString(reader, "SectionName"),
                    GroupName = GetString(reader, "GroupName")
                });
            }
        }

        if (result.Authorized && await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                var marks = new ComponentMarksDto();
                var componentValuesJson = GetNullableString(reader, "ComponentValues");
                BuildComponentMarksFromReader(reader, marks, componentValuesJson);

                result.ExistingMarks.Add(new MarksEntryExistingDto
                {
                    StudentId = GetInt32(reader, "StudentId"),
                    MarksObtained = GetDecimal(reader, "MarksObtained"),
                    Grade = GetNullableString(reader, "Grade"),
                    GradePoint = GetNullableDecimal(reader, "GradePoint"),
                    ComponentMarks = marks,
                    ComponentValues = componentValuesJson,
                    Status = GetInt32(reader, "Status"),
                    IsLocked = GetBoolean(reader, "IsLocked")
                });
            }
        }

        return result;
    }

    public async Task<TeacherResultSummaryDto> GetTeacherResultSummaryAsync(int teacherId, int examId, int subjectId, int classId, int sectionId, int? groupId, CancellationToken ct = default)
    {
        var summary = new TeacherResultSummaryDto();
        var connection = _context.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetTeacherResultSummary]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@TeacherId", teacherId);
        AddParameter(command, "@ExamId", examId);
        AddParameter(command, "@SubjectId", subjectId);
        AddParameter(command, "@ClassId", classId);
        AddParameter(command, "@SectionId", sectionId);
        AddParameter(command, "@GroupId", groupId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            summary.TotalStudents = GetInt32(reader, "TotalStudents");
            summary.MarksEntered = GetInt32(reader, "MarksEntered");
            summary.PassCount = GetInt32(reader, "PassCount");
            summary.FailCount = GetInt32(reader, "FailCount");
            summary.AvgMarks = GetDecimal(reader, "AvgMarks");
            summary.HighestMarks = GetDecimal(reader, "HighestMarks");
            summary.LowestMarks = GetDecimal(reader, "LowestMarks");
        }

        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                summary.GradeDistribution.Add(new GradeDistributionItemDto
                {
                    Grade = GetString(reader, "Grade"),
                    Count = GetInt32(reader, "Count")
                });
            }
        }

        return summary;
    }

    public async Task<List<TeacherExportRowDto>> GetTeacherExportSheetAsync(int teacherId, int examId, int subjectId, int classId, int sectionId, int? groupId, CancellationToken ct = default)
    {
        var items = new List<TeacherExportRowDto>();
        var connection = _context.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetTeacherExportSheet]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@TeacherId", teacherId);
        AddParameter(command, "@ExamId", examId);
        AddParameter(command, "@SubjectId", subjectId);
        AddParameter(command, "@ClassId", classId);
        AddParameter(command, "@SectionId", sectionId);
        AddParameter(command, "@GroupId", groupId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var marks = new ComponentMarksDto();
            BuildComponentMarksFromReader(reader, marks, null);

            items.Add(new TeacherExportRowDto
            {
                RollNumber = GetString(reader, "RollNumber"),
                StudentNo = GetString(reader, "StudentNo"),
                StudentName = GetString(reader, "StudentName"),
                ClassName = GetString(reader, "ClassName"),
                SectionName = GetString(reader, "SectionName"),
                GroupName = GetString(reader, "GroupName"),
                MarksObtained = GetDecimal(reader, "MarksObtained"),
                ComponentMarks = marks,
                Grade = GetNullableString(reader, "Grade"),
                GradePoint = GetNullableDecimal(reader, "GradePoint"),
                PassStatus = GetString(reader, "PassStatus"),
                Status = GetInt32(reader, "Status")
            });
        }
        return items;
    }

    private static void AddParameter(DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    private static async Task<IAsyncDisposable> OpenConnectionAsync(DbConnection connection)
    {
        var wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed) await connection.OpenAsync();
        return new ConnectionLease(connection, wasClosed);
    }

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly DbConnection _connection;
        private readonly bool _closeOnDispose;

        public ConnectionLease(DbConnection connection, bool closeOnDispose)
        {
            _connection = connection;
            _closeOnDispose = closeOnDispose;
        }

        public async ValueTask DisposeAsync()
        {
            if (_closeOnDispose) await _connection.CloseAsync();
        }
    }

    private static int GetOrdinal(DbDataReader reader, string name) => reader.GetOrdinal(name);
    private static string GetString(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? string.Empty : Convert.ToString(reader[name]) ?? string.Empty;
    private static string? GetNullableString(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? null : Convert.ToString(reader[name]);
    private static int GetInt32(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? 0 : Convert.ToInt32(reader[name]);
    private static int? GetNullableInt32(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? null : Convert.ToInt32(reader[name]);
    private static decimal GetDecimal(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? 0m : Convert.ToDecimal(reader[name]);
    private static decimal? GetNullableDecimal(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? null : Convert.ToDecimal(reader[name]);
    private static bool GetBoolean(DbDataReader reader, string name) => !reader.IsDBNull(GetOrdinal(reader, name)) && Convert.ToBoolean(reader[name]);
    private static DateTime GetDateTime(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? DateTime.MinValue : Convert.ToDateTime(reader[name]);

    private static void BuildComponentMarksFromReader(DbDataReader reader, ComponentMarksDto marks, string? componentValuesJson)
    {
        var codeToColumn = ComponentFieldMapper.GetCodeToColumnMap();
        foreach (var (code, columnName) in codeToColumn)
        {
            var val = GetNullableDecimal(reader, columnName);
            if (val.HasValue)
                marks[code] = val.Value;
        }

        if (!string.IsNullOrEmpty(componentValuesJson))
        {
            try
            {
                var parsed = JsonSerializer.Deserialize<Dictionary<string, decimal?>>(componentValuesJson);
                if (parsed != null)
                    foreach (var kvp in parsed)
                        if (!marks.ContainsKey(kvp.Key))
                            marks[kvp.Key] = kvp.Value;
            }
            catch { }
        }
    }
}
