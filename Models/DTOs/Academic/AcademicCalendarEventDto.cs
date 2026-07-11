using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Academic;

public class AcademicCalendarEventDto
{
    public int Id { get; set; }

    public int AcademicCalendarId { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = "";

    [Required]
    public string EventType { get; set; } = "";

    [StringLength(500)]
    public string? Description { get; set; }

    [Required]
    public DateTime StartDate { get; set; }

    [Required]
    public DateTime EndDate { get; set; }

    public bool IsRecurringWeekly { get; set; }

    public bool IsActive { get; set; }
}
