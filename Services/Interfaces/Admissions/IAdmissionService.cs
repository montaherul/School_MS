using SchoolManagementSystem.Models.DTOs.Admission;

namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface IAdmissionService
{
    Task<string> SubmitAsync(AdmissionCreateDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<int> ApproveAndConvertAsync(int applicationId, int sectionId, string approvedBy, CancellationToken cancellationToken = default);
    Task RejectAsync(int applicationId, string rejectedBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(int id, AdmissionCreateDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task<(List<AdmissionListResultDto> items, int totalRecords, object counts)> GetListByStoredProcedureAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        int classId = 0,
        CancellationToken cancellationToken = default,
        int? status = null);

    // Metadata lookups
    Task<SchoolManagementSystem.Models.Entities.Admission.AdmissionApplication?> GetByIdAsync(int id, CancellationToken ct = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default);
    Task<IEnumerable<dynamic>> GetAvailableClassesAsync(CancellationToken ct = default);
}

