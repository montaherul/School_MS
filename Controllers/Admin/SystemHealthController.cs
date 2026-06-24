using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Filters;

namespace SchoolManagementSystem.Controllers.Admin;

[RequirePermission("SystemHealth.View")]
public class SystemHealthController : Controller
{
    private readonly SchoolDbContext _db;

    public SystemHealthController(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var healthData = await _db.Database
            .SqlQueryRaw<DatabaseHealthResult>("EXEC SP_System_DatabaseHealth")
            .ToListAsync(ct);

        var metricsData = await _db.Database
            .SqlQueryRaw<DashboardMetric>("EXEC SP_System_DashboardMetrics")
            .ToListAsync(ct);

        var model = new SystemHealthViewModel
        {
            DatabaseHealth = healthData,
            DashboardMetrics = metricsData.FirstOrDefault(),
            ExamBreakdown = metricsData.Skip(1).Select(m => new ExamMetric
            {
                ExamName = m.ExamName ?? "",
                TotalStudents = m.TotalStudents,
                PublishedStudents = m.PublishedStudents,
                AvgGPA = m.AvgGPA
            }).ToList(),
            RecentActivity = new List<AuditEntry>() // populated from second result set
        };

        return View(model);
    }

    public class DatabaseHealthResult
    {
        public string DatabaseName { get; set; } = "";
        public int SizeMB { get; set; }
        public int DataSizeMB { get; set; }
        public int LogSizeMB { get; set; }
        public string EntityType { get; set; } = "";
        public int TotalCount { get; set; }
        public DateTime? LastBackup { get; set; }
        public DateTime? LastRestore { get; set; }
        public DateTime? LastResultPublish { get; set; }
        public DateTime? LastPublishAction { get; set; }
        public DateTime ReportGeneratedAt { get; set; }
        public int TotalProcedures { get; set; }
        public int WithDefinition { get; set; }
        public int DecompilationErrors { get; set; }
    }

    public class DashboardMetric
    {
        public decimal AvgQueryTimeMs { get; set; }
        public int SlowQueries { get; set; }
        public int FailedRequests { get; set; }
        public int PublishedResults { get; set; }
        public int PendingResults { get; set; }
        public int TotalUsersOnline { get; set; }
        public DateTime SampledAt { get; set; }
        public string ExamName { get; set; } = "";
        public int TotalStudents { get; set; }
        public int PublishedStudents { get; set; }
        public decimal AvgGPA { get; set; }
    }

    public class ExamMetric
    {
        public string ExamName { get; set; } = "";
        public int TotalStudents { get; set; }
        public int PublishedStudents { get; set; }
        public decimal AvgGPA { get; set; }
    }

    public class AuditEntry
    {
        public string Action { get; set; } = "";
        public string Entity { get; set; } = "";
        public DateTime Timestamp { get; set; }
        public int UserId { get; set; }
    }

    public class SystemHealthViewModel
    {
        public List<DatabaseHealthResult> DatabaseHealth { get; set; } = new();
        public DashboardMetric? DashboardMetrics { get; set; }
        public List<ExamMetric> ExamBreakdown { get; set; } = new();
        public List<AuditEntry> RecentActivity { get; set; } = new();
    }
}
