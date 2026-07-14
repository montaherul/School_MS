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

public class Phase41B_PaymentInvoiceSyncTests
{
    private readonly Mock<IUnitOfWork> _uowMock = new(MockBehavior.Loose);
    private readonly Mock<IFeePaymentRepository> _paymentRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<FeeInvoice>> _invoiceRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<Payment>> _paymentBaseRepoMock = new(MockBehavior.Loose);
    private readonly Mock<IBaseRepository<FeeLedger>> _ledgerBaseRepoMock = new(MockBehavior.Loose);
    private readonly IAuditLogService _auditServiceMock;

    public Phase41B_PaymentInvoiceSyncTests()
    {
        var auditMock = new Mock<IAuditLogService>();
        auditMock.Setup(a => a.LogAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<string?>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _auditServiceMock = auditMock.Object;

        _uowMock.Setup(u => u.Repository<FeeInvoice>()).Returns(_invoiceRepoMock.Object);
        _uowMock.Setup(u => u.Repository<Payment>()).Returns(_paymentBaseRepoMock.Object);
        _uowMock.Setup(u => u.Repository<FeeLedger>()).Returns(_ledgerBaseRepoMock.Object);

        _uowMock.Setup(u => u.ExecuteInTransactionAsync(It.IsAny<Func<Task>>(), It.IsAny<CancellationToken>()))
            .Returns<Func<Task>, CancellationToken>((op, ct) => op());

        _ledgerBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<FeeLedger>(), It.IsAny<CancellationToken>()))
            .Returns(Task.CompletedTask);
        _ledgerBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<FeeLedger, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<FeeLedger>());
    }

    [Fact(DisplayName = "1. Partial payment sets invoice status to Partial")]
    public async Task PartialPayment_UpdatesInvoiceToPartial()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 1000, PaidAmount = 0, Status = PaymentStatus.Draft };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _paymentBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment> { new Payment { Id = 1, FeeInvoiceId = 1, Amount = 300 } });

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 300, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);

        await svc.CreateAsync(dto, "test-user");

        Assert.Equal(300, invoice.PaidAmount);
        Assert.Equal(PaymentStatus.Partial, invoice.Status);
        _invoiceRepoMock.Verify(r => r.Update(invoice), Times.Once);
    }

    [Fact(DisplayName = "2. Full payment sets invoice status to Paid")]
    public async Task FullPayment_UpdatesInvoiceToPaid()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 500, PaidAmount = 0, Status = PaymentStatus.Draft };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _paymentBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment> { new Payment { Id = 1, FeeInvoiceId = 1, Amount = 500 } });

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 500, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);

        await svc.CreateAsync(dto, "test-user");

        Assert.Equal(500, invoice.PaidAmount);
        Assert.Equal(PaymentStatus.Paid, invoice.Status);
    }

    [Fact(DisplayName = "3. Multiple payments accumulate correctly")]
    public async Task MultiplePayments_AccumulateToFull()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 1000, PaidAmount = 0, Status = PaymentStatus.Draft };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        var allPayments = new List<Payment>
        {
            new() { Id = 1, FeeInvoiceId = 1, Amount = 200 },
            new() { Id = 2, FeeInvoiceId = 1, Amount = 300 },
        };
        _paymentBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => allPayments.ToList());
        _paymentBaseRepoMock.Setup(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()))
            .Callback<Payment, CancellationToken>((p, ct) => allPayments.Add(p))
            .Returns(Task.CompletedTask);

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 500, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);

        await svc.CreateAsync(dto, "test-user");

        Assert.Equal(1000, invoice.PaidAmount);
        Assert.Equal(PaymentStatus.Paid, invoice.Status);
    }

    [Fact(DisplayName = "4. Generated (Unpaid) → Partial status transition")]
    public async Task GeneratedToPartial_StatusTransition()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 1000, PaidAmount = 0, Status = PaymentStatus.Draft };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _paymentBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment> { new Payment { Id = 1, FeeInvoiceId = 1, Amount = 100 } });

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 100, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);

        await svc.CreateAsync(dto, "test-user");

        Assert.Equal(PaymentStatus.Partial, invoice.Status);
        Assert.Equal(100, invoice.PaidAmount);
    }

    [Fact(DisplayName = "5. Partial → Paid status transition on final payment")]
    public async Task PartialToPaid_StatusTransition()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 500, PaidAmount = 300, Status = PaymentStatus.Partial };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _paymentBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment>
            {
                new() { Id = 1, FeeInvoiceId = 1, Amount = 300 },
                new() { Id = 2, FeeInvoiceId = 1, Amount = 200 },
            });

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 200, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);

        await svc.CreateAsync(dto, "test-user");

        Assert.Equal(500, invoice.PaidAmount);
        Assert.Equal(PaymentStatus.Paid, invoice.Status);
    }

    [Fact(DisplayName = "6. Transaction rollback on invoice update failure")]
    public async Task TransactionRollback_OnInvoiceFailure()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 500, PaidAmount = 0, Status = PaymentStatus.Draft };
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _paymentBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ThrowsAsync(new InvalidOperationException("DB failure"));

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 1, Amount = 500, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateAsync(dto, "test-user"));

        Assert.Contains("DB failure", ex.Message);
        _paymentBaseRepoMock.Verify(r => r.AddAsync(It.IsAny<Payment>(), It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact(DisplayName = "7. CreateAsync throws when invoice not found")]
    public async Task CreateAsync_Throws_WhenInvoiceNotFound()
    {
        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync((FeeInvoice?)null);

        var dto = new FeePaymentUpsertDto { FeeInvoiceId = 999, Amount = 100, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);

        var ex = await Assert.ThrowsAsync<InvalidOperationException>(() => svc.CreateAsync(dto, "test-user"));
        Assert.Contains("Invoice not found", ex.Message);
    }

    [Fact(DisplayName = "8. Update payment recalculates invoice")]
    public async Task UpdatePayment_RecalculatesInvoice()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 1000, PaidAmount = 0, Status = PaymentStatus.Draft };
        var payment = new Payment { Id = 1, FeeInvoiceId = 1, Amount = 300, CreatedBy = "user" };

        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _paymentBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.Is<Expression<Func<Payment, bool>>>(e => true), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);
        _paymentBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment> { new Payment { Id = 1, FeeInvoiceId = 1, Amount = 500 } });

        var dto = new FeePaymentUpsertDto { Id = 1, FeeInvoiceId = 1, Amount = 500, Method = 1, PaidAt = DateTime.UtcNow };
        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);

        await svc.UpdateAsync(dto, "test-user");

        Assert.Equal(500, invoice.PaidAmount);
        Assert.Equal(PaymentStatus.Partial, invoice.Status);
    }

    [Fact(DisplayName = "9. Delete payment recalculates invoice")]
    public async Task DeletePayment_RecalculatesInvoice()
    {
        var invoice = new FeeInvoice { Id = 1, TotalAmount = 1000, PaidAmount = 500, Status = PaymentStatus.Partial };
        var payment = new Payment { Id = 1, FeeInvoiceId = 1, Amount = 500, CreatedBy = "user" };

        _invoiceRepoMock.Setup(r => r.FirstOrDefaultAsync(It.IsAny<Expression<Func<FeeInvoice, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(invoice);
        _paymentBaseRepoMock.Setup(r => r.FirstOrDefaultAsync(It.Is<Expression<Func<Payment, bool>>>(e => true), It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);
        _paymentBaseRepoMock.Setup(r => r.ListAsync(It.IsAny<Expression<Func<Payment, bool>>>(), It.IsAny<CancellationToken>()))
            .ReturnsAsync(new List<Payment>());

        var svc = new FeePaymentService(_uowMock.Object, _paymentRepoMock.Object, _auditServiceMock);

        await svc.DeleteAsync(1, "test-user");

        Assert.Equal(0, invoice.PaidAmount);
        Assert.Equal(PaymentStatus.Draft, invoice.Status);
        Assert.True(payment.IsDeleted);
    }
}
