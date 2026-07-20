using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.DTOs.AI;

public class AISettingDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public int DisplayOrder { get; set; }
}

public class AISettingUpsertDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
    public int DisplayOrder { get; set; }
}

public class AIProviderDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string ProviderType { get; set; } = string.Empty;
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int Priority { get; set; }
    public int RetryCount { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 60;
}

public class AIProviderUpsertDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProviderType { get; set; } = (int)AIProviderType.OpenAI;
    public string BaseUrl { get; set; } = string.Empty;
    public string ApiKey { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public int Priority { get; set; }
    public int RetryCount { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 60;
}

public class AIModelDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    public bool IsDefault { get; set; }
    public int MaxTokens { get; set; } = 2048;
    public double Temperature { get; set; } = 0.7;
    public bool IsEnabled { get; set; } = true;
}

public class AIModelUpsertDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public int Role { get; set; } = (int)AIModelRole.Student;
    public bool IsDefault { get; set; }
    public int MaxTokens { get; set; } = 2048;
    public double Temperature { get; set; } = 0.7;
    public bool IsEnabled { get; set; } = true;
}

public class AIPromptDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    public string Prompt { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public string Category { get; set; } = string.Empty;
}

public class AIPromptUpsertDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    public string Prompt { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public string Category { get; set; } = string.Empty;
}

public class AIFeatureFlagDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AIFeatureFlagUpsertDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string Category { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
}

public class AIQuotaDto
{
    public int Id { get; set; }
    public string Role { get; set; } = "Student";
    public int DailyLimit { get; set; }
    public int MinuteLimit { get; set; }
    public int MaxTokensPerRequest { get; set; }
    public bool IsUnlimited { get; set; }
}

public class AIQuotaUpsertDto
{
    public int Id { get; set; }
    public string Role { get; set; } = "Student";
    public int DailyLimit { get; set; }
    public int MinuteLimit { get; set; }
    public int MaxTokensPerRequest { get; set; }
    public bool IsUnlimited { get; set; }
}

public class AISecurityPolicyDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
}

public class AISecurityPolicyUpsertDto
{
    public int Id { get; set; }
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Category { get; set; } = "General";
}
