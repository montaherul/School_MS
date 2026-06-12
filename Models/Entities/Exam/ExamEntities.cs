using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Exam;

/// <summary>
/// Exam Types: Half Yearly, Annual, Final, Pre-Test, Test, First Terminal, Second Terminal
/// </summary>
public class ExamType : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Description { get; set; } = string.Empty;

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ICollection<ExamConfiguration> ExamConfigurations { get; set; } = [];
}

/// <summary>
/// Exam Configuration: Defines which exams belong to which class group
/// E.g., Class 1-2 has 3 exams (First Terminal, Second Terminal, Final)
/// Class 6-9 has 2 exams (Half Yearly, Annual)
/// Class 10 has 2 exams (Pre-Test, Test)
/// </summary>
public class ExamConfiguration : BaseEntity
{
    public int ExamTypeId { get; set; }
    public int ClassId { get; set; }
    public int? StudentGroupId { get; set; }

    [MaxLength(100)]
    public string DisplayName { get; set; } = string.Empty;

    public decimal ExamWeightage { get; set; } = 100; // Percentage weight in final calculation
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ExamType ExamType { get; set; } = null!;
    public virtual SchoolClass Class { get; set; } = null!;
}

/// <summary>
/// System-wide master catalog of exam component types (Written, MCQ, Practical, Viva, etc.)
/// Used by SubjectMarkStructure to define per-subject mark distributions.
/// Replaces hardcoded component columns with a dynamic, extensible registry.
/// </summary>
public class ExamComponent : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty; // "Written", "MCQ", "Practical", etc.

    [MaxLength(50)]
    public string Code { get; set; } = string.Empty; // "WRITTEN", "MCQ", "PRACTICAL"

    [MaxLength(500)]
    public string? Description { get; set; }

    public int DisplayOrder { get; set; } = 0;

    /// <summary>Default maximum marks for this component type (overridable per-subject).</summary>
    public decimal DefaultFullMarks { get; set; } = 100;

    /// <summary>Default pass marks for this component type (overridable per-subject).</summary>
    public decimal DefaultPassMarks { get; set; } = 33;

    /// <summary>Whether this is a practical component (affects grading rules).</summary>
    public bool IsPractical { get; set; } = false;

    /// <summary>Whether this component is optional for subjects.</summary>
    public bool IsOptional { get; set; } = false;

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Subject Mark Structure: Maps ExamComponent instances to subjects with FullMarks/PassMarks.
/// Supports exam-specific overrides, class-level defaults, and group-specific configurations.
/// This is the dynamic replacement for hardcoded component columns on ExamSubject.
/// </summary>
public class SubjectMarkStructure : BaseEntity
{
    public int ComponentId { get; set; } // FK → ExamComponent

    public int? ClassId { get; set; } // null = applies to all classes
    public int? SubjectId { get; set; } // null = applies to all subjects
    public int? StudentGroupId { get; set; } // null = applies to all groups

    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ExamComponent Component { get; set; } = null!;
    public virtual Academic.SchoolClass? Class { get; set; }
    public virtual Academic.Subject? Subject { get; set; }
    public virtual Academic.StudentGroup? StudentGroup { get; set; }
}

/// <summary>
/// Main Exam Entity
/// </summary>
public class Exam : BaseEntity
{
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public ExamTerm Term { get; set; } = ExamTerm.Other;
    public ResultWorkflowStatus Status { get; set; } = ResultWorkflowStatus.Draft;

    public int AcademicYearId { get; set; }

    /// <summary>Required: the primary class this exam belongs to.</summary>
    public int ClassId { get; set; }

    /// <summary>Nullable: section filter for this exam.</summary>
    public int? SectionId { get; set; }

    public int? StudentGroupId { get; set; }
    public DateOnly StartsOn { get; set; }
    public DateOnly EndsOn { get; set; }

    public bool IsLocked { get; set; } = false;
    public DateTime? LockedAt { get; set; }
    public int? LockedByUserId { get; set; }

    // Navigation
    public virtual ICollection<ExamSubject> ExamSubjects { get; set; } = [];
    public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = [];
    public virtual Academic.SchoolClass Class { get; set; } = null!;
    public virtual Academic.Section? Section { get; set; }
    public virtual Academic.StudentGroup? StudentGroup { get; set; }
}

/// <summary>
/// Exam Subject: Maps subjects to exams with marks configuration
/// </summary>
public class ExamSubject : BaseEntity
{
    public int ExamId { get; set; }
    public int SubjectId { get; set; }

    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
    public bool IsOptional { get; set; } = false;

    public int? TeacherId { get; set; }
    public int TotalWrittenMarks { get; set; }
    public int TotalMCQMarks { get; set; }
    public int TotalPracticalMarks { get; set; }
    public int TotalVivaMarks { get; set; }
    public int TotalAssignmentMarks { get; set; }
    public DateOnly? ExamDate { get; set; }
    public TimeOnly? ExamStartTime { get; set; }
    public int? ExamDuration { get; set; }
    [MaxLength(50)]
    public string? RoomNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Exam Exam { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual Teachers.Teacher? Teacher { get; set; }
}

/// <summary>
/// Exam Schedule: Date, time, and room assignment for each subject's exam
/// Supports class/group/section context for independent group scheduling (SSC routine).
/// </summary>
public class ExamSchedule : BaseEntity
{
    public int ExamId { get; set; }
    public int SubjectId { get; set; }

    /// <summary>Required: the class this schedule entry belongs to.</summary>
    public int ClassId { get; set; }

    /// <summary>Nullable: for group-based classes (9-10), identifies Science/BusinessStudies/Humanities.</summary>
    public int? StudentGroupId { get; set; }

    /// <summary>Nullable: for section-specific scheduling.</summary>
    public int? SectionId { get; set; }

    public DateOnly ExamDate { get; set; }
    public TimeOnly StartsAt { get; set; }
    public TimeOnly EndsAt { get; set; }

    [MaxLength(80)]
    public string RoomNo { get; set; } = string.Empty;

    [MaxLength(500)]
    public string Instructions { get; set; } = string.Empty;

    // Navigation
    public virtual Exam Exam { get; set; } = null!;
    public virtual Subject Subject { get; set; } = null!;
    public virtual SchoolClass Class { get; set; } = null!;
    public virtual StudentGroup? StudentGroup { get; set; }
    public virtual Section? Section { get; set; }
}

/// <summary>
/// Admit Card: Generated for each student for each exam
/// </summary>
public class AdmitCard : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }

    [MaxLength(40)]
    public string CardNo { get; set; } = string.Empty;

    [MaxLength(50)]
    public string? AdmitCardNumber { get; set; }

    public int? RollNumber { get; set; }

    [MaxLength(20)]
    public string? SeatNumber { get; set; }

    public bool IsIssued { get; set; } = false;
    public DateTime? IssuedAt { get; set; }
    public DateTime? PrintedAt { get; set; }
    public bool IsGenerated { get; set; } = false;

    // Navigation
    public virtual Exam Exam { get; set; } = null!;
    public virtual Student.Student Student { get; set; } = null!;
}


