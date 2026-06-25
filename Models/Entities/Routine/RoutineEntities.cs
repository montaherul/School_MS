using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Base;

namespace SchoolManagementSystem.Models.Entities.Routine;

public class RoutinePeriod : BaseEntity
{
    [MaxLength(50)] public string Name { get; set; } = string.Empty;
    public TimeSpan StartTime { get; set; }
    public TimeSpan EndTime { get; set; }
    public int PeriodNumber { get; set; }
    public bool IsBreak { get; set; }
    public bool IsActive { get; set; } = true;
}

public class Room : BaseEntity
{
    [MaxLength(50)] public string RoomNo { get; set; } = string.Empty;
    [MaxLength(100)] public string? Name { get; set; }
    public int Capacity { get; set; }
    [MaxLength(50)] public string? Building { get; set; }
    public int Floor { get; set; }
    [MaxLength(50)] public string RoomType { get; set; } = "Classroom";
    public bool IsLab { get; set; }
    public bool RequiresDoublePeriod { get; set; }
    public bool IsActive { get; set; } = true;
}

public class SubjectRequirement : BaseEntity
{
    public int AcademicYearId { get; set; }
    public int ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? GroupId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public int PeriodsPerWeek { get; set; }
    public bool RequiresLab { get; set; }
    public bool RequiresDoublePeriod { get; set; }
    public int Priority { get; set; }
    public int MaxConsecutive { get; set; } = 2;

    public AcademicYear? AcademicYear { get; set; }
    public SchoolClass? Class { get; set; }
    public Section? Section { get; set; }
    public StudentGroup? Group { get; set; }
    public Subject? Subject { get; set; }
    public Teacher? Teacher { get; set; }
}

public class RoutineEntry : BaseEntity
{
    public int AcademicYearId { get; set; }
    public int ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? GroupId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public int RoomId { get; set; }
    public int RoutinePeriodId { get; set; }
    public int DayNumber { get; set; }
    public bool IsLab { get; set; }
    public int? GenerationId { get; set; }
    public int? VersionId { get; set; }
    [MaxLength(500)] public string? Note { get; set; }

    public AcademicYear? AcademicYear { get; set; }
    public SchoolClass? Class { get; set; }
    public Section? Section { get; set; }
    public StudentGroup? Group { get; set; }
    public Subject? Subject { get; set; }
    public Teacher? Teacher { get; set; }
    public Room? Room { get; set; }
    public RoutinePeriod? RoutinePeriod { get; set; }
}

public class WorkingDay : BaseEntity
{
    public int AcademicYearId { get; set; }
    [MaxLength(20)] public string DayName { get; set; } = string.Empty;
    public int DayNumber { get; set; }
    public bool IsWorkingDay { get; set; } = true;
}

public class TeacherAvailability : BaseEntity
{
    public int TeacherId { get; set; }
    public int RoutinePeriodId { get; set; }
    public int DayNumber { get; set; }
    public bool IsAvailable { get; set; } = true;

    public Teacher? Teacher { get; set; }
    public RoutinePeriod? RoutinePeriod { get; set; }
}

public class RoutineGeneration : BaseEntity
{
    public int AcademicYearId { get; set; }
    [MaxLength(50)] public string Status { get; set; } = "Pending";
    public DateTime? StartedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public int TotalAssignments { get; set; }
    public int SuccessfulAssignments { get; set; }
    public int FailedAssignments { get; set; }
    public int ConflictsDetected { get; set; }
    [MaxLength(4000)] public string? ErrorMessage { get; set; }
}

public class RoutineConflict : BaseEntity
{
    public int? GenerationId { get; set; }
    [MaxLength(50)] public string ConflictType { get; set; } = string.Empty;
    [MaxLength(500)] public string Description { get; set; } = string.Empty;
    public int? TeacherId { get; set; }
    public int? RoomId { get; set; }
    public int? SubjectId { get; set; }
    public int? ClassId { get; set; }
    public int? RoutinePeriodId { get; set; }
    public int? DayNumber { get; set; }
    public bool IsResolved { get; set; }

    public Teacher? Teacher { get; set; }
    public Room? Room { get; set; }
    public Subject? Subject { get; set; }
    public SchoolClass? Class { get; set; }
    public RoutinePeriod? RoutinePeriod { get; set; }
}

public class RoutineVersion : BaseEntity
{
    public int AcademicYearId { get; set; }
    [MaxLength(100)] public string Name { get; set; } = string.Empty;
    [MaxLength(20)] public string Status { get; set; } = "Draft";
    public DateTime? PublishedAt { get; set; }
    public DateTime? ApprovedAt { get; set; }
    public int EntryCount { get; set; }

    public AcademicYear? AcademicYear { get; set; }
}

public class SubstituteAssignment : BaseEntity
{
    public int RoutineEntryId { get; set; }
    public int OriginalTeacherId { get; set; }
    public int SubstituteTeacherId { get; set; }
    public int AssignedById { get; set; }
    public DateTime AssignmentDate { get; set; }
    public DateTime? EffectiveDate { get; set; }
    public int? PeriodNumber { get; set; }
    public int? DayNumber { get; set; }
    [MaxLength(50)] public string Status { get; set; } = "Pending";
    [MaxLength(500)] public string? Reason { get; set; }
    public DateTime? ApprovedAt { get; set; }
    [MaxLength(500)] public string? Notes { get; set; }

    public RoutineEntry? RoutineEntry { get; set; }
    [global::System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(OriginalTeacherId))]
    public Teacher? OriginalTeacher { get; set; }
    [global::System.ComponentModel.DataAnnotations.Schema.ForeignKey(nameof(SubstituteTeacherId))]
    public Teacher? SubstituteTeacher { get; set; }
    public ApplicationUser? AssignedBy { get; set; }
}
