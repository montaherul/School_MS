using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using SchoolManagementSystem.Controllers.Dashboard;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Guardian;
using System.Security.Claims;
using Xunit;

namespace SchoolManagementSystem.Tests.Services;

public class GuardianFinanceControllerTests
{
    private readonly Mock<IStudentFinanceService> _financeMock = new(MockBehavior.Strict);
    private readonly Mock<IFeeReceiptService> _receiptMock = new(MockBehavior.Strict);
    private readonly Mock<IGuardianService> _guardianMock = new(MockBehavior.Strict);

    private GuardianFinanceController CreateController(int userId = 1)
    {
        var controller = new GuardianFinanceController(_financeMock.Object, _receiptMock.Object, _guardianMock.Object);
        var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
        {
            new Claim(ClaimTypes.NameIdentifier, userId.ToString()),
            new Claim(ClaimTypes.Role, "Guardian")
        }, "mock"));
        controller.ControllerContext = new ControllerContext { HttpContext = new DefaultHttpContext { User = user } };
        return controller;
    }

    [Fact(DisplayName = "GetInvoices returns data when guardian has access")]
    public async Task GetInvoices_WithAccess_ReturnsData()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _financeMock.Setup(f => f.GetInvoicesPagedAsync(1, 1, 10, null, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<StudentInvoiceDto>
            {
                Items = [new() { Id = 1, InvoiceNo = "INV-001", TotalAmount = 5000 }],
                TotalItems = 1, Page = 1, PageSize = 10
            });

        var controller = CreateController();
        var result = await controller.GetInvoices(1, 1, 10, null, null) as JsonResult;

        Assert.NotNull(result);
        var data = result.Value.GetType().GetProperty("data")?.GetValue(result.Value) as System.Collections.IList;
        Assert.Single(data!);
        _guardianMock.VerifyAll();
        _financeMock.VerifyAll();
    }

    [Fact(DisplayName = "GetInvoices returns empty when guardian lacks access")]
    public async Task GetInvoices_WithoutAccess_ReturnsEmpty()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 99, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var controller = CreateController();
        var result = await controller.GetInvoices(99) as JsonResult;

        Assert.NotNull(result);
        var data = result.Value.GetType().GetProperty("data")?.GetValue(result.Value) as System.Collections.IList;
        Assert.Empty(data!);
        _guardianMock.VerifyAll();
        _financeMock.VerifyNoOtherCalls();
    }

    [Fact(DisplayName = "GetPayments returns data when guardian has access")]
    public async Task GetPayments_WithAccess_ReturnsData()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _financeMock.Setup(f => f.GetPaymentsPagedAsync(1, 1, 10, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<StudentPaymentDto>
            {
                Items = [new() { Id = 1, Amount = 2000 }],
                TotalItems = 1, Page = 1, PageSize = 10
            });

        var controller = CreateController();
        var result = await controller.GetPayments(1) as JsonResult;

        Assert.NotNull(result);
        var data = result.Value.GetType().GetProperty("data")?.GetValue(result.Value) as System.Collections.IList;
        Assert.Single(data!);
        _guardianMock.VerifyAll();
        _financeMock.VerifyAll();
    }

    [Fact(DisplayName = "GetPayments returns empty when guardian lacks access")]
    public async Task GetPayments_WithoutAccess_ReturnsEmpty()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 99, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var controller = CreateController();
        var result = await controller.GetPayments(99) as JsonResult;

        Assert.NotNull(result);
        var data = result.Value.GetType().GetProperty("data")?.GetValue(result.Value) as System.Collections.IList;
        Assert.Empty(data!);
        _guardianMock.VerifyAll();
        _financeMock.VerifyNoOtherCalls();
    }

    [Fact(DisplayName = "GetLedger returns data when guardian has access")]
    public async Task GetLedger_WithAccess_ReturnsData()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _financeMock.Setup(f => f.GetLedgerPagedAsync(1, 1, 10, null, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new PagedResult<StudentLedgerEntryDto>
            {
                Items = [new() { Id = 1, Debit = 5000, Balance = 5000 }],
                TotalItems = 1, Page = 1, PageSize = 10
            });

        var controller = CreateController();
        var result = await controller.GetLedger(1) as JsonResult;

        Assert.NotNull(result);
        var data = result.Value.GetType().GetProperty("data")?.GetValue(result.Value) as System.Collections.IList;
        Assert.Single(data!);
        _guardianMock.VerifyAll();
        _financeMock.VerifyAll();
    }

    [Fact(DisplayName = "GetLedger returns empty when guardian lacks access")]
    public async Task GetLedger_WithoutAccess_ReturnsEmpty()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 99, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var controller = CreateController();
        var result = await controller.GetLedger(99) as JsonResult;

        Assert.NotNull(result);
        var data = result.Value.GetType().GetProperty("data")?.GetValue(result.Value) as System.Collections.IList;
        Assert.Empty(data!);
        _guardianMock.VerifyAll();
        _financeMock.VerifyNoOtherCalls();
    }

    [Fact(DisplayName = "GetFinanceSummary returns data when guardian has access")]
    public async Task GetFinanceSummary_WithAccess_ReturnsData()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _financeMock.Setup(f => f.GetFinanceSummaryAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync((5000m, 2000m, 3000m));
        _financeMock.Setup(f => f.GetLastPaymentAsync(1, It.IsAny<CancellationToken>())).ReturnsAsync(new StudentPaymentDto { Id = 5, Amount = 2000 });

        var controller = CreateController();
        var result = await controller.GetFinanceSummary(1) as JsonResult;

        Assert.NotNull(result);
        var totalDue = result.Value.GetType().GetProperty("totalDue")?.GetValue(result.Value);
        Assert.Equal(3000m, totalDue);
        _guardianMock.VerifyAll();
        _financeMock.VerifyAll();
    }

    [Fact(DisplayName = "GetFinanceSummary returns empty when guardian lacks access")]
    public async Task GetFinanceSummary_WithoutAccess_ReturnsZeros()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 99, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var controller = CreateController();
        var result = await controller.GetFinanceSummary(99) as JsonResult;

        Assert.NotNull(result);
        var totalDue = result.Value.GetType().GetProperty("totalDue")?.GetValue(result.Value);
        Assert.Equal(0m, totalDue);
        _guardianMock.VerifyAll();
        _financeMock.VerifyNoOtherCalls();
    }

    [Fact(DisplayName = "DownloadReceipt returns PDF when payment belongs to authorized student")]
    public async Task DownloadReceipt_WithValidPayment_ReturnsFile()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _receiptMock.Setup(r => r.GetReceiptDataAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FeeReceiptDto { StudentId = 1, ReceiptNo = "RCP-000010" });
        _receiptMock.Setup(r => r.GenerateReceiptPdfAsync(10, It.IsAny<CancellationToken>())).ReturnsAsync([0x25, 0x50, 0x44, 0x46]);

        var controller = CreateController();
        var result = await controller.DownloadReceipt(10, 1) as FileResult;

        Assert.NotNull(result);
        Assert.Equal("application/pdf", result.ContentType);
        _guardianMock.VerifyAll();
        _receiptMock.VerifyAll();
    }

    [Fact(DisplayName = "DownloadReceipt returns Forbid when payment studentId doesn't match")]
    public async Task DownloadReceipt_WithMismatchedStudentId_ReturnsForbid()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _receiptMock.Setup(r => r.GetReceiptDataAsync(10, It.IsAny<CancellationToken>()))
            .ReturnsAsync(new FeeReceiptDto { StudentId = 99, ReceiptNo = "RCP-000010" });

        var controller = CreateController();
        var result = await controller.DownloadReceipt(10, 1);

        Assert.IsType<ForbidResult>(result);
        _guardianMock.VerifyAll();
        _receiptMock.VerifyAll();
    }

    [Fact(DisplayName = "DownloadReceipt returns NotFound when payment not found")]
    public async Task DownloadReceipt_WithMissingPayment_ReturnsNotFound()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 1, It.IsAny<CancellationToken>())).ReturnsAsync(true);
        _receiptMock.Setup(r => r.GetReceiptDataAsync(999, It.IsAny<CancellationToken>())).ReturnsAsync((FeeReceiptDto?)null);

        var controller = CreateController();
        var result = await controller.DownloadReceipt(999, 1);

        Assert.IsType<ForbidResult>(result);
        _guardianMock.VerifyAll();
        _receiptMock.VerifyAll();
    }

    [Fact(DisplayName = "DownloadReceipt returns Forbid when guardian lacks student access")]
    public async Task DownloadReceipt_WithoutStudentAccess_ReturnsForbid()
    {
        _guardianMock.Setup(g => g.UserHasAccessToStudentAsync(1, 99, It.IsAny<CancellationToken>())).ReturnsAsync(false);

        var controller = CreateController();
        var result = await controller.DownloadReceipt(10, 99);

        Assert.IsType<ForbidResult>(result);
        _guardianMock.VerifyAll();
        _receiptMock.VerifyNoOtherCalls();
    }
}
