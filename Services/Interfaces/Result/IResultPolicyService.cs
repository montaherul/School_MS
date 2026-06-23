using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Services.Interfaces.Result;

/// <summary>
/// Service for managing result policies (exam weights, ranking rules).
/// Admin-configurable, no hardcoded weights.
/// </summary>
public interface IResultPolicyService
{
    // Result Policy CRUD
    Task<ResultPolicy?> GetResultPolicyAsync(int academicYearId, int? schoolClassId, CancellationToken ct = default);
    Task<List<ResultPolicy>> GetAllResultPoliciesAsync(int academicYearId, CancellationToken ct = default);
    Task<ResultPolicy> CreateResultPolicyAsync(ResultPolicy policy, List<ResultPolicyExamWeight> weights, CancellationToken ct = default);
    Task<ResultPolicy> UpdateResultPolicyAsync(ResultPolicy policy, List<ResultPolicyExamWeight> weights, CancellationToken ct = default);
    Task<bool> DeleteResultPolicyAsync(int policyId, CancellationToken ct = default);

    // Weight validation
    Task<bool> ValidateWeightsAsync(List<ResultPolicyExamWeight> weights, CancellationToken ct = default);

    // Ranking Rules CRUD
    Task<RankingRule?> GetRankingRuleAsync(int academicYearId, int? schoolClassId, CancellationToken ct = default);
    Task<List<RankingRule>> GetAllRankingRulesAsync(int academicYearId, CancellationToken ct = default);
    Task<RankingRule> CreateRankingRuleAsync(RankingRule rule, CancellationToken ct = default);
    Task<RankingRule> UpdateRankingRuleAsync(RankingRule rule, CancellationToken ct = default);
    Task<bool> DeleteRankingRuleAsync(int ruleId, CancellationToken ct = default);

    // Get effective weights for a class
    Task<Dictionary<int, decimal>> GetEffectiveExamWeightsAsync(int academicYearId, int classId, CancellationToken ct = default);
}
