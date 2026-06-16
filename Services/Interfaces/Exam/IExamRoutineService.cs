using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Services.Interfaces.Exam;

public interface IExamRoutineService
{
    Task<List<ExamRoutineDto>> GetStudentRoutineAsync(int studentId, CancellationToken ct = default);
    Task<ExamRoutineViewModel> GetStudentRoutineViewAsync(int studentId, CancellationToken ct = default);
    Task<List<ExamRoutineDto>> GetGuardianRoutineAsync(int studentId, CancellationToken ct = default);
    Task<ExamRoutineViewModel> GetGuardianRoutineViewAsync(int studentId, CancellationToken ct = default);
    Task<List<ExamRoutineDto>> GetTeacherRoutineAsync(int teacherId, CancellationToken ct = default);
    Task<ExamRoutineViewModel> GetTeacherRoutineViewAsync(int teacherId, CancellationToken ct = default);
    Task<List<ExamRoutineDto>> GetClassRoutineAsync(int examId, int classId, int? groupId = null, CancellationToken ct = default);
    Task<List<ExamRoutineDto>> GetPublishedExamsRoutineAsync(int classId, int? groupId = null, CancellationToken ct = default);
    Task<string> RenderRoutineHtmlAsync(List<ExamRoutineDto> schedules, string examName, string className, string? groupName, CancellationToken ct = default);
}
