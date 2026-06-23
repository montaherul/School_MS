using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SchoolManagementSystem.Models.Entities.Base;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Models.Entities.Result;

/// <summary>
/// Mark Entry: Teacher enters component-wise marks for students in an exam
/// Supports draft/submit workflow with proper state management
/// </summary>
public class MarkEntry : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }

    // Denormalized fields for query performance (avoid JOINs)
    public int AcademicYearId { get; set; }
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public int? StudentGroupId { get; set; }

    /// <summary>
    /// Component-wise marks entry
    /// </summary>
    public decimal? WrittenMarks { get; set; }
    public decimal? MCQMarks { get; set; }
    public decimal? CQMarks { get; set; }
    public decimal? PracticalMarks { get; set; }
    public decimal? VivaMarks { get; set; }
    public decimal? LabMarks { get; set; }
    public decimal? OralMarks { get; set; }
    public decimal? AssignmentMarks { get; set; }
    public decimal? ContinuousAssessmentMarks { get; set; }

    /// <summary>
    /// For Primary classes: Competency-based marks
    /// </summary>
    public decimal? CompetencyMarks { get; set; }
    public decimal? BehaviourMarks { get; set; }
    public decimal? ParticipationMarks { get; set; }

    /// <summary>
    /// Total obtained marks (calculated from components)
    /// </summary>
    public decimal MarksObtained { get; set; } = 0;

    [MaxLength(10)]
    public string? Grade { get; set; }
    public decimal? GradePoint { get; set; }

    public int EnteredByTeacherId { get; set; }
    public ResultWorkflowStatus Status { get; set; } = ResultWorkflowStatus.Draft;
    public bool IsLocked { get; set; } = false;
    public DateTime? LockedAt { get; set; }
    public DateTime? SubmittedAt { get; set; }

    // Navigation Properties
    public virtual Exam.Exam Exam { get; set; } = null!;
    public virtual Student.Student Student { get; set; } = null!;
    public virtual Academic.Subject Subject { get; set; } = null!;
    public virtual Teachers.Teacher EnteredByTeacher { get; set; } = null!;

    public int? CreatedByUserId { get; set; }
    public int? UpdatedByUserId { get; set; }

    /// <summary>
    /// JSON column for dynamic component values beyond the 12 standard fields.
    /// Format: {"COMPONENT_CODE": marks}
    /// Used with SubjectMarkStructure for unlimited dynamic component support.
    /// </summary>
    public string? ComponentValues { get; set; }

}

/// <summary>
/// GPA Configuration / Grading Rule: Bangladesh grading system
/// 80-100 = A+ = 5.00, 70-79 = A = 4.00, 60-69 = A- = 3.50, 50-59 = B = 3.00,
/// 40-49 = C = 2.00, 33-39 = D = 1.00, 0-32 = F = 0.00
/// </summary>
public class GradingRule : BaseEntity
{
    [MaxLength(10)]
    public string Grade { get; set; } = string.Empty;

    public decimal MinMarks { get; set; } = 0;
    public decimal MaxMarks { get; set; } = 100;
    public decimal GradePoint { get; set; } = 0;

    [MaxLength(50)]
    public string Description { get; set; } = string.Empty;

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Result Publication: Controls when exam results are published
/// One per exam, tracks status and approval workflow
/// </summary>
public class ResultPublication : BaseEntity
{
    public int ExamId { get; set; }

    // Denormalized for dashboard queries
    public int AcademicYearId { get; set; }

    public ResultWorkflowStatus Status { get; set; } = ResultWorkflowStatus.Submitted;
    public DateTime? PublishedAt { get; set; }
    public int? ApprovedByUserId { get; set; }
    public bool IsLocked { get; set; } = false;
    public DateTime? LockedAt { get; set; }

    [MaxLength(500)]
    public string? PublicationNotes { get; set; }

    // Navigation Properties
    public virtual Exam.Exam Exam { get; set; } = null!;
}

/// <summary>
/// Result Lock: Prevents further modifications to published results
/// Tracks who locked the results and when
/// </summary>
public class ResultLock : BaseEntity
{
    public int ExamId { get; set; }
    public int LockedByUserId { get; set; }
    public DateTime LockedAt { get; set; } = DateTime.Now;
    [MaxLength(260)]
    public string? Reason { get; set; }
    public bool CanUnlock { get; set; } = false;

    public virtual Exam.Exam Exam { get; set; } = null!;
}

/// <summary>
/// Student Subject Result: Result for a single subject in an exam
/// Calculated from MarkEntry after final marks are submitted
/// </summary>
public class StudentSubjectResult : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }

    // Denormalized fields for query performance
    public int AcademicYearId { get; set; }
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public int? StudentGroupId { get; set; }

    /// <summary>
    /// Whether this subject is optional per ClassSubject mapping
    /// (used instead of Subject.IsMandatory which is always true)
    /// </summary>
    public bool IsOptionalSubject { get; set; } = false;

    /// <summary>
    /// Whether this is a religion subject
    /// </summary>
    public bool IsReligionSubject { get; set; } = false;

    public decimal MarksObtained { get; set; } = 0;
    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;

    [MaxLength(10)]
    public string Grade { get; set; } = string.Empty;
    public decimal GradePoint { get; set; } = 0;
    public bool IsPassed { get; set; } = false;

    [MaxLength(500)]
    public string? Remarks { get; set; }

    public DateTime CalculatedAt { get; set; } = DateTime.Now;

    // Navigation Properties
    public virtual Exam.Exam Exam { get; set; } = null!;
    public virtual Student.Student Student { get; set; } = null!;
    public virtual Academic.Subject Subject { get; set; } = null!;
}

/// <summary>
/// Student Exam Result: Overall result for an exam (aggregate of all subjects)
/// Includes GPA, merit position, pass/fail status
/// </summary>
public class StudentExamResult : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }

    // Denormalized fields for query & report performance
    public int AcademicYearId { get; set; }
    public int ClassId { get; set; }
    public int SectionId { get; set; }
    public int? StudentGroupId { get; set; }

    public decimal TotalMarks { get; set; } = 0;
    public decimal TotalFullMarks { get; set; } = 0;
    public decimal Gpa { get; set; } = 0;

    [MaxLength(10)]
    public string Grade { get; set; } = string.Empty;

    /// <summary>
    /// Merit position in class/section
    /// </summary>
    public int Position { get; set; } = 0;

    /// <summary>
    /// Merit position in entire class
    /// </summary>
    public int ClassPosition { get; set; } = 0;

    /// <summary>
    /// Merit position in group (for class 9-10)
    /// </summary>
    public int? GroupPosition { get; set; }

    public bool IsPassed { get; set; } = false;
    public int FailedSubjectCount { get; set; } = 0;
    public int PassedSubjectCount { get; set; } = 0;

    public ResultWorkflowStatus Status { get; set; } = ResultWorkflowStatus.Draft;
    public DateTime? PublishedAt { get; set; }
    public DateTime CalculatedAt { get; set; } = DateTime.Now;

    [MaxLength(500)]
    public string? Remarks { get; set; }

    // Navigation Properties
    public virtual Exam.Exam Exam { get; set; } = null!;
    public virtual Student.Student Student { get; set; } = null!;
}

/// <summary>
/// Final Result: Aggregate result for entire academic year
/// Determines GPA, merit position, and promotion eligibility
/// </summary>
public class FinalResult : BaseEntity
{
    public int AcademicYearId { get; set; }
    public int StudentId { get; set; }
    public int SchoolClassId { get; set; }

    // Denormalized fields for promotion/final report queries
    public int SectionId { get; set; }
    public int? StudentGroupId { get; set; }

    /// <summary>Weighted GPA calculated using ResultPolicy exam weights.</summary>
    public decimal FinalGpa { get; set; } = 0;

    /// <summary>Weighted total marks calculated using ResultPolicy exam weights.</summary>
    public decimal WeightedTotalMarks { get; set; } = 0;

    /// <summary>School-wide rank.</summary>
    public int FinalPosition { get; set; } = 0;

    /// <summary>Class-level rank.</summary>
    public int FinalClassPosition { get; set; } = 0;

    /// <summary>Section-level rank.</summary>
    public int FinalSectionPosition { get; set; } = 0;

    /// <summary>Group-level rank (for class 9-10 groups: Science, Business Studies, Humanities).</summary>
    public int FinalGroupPosition { get; set; } = 0;

    [MaxLength(10)]
    public string FinalGrade { get; set; } = string.Empty;

    public PromotionStatus PromotionStatus { get; set; } = PromotionStatus.Pending;
    public bool IsPassed { get; set; } = false;
    public int TotalFailedSubjects { get; set; } = 0;

    /// <summary>Number of passed subjects across all exams in the year.</summary>
    public int TotalPassedSubjects { get; set; } = 0;

    /// <summary>Attendance percentage for the academic year (0-100).</summary>
    public decimal AttendancePercentage { get; set; } = 0;

    /// <summary>Roll number assigned by roll generation engine.</summary>
    public int? GeneratedRollNumber { get; set; }

    [MaxLength(500)]
    public string? PromotionRemarks { get; set; }

    public DateTime CalculatedAt { get; set; } = DateTime.Now;

    // Navigation Properties
    public virtual Academic.AcademicYear AcademicYear { get; set; } = null!;
    public virtual Student.Student Student { get; set; } = null!;
    public virtual Academic.SchoolClass Class { get; set; } = null!;
}

/// <summary>
/// Promotion History: Tracks student promotions across years
/// Records who promoted the student and when
/// </summary>
public class PromotionHistory : BaseEntity
{
    public int StudentId { get; set; }
    public int FromClassId { get; set; }
    public int ToClassId { get; set; }
    public int AcademicYearId { get; set; }

    public PromotionStatus Status { get; set; } = PromotionStatus.Promoted;
    public DateTime PromotedAt { get; set; } = DateTime.Now;
    public int? PromotedByUserId { get; set; }

    [MaxLength(500)]
    public string? Remarks { get; set; }

    // Navigation Properties
    public virtual Student.Student Student { get; set; } = null!;
    public virtual Academic.SchoolClass FromClass { get; set; } = null!;
    public virtual Academic.SchoolClass ToClass { get; set; } = null!;
    public virtual Academic.AcademicYear AcademicYear { get; set; } = null!;
}

/// <summary>
/// Result Audit Log: Comprehensive audit trail for all result modifications
/// Ensures compliance and provides history of changes
/// </summary>
public class ResultAuditLog : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }

    public decimal OldMarks { get; set; } = 0;
    public decimal NewMarks { get; set; } = 0;

    public int? OldGpa { get; set; }
    public int? NewGpa { get; set; }

    public int ChangedByUserId { get; set; }

    [MaxLength(260)]
    public string? Reason { get; set; }

    [MaxLength(100)]
    public string ChangeType { get; set; } = "MarkChange"; // MarkChange, GradeChange, PositionChange

    public DateTime ChangedAt { get; set; } = DateTime.Now;

    // Navigation Properties
    public virtual Exam.Exam Exam { get; set; } = null!;
    public virtual Student.Student Student { get; set; } = null!;
    public virtual Academic.Subject Subject { get; set; } = null!;
}

/// <summary>
/// Re-Evaluation Request: Track requests from students/guardians for mark re-evaluation
/// Supports approval/rejection workflow
/// </summary>
public class ReEvaluationRequest : BaseEntity
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public int RequestedByUserId { get; set; }

    public ReEvaluationStatus Status { get; set; } = ReEvaluationStatus.Requested;
    public decimal OldMarks { get; set; } = 0;
    public decimal? NewMarks { get; set; }

    [MaxLength(400)]
    public string? RequestReason { get; set; }

    [MaxLength(400)]
    public string? Notes { get; set; }

    public int? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Navigation Properties
    public virtual Exam.Exam Exam { get; set; } = null!;
    public virtual Student.Student Student { get; set; } = null!;
    public virtual Academic.Subject Subject { get; set; } = null!;
}

/// <summary>
/// Result Settings: Configurable rules for optional subject handling, fail behavior, and grading
/// </summary>
public class ResultSetting : BaseEntity
{
    public int? AcademicYearId { get; set; }

    /// <summary>
    /// How optional subjects affect GPA calculation
    /// </summary>
    public OptionalSubjectMode OptionalSubjectMode { get; set; } = OptionalSubjectMode.ExcludeFromGPA;

    /// <summary>
    /// How failed subjects affect overall result
    /// </summary>
    public FailSubjectMode FailSubjectMode { get; set; } = FailSubjectMode.StrictFail;

    /// <summary>
    /// Bonus GPA added for optional subjects (when mode=Bonus)
    /// Example: 0.50 means up to 0.50 bonus GPA
    /// </summary>
    public decimal OptionalBonusMaxGPA { get; set; } = 0.50m;

    /// <summary>
    /// Number of best optional subjects to include (when mode=BestOf)
    /// Example: 2 means best 2 optional subjects count
    /// </summary>
    public int BestOfCount { get; set; } = 1;

    /// <summary>
    /// Whether to include only passed optional subjects
    /// </summary>
    public bool RequirePassedOptionalOnly { get; set; } = true;

    /// <summary>
    /// Maximum failed compulsory subjects allowed for promotion
    /// </summary>
    public int MaxFailedCompulsoryAllowed { get; set; } = 0;

    /// <summary>
    /// Minimum GPA required for promotion
    /// </summary>
    public decimal MinimumPromotionGPA { get; set; } = 1.00m;

    /// <summary>
    /// Whether religion subjects count toward GPA
    /// </summary>
    public bool IncludeReligionInGPA { get; set; } = true;

    /// <summary>
    /// Whether to auto-calculate component totals on mark entry
    /// </summary>
    public bool AutoCalculateComponentTotal { get; set; } = true;

    /// <summary>
    /// Rounding precision for GPA (2 = standard)
    /// </summary>
    public int GpaRoundingPrecision { get; set; } = 2;

    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Promotion rules configuration per class. Overrides default rule calculation.
/// </summary>
public class ClassPromotionRule : BaseEntity
{
    public int ClassId { get; set; }

    public decimal MinimumGPA { get; set; } = 1.0m;
    public int MaximumFailedSubjects { get; set; } = 2;
    public bool AllowConditionalPromotion { get; set; } = true;
    public decimal ConditionalPromotionGPA { get; set; } = 0.8m;
    public bool RequireAllSubjectsPass { get; set; } = false;

    [MaxLength(500)]
    public string? CriticalSubjectsJson { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Academic.SchoolClass Class { get; set; } = null!;
}

/// <summary>
/// Result Policy: Admin-configurable academic year result aggregation weights.
/// Defines how exam types contribute to the weighted GPA (e.g., Half Yearly 40% + Annual 60%).
/// </summary>
public class ResultPolicy : BaseEntity
{
    public int AcademicYearId { get; set; }
    public int? SchoolClassId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [MaxLength(500)]
    public string? Description { get; set; }

    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Academic.AcademicYear AcademicYear { get; set; } = null!;
    public virtual Academic.SchoolClass? SchoolClass { get; set; }
    public virtual ICollection<ResultPolicyExamWeight> ExamWeights { get; set; } = [];
}

/// <summary>
/// Result Policy Exam Weight: Individual exam type weight within a ResultPolicy.
/// System validates that total weights = 100%.
/// </summary>
public class ResultPolicyExamWeight : BaseEntity
{
    public int ResultPolicyId { get; set; }

    public int ExamTypeId { get; set; }

    public decimal WeightPercentage { get; set; }

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ResultPolicy ResultPolicy { get; set; } = null!;
    public virtual Exam.ExamType ExamType { get; set; } = null!;
}

/// <summary>
/// Ranking Rule: Configurable ranking order per class/academic year.
/// Admin can reorder tie-breaking criteria.
/// </summary>
public class RankingRule : BaseEntity
{
    public int AcademicYearId { get; set; }
    public int? SchoolClassId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// JSON array of RankingTieBreaker values in priority order.
    /// Example: ["GpaDesc","MarksDesc","PassedSubjectsDesc","AttendanceDesc","RollAsc"]
    /// </summary>
    [MaxLength(2000)]
    public string TieBreakersJson { get; set; } = "[]";

    public bool IsDefault { get; set; } = false;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Academic.AcademicYear AcademicYear { get; set; } = null!;
    public virtual Academic.SchoolClass? SchoolClass { get; set; }
}

/// <summary>
/// Promotion Policy: Admin-configurable promotion rules per class.
/// Supports multiple promotion methods and combined rules.
/// </summary>
public class PromotionPolicy : BaseEntity
{
    public int AcademicYearId { get; set; }
    public int SchoolClassId { get; set; }

    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    public PromotionMethod PrimaryMethod { get; set; } = PromotionMethod.GpaBased;

    /// <summary>
    /// Minimum GPA for promotion (used when PrimaryMethod = GpaBased)
    /// </summary>
    public decimal MinimumGpa { get; set; } = 1.00m;

    /// <summary>
    /// Maximum rank position for promotion (used when PrimaryMethod = PositionBased)
    /// </summary>
    public int? MaxPositionForPromotion { get; set; }

    /// <summary>
    /// Top percentage to promote (e.g., 80 means top 80%). Used when PrimaryMethod = PositionBased.
    /// </summary>
    public decimal? TopPercentagePromote { get; set; }

    /// <summary>
    /// Minimum attendance percentage for promotion
    /// </summary>
    public decimal? MinimumAttendancePercentage { get; set; }

    /// <summary>
    /// Minimum passed subjects count for promotion
    /// </summary>
    public int? MinimumPassedSubjects { get; set; }

    /// <summary>
    /// Whether to use combined rules (AND logic)
    /// </summary>
    public bool UseCombinedRules { get; set; } = false;

    /// <summary>
    /// JSON array of subject names that are critical - student must pass these
    /// </summary>
    [MaxLength(2000)]
    public string? CriticalSubjectsJson { get; set; }

    /// <summary>
    /// Max critical subject failures allowed
    /// </summary>
    public int MaxCriticalSubjectFailures { get; set; } = 0;

    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Academic.AcademicYear AcademicYear { get; set; } = null!;
    public virtual Academic.SchoolClass SchoolClass { get; set; } = null!;
    public virtual ICollection<PromotionPolicyRule> Rules { get; set; } = [];
}

/// <summary>
/// Promotion Policy Rule: Individual rule criteria within a PromotionPolicy.
/// Supports complex AND/OR combined promotion logic.
/// </summary>
public class PromotionPolicyRule : BaseEntity
{
    public int PromotionPolicyId { get; set; }

    [MaxLength(100)]
    public string CriterionType { get; set; } = string.Empty; // Gpa, Marks, Position, Attendance, PassedSubjects

    [MaxLength(100)]
    public string Operator { get; set; } = string.Empty; // GreaterThan, LessThan, Equals, GreaterThanOrEqual

    public decimal ThresholdValue { get; set; }

    [MaxLength(100)]
    public string LogicalOperator { get; set; } = "AND"; // AND, OR

    public bool IsInverse { get; set; } = false; // true = "Fail if criterion met"

    public int DisplayOrder { get; set; } = 0;
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual PromotionPolicy PromotionPolicy { get; set; } = null!;
}

/// <summary>
/// Promotion Execution: Records actual promotion execution with details.
/// Created when admin approves promotion.
/// </summary>
public class PromotionExecution : BaseEntity
{
    public int AcademicYearId { get; set; }
    public int SchoolClassId { get; set; }
    public int? PromotionPolicyId { get; set; }

    public int TotalStudents { get; set; }
    public int PromotedCount { get; set; }
    public int RepeatCount { get; set; }
    public int FailedCount { get; set; }

    [MaxLength(500)]
    public string? Notes { get; set; }

    public int ExecutedByUserId { get; set; }
    public DateTime ExecutedAt { get; set; } = DateTime.Now;

    public bool IsApproved { get; set; } = false;
    public int? ApprovedByUserId { get; set; }
    public DateTime? ApprovedAt { get; set; }

    // Navigation
    public virtual Academic.AcademicYear AcademicYear { get; set; } = null!;
    public virtual Academic.SchoolClass SchoolClass { get; set; } = null!;
    public virtual PromotionPolicy? PromotionPolicy { get; set; }
}

/// <summary>
/// Roll Generation Configuration: Per-class roll number assignment strategy.
/// </summary>
public class RollGenerationConfig : BaseEntity
{
    public int AcademicYearId { get; set; }
    public int SchoolClassId { get; set; }

    public RollGenerationStrategy Strategy { get; set; } = RollGenerationStrategy.MeritBased;

    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Academic.AcademicYear AcademicYear { get; set; } = null!;
    public virtual Academic.SchoolClass SchoolClass { get; set; } = null!;
}

/// <summary>
/// Group Promotion Configuration: How students are assigned to groups for next class.
/// </summary>
public class GroupPromotionConfig : BaseEntity
{
    public int AcademicYearId { get; set; }
    public int FromClassId { get; set; }
    public int ToClassId { get; set; }

    public GroupAssignmentMethod AssignmentMethod { get; set; } = GroupAssignmentMethod.MeritBased;

    /// <summary>
    /// JSON config for method-specific settings.
    /// For SubjectGpaBased: {"Science": 4.0, "BusinessStudies": 3.0, "Humanities": 2.0}
    /// </summary>
    [MaxLength(2000)]
    public string? ConfigurationJson { get; set; }

    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual Academic.AcademicYear AcademicYear { get; set; } = null!;
    public virtual Academic.SchoolClass FromClass { get; set; } = null!;
    public virtual Academic.SchoolClass ToClass { get; set; } = null!;
}
