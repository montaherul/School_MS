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
    Task<PromotionPolicy?> GetPromotionPolicyAsync(int academicYearId, int schoolClassId, CancellationToken ct = default);
    Task<List<PromotionPolicy>> GetAllPromotionPoliciesAsync(int academicYearId, CancellationToken ct = default);
    Task<PromotionPolicy> CreatePromotionPolicyAsync(PromotionPolicy policy, List<PromotionPolicyRule> rules, CancellationToken ct = default);
    Task<PromotionPolicy> UpdatePromotionPolicyAsync(PromotionPolicy policy, List<PromotionPolicyRule> rules, CancellationToken ct = default);
    Task<bool> DeletePromotionPolicyAsync(int policyId, CancellationToken ct = default);

    // Promotion evaluation using policy
    Task<PromotionEligibilityResult> EvaluatePromotionAsync(int studentId, int academicYearId, CancellationToken ct = default);
    Task<List<PromotionEligibilityResult>> EvaluateClassPromotionAsync(int classId, int academicYearId, CancellationToken ct = default);

    // Execute promotion
    Task<PromotionExecutionResult> ExecutePromotionAsync(int classId, int academicYearId, int executedByUserId, CancellationToken ct = default);

    // Group promotion
    Task<List<GroupAssignmentResult>> AssignGroupsAsync(int fromClassId, int toClassId, int academicYearId, int? processedByUserId, CancellationToken ct = default);
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
