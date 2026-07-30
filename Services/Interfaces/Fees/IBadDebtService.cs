using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IBadDebtService
{
    Task<BadDebtResultDto> MarkAsBadDebtAsync(int invoiceId, string reason, string createdBy, CancellationToken ct = default);
    Task<BadDebtResultDto> MarkMultipleAsBadDebtAsync(List<int> invoiceIds, string reason, string createdBy, CancellationToken ct = default);
}
