using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Repositories.Interfaces.Academic;

public interface INctbComplianceRepository
{
    Task<NctbComplianceSpResult> GetComplianceReportSpAsync(int academicYearId, CancellationToken ct = default);
}
