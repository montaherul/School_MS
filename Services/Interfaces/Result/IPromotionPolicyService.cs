using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Services.Interfaces.Result;

/// <summary>
/// Service for managing promotion policies and executing promotions.
/// Fully configurable by admin, no hardcoded rules.
/// </summary>
public interface IPromotionPolicyService
{
    // Promotion Policy CRUD
    /// <summary>
    /// Get promotion policy by academic year and class
    /// </summary>
    Task<PromotionPolicy?> GetPromotionPolicyAsync(int academicYearId, int schoolClassId, CancellationToken ct = default);
    Task<List<PromotionPolicy>> GetAllPromotionPoliciesAsync(int academicYearId, CancellationToken ct = default);
    Task<PromotionPolicy> CreatePromotionPolicyAsync(PromotionPolicy policy, List<PromotionPolicyRule> rules, CancellationToken ct = default);
    Task<PromotionPolicy> UpdatePromotionPolicyAsync(PromotionPolicy policy, List<PromotionPolicyRule> rules, CancellationToken ct = default);
    Task<bool> DeletePromotionPolicyAsync(int policyId, CancellationToken ct = default);

    /// <summary>
    /// Get promotion policy by id with rules included
    /// </summary>
    Task<PromotionPolicy?> GetPolicyByIdWithRulesAsync(int policyId, CancellationToken ct = default);

    /// <summary>
    /// Get promotion execution for a class in a given academic year
    /// </summary>
    Task<PromotionExecution?> GetPromotionExecutionAsync(int academicYearId, int classId, CancellationToken ct = default);

    // Promotion evaluation using policy
    Task<PromotionEligibilityResult> EvaluatePromotionAsync(int studentId, int academicYearId, CancellationToken ct = default);
    Task<List<PromotionEligibilityResult>> EvaluateClassPromotionAsync(int classId, int academicYearId, CancellationToken ct = default);

    // Execute promotion
    Task<PromotionExecutionResult> ExecutePromotionAsync(int classId, int academicYearId, int executedByUserId, CancellationToken ct = default);

    // Group promotion
    Task<List<GroupAssignmentResult>> AssignGroupsAsync(int fromClassId, int toClassId, int academicYearId, int? processedByUserId, CancellationToken ct = default);
}


