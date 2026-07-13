using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IAutoBillingService
{
    Task<AutoBillingResultDto> GenerateMonthlyInvoicesAsync(int academicYearId, int dueDay = 10, CancellationToken ct = default);
    Task<AutoAssignmentResultDto> AssignFeeStructureAsync(int studentId, int academicYearId, CancellationToken ct = default);
    Task<FeeMigrationResultDto> MigrateFeeStructureAsync(int studentId, int oldClassId, int newClassId, int academicYearId, CancellationToken ct = default);
    Task<FeeCopyResultDto> CopyFeeStructureForAcademicYearAsync(int fromAcademicYearId, int toAcademicYearId, CancellationToken ct = default);
}
