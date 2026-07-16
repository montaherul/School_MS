using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;
using SchoolManagementSystem.Filters;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Route("SchoolPay/PaymentMethods")]
[RequirePermission("SchoolPay.Manage")]
public class PaymentMethodController : Controller
{
    private readonly IPaymentMethodManagementService _methodService;
    private readonly IProviderManagementService _providerService;
    private readonly ILogger<PaymentMethodController> _logger;

    public PaymentMethodController(
        IPaymentMethodManagementService methodService,
        IProviderManagementService providerService,
        ILogger<PaymentMethodController> logger)
    {
        _methodService = methodService;
        _providerService = providerService;
        _logger = logger;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var methods = await _methodService.GetAllMethodsAsync(ct);
        return View("~/Views/SchoolPay/PaymentMethods/Index.cshtml", methods);
    }

    [HttpGet("Create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var providers = await _providerService.GetAllProvidersAsync(ct);
        ViewBag.Providers = providers;
        return View("~/Views/SchoolPay/PaymentMethods/CreateEdit.cshtml", new SchoolPayMethodUpsertDto());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SchoolPayMethodUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var providers = await _providerService.GetAllProvidersAsync(ct);
            ViewBag.Providers = providers;
            return View("~/Views/SchoolPay/PaymentMethods/CreateEdit.cshtml", dto);
        }

        var user = User.Identity?.Name ?? "System";
        var id = await _methodService.CreateMethodAsync(dto, user, ct);
        TempData["SuccessMessage"] = "Payment method created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var method = await _methodService.GetMethodByIdAsync(id, ct);
        if (method == null) return NotFound();

        var dto = new SchoolPayMethodUpsertDto
        {
            Code = method.Code,
            Name = method.Name,
            LogoUrl = method.LogoUrl,
            PaymentProviderId = method.PaymentProviderId,
            DisplayOrder = method.DisplayOrder,
            IsDefault = method.IsDefault,
            IsRecommended = method.IsRecommended,
            IsPopular = method.IsPopular,
            PopularityRank = method.PopularityRank,
            BackgroundColor = method.BackgroundColor,
            TextColor = method.TextColor,
            Icon = method.Icon,
            CssClass = method.CssClass
        };

        var providers = await _providerService.GetAllProvidersAsync(ct);
        ViewBag.Providers = providers;
        return View("~/Views/SchoolPay/PaymentMethods/CreateEdit.cshtml", dto);
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SchoolPayMethodUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var providers = await _providerService.GetAllProvidersAsync(ct);
            ViewBag.Providers = providers;
            return View("~/Views/SchoolPay/PaymentMethods/CreateEdit.cshtml", dto);
        }

        var user = User.Identity?.Name ?? "System";
        var updated = await _methodService.UpdateMethodAsync(id, dto, user, ct);
        if (!updated) return NotFound();

        TempData["SuccessMessage"] = "Payment method updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("ToggleActive/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id, bool isActive, CancellationToken ct)
    {
        var user = User.Identity?.Name ?? "System";
        await _methodService.ToggleMethodActiveAsync(id, isActive, user, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("UpdateOrder/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateOrder(int id, int displayOrder, CancellationToken ct)
    {
        await _methodService.UpdateMethodOrderAsync(id, displayOrder, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _methodService.DeleteMethodAsync(id, ct);
        TempData["SuccessMessage"] = "Payment method deleted.";
        return RedirectToAction(nameof(Index));
    }
}
