using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Exam;

/// <summary>
    /// Main Exam Entity - Root container for an exam series (e.g., "Half Yearly 2026")
    /// Supports multiple classes, sections, subjects, and components
    /// </summary>
    public class Exam : BaseEntity
    {
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        public ExamTerm Term { get; set; } = ExamTerm.Other;
        public ResultWorkflowStatus Status { get; set; } = ResultWorkflowStatus.Draft;

        public int AcademicYearId { get; set; }

        public DateOnly StartsOn { get; set; }
        public DateOnly EndsOn { get; set; }

        public bool IsLocked { get; set; } = false;
        public DateTime? LockedAt { get; set; }
        public int? LockedByUserId { get; set; }

        public bool IsPublished { get; set; }
        public bool IsArchived { get; set; }
        public DateTime? ArchivedAt { get; set; }
        public int? ArchivedByUserId { get; set; }
        public string? ArchiveReason { get; set; }

        /// <summary>
        /// Legacy property for backward compatibility — maps to first ExamClass
        /// </summary>
        public int ClassId { get; set; }

        /// <summary>
        /// Legacy property for backward compatibility
        /// </summary>
        public int? SectionId { get; set; }

        /// <summary>
        /// Legacy property for backward compatibility
        /// </summary>
        public int? StudentGroupId { get; set; }

        /// <summary>
        /// Computed group key for UI grouping of same-named exams across classes.
        /// Format: "{AcademicYearId}_{NormalizedName}"
        /// Not stored in DB — computed at runtime.
        /// </summary>
        [NotMapped]
        public string ExamGroupKey => GenerateGroupKey(AcademicYearId, Name);

        public static string GenerateGroupKey(int academicYearId, string examName)
        {
            var normalized = (examName ?? "").Trim().ToUpperInvariant();
            var clean = global::System.Text.RegularExpressions.Regex.Replace(normalized, @"[\s\-]+", "_");
            return $"{academicYearId}_{clean}";
        }

        // Navigation
        public virtual Academic.SchoolClass Class { get; set; } = null!;
        public virtual Academic.Section? Section { get; set; }
        public virtual Academic.StudentGroup? StudentGroup { get; set; }
        public virtual ICollection<ExamClass> Classes { get; set; } = [];
        public virtual ICollection<ExamSubject> ExamSubjects { get; set; } = [];
        public virtual ICollection<ExamSchedule> ExamSchedules { get; set; } = [];
        public virtual ICollection<ExamTemplate> Templates { get; set; } = [];
    }

/// <summary>
/// Links Exam to Classes. One Exam can contain multiple classes.
/// </summary>
public class ExamClass : BaseEntity
{
    public int ExamId { get; set; }
    public int ClassId { get; set; }

    // Historical snapshot
    [MaxLength(100)]
    public string ClassName { get; set; } = "";
    public int SortOrder { get; set; }

    // Navigation
    public virtual Exam Exam { get; set; } = null!;
    public virtual Academic.SchoolClass Class { get; set; } = null!;
    public virtual ICollection<ExamSection> Sections { get; set; } = new List<ExamSection>();
    public virtual ICollection<ExamSubject> Subjects { get; set; } = new List<ExamSubject>();
}

/// <summary>
/// Sections within a class for the exam. Class 6 has sections A, B, C.
/// </summary>
public class ExamSection : BaseEntity
{
    public int ExamClassId { get; set; }
    public int SectionId { get; set; }

    // Historical snapshot
    [MaxLength(50)]
    public string SectionName { get; set; } = "";

    // Navigation
    public virtual ExamClass ExamClass { get; set; } = null!;
    public virtual Academic.Section Section { get; set; } = null!;
}

/// <summary>
/// Subjects within a class for the exam. Loaded from ClassSubject.
/// </summary>
public class ExamSubject : BaseEntity
{
    public int ExamId { get; set; }
    public int SubjectId { get; set; }
    public int ClassId { get; set; }
    public int? SectionId { get; set; }
    public int? StudentGroupId { get; set; }

    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
    public bool IsOptional { get; set; } = false;
    public bool IsReligionSubject { get; set; } = false;

    public int? TeacherId { get; set; }
    public DateOnly? ExamDate { get; set; }
    public TimeOnly? ExamStartTime { get; set; }
    public int? ExamDuration { get; set; }
    [MaxLength(50)]
    public string? RoomNumber { get; set; }
    public bool IsActive { get; set; } = true;

    // Historical snapshot — preserves data at exam creation time
    [MaxLength(100)]
    public string SubjectName { get; set; } = "";
    [MaxLength(50)]
    public string SubjectCode { get; set; } = "";
    [MaxLength(50)]
    public string SubjectType { get; set; } = ""; // Core, Elective, Vocational, Religion
    [MaxLength(50)]
    public string SubjectGroup { get; set; } = ""; // Science, BusinessStudies, Humanities, General
    public decimal TheoryMarks { get; set; } = 100;
    public decimal PracticalMarks { get; set; } = 0;
    [MaxLength(100)]
    public string? TeacherName { get; set; }
    [MaxLength(50)]
    public string? TeacherEmployeeCode { get; set; }
    public decimal Credit { get; set; }
    [MaxLength(20)]
    public string? NCTBCode { get; set; }

    // Navigation
    public virtual Exam Exam { get; set; } = null!;
    public virtual Academic.Subject Subject { get; set; } = null!;
    public virtual Teachers.Teacher? Teacher { get; set; }
    public virtual Academic.SchoolClass Class { get; set; } = null!;
    public virtual Academic.StudentGroup? StudentGroup { get; set; }
    public virtual ICollection<ExamSubjectComponent> Components { get; set; } = new List<ExamSubjectComponent>();
}

/// <summary>
/// Components per subject. Loaded from SubjectMarkStructure.
/// Each subject gets MCQ, CQ, Practical, etc. with their marks.
/// </summary>
public class ExamSubjectComponent : BaseEntity
{
    public int ExamSubjectId { get; set; }
    public int ComponentId { get; set; }
    public decimal MaxMarks { get; set; }
    public decimal PassMarks { get; set; }
    public int DisplayOrder { get; set; }

    // Historical snapshot
    [MaxLength(100)]
    public string ComponentName { get; set; } = "";
    [MaxLength(50)]
    public string ComponentCode { get; set; } = "";
    public decimal Weight { get; set; }

    // Navigation
    public virtual ExamSubject ExamSubject { get; set; } = null!;
    public virtual ExamComponent Component { get; set; } = null!;
}

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

    // Historical snapshot — preserves room info at scheduling time
    [MaxLength(100)]
    public string RoomName { get; set; } = "";
    [MaxLength(100)]
    public string BuildingName { get; set; } = "";
    [MaxLength(50)]
    public string ShiftName { get; set; } = "";

    [MaxLength(500)]
    public string Instructions { get; set; } = string.Empty;

    // Navigation
    public virtual Exam Exam { get; set; } = null!;
    public virtual Academic.Subject Subject { get; set; } = null!;
    public virtual Academic.SchoolClass Class { get; set; } = null!;
    public virtual Academic.StudentGroup? StudentGroup { get; set; }
    public virtual Academic.Section? Section { get; set; }
}

/// <summary>
/// Saved exam wizard template for reusing subject/component/mark configurations.
/// TemplateData stores the full ExamWizardStateDto as JSON.
/// </summary>
public class ExamTemplate : BaseEntity
{
    [MaxLength(200)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public int AcademicYearId { get; set; }

    public ExamTerm Term { get; set; }

    [MaxLength(50)]
    public string ExamType { get; set; } = string.Empty;

    /// <summary>JSON-serialized list of class IDs.</summary>
    public string ClassIdsJson { get; set; } = "[]";

    /// <summary>JSON-serialized list of ExamWizardSubjectDto.</summary>
    public string TemplateDataJson { get; set; } = "[]";

    public bool IsActive { get; set; } = true;
}

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