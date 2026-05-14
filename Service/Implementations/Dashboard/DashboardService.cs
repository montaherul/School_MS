using System;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Dashboard;

namespace SchoolManagementSystem.Service.Implementations.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IDashboardQueryRepository _dashboardQueryRepository;
    private readonly IUnitOfWork _uow;
    private readonly SchoolManagementSystem.Services.Interfaces.Employee.IEmployeeAttendanceService _employeeAttendanceService;
    private readonly SchoolManagementSystem.Services.Interfaces.Employee.IEmployeeLeaveService _employeeLeaveService;
    private readonly SchoolManagementSystem.Services.Interfaces.Employee.IEmployeePayrollService _employeePayrollService;

    public DashboardService(
        IDashboardRepository dashboardRepository, 
        IDashboardQueryRepository dashboardQueryRepository, 
        IUnitOfWork uow,
        SchoolManagementSystem.Services.Interfaces.Employee.IEmployeeAttendanceService employeeAttendanceService,
        SchoolManagementSystem.Services.Interfaces.Employee.IEmployeeLeaveService employeeLeaveService,
        SchoolManagementSystem.Services.Interfaces.Employee.IEmployeePayrollService employeePayrollService)
    {
        _dashboardRepository = dashboardRepository;
        _dashboardQueryRepository = dashboardQueryRepository;
        _uow = uow;
        _employeeAttendanceService = employeeAttendanceService;
        _employeeLeaveService = employeeLeaveService;
        _employeePayrollService = employeePayrollService;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var data = await _dashboardRepository.GetAdminDashboardDataAsync(cancellationToken);

        return new DashboardViewModel
        {
            TotalStudents = data.totalStudents,
            PendingAdmissions = data.pendingAdmissions,
            FeesCollected = data.feesCollected,
            FeesDue = data.feesTotal - data.feesCollected,
            AttendancePercentage = data.totalAttendance == 0 ? 0 : Math.Round((decimal)data.presentAttendance / data.totalAttendance * 100, 2),
            EmployeeAttendance = await _employeeAttendanceService.GetDashboardSummaryAsync(DateTime.Today, cancellationToken),
            PendingLeaveRequests = (await _employeeLeaveService.GetPagedAsync(1, 1, null, null, null, LeaveStatus.Pending, cancellationToken)).TotalItems,
            EmployeesOnLeaveToday = (await _employeeLeaveService.GetPagedAsync(1, 100, null, null, null, LeaveStatus.Approved, cancellationToken)).Items
                                    .Count(l => l.StartDate.Date <= DateTime.Today.Date && l.EndDate.Date >= DateTime.Today.Date),
            TotalPayrollExpense = (await _employeePayrollService.GetDashboardSummaryAsync(DateTime.Today.Month, DateTime.Today.Year, cancellationToken)).TotalExpense,
            PayrollPendingApproval = (await _employeePayrollService.GetDashboardSummaryAsync(DateTime.Today.Month, DateTime.Today.Year, cancellationToken)).TotalPending,
            StudentsByClass = data.studentsByClass,
            MonthlyCollections = data.monthlyCollections,
            RecentActivities = data.recentActivities
        };
    }

    public async Task<StudentDashboardViewModel> GetStudentDashboardAsync(int userId, CancellationToken cancellationToken = default)
    {
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Student profile not found for this user.");

        var data = await _dashboardRepository.GetStudentDashboardDataAsync(student.Id, student.ClassId, student.SectionId, cancellationToken);

        return new StudentDashboardViewModel
        {
            Id = student.Id,
            FullName = student.FullName,
            StudentNo = student.StudentNo,
            ClassName = student.Class?.Name ?? "N/A",
            SectionName = student.Section?.Name ?? "N/A",
            RollNumber = student.RollNumber,
            AttendancePercentage = data.totalAttendance == 0 ? 0 : Math.Round((decimal)data.presentAttendance / data.totalAttendance * 100, 2),
            TotalDue = data.totalInvoiced - data.totalPaid,
            StudentStatus = student.Status.ToString(),
            RecentNotices = data.recentNotices,
            UpcomingAssignments = data.upcomingAssignments
        };
    }

    public async Task<TeacherDashboardViewModel> GetTeacherDashboardAsync(int userId, CancellationToken cancellationToken = default)
    {
        var teacher = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.Teacher>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Teacher profile not found.");

        var userRoles = await _uow.Repository<SchoolManagementSystem.Models.Entities.Auth.UserRole>().Query()
            .AsNoTracking()
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role != null ? ur.Role.Name : null)
            .Where(roleName => roleName != null)
            .ToListAsync(cancellationToken)!;

        var model = new TeacherDashboardViewModel
        {
            TeacherId = teacher.Id,
            FullName = teacher.FullName,
            Designation = teacher.Designation,
            TeacherNo = teacher.TeacherNo,
            IsPrincipal = userRoles.Contains("Principal") || userRoles.Contains("Assistant Head"),
            IsSeniorLecturer = userRoles.Contains("Senior Lecturer"),
            MyClassesCount = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherClassAssignment>().CountAsync(a => a.TeacherId == teacher.Id && !a.IsDeleted, cancellationToken),
            MySubjectsCount = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherSubjectAssignment>().CountAsync(a => a.TeacherId == teacher.Id && !a.IsDeleted, cancellationToken),
            AttendanceRate = 95.5m // Placeholder or real logic
        };

        // Common Data
        model.RecentNotices = await _uow.Repository<SchoolManagementSystem.Models.Entities.Communication.Notice>().Query()
            .Where(n => !n.IsDeleted && (n.AudienceRole == "All" || n.AudienceRole == "Teacher"))
            .OrderByDescending(n => n.PublishAt)
            .Take(5)
            .Select(n => new RecentActivityItem("Notice", n.Title, n.PublishAt, n.Body ?? ""))
            .ToListAsync(cancellationToken);

        // Principal Specific
        if (model.IsPrincipal)
        {
            model.PrincipalStats = new PrincipalStats
            {
                TotalStaff = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.Teacher>().CountAsync(t => !t.IsDeleted, cancellationToken),
                TotalStudents = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().CountAsync(s => !s.IsDeleted, cancellationToken),
                MonthlyRevenue = await _uow.Repository<SchoolManagementSystem.Models.Entities.Fees.Payment>().Query().Where(p => p.PaidAt.Month == DateTime.Today.Month).SumAsync(p => p.Amount, cancellationToken),
                ExpensePercentage = 45.2m // Placeholder
            };
        }

        return model;
    }
}
