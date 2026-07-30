using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IAutoWriteOffService
{
    Task<AutoWriteOffResultDto> RunAsync(CancellationToken ct = default);
}
