using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.AI;

public class AISetting : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
    public bool IsEncrypted { get; set; }
    public int DisplayOrder { get; set; }
}

public class AIProvider : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public AIProviderType ProviderType { get; set; } = AIProviderType.OpenAI;
    public string? BaseUrl { get; set; }
    public string? ApiKeyEncrypted { get; set; }
    public bool IsEnabled { get; set; }
    public int Priority { get; set; }
    public int RetryCount { get; set; } = 3;
    public int TimeoutSeconds { get; set; } = 60;
}

public class AIModel : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public int ProviderId { get; set; }
    public AIModelRole Role { get; set; } = AIModelRole.Student;
    public bool IsDefault { get; set; }
    public int MaxTokens { get; set; } = 2048;
    public double Temperature { get; set; } = 0.7;
    public bool IsEnabled { get; set; } = true;

    public AIProvider Provider { get; set; } = null!;
}

public class AIPrompt : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Role { get; set; } = "Student";
    public string Prompt { get; set; } = string.Empty;
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public string? Category { get; set; }
}

public class AIFeatureFlag : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string DisplayName { get; set; } = string.Empty;
    public bool IsEnabled { get; set; }
    public string? Category { get; set; }
    public string? Description { get; set; }
}

public class AIQuota : BaseEntity
{
    public string Role { get; set; } = "Student";
    public int? DailyLimit { get; set; }
    public int? MinuteLimit { get; set; }
    public int? MaxTokensPerRequest { get; set; }
    public bool IsUnlimited { get; set; }
}

public class AISecurityPolicy : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Category { get; set; } = "General";
}

public class AIAuditLog : BaseEntity
{
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int? EntityId { get; set; }
    public string? OldValue { get; set; }
    public string? NewValue { get; set; }
    public string? IpAddress { get; set; }
    public string? UserAgent { get; set; }
}

public class AIKnowledgeBase : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? FilePath { get; set; }
    public string ContentType { get; set; } = "text";
    public long Size { get; set; }
    public int Version { get; set; } = 1;
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
}

public class AIKnowledgeChunk : BaseEntity
{
    public int KnowledgeBaseId { get; set; }
    public string Content { get; set; } = string.Empty;
    public int ChunkIndex { get; set; }
    public int TokenCount { get; set; }

    public AIKnowledgeBase KnowledgeBase { get; set; } = null!;
}

public class AIHealthCheck : BaseEntity
{
    public string Component { get; set; } = string.Empty;
    public string Status { get; set; } = "Healthy";
    public DateTime LastChecked { get; set; } = DateTime.UtcNow;
    public int? ResponseTimeMs { get; set; }
    public string? ErrorMessage { get; set; }
}

public class AIDashboardCache : BaseEntity
{
    public string Key { get; set; } = string.Empty;
    public string JsonData { get; set; } = string.Empty;
    public DateTime ExpiresAt { get; set; }
}
