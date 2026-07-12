using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Enums;

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

    /// <summary>
    /// Gets promotion history with includes for filtering
    /// </summary>
    Task<List<SchoolManagementSystem.Models.Entities.Result.PromotionHistory>> GetPromotionHistoryAsync(int? studentId, int? classId, int? academicYearId, CancellationToken ct = default);

    /// <summary>
    /// Gets simple student list by class for JSON dropdown
    /// </summary>
    Task<List<object>> GetClassStudentsJsonAsync(int classId, CancellationToken ct = default);
}
