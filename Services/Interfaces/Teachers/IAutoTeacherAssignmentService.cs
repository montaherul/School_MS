using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Services.Interfaces.Teachers;

public interface IAutoTeacherAssignmentService
{
    Task<AutoTeacherAssignmentResultDto> AutoAssignTeachersAsync(int examId, CancellationToken ct = default);
}
