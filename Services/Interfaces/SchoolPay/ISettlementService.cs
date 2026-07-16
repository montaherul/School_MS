using SchoolManagementSystem.Models.DTOs.SchoolPay;

namespace SchoolManagementSystem.Services.Interfaces.SchoolPay;

public interface ISettlementService
{
    Task<List<SchoolPaySettlementDto>> GetSettlementsAsync(CancellationToken ct = default);
    Task<SchoolPaySettlementDto?> GetSettlementByIdAsync(int id, CancellationToken ct = default);
    Task<bool> MarkAsSettledAsync(int id, string? providerSettlementId, CancellationToken ct = default);
}
