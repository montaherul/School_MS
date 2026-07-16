using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.Audit;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[AllowAnonymous]
[Route("SchoolPay/Checkout")]
public class SchoolPayCheckoutController : Controller
{
    private readonly ICheckoutService _checkoutService;
    private readonly IProviderManagementService _providerManagement;
    private readonly IFeeInvoiceService _feeInvoiceService;
    private readonly IOnlinePaymentService _onlinePaymentService;
    private readonly IAuditService _auditService;
    private readonly ILogger<SchoolPayCheckoutController> _logger;

    public SchoolPayCheckoutController(
        ICheckoutService checkoutService,
        IProviderManagementService providerManagement,
        IFeeInvoiceService feeInvoiceService,
        IOnlinePaymentService onlinePaymentService,
        IAuditService auditService,
        ILogger<SchoolPayCheckoutController> logger)
    {
        _checkoutService = checkoutService;
        _providerManagement = providerManagement;
        _feeInvoiceService = feeInvoiceService;
        _onlinePaymentService = onlinePaymentService;
        _auditService = auditService;
        _logger = logger;
    }

    [HttpGet("Index/{invoiceId:int}")]
    public async Task<IActionResult> Index(int invoiceId, int? studentId, CancellationToken ct)
    {
        var studentIdVal = studentId ?? 0;
        var invoice = await _feeInvoiceService.GetByIdAsync(invoiceId, ct);
        if (invoice == null) return NotFound("Invoice not found");

        var dueAmount = invoice.TotalAmount - invoice.PaidAmount;
        if (dueAmount <= 0)
        {
            TempData["InfoMessage"] = "This invoice is already paid.";
            return RedirectToAction(nameof(Success));
        }

        var providers = await _checkoutService.GetAvailableProvidersAsync(dueAmount, ct: ct);
        if (providers.Count == 0)
        {
            TempData["ErrorMessage"] = "No payment providers are currently available.";
            return View("~/Views/SchoolPay/Checkout/Index.cshtml", new SchoolPayCheckoutModel());
        }

        var paymentMethods = await _checkoutService.GetAvailablePaymentMethodsAsync(ct);

        var model = new SchoolPayCheckoutModel
        {
            InvoiceId = invoiceId,
            InvoiceNo = invoice.InvoiceNo,
            StudentId = studentIdVal,
            Amount = dueAmount,
            Providers = providers,
            PaymentMethods = paymentMethods
        };

        return View("~/Views/SchoolPay/Checkout/Index.cshtml", model);
    }

    [HttpPost("Pay/{invoiceId:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Pay(int invoiceId, int studentId, string providerCode, string? methodCode, string? returnUrl, string? cancelUrl, CancellationToken ct)
    {
        if (string.IsNullOrEmpty(providerCode))
        {
            TempData["ErrorMessage"] = "Please select a payment method.";
            return RedirectToAction(nameof(Index), new { invoiceId, studentId });
        }

        var baseUrl = $"{Request.Scheme}://{Request.Host}";
        returnUrl ??= $"{baseUrl}/SchoolPay/Checkout/Success";
        cancelUrl ??= $"{baseUrl}/SchoolPay/Checkout/Cancel";

        var result = await _checkoutService.InitiateDirectCheckoutAsync(
            invoiceId, studentId, providerCode, methodCode, returnUrl, cancelUrl, ct);

        if (!result.Success)
        {
            _logger.LogError("Checkout failed for invoice {InvoiceId}: {Error}", invoiceId, result.ErrorMessage);
            TempData["ErrorMessage"] = result.ErrorMessage ?? "Payment initialization failed. Please try again.";
            return RedirectToAction(nameof(Fail));
        }

        await _auditService.LogAsync(null, "SchoolPay", "CheckoutInit",
            $"InvoiceId={invoiceId}, StudentId={studentId}, Provider={providerCode}, Ref={result.TransactionReference}", ct);

        if (!string.IsNullOrEmpty(result.CheckoutUrl))
        {
            return Redirect(result.CheckoutUrl);
        }

        TempData["SuccessMessage"] = "Payment initialized successfully.";
        return RedirectToAction(nameof(Success));
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
