namespace SchoolManagementSystem.Models.DTOs.AI;

public class AIDashboardStatsDto
{
    public int TotalRequests { get; set; }
    public int ActiveUsers { get; set; }
    public int StudentsToday { get; set; }
    public int TeachersToday { get; set; }
    public long TotalTokens { get; set; }
    public long PromptTokens { get; set; }
    public long CompletionTokens { get; set; }
    public decimal DailyCost { get; set; }
    public decimal MonthlyCost { get; set; }
    public double AvgResponseTimeMs { get; set; }
    public string OpenAiStatus { get; set; } = "Unknown";
    public double ErrorRate { get; set; }
    public int RateLimitHits { get; set; }
    public int BlockedInjections { get; set; }
}

public class AIRequestChartPoint
{
    public string Hour { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class AICostChartPoint
{
    public string Date { get; set; } = string.Empty;
    public decimal Cost { get; set; }
}

public class TopUserDto
{
    public int UserId { get; set; }
    public string Name { get; set; } = string.Empty;
    public int RequestCount { get; set; }
}

public class TopSubjectDto
{
    public string SubjectName { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class TopPromptDto
{
    public string Prompt { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class AIConversationAdminDto
{
    public int Id { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Status { get; set; } = "Active";
    public bool IsPinned { get; set; }
    public int MessageCount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }
}
