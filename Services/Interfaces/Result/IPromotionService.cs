using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

/// <summary>
/// Service for managing student promotion logic
/// Supports Bangladesh education system promotion rules
/// </summary>
public interface IPromotionService
{
    /// <summary>
    /// Calculates promotion eligibility for a student
    /// </summary>
    Task<PromotionEligibility> CalculatePromotionEligibilityAsync(int studentId, int academicYearId);

    /// <summary>
    /// Processes promotion for entire class
    /// </summary>
    Task<PromotionResult> ProcessClassPromotionAsync(int classId, int academicYearId, int processedByUserId);

    /// <summary>
    /// Bulk promotion with configurable rules
    /// </summary>
    Task<BulkPromotionResult> BulkPromotionAsync(BulkPromotionRequest request);

    /// <summary>
    /// Gets promotion rules for a class
    /// </summary>
    Task<PromotionRules> GetPromotionRulesAsync(int classId);

    /// <summary>
    /// Updates promotion rules for a class
    /// </summary>
    Task UpdatePromotionRulesAsync(int classId, PromotionRules rules);

    /// <summary>
    /// Gets promotion history for a student
    /// </summary>
    Task<IEnumerable<PromotionRecord>> GetStudentPromotionHistoryAsync(int studentId);

    /// <summary>
    /// Reverses a promotion (admin only)
    /// </summary>
    Task ReversePromotionAsync(int promotionHistoryId, int reversedByUserId, string reason);

    /// <summary>
    /// Rebuilds cascade data for a promoted student across attendance, exam results, and group assignment.
    /// Batch-updates AttendanceRecord, StudentExamResult, StudentSubjectResult, FinalResult,
    /// and handles StudentGroupAssignment upsert.
    /// </summary>
    Task RebuildStudentCascadeAsync(int studentId, int newClassId, int? newSectionId, int? newGroupId, int academicYearId, CancellationToken ct = default);
}

public class PromotionEligibility
{
    public int StudentId { get; set; }
    public bool IsEligible { get; set; }
    public string Reason { get; set; } = string.Empty;
    public decimal GPA { get; set; }
    public int FailedSubjects { get; set; }
    public int TotalSubjects { get; set; }
    public string RecommendedAction { get; set; } = string.Empty; // Promote, Repeat, Conditional
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
    public decimal MinimumGPA { get; set; } = 1.0m; // D grade minimum
    public int MaximumFailedSubjects { get; set; } = 2;
    public bool AllowConditionalPromotion { get; set; } = true;
    public decimal ConditionalPromotionGPA { get; set; } = 0.8m;
    public bool RequireAllSubjectsPass { get; set; } = false; // For critical classes
    public List<string> CriticalSubjects { get; set; } = []; // Subjects that must be passed
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
