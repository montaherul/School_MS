using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Interfaces.Result;

/// <summary>
/// Service for generating roll numbers based on configurable strategies.
/// </summary>
public interface IRollGenerationService
{
    Task<RollGenerationConfig?> GetConfigAsync(int academicYearId, int classId, CancellationToken ct = default);
    Task<RollGenerationConfig> SaveConfigAsync(int academicYearId, int classId, RollGenerationStrategy strategy, CancellationToken ct = default);
    Task<List<RollGenerationResult>> GenerateRollsAsync(int academicYearId, int classId, CancellationToken ct = default);
}

public class RollGenerationResult
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int OldRoll { get; set; }
    public int NewRoll { get; set; }
    public string Strategy { get; set; } = string.Empty;
}
