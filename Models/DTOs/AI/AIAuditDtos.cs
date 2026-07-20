namespace SchoolManagementSystem.Models.DTOs.AI;

public class AIAuditLogDto
{
    public int Id { get; set; }
    public string Action { get; set; } = string.Empty;
    public string EntityType { get; set; } = string.Empty;
    public int EntityId { get; set; }
    public string OldValue { get; set; } = string.Empty;
    public string NewValue { get; set; } = string.Empty;
    public string IpAddress { get; set; } = string.Empty;
    public string UserAgent { get; set; } = string.Empty;
    public string PerformedBy { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class AIHealthCheckDto
{
    public int Id { get; set; }
    public string Component { get; set; } = string.Empty;
    public string Status { get; set; } = "Healthy";
    public DateTime LastChecked { get; set; }
    public int ResponseTimeMs { get; set; }
    public string ErrorMessage { get; set; } = string.Empty;
}
