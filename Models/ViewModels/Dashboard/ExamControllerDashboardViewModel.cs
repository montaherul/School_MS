using SchoolManagementSystem.Models.DTOs.Calendar;

namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class ExamControllerDashboardViewModel
{
    public string FullName { get; set; } = string.Empty;

    // 10 Stat Cards
    public int TotalExams { get; set; }
    public int DraftExams { get; set; }
    public int PublishedExams { get; set; }
    public int ActiveExamSchedules { get; set; }
    public int PendingMarksEntry { get; set; }
    public int ApprovedMarks { get; set; }
    public int PendingResultApproval { get; set; }
    public int PublishedResults { get; set; }
    public int StudentsAppearing { get; set; }
    public int TeachersAssigned { get; set; }

    // Charts
    public List<ChartPoint> ExamStatusDistribution { get; set; } = [];
    public List<ChartPoint> MarksEntryProgress { get; set; } = [];
    public List<ChartPoint> ResultPublicationProgress { get; set; } = [];

    // Widgets
    public List<UpcomingExamDto> UpcomingExams { get; set; } = [];
    public List<RecentActivityItem> RecentActivities { get; set; } = [];
}
