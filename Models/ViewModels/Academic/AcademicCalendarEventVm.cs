using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Enums;

public class AcademicCalendarEventVm
{
    public int Id { get; set; }

    public int AcademicCalendarId { get; set; }

    [Required]
    public string Title { get; set; } = "";

    public string? Description { get; set; }

    [Required]
    public AcademicEventType EventType { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public bool IsRecurringWeekly { get; set; }

    public bool IsActive { get; set; } = true;
}