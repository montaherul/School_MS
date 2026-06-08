namespace SchoolManagementSystem.Models.DTOs.Dashboard;

public class DashboardActivityDto
{
    public string Module { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public DateTime At { get; set; }
    public string Summary { get; set; } = string.Empty;
}
