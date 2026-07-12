using SchoolManagementSystem.Models.DTOs.Exam;

namespace SchoolManagementSystem.Services.Interfaces.Exam;

public interface IAutoScheduleService
{
    Task<AutoScheduleResultDto> GenerateScheduleAsync(int examId, CancellationToken ct = default);
}
