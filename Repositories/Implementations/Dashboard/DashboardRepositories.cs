using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.ViewModels.Dashboard;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Dashboard;
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

    public async Task<(int totalAttendance, int presentAttendance, decimal feesCollected, decimal feesTotal, List<DashboardChartDto> studentsByClass, List<DashboardChartDto> monthlyCollections, List<DashboardActivityDto> recentActivities, int totalStudents, int pendingAdmissions)> GetAdminDashboardDataAsync(CancellationToken ct, int? academicYearId = null)
    {
        DateTime? yearStart = null;
        DateTime? yearEnd = null;

        if (academicYearId.HasValue)
        {
            var year = await _db.AcademicYears
                .Where(y => y.Id == academicYearId.Value && !y.IsDeleted)
                .Select(y => new { y.StartsOn, y.EndsOn })
                .FirstOrDefaultAsync(ct);
            if (year != null)
            {
                yearStart = year.StartsOn;
                yearEnd = year.EndsOn;
            }
        }

        var studentQuery = _db.Students.Where(s => !s.IsDeleted);

        var admissionQuery = _db.Admissions.Where(a => a.Status == AdmissionStatus.Pending && !a.IsDeleted);
        if (yearStart.HasValue && yearEnd.HasValue)
            admissionQuery = admissionQuery.Where(a => a.CreatedAt >= yearStart.Value && a.CreatedAt <= yearEnd.Value);

        var attendanceQuery = _db.Attendance.Where(a => !a.IsDeleted);
        if (yearStart.HasValue && yearEnd.HasValue)
            attendanceQuery = attendanceQuery.Where(a => a.CreatedAt >= yearStart.Value && a.CreatedAt <= yearEnd.Value);

        var feeQuery = _db.FeeInvoices.Where(f => !f.IsDeleted);
        if (academicYearId.HasValue)
            feeQuery = feeQuery.Where(f => f.AcademicYearId == academicYearId.Value);

        var studentsByClassQuery = _db.Students.Where(s => !s.IsDeleted);
        if (yearStart.HasValue && yearEnd.HasValue)
            studentsByClassQuery = studentsByClassQuery.Where(s => s.CreatedAt >= yearStart.Value && s.CreatedAt <= yearEnd.Value);

        var monthlyFeeQuery = _db.FeeInvoices.Where(f => !f.IsDeleted && f.Status == PaymentStatus.Paid && f.UpdatedAt.HasValue);
        if (academicYearId.HasValue)
            monthlyFeeQuery = monthlyFeeQuery.Where(f => f.AcademicYearId == academicYearId.Value);

        var activitiesQuery = _db.ActivityLogs.Where(l => !l.IsDeleted);
        if (yearStart.HasValue && yearEnd.HasValue)
            activitiesQuery = activitiesQuery.Where(l => l.CreatedAt >= yearStart.Value && l.CreatedAt <= yearEnd.Value);

        var totalStudents = await studentQuery.CountAsync(ct);
        var pendingAdmissions = await admissionQuery.CountAsync(ct);
        var totalAttendance = await attendanceQuery.CountAsync(ct);
        var presentAttendance = await attendanceQuery.Where(a => a.Status == AttendanceStatus.Present || a.Status == AttendanceStatus.Late).CountAsync(ct);
        var feesCollected = await feeQuery.Where(f => f.Status == PaymentStatus.Paid).SumAsync(f => f.PaidAmount, ct);
        var feesTotal = await feeQuery.SumAsync(f => f.TotalAmount, ct);
        var studentsByClass = await studentsByClassQuery
            .GroupBy(s => s.ClassId)
            .Select(g => new DashboardChartDto { Label = g.Key.ToString(), Value = g.Count() })
            .ToListAsync(ct);
        var monthlyCollections = await monthlyFeeQuery
            .GroupBy(f => f.UpdatedAt.Value.Month)
            .Select(g => new DashboardChartDto { Label = g.Key.ToString(), Value = (int)g.Sum(f => f.PaidAmount) })
            .ToListAsync(ct);
        var recentActivities = await activitiesQuery
            .OrderByDescending(l => l.CreatedAt)
            .Take(10)
            .Select(l => new DashboardActivityDto
            {
                Title = l.Action ?? "",
                Module = l.Module ?? "",
                Summary = l.OldValues ?? "",
                At = l.CreatedAt
            })
            .ToListAsync(ct);

        return (totalAttendance, presentAttendance, feesCollected, feesTotal, studentsByClass, monthlyCollections, recentActivities, totalStudents, pendingAdmissions);
    }

    public async Task<(int totalAttendance, int presentCount, int absentCount, int lateCount, int leaveCount, decimal totalInvoiced, decimal totalPaid, List<DashboardActivityDto> recentNotices, List<DashboardAssignmentDto> upcomingAssignments)> GetStudentDashboardDataAsync(int studentId, int classId, int sectionId, CancellationToken ct)
    {
        var totalAttendance = await _db.Attendance.Where(a => a.StudentId == studentId && !a.IsDeleted).CountAsync(ct);
        var presentCount = await _db.Attendance.Where(a => a.StudentId == studentId && a.Status == AttendanceStatus.Present && !a.IsDeleted).CountAsync(ct);
        var absentCount = await _db.Attendance.Where(a => a.StudentId == studentId && a.Status == AttendanceStatus.Absent && !a.IsDeleted).CountAsync(ct);
        var lateCount = await _db.Attendance.Where(a => a.StudentId == studentId && a.Status == AttendanceStatus.Late && !a.IsDeleted).CountAsync(ct);
        var leaveCount = await _db.Attendance.Where(a => a.StudentId == studentId && a.Status == AttendanceStatus.Leave && !a.IsDeleted).CountAsync(ct);
        var totalInvoiced = await _db.FeeInvoices.Where(f => !f.IsDeleted && f.StudentId == studentId).SumAsync(f => f.TotalAmount, ct);
        var totalPaid = await _db.FeeInvoices.Where(f => !f.IsDeleted && f.StudentId == studentId && f.Status == PaymentStatus.Paid).SumAsync(f => f.PaidAmount, ct);

        var recentNotices = await _db.Notices
            .Where(n => !n.IsDeleted && n.IsPublished)
            .OrderByDescending(n => n.CreatedAt)
            .Take(5)
            .Select(n => new DashboardActivityDto
            {
                Title = n.Title ?? "",
                Module = "Notice",
                Summary = n.Body ?? "",
                At = n.CreatedAt
            })
            .ToListAsync(ct);

        var upcomingAssignments = await _db.Assignments
            .Where(a => !a.IsDeleted && a.Deadline > DateTime.UtcNow)
            .OrderBy(a => a.Deadline)
            .Take(5)
            .Select(a => new DashboardAssignmentDto
            {
                Title = a.Title ?? "",
                Subject = "",
                Deadline = a.Deadline
            })
            .ToListAsync(ct);

        return (totalAttendance, presentCount, absentCount, lateCount, leaveCount, totalInvoiced, totalPaid, recentNotices, upcomingAssignments);
    }

    public async Task<List<DashboardCalendarDto>> GetStudentAttendanceCalendarAsync(int studentId, int year, int month, CancellationToken ct)
    {
        var records = await _db.Attendance
            .AsNoTracking()
            .Where(a => a.StudentId == studentId
                && !a.IsDeleted
                && a.AttendanceDate.Year == year
                && a.AttendanceDate.Month == month)
            .OrderBy(a => a.AttendanceDate)
            .Select(a => new DashboardCalendarDto
            {
                Date = a.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                Status = a.Status.ToString()
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

    public async Task<(List<DashboardChartDto> Daily, List<DashboardChartDto> Monthly)> GetAttendanceAnalyticsAsync(CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetAttendanceAnalytics";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@StartDate", DateTime.Today.AddDays(-6)));
        command.Parameters.Add(new SqlParameter("@EndDate", DateTime.Today));

        if (command.Connection!.State != ConnectionState.Open)
            await command.Connection.OpenAsync(ct);

        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            var daily = new List<DashboardChartDto>();
            while (await reader.ReadAsync(ct))
            {
                daily.Add(new DashboardChartDto { Label = reader.GetDateTime(0).ToString("yyyy-MM-dd"), Value = reader.GetDecimal(3) });
            }

            var monthly = new List<DashboardChartDto>();
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    monthly.Add(new DashboardChartDto { Label = $"{reader.GetInt32(0)}-{reader.GetInt32(1):D2}", Value = reader.GetDecimal(4) });
                }
            }

            return (daily, monthly);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<List<DashboardChartDto>> GetClassAttendanceAnalyticsAsync(DateTime date, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetClassAttendanceAnalytics";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@Date", date.Date));

        if (command.Connection!.State != ConnectionState.Open)
            await command.Connection.OpenAsync(ct);

        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            var points = new List<DashboardChartDto>();
            while (await reader.ReadAsync(ct))
            {
                points.Add(new DashboardChartDto { Label = reader.GetString(1), Value = reader.GetDecimal(4) });
            }

            return points;
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<List<StudentResultViewModel>> GetStudentLatestResultsAsync(int studentId, CancellationToken cancellationToken = default)
    {
        return await _db.StudentExamResults
            .AsNoTracking()
            .Include(r => r.Exam)
            .Where(r => r.StudentId == studentId && !r.IsDeleted
                && (r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked))
            .OrderByDescending(r => r.CalculatedAt)
            .Take(5)
            .Select(r => new StudentResultViewModel
            {
                SubjectName = "Overall",
                ExamName = r.Exam.Name,
                ObtainedMarks = r.TotalMarks,
                FullMarks = r.TotalFullMarks,
                Grade = r.Grade,
                GPA = r.Gpa,
                IsPassed = r.IsPassed
            })
            .ToListAsync(cancellationToken);
    }

    public async Task<StudentRoutineWidgetDto> GetStudentRoutineWidgetAsync(int classId, int sectionId, int? groupId, CancellationToken ct)
    {
        var today = DateTime.UtcNow.DayOfWeek.ToString();
        var dayNames = new[] { "Sunday", "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday" };

        var routines = await _db.Set<SchoolManagementSystem.Models.Entities.Teachers.TeacherTimetable>()
            .Where(r => r.ClassId == classId && r.SectionId == sectionId && !r.IsDeleted
                && (groupId == null || r.GroupId == null || r.GroupId == groupId))
            .Select(r => new RoutineClassDto
            {
                SubjectName = r.Subject != null ? r.Subject.Name : "",
                TeacherName = r.Teacher != null ? r.Teacher.FullName : "",
                DayOfWeek = r.DayOfWeek,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                RoomNo = r.RoomNo
            })
            .ToListAsync(ct);

        var todayClasses = routines.Where(r => r.DayOfWeek == today).OrderBy(r => r.StartTime).ToList();
        var nextClass = todayClasses.FirstOrDefault();

        return new StudentRoutineWidgetDto
        {
            TodayClasses = todayClasses,
            ThisWeekClasses = routines.Where(r => dayNames.Contains(r.DayOfWeek)).OrderBy(r => Array.IndexOf(dayNames, r.DayOfWeek)).ThenBy(r => r.StartTime).ToList(),
            NextClass = nextClass
        };
    }

    public async Task<(int Pending, int Submitted, int Overdue, List<StudentAssignmentDto> Recent)> GetStudentAssignmentWidgetAsync(int studentId, int classId, int sectionId, CancellationToken ct)
    {
        var now = DateTime.UtcNow;
        var assignments = await _db.Assignments
            .Where(a => a.SchoolClassId == classId && a.SectionId == sectionId && !a.IsDeleted)
            .Select(a => new
            {
                a.Id,
                a.Title,
                a.Instructions,
                a.Deadline,
                a.Status,
                SubjectName = a.SubjectId > 0 ? _db.Subjects.Where(s => s.Id == a.SubjectId).Select(s => s.Name).FirstOrDefault() : "",
                TeacherName = a.TeacherProfileId > 0 ? _db.Teachers.Where(t => t.Id == a.TeacherProfileId).Select(t => t.FullName).FirstOrDefault() : ""
            })
            .ToListAsync(ct);

        var submissionIds = await _db.AssignmentSubmissions
            .Where(s => s.StudentId == studentId && !s.IsDeleted)
            .Select(s => s.AssignmentTaskId)
            .ToListAsync(ct);

        var pending = assignments.Where(a => !submissionIds.Contains(a.Id) && a.Deadline > now).ToList();
        var submitted = assignments.Where(a => submissionIds.Contains(a.Id)).ToList();
        var overdue = assignments.Where(a => !submissionIds.Contains(a.Id) && a.Deadline <= now).ToList();

        var recent = assignments.OrderByDescending(a => a.Deadline).Take(5).Select(a => new StudentAssignmentDto
        {
            Id = a.Id,
            Title = a.Title,
            Instructions = a.Instructions,
            Deadline = a.Deadline,
            AssignmentStatus = (int)a.Status,
            SubjectName = a.SubjectName ?? "",
            TeacherName = a.TeacherName ?? "",
            IsSubmitted = submissionIds.Contains(a.Id)
        }).ToList();

        return (pending.Count, submitted.Count, overdue.Count, recent);
    }

    public async Task<(List<StudentLibraryBookDto> Books, int Total)> GetStudentLibraryWidgetAsync(int studentId, CancellationToken ct)
    {
        var books = await _db.BookIssues
            .Where(bi => bi.StudentId == studentId && !bi.IsDeleted)
            .OrderByDescending(bi => bi.IssueDate)
            .Select(bi => new StudentLibraryBookDto
            {
                Id = bi.Id,
                BookTitle = bi.BookId > 0 ? _db.Books.Where(b => b.Id == bi.BookId).Select(b => b.Title).FirstOrDefault() ?? "" : "",
                Author = bi.BookId > 0 ? _db.Books.Where(b => b.Id == bi.BookId).Select(b => b.Author).FirstOrDefault() ?? "" : "",
                AccessionNo = bi.BookId > 0 ? _db.Books.Where(b => b.Id == bi.BookId).Select(b => b.AccessionNo).FirstOrDefault() ?? "" : "",
                IssueDate = bi.IssueDate,
                DueDate = bi.DueDate,
                ReturnedDate = bi.ReturnedDate,
                FineAmount = bi.FineAmount,
                Status = bi.ReturnedDate == null ? "Issued" : "Returned"
            })
            .ToListAsync(ct);

        return (books, books.Count);
    }

    public async Task<(int UnreadCount, List<StudentNotificationItemDto> Recent)> GetStudentNotificationWidgetAsync(int userId, CancellationToken ct)
    {
        var recent = await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .Take(10)
            .Select(n => new StudentNotificationItemDto
            {
                Id = n.Id,
                Title = n.Title,
                Body = n.Body,
                Channel = (int)n.Channel,
                IsRead = n.IsRead,
                SentAt = n.SentAt,
                CreatedAt = n.CreatedAt
            })
            .ToListAsync(ct);

        var unreadCount = recent.Count(n => !n.IsRead);

        return (unreadCount, recent);
    }

    // ──────────────── Teacher Widgets ────────────────────────────────────────

    public async Task<List<TeacherScheduleItemDto>> GetTeacherTimetableAsync(int teacherId, CancellationToken ct)
    {
        return await _db.Set<SchoolManagementSystem.Models.Entities.Teachers.TeacherTimetable>()
            .AsNoTracking()
            .Include(r => r.Subject)
            .Include(r => r.Class)
            .Include(r => r.Section)
            .Where(r => r.TeacherId == teacherId && !r.IsDeleted)
            .OrderBy(r => r.DayOfWeek == "Sunday" ? 0 :
                         r.DayOfWeek == "Monday" ? 1 :
                         r.DayOfWeek == "Tuesday" ? 2 :
                         r.DayOfWeek == "Wednesday" ? 3 :
                         r.DayOfWeek == "Thursday" ? 4 :
                         r.DayOfWeek == "Friday" ? 5 : 6)
            .ThenBy(r => r.StartTime)
            .Select(r => new TeacherScheduleItemDto
            {
                SubjectName = r.Subject != null ? r.Subject.Name : "",
                ClassName = r.Class != null ? r.Class.Name : "",
                SectionName = r.Section != null ? r.Section.Name : "",
                DayOfWeek = r.DayOfWeek,
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                RoomNo = r.RoomNo
            })
            .ToListAsync(ct);
    }

    public async Task<List<TeacherMarkEntryStatusDto>> GetTeacherMarkEntryStatusAsync(int teacherId, CancellationToken ct)
    {
        var markEntries = await _db.Marks
            .AsNoTracking()
            .Include(m => m.Exam)
            .Include(m => m.Subject)
            .Where(m => m.EnteredByTeacherId == teacherId && !m.IsDeleted)
            .GroupBy(m => new { m.ExamId, m.SubjectId, m.ClassId, m.SectionId })
            .Select(g => new
            {
                g.Key.ExamId,
                g.Key.SubjectId,
                g.Key.ClassId,
                g.Key.SectionId,
                ExamName = g.First().Exam.Name,
                SubjectName = g.First().Subject.Name,
                TotalStudents = g.Count(),
                MarksEntered = g.Count(m => m.MarksObtained > 0),
                Status = g.First().Status
            })
            .ToListAsync(ct);

        var result = new List<TeacherMarkEntryStatusDto>();
        foreach (var entry in markEntries)
        {
            var className = await _db.Classes.Where(c => c.Id == entry.ClassId && !c.IsDeleted).Select(c => c.Name).FirstOrDefaultAsync(ct) ?? "";
            var sectionName = await _db.Sections.Where(s => s.Id == entry.SectionId && !s.IsDeleted).Select(s => s.Name).FirstOrDefaultAsync(ct) ?? "";

            result.Add(new TeacherMarkEntryStatusDto
            {
                SubjectName = entry.SubjectName,
                ExamName = entry.ExamName,
                ClassName = className,
                SectionName = sectionName,
                TotalStudents = entry.TotalStudents,
                MarksEntered = entry.MarksEntered,
                PendingCount = entry.TotalStudents - entry.MarksEntered,
                Status = entry.Status.ToString()
            });
        }

        return result;
    }

    public async Task<(List<StudentAssignmentDto> Recent, int Total)> GetTeacherAssignmentWidgetAsync(int teacherId, CancellationToken ct)
    {
        var assignments = await _db.Assignments
            .AsNoTracking()
            .Where(a => a.TeacherProfileId == teacherId && !a.IsDeleted)
            .OrderByDescending(a => a.Deadline)
            .Select(a => new StudentAssignmentDto
            {
                Id = a.Id,
                Title = a.Title,
                Instructions = a.Instructions,
                Deadline = a.Deadline,
                AssignmentStatus = (int)a.Status,
                SubjectName = a.SubjectId > 0 ? _db.Subjects.Where(s => s.Id == a.SubjectId).Select(s => s.Name).FirstOrDefault() ?? "" : "",
                TeacherName = ""
            })
            .ToListAsync(ct);

        var total = assignments.Count;
        var recent = assignments.Take(5).ToList();

        return (recent, total);
    }

    public async Task<int> GetTeacherPendingResultCountAsync(int teacherId, CancellationToken ct)
    {
        return await _db.Marks
            .CountAsync(m => m.EnteredByTeacherId == teacherId && !m.IsDeleted && m.Status == ResultWorkflowStatus.Draft, ct);
    }

    public async Task<TeacherLeaveStatusDto> GetTeacherLeaveStatusAsync(int employeeId, CancellationToken ct)
    {
        var leaves = await _db.LeaveApplications
            .Where(l => l.EmployeeId == employeeId)
            .ToListAsync(ct);

        return new TeacherLeaveStatusDto
        {
            TotalLeaves = leaves.Count,
            ApprovedLeaves = leaves.Count(l => l.ApprovalStatus == LeaveStatus.Approved),
            PendingLeaves = leaves.Count(l => l.ApprovalStatus == LeaveStatus.Pending),
            RejectedLeaves = leaves.Count(l => l.ApprovalStatus == LeaveStatus.Rejected)
        };
    }

    public async Task<(int UnreadCount, List<TeacherNotificationItemDto> Recent)> GetTeacherNotificationWidgetAsync(int userId, CancellationToken ct)
    {
        var recent = await _db.Notifications
            .Where(n => n.UserId == userId && !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .Take(10)
            .Select(n => new TeacherNotificationItemDto
            {
                Id = n.Id,
                Title = n.Title,
                Body = n.Body,
                IsRead = n.IsRead,
                SentAt = n.SentAt
            })
            .ToListAsync(ct);

        var unreadCount = recent.Count(n => !n.IsRead);

        return (unreadCount, recent);
    }

    public async Task<LibrarianDashboardViewModel> GetLibrarianDashboardDataAsync(CancellationToken ct)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);

        var booksIssuedToday = await _db.BookIssues
            .CountAsync(bi => bi.IssueDate == today && !bi.IsDeleted, ct);

        var booksReturnedToday = await _db.BookIssues
            .CountAsync(bi => bi.ReturnedDate == today && !bi.IsDeleted, ct);

        var overdueBooks = await _db.BookIssues
            .CountAsync(bi => bi.ReturnedDate == null && bi.DueDate < today && !bi.IsDeleted, ct);

        var totalFineCollected = await _db.BookIssues
            .Where(bi => bi.ReturnedDate != null && !bi.IsDeleted)
            .SumAsync(bi => bi.FineAmount, ct);

        var activeMembers = await _db.BookIssues
            .Where(bi => !bi.IsDeleted)
            .Select(bi => bi.StudentId)
            .Distinct()
            .CountAsync(ct);

        var pendingReturns = await _db.BookIssues
            .CountAsync(bi => bi.ReturnedDate == null && !bi.IsDeleted, ct);

        var recentTransactions = await _db.BookIssues
            .Where(bi => !bi.IsDeleted)
            .OrderByDescending(bi => bi.IssueDate)
            .Take(10)
            .Select(bi => new LibrarianTransactionDto
            {
                Id = bi.Id,
                BookTitle = bi.BookId > 0 ? _db.Books.Where(b => b.Id == bi.BookId).Select(b => b.Title).FirstOrDefault() ?? "" : "",
                StudentName = bi.StudentId > 0 ? _db.Students.Where(s => s.Id == bi.StudentId).Select(s => s.FullName).FirstOrDefault() ?? "" : "",
                StudentNo = bi.StudentId > 0 ? _db.Students.Where(s => s.Id == bi.StudentId).Select(s => s.StudentNo).FirstOrDefault() ?? "" : "",
                IssueDate = bi.IssueDate,
                DueDate = bi.DueDate,
                ReturnedDate = bi.ReturnedDate,
                FineAmount = bi.FineAmount,
                Status = bi.ReturnedDate == null ? "Issued" : "Returned"
            })
            .ToListAsync(ct);

        var notifications = await _db.Notifications
            .Where(n => !n.IsDeleted)
            .OrderByDescending(n => n.CreatedAt)
            .Take(10)
            .Select(n => new LibrarianNotificationDto
            {
                Id = n.Id,
                Title = n.Title,
                Body = n.Body,
                IsRead = n.IsRead,
                SentAt = n.SentAt
            })
            .ToListAsync(ct);

        var unreadNotificationCount = notifications.Count(n => !n.IsRead);

        var dailyIssued = await _db.BookIssues.CountAsync(bi => bi.IssueDate == today && !bi.IsDeleted, ct);
        var dailyReturned = await _db.BookIssues.CountAsync(bi => bi.ReturnedDate == today && !bi.IsDeleted, ct);
        var dailyFines = await _db.BookIssues
            .Where(bi => bi.ReturnedDate == today && !bi.IsDeleted)
            .SumAsync(bi => bi.FineAmount, ct);

        var monthStart = new DateOnly(today.Year, today.Month, 1);
        var monthlyIssued = await _db.BookIssues
            .CountAsync(bi => bi.IssueDate >= monthStart && bi.IssueDate <= today && !bi.IsDeleted, ct);
        var monthlyReturned = await _db.BookIssues
            .CountAsync(bi => bi.ReturnedDate >= monthStart && bi.ReturnedDate <= today && !bi.IsDeleted, ct);
        var monthlyFines = await _db.BookIssues
            .Where(bi => bi.ReturnedDate >= monthStart && bi.ReturnedDate <= today && !bi.IsDeleted)
            .SumAsync(bi => bi.FineAmount, ct);
        var monthlyOverdue = await _db.BookIssues
            .CountAsync(bi => bi.ReturnedDate == null && bi.DueDate < today && !bi.IsDeleted, ct);

        return new LibrarianDashboardViewModel
        {
            BooksIssuedToday = booksIssuedToday,
            BooksReturnedToday = booksReturnedToday,
            OverdueBooks = overdueBooks,
            TotalFineCollected = totalFineCollected,
            ActiveMembers = activeMembers,
            PendingReturns = pendingReturns,
            RecentTransactions = recentTransactions,
            UnreadNotificationCount = unreadNotificationCount,
            RecentNotifications = notifications,
            DailyActivity = new DailyActivityReport
            {
                Issued = dailyIssued,
                Returned = dailyReturned,
                FinesCollected = dailyFines
            },
            MonthlyActivity = new MonthlyActivityReport
            {
                TotalIssued = monthlyIssued,
                TotalReturned = monthlyReturned,
                TotalFinesCollected = monthlyFines,
                TotalOverdue = monthlyOverdue
            }
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

    public Task<(int totalAttendance, int presentAttendance, decimal feesCollected, decimal feesTotal, List<DashboardChartDto> studentsByClass, List<DashboardChartDto> monthlyCollections, List<DashboardActivityDto> recentActivities, int totalStudents, int pendingAdmissions)> GetAdminDashboardDataAsync(CancellationToken ct, int? academicYearId = null) => _repo.GetAdminDashboardDataAsync(ct, academicYearId);

    public Task<(int totalAttendance, int presentCount, int absentCount, int lateCount, int leaveCount, decimal totalInvoiced, decimal totalPaid, List<DashboardActivityDto> recentNotices, List<DashboardAssignmentDto> upcomingAssignments)> GetStudentDashboardDataAsync(int studentId, int classId, int sectionId, CancellationToken ct) => _repo.GetStudentDashboardDataAsync(studentId, classId, sectionId, ct);

    public Task<List<DashboardCalendarDto>> GetStudentAttendanceCalendarAsync(int studentId, int year, int month, CancellationToken ct) => _repo.GetStudentAttendanceCalendarAsync(studentId, year, month, ct);

    public Task<DashboardAttendanceSummaryDto> GetAttendanceDashboardSummaryAsync(DateTime date, CancellationToken ct) => _repo.GetAttendanceDashboardSummaryAsync(date, ct);

    public Task<(List<DashboardChartDto> Daily, List<DashboardChartDto> Monthly)> GetAttendanceAnalyticsAsync(CancellationToken ct) => _repo.GetAttendanceAnalyticsAsync(ct);

    public Task<List<DashboardChartDto>> GetClassAttendanceAnalyticsAsync(DateTime date, CancellationToken ct) => _repo.GetClassAttendanceAnalyticsAsync(date, ct);

    public Task<List<StudentResultViewModel>> GetStudentLatestResultsAsync(int studentId, CancellationToken cancellationToken = default) => _repo.GetStudentLatestResultsAsync(studentId, cancellationToken);

    public Task<StudentRoutineWidgetDto> GetStudentRoutineWidgetAsync(int classId, int sectionId, int? groupId, CancellationToken ct) => _repo.GetStudentRoutineWidgetAsync(classId, sectionId, groupId, ct);

    public Task<(int Pending, int Submitted, int Overdue, List<StudentAssignmentDto> Recent)> GetStudentAssignmentWidgetAsync(int studentId, int classId, int sectionId, CancellationToken ct) => _repo.GetStudentAssignmentWidgetAsync(studentId, classId, sectionId, ct);

    public Task<(List<StudentLibraryBookDto> Books, int Total)> GetStudentLibraryWidgetAsync(int studentId, CancellationToken ct) => _repo.GetStudentLibraryWidgetAsync(studentId, ct);

    public Task<(int UnreadCount, List<StudentNotificationItemDto> Recent)> GetStudentNotificationWidgetAsync(int userId, CancellationToken ct) => _repo.GetStudentNotificationWidgetAsync(userId, ct);

    public Task<LibrarianDashboardViewModel> GetLibrarianDashboardDataAsync(CancellationToken ct) => _repo.GetLibrarianDashboardDataAsync(ct);
}
