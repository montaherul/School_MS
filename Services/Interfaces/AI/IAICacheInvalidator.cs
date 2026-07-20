namespace SchoolManagementSystem.Services.Interfaces.AI;

public interface IAICacheInvalidator
{
    void InvalidateSettings();
    void InvalidatePrompts();
    void InvalidateFeatureFlags();
    void InvalidatePolicies();
    void InvalidateAll();
    event Action? SettingsChanged;
    event Action? PromptsChanged;
    event Action? FeatureFlagsChanged;
    event Action? PoliciesChanged;
}
