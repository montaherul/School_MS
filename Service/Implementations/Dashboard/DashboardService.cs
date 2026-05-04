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

        //var recentActivities = await _db.Notices
        //    .OrderByDescending(x => x.PublishAt)
        //    .Take(5)
        //    .Select(x => new RecentActivityItem("Communication", x.Title, x.PublishAt))

        //    .ToListAsync(cancellationToken);

        return new DashboardViewModel
        {
            TotalStudents = await _db.Students.CountAsync(x => !x.IsDeleted, cancellationToken),
            PendingAdmissions = await _db.Admissions.CountAsync(x => x.Status == AdmissionStatus.Pending, cancellationToken),
            FeesCollected = feesCollected,
            FeesDue = feesTotal - feesCollected,
            AttendancePercentage = totalAttendance == 0 ? 0 : Math.Round((decimal)presentAttendance / totalAttendance * 100, 2),
            StudentsByClass = studentsByClass,
            MonthlyCollections = monthlyCollections,
           // RecentActivities = recentActivities
        };
    }

    public async Task<StudentDashboardViewModel> GetStudentDashboardAsync(int userId, CancellationToken cancellationToken = default)
    {
        var student = await _db.Students
            .Include(s => s.Class)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Student profile not found for this user.");

        var totalAttendance = await _db.Attendance.CountAsync(x => x.StudentId == student.Id, cancellationToken);
        var presentAttendance = await _db.Attendance.CountAsync(x => x.StudentId == student.Id && x.Status == AttendanceStatus.Present, cancellationToken);

        var totalInvoiced = await _db.FeeInvoices.Where(x => x.StudentId == student.Id && !x.IsDeleted).SumAsync(x => x.TotalAmount, cancellationToken);
        var totalPaid = await _db.FeeInvoices.Where(x => x.StudentId == student.Id && !x.IsDeleted).SumAsync(x => x.PaidAmount, cancellationToken);

        //var recentNotices = await _db.Notices
        //    .Where(x => x.AudienceRole == "All" || x.AudienceRole == "Student")
        //    .OrderByDescending(x => x.PublishAt)
        //    .Take(5)
        //    .Select(x => new RecentActivityItem("Notice", x.Title, x.PublishAt, x.Description ?? ""))
        //    .ToListAsync(cancellationToken);


        var upcomingAssignments = await _db.Assignments
            .Where(x => x.SchoolClassId == student.ClassId && x.SectionId == student.SectionId && x.Deadline >= DateTime.UtcNow)
            .OrderBy(x => x.Deadline)
            .Take(5)
            .Select(x => new AssignmentDashboardItem(
                _db.Subjects.Where(s => s.Id == x.SubjectId).Select(s => s.Name).FirstOrDefault() ?? "Unknown",
                x.Title,
                x.Deadline))
            .ToListAsync(cancellationToken);

        return new StudentDashboardViewModel
        {
            Id = student.Id,
            FullName = student.FullName,
            StudentNo = student.StudentNo,
            ClassName = student.Class?.Name ?? "N/A",
            SectionName = student.Section?.Name ?? "N/A",
            RollNumber = student.RollNumber,
            AttendancePercentage = totalAttendance == 0 ? 0 : Math.Round((decimal)presentAttendance / totalAttendance * 100, 2),
            TotalDue = totalInvoiced - totalPaid,
            StudentStatus = student.Status.ToString(),
         //   RecentNotices = recentNotices,
            UpcomingAssignments = upcomingAssignments
        };
    }
}
