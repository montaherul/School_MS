using Xunit;
using Moq;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Services.Implementations.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Repositories.Interfaces;
using System.Linq.Expressions;

namespace SchoolManagementSystem.Tests.Services;

public class Phase41B2_OverpaymentValidationTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<IFeePaymentRepository> _paymentRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<FeeInvoice>> _invoiceRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<Payment>> _paymentBaseRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<FeeLedger>> _ledgerBaseRepoMock = new(MockBehavior.Loose);
    private readonly List<Payment> _allPayments = new();

    public Phase41B2_OverpaymentValidationTests()
    {
        _uowMock.Setup(u => u.Repository<FeeInvoice>()).Returns(_invoiceRepoMock.Object);
        _uowMock.Setup(u => u.Repository<Payment>()).Returns(_paymentBaseRepoMock.Object);
        _uowMock.Setup(u => u.Repository<FeeLedger>()).Returns(_ledgerBaseRepoMock.Object);

        _uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((op, ct) => op());

        _ledgerBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<FeeLedger>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _ledgerBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<FeeLedger, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FeeLedger>());

        _paymentBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Payment, bool>> predicate, CancellationToken ct) =>
            {
                var compiled = predicate.Compile();
                return _allPayments.Where(p => compiled(p)).ToList();
            });

        _paymentBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Callback<Payment, CancellationToken>((p, ct) => _allPayments.Add(p))
            .Returns(Task.CompletedTask);

        _paymentBaseRepoMock.Setup(r => r.Update(It.IsAny<Payment>()))
            .Callback<Payment>(p =>
            {
                var idx = _allPayments.FindIndex(x => x.Id == p.Id);
                if (idx >= 0) _allPayments[idx] = p;
            });

        _paymentBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((Expression<Func<Payment, bool>> expr, CancellationToken ct) =>
            {
                var compiled = expr.Compile();
                return _allPayments.FirstOrDefault(p => compiled(p));
            });
    }

    // ───── Rule 1: Exact balance (remaining == payment) ──────────────

    [Fact(DisplayName = "1. Create payment exactly equal to remaining balance")]
    public async Task Create_ExactRemainingBalance_Succeeds()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 10000, PaidAmount = 8000, Status = PaymentStatus.Partial };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _allPayments.AddRange(new[]
        {
            new Payment { Id = 1, FeeInvoiceId = 1, Amount = 5000 },
            new Payment { Id = 2, FeeInvoiceId = 1, Amount = 3000 },
        });

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 2000, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        await svc.CreateAsync(dto, "test-user");

        Assert.Equal(10000, invoice.PaidAmount);
        Assert.Equal(PaymentStatus.Paid, invoice.Status);
    }

    // ───── Rule 2: Below remaining balance ───────────────────────────

    [Fact(DisplayName = "2. Create payment below remaining balance")]
    public async Task Create_BelowRemainingBalance_Succeeds()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 10000, PaidAmount = 5000, Status = PaymentStatus.Partial };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _allPayments.Add(new Payment { Id = 1, FeeInvoiceId = 1, Amount = 5000 });

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 3000, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        await svc.CreateAsync(dto, "test-user");

        Assert.Equal(8000, invoice.PaidAmount);
        Assert.Equal(PaymentStatus.Partial, invoice.Status);
    }

    // ───── Rule 3: Exceeding remaining balance → REJECTED ────────────

    [Fact(DisplayName = "3. Create payment exceeding remaining balance is rejected")]
    public async Task Create_ExceedingRemainingBalance_Throws()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 10000, PaidAmount = 8000, Status = PaymentStatus.Partial };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _allPayments.AddRange(new[]
        {
            new Payment { Id = 1, FeeInvoiceId = 1, Amount = 5000 },
            new Payment { Id = 2, FeeInvoiceId = 1, Amount = 3000 },
        });

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 5000, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateAsync(dto, "test-user"));
        Assert.Contains("exceeds outstanding", ex.Message);
    }

    // ───── Rule 4: Multiple payments reaching exact total ────────────

    [Fact(DisplayName = "4. Multiple payments reaching exact invoice total")]
    public async Task MultiplePayments_ExactTotal_Succeeds()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 10000, PaidAmount = 0, Status = PaymentStatus.Draft };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        var paymentIdSeq = 1;

        _paymentBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Callback<Payment, CancellationToken>((p, ct) =>
            {
                p.Id = paymentIdSeq++;
                _allPayments.Add(p);
            })
            .Returns(Task.CompletedTask);

        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        var id1 = await svc.CreateAsync(new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 3000, Method = 1, PaidAt = DateTime.UtcNow }, "user");
        var id2 = await svc.CreateAsync(new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 4000, Method = 1, PaidAt = DateTime.UtcNow }, "user");
        var id3 = await svc.CreateAsync(new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 3000, Method = 1, PaidAt = DateTime.UtcNow }, "user");

        Assert.Equal(10000, invoice.PaidAmount);
        Assert.Equal(PaymentStatus.Paid, invoice.Status);
    }

    // ───── Rule 5: Update remaining balance within limit ─────────────

    [Fact(DisplayName = "5. Update payment within balance succeeds")]
    public async Task UpdatePayment_WithinBalance_Succeeds()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 10000, PaidAmount = 5000, Status = PaymentStatus.Partial };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _allPayments.AddRange(new[]
        {
            new Payment { Id = 1, FeeInvoiceId = 1, Amount = 2000 },
            new Payment { Id = 2, FeeInvoiceId = 1, Amount = 3000 },
        });

        var dto = new FeePaymentUpsertDto { Id = 1, FeeInvoiceId = 1, Amount = 4000, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        await svc.UpdateAsync(dto, "test-user");

        Assert.Equal(7000, invoice.PaidAmount);
        Assert.Equal(PaymentStatus.Partial, invoice.Status);
    }

    // ───── Rule 6: Update exceeding balance → REJECTED ──────────────

    [Fact(DisplayName = "6. Update payment exceeding balance is rejected")]
    public async Task UpdatePayment_ExceedingBalance_Throws()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 10000, PaidAmount = 5000, Status = PaymentStatus.Partial };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _allPayments.AddRange(new[]
        {
            new Payment { Id = 1, FeeInvoiceId = 1, Amount = 2000 },
            new Payment { Id = 2, FeeInvoiceId = 1, Amount = 3000 },
        });

        var dto = new FeePaymentUpsertDto { Id = 1, FeeInvoiceId = 1, Amount = 8000, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.UpdateAsync(dto, "test-user"));
        Assert.Contains("exceeds outstanding", ex.Message);
    }

    // ───── Rule 7: Delete recalculates ──────────────────────────────

    [Fact(DisplayName = "7. Delete payment recalculates invoice correctly")]
    public async Task DeletePayment_RecalculatesInvoice()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 10000, PaidAmount = 5000, Status = PaymentStatus.Partial };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        var payment = new Payment { Id = 1, FeeInvoiceId = 1, Amount = 5000 };
        _allPayments.Add(payment);

        _paymentBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.Is<Expression<Func<Payment, bool>>>(e => true), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        await svc.DeleteAsync(1, "test-user");

        Assert.Equal(0, invoice.PaidAmount);
        Assert.Equal(PaymentStatus.Draft, invoice.Status);
        Assert.True(payment.IsDeleted);
    }

    // ───── Rule 8: Transaction commits after valid payment ───────────

    [Fact(DisplayName = "8. Transaction commits after valid payment")]
    public async Task ValidPayment_TransactionCommits()
    {
        var wasTransactionCalled = false;
        _uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((op, ct) =>
            {
                wasTransactionCalled = true;
                return op();
            });

        var invoice = new FeeInvoice { Id = 1, TotalAmount = 500, PaidAmount = 0, Status = PaymentStatus.Draft };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 500, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        var id = await svc.CreateAsync(dto, "test-user");
        Assert.True(wasTransactionCalled);
    }

    // ───── Rule 9: Transaction rolls back after invalid payment ──────

    [Fact(DisplayName = "9. Overpayment attempt does not create payment record")]
    public async Task Overpayment_DoesNotCreatePaymentRecord()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 10000, PaidAmount = 9000, Status = PaymentStatus.Partial };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        var addWasCalled = false;
        _paymentBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Callback<Payment, CancellationToken>((p, ct) => addWasCalled = true)
            .Returns(Task.CompletedTask);

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 2000, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateAsync(dto, "test-user"));
        Assert.False(addWasCalled);
        Assert.Empty(_allPayments);
    }

    // ───── Rule 10: Status remains Partial ──────────────────────────

    [Fact(DisplayName = "10. Invoice status remains Partial after below-full payment")]
    public async Task PartialPayment_StatusRemainsPartial()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 10000, PaidAmount = 6000, Status = PaymentStatus.Partial };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _allPayments.Add(new Payment { Id = 1, FeeInvoiceId = 1, Amount = 6000 });

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 3000, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        await svc.CreateAsync(dto, "test-user");
        Assert.Equal(PaymentStatus.Partial, invoice.Status);
    }

    // ───── Rule 11: Status remains Paid ────────────────────────────

    [Fact(DisplayName = "11. Invoice status remains Paid when already fully paid")]
    public async Task AlreadyPaidInvoice_StatusRemainsPaid()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 5000, PaidAmount = 5000, Status = PaymentStatus.Paid };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _allPayments.Add(new Payment { Id = 1, FeeInvoiceId = 1, Amount = 5000 });

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 100, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateAsync(dto, "test-user"));
        Assert.Contains("exceeds outstanding", ex.Message);
        Assert.Equal(PaymentStatus.Paid, invoice.Status);
    }

    // ───── Rule 12: Zero payment rejected ──────────────────────────

    [Fact(DisplayName = "12. Zero payment amount is rejected")]
    public async Task ZeroPayment_Throws()
    {
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 0, Method = 1, PaidAt = DateTime.UtcNow };
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateAsync(dto, "test-user"));
        Assert.Contains("greater than zero", ex.Message);
    }

    // ───── Rule 13: Negative payment rejected ────────────────────────

    [Fact(DisplayName = "13. Negative payment amount is rejected")]
    public async Task NegativePayment_Throws()
    {
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object);

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = -500, Method = 1, PaidAt = DateTime.UtcNow };
        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateAsync(dto, "test-user"));
        Assert.Contains("greater than zero", ex.Message);
    }
}
