using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Audit;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[AllowAnonymous]
[Route("SchoolPay/Checkout")]
public class SchoolPayCheckoutController : Controller
{
    private readonly IPaymentGatewayService _gatewayService;
    private readonly IOnlinePaymentService _onlinePaymentService;
    private readonly IAuditService _auditService;
    private readonly ILogger<SchoolPayCheckoutController> _logger;

    public SchoolPayCheckoutController(
        IPaymentGatewayService gatewayService,
        IOnlinePaymentService onlinePaymentService,
        IAuditService auditService,
        ILogger<SchoolPayCheckoutController> logger)
    {
        _gatewayService = gatewayService;
        _onlinePaymentService = onlinePaymentService;
        _auditService = auditService;
        _logger = logger;
    }

    [HttpGet("Index/{invoiceId:int}")]
    public async Task<IActionResult> Index(int invoiceId, int? studentId, CancellationToken ct)
    {
        var studentIdVal = studentId ?? 0;
        var baseUrl = $"{Request.Scheme}://{Request.Host}";

        var model = new SslCommerzCheckoutModel
        {
            InvoiceId = invoiceId,
            StudentId = studentIdVal,
            Amount = 0,
            ReturnUrl = $"{baseUrl}/SchoolPay/Checkout/Success",
            CancelUrl = $"{baseUrl}/SchoolPay/Checkout/Cancel"
        };

        return View("~/Views/SchoolPay/Checkout/Index.cshtml", model);
    }

    [HttpPost("Pay/{invoiceId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(int invoiceId, int studentId, string? returnUrl, string? cancelUrl, CancellationToken ct)
    {
        try
        {
            var baseUrl = $"{Request.Scheme}://{Request.Host}";
            returnUrl ??= $"{baseUrl}/SchoolPay/Checkout/Success";
            cancelUrl ??= $"{baseUrl}/SchoolPay/Checkout/Cancel";

            var request = await _onlinePaymentService.CreateGatewayPendingAsync(studentId, invoiceId, "SslCommerz", ct);
            var result = await _gatewayService.InitiatePaymentAsync(request.Id, null, ct);

            if (result == null || result.status != "SUCCESS")
            {
                _logger.LogError("SslCommerz init failed for invoice {InvoiceId}: {Error}", invoiceId, result?.failedreason);
                TempData["ErrorMessage"] = result?.failedreason ?? "Payment initialization failed.";
                return RedirectToAction(nameof(Fail));
            }

            await _auditService.LogAsync(null, "SchoolPay", "CheckoutInit",
                $"InvoiceId={invoiceId}, StudentId={studentId}, Provider=SslCommerz, Ref={result.tran_id}", ct);

            if (!string.IsNullOrEmpty(result.GatewayPageURL))
            {
                return Redirect(result.GatewayPageURL);
            }

            TempData["SuccessMessage"] = "Payment initialized successfully.";
            return RedirectToAction(nameof(Success));
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogError("Checkout failed for invoice {InvoiceId}: {Error}", invoiceId, ex.Message);
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Fail));
        }
    }

    [HttpGet("Success")]
    public IActionResult Success()
    {
        SetPortalNavigation();
        return View("~/Views/SchoolPay/Checkout/Success.cshtml");
    }

    [HttpGet("Cancel")]
    public IActionResult Cancel()
    {
        SetPortalNavigation();
        ViewBag.Message = "Payment was cancelled.";
        return View("~/Views/SchoolPay/Checkout/Cancel.cshtml");
    }

    [HttpGet("Fail")]
    public IActionResult Fail()
    {
        SetPortalNavigation();
        ViewBag.Message = TempData["ErrorMessage"] as string ?? "Payment failed. Please try again.";
        return View("~/Views/SchoolPay/Checkout/Fail.cshtml");
    }

    public class SslCommerzCheckoutModel
    {
        public int InvoiceId { get; set; }
        public int StudentId { get; set; }
        public decimal Amount { get; set; }
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
    }

    private void SetPortalNavigation()
    {
        var isAuthenticated = User?.Identity?.IsAuthenticated ?? false;
        if (!isAuthenticated)
        {
            ViewBag.PortalHome = "/";
            ViewBag.PortalLabel = "Home";
            return;
        }
        var roles = User.FindAll(ClaimTypes.Role).Select(r => r.Value).ToList();
        if (roles.Contains("Student"))
        {
            ViewBag.PortalHome = "/Student/Portal/Dashboard";
            ViewBag.PortalLabel = "Student Portal";
        }
        else if (roles.Contains("Guardian"))
        {
            ViewBag.PortalHome = "/Guardian/Portal/Dashboard";
            ViewBag.PortalLabel = "Guardian Portal";
        }
        else
        {
            ViewBag.PortalHome = "/Dashboard/Index";
            ViewBag.PortalLabel = "Dashboard";
        }
    }
}
