using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class LateFeeEngineService : ILateFeeEngineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _audit;

    public LateFeeEngineService(IUnitOfWork unitOfWork, IAuditLogService audit)
    {
        _unitOfWork = unitOfWork;
        _audit = audit;
    }

    public async Task<LateFeeEngineResultDto> RunAsync(CancellationToken cancellationToken = default)
    {
        var result = new LateFeeEngineResultDto();

        var rules = await _unitOfWork.Repository<LateFeeRule>().ListAsync(
            x => x.IsActive && !x.IsDeleted, cancellationToken);

        var overdueInvoices = await _unitOfWork.Repository<FeeInvoice>().ListAsync(
            x => !x.IsDeleted && (x.Status == PaymentStatus.Issued || x.Status == PaymentStatus.Partial)
                 && x.DueDate < DateOnly.FromDateTime(DateTime.UtcNow), cancellationToken);

        foreach (var invoice in overdueInvoices)
        {
            try
            {
                var studentClassId = await GetStudentClassIdAsync(invoice.StudentId, cancellationToken);

                var matchingRule = rules.FirstOrDefault(r =>
                    r.IsActive &&
                    (r.SchoolClassId == null || r.SchoolClassId == studentClassId));

                if (matchingRule == null) continue;

                var existingLateFee = await _unitOfWork.Repository<FeeLedger>().AnyAsync(
                    x => x.FeeInvoiceId == invoice.Id && x.TransactionType == FeeLedgerType.LateFee && !x.IsDeleted,
                    cancellationToken);
                if (existingLateFee) continue;

                var daysOverdue = DateTime.UtcNow.Date - invoice.DueDate.ToDateTime(TimeOnly.MinValue);
                var overdueDays = Math.Max(0, daysOverdue.Days - matchingRule.GraceDays);

                if (overdueDays <= 0) continue;

                decimal lateFeeAmount;
                if (matchingRule.FeeType == FeeDiscountType.Percentage)
                {
                    var perDayRate = (matchingRule.FeeValue / 100m) * invoice.TotalAmount;
                    lateFeeAmount = perDayRate * overdueDays;
                }
                else
                {
                    lateFeeAmount = matchingRule.FeeValue * overdueDays;
                }

                if (matchingRule.MaxFee > 0 && lateFeeAmount > matchingRule.MaxFee)
                    lateFeeAmount = matchingRule.MaxFee;

                invoice.LateFee += lateFeeAmount;
                invoice.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<FeeInvoice>().Update(invoice);

                var ledger = new FeeLedger
                {
                    StudentId = invoice.StudentId,
                    FeeInvoiceId = invoice.Id,
                    TransactionType = FeeLedgerType.LateFee,
                    Debit = lateFeeAmount,
                    Credit = 0,
                    Balance = lateFeeAmount,
                    Description = $"Late fee: {overdueDays} day(s) overdue at {matchingRule.FeeValue}{(matchingRule.FeeType == FeeDiscountType.Percentage ? "%" : "/day")}",
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = "system",
                    CreatedAt = DateTime.UtcNow
                };
                await _unitOfWork.Repository<FeeLedger>().AddAsync(ledger, cancellationToken);

                result.InvoicesProcessed++;
                result.TotalLateFeeApplied += lateFeeAmount;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Invoice #{invoice.Id}: {ex.Message}");
            }
        }

        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _audit.LogAsync("LateFees", "Apply", $"Late fee engine run: {result.InvoicesProcessed} invoice(s) processed, total {result.TotalLateFeeApplied} applied, {result.Errors.Count} error(s).", "system", cancellationToken: cancellationToken);

        return result;
    }

    private async Task<int?> GetStudentClassIdAsync(int studentId, CancellationToken cancellationToken)
    {
        var student = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
            .FirstOrDefaultAsync(x => x.Id == studentId && !x.IsDeleted, cancellationToken);
        return student?.ClassId;
    }
}
