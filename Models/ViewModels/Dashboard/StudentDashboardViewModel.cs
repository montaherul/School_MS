using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class StudentDashboardViewModel
{
    public int Id { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public decimal AttendancePercentage { get; set; }
    public decimal TotalDue { get; set; }
    public string StudentStatus { get; set; } = string.Empty;
    public List<RecentActivityItem> RecentNotices { get; set; } = new();
    public List<AssignmentDashboardItem> UpcomingAssignments { get; set; } = new();
    public List<AssignmentDashboardItem> Assignments { get; set; } = new();
}

public record AssignmentDashboardItem(string Subject, string Title, DateTime Deadline);
