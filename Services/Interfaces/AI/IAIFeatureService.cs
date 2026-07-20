using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Services.Interfaces.AI;

public interface IAIFeatureService
{
    Task<bool> IsFeatureEnabledAsync(string key, CancellationToken ct = default);
    Task<List<AIFeatureFlagDto>> GetAllFeatureFlagsAsync(CancellationToken ct = default);
    Task<bool> ToggleFeatureAsync(int id, bool enabled, CancellationToken ct = default);
}
