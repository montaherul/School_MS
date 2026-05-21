namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TotalStudents { get; set; }
    public int PendingAdmissions { get; set; }
    public decimal FeesCollected { get; set; }
    public decimal FeesDue { get; set; }
    public decimal AttendancePercentage { get; set; }
    public IReadOnlyList<ChartPoint> StudentsByClass { get; set; } = [];
    public IReadOnlyList<ChartPoint> MonthlyCollections { get; set; } = [];
    public IReadOnlyList<RecentActivityItem> RecentActivities { get; set; } = [];
    
    // Employee Workforce Metrics
    public int TotalEmployees { get; set; }
    public int TeachingStaffCount { get; set; }
    public int NonTeachingStaffCount { get; set; }
    public IReadOnlyList<ChartPoint> EmployeesByDepartment { get; set; } = [];

    // Academic Assignment Summary
    public int TotalClasses { get; set; }
    public int AssignedClasses { get; set; }
    public int UnassignedClasses => TotalClasses - AssignedClasses;
    
    public int TotalSubjects { get; set; }
    public int AssignedSubjects { get; set; }
    public int UnassignedSubjects => TotalSubjects - AssignedSubjects;
}

public record RecentActivityItem(string Module, string Title, DateTime At, string Summary = "");
