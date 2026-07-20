using SchoolManagementSystem.Models.Common;
using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Services.Interfaces.AI;

public interface IAIContextService
{
    Task<Result<AiContextDto>> GetStudentContextAsync(int studentId, CancellationToken ct = default);
}
