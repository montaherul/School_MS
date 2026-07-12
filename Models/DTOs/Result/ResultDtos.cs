using SchoolManagementSystem.Models.Entities.Academic;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using System.ComponentModel.DataAnnotations;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.DTOs.Exam;
namespace SchoolManagementSystem.Models.DTOs.Result;

/// <summary>
/// Comprehensive DTOs for Bangladesh School Examination & Result Management System
/// Supports curriculum-specific requirements, component marks, and reporting
/// </summary>

/// <summary>
/// Exam creation/update DTO with Bangladesh exam structure
/// </summary>
public class ExamUpsertDto
{
    public int? Id { get; set; }

    [Required(ErrorMessage = "Exam name is required")]
    [MaxLength(100)]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Exam term is required")]
    public ExamTerm Term { get; set; } = ExamTerm.Other;

    [Required(ErrorMessage = "Academic year is required")]
    public int AcademicYearId { get; set; }

    [Required(ErrorMessage = "Class is required")]
    public int ClassId { get; set; }

    public int? SectionId { get; set; }
    public int? StudentGroupId { get; set; }

    [Required(ErrorMessage = "Start date is required")]
    public DateOnly StartsOn { get; set; }

    [Required(ErrorMessage = "End date is required")]
    public DateOnly EndsOn { get; set; }

    public ResultWorkflowStatus Status { get; set; }
    public bool IsLocked { get; set; } = false;

    public string? ExamGroupKey { get; set; }

    public List<int>? SelectedClassIds { get; set; }
    public List<int>? SelectedSectionIds { get; set; }
    public List<int>? SelectedGroupIds { get; set; }
    public List<SubjectMarkConfigDto>? Subjects { get; set; }
}

/// <summary>
/// Per-subject mark configuration sent from the wizard
/// SubjectMarkStructure is the source of truth for component-level distribution.
/// </summary>
public class SubjectMarkConfigDto
{
    public int SubjectId { get; set; }
    public decimal FullMarks { get; set; } = 100;
    public decimal PassMarks { get; set; } = 33;
    public bool IsOptional { get; set; }
}

/// <summary>
/// Enhanced mark entry DTO with component support
/// </summary>
public class MarkEntryDto
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }

    // Component-wise marks (dynamic dictionary replaces 12 individual properties)
    public ComponentMarksDto ComponentMarks { get; set; } = new();

    // Calculated total
    public decimal MarksObtained { get; set; }
    public string? Grade { get; set; }
    public decimal? GradePoint { get; set; }

    public int TeacherId { get; set; }
    public ResultWorkflowStatus Status { get; set; } = ResultWorkflowStatus.Draft;
}

/// <summary>
/// Mark batch DTO for bulk operations with component support
/// </summary>
public class MarkBatchDto
{
    public int ExamId { get; set; }
    public int SubjectId { get; set; }
    public int TeacherId { get; set; }
    public List<MarkEntryDto> Marks { get; set; } = [];
}

public class ImportResultDto
{
    public int TotalRows { get; set; }
    public int SuccessCount { get; set; }
    public int SkippedCount { get; set; }
    public int ErrorCount { get; set; }
    public List<ImportErrorItemDto> Errors { get; set; } = [];
}

public class BatchSaveResultDto
{
    public int SavedCount { get; set; }
    public List<int> SkippedStudentIds { get; set; } = [];
}

public class ImportErrorItemDto
{
    public int RowNumber { get; set; }
    public string Message { get; set; } = "";
}

/// <summary>
    /// Enhanced student subject result DTO with Bangla names
    /// </summary>
    public class StudentSubjectResultDto
    {
        public int SubjectId { get; set; }
        public string SubjectCode { get; set; } = string.Empty;
        public string SubjectName { get; set; } = string.Empty;
        public string SubjectNameBn { get; set; } = string.Empty; // Bangla name
        public string SubjectGroup { get; set; } = string.Empty; // Common, Science, Humanities, etc.
        public int ExamId { get; set; }

        public decimal MarksObtained { get; set; }
        public decimal FullMarks { get; set; }
        public decimal PassMarks { get; set; }

        public string Grade { get; set; } = string.Empty;
        public decimal GradePoint { get; set; }
        public bool IsPassed { get; set; }

        [MaxLength(500)]
        public string? Remarks { get; set; }
        public decimal ObtainedMarks { get; set; }
        public decimal GPA { get; set; }
    }

/// <summary>
/// Enhanced student exam result DTO with merit positions
/// </summary>
public class StudentExamResultDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public ExamTerm Term { get; set; }
    public ResultWorkflowStatus Status { get; set; }

    // Student info
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;

    public decimal TotalMarks { get; set; }
    public decimal TotalFullMarks { get; set; }
    public decimal Gpa { get; set; }
    public string Grade { get; set; } = string.Empty;

    // Merit positions
    public int Position { get; set; } // Section position
    public int ClassPosition { get; set; } // Class position
    public int? GroupPosition { get; set; } // Science/Humanities group position

    public bool IsPassed { get; set; }
    public int FailedSubjectCount { get; set; }
    public int PassedSubjectCount { get; set; }

    public DateTime? PublishedAt { get; set; }
    public List<StudentSubjectResultDto> Subjects { get; set; } = [];
}

/// <summary>
/// Transcript DTO for comprehensive student academic record
/// </summary>
public class StudentTranscriptDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public string StudentNameBn { get; set; } = string.Empty;
    public string FatherName { get; set; } = string.Empty;
    public string MotherName { get; set; } = string.Empty;
    public DateTime DateOfBirth { get; set; }
    public int RollNumber { get; set; }
    public string RegistrationNumber { get; set; } = string.Empty;

    public string SchoolName { get; set; } = string.Empty;
    public string SchoolAddress { get; set; } = string.Empty;

    public int AcademicYearId { get; set; }
    public string AcademicYear { get; set; } = string.Empty;

    public decimal FinalGPA { get; set; }
    public string FinalGrade { get; set; } = string.Empty;
    public int MeritPosition { get; set; }
    public int TotalExamsTaken { get; set; }
    public int TotalAcademicYears { get; set; }

    // Phase 5: All 4 position types
    public int FinalClassPosition { get; set; }
    public int FinalSectionPosition { get; set; }
    public int FinalGroupPosition { get; set; }
    public decimal WeightedTotalMarks { get; set; }
    public int TotalPassedSubjects { get; set; }
    public int TotalFailedSubjects { get; set; }
    public decimal AttendancePercentage { get; set; }

    public List<StudentExamResultDto> ExamResults { get; set; } = [];
    public List<SubjectTranscriptDto> SubjectWiseResults { get; set; } = [];
}

public class SubjectTranscriptDto
{
    public string SubjectName { get; set; } = string.Empty;
    public string SubjectNameBn { get; set; } = string.Empty;
    public decimal TotalMarks { get; set; }
    public decimal FullMarks { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal GradePoint { get; set; }
    public bool IsPassed { get; set; }
    public string SubjectGroup { get; set; } = string.Empty;
}

/// <summary>
/// Tabulation sheet DTO for class/section result analysis
/// </summary>
public class TabulationSheetDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public int? SectionId { get; set; }
    public string SectionName { get; set; } = string.Empty;

    public List<TabulationStudentDto> Students { get; set; } = [];
    public List<TabulationSubjectDto> Subjects { get; set; } = [];
    public TabulationSummaryDto Summary { get; set; } = new();
}

public class TabulationStudentDto
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public Dictionary<int, SubjectMarkDto> SubjectMarks { get; set; } = []; // SubjectId -> Marks
    public decimal TotalMarks { get; set; }
    public decimal GPA { get; set; }
    public string Grade { get; set; } = string.Empty;
    public int Position { get; set; }
    public bool IsPassed { get; set; }
}

public class SubjectMarkDto
{
    public decimal MarksObtained { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal GradePoint { get; set; }
    public bool IsPassed { get; set; }
}

public class TabulationSubjectDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public decimal FullMarks { get; set; }
    public decimal PassMarks { get; set; }
    public decimal AverageMarks { get; set; }
    public decimal HighestMarks { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
    public decimal PassPercentage { get; set; }
}

public class TabulationSummaryDto
{
    public int TotalStudents { get; set; }
    public int PassedStudents { get; set; }
    public int FailedStudents { get; set; }
    public decimal PassPercentage { get; set; }
    public decimal ClassAverageGPA { get; set; }
    public decimal HighestGPA { get; set; }
    public decimal LowestGPA { get; set; }
}

/// <summary>
/// Merit list DTO for various ranking categories
/// </summary>
public class MeritListDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Class, Section, Group, School
    public List<MeritPositionDto> Positions { get; set; } = [];
}

public class MeritPositionDto
{
    public int Position { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public string ClassName { get; set; } = string.Empty;
    public string SectionName { get; set; } = string.Empty;
    public string GroupName { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public decimal TotalMarks { get; set; }
    public string Grade { get; set; } = string.Empty;
}

/// <summary>
/// Enhanced result summary with subject performance analytics
/// </summary>
public class ResultSummaryDto
{
    public int ExamId { get; set; }
    public string ExamName { get; set; } = string.Empty;
    public int ClassId { get; set; }
    public string ClassName { get; set; } = string.Empty;

    public int TotalStudents { get; set; }
    public int MarksEntered { get; set; }
    public int ResultsCalculated { get; set; }
    public int ResultsPublished { get; set; }

    public decimal ClassAverageGPA { get; set; }
    public decimal ClassAverageGpa { get; set; }
    public decimal PassPercentage { get; set; }
    public int PassedStudents { get; set; }
    public int FailedStudents { get; set; }
    public List<StudentExamResultDto> TopPerformers { get; set; } = [];

    public List<SubjectPerformanceDto> SubjectPerformance { get; set; } = [];
}

public class SubjectPerformanceDto
{
    public int SubjectId { get; set; }
    public string SubjectName { get; set; } = string.Empty;
    public decimal AverageMarks { get; set; }
    public decimal PassPercentage { get; set; }
    public decimal HighestMarks { get; set; }
    public decimal LowestMarks { get; set; }
    public int PassedCount { get; set; }
    public int FailedCount { get; set; }
}

/// <summary>
/// Enhanced re-evaluation DTOs
/// </summary>
public class ReEvaluationRequestDto
{
    public int ExamId { get; set; }
    public int StudentId { get; set; }
    public int SubjectId { get; set; }
    public string Reason { get; set; } = string.Empty;
    public string? Notes { get; set; }
    public int RequestedByUserId { get; set; }
}

public class ReEvaluationProcessDto
{
    public int RequestId { get; set; }
    public bool Approved { get; set; }
    public decimal? NewMarks { get; set; }
    public string? AdminNotes { get; set; }
    public int ProcessedByUserId { get; set; }
}

/// <summary>
/// Grading rule management DTO with Bangladesh system
/// </summary>
public class GradingRuleUpsertDto
{
    public int? Id { get; set; }
    public string Grade { get; set; } = string.Empty;
    public decimal MinMarks { get; set; }
    public decimal MaxMarks { get; set; }
    public decimal GradePoint { get; set; }
    public string Description { get; set; } = string.Empty;
    public int DisplayOrder { get; set; }
    public bool IsActive { get; set; } = true;
}

/// <summary>
/// Result publication DTO with enhanced features
/// </summary>
public class ResultPublishDto
{
    public int ExamId { get; set; }
    public bool LockResults { get; set; } = true;
    public string PublicationNotes { get; set; } = string.Empty;
    public int ApprovedByUserId { get; set; }
}

/// <summary>
/// Final result DTO for academic year summary
/// </summary>
public class FinalResultDto
{
    public int AcademicYearId { get; set; }
    public int StudentId { get; set; }
    public decimal FinalGpa { get; set; }
    public decimal WeightedTotalMarks { get; set; }
    public int FinalPosition { get; set; }
    public int FinalClassPosition { get; set; }
    public int FinalSectionPosition { get; set; }
    public int FinalGroupPosition { get; set; }
    public string FinalGrade { get; set; } = string.Empty;
    public int TotalPassedSubjects { get; set; }
    public int TotalFailedSubjects { get; set; }
    public decimal AttendancePercentage { get; set; }
    public PromotionStatus PromotionStatus { get; set; }
    public bool IsPassed { get; set; }
    public string? PromotionRemarks { get; set; }
}

public class ResultPublicationDto
{
    public int Id { get; set; }

    public string ExamName { get; set; } = "";

    public DateTime PublishedAt { get; set; }

    public bool IsPublished { get; set; }
}

public class PublicationHistoryEntryDto
{
    public string Timestamp { get; set; } = "";
    public string Action { get; set; } = "";
    public string PerformedBy { get; set; } = "";
    public string Notes { get; set; } = "";
}

public class IdNamePairDto
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
}

public class MeritListItem
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int RollNumber { get; set; }
    public decimal GPA { get; set; }
    public decimal TotalMarks { get; set; }
    public int Position { get; set; }
    public string Grade { get; set; } = string.Empty;
    public string Section { get; set; } = string.Empty;
    public string StudentGroup { get; set; } = string.Empty;
}

public class TopPerformer
{
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public int RollNumber { get; set; }
    public decimal GPA { get; set; }
    public string Grade { get; set; } = string.Empty;
    public int Position { get; set; }
}

public class FinalResultGenerationResult
{
    public int AcademicYearId { get; set; }
    public int TotalStudents { get; set; }
    public int GeneratedCount { get; set; }
    public int UpdatedCount { get; set; }
    public int FailedCount { get; set; }
    public List<string> Errors { get; set; } = [];
}

public class MarkEntryStatusDto
{
    public int ExamId { get; set; }
    public int SubjectId { get; set; }
    public int ClassId { get; set; }
    public ResultWorkflowStatus Status { get; set; }
}

public class PromotionRecord
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int FromClassId { get; set; }
    public int ToClassId { get; set; }
    public PromotionStatus Status { get; set; }
    public string Reason { get; set; } = string.Empty;
    public DateTime ProcessedAt { get; set; }
    public int ProcessedByUserId { get; set; }
}

public class PromotionEligibility
{
    public int StudentId { get; set; }
    public bool IsEligible { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public int FailedSubjects { get; set; }
    public int TotalSubjects { get; set; }
    public string RecommendedAction { get; set; } = string.Empty;
}

public class PromotionResult
{
    public int ClassId { get; set; }
    public int AcademicYearId { get; set; }
    public int TotalStudents { get; set; }
    public int PromotedCount { get; set; }
    public int RepeatCount { get; set; }
    public int ConditionalCount { get; set; }
    public List<PromotionRecord> Records { get; set; } = [];
}

public class BulkPromotionResult
{
    public int SuccessCount { get; set; }
    public int FailureCount { get; set; }
    public List<string> Errors { get; set; } = [];
    public List<PromotionRecord> SuccessfulPromotions { get; set; } = [];
}

public class BulkPromotionRequest
{
    public int FromClassId { get; set; }
    public int ToClassId { get; set; }
    public int AcademicYearId { get; set; }
    public int ProcessedByUserId { get; set; }
    public string Comments { get; set; } = string.Empty;
    public bool OverrideEligibility { get; set; } = false;
}

public class PromotionRules
{
    public int ClassId { get; set; }
    public decimal MinimumGPA { get; set; } = 1.0m;
    public int MaximumFailedSubjects { get; set; } = 2;
    public bool AllowConditionalPromotion { get; set; } = true;
    public decimal ConditionalPromotionGPA { get; set; } = 0.8m;
    public bool RequireAllSubjectsPass { get; set; } = false;
    public List<string> CriticalSubjects { get; set; } = [];
}

public class PromotionEligibilityResult
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public decimal FinalGpa { get; set; }
    public int FinalPosition { get; set; }
    public decimal AttendancePercentage { get; set; }
    public int TotalPassedSubjects { get; set; }
    public int TotalFailedSubjects { get; set; }
    public bool IsEligible { get; set; }
    public string Reason { get; set; } = string.Empty;
    public PromotionStatus Status { get; set; } = PromotionStatus.Pending;
}

public class PromotionExecutionResult
{
    public int AcademicYearId { get; set; }
    public int SchoolClassId { get; set; }
    public int TotalStudents { get; set; }
    public int PromotedCount { get; set; }
    public int RepeatCount { get; set; }
    public int FailedCount { get; set; }
    public List<PromotionRecord> Records { get; set; } = [];
    public List<string> Errors { get; set; } = [];
}

public class GroupAssignmentResult
{
    public int StudentId { get; set; }
    public string StudentName { get; set; } = string.Empty;
    public int? AssignedGroupId { get; set; }
    public string AssignedGroupName { get; set; } = string.Empty;
    public string Method { get; set; } = string.Empty;
}

