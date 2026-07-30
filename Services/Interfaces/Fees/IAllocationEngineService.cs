using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IAllocationEngineService
{
    Task<AllocationEngineResultDto> RunAsync(CancellationToken ct = default);
    Task<AllocationEngineResultDto> AllocateForPaymentAsync(int paymentId, CancellationToken ct = default);
}
