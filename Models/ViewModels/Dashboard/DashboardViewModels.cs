using SchoolManagementSystem.Models.DTOs.Employee;

namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class AdminDashboardViewModel
{
    public int TotalStudents { get; set; }
    public int TotalTeachers { get; set; }
    public int TotalEmployees { get; set; }
    public decimal MonthlyRevenue { get; set; }
    public decimal MonthlyExpense { get; set; }
    public int PendingLeaves { get; set; }
    public decimal AttendancePercentage { get; set; }
    public List<ChartPoint> MonthlyRevenueTrend { get; set; } = new();
}

public class EmployeeDashboardViewModel
{
    public long EmployeeId { get; set; }
    public string FullName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;
    public bool PresentToday { get; set; }
    public string? CheckInTime { get; set; }
    public decimal AttendancePercentage { get; set; }
    public int RemainingLeaves { get; set; }
    public int PendingLeaveRequests { get; set; }
    public decimal LastSalaryAmount { get; set; }
    public string? LastSalaryStatus { get; set; }
    public List<HolidayDto> UpcomingHolidays { get; set; } = new();
}



public class FinanceDashboardViewModel
{
    public decimal TotalMonthlyRevenue { get; set; }
    public decimal TotalPayrollExpense { get; set; }
    public int PayrollPendingApproval { get; set; }
    public decimal FeeCollectionRate { get; set; }
    public List<ChartPoint> ExpenseVsRevenue { get; set; } = new();
}

public class HolidayDto
{
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public int Days { get; set; }
}

public class ScheduleItemDto
{
    public string ClassName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string TimeSlot { get; set; } = string.Empty;
}


