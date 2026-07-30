using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IAllocationBatchRepository
{
    Task<AllocationEngineResultDto> RunBatchAllocationAsync(CancellationToken ct = default);
}
