using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class AutoWriteOffService : IAutoWriteOffService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAuditLogService _audit;
    private readonly IFinancialPeriodRepository _periodRepo;
    private readonly IJournalEntryRepository _journalRepo;

    public AutoWriteOffService(
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

    public async Task<AutoWriteOffResultDto> RunAsync(CancellationToken ct = default)
    {
        var result = new AutoWriteOffResultDto();

        var threshold = 1.00m;

        var smallBalanceInvoices = await _unitOfWork.Repository<FeeInvoice>().ListAsync(
            i => !i.IsDeleted
                 && (i.Status == PaymentStatus.Issued || i.Status == PaymentStatus.Partial)
                 && (i.TotalAmount - i.PaidAmount) > 0
                 && (i.TotalAmount - i.PaidAmount) <= threshold, ct);

        if (smallBalanceInvoices.Count == 0) return result;

        var periodId = await GetOrCreateActivePeriodAsync(ct);

        foreach (var invoice in smallBalanceInvoices)
        {
            try
            {
                var writeOffAmount = invoice.TotalAmount - invoice.PaidAmount;

                var journalNo = $"WO-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
                var entry = new JournalEntry
                {
                    CreatedBy = "system",
                    JournalNo = journalNo,
                    EntryDate = DateTime.UtcNow,
                    EntryType = JournalEntryType.Adjustment,
                    Description = $"Auto write-off - Invoice #{invoice.Id} (balance: {writeOffAmount})",
                    FinancialPeriodId = periodId,
                    ReferenceId = invoice.Id,
                    ReferenceType = "WriteOff",
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
                    CreatedBy = "system",
                    JournalEntryId = entry.Id,
                    AccountId = badDebtAccount?.Id ?? 1,
                    LineType = JournalLineType.Debit,
                    Amount = writeOffAmount,
                    Narration = $"Write-off Invoice #{invoice.Id}"
                };
                await _unitOfWork.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

                var creditLine = new JournalEntryLine
                {
                    CreatedBy = "system",
                    JournalEntryId = entry.Id,
                    AccountId = receivableAccount?.Id ?? 1,
                    LineType = JournalLineType.Credit,
                    Amount = writeOffAmount,
                    Narration = $"Write-off Invoice #{invoice.Id}"
                };
                await _unitOfWork.Repository<JournalEntryLine>().AddAsync(creditLine, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                await _journalRepo.PostJournalEntryAsync(entry.Id, "system", ct);

                invoice.Status = PaymentStatus.Waived;
                invoice.Remarks = $"Auto written-off (balance {writeOffAmount}). {invoice.Remarks}";
                invoice.UpdatedAt = DateTime.UtcNow;
                _unitOfWork.Repository<FeeInvoice>().Update(invoice);
                await _unitOfWork.SaveChangesAsync(ct);

                result.InvoicesWrittenOff++;
                result.TotalWrittenOff += writeOffAmount;
            }
            catch (Exception ex)
            {
                result.Errors.Add($"Invoice #{invoice.Id}: {ex.Message}");
            }
        }

        await _audit.LogAsync("AutoWriteOff", "Run",
            $"Auto write-off: {result.InvoicesWrittenOff} invoice(s) written off, total {result.TotalWrittenOff}, {result.Errors.Count} error(s)", "system", cancellationToken: ct);

        return result;
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
