using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

/// <summary>
/// Result policy service: admin-configurable exam weights and ranking rules.
/// No hardcoded weights — all configurable per academic year and class.
/// </summary>
public class ResultPolicyService : IResultPolicyService
{
    private readonly IUnitOfWork _uow;

    public ResultPolicyService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ResultPolicy?> GetResultPolicyAsync(int academicYearId, int? schoolClassId, CancellationToken ct = default)
    {
        return await _uow.Repository<ResultPolicy>().Query()
            .Include(p => p.ExamWeights).ThenInclude(w => w.ExamType)
            .Where(p => p.AcademicYearId == academicYearId
                && p.SchoolClassId == schoolClassId
                && p.IsActive && !p.IsDeleted)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<ResultPolicy>> GetAllResultPoliciesAsync(int academicYearId, CancellationToken ct = default)
    {
        return await _uow.Repository<ResultPolicy>().Query()
            .Include(p => p.ExamWeights).ThenInclude(w => w.ExamType)
            .Where(p => p.AcademicYearId == academicYearId && !p.IsDeleted)
            .OrderBy(p => p.SchoolClassId)
            .ToListAsync(ct);
    }

    public async Task<ResultPolicy> CreateResultPolicyAsync(ResultPolicy policy, List<ResultPolicyExamWeight> weights, CancellationToken ct = default)
    {
        if (!await ValidateWeightsAsync(weights, ct))
            throw new InvalidOperationException("Exam weights must total 100%.");

        await _uow.Repository<ResultPolicy>().AddAsync(policy);
        await _uow.SaveChangesAsync();

        foreach (var w in weights)
        {
            w.ResultPolicyId = policy.Id;
            await _uow.Repository<ResultPolicyExamWeight>().AddAsync(w);
        }
        await _uow.SaveChangesAsync();

        return policy;
    }

    public async Task<ResultPolicy> UpdateResultPolicyAsync(ResultPolicy policy, List<ResultPolicyExamWeight> weights, CancellationToken ct = default)
    {
        if (!await ValidateWeightsAsync(weights, ct))
            throw new InvalidOperationException("Exam weights must total 100%.");

        var existing = await _uow.Repository<ResultPolicy>().Query()
            .Include(p => p.ExamWeights)
            .FirstOrDefaultAsync(p => p.Id == policy.Id, ct);

        if (existing == null) throw new ArgumentException("Result policy not found");

        existing.Name = policy.Name;
        existing.Description = policy.Description;
        existing.IsDefault = policy.IsDefault;
        existing.IsActive = policy.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<ResultPolicy>().Update(existing);

        var existingWeights = await _uow.Repository<ResultPolicyExamWeight>().Query()
            .Where(w => w.ResultPolicyId == policy.Id).ToListAsync(ct);
        _uow.Repository<ResultPolicyExamWeight>().RemoveRange(existingWeights);

        foreach (var w in weights)
        {
            w.ResultPolicyId = policy.Id;
            w.Id = 0;
            await _uow.Repository<ResultPolicyExamWeight>().AddAsync(w);
        }

        await _uow.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteResultPolicyAsync(int policyId, CancellationToken ct = default)
    {
        var policy = await _uow.Repository<ResultPolicy>().GetByIdAsync(policyId);
        if (policy == null) return false;

        policy.IsDeleted = true;
        policy.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<ResultPolicy>().Update(policy);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ValidateWeightsAsync(List<ResultPolicyExamWeight> weights, CancellationToken ct = default)
    {
        if (weights == null || weights.Count == 0) return false;
        var total = weights.Where(w => w.IsActive).Sum(w => w.WeightPercentage);
        return Math.Abs(total - 100) < 0.01m;
    }

    public async Task<RankingRule?> GetRankingRuleAsync(int academicYearId, int? schoolClassId, CancellationToken ct = default)
    {
        return await _uow.Repository<RankingRule>().Query()
            .Where(r => r.AcademicYearId == academicYearId
                && r.SchoolClassId == schoolClassId
                && r.IsActive && !r.IsDeleted)
            .FirstOrDefaultAsync(ct);
    }

    public async Task<List<RankingRule>> GetAllRankingRulesAsync(int academicYearId, CancellationToken ct = default)
    {
        return await _uow.Repository<RankingRule>().Query()
            .Where(r => r.AcademicYearId == academicYearId && !r.IsDeleted)
            .ToListAsync(ct);
    }

    public async Task<RankingRule> CreateRankingRuleAsync(RankingRule rule, CancellationToken ct = default)
    {
        await _uow.Repository<RankingRule>().AddAsync(rule);
        await _uow.SaveChangesAsync();
        return rule;
    }

    public async Task<RankingRule> UpdateRankingRuleAsync(RankingRule rule, CancellationToken ct = default)
    {
        var existing = await _uow.Repository<RankingRule>().GetByIdAsync(rule.Id);
        if (existing == null) throw new ArgumentException("Ranking rule not found");

        existing.Name = rule.Name;
        existing.TieBreakersJson = rule.TieBreakersJson;
        existing.IsDefault = rule.IsDefault;
        existing.IsActive = rule.IsActive;
        existing.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<RankingRule>().Update(existing);
        await _uow.SaveChangesAsync();
        return existing;
    }

    public async Task<bool> DeleteRankingRuleAsync(int ruleId, CancellationToken ct = default)
    {
        var rule = await _uow.Repository<RankingRule>().GetByIdAsync(ruleId);
        if (rule == null) return false;

        rule.IsDeleted = true;
        rule.UpdatedAt = DateTime.UtcNow;
        _uow.Repository<RankingRule>().Update(rule);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<Dictionary<int, decimal>> GetEffectiveExamWeightsAsync(int academicYearId, int classId, CancellationToken ct = default)
    {
        var policy = await GetResultPolicyAsync(academicYearId, classId, ct)
            ?? await GetResultPolicyAsync(academicYearId, null, ct);

        if (policy == null)
        {
            var examTypes = await _uow.Repository<ExamType>().Query()
                .Where(et => et.IsActive && !et.IsDeleted).ToListAsync(ct);
            var equalWeight = examTypes.Count > 0 ? 100m / examTypes.Count : 100m;
            return examTypes.ToDictionary(et => et.Id, _ => equalWeight);
        }

        return policy.ExamWeights
            .Where(w => w.IsActive)
            .ToDictionary(w => w.ExamTypeId, w => w.WeightPercentage);
    }
}
