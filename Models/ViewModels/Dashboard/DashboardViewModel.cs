namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class DashboardViewModel
{
    public int TotalStudents { get; set; }
    public int PendingAdmissions { get; set; }
    public decimal FeesCollected { get; set; }
    public decimal FeesDue { get; set; }
    public decimal AttendancePercentage { get; set; }
    public IReadOnlyList<ChartPoint> StudentsByClass { get; set; } = [];
    public IReadOnlyList<ChartPoint> MonthlyCollections { get; set; } = [];
    public IReadOnlyList<RecentActivityItem> RecentActivities { get; set; } = [];
}

public record ChartPoint(string Label, decimal Value);
public record RecentActivityItem(string Module, string Title, DateTime At);
