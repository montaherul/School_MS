using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Library;

public class Book : BaseEntity
{
    [MaxLength(30)]
    public string AccessionNo { get; set; } = string.Empty;

    [MaxLength(160)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(120)]
    public string Author { get; set; } = string.Empty;

    public int TotalCopies { get; set; }
    public int AvailableCopies { get; set; }
}

public class BookIssue : BaseEntity
{
    public int BookId { get; set; }
    public int StudentId { get; set; }
    public DateOnly IssueDate { get; set; }
    public DateOnly DueDate { get; set; }
    public DateOnly? ReturnedDate { get; set; }
    public decimal FineAmount { get; set; }
}

public class BookReservation : BaseEntity
{
    public int BookId { get; set; }
    public int StudentId { get; set; }
    public DateTime ReservedAt { get; set; } = DateTime.UtcNow;
}
