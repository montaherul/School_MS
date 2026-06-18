using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IStudentFeeAssignmentService
{
    Task<PagedResult<StudentFeeAssignmentListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, int? feeStructureId = null, CancellationToken cancellationToken = default);
    Task<StudentFeeAssignmentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default);
    Task<int> CreateAsync(StudentFeeAssignmentUpsertDto dto, string createdBy, CancellationToken cancellationToken = default);
    Task UpdateAsync(StudentFeeAssignmentUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default);
    Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
    Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default);
}
