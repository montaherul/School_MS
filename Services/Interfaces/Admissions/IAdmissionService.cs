using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Admission;

namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface IAdmissionService
{
    Task<string> SubmitAsync(AdmissionCreateDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task<int> ApproveAndConvertAsync(int applicationId, int sectionId, string approvedBy, CancellationToken cancellationToken = default);
    Task RejectAsync(int applicationId, string rejectedBy, string? rejectionReason = null, CancellationToken cancellationToken = default);
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
    Task<IEnumerable<dynamic>> GetActiveStudentGroupsAsync(CancellationToken ct = default);
    Task<WorkflowInstance> InitializeWorkflowAsync(int applicationId, CancellationToken ct = default);
    Task<AdmissionTimelineDto> GetTimelineAsync(int applicationId, CancellationToken ct = default);

    // Bulk Operations
    Task<BulkOperationProgress> BulkApproveAsync(List<int> ids, int sectionId, string approvedBy, CancellationToken ct = default);
    Task<BulkOperationProgress> BulkRejectAsync(List<int> ids, string rejectedBy, string? reason = null, CancellationToken ct = default);
    Task<BulkOperationProgress> BulkDeleteAsync(List<int> ids, string updatedBy, CancellationToken ct = default);
    Task<BulkOperationProgress> BulkRestoreAsync(List<int> ids, string updatedBy, CancellationToken ct = default);
    Task<BulkOperationProgress> BulkExportAsync(List<int> ids, CancellationToken ct = default);
    Task<byte[]> BulkExportExcelAsync(List<int>? ids = null, CancellationToken ct = default);
}

