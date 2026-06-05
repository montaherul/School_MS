using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Models.DTOs.Attendance;

namespace SchoolManagementSystem.Repositories.Interfaces.Dashboard;

public interface IDashboardQueryRepository
{
    Task<(int totalAttendance, int presentAttendance, decimal feesCollected, decimal feesTotal, List<ChartPoint> studentsByClass, List<ChartPoint> monthlyCollections, List<RecentActivityItem> recentActivities, int totalStudents, int pendingAdmissions)> GetAdminDashboardDataAsync(CancellationToken ct);
    Task<(int totalAttendance, int presentAttendance, decimal totalInvoiced, decimal totalPaid, List<RecentActivityItem> recentNotices, List<AssignmentDashboardItem> upcomingAssignments)> GetStudentDashboardDataAsync(int studentId, int classId, int sectionId, CancellationToken ct);
    Task<List<AttendanceCalendarDto>> GetStudentAttendanceCalendarAsync(int studentId, int year, int month, CancellationToken ct);
    Task<DashboardAttendanceSummaryDto> GetAttendanceDashboardSummaryAsync(DateTime date, CancellationToken ct);
}
