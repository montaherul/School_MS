using SchoolManagementSystem.Models.DTOs.Result;
using StudentPortalResultViewModel = SchoolManagementSystem.Models.ViewModels.Result.StudentPortalResultViewModel;

namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IResultPublicationService
{
    Task SubmitExamResultsAsync(int examId, int classId);
    Task ApproveExamResultsAsync(int examId);
    Task PublishResultsAsync(ResultPublishDto dto);
    Task<IEnumerable<ResultPublicationDto>> GetResultPublicationsAsync();
    Task<StudentPortalResultViewModel> GetStudentResultsAsync(int studentId);
    Task<IEnumerable<StudentExamResultDto>> GetAllResultsAsync(int? examId, int? classId, string? status);
    Task RecalculateResultsAsync(int examId);
    Task RecalculateMeritPositionsAsync(int examId);
}

