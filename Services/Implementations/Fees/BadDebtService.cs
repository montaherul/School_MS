using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class BadDebtService : IBadDebtService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _audit;
    private readonly IFinancialPeriodRepository _periodRepo;
    private readonly IJournalEntryRepository _journalRepo;

    public BadDebtService(
        IUnitOfWork unitOfWork,
        IAuditLogService audit,
        IFinancialPeriodRepository periodRepo,
        IJournalEntryRepository journalRepo)
    {
        _unitOfWork = unitOfWork;
        _audit = audit;
        _periodRepo = periodRepo;
        _journalRepo = journalRepo;
    }

    public async Task<BadDebtResultDto> MarkAsBadDebtAsync(int invoiceId, string reason, string createdBy, CancellationToken ct = default)
    {
        var result = new BadDebtResultDto();

        var invoice = await _unitOfWork.Repository<FeeInvoice>()
            .FirstOrDefaultAsync(i => i.Id == invoiceId && !i.IsDeleted, ct);
        if (invoice == null)
        {
            result.Errors.Add($"Invoice #{invoiceId} not found.");
            return result;
        }

        var outstanding = invoice.TotalAmount - invoice.PaidAmount;
        if (outstanding <= 0)
        {
            result.Errors.Add($"Invoice #{invoiceId} has no outstanding balance.");
            return result;
        }

        var periodId = await GetOrCreateActivePeriodAsync(ct);
        var journalNo = $"BD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

        var entry = new JournalEntry
        {
            CreatedBy = createdBy,
            JournalNo = journalNo,
            EntryDate = DateTime.UtcNow,
            EntryType = JournalEntryType.Adjustment,
            Description = $"Bad debt - Invoice #{invoiceId}: {reason}",
            FinancialPeriodId = periodId,
            ReferenceId = invoiceId,
            ReferenceType = "BadDebt",
            IsPosted = false
        };
        await _unitOfWork.Repository<JournalEntry>().AddAsync(entry, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        var badDebtAccount = await _unitOfWork.Repository<ChartOfAccount>()
            .FirstOrDefaultAsync(a => a.AccountCode == "4-301" && !a.IsDeleted, ct);
        var receivableAccount = await _unitOfWork.Repository<ChartOfAccount>()
            .FirstOrDefaultAsync(a => a.AccountCode == "1-101" && !a.IsDeleted, ct);

        var debitLine = new JournalEntryLine
        {
            CreatedBy = createdBy,
            JournalEntryId = entry.Id,
            AccountId = badDebtAccount?.Id ?? 1,
            LineType = JournalLineType.Debit,
            Amount = outstanding,
            Narration = $"Bad debt Invoice #{invoiceId}: {reason}"
        };
        await _unitOfWork.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

        var creditLine = new JournalEntryLine
        {
            CreatedBy = createdBy,
            JournalEntryId = entry.Id,
            AccountId = receivableAccount?.Id ?? 1,
            LineType = JournalLineType.Credit,
            Amount = outstanding,
            Narration = $"Bad debt Invoice #{invoiceId}"
        };
        await _unitOfWork.Repository<JournalEntryLine>().AddAsync(creditLine, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _journalRepo.PostJournalEntryAsync(entry.Id, createdBy, ct);

        invoice.Status = PaymentStatus.Waived;
        invoice.Remarks = $"Bad debt: {reason}";
        invoice.UpdatedAt = DateTime.UtcNow;
        invoice.UpdatedBy = createdBy;
        _unitOfWork.Repository<FeeInvoice>().Update(invoice);
        await _unitOfWork.SaveChangesAsync(ct);

        result.InvoicesMarked = 1;
        result.TotalAmount = outstanding;
        result.JournalEntryId = entry.Id;

        await _audit.LogAsync("BadDebt", "Mark",
            $"Invoice #{invoiceId} marked as bad debt: {outstanding}. Reason: {reason}. Journal: #{entry.Id}", createdBy, cancellationToken: ct);

        return result;
    }

    public async Task<BadDebtResultDto> MarkMultipleAsBadDebtAsync(List<int> invoiceIds, string reason, string createdBy, CancellationToken ct = default)
    {
        var combined = new BadDebtResultDto();

        foreach (var id in invoiceIds)
        {
            var singleResult = await MarkAsBadDebtAsync(id, reason, createdBy, ct);
            combined.InvoicesMarked += singleResult.InvoicesMarked;
            combined.TotalAmount += singleResult.TotalAmount;
            combined.Errors.AddRange(singleResult.Errors);
        }

        return combined;
    }

    private async Task<int> GetOrCreateActivePeriodAsync(CancellationToken ct)
    {
        var active = await _periodRepo.GetActivePeriodAsync(ct);
        if (active != null) return active.Id;

        var now = DateTime.UtcNow;
        var period = new FinancialPeriod
        {
            CreatedBy = "system",
            Name = $"FY {now.Year}-{now.Year + 1}",
            StartDate = new DateTime(now.Year, 1, 1),
            EndDate = new DateTime(now.Year, 12, 31),
            Status = FinancialPeriodStatus.Open,
            IsActive = true
        };
        await _unitOfWork.Repository<FinancialPeriod>().AddAsync(period, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return period.Id;
    }
}
