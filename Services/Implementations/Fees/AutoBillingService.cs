using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class AutoBillingService : IAutoBillingService
{
    private readonly IAutoBillingRepository _autoBillingRepo;
    private readonly IAutoFeeAssignmentRepository _assignmentRepo;

    public AutoBillingService(IAutoBillingRepository autoBillingRepo, IAutoFeeAssignmentRepository assignmentRepo)
    {
        _autoBillingRepo = autoBillingRepo;
        _assignmentRepo = assignmentRepo;
    }

    public async Task<AutoBillingResultDto> GenerateMonthlyInvoicesAsync(int academicYearId, int dueDay = 10, CancellationToken ct = default)
    {
        return await _autoBillingRepo.GenerateMonthlyInvoicesAsync(academicYearId, dueDay, ct: ct);
    }

    public async Task<AutoAssignmentResultDto> AssignFeeStructureAsync(int studentId, int academicYearId, CancellationToken ct = default)
    {
        return await _assignmentRepo.AssignFeeStructureAsync(studentId, academicYearId, ct);
    }

    public async Task<FeeMigrationResultDto> MigrateFeeStructureAsync(int studentId, int oldClassId, int newClassId, int academicYearId, CancellationToken ct = default)
    {
        return await _assignmentRepo.MigrateFeeStructureAsync(studentId, oldClassId, newClassId, academicYearId, ct);
    }

    public async Task<FeeCopyResultDto> CopyFeeStructureForAcademicYearAsync(int fromAcademicYearId, int toAcademicYearId, CancellationToken ct = default)
    {
        return await _assignmentRepo.CopyFeeStructureForAcademicYearAsync(fromAcademicYearId, toAcademicYearId, ct);
    }
}
