namespace SchoolManagementSystem.Models.ViewModels.Dashboard;

public class LibrarianDashboardViewModel
{
    public string LibrarianName { get; set; } = string.Empty;
    public string Designation { get; set; } = string.Empty;

    public int BooksIssuedToday { get; set; }
    public int BooksReturnedToday { get; set; }
    public int OverdueBooks { get; set; }
    public decimal TotalFineCollected { get; set; }
    public int ActiveMembers { get; set; }
    public int PendingReturns { get; set; }

    public List<LibrarianTransactionDto> RecentTransactions { get; set; } = new();

    public int UnreadNotificationCount { get; set; }
    public List<LibrarianNotificationDto> RecentNotifications { get; set; } = new();

    public DailyActivityReport DailyActivity { get; set; } = new();
    public MonthlyActivityReport MonthlyActivity { get; set; } = new();
}

public class LibrarianTransactionDto
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string StudentName { get; set; } = string.Empty;
    public string StudentNo { get; set; } = string.Empty;
    public DateOnly IssueDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnedDate { get; set; }
    public decimal FineAmount { get; set; }
    public string Status { get; set; } = string.Empty;
}

public class LibrarianNotificationDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public bool IsRead { get; set; }
    public DateTime? SentAt { get; set; }
}

public class DailyActivityReport
{
    public int Issued { get; set; }
    public int Returned { get; set; }
    public decimal FinesCollected { get; set; }
}

public class MonthlyActivityReport
{
    public int TotalIssued { get; set; }
    public int TotalReturned { get; set; }
    public decimal TotalFinesCollected { get; set; }
    public int TotalOverdue { get; set; }
}
