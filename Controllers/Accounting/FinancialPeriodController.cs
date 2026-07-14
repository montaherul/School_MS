using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.ViewModels.Accounting;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Accounting;

[Authorize]
[Route("Accounting/[controller]")]
public class FinancialPeriodController : Controller
{
    private readonly IFinancialPeriodService _service;

    public FinancialPeriodController(IFinancialPeriodService service) { _service = service; }

    [HttpGet("")]
    [RequirePermission("Accounting.View")]
    public IActionResult Index() => View("~/Views/Accounting/FinancialPeriod/Index.cshtml");

    [HttpGet("GetList")]
    [RequirePermission("Accounting.View")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        return Json(new { data = result.Items, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet("CreateEdit/{id?}")]
    [RequirePermission("Accounting.ClosePeriod")]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View("~/Views/Accounting/FinancialPeriod/CreateEdit.cshtml", new FinancialPeriodViewModel
            {
                Id = dto.Id, Name = dto.Name, StartDate = dto.StartDate,
                EndDate = dto.EndDate, IsActive = dto.IsActive
            });
        }
        return View("~/Views/Accounting/FinancialPeriod/CreateEdit.cshtml", new FinancialPeriodViewModel
        {
            StartDate = new DateTime(DateTime.Today.Year, 1, 1),
            EndDate = new DateTime(DateTime.Today.Year, 12, 31)
        });
    }

    [HttpPost("CreateEdit")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Accounting.ClosePeriod")]
    public async Task<IActionResult> CreateEdit(FinancialPeriodViewModel vm)
    {
        if (!ModelState.IsValid) return View("~/Views/Accounting/FinancialPeriod/CreateEdit.cshtml", vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode)
        { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Financial period updated."; }
        else
        { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Financial period created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Close")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Accounting.ClosePeriod")]
    public async Task<IActionResult> Close(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _service.ClosePeriodAsync(id, userId);
            TempData["SuccessMessage"] = "Financial period closed.";
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost("Delete/{id:int}")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Accounting.Post")]
    public async Task<IActionResult> Delete(int id)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _service.DeleteAsync(id, userId);
            TempData["SuccessMessage"] = "Financial period deleted.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = $"Cannot delete financial period: {ex.Message}";
        }
        return RedirectToAction(nameof(Index));
    }
}
