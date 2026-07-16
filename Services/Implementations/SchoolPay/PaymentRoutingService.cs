using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Entities.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class PaymentRoutingService : IPaymentRoutingService
{
    private readonly ISchoolPayRepository _repo;
    private readonly ILogger<PaymentRoutingService> _logger;

    public PaymentRoutingService(ISchoolPayRepository repo, ILogger<PaymentRoutingService> logger)
    {
        _repo = repo;
        _logger = logger;
    }

    public async Task<List<SchoolPayRouteRuleDto>> GetAllRulesAsync(CancellationToken ct = default)
        => await _repo.GetAllRouteRulesAsync(ct);

    public async Task<SchoolPayRouteRuleDto?> GetRuleByIdAsync(int id, CancellationToken ct = default)
    {
        var rules = await _repo.GetAllRouteRulesAsync(ct);
        return rules.FirstOrDefault(r => r.Id == id);
    }

    public async Task<int> CreateRuleAsync(SchoolPayRouteRuleUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        var rule = new PaymentRouteRule
        {
            PaymentProviderId = dto.PaymentProviderId,
            RuleName = dto.RuleName,
            Priority = dto.Priority,
            MinAmount = dto.MinAmount,
            MaxAmount = dto.MaxAmount,
            FeeType = dto.FeeType,
            ConditionExpression = dto.ConditionExpression,
            IsActive = dto.IsActive,
            DisplayOrder = dto.DisplayOrder,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
        return await _repo.CreateRouteRuleAsync(rule, ct);
    }

    public async Task UpdateRuleAsync(int id, SchoolPayRouteRuleUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        var rule = await _repo.GetRouteRuleEntityByIdAsync(id, ct);
        if (rule == null) throw new KeyNotFoundException($"Route rule {id} not found");

        rule.PaymentProviderId = dto.PaymentProviderId;
        rule.RuleName = dto.RuleName;
        rule.Priority = dto.Priority;
        rule.MinAmount = dto.MinAmount;
        rule.MaxAmount = dto.MaxAmount;
        rule.FeeType = dto.FeeType;
        rule.ConditionExpression = dto.ConditionExpression;
        rule.IsActive = dto.IsActive;
        rule.DisplayOrder = dto.DisplayOrder;
        rule.UpdatedBy = updatedBy;
        rule.UpdatedAt = DateTime.UtcNow;

        await _repo.UpdateRouteRuleAsync(rule, ct);
    }

    public async Task DeleteRuleAsync(int id, CancellationToken ct = default)
        => await _repo.DeleteRouteRuleAsync(id, ct);

    public async Task<int?> ResolveProviderAsync(decimal amount, string? feeType = null, CancellationToken ct = default)
    {
        var rules = await _repo.GetActiveRouteRulesForAmountAsync(amount, feeType, ct);
        if (rules.Count == 0) return null;

        return rules[0].PaymentProviderId;
    }

    public async Task ToggleRuleActiveAsync(int id, bool isActive, CancellationToken ct = default)
    {
        var rule = await _repo.GetRouteRuleEntityByIdAsync(id, ct);
        if (rule == null) throw new KeyNotFoundException($"Route rule {id} not found");

        rule.IsActive = isActive;
        rule.UpdatedAt = DateTime.UtcNow;
        await _repo.UpdateRouteRuleAsync(rule, ct);
    }
}
