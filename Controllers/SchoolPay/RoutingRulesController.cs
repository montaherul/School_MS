using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/RoutingRules")]
public class RoutingRulesController : Controller
{
    private readonly IPaymentRoutingService _routingService;
    private readonly IProviderManagementService _providerService;

    public RoutingRulesController(
        IPaymentRoutingService routingService,
        IProviderManagementService providerService)
    {
        _routingService = routingService;
        _providerService = providerService;
    }

    [RequirePermission("SchoolPay.Manage")]
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var rules = await _routingService.GetAllRulesAsync(ct);
        return View(rules);
    }

    [RequirePermission("SchoolPay.Manage")]
    [HttpGet("Create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        ViewBag.Providers = await _providerService.GetAllProvidersAsync(ct);
        return View(new SchoolPayRouteRuleUpsertDto());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> Create(SchoolPayRouteRuleUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Providers = await _providerService.GetAllProvidersAsync(ct);
            return View(dto);
        }

        var createdBy = User.Identity?.Name ?? "System";
        await _routingService.CreateRuleAsync(dto, createdBy, ct);
        TempData["Success"] = "Route rule created successfully";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("SchoolPay.Manage")]
    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var rule = await _routingService.GetRuleByIdAsync(id, ct);
        if (rule == null) return NotFound();

        ViewBag.Providers = await _providerService.GetAllProvidersAsync(ct);

        var dto = new SchoolPayRouteRuleUpsertDto
        {
            PaymentProviderId = rule.PaymentProviderId,
            RuleName = rule.RuleName,
            Priority = rule.Priority,
            MinAmount = rule.MinAmount,
            MaxAmount = rule.MaxAmount,
            FeeType = rule.FeeType,
            ConditionExpression = rule.ConditionExpression,
            IsActive = rule.IsActive,
            DisplayOrder = rule.DisplayOrder
        };

        return View(dto);
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> Edit(int id, SchoolPayRouteRuleUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Providers = await _providerService.GetAllProvidersAsync(ct);
            return View(dto);
        }

        var updatedBy = User.Identity?.Name ?? "System";
        await _routingService.UpdateRuleAsync(id, dto, updatedBy, ct);
        TempData["Success"] = "Route rule updated successfully";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _routingService.DeleteRuleAsync(id, ct);
        TempData["Success"] = "Route rule deleted successfully";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Toggle/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("SchoolPay.Manage")]
    public async Task<IActionResult> Toggle(int id, bool isActive, CancellationToken ct)
    {
        await _routingService.ToggleRuleActiveAsync(id, isActive, ct);
        return RedirectToAction(nameof(Index));
    }
}
