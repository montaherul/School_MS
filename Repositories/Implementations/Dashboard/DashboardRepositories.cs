using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Dashboard;
using System.Data;
using Microsoft.Data.SqlClient;

namespace SchoolManagementSystem.Repositories.Implementations.Dashboard;

public class DashboardRepository : IDashboardRepository
{
    private readonly SchoolDbContext _db;

    public DashboardRepository(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<(int totalAttendance, int presentAttendance, decimal feesCollected, decimal feesTotal, List<DashboardChartDto> studentsByClass, List<DashboardChartDto> monthlyCollections, List<DashboardActivityDto> recentActivities, int totalStudents, int pendingAdmissions)> GetAdminDashboardDataAsync(CancellationToken ct, int? academicYearId = null)
    {
        DateTime? yearStart = null;
        DateTime? yearEnd = null;

        if (academicYearId.HasValue)
        {
            var year = await _db.AcademicYears
                .Where(y => y.Id == academicYearId.Value && !y.IsDeleted)
                .Select(y => new { y.StartsOn, y.EndsOn })
                .FirstOrDefaultAsync(ct);
            if (year != null)
            {
                yearStart = year.StartsOn;
                yearEnd = year.EndsOn;
            }
        }

        var studentQuery = _db.Students.Where(s => !s.IsDeleted);
        var totalStudents = await studentQuery.CountAsync(ct);

        var admissionQuery = _db.Admissions.Where(a => a.Status == AdmissionStatus.Pending && !a.IsDeleted);
        if (yearStart.HasValue && yearEnd.HasValue)
            admissionQuery = admissionQuery.Where(a => a.CreatedAt >= yearStart.Value && a.CreatedAt <= yearEnd.Value);
        var pendingAdmissions = await admissionQuery.CountAsync(ct);

        var attendanceQuery = _db.Attendance.Where(a => !a.IsDeleted);
        if (yearStart.HasValue && yearEnd.HasValue)
            attendanceQuery = attendanceQuery.Where(a => a.CreatedAt >= yearStart.Value && a.CreatedAt <= yearEnd.Value);
        var totalAttendance = await attendanceQuery.CountAsync(ct);
        var presentAttendance = await attendanceQuery.Where(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late).CountAsync(ct);

        var feeQuery = _db.FeeInvoices.Where(f => !f.IsDeleted);
        if (academicYearId.HasValue)
            feeQuery = feeQuery.Where(f => f.AcademicYearId == academicYearId.Value);
        var feesCollected = await feeQuery.Where(f => f.Status == PaymentStatus.Paid).SumAsync(f => f.PaidAmount, ct);
        var feesTotal = await feeQuery.SumAsync(f => f.TotalAmount, ct);

        var studentsByClassQuery = _db.Students.Where(s => !s.IsDeleted);
        if (yearStart.HasValue && yearEnd.HasValue)
            studentsByClassQuery = studentsByClassQuery.Where(s => s.CreatedAt >= yearStart.Value && s.CreatedAt <= yearEnd.Value);
        var studentsByClass = await studentsByClassQuery
            .GroupBy(s => s.ClassId)
            .Select(g => new DashboardChartDto { Label = g.Key.ToString(), Value = g.Count() })
            .ToListAsync(ct);

        var monthlyFeeQuery = _db.FeeInvoices.Where(f => !f.IsDeleted && f.Status == PaymentStatus.Paid && f.UpdatedAt.HasValue);
        if (academicYearId.HasValue)
            monthlyFeeQuery = monthlyFeeQuery.Where(f => f.AcademicYearId == academicYearId.Value);
        var monthlyCollections = await monthlyFeeQuery
            .GroupBy(f => f.UpdatedAt.Value.Month)
            .Select(g => new DashboardChartDto { Label = g.Key.ToString(), Value = (int)g.Sum(f => f.PaidAmount) })
            .ToListAsync(ct);

        var activitiesQuery = _db.ActivityLogs.Where(l => !l.IsDeleted);
        if (yearStart.HasValue && yearEnd.HasValue)
            activitiesQuery = activitiesQuery.Where(l => l.CreatedAt >= yearStart.Value && l.CreatedAt <= yearEnd.Value);
        var recentActivities = await activitiesQuery
            .OrderByDescending(l => l.CreatedAt)
            .Take(10)
            .Select(l => new DashboardActivityDto
            {
                Title = l.Action ?? "",
                Module = l.Module ?? "",
                Summary = l.OldValues ?? "",
                At = l.CreatedAt
            })
            .ToListAsync(ct);

        return (totalAttendance, presentAttendance, feesCollected, feesTotal, studentsByClass, monthlyCollections, recentActivities, totalStudents, pendingAdmissions);
    }

    public async Task<(int totalAttendance, int presentAttendance, decimal totalInvoiced, decimal totalPaid, List<DashboardActivityDto> recentNotices, List<DashboardAssignmentDto> upcomingAssignments)> GetStudentDashboardDataAsync(int studentId, int classId, int sectionId, CancellationToken ct)
    {
        var totalAttendance = await _db.Attendance.Where(a => a.StudentId == studentId && !a.IsDeleted).CountAsync(ct);
        var presentAttendance = await _db.Attendance.Where(a => a.StudentId == studentId && (a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late) && !a.IsDeleted).CountAsync(ct);
        var totalInvoiced = await _db.FeeInvoices.Where(f => !f.IsDeleted && f.StudentId == studentId).SumAsync(f => f.TotalAmount, ct);
        var totalPaid = await _db.FeeInvoices.Where(f => !f.IsDeleted && f.StudentId == studentId && f.Status == PaymentStatus.Paid).SumAsync(f => f.PaidAmount, ct);

        var recentNotices = await _db.Notices
            .Where(n => !n.IsDeleted && n.IsPublished)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .Select(n => new DashboardActivityDto
            {
                Title = n.Title ?? "",
                Module = "Notice",
                Summary = n.Body ?? "",
                At = n.CreatedAt
            })
            .ToListAsync(ct);

        var upcomingAssignments = await _db.Assignments
            .Where(a => !a.IsDeleted && a.Deadline > DateTime.UtcNow)
            .OrderBy(a => a.Deadline)
            .Take(5)
            .Select(a => new DashboardAssignmentDto
            {
                Title = a.Title ?? "",
                Subject = "",
                Deadline = a.Deadline
            })
            .ToListAsync(ct);

        return (totalAttendance, presentAttendance, totalInvoiced, totalPaid, recentNotices, upcomingAssignments);
    }

    public async Task<List<DashboardCalendarDto>> GetStudentAttendanceCalendarAsync(int studentId, int year, int month, CancellationToken ct)
    {
        var records = await _db.Attendance
            .AsNoTracking()
            .Where(a => a.StudentId == studentId
                && !a.IsDeleted
                && a.AttendanceDate.Year == year
                && a.AttendanceDate.Month == month)
            .OrderBy(a => a.AttendanceDate)
            .Select(a => new DashboardCalendarDto
            {
                Date = a.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                Status = a.Status.ToString()
            })
            .ToListAsync(ct);

        return records;
    }

    public async Task<DashboardAttendanceSummaryDto> GetAttendanceDashboardSummaryAsync(DateTime date, CancellationToken ct)
    {
        var summary = new DashboardAttendanceSummaryDto();
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetAttendanceDashboardSummary";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@Date", date));

        if (command.Connection.State != ConnectionState.Open) await command.Connection.OpenAsync(ct);
        using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            summary.TotalStudents = reader.GetInt32(reader.GetOrdinal("TotalStudents"));
            summary.StudentPresent = reader.GetInt32(reader.GetOrdinal("StudentPresent"));
            summary.StudentAbsent = reader.GetInt32(reader.GetOrdinal("StudentAbsent"));
            summary.StudentLate = reader.GetInt32(reader.GetOrdinal("StudentLate"));
            summary.StudentLeave = reader.GetInt32(reader.GetOrdinal("StudentLeave"));
            summary.StudentAttendancePercentage = reader.GetDecimal(reader.GetOrdinal("StudentAttendancePercentage"));
            summary.TotalEmployees = reader.GetInt32(reader.GetOrdinal("TotalEmployees"));
            summary.EmployeePresent = reader.GetInt32(reader.GetOrdinal("EmployeePresent"));
            summary.EmployeeAbsent = reader.GetInt32(reader.GetOrdinal("EmployeeAbsent"));
            summary.EmployeeLate = reader.GetInt32(reader.GetOrdinal("EmployeeLate"));
            summary.EmployeeLeave = reader.GetInt32(reader.GetOrdinal("EmployeeLeave"));
            summary.ClassesMissingAttendance = reader.GetInt32(reader.GetOrdinal("ClassesMissingAttendance"));
            summary.LockedSessions = reader.GetInt32(reader.GetOrdinal("LockedSessions"));
        }
        return summary;
    }

    public async Task<(List<DashboardChartDto> Daily, List<DashboardChartDto> Monthly)> GetAttendanceAnalyticsAsync(CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetAttendanceAnalytics";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@StartDate", DateTime.Today.AddDays(-6)));
        command.Parameters.Add(new SqlParameter("@EndDate", DateTime.Today));

        if (command.Connection!.State != ConnectionState.Open)
            await command.Connection.OpenAsync(ct);

        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            var daily = new List<DashboardChartDto>();
            while (await reader.ReadAsync(ct))
            {
                daily.Add(new DashboardChartDto { Label = reader.GetDateTime(0).ToString("yyyy-MM-dd"), Value = reader.GetDecimal(3) });
            }

            var monthly = new List<DashboardChartDto>();
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    monthly.Add(new DashboardChartDto { Label = $"{reader.GetInt32(0)}-{reader.GetInt32(1):D2}", Value = reader.GetDecimal(4) });
                }
            }

            return (daily, monthly);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<List<DashboardChartDto>> GetClassAttendanceAnalyticsAsync(DateTime date, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetClassAttendanceAnalytics";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@Date", date.Date));

        if (command.Connection!.State != ConnectionState.Open)
            await command.Connection.OpenAsync(ct);

        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            var points = new List<DashboardChartDto>();
            while (await reader.ReadAsync(ct))
            {
                points.Add(new DashboardChartDto { Label = reader.GetString(1), Value = reader.GetDecimal(4) });
            }

            return points;
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<List<StudentResultViewModel>> GetStudentLatestResultsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _db.StudentExamResults
            .AsNoTracking()
            .Include(r => r.Exam)
            .Where(r => r.StudentId == studentId && !r.IsDeleted
                && (r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked))
            .OrderByDescending(r => r.CalculatedAt)
            .Take(5)
            .Select(r => new StudentResultViewModel
            {
                SubjectName = "Overall",
                ExamName = r.Exam.Name,
                ObtainedMarks = r.TotalMarks,
                FullMarks = r.TotalFullMarks,
                Grade = r.Grade,
                GPA = r.Gpa,
                IsPassed = r.IsPassed
            })
            .ToListAsync(cancellationToken);
    }
}

public class DashboardQueryRepository : IDashboardQueryRepository
{
    private readonly SchoolDbContext _db;
    private readonly IDashboardRepository _repo;

    public DashboardQueryRepository(SchoolDbContext db, IDashboardRepository repo)
    {
        _db = db;
        _repo = repo;
    }

    public Task<(int totalAttendance, int presentAttendance, decimal feesCollected, decimal feesTotal, List<DashboardChartDto> studentsByClass, List<DashboardChartDto> monthlyCollections, List<DashboardActivityDto> recentActivities, int totalStudents, int pendingAdmissions)> GetAdminDashboardDataAsync(CancellationToken ct, int? academicYearId = null) => _repo.GetAdminDashboardDataAsync(ct, academicYearId);

    public Task<(int totalAttendance, int presentAttendance, decimal totalInvoiced, decimal totalPaid, List<DashboardActivityDto> recentNotices, List<DashboardAssignmentDto> upcomingAssignments)> GetStudentDashboardDataAsync(int studentId, int classId, int sectionId, CancellationToken ct) => _repo.GetStudentDashboardDataAsync(studentId, classId, sectionId, ct);

    public Task<List<DashboardCalendarDto>> GetStudentAttendanceCalendarAsync(int studentId, int year, int month, CancellationToken ct) => _repo.GetStudentAttendanceCalendarAsync(studentId, year, month, ct);

    public Task<DashboardAttendanceSummaryDto> GetAttendanceDashboardSummaryAsync(DateTime date, CancellationToken ct) => _repo.GetAttendanceDashboardSummaryAsync(date, ct);

    public Task<(List<DashboardChartDto> Daily, List<DashboardChartDto> Monthly)> GetAttendanceAnalyticsAsync(CancellationToken ct) => _repo.GetAttendanceAnalyticsAsync(ct);

    public Task<List<DashboardChartDto>> GetClassAttendanceAnalyticsAsync(DateTime date, CancellationToken ct) => _repo.GetClassAttendanceAnalyticsAsync(date, ct);

    public Task<List<StudentResultViewModel>> GetStudentLatestResultsAsync(int studentId, CancellationToken cancellationToken = default) => _repo.GetStudentLatestResultsAsync(studentId, cancellationToken);
}
