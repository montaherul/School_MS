using SchoolManagementSystem.Models.DTOs.Fees;

namespace SchoolManagementSystem.Repositories.Interfaces.Fees;

public interface IAutoBillingRepository
{
    Task<AutoBillingResultDto> GenerateMonthlyInvoicesAsync(int academicYearId, int dueDay = 10, int batchSize = 500, CancellationToken ct = default);
    Task<AutoBillingResultDto> GenerateOneTimeFeeInvoicesAsync(int academicYearId, int dueDay = 30, int batchSize = 500, CancellationToken ct = default);
    Task<AutoBillingResultDto> GenerateExamFeeInvoicesAsync(int academicYearId, string examName = "Term Exam", int dueDay = 15, int batchSize = 500, CancellationToken ct = default);
}
