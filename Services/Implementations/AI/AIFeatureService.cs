using SchoolManagementSystem.Models.DTOs.AI;
using SchoolManagementSystem.Repositories.Interfaces.AI;
using SchoolManagementSystem.Services.Interfaces.AI;

namespace SchoolManagementSystem.Services.Implementations.AI;

public class AIFeatureService : IAIFeatureService
{
    private readonly IAIAdminRepository _adminRepo;

    public AIFeatureService(IAIAdminRepository adminRepo)
    {
        _adminRepo = adminRepo;
    }

    public async Task<bool> IsFeatureEnabledAsync(string key, CancellationToken ct)
    {
        var flags = await _adminRepo.GetFeatureFlagsAsync(ct);
        return flags.FirstOrDefault(f => f.Key == key)?.IsEnabled ?? false;
    }

    public async Task<List<AIFeatureFlagDto>> GetAllFeatureFlagsAsync(CancellationToken ct)
    {
        return await _adminRepo.GetFeatureFlagsAsync(ct);
    }

    public async Task<bool> ToggleFeatureAsync(int id, bool enabled, CancellationToken ct)
    {
        var flags = await _adminRepo.GetFeatureFlagsAsync(ct);
        var flag = flags.FirstOrDefault(f => f.Id == id);
        if (flag == null) return false;

        var dto = new AIFeatureFlagUpsertDto
        {
            Key = flag.Key,
            DisplayName = flag.DisplayName,
            IsEnabled = enabled,
            Category = flag.Category,
            Description = flag.Description
        };
        await _adminRepo.UpsertFeatureFlagAsync(dto, "system", ct);
        return true;
    }
}
