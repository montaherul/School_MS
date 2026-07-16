using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Entities.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolPayPaymentMethod = SchoolManagementSystem.Models.Entities.SchoolPay.PaymentMethod;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Repositories.Implementations.SchoolPay;

public class SchoolPayRepository : ISchoolPayRepository
{
    private readonly SchoolDbContext _db;
    private readonly ILogger<SchoolPayRepository> _logger;

    public SchoolPayRepository(SchoolDbContext db, ILogger<SchoolPayRepository> logger)
    {
        _db = db;
        _logger = logger;
    }

    // ═══════════════════════════════════════════
    //  PROVIDER MANAGEMENT
    // ═══════════════════════════════════════════

    public async Task<List<SchoolPayProviderListDto>> GetAllProvidersAsync(CancellationToken ct = default)
    {
        return await _db.Set<PaymentProvider>()
            .Where(p => !p.IsDeleted)
            .Select(p => new SchoolPayProviderListDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                LogoUrl = p.LogoUrl,
                Status = p.Status,
                IsSandbox = p.IsSandbox,
                Priority = p.Priority,
                IsActive = p.IsActive,
                MethodCount = _db.Set<SchoolPayPaymentMethod>().Count(m => m.PaymentProviderId == p.Id && m.IsActive && !m.IsDeleted)
            })
            .OrderBy(p => p.Priority)
            .ToListAsync(ct);
    }

    public async Task<SchoolPayProviderDto?> GetProviderByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Set<PaymentProvider>()
            .Where(p => p.Id == id && !p.IsDeleted)
            .Select(p => new SchoolPayProviderDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                LogoUrl = p.LogoUrl,
                Description = p.Description,
                IsActive = p.IsActive,
                Status = p.Status,
                Priority = p.Priority,
                Methods = _db.Set<SchoolPayPaymentMethod>()
                    .Where(m => m.PaymentProviderId == p.Id && m.IsActive && !m.IsDeleted)
                    .OrderBy(m => m.DisplayOrder)
                    .Select(m => new SchoolPayProviderMethodDto
                    {
                        Id = m.Id,
                        Code = m.Code,
                        Name = m.Name,
                        LogoUrl = m.LogoUrl,
                        DisplayOrder = m.DisplayOrder
                    }).ToList()
            }).FirstOrDefaultAsync(ct);
    }

    public async Task<PaymentProvider?> GetProviderEntityByIdAsync(int id, CancellationToken ct = default)
        => await _db.Set<PaymentProvider>().FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, ct);

    public async Task<PaymentProvider?> GetProviderEntityByCodeAsync(string code, CancellationToken ct = default)
        => await _db.Set<PaymentProvider>().FirstOrDefaultAsync(p => p.Code == code && !p.IsDeleted, ct);

    public async Task<int> CreateProviderAsync(PaymentProvider provider, List<PaymentProviderConfiguration> configs, CancellationToken ct = default)
    {
        _db.Set<PaymentProvider>().Add(provider);
        await _db.SaveChangesAsync(ct);

        foreach (var cfg in configs)
        {
            cfg.PaymentProviderId = provider.Id;
            _db.Set<PaymentProviderConfiguration>().Add(cfg);
        }
        await _db.SaveChangesAsync(ct);
        return provider.Id;
    }

    public async Task UpdateProviderAsync(PaymentProvider provider, CancellationToken ct = default)
    {
        _db.Set<PaymentProvider>().Update(provider);
        await _db.SaveChangesAsync(ct);
    }

    public async Task UpsertProviderConfigurationAsync(int providerId, string key, string value, string updatedBy, CancellationToken ct = default)
    {
        var existing = await _db.Set<PaymentProviderConfiguration>()
            .FirstOrDefaultAsync(c => c.PaymentProviderId == providerId && c.Key == key && !c.IsDeleted, ct);
        if (existing != null)
        {
            existing.Value = value;
            existing.UpdatedBy = updatedBy;
            existing.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _db.Set<PaymentProviderConfiguration>().Add(new PaymentProviderConfiguration
            {
                PaymentProviderId = providerId,
                Key = key,
                Value = value,
                IsActive = true,
                CreatedBy = updatedBy,
                CreatedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<SchoolPayProviderDto>> GetActiveProviderDtosAsync(CancellationToken ct = default)
    {
        return await _db.Set<PaymentProvider>()
            .Where(p => p.IsActive && !p.IsDeleted && p.Status == ProviderStatus.Active)
            .OrderBy(p => p.Priority)
            .Select(p => new SchoolPayProviderDto
            {
                Id = p.Id,
                Code = p.Code,
                Name = p.Name,
                LogoUrl = p.LogoUrl,
                Description = p.Description,
                IsActive = p.IsActive,
                Status = p.Status,
                Priority = p.Priority
            }).ToListAsync(ct);
    }

    public async Task<List<SchoolPayProviderMethodDto>> GetPaymentMethodsAsync(int providerId, CancellationToken ct = default)
    {
        return await _db.Set<SchoolPayPaymentMethod>()
            .Where(m => m.PaymentProviderId == providerId && m.IsActive && !m.IsDeleted)
            .OrderBy(m => m.DisplayOrder)
            .Select(m => new SchoolPayProviderMethodDto
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                LogoUrl = m.LogoUrl,
                PaymentProviderId = m.PaymentProviderId,
                DisplayOrder = m.DisplayOrder,
                IsActive = m.IsActive,
                IsDefault = m.IsDefault,
                IsRecommended = m.IsRecommended,
                IsPopular = m.IsPopular
            }).ToListAsync(ct);
    }

    public async Task<List<SchoolPayProviderMethodDto>> GetAllPaymentMethodsAsync(CancellationToken ct = default)
    {
        return await _db.Set<SchoolPayPaymentMethod>()
            .Where(m => !m.IsDeleted)
            .OrderBy(m => m.PaymentProviderId)
            .ThenBy(m => m.DisplayOrder)
            .Select(m => new SchoolPayProviderMethodDto
            {
                Id = m.Id,
                Code = m.Code,
                Name = m.Name,
                LogoUrl = m.LogoUrl,
                PaymentProviderId = m.PaymentProviderId,
                ProviderName = m.PaymentProvider != null ? m.PaymentProvider.Name : null,
                DisplayOrder = m.DisplayOrder,
                IsActive = m.IsActive,
                IsDefault = m.IsDefault,
                IsRecommended = m.IsRecommended,
                IsPopular = m.IsPopular,
                PopularityRank = m.PopularityRank,
                BackgroundColor = m.BackgroundColor,
                TextColor = m.TextColor,
                Icon = m.Icon,
                CssClass = m.CssClass
            }).ToListAsync(ct);
    }

    public async Task<SchoolPayPaymentMethod?> GetPaymentMethodEntityByIdAsync(int id, CancellationToken ct = default)
        => await         _db.Set<SchoolPayPaymentMethod>().FirstOrDefaultAsync(m => m.Id == id && !m.IsDeleted, ct);

    public async Task<SchoolPayPaymentMethod?> GetPaymentMethodEntityByCodeAsync(string code, CancellationToken ct = default)
        => await _db.Set<SchoolPayPaymentMethod>().FirstOrDefaultAsync(m => m.Code == code && !m.IsDeleted, ct);

    public async Task<int> CreatePaymentMethodAsync(SchoolPayPaymentMethod method, CancellationToken ct = default)
    {
        _db.Set<SchoolPayPaymentMethod>().Add(method);
        await _db.SaveChangesAsync(ct);
        return method.Id;
    }

    public async Task UpdatePaymentMethodAsync(SchoolPayPaymentMethod method, CancellationToken ct = default)
    {
        _db.Set<SchoolPayPaymentMethod>().Update(method);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> TogglePaymentMethodActiveAsync(int id, bool isActive, CancellationToken ct = default)
    {
        var method = await         _db.Set<SchoolPayPaymentMethod>().FirstOrDefaultAsync(m => m.Id == id, ct);
        if (method == null) return false;
        method.IsActive = isActive;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> UpdatePaymentMethodOrderAsync(int id, int displayOrder, CancellationToken ct = default)
    {
        var method = await         _db.Set<SchoolPayPaymentMethod>().FirstOrDefaultAsync(m => m.Id == id, ct);
        if (method == null) return false;
        method.DisplayOrder = displayOrder;
        await _db.SaveChangesAsync(ct);
        return true;
    }

    // ═══════════════════════════════════════════
    //  TRANSACTIONS
    // ═══════════════════════════════════════════

    public async Task<List<SchoolPayTransactionDto>> GetTransactionsPagedAsync(int page, int pageSize, string? status = null, string? providerCode = null, CancellationToken ct = default)
    {
        var query = _db.Set<PaymentGatewayTransaction>()
            .Where(t => !t.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<SchoolPayTransactionStatus>(status, true, out var statusEnum))
            query = query.Where(t => t.Status == statusEnum);

        if (!string.IsNullOrEmpty(providerCode))
            query = query.Where(t => t.PaymentProvider != null && t.PaymentProvider.Code == providerCode);

        return await query
            .OrderByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new SchoolPayTransactionDto
            {
                Id = t.Id,
                ProviderName = t.PaymentProvider != null ? t.PaymentProvider.Name : "",
                MethodName = t.PaymentMethod != null ? t.PaymentMethod.Name : null,
                TransactionReference = t.TransactionReference,
                ProviderTransactionId = t.ProviderTransactionId,
                BankTransactionId = t.BankTransactionId,
                Amount = t.Amount,
                Currency = t.Currency,
                FeeAmount = t.FeeAmount,
                Status = t.Status,
                StatusMessage = t.StatusMessage,
                AttemptCount = t.AttemptCount,
                InitiatedAt = t.InitiatedAt,
                CompletedAt = t.CompletedAt
            }).ToListAsync(ct);
    }

    public async Task<int> GetTransactionCountAsync(string? status = null, string? providerCode = null, CancellationToken ct = default)
    {
        var query = _db.Set<PaymentGatewayTransaction>()
            .Where(t => !t.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(status) && Enum.TryParse<SchoolPayTransactionStatus>(status, true, out var statusEnum))
            query = query.Where(t => t.Status == statusEnum);

        if (!string.IsNullOrEmpty(providerCode))
            query = query.Where(t => t.PaymentProvider != null && t.PaymentProvider.Code == providerCode);

        return await query.CountAsync(ct);
    }

    public async Task<PaymentGatewayTransaction?> GetTransactionEntityByIdAsync(int id, CancellationToken ct = default)
        => await _db.Set<PaymentGatewayTransaction>()
            .Include(t => t.PaymentProvider)
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct);

    public async Task<int> CreateTransactionAsync(PaymentGatewayTransaction entity, CancellationToken ct = default)
    {
        _db.Set<PaymentGatewayTransaction>().Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Id;
    }

    // ═══════════════════════════════════════════
    //  WEBHOOKS
    // ═══════════════════════════════════════════

    public async Task<int> CreateWebhookAsync(PaymentGatewayWebhook webhook, CancellationToken ct = default)
    {
        _db.Set<PaymentGatewayWebhook>().Add(webhook);
        await _db.SaveChangesAsync(ct);
        return webhook.Id;
    }

    public async Task UpdateWebhookAsync(PaymentGatewayWebhook webhook, CancellationToken ct = default)
    {
        _db.Set<PaymentGatewayWebhook>().Update(webhook);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<SchoolPayWebhookDto>> GetRecentWebhooksAsync(int count = 50, CancellationToken ct = default)
    {
        return await _db.Set<PaymentGatewayWebhook>()
            .Where(w => !w.IsDeleted)
            .OrderByDescending(w => w.Id)
            .Take(count)
            .Select(w => new SchoolPayWebhookDto
            {
                Id = w.Id,
                ProviderName = _db.Set<PaymentProvider>()
                    .Where(p => p.Id == w.PaymentProviderId)
                    .Select(p => p.Name)
                    .FirstOrDefault() ?? "",
                TransactionReference = w.TransactionReference,
                ProviderEventType = w.ProviderEventType,
                Status = w.Status,
                ErrorMessage = w.ErrorMessage,
                AttemptCount = w.AttemptCount,
                ReceivedAt = w.ReceivedAt,
                ProcessedAt = w.ProcessedAt
            }).ToListAsync(ct);
    }

    public async Task<PaymentGatewayWebhook?> GetWebhookEntityByIdAsync(int id, CancellationToken ct = default)
        => await _db.Set<PaymentGatewayWebhook>()
            .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted, ct);

    // ═══════════════════════════════════════════
    //  SETTLEMENTS
    // ═══════════════════════════════════════════

    public async Task<List<SchoolPaySettlementDto>> GetSettlementsAsync(CancellationToken ct = default)
    {
        return await _db.Set<PaymentGatewaySettlement>()
            .Where(s => !s.IsDeleted)
            .OrderByDescending(s => s.Id)
            .Select(s => new SchoolPaySettlementDto
            {
                Id = s.Id,
                ProviderName = _db.Set<PaymentProvider>()
                    .Where(p => p.Id == s.PaymentProviderId)
                    .Select(p => p.Name)
                    .FirstOrDefault() ?? "",
                SettlementReference = s.SettlementReference,
                ProviderSettlementId = s.ProviderSettlementId,
                Amount = s.Amount,
                Currency = s.Currency,
                Status = s.Status,
                SettlementDate = s.SettlementDate,
                Remarks = s.Remarks
            }).ToListAsync(ct);
    }

    public async Task<SchoolPaySettlementDto?> GetSettlementByIdAsync(int id, CancellationToken ct = default)
    {
        return await _db.Set<PaymentGatewaySettlement>()
            .Where(s => s.Id == id && !s.IsDeleted)
            .Select(s => new SchoolPaySettlementDto
            {
                Id = s.Id,
                ProviderName = _db.Set<PaymentProvider>()
                    .Where(p => p.Id == s.PaymentProviderId)
                    .Select(p => p.Name)
                    .FirstOrDefault() ?? "",
                SettlementReference = s.SettlementReference,
                ProviderSettlementId = s.ProviderSettlementId,
                Amount = s.Amount,
                Currency = s.Currency,
                Status = s.Status,
                SettlementDate = s.SettlementDate,
                Remarks = s.Remarks
            }).FirstOrDefaultAsync(ct);
    }

    public async Task<PaymentGatewaySettlement?> GetSettlementEntityByIdAsync(int id, CancellationToken ct = default)
        => await _db.Set<PaymentGatewaySettlement>()
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);

    public async Task UpdateSettlementAsync(PaymentGatewaySettlement settlement, CancellationToken ct = default)
    {
        _db.Set<PaymentGatewaySettlement>().Update(settlement);
        await _db.SaveChangesAsync(ct);
    }

    public async Task<int> CreateSettlementAsync(PaymentGatewaySettlement entity, CancellationToken ct = default)
    {
        _db.Set<PaymentGatewaySettlement>().Add(entity);
        await _db.SaveChangesAsync(ct);
        return entity.Id;
    }

    // ═══════════════════════════════════════════
    //  REFUNDS
    // ═══════════════════════════════════════════

    public async Task<int> CreateRefundAsync(PaymentGatewayRefund refund, CancellationToken ct = default)
    {
        _db.Set<PaymentGatewayRefund>().Add(refund);
        await _db.SaveChangesAsync(ct);
        return refund.Id;
    }

    public async Task<List<SchoolPayRefundDto>> GetRefundsAsync(CancellationToken ct = default)
    {
        return await _db.Set<PaymentGatewayRefund>()
            .Where(r => !r.IsDeleted)
            .OrderByDescending(r => r.Id)
            .Select(r => new SchoolPayRefundDto
            {
                Id = r.Id,
                TransactionReference = _db.Set<PaymentGatewayTransaction>()
                    .Where(t => t.Id == r.PaymentGatewayTransactionId)
                    .Select(t => t.TransactionReference)
                    .FirstOrDefault() ?? "",
                RefundReference = r.RefundReference,
                ProviderRefundId = r.ProviderRefundId,
                RefundAmount = r.RefundAmount,
                Reason = r.Reason,
                Status = r.Status,
                RefundedAt = r.RefundedAt
            }).ToListAsync(ct);
    }

    // ═══════════════════════════════════════════
    //  DASHBOARD
    // ═══════════════════════════════════════════

    public async Task<SchoolPayDashboardDto> GetDashboardDataAsync(CancellationToken ct = default)
    {
        var transactions = await _db.Set<PaymentGatewayTransaction>()
            .Where(t => !t.IsDeleted)
            .ToListAsync(ct);

        var activeProviders = await _db.Set<PaymentProvider>()
            .CountAsync(p => p.IsActive && !p.IsDeleted, ct);

        var healthyProviders = await _db.Set<PaymentGatewayHealth>()
            .CountAsync(h => h.Status == ProviderHealthStatus.Healthy, ct);

        return new SchoolPayDashboardDto
        {
            TotalTransactions = transactions.Count,
            CompletedTransactions = transactions.Count(t => t.Status == SchoolPayTransactionStatus.Completed),
            FailedTransactions = transactions.Count(t => t.Status == SchoolPayTransactionStatus.Failed),
            PendingTransactions = transactions.Count(t => t.Status == SchoolPayTransactionStatus.Pending),
            TotalAmount = transactions.Where(t => t.Status == SchoolPayTransactionStatus.Completed).Sum(t => t.Amount),
            TotalFeeAmount = transactions.Where(t => t.FeeAmount.HasValue).Sum(t => t.FeeAmount ?? 0),
            ActiveProviders = activeProviders,
            HealthyProviders = healthyProviders,
            RecentTransactions = transactions
                .OrderByDescending(t => t.Id)
                .Take(10)
                .Select(t => new SchoolPayRecentTransactionDto
                {
                    TransactionReference = t.TransactionReference,
                    Amount = t.Amount,
                    ProviderName = t.PaymentProvider != null ? t.PaymentProvider.Name : "",
                    Status = t.Status,
                    InitiatedAt = t.InitiatedAt
                }).ToList()
        };
    }

    // ═══════════════════════════════════════════
    //  AUDIT
    // ═══════════════════════════════════════════

    public async Task<bool> LogAuditEventAsync(int? transactionId, string eventType, string? eventData, string? performedBy, string? ipAddress, CancellationToken ct = default)
    {
        try
        {
            _db.Set<PaymentGatewayAudit>().Add(new PaymentGatewayAudit
            {
                PaymentGatewayTransactionId = transactionId,
                EventType = eventType,
                EventData = eventData,
                PerformedBy = performedBy,
                IpAddress = ipAddress,
                PerformedAt = DateTime.UtcNow
            });
            await _db.SaveChangesAsync(ct);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log audit event");
            return false;
        }
    }

    // ═══════════════════════════════════════════
    //  HEALTH
    // ═══════════════════════════════════════════

    public async Task RecordHealthCheckAsync(int providerId, ProviderHealthStatus status, int responseTimeMs, decimal successRate, int totalRequests, int failedRequests, string? lastError, CancellationToken ct = default)
    {
        var health = new PaymentGatewayHealth
        {
            PaymentProviderId = providerId,
            Status = status,
            ResponseTimeMs = responseTimeMs,
            SuccessRate = successRate,
            TotalRequests = totalRequests,
            FailedRequests = failedRequests,
            LastError = lastError,
            LastCheckedAt = DateTime.UtcNow,
            CreatedAt = DateTime.UtcNow
        };
        _db.Set<PaymentGatewayHealth>().Add(health);
        await _db.SaveChangesAsync(ct);
    }

    // ═══════════════════════════════════════════
    //  SP-02: HEALTH MONITOR
    // ═══════════════════════════════════════════

    public async Task<List<SchoolPayHealthStatusDto>> GetLatestHealthStatusAsync(CancellationToken ct = default)
    {
        var providers = await _db.Set<PaymentProvider>()
            .Where(p => !p.IsDeleted)
            .ToListAsync(ct);

        var result = new List<SchoolPayHealthStatusDto>();
        foreach (var provider in providers)
        {
            var latestHealth = await _db.Set<PaymentGatewayHealth>()
                .Where(h => h.PaymentProviderId == provider.Id)
                .OrderByDescending(h => h.LastCheckedAt)
                .FirstOrDefaultAsync(ct);

            result.Add(new SchoolPayHealthStatusDto
            {
                ProviderId = provider.Id,
                ProviderName = provider.Name,
                Status = latestHealth?.Status ?? ProviderHealthStatus.Unknown,
                ResponseTimeMs = latestHealth?.ResponseTimeMs ?? 0,
                SuccessRate = latestHealth?.SuccessRate ?? 100m,
                TotalRequests = latestHealth?.TotalRequests ?? 0,
                FailedRequests = latestHealth?.FailedRequests ?? 0,
                LastError = latestHealth?.LastError,
                LastCheckedAt = latestHealth?.LastCheckedAt,
                LastSuccessAt = latestHealth?.LastSuccessAt
            });
        }
        return result;
    }

    public async Task<List<SchoolPayHealthHistoryDto>> GetHealthHistoryAsync(int providerId, int days = 30, CancellationToken ct = default)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        return await _db.Set<PaymentGatewayHealth>()
            .Where(h => h.PaymentProviderId == providerId && h.LastCheckedAt >= since)
            .OrderByDescending(h => h.LastCheckedAt)
            .Select(h => new SchoolPayHealthHistoryDto
            {
                Id = h.Id,
                ProviderName = _db.Set<PaymentProvider>()
                    .Where(p => p.Id == h.PaymentProviderId)
                    .Select(p => p.Name)
                    .FirstOrDefault() ?? "",
                Status = h.Status,
                ResponseTimeMs = h.ResponseTimeMs,
                LastError = h.LastError,
                LastCheckedAt = h.LastCheckedAt
            }).ToListAsync(ct);
    }

    // ═══════════════════════════════════════════
    //  SP-02: ROUTING RULES
    // ═══════════════════════════════════════════

    public async Task<List<SchoolPayRouteRuleDto>> GetAllRouteRulesAsync(CancellationToken ct = default)
    {
        return await _db.Set<PaymentRouteRule>()
            .Where(r => !r.IsDeleted)
            .OrderBy(r => r.DisplayOrder)
            .Select(r => new SchoolPayRouteRuleDto
            {
                Id = r.Id,
                ProviderName = _db.Set<PaymentProvider>()
                    .Where(p => p.Id == r.PaymentProviderId)
                    .Select(p => p.Name)
                    .FirstOrDefault() ?? "",
                PaymentProviderId = r.PaymentProviderId,
                RuleName = r.RuleName,
                Priority = r.Priority,
                MinAmount = r.MinAmount,
                MaxAmount = r.MaxAmount,
                FeeType = r.FeeType,
                ConditionExpression = r.ConditionExpression,
                IsActive = r.IsActive,
                DisplayOrder = r.DisplayOrder
            }).ToListAsync(ct);
    }

    public async Task<PaymentRouteRule?> GetRouteRuleEntityByIdAsync(int id, CancellationToken ct = default)
        => await _db.Set<PaymentRouteRule>().FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, ct);

    public async Task<int> CreateRouteRuleAsync(PaymentRouteRule rule, CancellationToken ct = default)
    {
        _db.Set<PaymentRouteRule>().Add(rule);
        await _db.SaveChangesAsync(ct);
        return rule.Id;
    }

    public async Task UpdateRouteRuleAsync(PaymentRouteRule rule, CancellationToken ct = default)
    {
        _db.Set<PaymentRouteRule>().Update(rule);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteRouteRuleAsync(int id, CancellationToken ct = default)
    {
        var rule = await _db.Set<PaymentRouteRule>().FirstOrDefaultAsync(r => r.Id == id && !r.IsDeleted, ct);
        if (rule != null)
        {
            rule.IsDeleted = true;
            rule.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task<List<SchoolPayRouteRuleDto>> GetActiveRouteRulesForAmountAsync(decimal amount, string? feeType, CancellationToken ct = default)
    {
        var query = _db.Set<PaymentRouteRule>()
            .Where(r => r.IsActive && !r.IsDeleted)
            .AsQueryable();

        if (!string.IsNullOrEmpty(feeType))
            query = query.Where(r => r.FeeType == null || r.FeeType == feeType);

        query = query.Where(r => (!r.MinAmount.HasValue || r.MinAmount <= amount)
                              && (!r.MaxAmount.HasValue || r.MaxAmount >= amount));

        return await query
            .OrderBy(r => r.DisplayOrder)
            .Select(r => new SchoolPayRouteRuleDto
            {
                Id = r.Id,
                ProviderName = _db.Set<PaymentProvider>()
                    .Where(p => p.Id == r.PaymentProviderId)
                    .Select(p => p.Name)
                    .FirstOrDefault() ?? "",
                PaymentProviderId = r.PaymentProviderId,
                RuleName = r.RuleName,
                Priority = r.Priority,
                MinAmount = r.MinAmount,
                MaxAmount = r.MaxAmount,
                FeeType = r.FeeType,
                ConditionExpression = r.ConditionExpression,
                IsActive = r.IsActive,
                DisplayOrder = r.DisplayOrder
            }).ToListAsync(ct);
    }

    // ═══════════════════════════════════════════
    //  SP-02: WEBHOOK QUEUE
    // ═══════════════════════════════════════════

    public async Task<List<PaymentGatewayWebhook>> GetPendingWebhooksForRetryAsync(int maxRetries = 3, int batchSize = 10, CancellationToken ct = default)
    {
        return await _db.Set<PaymentGatewayWebhook>()
            .Where(w => !w.IsDeleted
                && w.Status == SchoolPayWebhookStatus.Failed
                && w.AttemptCount < maxRetries)
            .OrderBy(w => w.AttemptCount)
            .ThenBy(w => w.Id)
            .Take(batchSize)
            .Include(w => w.PaymentProvider)
            .ToListAsync(ct);
    }

    // ═══════════════════════════════════════════
    //  SP-02: RECONCILIATION
    // ═══════════════════════════════════════════

    public async Task<List<PaymentGatewayTransaction>> GetTransactionsByDateRangeAsync(DateTime from, DateTime to, int? providerId, CancellationToken ct = default)
    {
        var query = _db.Set<PaymentGatewayTransaction>()
            .Where(t => !t.IsDeleted
                && t.CompletedAt >= from
                && t.CompletedAt <= to);

        if (providerId.HasValue)
            query = query.Where(t => t.PaymentProviderId == providerId.Value);

        return await query
            .OrderByDescending(t => t.CompletedAt)
            .Include(t => t.PaymentProvider)
            .ToListAsync(ct);
    }

    public async Task<List<SchoolPayReconciliationResultDto>> GetReconciliationDataAsync(int settlementId, CancellationToken ct = default)
    {
        var settlement = await _db.Set<PaymentGatewaySettlement>()
            .FirstOrDefaultAsync(s => s.Id == settlementId && !s.IsDeleted, ct);

        if (settlement == null) return new List<SchoolPayReconciliationResultDto>();

        var from = settlement.SettlementDate?.AddDays(-7) ?? DateTime.UtcNow.AddDays(-7);
        var to = settlement.SettlementDate?.AddDays(1) ?? DateTime.UtcNow;

        var transactions = await _db.Set<PaymentGatewayTransaction>()
            .Where(t => !t.IsDeleted
                && t.PaymentProviderId == settlement.PaymentProviderId
                && t.CompletedAt >= from
                && t.CompletedAt <= to)
            .OrderByDescending(t => t.CompletedAt)
            .ToListAsync(ct);

        var totalMatched = transactions.Where(t => t.Status == SchoolPayTransactionStatus.Completed).Sum(t => t.Amount);
        var difference = settlement.Amount - totalMatched;

        var lines = transactions.Select(t => new SchoolPayReconciliationLineDto
        {
            TransactionId = t.Id,
            TransactionReference = t.TransactionReference,
            Amount = t.Amount,
            Status = t.Status,
            CompletedAt = t.CompletedAt,
            IsMatched = t.Status == SchoolPayTransactionStatus.Completed
        }).ToList();

        return new List<SchoolPayReconciliationResultDto>
        {
            new SchoolPayReconciliationResultDto
            {
                SettlementId = settlement.Id,
                SettlementReference = settlement.SettlementReference,
                ProviderName = _db.Set<PaymentProvider>()
                    .Where(p => p.Id == settlement.PaymentProviderId)
                    .Select(p => p.Name)
                    .FirstOrDefault(),
                SettlementAmount = settlement.Amount,
                MatchedAmount = totalMatched,
                Difference = difference,
                MatchedTransactionCount = transactions.Count(t => t.Status == SchoolPayTransactionStatus.Completed),
                UnmatchedTransactionCount = transactions.Count(t => t.Status != SchoolPayTransactionStatus.Completed),
                Transactions = lines
            }
        };
    }

    // ═══════════════════════════════════════════
    //  SP-02: ANALYTICS
    // ═══════════════════════════════════════════

    public async Task<SchoolPayAnalyticsDto> GetAnalyticsAsync(int days = 30, CancellationToken ct = default)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        var transactions = await _db.Set<PaymentGatewayTransaction>()
            .Where(t => !t.IsDeleted && t.InitiatedAt >= since)
            .Include(t => t.PaymentProvider)
            .ToListAsync(ct);

        var volumeByDay = transactions
            .GroupBy(t => t.InitiatedAt?.Date ?? t.CompletedAt?.Date ?? DateTime.UtcNow.Date)
            .Select(g => new SchoolPayVolumeDayDto
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                TransactionCount = g.Count(),
                TotalAmount = g.Where(t => t.Status == SchoolPayTransactionStatus.Completed).Sum(t => t.Amount)
            })
            .OrderBy(v => v.Date)
            .ToList();

        var providerPerformance = transactions
            .GroupBy(t => t.PaymentProvider?.Name ?? "Unknown")
            .Select(g => new SchoolPayProviderPerformanceDto
            {
                ProviderName = g.Key,
                TotalTransactions = g.Count(),
                SuccessfulTransactions = g.Count(t => t.Status == SchoolPayTransactionStatus.Completed),
                FailedTransactions = g.Count(t => t.Status == SchoolPayTransactionStatus.Failed),
                SuccessRate = g.Count() > 0 ? Math.Round((decimal)g.Count(t => t.Status == SchoolPayTransactionStatus.Completed) / g.Count() * 100, 2) : 0,
                TotalAmount = g.Where(t => t.Status == SchoolPayTransactionStatus.Completed).Sum(t => t.Amount),
                AvgResponseTimeMs = 0
            })
            .OrderByDescending(p => p.TotalTransactions)
            .ToList();

        var successRateTrend = volumeByDay
            .Select(v => new SchoolPaySuccessRateTrendDto
            {
                Date = v.Date,
                SuccessRate = 0,
                TotalAmount = v.TotalAmount
            })
            .ToList();

        var completedCount = transactions.Count(t => t.Status == SchoolPayTransactionStatus.Completed);
        var failedCount = transactions.Count(t => t.Status == SchoolPayTransactionStatus.Failed);
        var totalAmount = transactions.Where(t => t.Status == SchoolPayTransactionStatus.Completed).Sum(t => t.Amount);

        var summary = new SchoolPayAnalyticsSummaryDto
        {
            TotalTransactions = transactions.Count,
            SuccessfulTransactions = completedCount,
            FailedTransactions = failedCount,
            TotalRevenue = totalAmount,
            AverageTransactionValue = completedCount > 0 ? Math.Round(totalAmount / completedCount, 2) : 0,
            OverallSuccessRate = transactions.Count > 0 ? Math.Round((decimal)completedCount / transactions.Count * 100, 2) : 0,
            ActiveProviders = providerPerformance.Count
        };

        return new SchoolPayAnalyticsDto
        {
            VolumeByDay = volumeByDay,
            ProviderPerformance = providerPerformance,
            SuccessRateTrend = successRateTrend,
            Summary = summary
        };
    }

    // ═══════════════════════════════════════════
    //  SP-03: OPERATIONS CENTER
    // ═══════════════════════════════════════════

    public async Task<SchoolPayOperationsDto> GetOperationsDataAsync(CancellationToken ct = default)
    {
        var transactions = await _db.Set<PaymentGatewayTransaction>()
            .Where(t => !t.IsDeleted)
            .ToListAsync(ct);

        var webhooks = await _db.Set<PaymentGatewayWebhook>()
            .Where(w => !w.IsDeleted)
            .ToListAsync(ct);

        var settlements = await _db.Set<PaymentGatewaySettlement>()
            .Where(s => !s.IsDeleted)
            .ToListAsync(ct);

        var refunds = await _db.Set<PaymentGatewayRefund>()
            .Where(r => !r.IsDeleted)
            .ToListAsync(ct);

        var health = await _db.Set<PaymentGatewayHealth>()
            .GroupBy(h => h.PaymentProviderId)
            .Select(g => g.OrderByDescending(h => h.LastCheckedAt).First())
            .ToListAsync(ct);

        var deadLetters = webhooks.Count(w => w.Status == SchoolPayWebhookStatus.DeadLetter);

        var alerts = new List<SchoolPayAlertDto>();

        var unhealthyProviders = health.Where(h => h.Status != ProviderHealthStatus.Healthy).ToList();
        foreach (var h in unhealthyProviders)
        {
            var providerName = await _db.Set<PaymentProvider>()
                .Where(p => p.Id == h.PaymentProviderId)
                .Select(p => p.Name)
                .FirstOrDefaultAsync(ct);

            alerts.Add(new SchoolPayAlertDto
            {
                Severity = h.Status == ProviderHealthStatus.Unhealthy ? "danger" : "warning",
                Message = $"{providerName} is {h.Status}",
                ActionUrl = "/SchoolPay/Provider",
                ActionText = "View Providers",
                Timestamp = h.LastCheckedAt ?? DateTime.UtcNow
            });
        }

        if (deadLetters > 0)
        {
            alerts.Add(new SchoolPayAlertDto
            {
                Severity = "warning",
                Message = $"{deadLetters} webhook(s) in Dead Letter Queue",
                ActionUrl = "/SchoolPay/DeadLetter",
                ActionText = "View DLQ",
                Timestamp = DateTime.UtcNow
            });
        }

        var pendingSettlements = settlements.Count(s => s.Status == SettlementStatus.Pending);
        if (pendingSettlements > 0)
        {
            alerts.Add(new SchoolPayAlertDto
            {
                Severity = "info",
                Message = $"{pendingSettlements} settlement(s) pending reconciliation",
                ActionUrl = "/SchoolPay/Reconciliation",
                ActionText = "Reconcile",
                Timestamp = DateTime.UtcNow
            });
        }

        return new SchoolPayOperationsDto
        {
            PendingPayments = transactions.Count(t => t.Status == SchoolPayTransactionStatus.Pending),
            GatewayPending = transactions.Count(t => t.Status == SchoolPayTransactionStatus.Processing),
            WebhookQueueSize = webhooks.Count(w => w.Status == SchoolPayWebhookStatus.Received || w.Status == SchoolPayWebhookStatus.Failed),
            SettlementPending = pendingSettlements,
            FailedPayments = transactions.Count(t => t.Status == SchoolPayTransactionStatus.Failed),
            RefundPending = refunds.Count(r => r.Status == RefundStatus.Requested || r.Status == RefundStatus.Approved),
            DisputedSettlements = settlements.Count(s => s.Status == SettlementStatus.Disputed),
            DeadLetterCount = deadLetters,
            HealthyProviders = health.Count(h => h.Status == ProviderHealthStatus.Healthy),
            DegradedProviders = health.Count(h => h.Status == ProviderHealthStatus.Degraded),
            UnhealthyProviders = health.Count(h => h.Status == ProviderHealthStatus.Unhealthy),
            Alerts = alerts,
            RecentTransactions = transactions
                .OrderByDescending(t => t.Id)
                .Take(10)
                .Select(t => new SchoolPayRecentTransactionDto
                {
                    TransactionReference = t.TransactionReference,
                    Amount = t.Amount,
                    ProviderName = t.PaymentProvider != null ? t.PaymentProvider.Name : "",
                    Status = t.Status,
                    InitiatedAt = t.InitiatedAt
                }).ToList()
        };
    }

    // ═══════════════════════════════════════════
    //  SP-03: DEAD LETTER QUEUE
    // ═══════════════════════════════════════════

    public async Task<List<SchoolPayDeadLetterDto>> GetDeadLetterItemsAsync(CancellationToken ct = default)
    {
        return await _db.Set<PaymentGatewayWebhook>()
            .Where(w => !w.IsDeleted && w.Status == SchoolPayWebhookStatus.DeadLetter)
            .OrderByDescending(w => w.Id)
            .Select(w => new SchoolPayDeadLetterDto
            {
                Id = w.Id,
                ProviderName = _db.Set<PaymentProvider>()
                    .Where(p => p.Id == w.PaymentProviderId)
                    .Select(p => p.Name)
                    .FirstOrDefault() ?? "",
                TransactionReference = w.TransactionReference,
                ProviderEventType = w.ProviderEventType,
                Status = w.Status,
                RawPayload = w.RawPayload,
                ErrorMessage = w.ErrorMessage,
                AttemptCount = w.AttemptCount,
                ReceivedAt = w.ReceivedAt,
                LastAttemptAt = w.ProcessedAt
            }).ToListAsync(ct);
    }

    public async Task<PaymentGatewayWebhook?> GetDeadLetterEntityByIdAsync(int id, CancellationToken ct = default)
        => await _db.Set<PaymentGatewayWebhook>()
            .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted && w.Status == SchoolPayWebhookStatus.DeadLetter, ct);

    public async Task ReprocessDeadLetterAsync(int id, CancellationToken ct = default)
    {
        var webhook = await _db.Set<PaymentGatewayWebhook>()
            .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted, ct);
        if (webhook != null)
        {
            webhook.Status = SchoolPayWebhookStatus.Received;
            webhook.AttemptCount = 0;
            webhook.ErrorMessage = null;
            webhook.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task IgnoreDeadLetterAsync(int id, CancellationToken ct = default)
    {
        var webhook = await _db.Set<PaymentGatewayWebhook>()
            .FirstOrDefaultAsync(w => w.Id == id && !w.IsDeleted, ct);
        if (webhook != null)
        {
            webhook.Status = SchoolPayWebhookStatus.Ignored;
            webhook.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }

    public async Task MoveToDeadLetterAsync(int webhookId, CancellationToken ct = default)
    {
        var webhook = await _db.Set<PaymentGatewayWebhook>()
            .FirstOrDefaultAsync(w => w.Id == webhookId && !w.IsDeleted, ct);
        if (webhook != null)
        {
            webhook.Status = SchoolPayWebhookStatus.DeadLetter;
            webhook.UpdatedAt = DateTime.UtcNow;
            await _db.SaveChangesAsync(ct);
        }
    }

    // ═══════════════════════════════════════════
    //  SP-03: PAYMENT TIMELINE
    // ═══════════════════════════════════════════

    public async Task<List<SchoolPayTimelineEntryDto>> GetTransactionTimelineAsync(int transactionId, CancellationToken ct = default)
    {
        return await _db.Set<PaymentGatewayAudit>()
            .Where(a => a.PaymentGatewayTransactionId == transactionId)
            .OrderBy(a => a.PerformedAt)
            .Select(a => new SchoolPayTimelineEntryDto
            {
                Id = a.Id,
                EventType = a.EventType,
                EventData = a.EventData,
                PerformedBy = a.PerformedBy,
                PerformedAt = a.PerformedAt,
                IpAddress = a.IpAddress
            }).ToListAsync(ct);
    }

    public async Task<SchoolPayTimelineDto?> GetFullTimelineAsync(int transactionId, CancellationToken ct = default)
    {
        var txn = await _db.Set<PaymentGatewayTransaction>()
            .FirstOrDefaultAsync(t => t.Id == transactionId && !t.IsDeleted, ct);

        if (txn == null) return null;

        var entries = await GetTransactionTimelineAsync(transactionId, ct);

        return new SchoolPayTimelineDto
        {
            TransactionId = txn.Id,
            TransactionReference = txn.TransactionReference,
            Amount = txn.Amount,
            Status = txn.Status,
            Entries = entries
        };
    }

    // ═══════════════════════════════════════════
    //  SP-04: SECURITY AUDIT
    // ═══════════════════════════════════════════

    public async Task<List<SchoolPaySecurityAuditEntryDto>> GetSecurityAuditLogAsync(int? providerId = null, int days = 30, CancellationToken ct = default)
    {
        var since = DateTime.UtcNow.AddDays(-days);
        var query = _db.Set<PaymentGatewaySecurityEvent>()
            .Where(e => e.PerformedAt >= since && !e.IsDeleted);

        if (providerId.HasValue)
            query = query.Where(e => e.PaymentProviderId == providerId.Value);

        return await query
            .OrderByDescending(e => e.PerformedAt)
            .Select(e => new SchoolPaySecurityAuditEntryDto
            {
                Id = e.Id,
                EventType = (int)e.EventType,
                EventTypeName = e.EventType.ToString(),
                PerformedBy = e.PerformedBy,
                IpAddress = e.IpAddress,
                Details = e.Details,
                PerformedAt = e.PerformedAt
            }).ToListAsync(ct);
    }

    public async Task LogSecurityEventAsync(PaymentSecurityEventType eventType, string? details, string? performedBy, string? ipAddress, CancellationToken ct = default)
    {
        _db.Set<PaymentGatewaySecurityEvent>().Add(new PaymentGatewaySecurityEvent
        {
            EventType = eventType,
            Details = details,
            PerformedBy = performedBy,
            IpAddress = ipAddress,
            PerformedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(ct);
    }

    // ═══════════════════════════════════════════
    //  SP-04: MONITORING
    // ═══════════════════════════════════════════

    public async Task<SchoolPayMonitoringDto> GetMonitoringDataAsync(CancellationToken ct = default)
    {
        var now = DateTime.UtcNow;
        var since24h = now.AddHours(-24);
        var since7d = now.AddDays(-7);

        var providers = await _db.Set<PaymentProvider>()
            .Where(p => !p.IsDeleted)
            .ToListAsync(ct);

        var uptimes = new List<SchoolPayProviderUptimeDto>();
        foreach (var p in providers)
        {
            var healthRecords = await _db.Set<PaymentGatewayHealth>()
                .Where(h => h.PaymentProviderId == p.Id && h.LastCheckedAt >= since7d)
                .ToListAsync(ct);

            var total = healthRecords.Count;
            var success = healthRecords.Count(h => h.Status == ProviderHealthStatus.Healthy);
            var avgResponse = healthRecords.Any() ? healthRecords.Average(h => (double)h.ResponseTimeMs) : 0;

            uptimes.Add(new SchoolPayProviderUptimeDto
            {
                ProviderName = p.Name,
                UptimePercentage = total > 0 ? Math.Round((double)success / total * 100, 2) : 100,
                TotalChecks = total,
                SuccessfulChecks = success,
                AvgResponseTimeMs = Math.Round(avgResponse, 2)
            });
        }

        var webhookLatencies = await _db.Set<PaymentGatewayWebhook>()
            .Where(w => !w.IsDeleted && w.ReceivedAt >= since7d && w.ProcessedAt != null)
            .GroupBy(w => w.ReceivedAt!.Value.Date)
            .Select(g => new SchoolPayWebhookLatencyDto
            {
                Date = g.Key.ToString("yyyy-MM-dd"),
                AvgProcessingTimeMs = g.Average(w => ((DateTime)w.ProcessedAt! - w.ReceivedAt!.Value).TotalMilliseconds),
                TotalProcessed = g.Count()
            })
            .OrderBy(w => w.Date)
            .ToListAsync(ct);

        var transactions24h = await _db.Set<PaymentGatewayTransaction>()
            .Where(t => !t.IsDeleted && t.InitiatedAt >= since24h)
            .ToListAsync(ct);

        var transactions7d = await _db.Set<PaymentGatewayTransaction>()
            .Where(t => !t.IsDeleted && t.InitiatedAt >= since7d)
            .ToListAsync(ct);

        var webhooks = await _db.Set<PaymentGatewayWebhook>().Where(w => !w.IsDeleted).ToListAsync(ct);
        var settlements = await _db.Set<PaymentGatewaySettlement>().Where(s => !s.IsDeleted).ToListAsync(ct);
        var refunds = await _db.Set<PaymentGatewayRefund>().Where(r => !r.IsDeleted).ToListAsync(ct);

        return new SchoolPayMonitoringDto
        {
            ProviderUptimes = uptimes,
            WebhookLatencies = webhookLatencies,
            QueueMetrics = new SchoolPayQueueMetricsDto
            {
                WebhookQueueDepth = webhooks.Count(w => w.Status == SchoolPayWebhookStatus.Received || w.Status == SchoolPayWebhookStatus.Failed),
                DlqDepth = webhooks.Count(w => w.Status == SchoolPayWebhookStatus.DeadLetter),
                PendingSettlements = settlements.Count(s => s.Status == SettlementStatus.Pending),
                PendingRefunds = refunds.Count(r => r.Status == RefundStatus.Requested || r.Status == RefundStatus.Approved)
            },
            Trends = new SchoolPayTrendDto
            {
                SuccessRate24h = transactions24h.Count > 0 ? Math.Round((double)transactions24h.Count(t => t.Status == SchoolPayTransactionStatus.Completed) / transactions24h.Count * 100, 2) : 0,
                SuccessRate7d = transactions7d.Count > 0 ? Math.Round((double)transactions7d.Count(t => t.Status == SchoolPayTransactionStatus.Completed) / transactions7d.Count * 100, 2) : 0,
                TotalTransactions24h = transactions24h.Count,
                TotalVolume24h = transactions24h.Where(t => t.Status == SchoolPayTransactionStatus.Completed).Sum(t => t.Amount)
            }
        };
    }

    // ═══════════════════════════════════════════
    //  SP-04: SECRET STORE
    // ═══════════════════════════════════════════

    public async Task<PaymentProviderConfiguration?> GetConfigByKeyAsync(int providerId, string key, CancellationToken ct = default)
        => await _db.Set<PaymentProviderConfiguration>()
            .FirstOrDefaultAsync(c => c.PaymentProviderId == providerId && c.Key == key && !c.IsDeleted, ct);

    public async Task UpdateConfigValueAsync(int providerId, string key, string encryptedValue, string updatedBy, CancellationToken ct = default)
    {
        var config = await _db.Set<PaymentProviderConfiguration>()
            .FirstOrDefaultAsync(c => c.PaymentProviderId == providerId && c.Key == key && !c.IsDeleted, ct);
        if (config != null)
        {
            config.Value = encryptedValue;
            config.UpdatedBy = updatedBy;
            config.UpdatedAt = DateTime.UtcNow;
        }
        else
        {
            _db.Set<PaymentProviderConfiguration>().Add(new PaymentProviderConfiguration
            {
                PaymentProviderId = providerId,
                Key = key,
                Value = encryptedValue,
                IsActive = true,
                CreatedBy = updatedBy,
                CreatedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task<List<SchoolPaySecretKeyDto>> GetSecretKeysAsync(int providerId, CancellationToken ct = default)
    {
        var configs = await _db.Set<PaymentProviderConfiguration>()
            .Where(c => c.PaymentProviderId == providerId && !c.IsDeleted)
            .ToListAsync(ct);

        return configs.Where(c => IsSecretKey(c.Key))
            .Select(c => new SchoolPaySecretKeyDto
            {
                Id = c.Id,
                KeyName = c.Key,
                KeyPreview = c.Value.Length > 4 ? $"****{c.Value[^4..]}" : "****",
                LastRotatedAt = c.UpdatedAt,
                IsActive = c.IsActive,
                Version = "v1"
            }).ToList();
    }

    private bool IsSecretKey(string key)
    {
        var secretKeywords = new[] { "api_key", "api_secret", "password", "secret", "token", "store_id", "signature_key" };
        return secretKeywords.Any(k => key.Contains(k, StringComparison.OrdinalIgnoreCase));
    }
}
