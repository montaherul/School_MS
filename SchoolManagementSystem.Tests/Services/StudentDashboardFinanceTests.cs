using Moq;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Repositories.Interfaces.Fees;
using SchoolManagementSystem.Services.Implementations.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class StudentDashboardFinanceTests
{
    private readonly Mock<IStudentFinanceRepository> _repoMock = new(MockBehavior.Strict);
    private IStudentFinanceService CreateService() => new StudentFinanceService(_repoMock.Object);

    [Fact]
    public async Task GetInvoicesPagedAsync_ReturnsPagedInvoices()
    {
        var invoices = new List<StudentInvoiceDto>
        {
            new() { Id = 1, InvoiceNo = "INV-001", TotalAmount = 5000, PaidAmount = 2000, Status = 2, TotalRecords = 2 },
            new() { Id = 2, InvoiceNo = "INV-002", TotalAmount = 3000, PaidAmount = 3000, Status = 3, TotalRecords = 2 }
        };
        _repoMock.Setup(r => r.GetInvoicesPagedAsync(1, 1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((invoices, 2));

        var service = CreateService();
        var result = await service.GetInvoicesPagedAsync(1, 1, 10, null, null, CancellationToken.None);

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(2, result.Items.Count);
        Assert.Contains(result.Items, i => i.InvoiceNo == "INV-001");
        _repoMock.VerifyAll();
    }

    [Fact]
    public async Task GetInvoicesPagedAsync_EmptyList_WhenNoInvoices()
    {
        _repoMock.Setup(r => r.GetInvoicesPagedAsync(99, 1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((new List<StudentInvoiceDto>(), 0));

        var service = CreateService();
        var result = await service.GetInvoicesPagedAsync(99, 1, 10, null, null, CancellationToken.None);

        Assert.Empty(result.Items);
        Assert.Equal(0, result.TotalItems);
        _repoMock.VerifyAll();
    }

    [Fact]
    public async Task GetPaymentsPagedAsync_ReturnsPagedPayments()
    {
        var payments = new List<StudentPaymentDto>
        {
            new() { Id = 1, Amount = 2000, PaymentDate = DateTime.UtcNow, InvoiceNo = "INV-001", TotalRecords = 1 }
        };
        _repoMock.Setup(r => r.GetPaymentsPagedAsync(1, 1, 10, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((payments, 1));

        var service = CreateService();
        var result = await service.GetPaymentsPagedAsync(1, 1, 10, null, CancellationToken.None);

        Assert.Single(result.Items);
        Assert.Equal(2000, result.Items[0].Amount);
        _repoMock.VerifyAll();
    }

    [Fact]
    public async Task GetLedgerPagedAsync_ReturnsPagedLedger()
    {
        var entries = new List<StudentLedgerEntryDto>
        {
            new() { Id = 1, Debit = 5000, Credit = 0, Balance = 5000, Type = 1, Description = "Invoice", TotalRecords = 2 },
            new() { Id = 2, Debit = 0, Credit = 2000, Balance = 3000, Type = 2, Description = "Payment", TotalRecords = 2 }
        };
        _repoMock.Setup(r => r.GetLedgerPagedAsync(1, 1, 10, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync((entries, 2));

        var service = CreateService();
        var result = await service.GetLedgerPagedAsync(1, 1, 10, null, CancellationToken.None);

        Assert.Equal(2, result.TotalItems);
        Assert.Equal(5000, result.Items[0].Debit);
        Assert.Equal(2000, result.Items[1].Credit);
        _repoMock.VerifyAll();
    }

    [Fact]
    public async Task GetFinanceSummaryAsync_CalculatesCorrectTotals()
    {
        _repoMock.Setup(r => r.GetFinanceSummaryAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync((10000m, 6000m));

        var service = CreateService();
        var (invoiced, paid, due) = await service.GetFinanceSummaryAsync(1, CancellationToken.None);

        Assert.Equal(10000, invoiced);
        Assert.Equal(6000, paid);
        Assert.Equal(4000, due);
        _repoMock.VerifyAll();
    }

    [Fact]
    public async Task GetFinanceSummaryAsync_ZeroWhenNoData()
    {
        _repoMock.Setup(r => r.GetFinanceSummaryAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((0m, 0m));

        var service = CreateService();
        var (invoiced, paid, due) = await service.GetFinanceSummaryAsync(99, CancellationToken.None);

        Assert.Equal(0, invoiced);
        Assert.Equal(0, paid);
        Assert.Equal(0, due);
        _repoMock.VerifyAll();
    }

    [Fact]
    public async Task GetLastPaymentAsync_ReturnsLastPayment_WhenExists()
    {
        var payment = new StudentPaymentDto { Id = 5, Amount = 2500, PaymentDate = DateTime.UtcNow };
        _repoMock.Setup(r => r.GetLastPaymentAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(payment);

        var service = CreateService();
        var result = await service.GetLastPaymentAsync(1, CancellationToken.None);

        Assert.NotNull(result);
        Assert.Equal(2500, result.Amount);
        _repoMock.VerifyAll();
    }

    [Fact]
    public async Task GetLastPaymentAsync_ReturnsNull_WhenNoPayments()
    {
        _repoMock.Setup(r => r.GetLastPaymentAsync(99, It.IsAny<CancellationToken>()))
            .ReturnsAsync((StudentPaymentDto?)null);

        var service = CreateService();
        var result = await service.GetLastPaymentAsync(99, CancellationToken.None);

        Assert.Null(result);
        _repoMock.VerifyAll();
    }
}