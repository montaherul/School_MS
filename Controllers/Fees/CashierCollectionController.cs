using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
[RequirePermission("CashierCollection.View")]
public class CashierCollectionController : Controller
{
    private readonly ICashierCollectionService _service;
    private readonly IFeeSecurityService _security;
    private readonly IFeeReceiptService _receiptService;
    private readonly IFinancePostingService _postingService;

    public CashierCollectionController(
        ICashierCollectionService service,
        IFeeSecurityService security,
        IFeeReceiptService receiptService,
        IFinancePostingService postingService)
    {
        _service = service;
        _security = security;
        _receiptService = receiptService;
        _postingService = postingService;
    }

    [HttpGet]
    public IActionResult Index()
    {
        return View("~/Views/Fee/CashierCollection/Index.cshtml");
    }

    [HttpPost]
    public async Task<IActionResult> Search([FromBody] CashierCollectionSearchDto dto)
    {
        var results = await _service.SearchStudentsAsync(dto.SearchTerm ?? "");
        return Json(results);
    }

    [HttpGet]
    public async Task<IActionResult> Collect(int studentId)
    {
        if (!_security.CanAccessStudentData(User, studentId))
            return Forbid();

        var data = await _service.GetStudentCollectionDataAsync(studentId);
        if (data == null) return NotFound();
        return View("~/Views/Fee/CashierCollection/Collect.cshtml", data);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(int studentId, [FromBody] CashierPayRequest request)
    {
        if (!_security.CanAccessStudentData(User, studentId))
            return Forbid();

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        var paymentDto = new CashierPaymentDto
        {
            Amount = request.Amount,
            LateFee = request.LateFee,
            DiscountAmount = request.DiscountAmount,
            Method = request.Method,
            ReferenceNo = request.ReferenceNo,
            Remarks = request.Remarks
        };

        var result = await _service.ProcessPaymentAsync(studentId, request.InvoiceIds, paymentDto, userId);
        if (result.Success && result.PaymentId > 0)
        {
            await _postingService.PostFeeCollectionAsync(
                studentId, request.Amount, request.InvoiceIds.FirstOrDefault(), userId);
        }
        return Json(result);
    }

    [HttpGet]
    public async Task<IActionResult> Success(int paymentId)
    {
        var receipt = await _receiptService.GetReceiptDataAsync(paymentId);
        if (receipt == null) return NotFound();
        return View("~/Views/Fee/CashierCollection/Success.cshtml", receipt);
    }
}

public class CashierPayRequest
{
    public List<int> InvoiceIds { get; set; } = [];
    public decimal Amount { get; set; }
    public decimal LateFee { get; set; }
    public decimal DiscountAmount { get; set; }
    public int Method { get; set; } = 1;
    public string? ReferenceNo { get; set; }
    public string? Remarks { get; set; }
}
