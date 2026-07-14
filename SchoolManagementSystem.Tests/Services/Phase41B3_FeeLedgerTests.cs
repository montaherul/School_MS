using Xunit;
using Moq;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Services.Implementations.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SchoolManagementSystem.Tests.Services;

public class Phase41B3_FeeLedgerTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<IFeePaymentRepository> _paymentRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IFeeInvoiceRepository> _invoiceRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<FeeInvoice>> _invoiceBaseRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<Payment>> _paymentBaseRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<FeeLedger>> _ledgerBaseRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<FeeWaiver>> _waiverBaseRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IFeeWaiverRepository> _waiverRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<FeeDiscount>> _discountBaseRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IFeeDiscountRepository> _discountRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<FeeRefund>> _refundBaseRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IFeeRefundRepository> _refundRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<Payment>> _paymentForRefundRepoMock = new(MockBehavior.Loose);

    private readonly List<FeeLedger> _ledgerEntries = new();
    private readonly List<Payment> _payments = new();
    private readonly List<FeeWaiver> _waivers = new();
    private readonly List<FeeDiscount> _discounts = new();
    private readonly List<FeeRefund> _refunds = new();
    private readonly IAuditLogService _auditServiceMock;

    public Phase41B3_FeeLedgerTests()
    {
        var auditMock = new Mock<IAuditLogService>();
        auditMock.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _auditServiceMock = auditMock.Object;

        _uowMock.Setup(u => u.Repository<FeeInvoice>()).Returns(_invoiceBaseRepoMock.Object);
        _uowMock.Setup(u => u.Repository<Payment>()).Returns(_paymentBaseRepoMock.Object);
        _uowMock.Setup(u => u.Repository<FeeLedger>()).Returns(_ledgerBaseRepoMock.Object);
        _uowMock.Setup(u => u.Repository<FeeWaiver>()).Returns(_waiverBaseRepoMock.Object);
        _uowMock.Setup(u => u.Repository<FeeDiscount>()).Returns(_discountBaseRepoMock.Object);
        _uowMock.Setup(u => u.Repository<FeeRefund>()).Returns(_refundBaseRepoMock.Object);

        _uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((op, ct) => op());

        _ledgerBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<FeeLedger>(), It.IsAny<CancellationToken>()))
            .Callback<FeeLedger, CancellationToken>((e, ct) =>
            {
                e.Id = _ledgerEntries.Count + 1;
                _ledgerEntries.Add(e);
            })
            .Returns(Task.CompletedTask);

        _ledgerBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<FeeLedger, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<FeeLedger, bool>> predicate, CancellationToken ct) =>
                _ledgerEntries.Where(predicate.Compile()).ToList());

        _ledgerBaseRepoMock.Setup(r => r.Update(It.IsAny<FeeLedger>()))
            .Callback<FeeLedger>(e =>
            {
                var idx = _ledgerEntries.FindIndex(x => x.Id == e.Id);
                if (idx >= 0) _ledgerEntries[idx] = e;
            });

        _paymentBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => _payments.ToList());

        _paymentBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Callback<Payment, CancellationToken>((p, ct) =>
            {
                p.Id = _payments.Count + 1;
                _payments.Add(p);
            })
            .Returns(Task.CompletedTask);

        _paymentBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Payment, bool>> expr, CancellationToken ct) =>
                _payments.AsQueryable().FirstOrDefault(expr));
    }

    // ─── 1. Invoice creates ledger ────────────────────────────────

    [Fact(DisplayName = "1. Invoice creation creates FeeLedger entry")]
    public async Task InvoiceCreation_CreatesLedger()
    {
        var svc = new FeeInvoiceService(_uowMock.Object, _invoiceRepoMock.Object, _auditServiceMock);
        var invoice = new FeeInvoice { Id = 1, StudentId = 10, InvoiceNo = "INV-001", TotalAmount = 5000 };

        await svc.CreateAsync(invoice, "test-user");

        Assert.Single(_ledgerEntries);
        var entry = _ledgerEntries[0];
        Assert.Equal(10, entry.StudentId);
        Assert.Equal(FeeLedgerType.Invoice, entry.TransactionType);
        Assert.Equal(5000, entry.Debit);
        Assert.Equal(0, entry.Credit);
        Assert.Equal(5000, entry.Balance);
        Assert.Contains("INV-001", entry.Description);
    }

    // ─── 2. Payment creates ledger ─────────────────────────────────

    [Fact(DisplayName = "2. Payment creation creates FeeLedger entry")]
    public async Task PaymentCreation_CreatesLedger()
    {
        var invoice = new FeeInvoice { Id = 1, StudentId = 10, TotalAmount = 5000, PaidAmount = 0 };
        _invoiceBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);
        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 2000, Method = 1, PaidAt = DateTime.UtcNow, ReferenceNo = "PAY-001" };

        await svc.CreateAsync(dto, "test-user");

        var ledgerEntry = _ledgerEntries.SingleOrDefault(e => e.TransactionType == FeeLedgerType.Payment);
        Assert.NotNull(ledgerEntry);
        Assert.Equal(10, ledgerEntry.StudentId);
        Assert.Equal(1, ledgerEntry.FeeInvoiceId);
        Assert.Equal(0, ledgerEntry.Debit);
        Assert.Equal(2000, ledgerEntry.Credit);
        Assert.Equal(-2000, ledgerEntry.Balance);
        Assert.Contains("PAY-001", ledgerEntry.Description);
    }

    // ─── 3. Payment update updates ledger ──────────────────────────

    [Fact(DisplayName = "3. Payment update soft-deletes old ledger and creates new")]
    public async Task PaymentUpdate_UpdatesLedger()
    {
        var invoice = new FeeInvoice { Id = 1, StudentId = 10, TotalAmount = 5000, PaidAmount = 2000 };
        _invoiceBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _payments.Add(new Payment { Id = 1, FeeInvoiceId = 1, Amount = 2000 });

        var oldLedger = new FeeLedger { Id = 1, StudentId = 10, FeePaymentId = 1, TransactionType = FeeLedgerType.Payment, Credit = 2000, IsDeleted = false };
        _ledgerEntries.Add(oldLedger);

        _paymentBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.Is<Expression<Func<Payment, bool>>>(e => true), It.IsAny<CancellationToken>()))
            .ReturnsAsync(_payments.First());

        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);
        var dto = new FeePaymentUpsertDto { Id = 1, FeeInvoiceId = 1, Amount = 3000, Method = 1, PaidAt = DateTime.UtcNow, ReferenceNo = "PAY-001" };

        await svc.UpdateAsync(dto, "test-user");

        Assert.True(oldLedger.IsDeleted);
        var newEntry = _ledgerEntries.Last(e => e.TransactionType == FeeLedgerType.Payment);
        Assert.Equal(3000, newEntry.Credit);
        Assert.False(newEntry.IsDeleted);
    }

    // ─── 4. Payment delete creates reversal ledger ─────────────────

    [Fact(DisplayName = "4. Payment deletion creates reversal FeeLedger entry")]
    public async Task PaymentDeletion_CreatesReversalLedger()
    {
        var invoice = new FeeInvoice { Id = 1, StudentId = 10, TotalAmount = 5000, PaidAmount = 2000 };
        _invoiceBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        var payment = new Payment { Id = 1, FeeInvoiceId = 1, Amount = 2000, ReferenceNo = "PAY-001" };
        _payments.Add(payment);

        _paymentBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.Is<Expression<Func<Payment, bool>>>(e => true), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);
        await svc.DeleteAsync(1, "test-user");

        var reversal = _ledgerEntries.SingleOrDefault(e => e.Description != null && e.Description.Contains("reversal"));
        Assert.NotNull(reversal);
        Assert.Equal(FeeLedgerType.Payment, reversal.TransactionType);
        Assert.Equal(2000, reversal.Debit);
        Assert.Equal(0, reversal.Credit);
        Assert.Equal(2000, reversal.Balance);
        Assert.Equal(10, reversal.StudentId);
    }

    // ─── 5. Waiver creates ledger when approved ───────────────────

    [Fact(DisplayName = "5. Approved waiver creates FeeLedger entry")]
    public async Task ApprovedWaiver_CreatesLedger()
    {
        _waiverBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<FeeWaiver>(), It.IsAny<CancellationToken>()))
            .Callback<FeeWaiver, CancellationToken>((w, ct) =>
            {
                w.Id = _waivers.Count + 1;
                _waivers.Add(w);
            })
            .Returns(Task.CompletedTask);

        var svc = new FeeWaiverService(_uowMock.Object, _waiverRepoMock.Object, _auditServiceMock);
        var dto = new FeeWaiverUpsertDto { StudentId = 10, FeeInvoiceId = 1, WaiverAmount = 1000, IsApproved = true };

        await svc.CreateAsync(dto, "test-user");

        var ledgerEntry = _ledgerEntries.SingleOrDefault(e => e.TransactionType == FeeLedgerType.Waiver);
        Assert.NotNull(ledgerEntry);
        Assert.Equal(10, ledgerEntry.StudentId);
        Assert.Equal(0, ledgerEntry.Debit);
        Assert.Equal(1000, ledgerEntry.Credit);
        Assert.Equal(-1000, ledgerEntry.Balance);
    }

    // ─── 6. Refund creates ledger ──────────────────────────────────

    [Fact(DisplayName = "6. Refund approval creates FeeLedger entry")]
    public async Task RefundCreation_CreatesLedger()
    {
        var invoice = new FeeInvoice { Id = 1, StudentId = 10, TotalAmount = 5000 };
        var payment = new Payment { Id = 1, FeeInvoiceId = 1, Amount = 5000 };

        _paymentForRefundRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);
        _invoiceBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        _uowMock.Setup(u => u.Repository<Payment>()).Returns(_paymentForRefundRepoMock.Object);

        _refundBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<FeeRefund>(), It.IsAny<CancellationToken>()))
            .Callback<FeeRefund, CancellationToken>((r, ct) =>
            {
                r.Id = _refunds.Count + 1;
                _refunds.Add(r);
            })
            .Returns(Task.CompletedTask);

        _refundBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeRefund, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<FeeRefund, bool>> expr, CancellationToken ct) =>
                _refunds.AsQueryable().FirstOrDefault(expr));

        var svc = new FeeRefundService(_uowMock.Object, _refundRepoMock.Object, _auditServiceMock);

        var dto = new FeeRefundUpsertDto { FeePaymentId = 1, RefundAmount = 500, ReferenceNo = "REF-001", IsApproved = false };
        await svc.CreateAsync(dto, "test-user");

        Assert.Empty(_ledgerEntries.Where(e => e.TransactionType == FeeLedgerType.Refund));

        await svc.ApproveAsync(1, "test-user");

        var ledgerEntry = _ledgerEntries.SingleOrDefault(e => e.TransactionType == FeeLedgerType.Refund);
        Assert.NotNull(ledgerEntry);
        Assert.Equal(10, ledgerEntry.StudentId);
        Assert.Equal(500, ledgerEntry.Debit);
        Assert.Equal(0, ledgerEntry.Credit);
    }

    // ─── 7. Discount creates ledger ────────────────────────────────

    [Fact(DisplayName = "7. Discount creation creates FeeLedger entry")]
    public async Task DiscountCreation_CreatesLedger()
    {
        _discountBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<FeeDiscount>(), It.IsAny<CancellationToken>()))
            .Callback<FeeDiscount, CancellationToken>((d, ct) =>
            {
                d.Id = _discounts.Count + 1;
                _discounts.Add(d);
            })
            .Returns(Task.CompletedTask);

        var svc = new FeeDiscountService(_uowMock.Object, _discountRepoMock.Object, _auditServiceMock);
        var dto = new FeeDiscountUpsertDto { Name = "Early Bird", Value = 500, DiscountType = 1 };

        await svc.CreateAsync(dto, "test-user");

        var entry = _ledgerEntries.SingleOrDefault(e => e.TransactionType == FeeLedgerType.Discount);
        Assert.NotNull(entry);
        Assert.Equal(0, entry.Debit);
        Assert.Equal(500, entry.Credit);
        Assert.Equal(-500, entry.Balance);
        Assert.Contains("Early Bird", entry.Description);
    }

    // ─── 8. Ledger transaction rollback ────────────────────────────

    [Fact(DisplayName = "8. Ledger write rolls back on payment failure")]
    public async Task LedgerRollback_OnPaymentFailure()
    {
        _uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((op, ct) =>
            {
                op().GetAwaiter().GetResult();
                return Task.CompletedTask;
            });

        var invoice = new FeeInvoice { Id = 1, StudentId = 10, TotalAmount = 5000, PaidAmount = 0 };
        _invoiceBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        _paymentBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB failure"));

        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);
        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 2000, Method = 1, PaidAt = DateTime.UtcNow };

        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateAsync(dto, "test-user"));
        Assert.Empty(_ledgerEntries);
    }

    // ─── 9. Ledger references correct StudentId ────────────────────

    [Fact(DisplayName = "9. Ledger entries reference correct StudentId via invoice")]
    public async Task LedgerEntry_HasCorrectStudentId()
    {
        var invoice = new FeeInvoice { Id = 1, StudentId = 42, TotalAmount = 3000, PaidAmount = 0 };
        _invoiceBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);
        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 3000, Method = 1, PaidAt = DateTime.UtcNow };

        await svc.CreateAsync(dto, "test-user");

        var entry = _ledgerEntries.Single(e => e.TransactionType == FeeLedgerType.Payment);
        Assert.Equal(42, entry.StudentId);
    }

    // ─── 10. Ledger references correct InvoiceNo ───────────────────

    [Fact(DisplayName = "10. Invoice ledger entry contains InvoiceNo in description")]
    public async Task InvoiceLedger_HasInvoiceNo()
    {
        var svc = new FeeInvoiceService(_uowMock.Object, _invoiceRepoMock.Object, _auditServiceMock);
        var invoice = new FeeInvoice { Id = 1, StudentId = 10, InvoiceNo = "INV-2024-001", TotalAmount = 7500 };

        await svc.CreateAsync(invoice, "test-user");

        var entry = _ledgerEntries.Single();
        Assert.Contains("INV-2024-001", entry.Description);
    }

    // ─── 11. Ledger references correct PaymentNo ──────────────────

    [Fact(DisplayName = "11. Payment ledger entry contains ReferenceNo in description")]
    public async Task PaymentLedger_HasReferenceNo()
    {
        var invoice = new FeeInvoice { Id = 1, StudentId = 10, TotalAmount = 5000, PaidAmount = 0 };
        _invoiceBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);
        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 2000, Method = 1, PaidAt = DateTime.UtcNow, ReferenceNo = "PAY-REF-001" };

        await svc.CreateAsync(dto, "test-user");

        var entry = _ledgerEntries.Single(e => e.TransactionType == FeeLedgerType.Payment);
        Assert.Contains("PAY-REF-001", entry.Description);
    }

    // ─── 12. Ledger balance correctness ────────────────────────────

    [Fact(DisplayName = "12. Multiple ledger entries total balance matches expected")]
    public async Task MultipleLedgerEntries_BalanceCorrect()
    {
        var invoice = new FeeInvoice { Id = 1, StudentId = 10, InvoiceNo = "INV-001", TotalAmount = 5000, PaidAmount = 0 };
        _invoiceBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var svc = new FeeInvoiceService(_uowMock.Object, _invoiceRepoMock.Object, _auditServiceMock);
        await svc.CreateAsync(invoice, "test-user");

        var paymentSvc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);
        var dto1 = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 3000, Method = 1, PaidAt = DateTime.UtcNow, ReferenceNo = "P1" };
        await paymentSvc.CreateAsync(dto1, "test-user");

        var dto2 = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 2000, Method = 1, PaidAt = DateTime.UtcNow, ReferenceNo = "P2" };
        await paymentSvc.CreateAsync(dto2, "test-user");

        var totalDebit = _ledgerEntries.Sum(e => e.Debit);
        var totalCredit = _ledgerEntries.Sum(e => e.Credit);
        Assert.Equal(5000, totalDebit);
        Assert.Equal(5000, totalCredit);
        Assert.Equal(0, totalDebit - totalCredit);
    }
}
