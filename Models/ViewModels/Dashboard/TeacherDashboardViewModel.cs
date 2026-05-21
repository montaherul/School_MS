using SchoolManagementSystem.Models.ViewModels.Dashboard;

namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class TeacherDashboardViewModel
{
    public int TeacherId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public string TeacherNo { get; set; } = string.Empty;
    
    // Role Indicators
    public bool IsPrincipal { get; set; }
    public bool IsSeniorLecturer { get; set; }
    
    // Stats
    public int MyClassesCount { get; set; }
    public int MySubjectsCount { get; set; }
    public decimal AttendanceRate { get; set; }
    public int PendingResultEntries { get; set; }
    
    // Lists
    public List<string> MyClasses { get; set; } = new();
    public List<string> MySubjects { get; set; } = new();
    public List<AssignmentDashboardItem> UpcomingAssignments { get; set; } = new();
    public List<RecentActivityItem> RecentNotices { get; set; } = new();
    
    // Principal/Admin Specific
    public PrincipalStats? PrincipalStats { get; set; }
}

public class PrincipalStats
{
    public int TotalStaff { get; set; }
    public int TotalStudents { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal ExpensePercentage { get; set; }
    public List<ChartPoint> DepartmentPerformance { get; set; } = new();
}
