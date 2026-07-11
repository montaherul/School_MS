using SchoolManagementSystem.Models.DTOs.Academic;

namespace SchoolManagementSystem.Services.Interfaces.Academic;

public interface INctbComplianceService
{
    Task<NctbComplianceReportDto> GetComplianceReportAsync(int academicYearId, CancellationToken ct = default);
    Task<List<CurriculumVersionDto>> GetCurriculumVersionsAsync(CancellationToken ct = default);
    Task<CurriculumVersionDto> GetCurriculumVersionByIdAsync(int id, CancellationToken ct = default);
    Task<CurriculumVersionDto> CreateCurriculumVersionAsync(CurriculumVersionUpsertDto dto, CancellationToken ct = default);
    Task<CurriculumVersionDto> UpdateCurriculumVersionAsync(int id, CurriculumVersionUpsertDto dto, CancellationToken ct = default);
    Task<bool> DeleteCurriculumVersionAsync(int id, CancellationToken ct = default);
}
