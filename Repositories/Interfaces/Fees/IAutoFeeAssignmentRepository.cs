using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IAutoFeeAssignmentRepository
{
    Task<AutoAssignmentResultDto> AssignFeeStructureAsync(int studentId, int academicYearId, CancellationToken ct = default);
    Task<FeeMigrationResultDto> MigrateFeeStructureAsync(int studentId, int oldClassId, int newClassId, int academicYearId, CancellationToken ct = default);
    Task<FeeCopyResultDto> CopyFeeStructureForAcademicYearAsync(int fromAcademicYearId, int toAcademicYearId, CancellationToken ct = default);
}
