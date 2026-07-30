using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IScholarshipBatchRepository
{
    Task<ScholarshipEngineResultDto> ApplyScholarshipsAsync(CancellationToken ct = default);
}
