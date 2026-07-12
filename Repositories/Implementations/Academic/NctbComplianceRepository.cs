using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Academic;

namespace SchoolManagementSystem.Repositories.Implementations.Academic;

public class NctbComplianceRepository : BaseRepository<AcademicYear>, INctbComplianceRepository
{
    public NctbComplianceRepository(SchoolDbContext db) : base(db) { }

    public async Task<NctbComplianceSpResult> GetComplianceReportSpAsync(int academicYearId, CancellationToken ct = default)
    {
        var results = await ExecuteStoredProcAsync<NctbComplianceSpResult>(
            "sp_GetNctbComplianceReport", academicYearId);

        return results.FirstOrDefault() ?? new NctbComplianceSpResult
        {
            AcademicYearId = academicYearId,
            AcademicYearName = "Unknown"
        };
    }
}
