using SchoolManagementSystem.Models.ViewModels.Dashboard;

namespace SchoolManagementSystem.Repositories.Interfaces.Dashboard;

public interface IDashboardRepository
{
    Task<(int totalAttendance, int presentAttendance, decimal feesCollected, decimal feesTotal, List<ChartPoint> studentsByClass, List<ChartPoint> monthlyCollections, List<RecentActivityItem> recentActivities, int totalStudents, int pendingAdmissions)> GetAdminDashboardDataAsync(CancellationToken ct);
    Task<(int totalAttendance, int presentAttendance, decimal totalInvoiced, decimal totalPaid, List<RecentActivityItem> recentNotices, List<AssignmentDashboardItem> upcomingAssignments)> GetStudentDashboardDataAsync(int studentId, int classId, int sectionId, CancellationToken ct);
}
