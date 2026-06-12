using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Dashboard;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Services.Interfaces.Guardian;

namespace SchoolManagementSystem.Service.Implementations.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IDashboardQueryRepository _dashboardQueryRepository;
    private readonly IUnitOfWork _uow;
    private readonly IGuardianService _guardianService;

    public DashboardService(
        IDashboardRepository dashboardRepository,
        IDashboardQueryRepository dashboardQueryRepository,
        IUnitOfWork uow,
        IGuardianService guardianService)
    {
        _dashboardRepository = dashboardRepository;
        _dashboardQueryRepository = dashboardQueryRepository;
        _uow = uow;
        _guardianService = guardianService;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var data = await _dashboardRepository.GetAdminDashboardDataAsync(cancellationToken);
        var attendanceSummary = await _dashboardRepository.GetAttendanceDashboardSummaryAsync(DateTime.Today, cancellationToken);

        var employeeRepo = _uow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>();
        var totalEmployees = await employeeRepo.CountAsync(e => !e.IsDeleted, cancellationToken);
        var teachingStaff = await employeeRepo.CountAsync(e => !e.IsDeleted && e.IsTeachingStaff, cancellationToken);
        var nonTeachingStaff = totalEmployees - teachingStaff;

        var employeesByDept = await employeeRepo.Query()
            .Where(e => !e.IsDeleted && e.Department != null)
            .GroupBy(e => e.Department!.Name)
            .Select(g => new DashboardChartDto { Label = g.Key, Value = g.Count() })
            .ToListAsync(cancellationToken);

        var totalClasses = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.SchoolClass>().CountAsync(c => !c.IsDeleted, cancellationToken);
        var assignedClasses = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherClassAssignment>().Query()
            .Where(a => a.IsActive && !a.IsDeleted)
            .Select(a => a.ClassId)
            .Distinct()
            .CountAsync(cancellationToken);

        var totalSubjects = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.Subject>().CountAsync(s => !s.IsDeleted, cancellationToken);
        var assignedSubjects = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherSubjectAssignment>().Query()
            .Where(a => a.IsActive && !a.IsDeleted)
            .Select(a => a.SubjectId)
            .Distinct()
            .CountAsync(cancellationToken);

        var (dailyTrend, monthlyTrend) = await GetAttendanceAnalyticsAsync(cancellationToken);
        var classWisePoints = await GetClassAttendanceAnalyticsAsync(DateTime.Today, cancellationToken);

        return new DashboardViewModel
        {
            TotalStudents = data.totalStudents,
            PendingAdmissions = data.pendingAdmissions,
            FeesCollected = data.feesCollected,
            FeesDue = data.feesTotal - data.feesCollected,
            AttendancePercentage = attendanceSummary.StudentAttendancePercentage,
            StudentsByClass = data.studentsByClass.Select(MapChart).ToList(),
            MonthlyCollections = data.monthlyCollections.Select(MapChart).ToList(),
            RecentActivities = data.recentActivities.Select(MapActivity).ToList(),
            TotalEmployees = totalEmployees,
            TeachingStaffCount = teachingStaff,
            NonTeachingStaffCount = nonTeachingStaff,
            EmployeesByDepartment = employeesByDept.Select(MapChart).ToList(),
            TotalClasses = totalClasses,
            AssignedClasses = assignedClasses,
            TotalSubjects = totalSubjects,
            AssignedSubjects = assignedSubjects,
            StudentPresentToday = attendanceSummary.StudentPresent,
            StudentAbsentToday = attendanceSummary.StudentAbsent,
            StudentLateToday = attendanceSummary.StudentLate,
            StudentAttendancePercentageToday = attendanceSummary.StudentAttendancePercentage,
            EmployeePresentToday = attendanceSummary.EmployeePresent,
            EmployeeAbsentToday = attendanceSummary.EmployeeAbsent,
            EmployeeLateToday = attendanceSummary.EmployeeLate,
            ClassesMissingAttendance = attendanceSummary.ClassesMissingAttendance,
            LockedSessionsPendingApproval = attendanceSummary.LockedSessions,
            TeachersNotSubmittedToday = attendanceSummary.ClassesMissingAttendance,
            AttendanceDailyTrend = dailyTrend,
            AttendanceMonthlyTrend = monthlyTrend,
            ClassWiseAttendance = classWisePoints
        };
    }

    private async Task<(List<ChartPoint> Daily, List<ChartPoint> Monthly)> GetAttendanceAnalyticsAsync(CancellationToken ct)
    {
        var (daily, monthly) = await _dashboardRepository.GetAttendanceAnalyticsAsync(ct);
        return (daily.Select(MapChart).ToList(), monthly.Select(MapChart).ToList());
    }

    private async Task<List<ChartPoint>> GetClassAttendanceAnalyticsAsync(DateTime date, CancellationToken ct)
    {
        var points = await _dashboardRepository.GetClassAttendanceAnalyticsAsync(date, ct);
        return points.Select(MapChart).ToList();
    }

    public async Task<StudentDashboardViewModel> GetStudentDashboardAsync(int userId, CancellationToken cancellationToken = default)
    {
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Student profile not found for this user.");

        var data = await _dashboardRepository.GetStudentDashboardDataAsync(student.Id, student.ClassId, student.SectionId, cancellationToken);
        var calendar = await _dashboardRepository.GetStudentAttendanceCalendarAsync(student.Id, DateTime.Today.Year, DateTime.Today.Month, cancellationToken);

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
            RecentNotices = data.recentNotices.Select(MapActivity).ToList(),
            UpcomingAssignments = data.upcomingAssignments.Select(MapAssignment).ToList(),
            AttendanceCalendar = calendar.Select(MapCalendar).ToList()
        };
    }

    public async Task<TeacherDashboardViewModel> GetTeacherDashboardAsync(int userId, CancellationToken cancellationToken = default)
    {
        var teacher = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.Teacher>().Query()
            .AsNoTracking()
            .Include(t => t.Employee)
            .FirstOrDefaultAsync(t => t.Employee.UserId == userId && !t.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Teacher profile not found.");

        var userRoles = await _uow.Repository<SchoolManagementSystem.Models.Entities.Auth.UserRole>().Query()
            .AsNoTracking()
            .Include(ur => ur.Role)
            .Where(ur => ur.UserId == userId)
            .Select(ur => ur.Role != null ? ur.Role.Name : null)
            .Where(roleName => roleName != null)
            .ToListAsync(cancellationToken)!;

        var classAssignments = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherClassAssignment>().Query()
            .AsNoTracking()
            .Include(a => a.Class)
            .Include(a => a.Section)
            .Where(a => a.TeacherId == teacher.Id && !a.IsDeleted)
            .ToListAsync(cancellationToken);

        var subjectAssignments = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherSubjectAssignment>().Query()
            .AsNoTracking()
            .Include(a => a.Subject)
            .Include(a => a.Class)
            .Include(a => a.Section)
            .Where(a => a.TeacherId == teacher.Id && !a.IsDeleted)
            .ToListAsync(cancellationToken);

        var employeeId = teacher.EmployeeId;
        var now = DateTime.Today;
        var monthStart = new DateTime(now.Year, now.Month, 1);
        var attendanceRecords = await _uow.Repository<SchoolManagementSystem.Models.Entities.Attendance.EmployeeAttendance>().Query()
            .Where(a => a.EmployeeId == employeeId && a.AttendanceDate >= monthStart && a.AttendanceDate <= now && !a.IsDeleted)
            .ToListAsync(cancellationToken);
        var totalDays = attendanceRecords.Count;
        var presentDays = attendanceRecords.Count(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late);
        var attendanceRate = totalDays > 0 ? Math.Round((decimal)presentDays / totalDays * 100, 2) : 0m;

        var model = new TeacherDashboardViewModel
        {
            TeacherId = teacher.Id,
            FullName = teacher.FullName,
            Designation = teacher.Designation,
            TeacherNo = teacher.TeacherNo,
            IsPrincipal = userRoles.Contains("Principal") || userRoles.Contains("Assistant Head"),
            IsSeniorLecturer = userRoles.Contains("Senior Lecturer"),
            MyClassesCount = classAssignments.Count,
            MySubjectsCount = subjectAssignments.Count,
            MyClasses = classAssignments.Select(a => $"{a.Class?.Name} {a.Section?.Name}").ToList(),
            MySubjects = subjectAssignments.Select(a => $"{a.Subject?.Name} ({a.Class?.Name}{a.Section?.Name})").ToList(),
            AttendanceRate = attendanceRate
        };

        var notices = await _uow.Repository<SchoolManagementSystem.Models.Entities.Communication.Notice>().Query()
            .Where(n => !n.IsDeleted && (n.AudienceRole == "All" || n.AudienceRole == "Teacher"))
            .OrderByDescending(n => n.PublishAt)
            .Take(5)
            .Select(n => new DashboardActivityDto { Module = "Notice", Title = n.Title, At = n.PublishAt, Summary = n.Body ?? "" })
            .ToListAsync(cancellationToken);
        model.RecentNotices = notices.Select(MapActivity).ToList();

        if (model.IsPrincipal)
        {
            model.PrincipalStats = new PrincipalStats
            {
                TotalStaff = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.Teacher>().CountAsync(t => !t.IsDeleted, cancellationToken),
                TotalStudents = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().CountAsync(s => !s.IsDeleted, cancellationToken),
                MonthlyRevenue = await _uow.Repository<SchoolManagementSystem.Models.Entities.Fees.Payment>().Query().Where(p => p.PaidAt.Month == DateTime.Today.Month).SumAsync(p => p.Amount, cancellationToken),
                ExpensePercentage = 45.2m
            };
        }

        return model;
    }

    public async Task<GuardianDashboardViewModel> GetGuardianDashboardAsync(int userId, CancellationToken cancellationToken = default)
    {
        var data = await _guardianService.GetDashboardByUserIdAsync(userId);

        var guardian = await _uow.Repository<SchoolManagementSystem.Models.Entities.Guardian.Guardian>().Query()
            .FirstOrDefaultAsync(g => g.UserId == userId, cancellationToken)
            ?? throw new InvalidOperationException("Guardian profile not found.");

        var model = new GuardianDashboardViewModel
        {
            GuardianId = guardian.Id,
            GuardianCode = guardian.GuardianCode,
            GuardianName = guardian.FullName,
            TotalOutstandingFees = data.TotalOutstandingFees,
            UnreadNotifications = data.UnreadNotifications,
            RecentNotices = data.RecentNotices,
            Children = data.ChildrenAttendance.Select(c => new GuardianChildSummaryViewModel
            {
                StudentId = c.StudentId,
                FullName = c.FullName,
                PresentCount = c.PresentCount,
                AbsentCount = c.AbsentCount,
                TotalDays = c.TotalDays,
                AttendancePercentage = c.AttendancePercentage
            }).ToList()
        };

        var studentGuardians = await _uow.Repository<SchoolManagementSystem.Models.Entities.Guardian.StudentGuardian>().Query()
            .Include(sg => sg.Student).ThenInclude(s => s.Class)
            .Include(sg => sg.Student).ThenInclude(s => s.Section)
            .Where(sg => sg.GuardianId == guardian.Id && !sg.IsDeleted)
            .ToListAsync(cancellationToken);

        if (studentGuardians.Any())
        {
            var selectedStudent = studentGuardians.FirstOrDefault(sg => sg.IsPrimaryGuardian)?.Student ?? studentGuardians.First().Student;
            if (selectedStudent != null)
            {
                model.StudentName = selectedStudent.FullName;
                model.ClassName = selectedStudent.Class?.Name ?? "N/A";
                model.SectionName = selectedStudent.Section?.Name ?? "N/A";
                model.RollNumber = selectedStudent.RollNumber.ToString();

                var today = DateTime.Today;
                var startOfMonth = new DateOnly(today.Year, today.Month, 1);
                var endOfMonth = startOfMonth.AddMonths(1).AddDays(-1);
                var attendanceRecords = await _uow.Repository<SchoolManagementSystem.Models.Entities.Attendance.AttendanceRecord>().Query()
                    .Where(a => a.StudentId == selectedStudent.Id && a.AttendanceDate >= startOfMonth && a.AttendanceDate <= endOfMonth && !a.IsDeleted)
                    .OrderByDescending(a => a.AttendanceDate)
                    .ToListAsync(cancellationToken);

                model.PresentCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Present);
                model.AbsentCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Absent);
                model.LateCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Late);
                model.LeaveCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Leave);
                var totalDays = attendanceRecords.Count;
                model.AttendancePercentage = totalDays > 0 ? Math.Round((double)(model.PresentCount + model.LateCount) / totalDays * 100, 2) : 100.0;

                model.AttendanceHistory = attendanceRecords.Take(10).Select(a => new StudentAttendanceDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    StudentNo = selectedStudent.StudentNo,
                    StudentName = selectedStudent.FullName,
                    RollNumber = selectedStudent.RollNumber.ToString(),
                    ClassId = a.SchoolClassId,
                    ClassName = selectedStudent.Class?.Name ?? "",
                    SectionId = a.SectionId,
                    SectionName = selectedStudent.Section?.Name ?? "",
                    AttendanceDate = a.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                    Status = a.Status,
                    StatusName = a.Status.ToString(),
                    Remarks = a.Remarks ?? ""
                }).ToList();

                model.AttendanceCalendar = attendanceRecords.Select(a => new AttendanceCalendarDto
                {
                    Date = a.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                    Status = a.Status.ToString(),
                    StatusColor = a.Status == AttendanceStatus.Present ? "#22c55e" : a.Status == AttendanceStatus.Absent ? "#ef4444" : a.Status == AttendanceStatus.Late ? "#eab308" : "#8b5cf6"
                }).ToList();

                model.Alerts = new List<string>();
                if (model.AttendancePercentage < 75.0)
                {
                    model.Alerts.Add($"Low Attendance Alert: {selectedStudent.FullName}'s attendance is {model.AttendancePercentage}%, which is below the minimum required 75%.");
                }
                var recentAbsent = attendanceRecords.FirstOrDefault(a => a.Status == AttendanceStatus.Absent);
                if (recentAbsent != null)
                {
                    model.Alerts.Add($"Absent Notification: {selectedStudent.FullName} was marked absent on {recentAbsent.AttendanceDate:dd MMM yyyy}.");
                }

                var invoices = await _uow.Repository<SchoolManagementSystem.Models.Entities.Fees.FeeInvoice>().Query()
                    .Where(fi => fi.StudentId == selectedStudent.Id && !fi.IsDeleted)
                    .ToListAsync(cancellationToken);
                model.SelectedChildOutstandingFees = invoices.Where(i => (int)i.Status != 3).Sum(i => i.TotalAmount - i.PaidAmount);
                model.SelectedChildTotalPaid = invoices.Sum(i => i.PaidAmount);
                model.SelectedChildInvoiceCount = invoices.Count;

                var latestResult = await _uow.Repository<SchoolManagementSystem.Models.Entities.Result.StudentExamResult>().Query()
                    .Where(r => r.StudentId == selectedStudent.Id && !r.IsDeleted)
                    .OrderByDescending(r => r.ExamId)
                    .FirstOrDefaultAsync(cancellationToken);
                if (latestResult != null)
                {
                    model.SelectedChildLatestGPA = latestResult.Gpa;
                    model.SelectedChildLatestGrade = latestResult.Grade ?? string.Empty;
                    model.SelectedChildLatestPassed = latestResult.IsPassed;
                }

                model.SelectedChildLeaveCount = await _uow.Repository<SchoolManagementSystem.Models.Entities.Attendance.StudentLeaveApplication>().Query()
                    .CountAsync(l => l.StudentId == selectedStudent.Id, cancellationToken);
                model.SelectedChildPendingLeaveCount = await _uow.Repository<SchoolManagementSystem.Models.Entities.Attendance.StudentLeaveApplication>().Query()
                    .CountAsync(l => l.StudentId == selectedStudent.Id && l.ApprovalStatus == SchoolManagementSystem.Models.Entities.Attendance.StudentLeaveApplication.ApprovalStatusEnum.Pending, cancellationToken);
            }
        }

        return model;
    }

    private static ChartPoint MapChart(DashboardChartDto dto) => new ChartPoint(dto.Label, dto.Value);

    private static RecentActivityItem MapActivity(DashboardActivityDto dto) => new RecentActivityItem(dto.Module, dto.Title, dto.At, dto.Summary);

    private static AssignmentDashboardItem MapAssignment(DashboardAssignmentDto dto) => new AssignmentDashboardItem(dto.Subject, dto.Title, dto.Deadline);

    private static AttendanceCalendarDto MapCalendar(DashboardCalendarDto dto) => new AttendanceCalendarDto
    {
        Date = dto.Date,
        Status = dto.Status,
        StatusColor = dto.Status.ToLower() switch
        {
            "present" => "success",
            "absent" => "danger",
            "late" => "warning",
            "leave" => "info",
            _ => "secondary"
        }
    };
}
