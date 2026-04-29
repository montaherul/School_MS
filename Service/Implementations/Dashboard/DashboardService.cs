using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Service.Interfaces.Dashboard;

namespace SchoolManagementSystem.Service.Implementations.Dashboard;

public class DashboardService : IDashboardService
{
    private readonly SchoolDbContext _db;

    public DashboardService(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task<DashboardViewModel> GetDashboardAsync(CancellationToken cancellationToken = default)
    {
        var totalAttendance = await _db.Attendance.CountAsync(cancellationToken);
        var presentAttendance = await _db.Attendance.CountAsync(x => x.Status == AttendanceStatus.Present, cancellationToken);
        var feesCollected = await _db.FeeInvoices.SumAsync(x => x.PaidAmount, cancellationToken);
        var feesTotal = await _db.FeeInvoices.SumAsync(x => x.TotalAmount, cancellationToken);

        var studentsByClass = await _db.Classes
            .Where(c => !c.IsDeleted) // 🔥 FILTER DELETED CLASSES
            .Select(c => new ChartPoint(
                c.Name,
                _db.Students.Count(s => s.ClassId == c.Id && !s.IsDeleted) // 🔥 FILTER STUDENTS
            ))
            .ToListAsync(cancellationToken);

        var monthlyCollectionRows = await _db.Payments
            .GroupBy(p => new { p.PaidAt.Year, p.PaidAt.Month })
            .Select(g => new { g.Key.Year, g.Key.Month, Amount = g.Sum(p => p.Amount) })
            .OrderBy(x => x.Year)
            .ThenBy(x => x.Month)
            .ToListAsync(cancellationToken);
        var monthlyCollections = monthlyCollectionRows
            .Select(x => new ChartPoint($"{x.Year}-{x.Month:00}", x.Amount))
            .ToList();

        var recentActivities = await _db.Notices
            .OrderByDescending(x => x.PublishAt)
            .Take(5)
            .Select(x => new RecentActivityItem("Communication", x.Title, x.PublishAt))
            .ToListAsync(cancellationToken);

        return new DashboardViewModel
        {
            TotalStudents = await _db.Students.CountAsync(cancellationToken),
            PendingAdmissions = await _db.Admissions.CountAsync(x => x.Status == AdmissionStatus.Pending, cancellationToken),
            FeesCollected = feesCollected,
            FeesDue = feesTotal - feesCollected,
            AttendancePercentage = totalAttendance == 0 ? 0 : Math.Round((decimal)presentAttendance / totalAttendance * 100, 2),
            StudentsByClass = studentsByClass,
            MonthlyCollections = monthlyCollections,
            RecentActivities = recentActivities
        };
    }
}
