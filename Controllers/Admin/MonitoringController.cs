using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Controllers.Admin;

[Authorize(Roles = "Admin,Super Admin,Principal")]
public class MonitoringController : Controller
{
    private readonly SchoolDbContext _db;

    public MonitoringController(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var metrics = await _db.Database
            .SqlQueryRaw<DashboardMetricsResult>("EXEC SP_System_DashboardMetrics")
            .ToListAsync(ct);

        var model = new MonitoringViewModel
        {
            Metrics = metrics.FirstOrDefault(),
            Exams = new List<ExamMetricsResult>(),
            AuditLogs = new List<RecentAuditResult>()
        };

        // Parse remaining result sets
        if (metrics.Count > 0)
        {
            model.Metrics = metrics[0];
        }
        // Note: Multi-result-set SP parsing would require raw ADO.NET for ExamBreakdown + AuditLogs
        // For the dashboard, we show what we can from the first result set

        return View(model);
    }

    public class DashboardMetricsResult
    {
        public decimal AvgQueryTimeMs { get; set; }
        public int SlowQueries { get; set; }
        public int FailedRequests { get; set; }
        public int PublishedResults { get; set; }
        public int PendingResults { get; set; }
        public int TotalUsersOnline { get; set; }
        public DateTime SampledAt { get; set; }
    }

    public class ExamMetricsResult
    {
        public int ExamId { get; set; }
        public string ExamName { get; set; } = "";
        public string ExamType { get; set; } = "";
        public string Status { get; set; } = "";
        public int TotalStudents { get; set; }
        public int PublishedStudents { get; set; }
        public decimal AvgGPA { get; set; }
    }

    public class RecentAuditResult
    {
        public string Action { get; set; } = "";
        public string Entity { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
    }

    public class MonitoringViewModel
    {
        public DashboardMetricsResult? Metrics { get; set; }
        public List<ExamMetricsResult> Exams { get; set; } = new();
        public List<RecentAuditResult> AuditLogs { get; set; } = new();
    }
}
