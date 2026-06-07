using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Repositories.Interfaces.Result;

public interface IStudentExamResultRepository : IBaseRepository<StudentExamResult>
{
    Task<(List<ResultListItemDto> Items, int TotalCount)> GetResultListAsync(int? examId, int? classId, int? sectionId, int? studentGroupId, int? status, string? searchTerm, int pageNumber, int pageSize, CancellationToken ct);
    Task<ResultSummaryStatsDto?> GetResultSummaryStatsAsync(int examId, CancellationToken ct);
    Task<List<GradeDistributionDto>> GetGradeDistributionAsync(int examId, CancellationToken ct);
    Task<List<ClassWiseResultDto>> GetClassWiseResultsAsync(int examId, CancellationToken ct);
    Task<List<GroupWiseResultDto>> GetGroupWiseResultsAsync(int examId, CancellationToken ct);
    Task<List<TopStudentDto>> GetTopStudentsAsync(int examId, CancellationToken ct);
    Task<(List<StudentResultExamDto> Exams, List<StudentResultSubjectDto> Subjects)> GetStudentResultsAsync(int studentId, int? academicYearId, CancellationToken ct);
    Task<ReportCardDto?> GetReportCardAsync(int examId, int studentId, CancellationToken ct);
    Task<(TranscriptStudentInfoDto? Info, List<TranscriptExamResultDto> Exams, List<TranscriptSubjectResultDto> Subjects, TranscriptOverallStatsDto? Stats)> GetTranscriptAsync(int studentId, CancellationToken ct);
}