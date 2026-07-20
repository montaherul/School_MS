namespace SchoolManagementSystem.Models.DTOs.AI;

public class UsageDailySummaryDto
{
    public DateTime UsageDate { get; set; }
    public string Model { get; set; } = string.Empty;
    public int ConversationCount { get; set; }
    public int RequestCount { get; set; }
    public int TotalPromptTokens { get; set; }
    public int TotalCompletionTokens { get; set; }
    public int TotalTokens { get; set; }
    public decimal TotalCost { get; set; }
    public double AvgLatencyMs { get; set; }
}
