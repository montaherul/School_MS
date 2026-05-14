namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TotalStudents { get; set; }
    public int PendingAdmissions { get; set; }
    public decimal FeesCollected { get; set; }
    public decimal FeesDue { get; set; }
    public decimal AttendancePercentage { get; set; }
    public SchoolManagementSystem.Models.DTOs.Employee.EmployeeAttendanceSummaryDto? EmployeeAttendance { get; set; }
    public int PendingLeaveRequests { get; set; }
    public int EmployeesOnLeaveToday { get; set; }
    public decimal TotalPayrollExpense { get; set; }
    public int PayrollPendingApproval { get; set; }
    public IReadOnlyList<ChartPoint> StudentsByClass { get; set; } = [];
    public IReadOnlyList<ChartPoint> MonthlyCollections { get; set; } = [];
    public IReadOnlyList<RecentActivityItem> RecentActivities { get; set; } = [];
    public decimal TotalCollections { get; set; }
}

public record RecentActivityItem(string Module, string Title, DateTime At, string Summary = "");
