using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface ILateFeeEngineService
{
    Task<LateFeeEngineResultDto> RunAsync(CancellationToken cancellationToken = default);
}
