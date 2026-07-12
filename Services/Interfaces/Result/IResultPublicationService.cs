using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IResultPublicationService
{
    Task SubmitExamResultsAsync(int examId, int classId);
    Task ApproveExamResultsAsync(int examId);
    Task PublishResultsAsync(ResultPublishDto dto);
    Task ReviewExamResultsAsync(int examId, int reviewerUserId);
    Task ApproveReviewedResultsAsync(int examId, int approverUserId);
    Task UnpublishResultsAsync(int examId);
    Task RepublishResultsAsync(int examId);
    Task<ResultPublicationDto> GetPublicationStatusAsync(int examId);
    Task<IEnumerable<ResultPublicationDto>> GetResultPublicationsAsync();
    Task<StudentPortalResultDto> GetStudentResultsAsync(int studentId);
    Task<IEnumerable<StudentExamResultDto>> GetAllResultsAsync(int? examId, int? classId, string? status);
    Task<int> RejectResultsAsync(int examId, string updatedBy);
    Task<List<PublicationHistoryEntryDto>> GetPublicationHistoryAsync(int yearId, CancellationToken ct = default);
    Task<(List<PublicationDashboardExamDto> Exams, PublicationDashboardSummaryDto? Summary)> GetPublicationDashboardAsync(int academicYearId, CancellationToken ct = default);

    // DIP compliance: replaces direct IResultAuditLogRepository access from controllers
    Task<List<SchoolManagementSystem.Models.Entities.Result.ResultAuditLog>> GetAuditLogsAsync(int? examId, int? studentId, CancellationToken ct = default);
}

