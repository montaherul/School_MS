using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.ViewModels.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IResultAnalyticsService
{
    Task<ResultDashboardViewModel> GetAdminDashboardAsync();
    Task<TabulationSheetDto> GetTabulationSheetAsync(int examId, int? classId, int? sectionId);
    Task<IEnumerable<SubjectPerformanceDto>> GetSubjectAnalysisAsync(int examId);
    Task<ResultSummaryDto> GetClassPerformanceAsync(int examId, int classId);
}

