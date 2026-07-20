using System.Threading;
using SchoolManagementSystem.Services.Interfaces.AI;

namespace SchoolManagementSystem.Services.Implementations.AI;

public class AICacheInvalidator : IAICacheInvalidator
{
    private long _settingsVersion;
    private long _promptsVersion;
    private long _featureFlagsVersion;
    private long _policiesVersion;

    public event Action? SettingsChanged;
    public event Action? PromptsChanged;
    public event Action? FeatureFlagsChanged;
    public event Action? PoliciesChanged;

    public void InvalidateSettings()
    {
        Interlocked.Increment(ref _settingsVersion);
        SettingsChanged?.Invoke();
    }

    public void InvalidatePrompts()
    {
        Interlocked.Increment(ref _promptsVersion);
        PromptsChanged?.Invoke();
    }

    public void InvalidateFeatureFlags()
    {
        Interlocked.Increment(ref _featureFlagsVersion);
        FeatureFlagsChanged?.Invoke();
    }

    public void InvalidatePolicies()
    {
        Interlocked.Increment(ref _policiesVersion);
        PoliciesChanged?.Invoke();
    }

    public void InvalidateAll()
    {
        InvalidateSettings();
        InvalidatePrompts();
        InvalidateFeatureFlags();
        InvalidatePolicies();
    }
}
