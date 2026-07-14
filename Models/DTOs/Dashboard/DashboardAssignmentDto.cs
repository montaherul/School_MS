namespace SchoolManagementSystem.Models.DTOs.Dashboard;

public record AssignmentDashboardItem(string Subject, string Title, DateTime Deadline);

public class DashboardAssignmentDto
{
    public string Subject { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
}
