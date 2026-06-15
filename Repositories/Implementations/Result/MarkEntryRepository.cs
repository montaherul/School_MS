using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using System.Data;
using System.Data.Common;
using SchoolManagementSystem.Services.Implementations.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class MarkEntryRepository : BaseRepository<MarkEntry>, IMarkEntryRepository
{
    public MarkEntryRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<MarkEntrySheetDto>> GetMarkEntrySheetAsync(int examId, int classId, int sectionId, int subjectId, CancellationToken ct)
    {
        var isOptionalSubject = await _db.ClassSubjects
            .AnyAsync(cs => cs.SchoolClassId == classId && cs.SubjectId == subjectId && cs.IsOptional && !cs.IsDeleted, ct);

        var query = _db.Students.AsNoTracking()
            .Where(s => s.ClassId == classId && s.SectionId == sectionId && !s.IsDeleted);

        if (isOptionalSubject)
            query = query.Where(s => s.OptionalSubjectId == subjectId);

        var raw = await query
            .GroupJoin(
                _db.Marks.Where(m => m.ExamId == examId && m.SubjectId == subjectId),
                s => s.Id,
                m => m.StudentId,
                (s, marks) => new { Student = s, Mark = marks.FirstOrDefault() })
            .OrderBy(x => x.Student.RollNumber)
            .ToListAsync(ct);

        return raw.Select(x => new MarkEntrySheetDto
        {
            StudentId = x.Student.Id,
            StudentNo = x.Student.StudentNo,
            StudentName = x.Student.FullName,
            RollNumber = x.Student.RollNumber,
            MarksObtained = x.Mark?.MarksObtained,
            Grade = x.Mark?.Grade,
            IsLocked = x.Mark?.IsLocked ?? false,
            ComponentMarks = x.Mark != null ? ComponentFieldMapper.FromEntity(x.Mark) : new ComponentMarksDto(),
            ComponentValues = x.Mark?.ComponentValues,
            EnteredByTeacherId = x.Mark?.EnteredByTeacherId
        }).ToList();
    }

    public async Task<List<MarksEntryStudentDto>> GetMarksEntryListAsync(int examId, int classId, int sectionId, int subjectId, CancellationToken ct, int? optionalSubjectId = null)
    {
        var result = new List<MarksEntryStudentDto>();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetMarksEntryList]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@ExamId", examId);
        AddParameter(command, "@ClassId", classId);
        AddParameter(command, "@SectionId", sectionId);
        AddParameter(command, "@SubjectId", subjectId);
        AddParameter(command, "@OptionalSubjectId", (object?)optionalSubjectId ?? DBNull.Value);

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            var marks = new ComponentMarksDto();
            var componentValuesJson = GetNullableString(reader, "ComponentValues");
            BuildComponentMarksFromReader(reader, marks, componentValuesJson);

            result.Add(new MarksEntryStudentDto
            {
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                StudentNo = GetString(reader, "StudentNo"),
                RollNumber = GetString(reader, "RollNumber"),
                ClassId = GetInt32(reader, "ClassId"),
                SectionId = GetInt32(reader, "SectionId"),
                ClassName = GetString(reader, "ClassName"),
                SectionName = GetString(reader, "SectionName"),
                MarkId = GetNullableInt32(reader, "MarkId"),
                MarksObtained = GetNullableDecimal(reader, "MarksObtained"),
                ComponentMarks = marks,
                ComponentValues = componentValuesJson,
                Grade = GetNullableString(reader, "Grade"),
                GradePoint = GetNullableDecimal(reader, "GradePoint"),
                IsLocked = GetNullableBoolean(reader, "IsLocked"),
                MarkStatus = GetNullableInt32(reader, "MarkStatus"),
                HasEntry = GetBoolean(reader, "HasEntry")
            });
        }
        return result;
    }

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
                var parsed = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, decimal?>>(componentValuesJson);
                if (parsed != null)
                    foreach (var kvp in parsed)
                        if (!marks.ContainsKey(kvp.Key))
                            marks[kvp.Key] = kvp.Value;
            }
            catch { }
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
    private static bool? GetNullableBoolean(DbDataReader reader, string name)
    {
        if (reader.IsDBNull(GetOrdinal(reader, name))) return null;
        return Convert.ToBoolean(reader[name]);
    }
    private static DateTime GetDateTime(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? DateTime.MinValue : Convert.ToDateTime(reader[name]);
}
