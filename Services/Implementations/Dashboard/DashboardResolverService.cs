using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Services.Interfaces.Dashboard;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Constants;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;

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
    private readonly SchoolDbContext _db;

    public DashboardResolverService(
        IEmployeeService employeeService,
        IEmployeeLeaveService leaveService,
        IEmployeeAttendanceService attendanceService,
        IEmployeePayrollService payrollService,
        IEmployeeRepository employeeRepo,
        IStudentRepository studentRepo,
        IHolidayRepository holidayRepo,
        IClassRoutineService routineService,
        ITeacherAcademicService teacherAcademicService,
        SchoolDbContext db)
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
        _db = db;
    }

    public async Task<string> ResolveDashboardViewAsync(int userId)
    {
        var topRole = await GetTopPriorityRoleAsync(userId);
        if (topRole == null) return "EmployeeDashboard";

        return topRole.Name switch
        {
            Roles.SuperAdmin => "AdminDashboard",
            Roles.Admin => "AdminDashboard",
            Roles.Principal => "ExecutiveDashboard",
            Roles.HRManager => "HRDashboard",
            Roles.Accountant => "FinanceDashboard",
            Roles.Teacher => "TeacherDashboard",
            Roles.Librarian => "LibraryDashboard",
            Roles.Staff => "StaffDashboard",
            Roles.Student => "StudentDashboard",
            _ => "EmployeeDashboard"
        };
    }

    public async Task<string> GetDashboardViewNameAsync(string[] roles)
    {
        // Legacy support - using priority logic instead of raw array check
        if (roles.Contains(Roles.SuperAdmin) || roles.Contains(Roles.Admin)) return "AdminDashboard";
        if (roles.Contains(Roles.Principal)) return "ExecutiveDashboard";
        if (roles.Contains(Roles.HRManager)) return "HRDashboard";
        if (roles.Contains(Roles.Accountant)) return "FinanceDashboard";
        if (roles.Contains(Roles.Teacher)) return "TeacherDashboard";
        if (roles.Contains(Roles.Librarian)) return "LibraryDashboard";
        if (roles.Contains(Roles.Staff)) return "StaffDashboard";
        if (roles.Contains(Roles.Student)) return "StudentDashboard";
        return "EmployeeDashboard";
    }

    public async Task<object> GetDashboardModelAsync(long userId, string[] roles, CancellationToken ct = default)
    {
        // Use resolved top role for primary model, then potentially merge widgets
        var viewName = await GetDashboardViewNameAsync(roles);

        if (viewName == "AdminDashboard")
        {
            var model = new AdminDashboardViewModel();
            try { model.TotalStudents = await _studentRepo.Query().CountAsync(ct); } catch { }
            try { model.TotalEmployees = await _employeeRepo.Query().CountAsync(ct); } catch { }
            try { 
                var leaveResult = await _leaveService.GetPagedAsync(1, 1, null, null, null, LeaveStatus.Pending, ct);
                model.PendingLeaves = leaveResult?.TotalItems ?? 0;
            } catch { }
            
            model.AttendancePercentage = 95.5m; // Placeholder for now
            return model;
        }

        if (viewName == "FinanceDashboard")
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

            if (viewName == "TeacherDashboard" || roles.Contains(Roles.Teacher))
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

        return new object();
    }

    public async Task<IEnumerable<string>> GetAuthorizedWidgetsAsync(int userId)
    {
        var permissions = await GetMergedPermissionsAsync(userId);
        var widgets = new List<string>();

        if (permissions.Contains(Permissions.Payroll.View)) widgets.Add("PayrollSummary");
        if (permissions.Contains(Permissions.Employee.View)) widgets.Add("StaffStats");
        if (permissions.Contains(Permissions.Attendance.View)) widgets.Add("AttendanceChart");
        if (permissions.Contains(Permissions.Result.View)) widgets.Add("PerformanceOverview");
        if (permissions.Contains(Permissions.Admission.View)) widgets.Add("AdmissionFunnel");

        return widgets.Distinct();
    }

    public async Task<bool> HasPermissionAsync(int userId, string permissionCode)
    {
        var user = await _db.Set<ApplicationUser>()
            .Include(u => u.UserRoles).ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.Id == userId);

        if (user == null) return false;

        // Super Admin Override
        if (user.UserRoles.Any(ur => ur.Role?.Name == Roles.SuperAdmin)) return true;

        var permissions = await GetMergedPermissionsAsync(userId);
        return permissions.Contains(permissionCode);
    }

    private async Task<Role?> GetTopPriorityRoleAsync(int userId)
    {
        return await _db.Set<UserRole>()
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Where(ur => (!ur.EffectiveFrom.HasValue || ur.EffectiveFrom <= DateTime.UtcNow) && 
                         (!ur.EffectiveTo.HasValue || ur.EffectiveTo >= DateTime.UtcNow))
            .OrderByDescending(ur => ur.IsPrimary)
            .ThenByDescending(ur => ur.Role!.Priority)
            .Select(ur => ur.Role)
            .FirstOrDefaultAsync();
    }

    private async Task<HashSet<string>> GetMergedPermissionsAsync(int userId)
    {
        return (await _db.Set<UserRole>()
            .Where(ur => ur.UserId == userId)
            .SelectMany(ur => ur.Role!.RolePermissions)
            .Select(rp => rp.Permission!.Code)
            .Distinct()
            .ToListAsync()).ToHashSet();
    }
}
