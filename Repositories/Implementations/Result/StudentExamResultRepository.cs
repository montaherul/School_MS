using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Implementations.Result;
using System.Data;
using System.Data.Common;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class StudentExamResultRepository : BaseRepository<StudentExamResult>, IStudentExamResultRepository 
{ 
    public StudentExamResultRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<ResultListItemDto> Items, int TotalCount)> GetResultListAsync(
        int? examId, int? classId, int? sectionId, int? studentGroupId, int? status,
        string? searchTerm, int pageNumber, int pageSize, CancellationToken ct)
    {
        var items = new List<ResultListItemDto>();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetResultList]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@ExamId", examId);
        AddParameter(command, "@ClassId", classId);
        AddParameter(command, "@SectionId", sectionId);
        AddParameter(command, "@StudentGroupId", studentGroupId);
        AddParameter(command, "@Status", status);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);

        var totalCount = 0;
        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            totalCount = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
        }

        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                items.Add(new ResultListItemDto
                {
                    Id = GetInt32(reader, "Id"),
                    ExamId = GetInt32(reader, "ExamId"),
                    ExamName = GetString(reader, "ExamName"),
                    Term = GetString(reader, "Term"),
                    StudentId = GetInt32(reader, "StudentId"),
                    StudentName = GetString(reader, "StudentName"),
                    StudentNo = GetString(reader, "StudentNo"),
                    RollNumber = GetString(reader, "RollNumber"),
                    ClassId = GetInt32(reader, "ClassId"),
                    ClassName = GetString(reader, "ClassName"),
                    SectionId = GetNullableInt32(reader, "SectionId"),
                    SectionName = GetString(reader, "SectionName"),
                    StudentGroupId = GetNullableInt32(reader, "StudentGroupId"),
                    GroupName = GetString(reader, "GroupName"),
                    TotalMarks = GetDecimal(reader, "TotalMarks"),
                    TotalFullMarks = GetDecimal(reader, "TotalFullMarks"),
                    Gpa = GetDecimal(reader, "Gpa"),
                    Grade = GetString(reader, "Grade"),
                    Position = GetInt32(reader, "Position"),
                    ClassPosition = GetInt32(reader, "ClassPosition"),
                    GroupPosition = GetNullableInt32(reader, "GroupPosition"),
                    IsPassed = GetBoolean(reader, "IsPassed"),
                    FailedSubjectCount = GetInt32(reader, "FailedSubjectCount"),
                    PassedSubjectCount = GetInt32(reader, "PassedSubjectCount"),
                    Status = GetInt32(reader, "Status"),
                    PublishedAt = GetNullableDateTime(reader, "PublishedAt")
                });
            }
        }
        return (items, totalCount);
    }

    public async Task<ResultSummaryStatsDto?> GetResultSummaryStatsAsync(int examId, CancellationToken ct)
    {
        ResultSummaryStatsDto? stats = null;
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetResultSummary]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@ExamId", examId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            stats = new ResultSummaryStatsDto
            {
                TotalStudents = GetInt32(reader, "TotalStudents"),
                PassedCount = GetInt32(reader, "PassedCount"),
                FailedCount = GetInt32(reader, "FailedCount"),
                AverageGPA = GetDecimal(reader, "AverageGPA"),
                HighestGPA = GetDecimal(reader, "HighestGPA"),
                LowestGPA = GetDecimal(reader, "LowestGPA"),
                PassPercentage = GetDecimal(reader, "PassPercentage")
            };
        }
        return stats;
    }

    public async Task<List<GradeDistributionDto>> GetGradeDistributionAsync(int examId, CancellationToken ct)
    {
        var result = new List<GradeDistributionDto>();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetResultSummary]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@ExamId", examId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                result.Add(new GradeDistributionDto
                {
                    Grade = GetString(reader, "Grade"),
                    Count = GetInt32(reader, "Count")
                });
            }
        }
        return result;
    }

    public async Task<List<ClassWiseResultDto>> GetClassWiseResultsAsync(int examId, CancellationToken ct)
    {
        var result = new List<ClassWiseResultDto>();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetResultSummary]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@ExamId", examId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        int rs = 0;
        while (await reader.NextResultAsync(ct))
        {
            rs++;
            if (rs == 2)
            {
                while (await reader.ReadAsync(ct))
                {
                    result.Add(new ClassWiseResultDto
                    {
                        ClassId = GetInt32(reader, "ClassId"),
                        ClassName = GetString(reader, "ClassName"),
                        TotalStudents = GetInt32(reader, "TotalStudents"),
                        PassedCount = GetInt32(reader, "PassedCount"),
                        AverageGPA = GetDecimal(reader, "AverageGPA")
                    });
                }
                break;
            }
        }
        return result;
    }

    public async Task<List<GroupWiseResultDto>> GetGroupWiseResultsAsync(int examId, CancellationToken ct)
    {
        var result = new List<GroupWiseResultDto>();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetResultSummary]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@ExamId", examId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        int rs = 0;
        while (await reader.NextResultAsync(ct))
        {
            rs++;
            if (rs == 3)
            {
                while (await reader.ReadAsync(ct))
                {
                    result.Add(new GroupWiseResultDto
                    {
                        StudentGroupId = GetNullableInt32(reader, "StudentGroupId"),
                        GroupName = GetString(reader, "GroupName"),
                        TotalStudents = GetInt32(reader, "TotalStudents"),
                        PassedCount = GetInt32(reader, "PassedCount"),
                        AverageGPA = GetDecimal(reader, "AverageGPA")
                    });
                }
                break;
            }
        }
        return result;
    }

    public async Task<List<TopStudentDto>> GetTopStudentsAsync(int examId, CancellationToken ct)
    {
        var result = new List<TopStudentDto>();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetResultSummary]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@ExamId", examId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        int rs = 0;
        while (await reader.NextResultAsync(ct))
        {
            rs++;
            if (rs == 4)
            {
                while (await reader.ReadAsync(ct))
                {
                    result.Add(new TopStudentDto
                    {
                        Id = GetInt32(reader, "Id"),
                        StudentId = GetInt32(reader, "StudentId"),
                        StudentName = GetString(reader, "StudentName"),
                        RollNumber = GetString(reader, "RollNumber"),
                        ClassName = GetString(reader, "ClassName"),
                        Gpa = GetDecimal(reader, "Gpa"),
                        Grade = GetString(reader, "Grade"),
                        Position = GetInt32(reader, "Position"),
                        ClassPosition = GetInt32(reader, "ClassPosition")
                    });
                }
                break;
            }
        }
        return result;
    }

    public async Task<(List<StudentResultExamDto> Exams, List<StudentResultSubjectDto> Subjects)> GetStudentResultsAsync(int studentId, int? academicYearId, CancellationToken ct)
    {
        var exams = new List<StudentResultExamDto>();
        var subjects = new List<StudentResultSubjectDto>();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetStudentResults]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@StudentId", studentId);
        AddParameter(command, "@AcademicYearId", academicYearId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            exams.Add(new StudentResultExamDto
            {
                Id = GetInt32(reader, "Id"),
                ExamId = GetInt32(reader, "ExamId"),
                ExamName = GetString(reader, "ExamName"),
                Term = GetString(reader, "Term"),
                StartsOn = DateOnly.FromDateTime(GetDateTime(reader, "StartsOn")),
                EndsOn = DateOnly.FromDateTime(GetDateTime(reader, "EndsOn")),
                TotalMarks = GetDecimal(reader, "TotalMarks"),
                TotalFullMarks = GetDecimal(reader, "TotalFullMarks"),
                Gpa = GetDecimal(reader, "Gpa"),
                Grade = GetString(reader, "Grade"),
                Position = GetInt32(reader, "Position"),
                ClassPosition = GetInt32(reader, "ClassPosition"),
                GroupPosition = GetNullableInt32(reader, "GroupPosition"),
                IsPassed = GetBoolean(reader, "IsPassed"),
                FailedSubjectCount = GetInt32(reader, "FailedSubjectCount"),
                PassedSubjectCount = GetInt32(reader, "PassedSubjectCount"),
                PublishedAt = GetNullableDateTime(reader, "PublishedAt"),
                Status = GetInt32(reader, "Status")
            });
        }

        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                subjects.Add(new StudentResultSubjectDto
                {
                    Id = GetInt32(reader, "Id"),
                    ExamId = GetInt32(reader, "ExamId"),
                    ExamName = GetString(reader, "ExamName"),
                    SubjectId = GetInt32(reader, "SubjectId"),
                    SubjectName = GetString(reader, "SubjectName"),
                    SubjectCode = GetString(reader, "SubjectCode"),
                    IsOptionalSubject = GetBoolean(reader, "IsOptionalSubject"),
                    IsReligionSubject = GetBoolean(reader, "IsReligionSubject"),
                    MarksObtained = GetDecimal(reader, "MarksObtained"),
                    FullMarks = GetDecimal(reader, "FullMarks"),
                    PassMarks = GetDecimal(reader, "PassMarks"),
                    Grade = GetString(reader, "Grade"),
                    GradePoint = GetDecimal(reader, "GradePoint"),
                    IsPassed = GetBoolean(reader, "IsPassed")
                });
            }
        }
        return (exams, subjects);
    }

    public async Task<ReportCardDto?> GetReportCardAsync(int examId, int studentId, CancellationToken ct)
    {
        ReportCardDto? reportCard = null;
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetReportCard]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@ExamId", examId);
        AddParameter(command, "@StudentId", studentId);

        await using var reader = await command.ExecuteReaderAsync(ct);

        if (await reader.ReadAsync(ct))
        {
            reportCard = new ReportCardDto
            {
                SchoolName = GetString(reader, "SchoolName"),
                SchoolAddress = GetString(reader, "SchoolAddress"),
                EIIN = GetString(reader, "EIIN"),
                SchoolLogoPath = GetString(reader, "SchoolLogoPath")
            };
        }

        if (reportCard != null && await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
        {
            reportCard.StudentId = GetInt32(reader, "StudentId");
            reportCard.StudentName = GetString(reader, "StudentName");
            reportCard.StudentNo = GetString(reader, "StudentNo");
            reportCard.RollNumber = GetString(reader, "RollNumber");
            reportCard.DateOfBirth = GetNullableDateOnly(reader, "DateOfBirth");
            reportCard.FatherName = GetString(reader, "FatherName");
            reportCard.MotherName = GetString(reader, "MotherName");
            reportCard.ClassId = GetInt32(reader, "ClassId");
            reportCard.ClassName = GetString(reader, "ClassName");
            reportCard.SectionId = GetNullableInt32(reader, "SectionId");
            reportCard.SectionName = GetString(reader, "SectionName");
            reportCard.StudentGroupId = GetNullableInt32(reader, "StudentGroupId");
            reportCard.GroupName = GetString(reader, "GroupName");
            reportCard.PhotoPath = GetString(reader, "PhotoPath");
        }

        if (reportCard != null && await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
        {
            reportCard.ExamId = GetInt32(reader, "ExamId");
            reportCard.ExamName = GetString(reader, "ExamName");
            reportCard.Term = GetString(reader, "Term");
            reportCard.StartsOn = DateOnly.FromDateTime(GetDateTime(reader, "StartsOn"));
            reportCard.EndsOn = DateOnly.FromDateTime(GetDateTime(reader, "EndsOn"));
            reportCard.AcademicYearId = GetInt32(reader, "AcademicYearId");
            reportCard.AcademicYearName = GetString(reader, "AcademicYearName");
        }

        if (reportCard != null && await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                var marks = new ComponentMarksDto();
                BuildReportCardMarksFromReader(reader, marks, GetNullableString(reader, "ComponentValues"));

                reportCard.Subjects.Add(new ReportCardSubjectDto
                {
                    SubjectId = GetInt32(reader, "SubjectId"),
                    SubjectName = GetString(reader, "SubjectName"),
                    SubjectCode = GetString(reader, "SubjectCode"),
                    FullMarks = GetDecimal(reader, "FullMarks"),
                    PassMarks = GetDecimal(reader, "PassMarks"),
                    MarksObtained = GetDecimal(reader, "MarksObtained"),
                    Grade = GetString(reader, "Grade"),
                    GradePoint = GetDecimal(reader, "GradePoint"),
                    IsPassed = GetBoolean(reader, "IsPassed"),
                    IsOptionalSubject = GetBoolean(reader, "IsOptionalSubject"),
                    IsReligionSubject = GetBoolean(reader, "IsReligionSubject"),
                    ComponentMarks = marks
                });
            }
        }

        if (reportCard != null && await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
        {
            reportCard.Summary = new ReportCardSummaryDto
            {
                TotalMarks = GetDecimal(reader, "TotalMarks"),
                TotalFullMarks = GetDecimal(reader, "TotalFullMarks"),
                Gpa = GetDecimal(reader, "Gpa"),
                Grade = GetString(reader, "Grade"),
                Position = GetInt32(reader, "Position"),
                ClassPosition = GetInt32(reader, "ClassPosition"),
                GroupPosition = GetNullableInt32(reader, "GroupPosition"),
                IsPassed = GetBoolean(reader, "IsPassed"),
                FailedSubjectCount = GetInt32(reader, "FailedSubjectCount"),
                PassedSubjectCount = GetInt32(reader, "PassedSubjectCount"),
                Status = GetInt32(reader, "Status"),
                PublishedAt = GetNullableDateTime(reader, "PublishedAt")
            };
        }

        return reportCard;
    }

    private static void BuildReportCardMarksFromReader(DbDataReader reader, ComponentMarksDto marks, string? componentValuesJson)
    {
        var reportCardColumnMap = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
        {
            ["MarksWritten"] = "WRITTEN",
            ["MarksMCQ"] = "MCQ",
            ["MarksPractical"] = "PRACTICAL",
            ["MarksViva"] = "VIVA",
            ["MarksLab"] = "LAB",
            ["MarksOral"] = "ORAL",
            ["MarksAssignment"] = "ASSIGNMENT",
            ["MarksContinuousAssessment"] = "CONTINUOUS_ASSESSMENT",
            ["MarksCompetency"] = "COMPETENCY",
            ["MarksBehaviour"] = "BEHAVIOUR",
            ["MarksParticipation"] = "PARTICIPATION",
        };

        foreach (var (columnName, code) in reportCardColumnMap)
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
