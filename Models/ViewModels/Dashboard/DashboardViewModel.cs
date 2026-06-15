using SchoolManagementSystem.Models.DTOs.Calendar;

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
    // Attendance KPIs - Students
    public int StudentPresentToday { get; set; }
    public int StudentAbsentToday { get; set; }
    public int StudentLateToday { get; set; }
    public decimal StudentAttendancePercentageToday { get; set; }

    // Attendance KPIs - Employees
    public int EmployeePresentToday { get; set; }
    public int EmployeeAbsentToday { get; set; }
    public int EmployeeLateToday { get; set; }

    // Alerts
    public int ClassesMissingAttendance { get; set; }
    public int LockedSessionsPendingApproval { get; set; }
    public int TeachersNotSubmittedToday { get; set; }

    // Attendance charts
    public IReadOnlyList<ChartPoint> AttendanceDailyTrend { get; set; } = [];
    public IReadOnlyList<ChartPoint> AttendanceMonthlyTrend { get; set; } = [];
    public IReadOnlyList<ChartPoint> ClassWiseAttendance { get; set; } = [];
    
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

    // ID Card Stats
    public int TotalStudentsWithCards { get; set; }
    public int ActiveStudentsWithCards { get; set; }
    public int TotalEmployeesWithCards { get; set; }
    public int ActiveEmployeesWithCards { get; set; }

    // Calendar Widgets
    public CalendarWidgetDto CalendarWidgets { get; set; } = new();
}

public record RecentActivityItem(string Module, string Title, DateTime At, string Summary = "");
