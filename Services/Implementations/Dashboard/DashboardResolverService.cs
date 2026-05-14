using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Services.Interfaces.Dashboard;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Services.Implementations.Dashboard;

public class DashboardResolverService : IDashboardResolverService
{
    private readonly IEmployeeService _employeeService;
    private readonly IEmployeeLeaveService _leaveService;
    private readonly IEmployeeAttendanceService _attendanceService;
    private readonly IEmployeePayrollService _payrollService;
    private readonly IEmployeeRepository _employeeRepo;
    private readonly IStudentRepository _studentRepo; 
    private readonly IHolidayRepository _holidayRepo;
    private readonly IClassRoutineService _routineService;
    private readonly ITeacherAcademicService _teacherAcademicService;

    public DashboardResolverService(
        IEmployeeService employeeService,
        IEmployeeLeaveService leaveService,
        IEmployeeAttendanceService attendanceService,
        IEmployeePayrollService payrollService,
        IEmployeeRepository employeeRepo,
        IStudentRepository studentRepo,
        IHolidayRepository holidayRepo,
        IClassRoutineService routineService,
        ITeacherAcademicService teacherAcademicService)
    {
        _employeeService = employeeService;
        _leaveService = leaveService;
        _attendanceService = attendanceService;
        _payrollService = payrollService;
        _employeeRepo = employeeRepo;
        _studentRepo = studentRepo;
        _holidayRepo = holidayRepo;
        _routineService = routineService;
        _teacherAcademicService = teacherAcademicService;
    }

    public async Task<string> GetDashboardViewNameAsync(string[] roles)
    {
        if (roles.Contains("Admin") || roles.Contains("SuperAdmin")) return "AdminDashboard";
        if (roles.Contains("Principal")) return "ExecutiveDashboard";
        if (roles.Contains("Accountant") || roles.Contains("Finance")) return "FinanceDashboard";
        if (roles.Contains("Teacher")) return "TeacherDashboard";
        return "EmployeeDashboard";
    }

    public async Task<object> GetDashboardModelAsync(long userId, string[] roles, CancellationToken ct = default)
    {
        if (roles.Contains("Admin") || roles.Contains("SuperAdmin"))
        {
            return new AdminDashboardViewModel
            {
                TotalStudents = await _studentRepo.Query().CountAsync(ct),
                TotalEmployees = await _employeeRepo.Query().CountAsync(ct),
                PendingLeaves = (await _leaveService.GetPagedAsync(1, 1, null, null, null, LeaveStatus.Pending, ct)).TotalItems,
                AttendancePercentage = 95.5m // Placeholder
            };
        }

        if (roles.Contains("Accountant") || roles.Contains("Finance"))
        {
            var summary = await _payrollService.GetDashboardSummaryAsync(DateTime.Today.Month, DateTime.Today.Year, ct);
            return new FinanceDashboardViewModel
            {
                TotalPayrollExpense = summary.TotalExpense,
                PayrollPendingApproval = summary.TotalPending
            };
        }

        // Default to Employee Dashboard for any staff member
        var employeeId = await _employeeService.GetEmployeeIdByUserIdAsync(userId);
        if (employeeId.HasValue)
        {
            var emp = await _employeeRepo.FirstOrDefaultAsync(e => e.Id == employeeId.Value, ct);
            var leaveSummary = await _leaveService.GetEmployeeLeaveSummaryAsync(employeeId.Value, DateTime.Today.Year);
            var attendSummary = await _attendanceService.GetEmployeeSummaryAsync(employeeId.Value);

            var upcomingHolidays = await _holidayRepo.GetUpcomingHolidaysAsync(5, ct);
            var lastPayroll = await _payrollService.GetRecentByEmployeeIdAsync(employeeId.Value, 1, ct);

            var model = new EmployeeDashboardViewModel
            {
                EmployeeId = employeeId.Value,
                FullName = emp?.FullName ?? "Staff Member",
                Designation = emp?.Designation?.Name ?? "Employee",
                AttendancePercentage = (decimal)attendSummary.AttendancePercentage,
                RemainingLeaves = leaveSummary.RemainingBalance,
                PendingLeaveRequests = leaveSummary.PendingRequests,
                UpcomingHolidays = upcomingHolidays.Select(h => new HolidayDto {
                    Name = h.Name,
                    StartDate = h.StartDate,
                    Days = (h.EndDate - h.StartDate).Days + 1
                }).ToList(),
                LastSalaryAmount = lastPayroll.FirstOrDefault()?.NetSalary ?? 0,
                LastSalaryStatus = lastPayroll.FirstOrDefault()?.PaymentStatus.ToString() ?? "N/A"
            };

            if (roles.Contains("Teacher"))
            {
                var workload = await _teacherAcademicService.GetWorkloadAsync(employeeId.Value, ct);
                var schedule = await _routineService.GetByTeacherAsync(employeeId.Value, ct);
                var todaySchedule = schedule.Where(s => s.DayOfWeek == DateTime.Today.DayOfWeek).ToList();

                return new TeacherDashboardViewModel
                {
                    EmployeeId = model.EmployeeId,
                    FullName = model.FullName,
                    Designation = model.Designation,
                    AttendancePercentage = model.AttendancePercentage,
                    RemainingLeaves = model.RemainingLeaves,
                    PendingLeaveRequests = model.PendingLeaveRequests,
                    UpcomingHolidays = model.UpcomingHolidays,
                    LastSalaryAmount = model.LastSalaryAmount,
                    LastSalaryStatus = model.LastSalaryStatus,
                    
                    TodayClasses = todaySchedule.Count,
                    WeeklyPeriods = workload.WeeklyPeriods,
                    PendingMarks = workload.PendingMarkEntries,
                    TodaysSchedule = todaySchedule.Select(s => new ScheduleItemDto {
                        ClassName = s.ClassName + " (" + s.SectionName + ")",
                        SubjectName = s.SubjectName,
                        TimeSlot = s.StartTime.ToString(@"hh\:mm") + " - " + s.EndTime.ToString(@"hh\:mm")
                    }).ToList()
                };
            }

            return model;
        }

        return new object(); // Should not happen for auth users
    }
}
