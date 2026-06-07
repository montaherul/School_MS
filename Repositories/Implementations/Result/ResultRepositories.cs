using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using System.Data;
using System.Data.Common;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class ExamRepository : BaseRepository<Exam>, IExamRepository
{
    public ExamRepository(SchoolDbContext db) : base(db) { }

    public async Task<IEnumerable<ExamListDto>> GetExamsForAdminAsync(int academicYearId, CancellationToken ct)
    {
        var query = _db.Exams.AsNoTracking()
            .Where(e => !e.IsDeleted);

        if (academicYearId > 0)
            query = query.Where(e => e.AcademicYearId == academicYearId);

        return await query
            .OrderByDescending(e => e.CreatedAt)
            .Select(e => new ExamListDto
            {
                Id = e.Id,
                Name = e.Name,
                Term = e.Term,
                StartsOn = e.StartsOn,
                EndsOn = e.EndsOn,
                Status = e.Status,
                AcademicYearId = e.AcademicYearId,
                AcademicYearName = _db.AcademicYears
                    .Where(ay => ay.Id == e.AcademicYearId)
                    .Select(ay => ay.Name)
                    .FirstOrDefault() ?? string.Empty,
                StudentGroupId = e.StudentGroupId,
                IsLocked = e.IsLocked,
                SubjectCount = e.ExamSubjects.Count(es => !es.IsDeleted),
                StudentResultCount = _db.StudentExamResults.Count(ser => ser.ExamId == e.Id && !ser.IsDeleted),
                CreatedAt = e.CreatedAt
            })
            .ToListAsync(ct);
    }

    public async Task<ExamDashboardDto> GetDashboardDataAsync(int academicYearId, CancellationToken ct)
    {
        var result = new ExamDashboardDto();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetExamDashboard]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@AcademicYearId", academicYearId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            result.TotalExams = GetInt32(reader, "TotalExams");
            result.DraftExams = GetInt32(reader, "DraftExams");
            result.SubmittedExams = GetInt32(reader, "SubmittedExams");
            result.ReviewedExams = GetInt32(reader, "ReviewedExams");
            result.ApprovedExams = GetInt32(reader, "ApprovedExams");
            result.PublishedExams = GetInt32(reader, "PublishedExams");
            result.LockedExams = GetInt32(reader, "LockedExams");
            result.UnpublishedExams = GetInt32(reader, "UnpublishedExams");
            result.StudentsAppeared = GetInt32(reader, "StudentsAppeared");
        }
        return result;
    }

    public async Task<List<ExamStatusDistributionDto>> GetStatusDistributionAsync(int academicYearId, CancellationToken ct)
    {
        var result = new List<ExamStatusDistributionDto>();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetExamDashboard]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@AcademicYearId", academicYearId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        // Skip first result set
        while (await reader.NextResultAsync(ct))
        {
            if (reader.FieldCount >= 2 && reader.GetName(0) == "Status")
            {
                while (await reader.ReadAsync(ct))
                {
                    result.Add(new ExamStatusDistributionDto
                    {
                        Status = GetInt32(reader, "Status"),
                        Count = GetInt32(reader, "Count")
                    });
                }
                break;
            }
        }
        return result;
    }

    public async Task<List<ExamPassRateDto>> GetExamPassRatesAsync(int academicYearId, CancellationToken ct)
    {
        var result = new List<ExamPassRateDto>();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetExamDashboard]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@AcademicYearId", academicYearId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        // Skip to pass rate result set
        int resultSet = 0;
        while (await reader.NextResultAsync(ct))
        {
            resultSet++;
            if (resultSet == 3) // Pass rate is the 4th result set (after status dist)
            {
                while (await reader.ReadAsync(ct))
                {
                    var fieldCount = reader.FieldCount;
                    if (fieldCount >= 5 && reader.GetName(0) == "ExamId")
                    {
                        result.Add(new ExamPassRateDto
                        {
                            ExamId = GetInt32(reader, "ExamId"),
                            ExamName = GetString(reader, "ExamName"),
                            TotalStudents = GetInt32(reader, "TotalStudents"),
                            PassedCount = GetInt32(reader, "PassedCount"),
                            FailedCount = GetInt32(reader, "FailedCount"),
                            PassPercentage = GetDecimal(reader, "PassPercentage")
                        });
                    }
                }
                break;
            }
        }
        return result;
    }

    public async Task<(IEnumerable<ExamListDto> Items, int TotalCount)> GetExamListAsync(
        int academicYearId, string? searchTerm, int? status,
        int pageNumber, int pageSize, string sortColumn, string sortDirection, CancellationToken ct)
    {
        var items = new List<ExamListDto>();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetExamList]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@AcademicYearId", academicYearId);
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@Status", status);
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);
        AddParameter(command, "@SortColumn", sortColumn);
        AddParameter(command, "@SortDirection", sortDirection);

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
                items.Add(new ExamListDto
                {
                    Id = GetInt32(reader, "Id"),
                    Name = GetString(reader, "Name"),
                    Term = (ExamTerm)GetInt32(reader, "Term"),
                    StartsOn = DateOnly.FromDateTime(GetDateTime(reader, "StartsOn")),
                    EndsOn = DateOnly.FromDateTime(GetDateTime(reader, "EndsOn")),
                    Status = (ResultWorkflowStatus)GetInt32(reader, "Status"),
                    AcademicYearId = GetInt32(reader, "AcademicYearId"),
                    StudentGroupId = GetNullableInt32(reader, "StudentGroupId"),
                    IsLocked = GetBoolean(reader, "IsLocked"),
                    SubjectCount = GetInt32(reader, "SubjectCount"),
                    StudentResultCount = GetInt32(reader, "StudentResultCount"),
                    CreatedAt = GetDateTime(reader, "CreatedAt")
                });
            }
        }
        return (items, totalCount);
    }

    public async Task<ExamDetailsDto?> GetExamDetailsAsync(int examId, CancellationToken ct)
    {
        return await _db.Exams.AsNoTracking()
            .Where(e => e.Id == examId && !e.IsDeleted)
            .Select(e => new ExamDetailsDto
            {
                Id = e.Id,
                Name = e.Name,
                Term = e.Term,
                StartsOn = e.StartsOn,
                EndsOn = e.EndsOn,
                Status = e.Status,
                AcademicYearId = e.AcademicYearId,
                AcademicYearName = _db.AcademicYears
                    .Where(ay => ay.Id == e.AcademicYearId)
                    .Select(ay => ay.Name)
                    .FirstOrDefault() ?? string.Empty,
                StudentGroupId = e.StudentGroupId,
                StudentGroupName = e.StudentGroup != null ? e.StudentGroup.Name : null,
                IsLocked = e.IsLocked,
                LockedAt = e.LockedAt,
                SubjectCount = e.ExamSubjects.Count(es => !es.IsDeleted),
                StudentResultCount = _db.StudentExamResults.Count(ser => ser.ExamId == e.Id && !ser.IsDeleted),
                CreatedAt = e.CreatedAt,
                CreatedBy = e.CreatedBy
            })
            .FirstOrDefaultAsync(ct);
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
    private static int GetInt32(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : Convert.ToInt32(reader[name]);
    private static int? GetNullableInt32(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToInt32(reader[name]);
    private static decimal GetDecimal(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0m : Convert.ToDecimal(reader[name]);
    private static bool GetBoolean(DbDataReader reader, string name) => !reader.IsDBNull(reader.GetOrdinal(name)) && Convert.ToBoolean(reader[name]);
    private static DateTime GetDateTime(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? DateTime.MinValue : Convert.ToDateTime(reader[name]);

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly DbConnection _connection;
        private readonly bool _closeOnDispose;
        public ConnectionLease(DbConnection connection, bool closeOnDispose) { _connection = connection; _closeOnDispose = closeOnDispose; }
        public async ValueTask DisposeAsync() { if (_closeOnDispose) await _connection.CloseAsync(); }
    }
}

public class MarkEntryRepository : BaseRepository<MarkEntry>, IMarkEntryRepository
{
    public MarkEntryRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<StudentMarkViewModel>> GetMarkEntrySheetAsync(int examId, int classId, int sectionId, int subjectId, CancellationToken ct)
    {
        return await _db.Students.AsNoTracking()
            .Where(s => s.ClassId == classId && s.SectionId == sectionId && !s.IsDeleted)
            .GroupJoin(
                _db.Marks.Where(m => m.ExamId == examId && m.SubjectId == subjectId),
                s => s.Id,
                m => m.StudentId,
                (s, marks) => new { Student = s, Marks = marks })
            .Select(x => new StudentMarkViewModel
            {
                StudentId = x.Student.Id,
                StudentNo = x.Student.StudentNo,
                StudentName = x.Student.FullName,
                RollNumber = x.Student.RollNumber,
                MarksObtained = x.Marks.Select(m => m.MarksObtained).FirstOrDefault(),
                Grade = x.Marks.Select(m => m.Grade).FirstOrDefault(),
                IsLocked = x.Marks.Select(m => m.IsLocked).FirstOrDefault()
            })
            .OrderBy(x => x.RollNumber)
            .ToListAsync(ct);
    }

    public async Task<List<MarksEntryStudentDto>> GetMarksEntryListAsync(int examId, int classId, int sectionId, int subjectId, CancellationToken ct)
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

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
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
                WrittenMarks = GetNullableDecimal(reader, "WrittenMarks"),
                MCQMarks = GetNullableDecimal(reader, "MCQMarks"),
                PracticalMarks = GetNullableDecimal(reader, "PracticalMarks"),
                AssignmentMarks = GetNullableDecimal(reader, "AssignmentMarks"),
                VivaMarks = GetNullableDecimal(reader, "VivaMarks"),
                LabMarks = GetNullableDecimal(reader, "LabMarks"),
                ContinuousAssessmentMarks = GetNullableDecimal(reader, "ContinuousAssessmentMarks"),
                OralMarks = GetNullableDecimal(reader, "OralMarks"),
                Grade = GetNullableString(reader, "Grade"),
                GradePoint = GetNullableDecimal(reader, "GradePoint"),
                IsLocked = GetNullableBoolean(reader, "IsLocked"),
                MarkStatus = GetNullableInt32(reader, "MarkStatus"),
                IsAbsent = GetNullableBoolean(reader, "IsAbsent"),
                HasEntry = GetBoolean(reader, "HasEntry")
            });
        }
        return result;
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
    private static int? GetNullableInt32(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToInt32(reader[name]);
    private static decimal GetDecimal(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0m : Convert.ToDecimal(reader[name]);
    private static decimal? GetNullableDecimal(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToDecimal(reader[name]);
    private static bool GetBoolean(DbDataReader reader, string name) => !reader.IsDBNull(reader.GetOrdinal(name)) && Convert.ToBoolean(reader[name]);
    private static bool? GetNullableBoolean(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToBoolean(reader[name]);
    private static DateTime GetDateTime(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? DateTime.MinValue : Convert.ToDateTime(reader[name]);

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly DbConnection _connection;
        private readonly bool _closeOnDispose;
        public ConnectionLease(DbConnection connection, bool closeOnDispose) { _connection = connection; _closeOnDispose = closeOnDispose; }
        public async ValueTask DisposeAsync() { if (_closeOnDispose) await _connection.CloseAsync(); }
    }
}

public class GradingRuleRepository : BaseRepository<GradingRule>, IGradingRuleRepository 
{ 
    public GradingRuleRepository(SchoolDbContext db) : base(db) { } 
}

public class ResultPublicationRepository : BaseRepository<ResultPublication>, IResultPublicationRepository 
{ 
    public ResultPublicationRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<PublicationDashboardExamDto> Exams, PublicationDashboardSummaryDto Summary)> GetPublicationDashboardAsync(int academicYearId, CancellationToken ct)
    {
        var exams = new List<PublicationDashboardExamDto>();
        var summary = new PublicationDashboardSummaryDto();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetResultPublicationDashboard]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@AcademicYearId", academicYearId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            exams.Add(new PublicationDashboardExamDto
            {
                ExamId = GetInt32(reader, "ExamId"),
                ExamName = GetString(reader, "ExamName"),
                Term = GetString(reader, "Term"),
                StartsOn = DateOnly.FromDateTime(GetDateTime(reader, "StartsOn")),
                EndsOn = DateOnly.FromDateTime(GetDateTime(reader, "EndsOn")),
                Status = GetInt32(reader, "Status"),
                IsLocked = GetBoolean(reader, "IsLocked"),
                LockedAt = GetNullableDateTime(reader, "LockedAt"),
                LockedByUserId = GetNullableInt32(reader, "LockedByUserId"),
                TotalResults = GetInt32(reader, "TotalResults"),
                PublishedResults = GetInt32(reader, "PublishedResults"),
                ApprovedResults = GetInt32(reader, "ApprovedResults"),
                ReviewedResults = GetInt32(reader, "ReviewedResults"),
                SubmittedResults = GetInt32(reader, "SubmittedResults"),
                DraftResults = GetInt32(reader, "DraftResults"),
                LockedDateTime = GetNullableDateTime(reader, "LockedDateTime")
            });
        }

        if (await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
        {
            summary.TotalExams = GetInt32(reader, "TotalExams");
            summary.PublishedExams = GetInt32(reader, "PublishedExams");
            summary.ApprovedExams = GetInt32(reader, "ApprovedExams");
            summary.ReviewedExams = GetInt32(reader, "ReviewedExams");
            summary.SubmittedExams = GetInt32(reader, "SubmittedExams");
            summary.DraftExams = GetInt32(reader, "DraftExams");
            summary.TotalStudentResults = GetInt32(reader, "TotalStudentResults");
            summary.TotalPublishedResults = GetInt32(reader, "TotalPublishedResults");
        }

        return (exams, summary);
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
    private static int GetInt32(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : Convert.ToInt32(reader[name]);
    private static int? GetNullableInt32(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToInt32(reader[name]);
    private static decimal GetDecimal(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0m : Convert.ToDecimal(reader[name]);
    private static bool GetBoolean(DbDataReader reader, string name) => !reader.IsDBNull(reader.GetOrdinal(name)) && Convert.ToBoolean(reader[name]);
    private static DateTime? GetNullableDateTime(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToDateTime(reader[name]);

    private static DateTime GetDateTime(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? DateTime.MinValue : Convert.ToDateTime(reader[name]);

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly DbConnection _connection;
        private readonly bool _closeOnDispose;
        public ConnectionLease(DbConnection connection, bool closeOnDispose) { _connection = connection; _closeOnDispose = closeOnDispose; }
        public async ValueTask DisposeAsync() { if (_closeOnDispose) await _connection.CloseAsync(); }
    }
}

public class StudentSubjectResultRepository : BaseRepository<StudentSubjectResult>, IStudentSubjectResultRepository 
{ 
    public StudentSubjectResultRepository(SchoolDbContext db) : base(db) { } 
}

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
        // Skip first result set
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
        // Skip to 3rd result set
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
        // Skip to 5th result set
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

        // School info
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

        // Student info
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

        // Exam info
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

        // Subjects
        if (reportCard != null && await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
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
                    WrittenMarks = GetNullableDecimal(reader, "WrittenMarks"),
                    MCQMarks = GetNullableDecimal(reader, "MCQMarks"),
                    PracticalMarks = GetNullableDecimal(reader, "PracticalMarks"),
                    VivaMarks = GetNullableDecimal(reader, "VivaMarks"),
                    LabMarks = GetNullableDecimal(reader, "LabMarks"),
                    OralMarks = GetNullableDecimal(reader, "OralMarks"),
                    AssignmentMarks = GetNullableDecimal(reader, "AssignmentMarks"),
                    ContinuousAssessmentMarks = GetNullableDecimal(reader, "ContinuousAssessmentMarks"),
                    MarksWritten = GetNullableDecimal(reader, "MarksWritten"),
                    MarksMCQ = GetNullableDecimal(reader, "MarksMCQ"),
                    MarksPractical = GetNullableDecimal(reader, "MarksPractical"),
                    MarksViva = GetNullableDecimal(reader, "MarksViva"),
                    MarksLab = GetNullableDecimal(reader, "MarksLab"),
                    MarksOral = GetNullableDecimal(reader, "MarksOral"),
                    MarksAssignment = GetNullableDecimal(reader, "MarksAssignment"),
                    MarksContinuousAssessment = GetNullableDecimal(reader, "MarksContinuousAssessment")
                });
            }
        }

        // Summary
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

    public async Task<(TranscriptStudentInfoDto? Info, List<TranscriptExamResultDto> Exams, List<TranscriptSubjectResultDto> Subjects, TranscriptOverallStatsDto? Stats)> GetTranscriptAsync(int studentId, CancellationToken ct)
    {
        TranscriptStudentInfoDto? info = null;
        var exams = new List<TranscriptExamResultDto>();
        var subjects = new List<TranscriptSubjectResultDto>();
        TranscriptOverallStatsDto? stats = null;

        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetTranscript]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@StudentId", studentId);

        await using var reader = await command.ExecuteReaderAsync(ct);

        if (await reader.ReadAsync(ct))
        {
            info = new TranscriptStudentInfoDto
            {
                StudentId = GetInt32(reader, "StudentId"),
                StudentName = GetString(reader, "StudentName"),
                StudentNo = GetString(reader, "StudentNo"),
                RollNumber = GetString(reader, "RollNumber"),
                DateOfBirth = GetNullableDateOnly(reader, "DateOfBirth"),
                FatherName = GetString(reader, "FatherName"),
                MotherName = GetString(reader, "MotherName"),
                ClassId = GetInt32(reader, "ClassId"),
                ClassName = GetString(reader, "ClassName"),
                SectionId = GetNullableInt32(reader, "SectionId"),
                SectionName = GetString(reader, "SectionName"),
                StudentGroupId = GetNullableInt32(reader, "StudentGroupId"),
                GroupName = GetString(reader, "GroupName"),
                CurrentAcademicYear = GetString(reader, "CurrentAcademicYear")
            };
        }

        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                exams.Add(new TranscriptExamResultDto
                {
                    AcademicYearId = GetInt32(reader, "AcademicYearId"),
                    AcademicYearName = GetString(reader, "AcademicYearName"),
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
                    IsPassed = GetBoolean(reader, "IsPassed"),
                    FailedSubjectCount = GetInt32(reader, "FailedSubjectCount"),
                    PassedSubjectCount = GetInt32(reader, "PassedSubjectCount")
                });
            }
        }

        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                subjects.Add(new TranscriptSubjectResultDto
                {
                    ExamId = GetInt32(reader, "ExamId"),
                    ExamName = GetString(reader, "ExamName"),
                    SubjectId = GetInt32(reader, "SubjectId"),
                    SubjectName = GetString(reader, "SubjectName"),
                    SubjectCode = GetString(reader, "SubjectCode"),
                    MarksObtained = GetDecimal(reader, "MarksObtained"),
                    FullMarks = GetDecimal(reader, "FullMarks"),
                    PassMarks = GetDecimal(reader, "PassMarks"),
                    Grade = GetString(reader, "Grade"),
                    GradePoint = GetDecimal(reader, "GradePoint"),
                    IsPassed = GetBoolean(reader, "IsPassed")
                });
            }
        }

        if (await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
        {
            stats = new TranscriptOverallStatsDto
            {
                TotalExamsTaken = GetInt32(reader, "TotalExamsTaken"),
                TotalAcademicYears = GetInt32(reader, "TotalAcademicYears"),
                AverageGPA = GetDecimal(reader, "AverageGPA"),
                BestGPA = GetDecimal(reader, "BestGPA"),
                PassedExams = GetInt32(reader, "PassedExams"),
                FailedExams = GetInt32(reader, "FailedExams")
            };
        }

        return (info, exams, subjects, stats);
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
    private static int GetInt32(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0 : Convert.ToInt32(reader[name]);
    private static int? GetNullableInt32(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToInt32(reader[name]);
    private static decimal GetDecimal(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? 0m : Convert.ToDecimal(reader[name]);
    private static decimal? GetNullableDecimal(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToDecimal(reader[name]);
    private static bool GetBoolean(DbDataReader reader, string name) => !reader.IsDBNull(reader.GetOrdinal(name)) && Convert.ToBoolean(reader[name]);
    private static DateTime GetDateTime(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? DateTime.MinValue : Convert.ToDateTime(reader[name]);
    private static DateTime? GetNullableDateTime(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : Convert.ToDateTime(reader[name]);
    private static DateOnly? GetNullableDateOnly(DbDataReader reader, string name) => reader.IsDBNull(reader.GetOrdinal(name)) ? null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal(name)));

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly DbConnection _connection;
        private readonly bool _closeOnDispose;
        public ConnectionLease(DbConnection connection, bool closeOnDispose) { _connection = connection; _closeOnDispose = closeOnDispose; }
        public async ValueTask DisposeAsync() { if (_closeOnDispose) await _connection.CloseAsync(); }
    }
}

public class ReEvaluationRequestRepository : BaseRepository<ReEvaluationRequest>, IReEvaluationRequestRepository 
{ 
    public ReEvaluationRequestRepository(SchoolDbContext db) : base(db) { } 
}

public class ResultAuditLogRepository : BaseRepository<ResultAuditLog>, IResultAuditLogRepository 
{ 
    public ResultAuditLogRepository(SchoolDbContext db) : base(db) { } 
}

public class MeritResultRepository : BaseRepository<MeritResult>, IMeritResultRepository 
{ 
    public MeritResultRepository(SchoolDbContext db) : base(db) { } 
}

public class FinalResultRepository : BaseRepository<FinalResult>, IFinalResultRepository 
{ 
    public FinalResultRepository(SchoolDbContext db) : base(db) { } 
}

public class PromotionHistoryRepository : BaseRepository<PromotionHistory>, IPromotionHistoryRepository 
{ 
    public PromotionHistoryRepository(SchoolDbContext db) : base(db) { } 
}