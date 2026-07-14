using Xunit;
using Moq;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Accounting;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Implementations.Accounting;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Repositories.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Accounting;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Linq.Expressions;

namespace SchoolManagementSystem.Tests.Services;

public class Phase103_AdmissionAccountingTests
{
    private readonly Mock<IUnitOfWork> _uow = new(MockBehavior.Loose);
    private readonly Mock<IFinancialPeriodRepository> _periodRepo = new(MockBehavior.Loose);
    private readonly Mock<IJournalEntryRepository> _journalRepo = new(MockBehavior.Loose);
    private readonly FinancePostingService _svc;

    private ChartOfAccount _cashAccount = null!;
    private ChartOfAccount _sslCommerzAccount = null!;
    private ChartOfAccount _incomeAccount = null!;
    private FinancialPeriod _openPeriod = null!;
    private List<JournalEntry> _journalEntries = [];
    private List<JournalEntryLine> _journalLines = [];
    private List<GeneralLedgerEntry> _ledgerEntries = [];
    private List<AdmissionReceipt> _receipts = [];
    private int _nextJournalId = 1;

    public Phase103_AdmissionAccountingTests()
    {
        _cashAccount = new ChartOfAccount
        {
            Id = 1, AccountCode = "1-001", AccountName = "Cash",
            AccountType = AccountType.Asset, CreatedBy = "seed"
        };
        _sslCommerzAccount = new ChartOfAccount
        {
            Id = 2, AccountCode = "1-003", AccountName = "SSLCommerz Clearing",
            AccountType = AccountType.Asset, CreatedBy = "seed"
        };
        _incomeAccount = new ChartOfAccount
        {
            Id = 3, AccountCode = "3-201", AccountName = "Admission Fee Income",
            AccountType = AccountType.Income, CreatedBy = "seed"
        };
        _openPeriod = new FinancialPeriod
        {
            Id = 1, Name = "FY 2026", StartDate = new DateTime(2026, 1, 1),
            EndDate = new DateTime(2026, 12, 31), Status = FinancialPeriodStatus.Open,
            IsActive = true, CreatedBy = "seed"
        };

        _periodRepo.Setup(r => r.GetActivePeriodAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(_openPeriod);

        SetupUowRepository(_uow, new Mock<IBaseRepository<ChartOfAccount>>(), out Mock<IBaseRepository<ChartOfAccount>> coaBase)
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<ChartOfAccount, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<ChartOfAccount, bool>> expr, CancellationToken ct) =>
            {
                var compiled = expr.Compile();
                if (compiled(_sslCommerzAccount)) return _sslCommerzAccount;
                if (compiled(_cashAccount)) return _cashAccount;
                if (compiled(_incomeAccount)) return _incomeAccount;
                return null;
            });

        SetupUowRepository(_uow, new Mock<IBaseRepository<JournalEntry>>(), out Mock<IBaseRepository<JournalEntry>> journalBase)
            .Setup(r => r.AddAsync(It.IsAny<JournalEntry>(), It.IsAny<CancellationToken>()))
            .Callback<JournalEntry, CancellationToken>((e, _) =>
            {
                e.Id = _nextJournalId++;
                _journalEntries.Add(e);
            })
            .Returns(Task.CompletedTask);

        SetupUowRepository(_uow, new Mock<IBaseRepository<JournalEntryLine>>(), out Mock<IBaseRepository<JournalEntryLine>> lineBase)
            .Setup(r => r.AddAsync(It.IsAny<JournalEntryLine>(), It.IsAny<CancellationToken>()))
            .Callback<JournalEntryLine, CancellationToken>((e, _) => _journalLines.Add(e))
            .Returns(Task.CompletedTask);

        SetupUowRepository(_uow, new Mock<IBaseRepository<GeneralLedgerEntry>>(), out Mock<IBaseRepository<GeneralLedgerEntry>> glBase)
            .Setup(r => r.AddAsync(It.IsAny<GeneralLedgerEntry>(), It.IsAny<CancellationToken>()))
            .Callback<GeneralLedgerEntry, CancellationToken>((e, _) => _ledgerEntries.Add(e))
            .Returns(Task.CompletedTask);

        SetupUowRepository(_uow, new Mock<IBaseRepository<AdmissionReceipt>>(), out Mock<IBaseRepository<AdmissionReceipt>> recBase)
            .Setup(r => r.AddAsync(It.IsAny<AdmissionReceipt>(), It.IsAny<CancellationToken>()))
            .Callback<AdmissionReceipt, CancellationToken>((e, _) => _receipts.Add(e))
            .Returns(Task.CompletedTask);

        SetupUowRepository(_uow, new Mock<IBaseRepository<AdmissionApplication>>(), out Mock<IBaseRepository<AdmissionApplication>> appBase)
            .Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<AdmissionApplication, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new AdmissionApplication { Id = 42, ApplicantName = "Test Student" });

        _journalRepo.Setup(r => r.AddAsync(It.IsAny<JournalEntry>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        _journalRepo.Setup(r => r.PostJournalEntryAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);

        // SaveChangesAsync called multiple times — must always succeed
        _uow.Setup(u => u.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _svc = new FinancePostingService(_uow.Object, _periodRepo.Object, _journalRepo.Object);
    }

    private static Mock<IBaseRepository<T>> SetupUowRepository<T>(
        Mock<IUnitOfWork> uow, Mock<IBaseRepository<T>> mock,
        out Mock<IBaseRepository<T>> outMock) where T : class
    {
        outMock = mock;
        uow.Setup(u => u.Repository<T>()).Returns(mock.Object);
        return mock;
    }

    [Fact(DisplayName = "1. PostAdmissionFeeAsync creates JournalEntry with correct type")]
    public async Task PostAdmissionFeeAsync_CreatesJournalEntry()
    {
        await _svc.PostAdmissionFeeAsync(admissionId: 42, amount: 5000m,
            paymentMethod: "SSLCommerz", gatewayTransactionId: "TXN123",
            createdBy: "test-user", CancellationToken.None);

        var entry = _journalEntries.SingleOrDefault();
        Assert.NotNull(entry);
        Assert.Equal(JournalEntryType.FeeCollection, entry!.EntryType);
        Assert.Contains("Admission fee", entry.Description);
        Assert.Contains("42", entry.Description);
    }

    [Fact(DisplayName = "2. PostAdmissionFeeAsync creates debit and credit journal lines")]
    public async Task PostAdmissionFeeAsync_CreatesBalancedLines()
    {
        await _svc.PostAdmissionFeeAsync(admissionId: 42, amount: 5000m,
            paymentMethod: "Cash", gatewayTransactionId: "CASH001",
            createdBy: "test-user", CancellationToken.None);

        var debits = _journalLines.Where(l => l.LineType == JournalLineType.Debit).ToList();
        var credits = _journalLines.Where(l => l.LineType == JournalLineType.Credit).ToList();

        Assert.Single(debits);
        Assert.Single(credits);
        Assert.Equal(5000m, debits[0].Amount);
        Assert.Equal(5000m, credits[0].Amount);
    }

    [Fact(DisplayName = "3. PostAdmissionFeeAsync uses SSLCommerz clearing account for online payment")]
    public async Task PostAdmissionFeeAsync_UsesSslCommerzClearing_WhenOnline()
    {
        await _svc.PostAdmissionFeeAsync(admissionId: 42, amount: 5000m,
            paymentMethod: "SSLCommerz", gatewayTransactionId: "SSL001",
            createdBy: "test-user", CancellationToken.None);

        var debitLine = _journalLines.Single(l => l.LineType == JournalLineType.Debit);
        Assert.Equal(_sslCommerzAccount.Id, debitLine.AccountId);
    }

    [Fact(DisplayName = "4. PostAdmissionFeeAsync uses Cash account for manual payment")]
    public async Task PostAdmissionFeeAsync_UsesCashAccount_WhenManual()
    {
        await _svc.PostAdmissionFeeAsync(admissionId: 42, amount: 5000m,
            paymentMethod: "Cash", gatewayTransactionId: "CASH001",
            createdBy: "test-user", CancellationToken.None);

        var debitLine = _journalLines.Single(l => l.LineType == JournalLineType.Debit);
        Assert.Equal(_cashAccount.Id, debitLine.AccountId);
    }

    [Fact(DisplayName = "5. PostAdmissionFeeAsync creates AdmissionReceipt")]
    public async Task PostAdmissionFeeAsync_CreatesReceipt()
    {
        await _svc.PostAdmissionFeeAsync(admissionId: 42, amount: 5000m,
            paymentMethod: "bKash", gatewayTransactionId: "BKASH001",
            createdBy: "admin", CancellationToken.None);

        var receipt = _receipts.SingleOrDefault();
        Assert.NotNull(receipt);
        Assert.Equal(42, receipt!.AdmissionApplicationId);
        Assert.Equal(5000m, receipt.Amount);
        Assert.Equal("bKash", receipt.PaymentMethod);
    }

    [Fact(DisplayName = "6. PostAdmissionFeeAsync calls PostJournalEntryAsync (SP) to create GL entries")]
    public async Task PostAdmissionFeeAsync_CallsPostJournalEntry()
    {
        await _svc.PostAdmissionFeeAsync(admissionId: 42, amount: 5000m,
            paymentMethod: "SSLCommerz", gatewayTransactionId: "TXN123",
            createdBy: "test-user", CancellationToken.None);

        _journalRepo.Verify(r => r.PostJournalEntryAsync(It.IsAny<int>(), It.IsAny<string>(), It.IsAny<CancellationToken>()),
            Times.Once);
    }

    [Fact(DisplayName = "7. PostAdmissionRefundAsync creates reversal journal entry")]
    public async Task PostAdmissionRefundAsync_CreatesReverseEntry()
    {
        await _svc.PostAdmissionRefundAsync(admissionId: 42, amount: 5000m,
            reason: "Cancelled application", createdBy: "admin",
            CancellationToken.None);

        var entry = _journalEntries.SingleOrDefault();
        Assert.NotNull(entry);
        Assert.Equal(JournalEntryType.Adjustment, entry!.EntryType);
        Assert.Contains("refund", entry.Description, StringComparison.OrdinalIgnoreCase);
    }

    [Fact(DisplayName = "8. PostAdmissionFeeAsync auto-creates period when no active period exists")]
    public async Task PostAdmissionFeeAsync_AutoCreatesPeriod()
    {
        _periodRepo.Setup(r => r.GetActivePeriodAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync((FinancialPeriod?)null);

        FinancialPeriod? createdPeriod = null;
        Mock<IBaseRepository<FinancialPeriod>> periodBase = new(MockBehavior.Loose);
        periodBase.Setup(r => r.AddAsync(It.IsAny<FinancialPeriod>(), It.IsAny<CancellationToken>()))
            .Callback<FinancialPeriod, CancellationToken>((p, _) => createdPeriod = p)
            .Returns(Task.CompletedTask);
        _uow.Setup(r => r.Repository<FinancialPeriod>()).Returns(periodBase.Object);

        await _svc.PostAdmissionFeeAsync(admissionId: 42, amount: 5000m,
            paymentMethod: "Cash", gatewayTransactionId: "CASH001",
            createdBy: "test", CancellationToken.None);

        Assert.NotNull(createdPeriod);
        Assert.Contains("FY", createdPeriod!.Name);
    }

    [Fact(DisplayName = "9. PostAdmissionFeeAsync with 0 amount still creates receipt")]
    public async Task PostAdmissionFeeAsync_ZeroAmount()
    {
        await _svc.PostAdmissionFeeAsync(admissionId: 42, amount: 0m,
            paymentMethod: "Online", gatewayTransactionId: "FREE001",
            createdBy: "system", CancellationToken.None);

        Assert.NotEmpty(_receipts);
        Assert.NotEmpty(_journalEntries);
    }
}
