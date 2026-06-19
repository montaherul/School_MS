using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Library;
using SchoolManagementSystem.Models.Entities.Notification;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Implementations.Dashboard;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class LibrarianDashboardTests
{
    private SchoolDbContext CreateDbContext()
    {
        var options = new DbContextOptionsBuilder<SchoolDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;
        var db = new SchoolDbContext(options);

        var books = new List<Book>
        {
            new() { Id = 1, Title = "C# Programming", Author = "John Doe", AccessionNo = "ACC-001", TotalCopies = 5, AvailableCopies = 4 },
            new() { Id = 2, Title = "ASP.NET Core", Author = "Jane Smith", AccessionNo = "ACC-002", TotalCopies = 3, AvailableCopies = 2 }
        };
        db.Books.AddRange(books);

        db.SaveChanges();
        return db;
    }

    [Fact]
    public async Task GetLibrarianDashboardDataAsync_ReturnsCorrectCounts()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var pastDate = today.AddDays(-5);
        var futureDate = today.AddDays(5);

        db.BookIssues.AddRange(
            new BookIssue { Id = 1, BookId = 1, StudentId = 1, IssueDate = today, DueDate = futureDate, FineAmount = 0 },
            new BookIssue { Id = 2, BookId = 1, StudentId = 2, IssueDate = today, DueDate = futureDate, FineAmount = 0 },
            new BookIssue { Id = 3, BookId = 2, StudentId = 1, IssueDate = pastDate, DueDate = pastDate, FineAmount = 100, ReturnedDate = today },
            new BookIssue { Id = 4, BookId = 2, StudentId = 3, IssueDate = pastDate, DueDate = pastDate.AddDays(-2), FineAmount = 0 }
        );
        db.SaveChanges();

        var result = await repo.GetLibrarianDashboardDataAsync(CancellationToken.None);

        Assert.Equal(2, result.BooksIssuedToday);
        Assert.Equal(1, result.BooksReturnedToday);
        Assert.Equal(1, result.OverdueBooks);
        Assert.Equal(100, result.TotalFineCollected);
        Assert.Equal(3, result.ActiveMembers);
        Assert.Equal(3, result.PendingReturns);
    }

    [Fact]
    public async Task GetLibrarianDashboardDataAsync_EmptyDashboard_WhenNoLibraryData()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var result = await repo.GetLibrarianDashboardDataAsync(CancellationToken.None);

        Assert.Equal(0, result.BooksIssuedToday);
        Assert.Equal(0, result.BooksReturnedToday);
        Assert.Equal(0, result.OverdueBooks);
        Assert.Equal(0, result.TotalFineCollected);
        Assert.Equal(0, result.ActiveMembers);
        Assert.Equal(0, result.PendingReturns);
        Assert.Empty(result.RecentTransactions);
        Assert.Empty(result.RecentNotifications);
        Assert.Equal(0, result.UnreadNotificationCount);
    }

    [Fact]
    public async Task GetLibrarianDashboardDataAsync_OverdueDetection_WorksCorrectly()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);
        var today = DateOnly.FromDateTime(DateTime.Today);

        db.BookIssues.AddRange(
            new BookIssue { Id = 1, BookId = 1, StudentId = 1, IssueDate = today.AddDays(-10), DueDate = today.AddDays(-2), FineAmount = 0 },
            new BookIssue { Id = 2, BookId = 1, StudentId = 2, IssueDate = today.AddDays(-5), DueDate = today.AddDays(5), FineAmount = 0 },
            new BookIssue { Id = 3, BookId = 2, StudentId = 1, IssueDate = today.AddDays(-20), DueDate = today.AddDays(-10), FineAmount = 50, ReturnedDate = today }
        );
        db.SaveChanges();

        var result = await repo.GetLibrarianDashboardDataAsync(CancellationToken.None);

        Assert.Equal(1, result.OverdueBooks);
        Assert.Equal(2, result.PendingReturns); // Issue 1 (overdue) + Issue 2 (not overdue) both have ReturnedDate=null
        Assert.Equal(50, result.TotalFineCollected);
    }

    [Fact]
    public async Task GetLibrarianDashboardDataAsync_DeletedRecords_AreExcluded()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);
        var today = DateOnly.FromDateTime(DateTime.Today);

        db.BookIssues.AddRange(
            new BookIssue { Id = 1, BookId = 1, StudentId = 1, IssueDate = today, DueDate = today.AddDays(5), FineAmount = 0 },
            new BookIssue { Id = 2, BookId = 1, StudentId = 2, IssueDate = today, DueDate = today.AddDays(5), FineAmount = 0, IsDeleted = true }
        );
        db.SaveChanges();

        var result = await repo.GetLibrarianDashboardDataAsync(CancellationToken.None);

        Assert.Equal(1, result.BooksIssuedToday);
        Assert.Equal(1, result.ActiveMembers);
    }

    [Fact]
    public async Task GetLibrarianDashboardDataAsync_RecentTransactions_ReturnsLast10()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);
        var today = DateOnly.FromDateTime(DateTime.Today);

        for (int i = 1; i <= 15; i++)
        {
            db.BookIssues.Add(new BookIssue
            {
                Id = i,
                BookId = 1,
                StudentId = 1,
                IssueDate = today.AddDays(-i),
                DueDate = today.AddDays(10 - i),
                FineAmount = 0
            });
        }
        db.SaveChanges();

        var result = await repo.GetLibrarianDashboardDataAsync(CancellationToken.None);

        Assert.Equal(10, result.RecentTransactions.Count);
    }

    [Fact]
    public async Task GetLibrarianDashboardDataAsync_Reports_AreCorrect()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);
        var today = DateOnly.FromDateTime(DateTime.Today);
        var monthStart = new DateOnly(today.Year, today.Month, 1);

        db.BookIssues.AddRange(
            new BookIssue { Id = 1, BookId = 1, StudentId = 1, IssueDate = today, DueDate = today.AddDays(5), FineAmount = 0 },
            new BookIssue { Id = 2, BookId = 1, StudentId = 2, IssueDate = today, DueDate = today.AddDays(3), FineAmount = 20, ReturnedDate = today },
            new BookIssue { Id = 3, BookId = 2, StudentId = 3, IssueDate = today.AddDays(-30), DueDate = today.AddDays(-25), FineAmount = 0 },
            new BookIssue { Id = 4, BookId = 2, StudentId = 4, IssueDate = monthStart, DueDate = monthStart.AddDays(5), FineAmount = 15, ReturnedDate = monthStart.AddDays(1) }
        );
        db.SaveChanges();

        var result = await repo.GetLibrarianDashboardDataAsync(CancellationToken.None);

        Assert.Equal(2, result.DailyActivity.Issued); // Issues 1 and 2 both issued today
        Assert.Equal(1, result.DailyActivity.Returned);
        Assert.Equal(20, result.DailyActivity.FinesCollected);

        Assert.True(result.MonthlyActivity.TotalIssued >= 2);
        Assert.True(result.MonthlyActivity.TotalReturned >= 2);
    }

    [Fact]
    public async Task GetLibrarianDashboardDataAsync_Notifications_AreLoaded()
    {
        using var db = CreateDbContext();
        var repo = new DashboardRepository(db);

        var notifications = new List<NotificationMessage>
        {
            new() { Id = 1, Title = "Overdue Alert", Body = "Book is overdue", IsRead = false, Channel = NotificationChannel.InApp, CreatedAt = DateTime.UtcNow.AddDays(-1) },
            new() { Id = 2, Title = "New Book Arrived", Body = "Check new arrivals", IsRead = true, Channel = NotificationChannel.InApp, CreatedAt = DateTime.UtcNow.AddDays(-5) },
            new() { Id = 3, Title = "Library Notice", Body = "Library will close early", IsRead = false, Channel = NotificationChannel.InApp, CreatedAt = DateTime.UtcNow }
        };
        db.Notifications.AddRange(notifications);
        db.SaveChanges();

        var result = await repo.GetLibrarianDashboardDataAsync(CancellationToken.None);

        Assert.Equal(2, result.UnreadNotificationCount);
        Assert.Equal(3, result.RecentNotifications.Count);
    }
}
