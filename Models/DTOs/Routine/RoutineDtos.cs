using System.ComponentModel.DataAnnotations;

namespace SchoolManagementSystem.Models.DTOs.Routine;

public class RoutinePeriodListItemDto
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string StartTime { get; set; } = string.Empty;
    public string EndTime { get; set; } = string.Empty;
    public int PeriodNumber { get; set; }
    public bool IsBreak { get; set; }
    public bool IsActive { get; set; }
}

public class RoutinePeriodUpsertDto : IValidatableObject
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string Name { get; set; } = string.Empty;

    [Required]
    public TimeSpan StartTime { get; set; }

    [Required]
    public TimeSpan EndTime { get; set; }

    public int PeriodNumber { get; set; }
    public bool IsBreak { get; set; }
    public bool IsActive { get; set; } = true;

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        if (StartTime >= EndTime)
            yield return new ValidationResult("Start time must be before end time.", new[] { nameof(EndTime) });
    }
}

public class RoomListItemDto
{
    public int Id { get; set; }
    public string RoomNo { get; set; } = string.Empty;
    public string? Name { get; set; }
    public int Capacity { get; set; }
    public string? Building { get; set; }
    public int Floor { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public bool IsLab { get; set; }
    public bool RequiresDoublePeriod { get; set; }
    public bool IsActive { get; set; }
}

public class RoomUpsertDto
{
    public int Id { get; set; }

    [Required]
    [StringLength(50)]
    public string RoomNo { get; set; } = string.Empty;

    [StringLength(100)]
    public string? Name { get; set; }

    public int Capacity { get; set; }

    [StringLength(50)]
    public string? Building { get; set; }

    public int Floor { get; set; }

    [StringLength(50)]
    public string RoomType { get; set; } = "Classroom";

    public bool IsLab { get; set; }
    public bool RequiresDoublePeriod { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SubjectRequirementListItemDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int PeriodsPerWeek { get; set; }
    public bool RequiresLab { get; set; }
    public bool RequiresDoublePeriod { get; set; }
    public int Priority { get; set; }
    public int MaxConsecutive { get; set; }
}

public class SubjectRequirementUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int AcademicYearId { get; set; }

    [Required]
    public int ClassId { get; set; }

    public int? SectionId { get; set; }
    public int? GroupId { get; set; }

    [Required]
    public int SubjectId { get; set; }

    [Required]
    public int TeacherId { get; set; }

    public int PeriodsPerWeek { get; set; }
    public bool RequiresLab { get; set; }
    public bool RequiresDoublePeriod { get; set; }
    public int Priority { get; set; }
    public int MaxConsecutive { get; set; } = 2;
}

public class WorkingDayListItemDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string DayName { get; set; } = string.Empty;
    public int DayNumber { get; set; }
    public bool IsWorkingDay { get; set; }
}

public class WorkingDayUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int AcademicYearId { get; set; }

    [Required]
    [StringLength(20)]
    public string DayName { get; set; } = string.Empty;

    public int DayNumber { get; set; }
    public bool IsWorkingDay { get; set; } = true;
}

public class TeacherAvailabilityListItemDto
{
    public int Id { get; set; }
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int RoutinePeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public int DayNumber { get; set; }
    public string DayName { get; set; } = string.Empty;
    public bool IsAvailable { get; set; }
}

public class TeacherAvailabilityUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int TeacherId { get; set; }

    [Required]
    public int RoutinePeriodId { get; set; }

    public int DayNumber { get; set; }
    public bool IsAvailable { get; set; } = true;
}

public class RoutineEntryListItemDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int? SectionId { get; set; }
    public string? SectionName { get; set; }
    public int? GroupId { get; set; }
    public string? GroupName { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomNo { get; set; } = string.Empty;
    public int RoutinePeriodId { get; set; }
    public string PeriodName { get; set; } = string.Empty;
    public int DayNumber { get; set; }
    public string DayName { get; set; } = string.Empty;
    public bool IsLab { get; set; }
    public string? Note { get; set; }
}

public class RoutineEntryUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int AcademicYearId { get; set; }

    [Required]
    public int ClassId { get; set; }

    public int? SectionId { get; set; }
    public int? GroupId { get; set; }

    [Required]
    public int SubjectId { get; set; }

    [Required]
    public int TeacherId { get; set; }

    [Required]
    public int RoomId { get; set; }

    [Required]
    public int RoutinePeriodId { get; set; }

    public int DayNumber { get; set; }
    public bool IsLab { get; set; }

    [StringLength(500)]
    public string? Note { get; set; }
}

public class RoutineGenerationListItemDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? StartedAt { get; set; }
    public string? CompletedAt { get; set; }
    public int TotalAssignments { get; set; }
    public int SuccessfulAssignments { get; set; }
    public int FailedAssignments { get; set; }
    public int ConflictsDetected { get; set; }
    public string? ErrorMessage { get; set; }
}

public class RoutineConflictListItemDto
{
    public int Id { get; set; }
    public int? GenerationId { get; set; }
    public string ConflictType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public string? TeacherName { get; set; }
    public int? RoomId { get; set; }
    public string? RoomNo { get; set; }
    public int? SubjectId { get; set; }
    public string? SubjectName { get; set; }
    public int? ClassId { get; set; }
    public string? ClassName { get; set; }
    public int? RoutinePeriodId { get; set; }
    public string? PeriodName { get; set; }
    public int? DayNumber { get; set; }
    public string? DayName { get; set; }
    public bool IsResolved { get; set; }
}

public class RoutineVersionListItemDto
{
    public int Id { get; set; }
    public int AcademicYearId { get; set; }
    public string AcademicYearName { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int EntryCount { get; set; }
    public string? PublishedAt { get; set; }
    public string? ApprovedAt { get; set; }
}

public class RoutineVersionUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int AcademicYearId { get; set; }

    [Required]
    [StringLength(100)]
    public string Name { get; set; } = string.Empty;

    [StringLength(20)]
    public string Status { get; set; } = "Draft";

    public int EntryCount { get; set; }
}

public class RoutineDashboardDto
{
    public int TotalTeachers { get; set; }
    public int TotalRooms { get; set; }
    public int TotalClasses { get; set; }
    public int TotalSubjects { get; set; }
    public int TotalEntries { get; set; }
    public int TotalConflicts { get; set; }
    public int? LastGenerationId { get; set; }
    public string LastGenerationStatus { get; set; } = string.Empty;
    public int? PublishedVersionId { get; set; }
    public string? PublishedVersionName { get; set; }
    public List<TeacherLoadDto>? TeacherLoadSummary { get; set; }
    public List<RoomUtilizationDto>? RoomUtilization { get; set; }
}

public class TeacherLoadDto
{
    public int TeacherId { get; set; }
    public string TeacherName { get; set; } = string.Empty;
    public int TotalPeriodsPerWeek { get; set; }
    public Dictionary<int, int> WeeklyPeriodsByDay { get; set; } = new();
    public int TotalClasses { get; set; }
    public int TotalSubjects { get; set; }
    public double UtilizationPercent { get; set; }
    public int MaxPeriodsPerDay { get; set; }
    public int WorkingDays { get; set; }
    public double AveragePerDay { get; set; }
}

public class RoomUtilizationDto
{
    public int RoomId { get; set; }
    public string RoomNo { get; set; } = string.Empty;
    public string? Building { get; set; }
    public int Capacity { get; set; }
    public int TotalSlotsPerWeek { get; set; }
    public int UsedSlots { get; set; }
    public double UtilizationPercent { get; set; }
    public int? PeakDay { get; set; }
    public int PeakPeriodCount { get; set; }
}

public class RoutinePrintViewModel
{
    public List<RoutineEntryListItemDto> Entries { get; set; } = new();
    public List<RoutinePeriodListItemDto> Periods { get; set; } = new();
}

public class SubstituteAssignmentListItemDto
{
    public int Id { get; set; }
    public int RoutineEntryId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public string ClassName { get; set; } = string.Empty;
    public string OriginalTeacherName { get; set; } = string.Empty;
    public string SubstituteTeacherName { get; set; } = string.Empty;
    public string AssignedByName { get; set; } = string.Empty;
    public DateTime AssignmentDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
    public DateTime? ApprovedAt { get; set; }
}

public class SubstituteAssignmentUpsertDto
{
    public int Id { get; set; }

    [Required]
    public int RoutineEntryId { get; set; }

    [Required]
    public int SubstituteTeacherId { get; set; }

    public DateTime? EffectiveDate { get; set; }

    [StringLength(500)]
    public string? Reason { get; set; }

    [StringLength(500)]
    public string? Notes { get; set; }
}
