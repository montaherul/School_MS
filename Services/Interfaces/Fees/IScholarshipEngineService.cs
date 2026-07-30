using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IScholarshipEngineService
{
    Task<ScholarshipEngineResultDto> RunAsync(CancellationToken ct = default);
}
