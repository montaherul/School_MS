using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Library;
using SchoolManagementSystem.Models.Entities.Assignment;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Entities.Notification;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Implementations.Dashboard;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class StudentDashboardWidgetTests
{
    private SchoolDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new SchoolDbContext(options);

        var subjects = new List<Subject>
        {
            new() { Id = 1, Name = "Mathematics", Code = "MATH" }
        };
        db.Subjects.AddRange(subjects);

        var employees = new List<Employee>
        {
            new() { Id = 1, FullName = "Mr. Smith", EmployeeCode = "EMP001", Phone = "1234567890", Email = "smith@school.local", Status = "Active" }
        };
        db.Employees.AddRange(employees);

        var teachers = new List<Teacher>
        {
            new() { Id = 1, EmployeeId = 1, TeacherCode = "TCH001" }
        };
        db.Teachers.AddRange(teachers);

        var routines = new List<TeacherTimetable>
        {
            new() { Id = 1, ClassId = 1, SectionId = 1, SubjectId = 1, TeacherId = 1, DayOfWeek = DateTime.UtcNow.DayOfWeek.ToString(), StartTime = new TimeSpan(8, 0, 0), EndTime = new TimeSpan(9, 0, 0), RoomNo = "101" },
            new() { Id = 2, ClassId = 1, SectionId = 1, SubjectId = 1, TeacherId = 1, DayOfWeek = "Monday", StartTime = new TimeSpan(10, 0, 0), EndTime = new TimeSpan(11, 0, 0) }
        };
        db.Set<TeacherTimetable>().AddRange(routines);

        var assignments = new List<AssignmentTask>
        {
            new() { Id = 1, SchoolClassId = 1, SectionId = 1, SubjectId = 1, TeacherProfileId = 1, Title = "Homework 1", Deadline = DateTime.UtcNow.AddDays(7), Status = AssignmentStatus.Open },
            new() { Id = 2, SchoolClassId = 1, SectionId = 1, SubjectId = 1, TeacherProfileId = 1, Title = "Overdue Task", Deadline = DateTime.UtcNow.AddDays(-7), Status = AssignmentStatus.Open },
            new() { Id = 3, SchoolClassId = 1, SectionId = 1, SubjectId = 1, TeacherProfileId = 1, Title = "Submitted Task", Deadline = DateTime.UtcNow.AddDays(-1), Status = AssignmentStatus.Open }
        };
        db.Assignments.AddRange(assignments);

        var submissions = new List<AssignmentSubmission>
        {
            new() { Id = 1, AssignmentTaskId = 3, StudentId = 1, FilePath = "/test.pdf", SubmittedAt = DateTime.UtcNow.AddDays(-1) }
        };
        db.AssignmentSubmissions.AddRange(submissions);

        var books = new List<Book>
        {
            new() { Id = 1, Title = "C# Programming", Author = "John Doe", AccessionNo = "ACC-001", TotalCopies = 5, AvailableCopies = 4 }
        };
        db.Books.AddRange(books);

        var bookIssues = new List<BookIssue>
        {
            new() { Id = 1, BookId = 1, StudentId = 1, IssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-10)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(10)), FineAmount = 0 },
            new() { Id = 2, BookId = 1, StudentId = 1, IssueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-20)), DueDate = DateOnly.FromDateTime(DateTime.Today.AddDays(-5)), FineAmount = 50, ReturnedDate = DateOnly.FromDateTime(DateTime.Today) }
        };
        db.BookIssues.AddRange(bookIssues);

        var notifications = new List<NotificationMessage>
        {
            new() { Id = 1, UserId = 1, Title = "Fee Due", Body = "Please pay your fees", IsRead = false, Channel = NotificationChannel.InApp, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 2, UserId = 1, Title = "Exam Alert", Body = "Exam starts tomorrow", IsRead = true, Channel = NotificationChannel.InApp, CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new() { Id = 3, UserId = 2, Title = "Other User", IsRead = false, Channel = NotificationChannel.InApp, CreatedAt = DateTime.UtcNow }
        };
        db.Notifications.AddRange(notifications);

        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task GetStudentRoutineWidgetAsync_ReturnsTodayClasses()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetStudentRoutineWidgetAsync(1, 1, null, CancellationToken.None);

        var today = DateTime.UtcNow.DayOfWeek.ToString();
        Assert.NotEmpty(result.TodayClasses);
        Assert.All(result.TodayClasses, c => Assert.Equal(today, c.DayOfWeek));
        Assert.Contains(result.ThisWeekClasses, c => c.DayOfWeek == "Monday" || c.DayOfWeek == today);
    }

    [Fact]
    public async Task GetStudentRoutineWidgetAsync_NextClass_IsFirstTodayClass()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetStudentRoutineWidgetAsync(1, 1, null, CancellationToken.None);

        if (result.TodayClasses.Any())
        {
            Assert.NotNull(result.NextClass);
            Assert.Equal(result.TodayClasses.First().SubjectName, result.NextClass.SubjectName);
        }
    }

    [Fact]
    public async Task GetStudentRoutineWidgetAsync_ReturnsEmpty_WhenNoRoutine()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetStudentRoutineWidgetAsync(999, 999, null, CancellationToken.None);

        Assert.Empty(result.TodayClasses);
        Assert.Empty(result.ThisWeekClasses);
        Assert.Null(result.NextClass);
    }

    [Fact]
    public async Task GetStudentAssignmentWidgetAsync_ReturnsCorrectCounts()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (pending, submitted, overdue, recent) = await repo.GetStudentAssignmentWidgetAsync(1, 1, 1, CancellationToken.None);

        Assert.Equal(1, pending);   // Homework 1 (not submitted, future)
        Assert.Equal(1, submitted); // Submitted Task
        Assert.Equal(1, overdue);   // Overdue Task (not submitted, past)
        Assert.Equal(3, recent.Count);
    }

    [Fact]
    public async Task GetStudentAssignmentWidgetAsync_StudentWithNoSubmissions_AllCountedAsPendingOrOverdue()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (pending, submitted, overdue, recent) = await repo.GetStudentAssignmentWidgetAsync(99, 1, 1, CancellationToken.None);

        Assert.Equal(1, pending);   // Homework 1 (future)
        Assert.Equal(0, submitted);
        Assert.Equal(2, overdue);   // Overdue Task + Submitted Task (not submitted by student 99)
    }

    [Fact]
    public async Task GetStudentLibraryWidgetAsync_ReturnsIssuedBooks()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (books, total) = await repo.GetStudentLibraryWidgetAsync(1, CancellationToken.None);

        Assert.Equal(2, total);
        Assert.Contains(books, b => b.Status == "Issued");
        Assert.Contains(books, b => b.Status == "Returned");
    }

    [Fact]
    public async Task GetStudentLibraryWidgetAsync_Empty_WhenNoBooks()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (books, total) = await repo.GetStudentLibraryWidgetAsync(99, CancellationToken.None);

        Assert.Empty(books);
        Assert.Equal(0, total);
    }

    [Fact]
    public async Task GetStudentNotificationWidgetAsync_ReturnsOnlyUserNotifications()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (unreadCount, recent) = await repo.GetStudentNotificationWidgetAsync(1, CancellationToken.None);

        Assert.Equal(1, unreadCount); // Only the "Fee Due" notification is unread for user 1
        Assert.Equal(2, recent.Count);
        Assert.All(recent, n => Assert.Equal(1, n.Channel)); // NotificationChannel.InApp = 1
    }

    [Fact]
    public async Task GetStudentNotificationWidgetAsync_ZeroUnread_WhenAllRead()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (unreadCount, recent) = await repo.GetStudentNotificationWidgetAsync(2, CancellationToken.None);

        Assert.Equal(1, unreadCount); // User 2 has 1 notification that is unread
        Assert.Single(recent);
    }

    [Fact]
    public async Task GetStudentNotificationWidgetAsync_Empty_WhenNoUser()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var (unreadCount, recent) = await repo.GetStudentNotificationWidgetAsync(99, CancellationToken.None);

        Assert.Equal(0, unreadCount);
        Assert.Empty(recent);
    }
}