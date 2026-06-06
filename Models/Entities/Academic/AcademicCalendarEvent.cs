using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Academic;

public class AcademicCalendarEvent : BaseEntity
{
    public int AcademicCalendarId { get; set; }
    public AcademicCalendar? AcademicCalendar { get; set; }

    [MaxLength(200)]
    public string Title { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public AcademicEventType EventType { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public bool IsRecurringWeekly { get; set; }

    public bool IsActive { get; set; } = true;
}