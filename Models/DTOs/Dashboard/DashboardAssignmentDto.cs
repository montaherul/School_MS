namespace SchoolManagementSystem.Models.DTOs.Dashboard;

public class DashboardAssignmentDto
{
    public string Subject { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime Deadline { get; set; }
}
