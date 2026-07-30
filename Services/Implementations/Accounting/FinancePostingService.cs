using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Guardian;
using GuardianEntity = SchoolManagementSystem.Models.Entities.Guardian.Guardian;
using StudentEntity = SchoolManagementSystem.Models.Entities.Student.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Accounting;

public class FinancePostingService : IFinancePostingService
{
    private readonly IUnitOfWork _uow;
    private readonly IFinancialPeriodRepository _periodRepo;
    private readonly IJournalEntryRepository _journalRepo;
    private readonly IEmailService _emailService;

    public FinancePostingService(IUnitOfWork uow, IFinancialPeriodRepository periodRepo, IJournalEntryRepository journalRepo, IEmailService emailService)
    {
        _uow = uow;
        _periodRepo = periodRepo;
        _journalRepo = journalRepo;
        _emailService = emailService;
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

    public async Task<int> PostFeeCollectionFullAsync(int studentId, List<int> invoiceIds, CashierPaymentDto payment, string createdBy, CancellationToken ct)
    {
        var paymentId = 0;
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var periodId = await GetOrCreateActivePeriodAsync(ct);
            var isOnline = payment.Method == (int)PaymentMethod.Online;
            var debitAccountCode = isOnline ? "1-003" : "1-001";
            var debitAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == debitAccountCode && !a.IsDeleted, ct);
            var debitAccountId = debitAccount?.Id
                ?? throw new InvalidOperationException($"Account {debitAccountCode} not found. Set up the account first.");

            foreach (var invoiceId in invoiceIds)
            {
                var invoice = await _uow.Repository<FeeInvoice>()
                    .FirstOrDefaultAsync(x => x.Id == invoiceId && !x.IsDeleted, ct)
                    ?? throw new InvalidOperationException($"Invoice #{invoiceId} not found.");

                var remainingBalance = invoice.TotalAmount - invoice.PaidAmount;
                if (remainingBalance <= 0) continue;

                var allocAmount = Math.Min(payment.Amount, remainingBalance);

                var pay = new Payment
                {
                    CreatedBy = createdBy,
                    FeeInvoiceId = invoiceId,
                    Amount = allocAmount,
                    LateFee = payment.LateFee,
                    DiscountAmount = payment.DiscountAmount,
                    Method = (PaymentMethod)payment.Method,
                    ReferenceNo = payment.ReferenceNo,
                    PaidAt = DateTime.UtcNow,
                    Remarks = payment.Remarks,
                    PostingStatus = PostingStatus.Pending,
                    PostingError = null
                };
                await _uow.Repository<Payment>().AddAsync(pay, ct);

                invoice.PaidAmount += allocAmount;
                invoice.UpdatedAt = DateTime.UtcNow;
                var dueAmount = invoice.TotalAmount - invoice.PaidAmount;
                invoice.Status = dueAmount <= 0 ? PaymentStatus.Paid : PaymentStatus.Partial;
                _uow.Repository<FeeInvoice>().Update(invoice);

                var ledger = new FeeLedger
                {
                    StudentId = studentId,
                    FeeInvoiceId = invoiceId,
                    FeePaymentId = pay.Id,
                    TransactionType = FeeLedgerType.Payment,
                    Debit = 0,
                    Credit = allocAmount,
                    Balance = -allocAmount,
                    Description = $"Payment: {payment.ReferenceNo ?? "N/A"}",
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                };
                await _uow.Repository<FeeLedger>().AddAsync(ledger);

                payment.Amount -= allocAmount;
                if (paymentId == 0) paymentId = pay.Id;

                // Accounting posting
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
                    IsPosted = false,
                    PostingStatus = PostingStatus.Pending
                };
                await _uow.Repository<JournalEntry>().AddAsync(entry, ct);

                var debitLine = new JournalEntryLine
                {
                    CreatedBy = createdBy,
                    JournalEntryId = entry.Id,
                    AccountId = debitAccountId,
                    LineType = JournalLineType.Debit,
                    Amount = allocAmount,
                    Narration = $"Fee collection - Invoice #{invoiceId}"
                };
                await _uow.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

                var receivableAccount = await _uow.Repository<ChartOfAccount>()
                    .FirstOrDefaultAsync(a => a.AccountCode == "1-101" && !a.IsDeleted, ct);
                var receivableId = receivableAccount?.Id ?? debitAccountId;

                var creditLine = new JournalEntryLine
                {
                    CreatedBy = createdBy,
                    JournalEntryId = entry.Id,
                    AccountId = receivableId,
                    LineType = JournalLineType.Credit,
                    Amount = allocAmount,
                    Narration = $"Fee collection - Invoice #{invoiceId}"
                };
                await _uow.Repository<JournalEntryLine>().AddAsync(creditLine, ct);

                await _uow.SaveChangesAsync(ct);

                await PostJournalEntryInternal(entry.Id, createdBy, ct);

                pay.PostingStatus = PostingStatus.Posted;
                pay.PostedAt = DateTime.UtcNow;
                _uow.Repository<Payment>().Update(pay);
                await _uow.SaveChangesAsync(ct);
            }
        }, ct);

        await SendPaymentReceiptAsync(studentId, paymentId, payment, ct);

        return paymentId;
    }

    private async Task SendPaymentReceiptAsync(int studentId, int paymentId, CashierPaymentDto payment, CancellationToken ct)
    {
        try
        {
            var schoolName = "School Management System";
            var methodName = ((PaymentMethod)payment.Method).ToString();

            // Priority 1: Primary guardian with email notifications opted in
            var primarySg = await _uow.Repository<StudentGuardian>()
                .FirstOrDefaultAsync(x => x.StudentId == studentId && x.IsPrimaryGuardian && !x.IsDeleted, ct);
            if (primarySg is not null)
            {
                var guardian = await _uow.Repository<GuardianEntity>()
                    .FirstOrDefaultAsync(x => x.Id == primarySg.GuardianId && !x.IsDeleted, ct);
                if (guardian is not null && guardian.ReceiveEmailNotifications && !string.IsNullOrWhiteSpace(guardian.Email))
                {
                    await _emailService.SendPaymentReceiptAsync(
                        guardian.Email, guardian.FullName,
                        $"PAY-{paymentId:D6}", payment.Amount, DateTime.UtcNow,
                        methodName, schoolName, ct);
                    return;
                }
            }

            // Priority 2: Any guardian with email notifications opted in
            var allSgs = await _uow.Repository<StudentGuardian>()
                .ListAsync(x => x.StudentId == studentId && !x.IsDeleted, ct);
            var guardianIds = allSgs.Select(x => x.GuardianId).ToList();
            if (guardianIds.Count > 0)
            {
                foreach (var gid in guardianIds)
                {
                    var g = await _uow.Repository<GuardianEntity>()
                        .FirstOrDefaultAsync(x => x.Id == gid && !x.IsDeleted
                            && x.ReceiveEmailNotifications && x.Email != null && x.Email != string.Empty, ct);
                    if (g is not null)
                    {
                        await _emailService.SendPaymentReceiptAsync(
                            g.Email!, g.FullName,
                            $"PAY-{paymentId:D6}", payment.Amount, DateTime.UtcNow,
                            methodName, schoolName, ct);
                        return;
                    }
                }
            }

            // Priority 3: Fallback to student email
            var student = await _uow.Repository<StudentEntity>()
                .FirstOrDefaultAsync(x => x.Id == studentId && !x.IsDeleted, ct);
            if (student is not null && !string.IsNullOrWhiteSpace(student.EmailAddress))
            {
                await _emailService.SendPaymentReceiptAsync(
                    student.EmailAddress, student.FullName,
                    $"PAY-{paymentId:D6}", payment.Amount, DateTime.UtcNow,
                    methodName, schoolName, ct);
            }
        }
        catch
        {
        }
    }

    public async Task<int> PostFeeCollectionAsync(int studentId, decimal amount, int invoiceId, string createdBy, CancellationToken ct)
    {
        var cashAccount = await _uow.Repository<ChartOfAccount>()
            .FirstOrDefaultAsync(a => a.AccountCode == "1-001" && !a.IsDeleted, ct);
        var accountId = cashAccount?.Id
            ?? throw new InvalidOperationException("Default cash account (1-001) not found. Set up a Cash in Hand account first.");
        return await PostFeeCollectionAsync(studentId, amount, invoiceId, accountId, createdBy, ct);
    }

    public async Task<int> PostFeeCollectionAsync(int studentId, decimal amount, int invoiceId, string paymentMethod, string createdBy, CancellationToken ct)
    {
        var isOnline = paymentMethod.Contains("Online", StringComparison.OrdinalIgnoreCase)
            || paymentMethod.Contains("SSL", StringComparison.OrdinalIgnoreCase);
        var debitAccountCode = isOnline ? "1-003" : "1-001";
        var debitAccount = await _uow.Repository<ChartOfAccount>()
            .FirstOrDefaultAsync(a => a.AccountCode == debitAccountCode && !a.IsDeleted, ct);
        var accountId = debitAccount?.Id
            ?? throw new InvalidOperationException($"Account {debitAccountCode} not found. Set up the account first.");
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
                IsPosted = false,
                PostingStatus = PostingStatus.Pending
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
                IsPosted = false,
                PostingStatus = PostingStatus.Pending
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
                IsPosted = false,
                PostingStatus = PostingStatus.Pending
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
                IsPosted = false,
                PostingStatus = PostingStatus.Pending
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
                IsPosted = false,
                PostingStatus = PostingStatus.Pending
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
                IsPosted = false,
                PostingStatus = PostingStatus.Pending
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

    public async Task<int> PostFeeDiscountAsync(int studentId, decimal amount, int invoiceId, string description, string createdBy, CancellationToken ct)
    {
        var entryId = 0;
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var periodId = await GetOrCreateActivePeriodAsync(ct);
            var journalNo = $"FD-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var entry = new JournalEntry
            {
                CreatedBy = createdBy,
                JournalNo = journalNo,
                EntryDate = DateTime.UtcNow,
                EntryType = JournalEntryType.Adjustment,
                Description = description,
                FinancialPeriodId = periodId,
                ReferenceId = invoiceId,
                ReferenceType = "FeeDiscount",
                IsPosted = false,
                PostingStatus = PostingStatus.Pending
            };
            await _uow.Repository<JournalEntry>().AddAsync(entry, ct);
            await _uow.SaveChangesAsync(ct);

            var discountAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "4-201" && !a.IsDeleted, ct);
            var receivableAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "1-101" && !a.IsDeleted, ct);

            var debitLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = discountAccount?.Id ?? 1,
                LineType = JournalLineType.Debit,
                Amount = amount,
                Narration = description
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

            var creditLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = receivableAccount?.Id ?? 1,
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

    public async Task<int> PostFeeRefundAsync(int refundId, string createdBy, CancellationToken ct)
    {
        var entryId = 0;
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var refund = await _uow.Repository<FeeRefund>()
                .FirstOrDefaultAsync(r => r.Id == refundId && !r.IsDeleted, ct)
                ?? throw new InvalidOperationException($"Fee refund #{refundId} not found.");

            var payment = await _uow.Repository<Payment>()
                .FirstOrDefaultAsync(p => p.Id == refund.FeePaymentId && !p.IsDeleted, ct)
                ?? throw new InvalidOperationException($"Payment #{refund.FeePaymentId} not found.");

            var invoice = await _uow.Repository<FeeInvoice>()
                .FirstOrDefaultAsync(i => i.Id == payment.FeeInvoiceId && !i.IsDeleted, ct);

            var periodId = await GetOrCreateActivePeriodAsync(ct);
            var journalNo = $"FR-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var entry = new JournalEntry
            {
                CreatedBy = createdBy,
                JournalNo = journalNo,
                EntryDate = DateTime.UtcNow,
                EntryType = JournalEntryType.Adjustment,
                Description = $"Fee refund #{refundId} — {refund.Reason}",
                FinancialPeriodId = periodId,
                ReferenceId = refundId,
                ReferenceType = "FeeRefund",
                IsPosted = false,
                PostingStatus = PostingStatus.Pending
            };
            await _uow.Repository<JournalEntry>().AddAsync(entry, ct);
            await _uow.SaveChangesAsync(ct);

            var incomeAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "3-101" && !a.IsDeleted, ct);
            var cashAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "1-001" && !a.IsDeleted, ct);

            var debitLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = incomeAccount?.Id ?? 1,
                LineType = JournalLineType.Debit,
                Amount = refund.RefundAmount,
                Narration = $"Fee refund #{refundId} — {refund.Reason}"
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

            var creditLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = cashAccount?.Id ?? 1,
                LineType = JournalLineType.Credit,
                Amount = refund.RefundAmount,
                Narration = $"Fee refund payout #{refundId}"
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(creditLine, ct);
            await _uow.SaveChangesAsync(ct);

            await PostJournalEntryInternal(entry.Id, createdBy, ct);
            entryId = entry.Id;
        }, ct);

        return entryId;
    }

    public async Task<int> PostLateFeeAsync(int studentId, decimal amount, int invoiceId, string description, string createdBy, CancellationToken ct)
    {
        var entryId = 0;
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var periodId = await GetOrCreateActivePeriodAsync(ct);
            var journalNo = $"LF-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var entry = new JournalEntry
            {
                CreatedBy = createdBy,
                JournalNo = journalNo,
                EntryDate = DateTime.UtcNow,
                EntryType = JournalEntryType.Adjustment,
                Description = description,
                FinancialPeriodId = periodId,
                ReferenceId = invoiceId,
                ReferenceType = "LateFee",
                IsPosted = false,
                PostingStatus = PostingStatus.Pending
            };
            await _uow.Repository<JournalEntry>().AddAsync(entry, ct);
            await _uow.SaveChangesAsync(ct);

            var receivableAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "1-101" && !a.IsDeleted, ct);
            var lateFeeIncomeAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "3-601" && !a.IsDeleted, ct);

            var debitLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = receivableAccount?.Id ?? 1,
                LineType = JournalLineType.Debit,
                Amount = amount,
                Narration = description
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

            var creditLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = lateFeeIncomeAccount?.Id ?? 1,
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

    public async Task<int> PostFineAsync(int studentId, decimal amount, int invoiceId, string description, string createdBy, CancellationToken ct)
    {
        var entryId = 0;
        await _uow.ExecuteInTransactionAsync(async () =>
        {
            var periodId = await GetOrCreateActivePeriodAsync(ct);
            var journalNo = $"FN-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString()[..6].ToUpper()}";

            var entry = new JournalEntry
            {
                CreatedBy = createdBy,
                JournalNo = journalNo,
                EntryDate = DateTime.UtcNow,
                EntryType = JournalEntryType.Adjustment,
                Description = description,
                FinancialPeriodId = periodId,
                ReferenceId = invoiceId,
                ReferenceType = "Fine",
                IsPosted = false,
                PostingStatus = PostingStatus.Pending
            };
            await _uow.Repository<JournalEntry>().AddAsync(entry, ct);
            await _uow.SaveChangesAsync(ct);

            var receivableAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "1-101" && !a.IsDeleted, ct);
            var fineIncomeAccount = await _uow.Repository<ChartOfAccount>()
                .FirstOrDefaultAsync(a => a.AccountCode == "3-602" && !a.IsDeleted, ct);

            var debitLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = receivableAccount?.Id ?? 1,
                LineType = JournalLineType.Debit,
                Amount = amount,
                Narration = description
            };
            await _uow.Repository<JournalEntryLine>().AddAsync(debitLine, ct);

            var creditLine = new JournalEntryLine
            {
                CreatedBy = createdBy,
                JournalEntryId = entry.Id,
                AccountId = fineIncomeAccount?.Id ?? 1,
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

    private async Task PostJournalEntryInternal(int entryId, string postedBy, CancellationToken ct)
    {
        await _journalRepo.PostJournalEntryAsync(entryId, postedBy, ct);
        var entry = await _uow.Repository<JournalEntry>().FirstOrDefaultAsync(e => e.Id == entryId, ct);
        if (entry != null)
        {
            entry.PostingStatus = PostingStatus.Posted;
            entry.PostedAt = DateTime.UtcNow;
            entry.PostedBy = postedBy;
            entry.PostingError = null;
            _uow.Repository<JournalEntry>().Update(entry);
            await _uow.SaveChangesAsync(ct);
        }
    }
}
