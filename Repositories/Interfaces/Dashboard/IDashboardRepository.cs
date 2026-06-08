using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Models.DTOs.Attendance;

namespace SchoolManagementSystem.Repositories.Interfaces.Dashboard;

public interface IDashboardRepository
{
    Task<(int totalAttendance, int presentAttendance, decimal feesCollected, decimal feesTotal, List<DashboardChartDto> studentsByClass, List<DashboardChartDto> monthlyCollections, List<DashboardActivityDto> recentActivities, int totalStudents, int pendingAdmissions)> GetAdminDashboardDataAsync(CancellationToken ct);
    Task<(int totalAttendance, int presentAttendance, decimal totalInvoiced, decimal totalPaid, List<DashboardActivityDto> recentNotices, List<DashboardAssignmentDto> upcomingAssignments)> GetStudentDashboardDataAsync(int studentId, int classId, int sectionId, CancellationToken ct);
    Task<List<DashboardCalendarDto>> GetStudentAttendanceCalendarAsync(int studentId, int year, int month, CancellationToken ct);
    Task<DashboardAttendanceSummaryDto> GetAttendanceDashboardSummaryAsync(DateTime date, CancellationToken ct);
    Task<(List<DashboardChartDto> Daily, List<DashboardChartDto> Monthly)> GetAttendanceAnalyticsAsync(CancellationToken ct);
    Task<List<DashboardChartDto>> GetClassAttendanceAnalyticsAsync(DateTime date, CancellationToken ct);
}
