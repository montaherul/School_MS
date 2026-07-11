using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace SchoolManagementSystem.Models.DTOs.Academic;

#pragma warning disable CS8618

public class CalendarEventDto
{
    [JsonPropertyName("id")]
    public string Id { get; set; }

    [JsonPropertyName("title")]
    public string Title { get; set; }

    [JsonPropertyName("description")]
    public string Description { get; set; }

    [JsonPropertyName("date")]
    public string Date { get; set; }

    [JsonPropertyName("dayName")]
    public string? DayName { get; set; }

    [JsonPropertyName("isHoliday")]
    public bool IsHoliday { get; set; }

    [JsonPropertyName("isWorkingDay")]
    public bool IsWorkingDay { get; set; }

    [JsonPropertyName("isExamDay")]
    public bool IsExamDay { get; set; }

    [JsonPropertyName("isEventDay")]
    public bool IsEventDay { get; set; }

    [JsonPropertyName("isWebsiteEvent")]
    public bool IsWebsiteEvent { get; set; }

    [JsonPropertyName("remarks")]
    public string? Remarks { get; set; }

    [JsonPropertyName("holidayType")]
    public string? HolidayType { get; set; }

    [JsonPropertyName("venue")]
    public string? Venue { get; set; }

    [JsonPropertyName("source")]
    public string Source { get; set; }

    [JsonPropertyName("sourceId")]
    public int SourceId { get; set; }
}

#pragma warning restore CS8618

public class AcademicCalendarListItemDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsHoliday { get; set; }
    public bool IsWorkingDay { get; set; }
    public bool IsExamDay { get; set; }
    public bool IsEventDay { get; set; }
    public string? Remarks { get; set; }
    public string? HolidayType { get; set; }
    public bool IsActive { get; set; }
    public int TotalRecords { get; set; }
}

public class AcademicCalendarUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int AcademicYearId { get; set; }

    [Required]
    public DateOnly Date { get; set; }

    [Required]
    [StringLength(200)]
    public string Title { get; set; } = string.Empty;

    [StringLength(500)]
    public string Description { get; set; } = string.Empty;

    public bool IsHoliday { get; set; }
    public bool IsWorkingDay { get; set; } = true;
    public bool IsExamDay { get; set; }
    public bool IsEventDay { get; set; }

    [StringLength(500)]
    public string? Remarks { get; set; }

    [StringLength(100)]
    public string? HolidayType { get; set; }

    public bool IsActive { get; set; } = true;
}

public class AgendaItemDto
{
    public string Date { get; set; } = string.Empty;
    public string DayName { get; set; } = string.Empty;
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string? HolidayType { get; set; }
    public string? Venue { get; set; }
    public string Source { get; set; } = string.Empty;
}

public class AcademicCalendarDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public DateOnly Date { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsHoliday { get; set; }
    public bool IsWorkingDay { get; set; }
    public bool IsExamDay { get; set; }
    public bool IsEventDay { get; set; }
    public string? Remarks { get; set; }
    public string? HolidayType { get; set; }
    public bool IsActive { get; set; }
    public string CreatedBy { get; set; } = "system";
    public DateTime CreatedAt { get; set; }
    public string? UpdatedBy { get; set; }
    public DateTime? UpdatedAt { get; set; }
}

public class CalendarPublishedEventDto
{
    public int Id { get; set; }
    public string? Title { get; set; }
    public string? Description { get; set; }
    public DateTime EventDate { get; set; }
    public string? EventLocation { get; set; }
}

public class CalendarExamScheduleDto
{
    public int Id { get; set; }
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string SubjectName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string? StudentGroupName { get; set; }
    public DateOnly ExamDate { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }
    public string? RoomNo { get; set; }
    public string? Instructions { get; set; }
}
