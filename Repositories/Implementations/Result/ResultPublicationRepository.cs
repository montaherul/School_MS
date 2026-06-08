using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using System.Data;
using System.Data.Common;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

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
}
