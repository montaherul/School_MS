using SchoolManagementSystem.Models.DTOs.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IResultAnalyticsService
{
    Task<AdminDashboardDto> GetAdminDashboardAsync();
    Task<TabulationSheetDto> GetTabulationSheetAsync(int examId, int? classId, int? sectionId);
    Task<IEnumerable<SubjectPerformanceDto>> GetSubjectAnalysisAsync(int examId);
    Task<ResultSummaryDto> GetClassPerformanceAsync(int examId, int classId);
}

