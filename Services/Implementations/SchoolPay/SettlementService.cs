using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Services.Implementations.SchoolPay;

public class SettlementService : ISettlementService
{
    private readonly ISchoolPayRepository _repository;
    private readonly ILogger<SettlementService> _logger;

    public SettlementService(ISchoolPayRepository repository, ILogger<SettlementService> logger)
    {
        _repository = repository;
        _logger = logger;
    }

    public async Task<List<SchoolPaySettlementDto>> GetSettlementsAsync(CancellationToken ct = default)
        => await _repository.GetSettlementsAsync(ct);

    public async Task<SchoolPaySettlementDto?> GetSettlementByIdAsync(int id, CancellationToken ct = default)
        => await _repository.GetSettlementByIdAsync(id, ct);

    public async Task<bool> MarkAsSettledAsync(int id, string? providerSettlementId, CancellationToken ct = default)
    {
        var settlement = await _repository.GetSettlementEntityByIdAsync(id, ct);
        if (settlement == null) return false;

        settlement.Status = SettlementStatus.Settled;
        settlement.ProviderSettlementId = providerSettlementId;
        settlement.SettlementDate = DateTime.UtcNow;
        settlement.UpdatedAt = DateTime.UtcNow;
        await _repository.UpdateSettlementAsync(settlement, ct);
        return true;
    }
}
