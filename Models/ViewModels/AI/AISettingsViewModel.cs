using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Models.DTOs.AI;

namespace SchoolManagementSystem.Models.ViewModels.AI;

public class AISettingsIndexViewModel
{
    public List<AISettingDto> Settings { get; set; } = [];
    public List<AIProviderDto> Providers { get; set; } = [];
    public List<AIModelDto> Models { get; set; } = [];
    public List<AIFeatureFlagDto> FeatureFlags { get; set; } = [];
    public List<AIQuotaDto> Quotas { get; set; } = [];
    public List<AISecurityPolicyDto> SecurityPolicies { get; set; } = [];
    public List<AIPromptDto> Prompts { get; set; } = [];
}

public class AIProviderEditViewModel
{
    public AIProviderDto Provider { get; set; } = new();
    public List<SelectListItem> ProviderTypes { get; set; } = [];
}

public class AIModelEditViewModel
{
    public AIModelDto Model { get; set; } = new();
    public List<SelectListItem> Providers { get; set; } = [];
    public List<SelectListItem> Roles { get; set; } = [];
}

public class AIPromptEditViewModel
{
    public AIPromptDto Prompt { get; set; } = new();
}

public class AIFeatureFlagEditViewModel
{
    public AIFeatureFlagDto Flag { get; set; } = new();
}

public class AIQuotaEditViewModel
{
    public AIQuotaDto Quota { get; set; } = new();
    public List<SelectListItem> Roles { get; set; } = [];
}
