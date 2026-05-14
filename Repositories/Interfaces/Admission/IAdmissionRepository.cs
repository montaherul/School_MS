using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.DTOs.Admission;

namespace SchoolManagementSystem.Repositories.Interfaces.Admission;

public interface IAdmissionRepository : IBaseRepository<AdmissionApplication>
{
    Task<(List<AdmissionListResultDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int classId, int? status, CancellationToken ct);
}
