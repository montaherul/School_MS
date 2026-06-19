using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Assignment;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Entities.Notification;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Implementations.Dashboard;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class TeacherDashboardWidgetTests
{
    private SchoolDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new SchoolDbContext(options);

        var subjects = new List<Subject>
        {
            new() { Id = 1, Name = "Mathematics", Code = "MATH" },
            new() { Id = 2, Name = "Physics", Code = "PHY" }
        };
        db.Subjects.AddRange(subjects);

        var classes = new List<SchoolClass>
        {
            new() { Id = 1, Name = "Class 10" },
            new() { Id = 2, Name = "Class 9" }
        };
        db.Classes.AddRange(classes);

        var sections = new List<Section>
        {
            new() { Id = 1, Name = "A" },
            new() { Id = 2, Name = "B" }
        };
        db.Sections.AddRange(sections);

        var employees = new List<Employee>
        {
            new() { Id = 1, FullName = "Mr. Smith", EmployeeCode = "EMP001", Phone = "1234567890", Email = "smith@school.local", Status = "Active" },
            new() { Id = 2, FullName = "Mrs. Jones", EmployeeCode = "EMP002", Phone = "0987654321", Email = "jones@school.local", Status = "Active" }
        };
        db.Employees.AddRange(employees);

        var teachers = new List<Teacher>
        {
            new() { Id = 1, EmployeeId = 1, TeacherCode = "TCH001" },
            new() { Id = 2, EmployeeId = 2, TeacherCode = "TCH002" }
        };
        db.Teachers.AddRange(teachers);

        var routines = new List<TeacherTimetable>
        {
            new() { Id = 1, ClassId = 1, SectionId = 1, SubjectId = 1, TeacherId = 1, DayOfWeek = DateTime.UtcNow.DayOfWeek.ToString(), StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(9, 0, 0), RoomNo = "101" },
            new() { Id = 2, ClassId = 1, SectionId = 1, SubjectId = 2, TeacherId = 1, DayOfWeek = "Monday", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 0, 0), RoomNo = "102" },
            new() { Id = 3, ClassId = 2, SectionId = 2, SubjectId = 1, TeacherId = 2, DayOfWeek = DateTime.UtcNow.DayOfWeek.ToString(), StartTime = new TimeSpan(9, 0, 0), EndTime = new TimeSpan(10, 0, 0) }
        };
        db.Set<TeacherTimetable>().AddRange(routines);

        var exams = new List<Exam>
        {
            new() { Id = 1, Name = "Final Exam 2026", AcademicYearId = 1, ClassId = 1, StartsOn = new(2026, 1, 1), EndsOn = new(2026, 3, 31) }
        };
        db.Exams.AddRange(exams);

        var markEntries = new List<MarkEntry>
        {
            new() { Id = 1, ExamId = 1, StudentId = 1, SubjectId = 1, ClassId = 1, SectionId = 1, EnteredByTeacherId = 1, MarksObtained = 85, Status = ResultWorkflowStatus.Draft },
            new() { Id = 2, ExamId = 1, StudentId = 2, SubjectId = 1, ClassId = 1, SectionId = 1, EnteredByTeacherId = 1, MarksObtained = 0, Status = ResultWorkflowStatus.Draft },
            new() { Id = 3, ExamId = 1, StudentId = 3, SubjectId = 1, ClassId = 1, SectionId = 1, EnteredByTeacherId = 1, MarksObtained = 90, Status = ResultWorkflowStatus.Submitted },
            new() { Id = 4, ExamId = 1, StudentId = 1, SubjectId = 2, ClassId = 1, SectionId = 1, EnteredByTeacherId = 1, MarksObtained = 75, Status = ResultWorkflowStatus.Draft },
            new() { Id = 5, ExamId = 1, StudentId = 1, SubjectId = 1, ClassId = 1, SectionId = 1, EnteredByTeacherId = 2, MarksObtained = 88, Status = ResultWorkflowStatus.Draft }
        };
        db.Marks.AddRange(markEntries);

        var assignments = new List<AssignmentTask>
        {
            new() { Id = 1, SchoolClassId = 1, SectionId = 1, SubjectId = 1, TeacherProfileId = 1, Title = "Homework 1", Deadline = DateTime.UtcNow.AddDays(7), Status = AssignmentStatus.Open },
            new() { Id = 2, SchoolClassId = 1, SectionId = 1, SubjectId = 2, TeacherProfileId = 1, Title = "Past Due Task", Deadline = DateTime.UtcNow.AddDays(-3), Status = AssignmentStatus.Open },
            new() { Id = 3, SchoolClassId = 2, SectionId = 2, SubjectId = 1, TeacherProfileId = 2, Title = "Other Teacher Task", Deadline = DateTime.UtcNow.AddDays(5), Status = AssignmentStatus.Open }
        };
        db.Assignments.AddRange(assignments);

        var leaves = new List<LeaveApplication>
        {
            new() { Id = 1, EmployeeId = 1, LeaveTypeId = 1, FromDate = DateTime.Today.AddDays(-5), ToDate = DateTime.Today.AddDays(-3), TotalDays = 3, ApprovalStatus = LeaveStatus.Approved },
            new() { Id = 2, EmployeeId = 1, LeaveTypeId = 1, FromDate = DateTime.Today.AddDays(1), ToDate = DateTime.Today.AddDays(2), TotalDays = 2, ApprovalStatus = LeaveStatus.Pending },
            new() { Id = 3, EmployeeId = 1, LeaveTypeId = 2, FromDate = DateTime.Today.AddDays(-10), ToDate = DateTime.Today.AddDays(-8), TotalDays = 3, ApprovalStatus = LeaveStatus.Rejected },
            new() { Id = 4, EmployeeId = 2, LeaveTypeId = 1, FromDate = DateTime.Today, ToDate = DateTime.Today, TotalDays = 1, ApprovalStatus = LeaveStatus.Approved }
        };
        db.LeaveApplications.AddRange(leaves);

        var notifications = new List<NotificationMessage>
        {
            new() { Id = 1, UserId = 1, Title = "Meeting Reminder", Body = "Staff meeting at 3pm", IsRead = false, Channel = NotificationChannel.InApp, CreatedAt = DateTime.UtcNow.AddHours(-2) },
            new() { Id = 2, UserId = 1, Title = "Result Deadline", Body = "Submit results by Friday", IsRead = true, Channel = NotificationChannel.InApp, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 3, UserId = 2, Title = "Other User", IsRead = false, Channel = NotificationChannel.InApp, CreatedAt = DateTime.UtcNow },
            new() { Id = 4, UserId = 1, Title = "Leave Approved", Body = "Your leave has been approved", IsRead = false, Channel = NotificationChannel.InApp, CreatedAt = DateTime.UtcNow.AddHours(-1) }
        };
        db.Notifications.AddRange(notifications);

        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task GetTeacherTimetableAsync_ReturnsOnlyTeacherSchedule()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetTeacherTimetableAsync(1, CancellationToken.None);

        Assert.NotEmpty(result);
        Assert.All(result, s => Assert.True(
            s.SubjectName.Contains("Mathematics", StringComparison.OrdinalIgnoreCase) ||
            s.SubjectName.Contains("Physics", StringComparison.OrdinalIgnoreCase)));
        Assert.DoesNotContain(result, s => s.ClassName == "Class 9");
    }

    [Fact]
    public async Task GetTeacherTimetableAsync_DifferentTeacher_ReturnsOnlyTheirSchedule()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetTeacherTimetableAsync(2, CancellationToken.None);

        Assert.Single(result);
        Assert.Contains("Class 9", result[0].ClassName);
    }

    [Fact]
    public async Task GetTeacherMarkEntryStatusAsync_ReturnsGroupedEntryData()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetTeacherMarkEntryStatusAsync(1, CancellationToken.None);

        Assert.NotEmpty(result);
        var mathSubject = result.FirstOrDefault(r => r.SubjectName == "Mathematics");
        Assert.NotNull(mathSubject);
        Assert.Equal(3, mathSubject.TotalStudents);
        Assert.True(mathSubject.MarksEntered >= 2); // 2 marks entries with MarksObtained > 0 for subject 1
    }

    [Fact]
    public async Task GetTeacherMarkEntryStatusAsync_ExcludesOtherTeachers()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetTeacherMarkEntryStatusAsync(2, CancellationToken.None);

        Assert.Single(result);
        Assert.Equal("Mathematics", result[0].SubjectName);
    }

    [Fact]
    public async Task GetTeacherAssignmentWidgetAsync_ReturnsOnlyTeacherAssignments()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (recent, total) = await repo.GetTeacherAssignmentWidgetAsync(1, CancellationToken.None);

        Assert.Equal(2, total);
        Assert.True(recent.Any(a => a.Title == "Homework 1"));
        Assert.True(recent.Any(a => a.Title == "Past Due Task"));
    }

    [Fact]
    public async Task GetTeacherAssignmentWidgetAsync_OtherTeacher_ReturnsDifferentSet()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (recent, total) = await repo.GetTeacherAssignmentWidgetAsync(2, CancellationToken.None);

        Assert.Equal(1, total);
    }

    [Fact]
    public async Task GetTeacherPendingResultCountAsync_CountsOnlyDraftEntries()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var count = await repo.GetTeacherPendingResultCountAsync(1, CancellationToken.None);

        // Teacher 1 has mark entries: ids 1 (draft), 2 (draft), 3 (submitted), 4 (draft) = 3 draft
        Assert.Equal(3, count);
    }

    [Fact]
    public async Task GetTeacherPendingResultCountAsync_OtherTeacher_ReturnsCorrectCount()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var count = await repo.GetTeacherPendingResultCountAsync(2, CancellationToken.None);

        Assert.Equal(1, count); // Teacher 2 has 1 draft entry (id 5)
    }

    [Fact]
    public async Task GetTeacherLeaveStatusAsync_ReturnsCorrectCounts()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetTeacherLeaveStatusAsync(1, CancellationToken.None);

        Assert.Equal(3, result.TotalLeaves);
        Assert.Equal(1, result.ApprovedLeaves);
        Assert.Equal(1, result.PendingLeaves);
        Assert.Equal(1, result.RejectedLeaves);
    }

    [Fact]
    public async Task GetTeacherLeaveStatusAsync_OtherEmployee_ReturnsDifferentCounts()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetTeacherLeaveStatusAsync(2, CancellationToken.None);

        Assert.Equal(1, result.TotalLeaves);
        Assert.Equal(1, result.ApprovedLeaves);
        Assert.Equal(0, result.PendingLeaves);
        Assert.Equal(0, result.RejectedLeaves);
    }

    [Fact]
    public async Task GetTeacherLeaveStatusAsync_NoLeaves_ReturnsZeros()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetTeacherLeaveStatusAsync(99, CancellationToken.None);

        Assert.Equal(0, result.TotalLeaves);
        Assert.Equal(0, result.ApprovedLeaves);
        Assert.Equal(0, result.PendingLeaves);
        Assert.Equal(0, result.RejectedLeaves);
    }

    [Fact]
    public async Task GetTeacherNotificationWidgetAsync_ReturnsOnlyUserNotifications()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (unreadCount, recent) = await repo.GetTeacherNotificationWidgetAsync(1, CancellationToken.None);

        Assert.Equal(2, unreadCount); // Notifications 1 and 4 are unread for user 1
        Assert.Equal(3, recent.Count);
    }

    [Fact]
    public async Task GetTeacherNotificationWidgetAsync_ExcludesOtherUsers()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (unreadCount, recent) = await repo.GetTeacherNotificationWidgetAsync(2, CancellationToken.None);

        Assert.Single(recent);
    }

    [Fact]
    public async Task GetTeacherNotificationWidgetAsync_Empty_WhenNoUser()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (unreadCount, recent) = await repo.GetTeacherNotificationWidgetAsync(99, CancellationToken.None);

        Assert.Equal(0, unreadCount);
        Assert.Empty(recent);
    }

    [Fact]
    public async Task GetTeacherTimetableAsync_ReturnsCorrectPropertyMapping()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetTeacherTimetableAsync(1, CancellationToken.None);

        var todaySlot = result.FirstOrDefault(s => s.DayOfWeek == DateTime.UtcNow.DayOfWeek.ToString());
        if (todaySlot != null)
        {
            Assert.Equal("Mathematics", todaySlot.SubjectName);
            Assert.Equal("Class 10", todaySlot.ClassName);
            Assert.Equal("A", todaySlot.SectionName);
            Assert.Equal("101", todaySlot.RoomNo);
        }
    }
}
