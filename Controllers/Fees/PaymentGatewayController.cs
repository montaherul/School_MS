using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Audit;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[AllowAnonymous]
[Route("Fees/PaymentGateway")]
public class PaymentGatewayController : Controller
{
    private readonly IPaymentGatewayService _gatewayService;
    private readonly IOnlinePaymentService _onlinePaymentService;
    private readonly IFeeReceiptService _receiptService;
    private readonly IAuditService _auditService;
    private readonly ILogger<PaymentGatewayController> _logger;

    public PaymentGatewayController(
        IPaymentGatewayService gatewayService,
        IOnlinePaymentService onlinePaymentService,
        IFeeReceiptService receiptService,
        IAuditService auditService,
        ILogger<PaymentGatewayController> logger)
    {
        _gatewayService = gatewayService;
        _onlinePaymentService = onlinePaymentService;
        _receiptService = receiptService;
        _auditService = auditService;
        _logger = logger;
    }

    private void SetPortalNavigation()
    {
        var isAuthenticated = User?.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            ViewBag.PortalHome = "/";
            ViewBag.PortalFees = "/Admission/Apply";
            ViewBag.PortalLabel = "Home";
            return;
        }
        var roles = User?.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList() ?? [];
        if (roles.Contains("Student"))
        {
            ViewBag.PortalHome = "/Student/Portal/Dashboard";
            ViewBag.PortalFees = "/Student/Portal/Fees";
            ViewBag.PortalLabel = "Student Portal";
        }
        else if (roles.Contains("Guardian"))
        {
            ViewBag.PortalHome = "/Guardian/Portal/Dashboard";
            ViewBag.PortalFees = "/Guardian/Portal/Fees";
            ViewBag.PortalLabel = "Guardian Portal";
        }
        else
        {
            ViewBag.PortalHome = "/Dashboard/Index";
            ViewBag.PortalFees = "/Fee/FeeDashboard/Index";
            ViewBag.PortalLabel = "Dashboard";
        }
    }

    [HttpGet("Init/{requestId:int}")]
    public async Task<IActionResult> Init(int requestId, CancellationToken ct)
    {
        var request = await _onlinePaymentService.GetByIdAsync(requestId, ct);
        if (request == null)
            return NotFound("Payment request not found.");

        if (request.Status == OnlinePaymentRequestStatus.Verified)
        {
            TempData["SuccessMessage"] = "This payment has already been verified.";
            return RedirectToAction(nameof(Success));
        }

        var result = await _gatewayService.InitiatePaymentAsync(requestId, null, ct);
        if (result == null || result.status != "SUCCESS" || string.IsNullOrEmpty(result.GatewayPageURL))
        {
            _logger.LogError("SSLCommerz init failed for request {RequestId}: {Reason}", requestId, result?.failedreason);
            TempData["ErrorMessage"] = "Payment gateway initialization failed. Please try again later.";
            return RedirectToAction(nameof(Fail));
        }

        return Redirect(result.GatewayPageURL);
    }

    [HttpGet("PayDirect/{invoiceId:int}")]
    public async Task<IActionResult> PayDirect(int invoiceId, int? studentId, CancellationToken ct)
    {
        if (studentId == null || studentId == 0)
            return RedirectToAction(nameof(Fail));

        try
        {
            var request = await _onlinePaymentService.CreateGatewayPendingAsync(studentId.Value, invoiceId, "portal", ct);
            await _auditService.LogAsync(null, "Payment", "GatewayInitRequest",
                $"InvoiceId={invoiceId}, StudentId={studentId}, RequestId={request.Id}", ct);
            return RedirectToAction(nameof(Init), new { requestId = request.Id });
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning(ex, "PayDirect failed for invoice {InvoiceId}", invoiceId);
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Fail));
        }
    }

    [HttpGet("Success")]
    public async Task<IActionResult> Success(
        string? tran_id,
        string? bank_tran_id,
        string? val_id,
        string? card_type,
        string? status,
        CancellationToken ct)
    {
        SetPortalNavigation();

        if (string.IsNullOrEmpty(tran_id))
        {
            ViewBag.Message = "Invalid payment response.";
            return View("~/Views/Fees/PaymentGateway/Success.cshtml");
        }

        var request = await _onlinePaymentService.GetByGatewayTransactionIdAsync(tran_id, ct);
        if (request == null)
        {
            ViewBag.Message = "Payment request not found for this transaction.";
            return View("~/Views/Fees/PaymentGateway/Success.cshtml");
        }

        var paymentId = 0;

        if (request.Status == OnlinePaymentRequestStatus.Verified)
        {
            ViewBag.Message = "Payment already verified successfully.";
            ViewBag.InvoiceNo = request.FeeInvoice?.InvoiceNo;
            ViewBag.Amount = request.Amount;
            ViewBag.TransactionId = bank_tran_id ?? tran_id;
            return View("~/Views/Fees/PaymentGateway/Success.cshtml");
        }

        if (!string.IsNullOrEmpty(val_id))
        {
            paymentId = await _gatewayService.ProcessIpnAsync(bank_tran_id, tran_id, val_id, status ?? "VALID", ct);
        }

        if (paymentId > 0)
        {
            ViewBag.Message = "Payment successful and verified!";
            ViewBag.InvoiceNo = request.FeeInvoice?.InvoiceNo;
            ViewBag.Amount = request.Amount;
            ViewBag.TransactionId = bank_tran_id ?? tran_id;
            ViewBag.PaymentId = paymentId;
        }
        else
        {
            ViewBag.Message = "Payment received but verification is pending. Admin will verify shortly.";
            ViewBag.InvoiceNo = request.FeeInvoice?.InvoiceNo;
            ViewBag.Amount = request.Amount;
            ViewBag.TransactionId = bank_tran_id ?? tran_id;
        }

        return View("~/Views/Fees/PaymentGateway/Success.cshtml");
    }

    [HttpGet("Receipt/{paymentId:int}")]
    public async Task<IActionResult> Receipt(int paymentId, CancellationToken ct)
    {
        var data = await _receiptService.GetReceiptDataAsync(paymentId, ct);
        if (data is null) return NotFound();
        return View("~/Views/Fee/FeePayment/Receipt.cshtml", data);
    }

    [HttpGet("DownloadReceipt/{paymentId:int}")]
    public async Task<IActionResult> DownloadReceipt(int paymentId, CancellationToken ct)
    {
        var pdf = await _receiptService.GenerateReceiptPdfAsync(paymentId, ct);
        if (pdf.Length == 0) return NotFound();
        return File(pdf, "application/pdf", $"receipt-{paymentId:D6}.pdf");
    }

    [HttpGet("Cancel")]
    public IActionResult Cancel()
    {
        SetPortalNavigation();
        ViewBag.Message = "Payment was cancelled.";
        return View("~/Views/Fees/PaymentGateway/Cancel.cshtml");
    }

    [HttpGet("Fail")]
    public IActionResult Fail()
    {
        SetPortalNavigation();
        ViewBag.Message = "Payment failed. Please try again.";
        return View("~/Views/Fees/PaymentGateway/Fail.cshtml");
    }

    [HttpPost("Ipn")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Ipn(CancellationToken ct)
    {
        var bankTranId = Request.Form["bank_tran_id"].FirstOrDefault();
        var tranId = Request.Form["tran_id"].FirstOrDefault();
        var valId = Request.Form["val_id"].FirstOrDefault();
        var status = Request.Form["status"].FirstOrDefault();

        _logger.LogInformation("SSLCommerz IPN received: tran_id={TranId}, status={Status}, bank_tran_id={BankTranId}", tranId, status, bankTranId);

        if (string.IsNullOrEmpty(tranId))
        {
            _logger.LogWarning("IPN missing tran_id");
            return Ok("FAILED");
        }

        var paymentId = await _gatewayService.ProcessIpnAsync(bankTranId, tranId, valId, status ?? "VALID", ct);
        return Ok(paymentId > 0 ? "SUCCESS" : "FAILED");
    }
}
