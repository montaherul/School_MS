namespace SchoolManagementSystem.Models.DTOs.Dashboard;

public class StudentInvoiceDto
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal PaidAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LateFee { get; set; }
    public int Status { get; set; }
    public int TotalRecords { get; set; }
    public decimal DueAmount => TotalAmount + LateFee - PaidAmount - DiscountAmount;
}

public class StudentPaymentDto
{
    public int Id { get; set; }
    public DateTime PaymentDate { get; set; }
    public int Method { get; set; }
    public string? ReferenceNo { get; set; }
    public decimal Amount { get; set; }
    public decimal LateFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
}

public class StudentLedgerEntryDto
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public int Type { get; set; }
    public string? Description { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
    public int TotalRecords { get; set; }
}

public class StudentAssignmentDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Instructions { get; set; }
    public DateTime Deadline { get; set; }
    public int AssignmentStatus { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string TeacherName { get; set; } = string.Empty;
    public bool IsSubmitted { get; set; }
    public DateTime? SubmittedAt { get; set; }
    public decimal? Marks { get; set; }
    public string? Feedback { get; set; }
    public int TotalRecords { get; set; }
}

public class StudentLibraryBookDto
{
    public int Id { get; set; }
    public string BookTitle { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public string AccessionNo { get; set; } = string.Empty;
    public DateOnly IssueDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnedDate { get; set; }
    public decimal FineAmount { get; set; }
    public string Status { get; set; } = string.Empty;
    public int TotalRecords { get; set; }
}

public class StudentNotificationItemDto
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string? Body { get; set; }
    public int Channel { get; set; }
    public bool IsRead { get; set; }
    public DateTime? SentAt { get; set; }
    public DateTime CreatedAt { get; set; }
    public int TotalRecords { get; set; }
}

public class PagedResponse<T>
{
    public List<T> Data { get; set; } = new();
    public int TotalRecords { get; set; }
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
    public int LastPage => (int)Math.Ceiling((double)TotalRecords / Math.Max(PageSize, 1));
}