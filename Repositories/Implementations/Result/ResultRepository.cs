using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.ViewModels.Exam;
using SchoolManagementSystem.Models.Enums;
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

        var examIds = await query.Select(e => e.Id).ToListAsync(ct);
        
        if (!examIds.Any())
            return [];

        var academicYearNames = await _db.AcademicYears
            .AsNoTracking()
            .Where(ay => query.Select(e => e.AcademicYearId).Contains(ay.Id))
            .ToDictionaryAsync(ay => ay.Id, ay => ay.Name, ct);

        var subjectCounts = await _db.ExamSubjects
            .AsNoTracking()
            .Where(es => examIds.Contains(es.ExamId) && !es.IsDeleted)
            .GroupBy(es => es.ExamId)
            .Select(g => new { ExamId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ExamId, x => x.Count, ct);

        var studentResultCounts = await _db.StudentExamResults
            .AsNoTracking()
            .Where(ser => examIds.Contains(ser.ExamId) && !ser.IsDeleted)
            .GroupBy(ser => ser.ExamId)
            .Select(g => new { ExamId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.ExamId, x => x.Count, ct);

        var exams = await query
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
                AcademicYearName = academicYearNames.GetValueOrDefault(e.AcademicYearId, string.Empty),
                StudentGroupId = e.StudentGroupId,
                IsLocked = e.IsLocked,
                SubjectCount = subjectCounts.GetValueOrDefault(e.Id, 0),
                StudentResultCount = studentResultCounts.GetValueOrDefault(e.Id, 0),
                CreatedAt = e.CreatedAt
            })
            .ToListAsync(ct);

        return exams;
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
        int resultSet = 0;
        while (await reader.NextResultAsync(ct))
        {
            resultSet++;
            if (resultSet == 3)
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
        var exam = await _db.Exams.AsNoTracking()
            .Where(e => e.Id == examId && !e.IsDeleted)
            .Select(e => new {
                e.Id, e.Name, e.Term, e.StartsOn, e.EndsOn, e.Status,
                e.AcademicYearId, e.ClassId, e.SectionId, e.StudentGroupId,
                e.IsLocked, e.LockedAt, e.CreatedAt, e.CreatedBy,
                StudentGroupName = e.StudentGroup != null ? e.StudentGroup.Name : (string?)null,
                ClassName = e.Class.Name,
                SectionName = e.Section != null ? e.Section.Name : (string?)null
            })
            .FirstOrDefaultAsync(ct);

        if (exam == null) return null;

        var academicYearName = await _db.AcademicYears
            .AsNoTracking()
            .Where(ay => ay.Id == exam.AcademicYearId)
            .Select(ay => ay.Name)
            .FirstOrDefaultAsync(ct);

        var subjectCount = await _db.ExamSubjects
            .AsNoTracking()
            .CountAsync(es => es.ExamId == examId && !es.IsDeleted, ct);

        var subjectNames = await _db.ExamSubjects
            .AsNoTracking()
            .Include(es => es.Subject)
            .Where(es => es.ExamId == examId && !es.IsDeleted)
            .Select(es => es.Subject != null ? es.Subject.Name : "")
            .ToListAsync(ct);

        var studentResultCount = await _db.StudentExamResults
            .AsNoTracking()
            .CountAsync(ser => ser.ExamId == examId && !ser.IsDeleted, ct);

        return new ExamDetailsDto
        {
            Id = exam.Id,
            Name = exam.Name,
            Term = exam.Term,
            StartsOn = exam.StartsOn,
            EndsOn = exam.EndsOn,
            Status = exam.Status,
            AcademicYearId = exam.AcademicYearId,
            AcademicYearName = academicYearName ?? string.Empty,
            ClassId = exam.ClassId,
            ClassName = exam.ClassName,
            SectionId = exam.SectionId,
            SectionName = exam.SectionName,
            StudentGroupId = exam.StudentGroupId,
            StudentGroupName = exam.StudentGroupName,
            IsLocked = exam.IsLocked,
            LockedAt = exam.LockedAt,
            SubjectCount = subjectCount,
            SubjectNames = subjectNames,
            StudentResultCount = studentResultCount,
            CreatedAt = exam.CreatedAt,
            CreatedBy = exam.CreatedBy
        };
    }

    public async Task<ExamReadinessReportDto> GetExamReadinessReportAsync(int academicYearId, CancellationToken ct)
    {
        var report = new ExamReadinessReportDto();
        var connection = _db.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection, ct);
        await using var command = connection.CreateCommand();
        command.CommandText = "sp_GetExamReadinessReport";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@AcademicYearId", academicYearId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            report.TotalExams = GetInt32(reader, "TotalExams");
            report.DraftExams = GetInt32(reader, "DraftExams");
            report.ReadyExams = GetInt32(reader, "ReadyExams");
            report.ClassesWithExams = GetInt32(reader, "ClassesWithExams");
            report.TotalActiveClasses = GetInt32(reader, "TotalActiveClasses");
        }

        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                report.ExamsWithoutSubjects.Add(new ExamReadinessIssueDto
                {
                    ExamId = GetInt32(reader, "ExamId"),
                    ExamName = GetString(reader, "ExamName"),
                    ClassName = GetString(reader, "ClassName"),
                    SubjectCount = GetInt32(reader, "SubjectCount")
                });
            }
        }

        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                report.ExamsWithoutSchedule.Add(new ExamReadinessIssueDto
                {
                    ExamId = GetInt32(reader, "ExamId"),
                    ExamName = GetString(reader, "ExamName"),
                    ClassName = GetString(reader, "ClassName"),
                    SubjectCount = GetInt32(reader, "SubjectCount"),
                    ScheduledCount = GetInt32(reader, "ScheduledCount")
                });
            }
        }

        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                report.ExamsWithoutGradingRules.Add(new ExamReadinessIssueDto
                {
                    ExamId = GetInt32(reader, "ExamId"),
                    ExamName = GetString(reader, "ExamName"),
                    ClassName = GetString(reader, "ClassName")
                });
            }
        }

        return report;
    }
}
