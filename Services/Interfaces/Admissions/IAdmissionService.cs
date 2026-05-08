using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.DTOs.Admission.StoredProcedures;

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
}
