using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Accounting;

public class FinancePostingService : IFinancePostingService
{
    private readonly IUnitOfWork _uow;
    private readonly IFinancialPeriodRepository _periodRepo;
    private readonly IJournalEntryRepository _journalRepo;

    public FinancePostingService(IUnitOfWork uow, IFinancialPeriodRepository periodRepo, IJournalEntryRepository journalRepo)
    {
        _uow = uow;
        _periodRepo = periodRepo;
        _journalRepo = journalRepo;
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
        await _uow.Repository<FinancialPeriod>().AddAsync(period, ct);
        await _uow.SaveChangesAsync(ct);
        return period.Id;
    }

    public async Task<int> PostFeeCollectionAsync(int studentId, decimal amount, int invoiceId, string createdBy, CancellationToken ct)
    {
        var cashAccount = await _uow.Repository<ChartOfAccount>()
            .FirstOrDefaultAsync(a => a.AccountCode == "1-001" && !a.IsDeleted, ct);
        var accountId = cashAccount?.Id
            ?? throw new InvalidOperationException("Default cash account (1-001) not found. Set up a Cash in Hand account first.");
        return await PostFeeCollectionAsync(studentId, amount, invoiceId, accountId, createdBy, ct);
    }

    public async Task<int> PostFeeCollectionAsync(int studentId, decimal amount, int invoiceId, int accountId, string createdBy, CancellationToken ct)
    {
        var entryId = 0;
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var periodId = await GetOrCreateActivePeriodAsync(ct);
            var journalNo = $"FC-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var entry = new JournalEntry
            {
                CreatedBy = createdBy,
                JournalNo = journalNo,
                EntryDate = DateTime.UtcNow,
                EntryType = JournalEntryType.FeeCollection,
                Description = $"Fee collection - Invoice #{invoiceId}",
                FinancialPeriodId = periodId,
                ReferenceId = invoiceId,
                ReferenceType = "FeeInvoice",
                IsPosted = false
            };
            await _uow.Repository<JournalEntry>().AddAsync(entry, ct);
            await _uow.SaveChangesAsync(ct);

            // Debit: Cash/Bank account
            var debitLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = accountId,
                LineType = JournalLineType.Debit,
                Amount = amount,
                Narration = $"Fee collection - Invoice #{invoiceId}"
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

            // Credit: Student Receivable account (find or use default)
            var receivableAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "1-101" && !a.IsDeleted, ct);
            var receivableId = receivableAccount?.Id ?? accountId;

            var creditLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = receivableId,
                LineType = JournalLineType.Credit,
                Amount = amount,
                Narration = $"Fee collection - Invoice #{invoiceId}"
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(creditLine, ct);
            await _uow.SaveChangesAsync(ct);

            await PostJournalEntryInternal(entry.Id, createdBy, ct);
            entryId = entry.Id;
        }, ct);

        return entryId;
    }

    public async Task<int> PostFeeWaiverAsync(int studentId, decimal amount, string description, string createdBy, CancellationToken ct)
    {
        var entryId = 0;
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var periodId = await GetOrCreateActivePeriodAsync(ct);
            var journalNo = $"FW-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var entry = new JournalEntry
            {
                CreatedBy = createdBy,
                JournalNo = journalNo,
                EntryDate = DateTime.UtcNow,
                EntryType = JournalEntryType.Adjustment,
                Description = description,
                FinancialPeriodId = periodId,
                IsPosted = false
            };
            await _uow.Repository<JournalEntry>().AddAsync(entry, ct);
            await _uow.SaveChangesAsync(ct);

            var waiverAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "3-501" && !a.IsDeleted, ct);
            var receivableAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "1-101" && !a.IsDeleted, ct);

            var debitLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = receivableAccount?.Id ?? 1,
                LineType = JournalLineType.Debit,
                Amount = -amount,
                Narration = description
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

            var creditLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = waiverAccount?.Id ?? 1,
                LineType = JournalLineType.Credit,
                Amount = -amount,
                Narration = description
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(creditLine, ct);
            await _uow.SaveChangesAsync(ct);

            await PostJournalEntryInternal(entry.Id, createdBy, ct);
            entryId = entry.Id;
        }, ct);

        return entryId;
    }

    public async Task<int> PostBankReceiptAsync(int accountId, decimal amount, string referenceNo, string description, string createdBy, CancellationToken ct)
    {
        var entryId = 0;
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var periodId = await GetOrCreateActivePeriodAsync(ct);
            var journalNo = $"BR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var entry = new JournalEntry
            {
                CreatedBy = createdBy,
                JournalNo = journalNo,
                EntryDate = DateTime.UtcNow,
                EntryType = JournalEntryType.BankReceipt,
                Description = description,
                FinancialPeriodId = periodId,
                ReferenceId = null,
                ReferenceType = referenceNo,
                IsPosted = false
            };
            await _uow.Repository<JournalEntry>().AddAsync(entry, ct);
            await _uow.SaveChangesAsync(ct);

            var debitLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = accountId,
                LineType = JournalLineType.Debit,
                Amount = amount,
                Narration = description
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

            var incomeAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "3-101" && !a.IsDeleted, ct);

            var creditLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = incomeAccount?.Id ?? accountId,
                LineType = JournalLineType.Credit,
                Amount = amount,
                Narration = description
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(creditLine, ct);
            await _uow.SaveChangesAsync(ct);

            await PostJournalEntryInternal(entry.Id, createdBy, ct);
            entryId = entry.Id;
        }, ct);

        return entryId;
    }

    public async Task<int> PostBankPaymentAsync(int accountId, decimal amount, string referenceNo, string description, string createdBy, CancellationToken ct)
    {
        var entryId = 0;
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var periodId = await GetOrCreateActivePeriodAsync(ct);
            var journalNo = $"BP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var entry = new JournalEntry
            {
                CreatedBy = createdBy,
                JournalNo = journalNo,
                EntryDate = DateTime.UtcNow,
                EntryType = JournalEntryType.BankPayment,
                Description = description,
                FinancialPeriodId = periodId,
                ReferenceType = referenceNo,
                IsPosted = false
            };
            await _uow.Repository<JournalEntry>().AddAsync(entry, ct);
            await _uow.SaveChangesAsync(ct);

            var expenseAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "4-101" && !a.IsDeleted, ct);

            var debitLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = expenseAccount?.Id ?? accountId,
                LineType = JournalLineType.Debit,
                Amount = amount,
                Narration = description
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

            var creditLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = accountId,
                LineType = JournalLineType.Credit,
                Amount = amount,
                Narration = description
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(creditLine, ct);
            await _uow.SaveChangesAsync(ct);

            await PostJournalEntryInternal(entry.Id, createdBy, ct);
            entryId = entry.Id;
        }, ct);

        return entryId;
    }

    public async Task<int> PostAdmissionFeeAsync(int admissionId, decimal amount, string paymentMethod, string gatewayTransactionId, string createdBy, CancellationToken ct)
    {
        var entryId = 0;
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var periodId = await GetOrCreateActivePeriodAsync(ct);
            var journalNo = $"AF-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var entry = new JournalEntry
            {
                CreatedBy = createdBy,
                JournalNo = journalNo,
                EntryDate = DateTime.UtcNow,
                EntryType = JournalEntryType.FeeCollection,
                Description = $"Admission fee - App #{admissionId}",
                FinancialPeriodId = periodId,
                ReferenceId = admissionId,
                ReferenceType = "AdmissionApplication",
                IsPosted = false
            };
            await _uow.Repository<JournalEntry>().AddAsync(entry, ct);
            await _uow.SaveChangesAsync(ct);

            // Debit: SSLCommerz Clearing (1-003) or Cash (1-001) based on payment method
            var isOnline = paymentMethod.Contains("Online", StringComparison.OrdinalIgnoreCase)
                || paymentMethod.Contains("SSL", StringComparison.OrdinalIgnoreCase);
            var debitAccountCode = isOnline ? "1-003" : "1-001";
            var debitAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == debitAccountCode && !a.IsDeleted, ct);
            var debitAccountId = debitAccount?.Id
                ?? throw new InvalidOperationException($"Account {debitAccountCode} not found. Set up the account first.");

            var debitLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = debitAccountId,
                LineType = JournalLineType.Debit,
                Amount = amount,
                Narration = $"Admission fee payment - App #{admissionId}, Ref: {gatewayTransactionId}"
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

            // Credit: Admission Fee Income (3-201)
            var incomeAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "3-201" && !a.IsDeleted, ct);
            var incomeAccountId = incomeAccount?.Id
                ?? throw new InvalidOperationException("Admission Fee Income account (3-201) not found. Set up the account first.");

            var creditLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = incomeAccountId,
                LineType = JournalLineType.Credit,
                Amount = amount,
                Narration = $"Admission fee income - App #{admissionId}, Ref: {gatewayTransactionId}"
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(creditLine, ct);
            await _uow.SaveChangesAsync(ct);

            await PostJournalEntryInternal(entry.Id, createdBy, ct);

            // Create AdmissionReceipt
            var app = await _uow.Repository<AdmissionApplication>()
                .FirstOrDefaultAsync(a => a.Id == admissionId && !a.IsDeleted, ct);

            var receiptNo = $"ADM-RCP-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";
            var receipt = new AdmissionReceipt
            {
                ReceiptNo = receiptNo,
                AdmissionApplicationId = admissionId,
                Amount = amount,
                PaymentMethod = paymentMethod,
                GatewayTransactionId = gatewayTransactionId,
                ApplicantName = app?.ApplicantName,
                ReceiptDate = DateTime.UtcNow,
                Remarks = $"Admission fee payment via {paymentMethod}. App #{admissionId}",
                CreatedBy = createdBy,
                CreatedAt = DateTime.UtcNow
            };
            await _uow.Repository<AdmissionReceipt>().AddAsync(receipt, ct);
            await _uow.SaveChangesAsync(ct);
            entryId = entry.Id;
        }, ct);

        return entryId;
    }

    public async Task<int> PostAdmissionRefundAsync(int admissionId, decimal amount, string reason, string createdBy, CancellationToken ct)
    {
        var entryId = 0;
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var periodId = await GetOrCreateActivePeriodAsync(ct);
            var journalNo = $"AR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var entry = new JournalEntry
            {
                CreatedBy = createdBy,
                JournalNo = journalNo,
                EntryDate = DateTime.UtcNow,
                EntryType = JournalEntryType.Adjustment,
                Description = $"Admission fee refund - App #{admissionId}",
                FinancialPeriodId = periodId,
                ReferenceId = admissionId,
                ReferenceType = "AdmissionRefund",
                IsPosted = false
            };
            await _uow.Repository<JournalEntry>().AddAsync(entry, ct);
            await _uow.SaveChangesAsync(ct);

            // Debit: Admission Fee Income (reverse income)
            var incomeAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "3-201" && !a.IsDeleted, ct);
            var incomeAccountId = incomeAccount?.Id
                ?? throw new InvalidOperationException("Admission Fee Income account (3-201) not found.");

            var debitLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = incomeAccountId,
                LineType = JournalLineType.Debit,
                Amount = amount,
                Narration = $"Admission fee refund - App #{admissionId}, Reason: {reason}"
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

            // Credit: Cash / SSLCommerz Clearing
            var cashAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "1-001" && !a.IsDeleted, ct);
            var cashAccountId = cashAccount?.Id
                ?? throw new InvalidOperationException("Cash account (1-001) not found.");

            var creditLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = cashAccountId,
                LineType = JournalLineType.Credit,
                Amount = amount,
                Narration = $"Admission fee refund payout - App #{admissionId}"
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(creditLine, ct);
            await _uow.SaveChangesAsync(ct);

            await PostJournalEntryInternal(entry.Id, createdBy, ct);

            // Mark receipt as refunded
            var receipt = await _uow.Repository<AdmissionReceipt>()
                .FirstOrDefaultAsync(r => r.AdmissionApplicationId == admissionId && !r.IsRefunded && !r.IsDeleted, ct);
            if (receipt != null)
            {
                receipt.IsRefunded = true;
                receipt.RefundAmount = amount;
                receipt.RefundedAt = DateTime.UtcNow;
                receipt.RefundedBy = createdBy;
                receipt.RefundReason = reason;
                receipt.UpdatedAt = DateTime.UtcNow;
                _uow.Repository<AdmissionReceipt>().Update(receipt);
                await _uow.SaveChangesAsync(ct);
            }
            entryId = entry.Id;
        }, ct);

        return entryId;
    }

    private async Task PostJournalEntryInternal(int entryId, string postedBy, CancellationToken ct)
    {
        await _journalRepo.PostJournalEntryAsync(entryId, postedBy, ct);
    }
}
