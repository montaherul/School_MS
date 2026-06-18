using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IStudentFeeAssignmentRepository : IBaseRepository<StudentFeeAssignment>
{
    Task<(List<StudentFeeAssignmentListItemDto> items, int totalRecords)> GetListByStoredProcedureAsync(
        int pageNumber, int pageSize, string? searchTerm, int? studentId, int? feeStructureId, CancellationToken ct);
}
