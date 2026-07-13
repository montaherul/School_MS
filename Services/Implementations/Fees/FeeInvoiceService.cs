using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class FeeInvoiceService : IFeeInvoiceService
{
    private readonly IUnitOfWork _uow;
    private readonly IFeeInvoiceRepository _invoiceRepository;
    private readonly IAuditLogService _audit;

    public FeeInvoiceService(IUnitOfWork uow, IFeeInvoiceRepository invoiceRepository, IAuditLogService audit)
    {
        _uow = uow;
        _invoiceRepository = invoiceRepository;
        _audit = audit;
    }

    public async Task<PagedResult<FeeInvoiceListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? studentId = null, int? status = null, CancellationToken cancellationToken = default)
    {
        page = Math.Max(page, 1);
        pageSize = Math.Clamp(pageSize, 5, 100);
        var (items, totalCount) = await _invoiceRepository.GetListByStoredProcedureAsync(page, pageSize, search, studentId, status, cancellationToken);
        return new PagedResult<FeeInvoiceListItemDto> { Items = items, Page = page, PageSize = pageSize, TotalItems = totalCount };
    }

    public async Task<FeeInvoice?> GetByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _invoiceRepository.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);
    }

    public async Task<int> CreateAsync(FeeInvoice invoice, string createdBy, CancellationToken cancellationToken = default)
    {
        invoice.CreatedBy = createdBy;
        invoice.CreatedAt = DateTime.UtcNow;
        await _invoiceRepository.AddAsync(invoice, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        var ledger = new FeeLedger
        {
            StudentId = invoice.StudentId,
            FeeInvoiceId = invoice.Id,
            TransactionType = FeeLedgerType.Invoice,
            Debit = invoice.TotalAmount,
            Credit = 0,
            Balance = invoice.TotalAmount,
            Description = $"Invoice created: {invoice.InvoiceNo}",
            TransactionDate = DateTime.UtcNow,
            CreatedBy = createdBy,
            CreatedAt = DateTime.UtcNow
        };
        await _uow.Repository<FeeLedger>().AddAsync(ledger, cancellationToken);
        await _uow.SaveChangesAsync(cancellationToken);

        await _audit.LogAsync("FeeInvoices", "Create", $"Invoice {invoice.InvoiceNo} created for student {invoice.StudentId}, amount {invoice.TotalAmount}", createdBy, cancellationToken: cancellationToken);

        return invoice.Id;
    }

    public async Task UpdateAsync(FeeInvoice invoice, string updatedBy, CancellationToken cancellationToken = default)
    {
        var existing = await _invoiceRepository.FirstOrDefaultAsync(x => x.Id == invoice.Id && !x.IsDeleted, cancellationToken)
            ?? throw new Exception("Invoice not found");

        existing.InvoiceNo = invoice.InvoiceNo; existing.StudentId = invoice.StudentId; existing.AcademicYearId = invoice.AcademicYearId;
        existing.DueDate = invoice.DueDate; existing.TotalAmount = invoice.TotalAmount; existing.PaidAmount = invoice.PaidAmount;
        existing.DiscountAmount = invoice.DiscountAmount; existing.LateFee = invoice.LateFee; existing.Status = invoice.Status;
        existing.Remarks = invoice.Remarks;
        existing.UpdatedBy = updatedBy; existing.UpdatedAt = DateTime.UtcNow;

        _invoiceRepository.Update(existing);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var existing = await _invoiceRepository.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new Exception("Invoice not found");

        existing.IsDeleted = true; existing.UpdatedBy = updatedBy; existing.UpdatedAt = DateTime.UtcNow;
        _invoiceRepository.Update(existing);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task RestoreAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var existing = await _invoiceRepository.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("FeeInvoice not found or not deleted.");
        existing.IsDeleted = false;
        existing.UpdatedBy = updatedBy;
        existing.UpdatedAt = DateTime.UtcNow;
        _invoiceRepository.Update(existing);
        await _uow.SaveChangesAsync(cancellationToken);
    }

    public async Task<LateFeeEngineResultDto> ApplyLateFeesAsync(CancellationToken cancellationToken = default)
    {
        var result = new LateFeeEngineResultDto();

        var rules = (await _uow.Repository<LateFeeRule>()
            .ListAsync(x => x.IsActive && !x.IsDeleted, cancellationToken)).ToList();

        if (rules.Count == 0)
        {
            result.Errors.Add("No active late fee rules found.");
            return result;
        }

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        var overdueInvoices = (await _uow.Repository<FeeInvoice>()
            .ListAsync(x => !x.IsDeleted
                && x.DueDate < today
                && x.Status != PaymentStatus.Paid
                && x.Status != PaymentStatus.Waived, cancellationToken)).ToList();

        if (overdueInvoices.Count == 0)
        {
            result.Errors.Add("No overdue invoices found.");
            return result;
        }

        var studentIds = overdueInvoices.Select(i => i.StudentId).Distinct().ToList();
        var students = (await _uow.Repository<Student>()
            .ListAsync(x => studentIds.Contains(x.Id) && !x.IsDeleted, cancellationToken)).ToList();
        var studentClassMap = students.ToDictionary(s => s.Id, s => s.ClassId);

        var invoiceIds = overdueInvoices.Select(i => i.Id).ToList();
        var allItems = (await _uow.Repository<FeeInvoiceItem>()
            .ListAsync(x => invoiceIds.Contains(x.FeeInvoiceId) && !x.IsDeleted, cancellationToken)).ToList();
        var invoiceCategoryIds = allItems
            .GroupBy(i => i.FeeInvoiceId)
            .ToDictionary(g => g.Key, g => g.Select(i => i.FeeCategoryId).Distinct().ToList());

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            foreach (var invoice in overdueInvoices)
            {
                try
                {
                    if (invoice.LateFee > 0) continue;

                    var hasLedger = await _uow.Repository<FeeLedger>().AnyAsync(
                        x => x.FeeInvoiceId == invoice.Id
                            && x.TransactionType == FeeLedgerType.LateFee
                            && !x.IsDeleted,
                        cancellationToken);
                    if (hasLedger) continue;

                    if (!studentClassMap.TryGetValue(invoice.StudentId, out var classId))
                        continue;

                    var invoiceCats = invoiceCategoryIds.GetValueOrDefault(invoice.Id, []);

                    LateFeeRule? selectedRule = null;
                    var selectedPriority = int.MaxValue;

                    foreach (var rule in rules)
                    {
                        var classMatch = rule.SchoolClassId == null || rule.SchoolClassId == classId;
                        var categoryMatch = rule.FeeCategoryId == null || invoiceCats.Contains(rule.FeeCategoryId);
                        if (!classMatch || !categoryMatch) continue;

                        var priority = (rule.SchoolClassId != null, rule.FeeCategoryId != null) switch
                        {
                            (true, true) => 1,
                            (true, false) => 2,
                            (false, true) => 3,
                            (false, false) => 4
                        };

                        if (priority < selectedPriority)
                        {
                            selectedPriority = priority;
                            selectedRule = rule;
                        }
                    }

                    if (selectedRule == null) continue;

                    var daysLate = today.DayNumber - invoice.DueDate.DayNumber;
                    var daysOverdue = daysLate - selectedRule.GraceDays;
                    if (daysOverdue <= 0) continue;

                    decimal lateFeeAmount;
                    if (selectedRule.FeeType == FeeDiscountType.Percentage)
                    {
                        lateFeeAmount = invoice.TotalAmount * selectedRule.FeeValue / 100m;
                        if (selectedRule.MaxFee > 0 && lateFeeAmount > selectedRule.MaxFee)
                            lateFeeAmount = selectedRule.MaxFee;
                    }
                    else
                    {
                        lateFeeAmount = selectedRule.FeeValue;
                    }

                    if (lateFeeAmount <= 0) continue;

                    invoice.LateFee += lateFeeAmount;
                    invoice.UpdatedAt = DateTime.UtcNow;
                    _uow.Repository<FeeInvoice>().Update(invoice);

                    var ledger = new FeeLedger
                    {
                        StudentId = invoice.StudentId,
                        FeeInvoiceId = invoice.Id,
                        TransactionType = FeeLedgerType.LateFee,
                        Debit = lateFeeAmount,
                        Credit = 0,
                        Balance = lateFeeAmount,
                        Description = $"Late fee: {selectedRule.Name} ({daysOverdue} day(s) overdue, grace {selectedRule.GraceDays})",
                        TransactionDate = DateTime.UtcNow,
                        CreatedBy = "system",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _uow.Repository<FeeLedger>().AddAsync(ledger, cancellationToken);

                    result.InvoicesProcessed++;
                    result.TotalLateFeeApplied += lateFeeAmount;
                }
                catch (Exception ex)
                {
                    result.Errors.Add($"Invoice #{invoice.Id}: {ex.Message}");
                }
            }

            await _uow.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        if (result.InvoicesProcessed == 0 && result.Errors.Count == 0)
            result.Errors.Add("No invoices required late fee application.");

        return result;
    }

    public async Task<AutoBillingResultDto> CancelInvoiceAsync(int invoiceId, string reason, string cancelledBy, CancellationToken cancellationToken = default)
    {
        var invoice = await _invoiceRepository.FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Invoice not found.");

        if (invoice.Status == PaymentStatus.Paid)
            return new AutoBillingResultDto { ErrorMessage = "Cannot cancel a fully paid invoice. Process a refund instead." };

        if (invoice.Status == PaymentStatus.Waived || invoice.Status == PaymentStatus.Cancelled || invoice.Status == PaymentStatus.Refunded)
            return new AutoBillingResultDto { ErrorMessage = "Invoice is already closed (waived, cancelled, or refunded)." };

        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var payments = await _uow.Repository<Payment>()
                .ListAsync(x => x.FeeInvoiceId == invoiceId && !x.IsDeleted, cancellationToken);

            foreach (var payment in payments)
            {
                payment.IsDeleted = true;
                payment.UpdatedBy = cancelledBy;
                payment.UpdatedAt = DateTime.UtcNow;
                _uow.Repository<Payment>().Update(payment);

                var refund = new FeeRefund
                {
                    FeePaymentId = payment.Id,
                    RefundAmount = payment.Amount,
                    RefundMethod = payment.Method,
                    ReferenceNo = $"CNCL-{invoice.InvoiceNo}",
                    Reason = reason,
                    IsApproved = true,
                    ApprovedBy = cancelledBy,
                    ApprovedAt = DateTime.UtcNow,
                    RefundDate = DateTime.UtcNow,
                    CreatedBy = cancelledBy,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Repository<FeeRefund>().AddAsync(refund, cancellationToken);

                var refundLedger = new FeeLedger
                {
                    StudentId = invoice.StudentId,
                    FeeInvoiceId = invoiceId,
                    FeePaymentId = payment.Id,
                    FeeRefundId = refund.Id,
                    TransactionType = FeeLedgerType.Refund,
                    Debit = payment.Amount,
                    Credit = 0,
                    Balance = payment.Amount,
                    Description = $"Auto-refund on invoice cancel: {reason}",
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = cancelledBy,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Repository<FeeLedger>().AddAsync(refundLedger, cancellationToken);
            }

            invoice.Status = PaymentStatus.Cancelled;
            invoice.PaidAmount = 0;
            invoice.Remarks = $"Cancelled: {reason}";
            invoice.UpdatedBy = cancelledBy;
            invoice.UpdatedAt = DateTime.UtcNow;
            _invoiceRepository.Update(invoice);

            var cancelLedger = new FeeLedger
            {
                StudentId = invoice.StudentId,
                FeeInvoiceId = invoiceId,
                TransactionType = FeeLedgerType.Adjustment,
                Debit = 0,
                Credit = invoice.TotalAmount,
                Balance = -invoice.TotalAmount,
                Description = $"Invoice cancelled: {reason}",
                TransactionDate = DateTime.UtcNow,
                CreatedBy = cancelledBy,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Repository<FeeLedger>().AddAsync(cancelLedger, cancellationToken);

            await _uow.SaveChangesAsync(cancellationToken);
        }, cancellationToken);

        await _audit.LogAsync("FeeInvoices", "Cancel", $"Invoice {invoice.InvoiceNo} cancelled. Reason: {reason}", cancelledBy, cancellationToken: cancellationToken);

        return new AutoBillingResultDto { InvoicesGenerated = 1 };
    }
}
