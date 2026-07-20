using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Common;
using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Repositories.Interfaces.AI;
using SchoolManagementSystem.Services.Interfaces.AI;

namespace SchoolManagementSystem.Services.Implementations.AI;

public class AIContextService : IAIContextService
{
    private readonly IAIContextRepository _repo;
    private readonly ILogger<AIContextService> _logger;

    public AIContextService(IAIContextRepository repo, ILogger<AIContextService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<Result<AiContextDto>> GetStudentContextAsync(int studentId, CancellationToken ct)
    {
        try
        {
            var context = await _repo.GetStudentContextAsync(studentId, ct);
            if (context is null)
            {
                _logger.LogWarning("Student {StudentId} not found for AI context", studentId);
                return Result<AiContextDto>.Fail("Student not found.", "STUDENT_NOT_FOUND");
            }
            return Result<AiContextDto>.Success(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load AI context for student {StudentId}", studentId);
            return Result<AiContextDto>.Fail("Failed to load student context.", "CONTEXT_LOAD_FAILED");
        }
    }
}
