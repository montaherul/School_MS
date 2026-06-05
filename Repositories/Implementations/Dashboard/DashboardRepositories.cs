using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Repositories.Interfaces.Dashboard;
using SchoolManagementSystem.Models.DTOs.Attendance;
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

    public async Task<(int totalAttendance, int presentAttendance, decimal feesCollected, decimal feesTotal, List<ChartPoint> studentsByClass, List<ChartPoint> monthlyCollections, List<RecentActivityItem> recentActivities, int totalStudents, int pendingAdmissions)> GetAdminDashboardDataAsync(CancellationToken ct)
    {
        var totalStudents = await _db.Students.Where(s => !s.IsDeleted).CountAsync(ct);
        var pendingAdmissions = await _db.Admissions.Where(a => a.Status == AdmissionStatus.Pending).CountAsync(ct);
        var approvedAdmissions = await _db.Admissions.Where(a => a.Status == AdmissionStatus.Approved).CountAsync(ct);
        var rejectedAdmissions = await _db.Admissions.Where(a => a.Status == AdmissionStatus.Rejected).CountAsync(ct);
        var convertedAdmissions = await _db.Admissions.Where(a => a.Status == AdmissionStatus.Converted).CountAsync(ct);
        var totalAttendance = await _db.Attendance.Where(a => !a.IsDeleted).CountAsync(ct);
        var presentAttendance = await _db.Attendance.Where(a => (a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late) && !a.IsDeleted).CountAsync(ct);
        var feesCollected = await _db.FeeInvoices.Where(f => (int)f.Status == 1).SumAsync(f => f.PaidAmount, ct);
        var feesTotal = await _db.FeeInvoices.SumAsync(f => f.TotalAmount, ct);

        // Get students count by class
        var studentsByClass = await _db.Students
            .Where(s => !s.IsDeleted)
            .GroupBy(s => s.ClassId)
            .Select(g => new ChartPoint(g.Key.ToString(), g.Count()))
            .ToListAsync(ct);

        // Get monthly fee collections
        var monthlyCollections = await _db.FeeInvoices
            .Where(f => (int)f.Status == 1 && f.UpdatedAt.HasValue)
            .GroupBy(f => f.UpdatedAt.Value.Month)
            .Select(g => new ChartPoint(g.Key.ToString(), (int)g.Sum(f => f.PaidAmount)))
            .ToListAsync(ct);

        var recentActivities = new List<RecentActivityItem>();

        return (totalAttendance, presentAttendance, feesCollected, feesTotal, studentsByClass, monthlyCollections, recentActivities, totalStudents, pendingAdmissions);
    }

    public async Task<(int totalAttendance, int presentAttendance, decimal totalInvoiced, decimal totalPaid, List<RecentActivityItem> recentNotices, List<AssignmentDashboardItem> upcomingAssignments)> GetStudentDashboardDataAsync(int studentId, int classId, int sectionId, CancellationToken ct)
    {
        var totalAttendance = await _db.Attendance.Where(a => a.StudentId == studentId && !a.IsDeleted).CountAsync(ct);
        var presentAttendance = await _db.Attendance.Where(a => a.StudentId == studentId && (a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late) && !a.IsDeleted).CountAsync(ct);
        var totalInvoiced = await _db.FeeInvoices.Where(f => f.StudentId == studentId).SumAsync(f => f.TotalAmount, ct);
        var totalPaid = await _db.FeeInvoices.Where(f => f.StudentId == studentId && (int)f.Status == 1).SumAsync(f => f.PaidAmount, ct);

        var recentNotices = new List<RecentActivityItem>();
        var upcomingAssignments = new List<AssignmentDashboardItem>();

        return (totalAttendance, presentAttendance, totalInvoiced, totalPaid, recentNotices, upcomingAssignments);
    }

    public async Task<List<AttendanceCalendarDto>> GetStudentAttendanceCalendarAsync(int studentId, int year, int month, CancellationToken ct)
    {
        var records = await _db.Attendance
            .AsNoTracking()
            .Where(a => a.StudentId == studentId
                && !a.IsDeleted
                && a.AttendanceDate.Year == year
                && a.AttendanceDate.Month == month)
            .OrderBy(a => a.AttendanceDate)
            .Select(a => new AttendanceCalendarDto
            {
                Date = a.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                Status = a.Status.ToString(),
                StatusColor = GetStatusColor(a.Status.ToString())
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

    private string GetStatusColor(string status)
    {
        return status.ToLower() switch
        {
            "present" => "success",
            "absent" => "danger",
            "late" => "warning",
            "leave" => "info",
            _ => "secondary"
        };
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

    public Task<(int totalAttendance, int presentAttendance, decimal feesCollected, decimal feesTotal, List<ChartPoint> studentsByClass, List<ChartPoint> monthlyCollections, List<RecentActivityItem> recentActivities, int totalStudents, int pendingAdmissions)> GetAdminDashboardDataAsync(CancellationToken ct) => _repo.GetAdminDashboardDataAsync(ct);
    
    public Task<(int totalAttendance, int presentAttendance, decimal totalInvoiced, decimal totalPaid, List<RecentActivityItem> recentNotices, List<AssignmentDashboardItem> upcomingAssignments)> GetStudentDashboardDataAsync(int studentId, int classId, int sectionId, CancellationToken ct) => _repo.GetStudentDashboardDataAsync(studentId, classId, sectionId, ct);

    public Task<List<AttendanceCalendarDto>> GetStudentAttendanceCalendarAsync(int studentId, int year, int month, CancellationToken ct) => _repo.GetStudentAttendanceCalendarAsync(studentId, year, month,ct);

    public Task<DashboardAttendanceSummaryDto> GetAttendanceDashboardSummaryAsync(DateTime date, CancellationToken ct) => _repo.GetAttendanceDashboardSummaryAsync(date, ct);
}
