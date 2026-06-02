using System;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Service.Interfaces.Dashboard;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Dashboard;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.DTOs.Attendance;

namespace SchoolManagementSystem.Service.Implementations.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _dashboardRepository;
    private readonly IDashboardQueryRepository _dashboardQueryRepository;
    private readonly IUnitOfWork _uow;

    public DashboardService(IDashboardRepository dashboardRepository, IDashboardQueryRepository dashboardQueryRepository, IUnitOfWork uow)
    {
        _dashboardRepository = dashboardRepository;
        _dashboardQueryRepository = dashboardQueryRepository;
        _uow = uow;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var data = await _dashboardRepository.GetAdminDashboardDataAsync(cancellationToken);

        var employeeRepo = _uow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>();
        var totalEmployees = await employeeRepo.CountAsync(e => !e.IsDeleted, cancellationToken);
        var teachingStaff = await employeeRepo.CountAsync(e => !e.IsDeleted && e.IsTeachingStaff, cancellationToken);
        var nonTeachingStaff = totalEmployees - teachingStaff;

        var employeesByDept = await employeeRepo.Query()
            .Where(e => !e.IsDeleted && e.Department != null)
            .GroupBy(e => e.Department!.Name)
            .Select(g => new ChartPoint(g.Key, g.Count()))
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

        // Attendance KPIs
        var todayDateOnly = DateOnly.FromDateTime(DateTime.UtcNow.Date);
        var studentAttendanceQ = _uow.Repository<SchoolManagementSystem.Models.Entities.Attendance.AttendanceRecord>().Query().Where(a => a.AttendanceDate == todayDateOnly && !a.IsDeleted);
        var studentPresentToday = await studentAttendanceQ.CountAsync(a => a.Status == AttendanceStatus.Present, cancellationToken);
        var studentAbsentToday = await studentAttendanceQ.CountAsync(a => a.Status == AttendanceStatus.Absent, cancellationToken);
        var studentLateToday = await studentAttendanceQ.CountAsync(a => a.Status == AttendanceStatus.Late, cancellationToken);
        var totalStudentMarked = await studentAttendanceQ.CountAsync(cancellationToken);
        var studentAttendancePctToday = totalStudentMarked == 0 ? 0m : Math.Round((decimal)studentPresentToday / totalStudentMarked * 100, 2);

        // Employee attendance for today (range based)
        var todayStart = DateTime.UtcNow.Date;
        var tomorrowStart = todayStart.AddDays(1);
        var employeeAttendanceQ = _uow.Repository<EmployeeAttendance>().Query().Where(a => a.AttendanceDate >= todayStart && a.AttendanceDate < tomorrowStart);
        var employeePresent = await employeeAttendanceQ.CountAsync(a => a.Status == AttendanceStatus.Present, cancellationToken);
        var employeeAbsent = await employeeAttendanceQ.CountAsync(a => a.Status == AttendanceStatus.Absent, cancellationToken);
        var employeeLate = await employeeAttendanceQ.CountAsync(a => a.Status == AttendanceStatus.Late, cancellationToken);

        // Alerts: classes missing attendance and locked sessions pending approval
        var classesWithSessionToday = await _uow.Repository<AttendanceSession>().Query().Where(s => s.AttendanceDate == todayDateOnly && !s.IsDeleted).Select(s => s.SchoolClassId).Distinct().CountAsync(cancellationToken);
        var classesMissing = Math.Max(0, totalClasses - classesWithSessionToday);

        var lockedPending = await _uow.Repository<AttendanceSession>().Query().Where(s => s.Status == AttendanceSessionStatus.Locked && !s.IsDeleted).CountAsync(cancellationToken);

        // Teachers not submitted approximation: count active teacher assignments minus distinct session creators for today
        var assignedTeacherCount = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherClassAssignment>().Query().Where(a => a.IsActive && !a.IsDeleted).Select(a => a.TeacherId).Distinct().CountAsync(cancellationToken);
        var creatorsToday = await _uow.Repository<AttendanceSession>().Query().Where(s => s.AttendanceDate == todayDateOnly && !s.IsDeleted).Select(s => s.CreatedBy).Distinct().CountAsync(cancellationToken);
        var teachersNotSubmitted = Math.Max(0, assignedTeacherCount - creatorsToday);

        // Daily attendance trend (last 7 days)
        var last7 = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(-6));
        var dailyTrendRaw = await _uow.Repository<SchoolManagementSystem.Models.Entities.Attendance.AttendanceRecord>().Query()
            .Where(a => a.AttendanceDate >= last7 && !a.IsDeleted)
            .GroupBy(a => a.AttendanceDate)
            .Select(g => new
            {
                Date = g.Key,
                Count = g.Count()
            })
            .OrderBy(x => x.Date)
            .ToListAsync(cancellationToken);

        var dailyTrend = dailyTrendRaw
            .Select(x => new ChartPoint(
                x.Date.ToString("yyyy-MM-dd"),
                (decimal)x.Count
            ))
            .ToList();

        // Monthly trend (last 6 months)
        var sixMonthsAgo = DateOnly.FromDateTime(DateTime.UtcNow.AddMonths(-5).Date);
        var monthlyTrendRaw = await _uow.Repository<SchoolManagementSystem.Models.Entities.Attendance.AttendanceRecord>().Query()
            .Where(a => a.AttendanceDate >= sixMonthsAgo && !a.IsDeleted)
            .GroupBy(a => new { a.AttendanceDate.Year, a.AttendanceDate.Month })
            .Select(g => new
            {
                Year = g.Key.Year,
                Month = g.Key.Month,
                Count = g.Count()
            })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync(cancellationToken);

        var monthlyTrend = monthlyTrendRaw
            .Select(x => new ChartPoint($"{x.Year}-{x.Month:D2}", (decimal)x.Count))
            .ToList();

        // Class-wise attendance percentage (recent period)
        var classGroups = await _uow.Repository<SchoolManagementSystem.Models.Entities.Attendance.AttendanceRecord>().Query()
            .Where(a => !a.IsDeleted)
            .GroupBy(a => a.SchoolClassId)
            .Select(g => new { ClassId = g.Key, Present = g.Count(x => x.Status == AttendanceStatus.Present), Total = g.Count() })
            .ToListAsync(cancellationToken);

        var classWisePoints = new List<ChartPoint>();
        foreach (var cg in classGroups)
        {
            var className = await _uow.Repository<SchoolManagementSystem.Models.Entities.Academic.SchoolClass>().Query().Where(c => c.Id == cg.ClassId).Select(c => c.Name).FirstOrDefaultAsync(cancellationToken) ?? cg.ClassId.ToString();
            var pct = cg.Total == 0 ? 0 : (int)((cg.Present * 100) / cg.Total);
            classWisePoints.Add(new ChartPoint(className, pct));
        }

        return new DashboardViewModel
        {
            TotalStudents = data.totalStudents,
            PendingAdmissions = data.pendingAdmissions,
            FeesCollected = data.feesCollected,
            FeesDue = data.feesTotal - data.feesCollected,
            AttendancePercentage = data.totalAttendance == 0 ? 0 : Math.Round((decimal)data.presentAttendance / data.totalAttendance * 100, 2),
            StudentsByClass = data.studentsByClass,
            MonthlyCollections = data.monthlyCollections,
            RecentActivities = data.recentActivities,
            TotalEmployees = totalEmployees,
            TeachingStaffCount = teachingStaff,
            NonTeachingStaffCount = nonTeachingStaff,
            EmployeesByDepartment = employeesByDept,
            TotalClasses = totalClasses,
            AssignedClasses = assignedClasses,
            TotalSubjects = totalSubjects,
            AssignedSubjects = assignedSubjects,
            StudentPresentToday = studentPresentToday,
            StudentAbsentToday = studentAbsentToday,
            StudentLateToday = studentLateToday,
            StudentAttendancePercentageToday = studentAttendancePctToday,
            EmployeePresentToday = employeePresent,
            EmployeeAbsentToday = employeeAbsent,
            EmployeeLateToday = employeeLate,
            ClassesMissingAttendance = classesMissing,
            LockedSessionsPendingApproval = lockedPending,
            TeachersNotSubmittedToday = teachersNotSubmitted,
            AttendanceDailyTrend = dailyTrend,
            AttendanceMonthlyTrend = monthlyTrend,
            ClassWiseAttendance = classWisePoints
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

    public async Task<GuardianDashboardViewModel> GetGuardianDashboardAsync(int userId, CancellationToken cancellationToken = default)
    {
        var user = await _uow.Repository<SchoolManagementSystem.Models.Entities.Auth.ApplicationUser>().GetByIdAsync(userId, cancellationToken)
            ?? throw new InvalidOperationException("User not found.");

        var guardian = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Guardian>().Query()
            .Include(g => g.Student)
                .ThenInclude(s => s!.Class)
            .Include(g => g.Student)
                .ThenInclude(s => s!.Section)
            .FirstOrDefaultAsync(g => g.Email == user.Email || g.Phone == user.PhoneNumber || g.Name == user.UserName, cancellationToken)
            ?? throw new InvalidOperationException("Guardian profile not found for this user.");

        if (guardian.Student == null)
            throw new InvalidOperationException("No student associated with this Guardian profile.");

        var studentId = guardian.StudentId;
        var student = guardian.Student;

        var attendanceRecords = await _uow.Repository<AttendanceRecord>().Query()
            .Where(a => a.StudentId == studentId && !a.IsDeleted)
            .OrderByDescending(a => a.AttendanceDate)
            .ToListAsync(cancellationToken);

        var presentCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Present);
        var absentCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Absent);
        var lateCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Late);
        var leaveCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Leave);
        var totalDays = attendanceRecords.Count;
        var attendancePct = totalDays == 0 ? 0 : Math.Round((double)(presentCount + lateCount) / totalDays * 100, 2);

        var historyDtos = attendanceRecords.Select(a => new StudentAttendanceDto
        {
            Id = a.Id,
            StudentId = studentId,
            StudentNo = student.StudentNo,
            StudentName = student.FullName,
            RollNumber = student.RollNumber.ToString(),
            ClassId = student.ClassId,
            ClassName = student.Class?.Name ?? "",
            SectionId = student.SectionId,
            SectionName = student.Section?.Name ?? "",
            AttendanceDate = a.AttendanceDate.ToDateTime(TimeOnly.MinValue),
            Status = a.Status,
            StatusName = a.Status.ToString(),
            Remarks = a.Remarks ?? ""
        }).ToList();

        var alerts = new List<string>();
        if (attendancePct < 85 && totalDays > 0)
        {
            alerts.Add($"Low attendance warning: {student.FullName}'s attendance is {attendancePct}%, which is below the required 85%.");
        }

        var recentAbsences = historyDtos.Where(h => h.Status == AttendanceStatus.Absent).Take(2).ToList();
        foreach (var absence in recentAbsences)
        {
            alerts.Add($"Absence Alert: Marked ABSENT on {absence.AttendanceDate:dd MMM yyyy}.");
        }

        var today = DateTime.Today;
        var monthlyRecords = attendanceRecords.Where(a => a.AttendanceDate.Year == today.Year && a.AttendanceDate.Month == today.Month).ToList();
        var mTotal = monthlyRecords.Count;
        var mPresent = monthlyRecords.Count(r => r.Status == AttendanceStatus.Present);
        var mAbsent = monthlyRecords.Count(r => r.Status == AttendanceStatus.Absent);
        var mLate = monthlyRecords.Count(r => r.Status == AttendanceStatus.Late);
        var mLeave = monthlyRecords.Count(r => r.Status == AttendanceStatus.Leave);
        var mPct = mTotal == 0 ? 0 : Math.Round((double)(mPresent + mLate) / mTotal * 100, 2);

        var monthlySummary = new StudentAttendanceMonthlySummaryDto
        {
            StudentId = studentId,
            StudentNo = student.StudentNo,
            StudentName = student.FullName,
            RollNumber = student.RollNumber.ToString(),
            Year = today.Year,
            Month = today.Month,
            TotalDays = mTotal,
            PresentCount = mPresent,
            AbsentCount = mAbsent,
            LateCount = mLate,
            LeaveCount = mLeave,
            AttendancePercentage = mPct
        };

        return new GuardianDashboardViewModel
        {
            GuardianId = guardian.Id,
            GuardianName = guardian.Name,
            StudentId = studentId,
            StudentName = student.FullName,
            StudentNo = student.StudentNo,
            ClassName = student.Class?.Name ?? "",
            SectionName = student.Section?.Name ?? "",
            RollNumber = student.RollNumber.ToString(),
            AttendancePercentage = attendancePct,
            PresentCount = presentCount,
            AbsentCount = absentCount,
            LateCount = lateCount,
            LeaveCount = leaveCount,
            AttendanceHistory = historyDtos.Take(10).ToList(),
            AbsentHistory = historyDtos.Where(h => h.Status == AttendanceStatus.Absent).ToList(),
            LateHistory = historyDtos.Where(h => h.Status == AttendanceStatus.Late).ToList(),
            Alerts = alerts,
            MonthlySummary = monthlySummary
        };
    }
}
