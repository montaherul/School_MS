using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.SchoolPay;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.SchoolPay;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.SchoolPay;

[Authorize]
[Route("SchoolPay/Provider")]
public class ProviderManagementController : Controller
{
    private readonly IProviderManagementService _providerManagement;
    private readonly ILogger<ProviderManagementController> _logger;

    public ProviderManagementController(
        IProviderManagementService providerManagement,
        ILogger<ProviderManagementController> logger)
    {
        _providerManagement = providerManagement;
        _logger = logger;
    }

    [HttpGet("Index")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var providers = await _providerManagement.GetAllProvidersAsync(ct);
        return View("~/Views/SchoolPay/Provider/Index.cshtml", providers);
    }

    [HttpGet("Create")]
    public IActionResult Create()
    {
        return View("~/Views/SchoolPay/Provider/CreateEdit.cshtml", new SchoolPayProviderUpsertDto());
    }

    [HttpPost("Create")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(SchoolPayProviderUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View("~/Views/SchoolPay/Provider/CreateEdit.cshtml", dto);

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        var id = await _providerManagement.CreateProviderAsync(dto, user, ct);
        TempData["SuccessMessage"] = $"Provider '{dto.Name}' created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet("Edit/{id:int}")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var provider = await _providerManagement.GetProviderByIdAsync(id, ct);
        if (provider == null) return NotFound();

        var dto = new SchoolPayProviderUpsertDto
        {
            Code = provider.Code,
            Name = provider.Name,
            Description = provider.Description,
            LogoUrl = provider.LogoUrl,
            IsSandbox = true,
            Priority = provider.Priority
        };
        return View("~/Views/SchoolPay/Provider/CreateEdit.cshtml", dto);
    }

    [HttpPost("Edit/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, SchoolPayProviderUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
            return View("~/Views/SchoolPay/Provider/CreateEdit.cshtml", dto);

        var user = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        var success = await _providerManagement.UpdateProviderAsync(id, dto, user, ct);
        if (!success) return NotFound();

        TempData["SuccessMessage"] = "Provider updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("ToggleStatus/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleStatus(int id, bool isActive, CancellationToken ct)
    {
        var user = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _providerManagement.ToggleProviderStatusAsync(id, isActive, user, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("ToggleSandbox/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleSandbox(int id, bool isSandbox, CancellationToken ct)
    {
        await _providerManagement.ToggleSandboxModeAsync(id, isSandbox, ct);
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _providerManagement.DeleteProviderAsync(id, ct);
        TempData["SuccessMessage"] = "Provider deleted successfully.";
        return RedirectToAction(nameof(Index));
    }
}
