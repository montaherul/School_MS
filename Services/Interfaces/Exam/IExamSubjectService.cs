using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.ViewModels.Exam;

namespace SchoolManagementSystem.Services.Interfaces.Exam;

public interface IExamSubjectService
{
    Task SetupSubjectsAsync(int examId, List<ExamSubjectConfigDto> subjects);
    Task<ExamSubjectSetupViewModel> GetSubjectSetupAsync(int examId);
    Task<ExamSubjectConfigDto> GetSubjectSetupAsyncBySubjectId(int examSubjectId);
    Task UpdateSubjectConfigAsync(int examSubjectId, ExamSubjectConfigDto dto);
    Task RemoveSubjectAsync(int examSubjectId);
    Task<List<ExamScheduleDto>> GetScheduleAsync(int examId);
    Task SaveScheduleAsync(int examId, List<ExamScheduleDto> schedules);
}
