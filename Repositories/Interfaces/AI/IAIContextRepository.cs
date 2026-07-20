using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Repositories.Interfaces.AI;

public interface IAIContextRepository
{
    Task<AiContextDto?> GetStudentContextAsync(int studentId, CancellationToken ct = default);
}
