using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Reports;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class LateFeeRuleController : Controller
{
    private const string ViewPath = "~/Views/Fee/LateFeeRule";
    private readonly ILateFeeRuleService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IFeeInvoiceService _invoiceService;
    public LateFeeRuleController(ILateFeeRuleService service, IFeeSecurityService security, IPdfGenerator pdfGenerator, IFeeInvoiceService invoiceService) { _service = service; _security = security; _pdfGenerator = pdfGenerator; _invoiceService = invoiceService; }

    [RequirePermission("LateFeeRules.Read")]
    public IActionResult Index() { return View($"{ViewPath}/Index.cshtml"); }

    [HttpGet]
    [RequirePermission("LateFeeRules.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("LateFeeRules.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("LateFeeRules.Read")]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "LateFeeRules.Update" : "LateFeeRules.Create"))
            return Forbid();
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View($"{ViewPath}/CreateEdit.cshtml", new LateFeeRuleViewModel { Id = dto.Id, Name = dto.Name, GraceDays = dto.GraceDays, FeeType = dto.FeeType, FeeValue = dto.FeeValue, MaxFee = dto.MaxFee, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId, IsActive = dto.IsActive });
        }
        return View($"{ViewPath}/CreateEdit.cshtml", new LateFeeRuleViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(LateFeeRuleViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "LateFeeRules.Update" : "LateFeeRules.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View($"{ViewPath}/CreateEdit.cshtml", vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Rule updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Rule created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(LateFeeRuleViewModel vm) => CreateEdit(vm);

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("LateFeeRules.Update")]
    public async Task<IActionResult> ApplyLateFees()
    {
        var result = await _invoiceService.ApplyLateFeesAsync();
        TempData["SuccessMessage"] = $"Late fees applied: {result.InvoicesProcessed} invoice(s) processed, total ৳{result.TotalLateFeeApplied:N2}.";
        if (result.Errors.Count > 0)
            TempData["ErrorMessage"] = string.Join(" | ", result.Errors);
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("LateFeeRules.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Details.cshtml", new LateFeeRuleViewModel { Id = dto.Id, Name = dto.Name, GraceDays = dto.GraceDays, FeeType = dto.FeeType, FeeValue = dto.FeeValue, MaxFee = dto.MaxFee, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId, IsActive = dto.IsActive });
    }

    [HttpGet]
    [RequirePermission("LateFeeRules.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Delete.cshtml", new LateFeeRuleViewModel { Id = dto.Id, Name = dto.Name, GraceDays = dto.GraceDays, FeeType = dto.FeeType, FeeValue = dto.FeeValue, MaxFee = dto.MaxFee, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId, IsActive = dto.IsActive });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("LateFeeRules.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Rule deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleActive(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        var isActive = await _service.ToggleActiveAsync(id, userId);
        return Json(new { success = true, isActive });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("LateFeeRules.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Late fee rule restored successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("LateFeeRules.Read")]
    public async Task<IActionResult> ExportExcel(string? search = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search);
        var bytes = FeeListExporter.ExportToExcel(result.Items.ToList(), "Late Fee Rules");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "late-fee-rules.xlsx");
    }

    [HttpGet]
    [RequirePermission("LateFeeRules.Read")]
    public async Task<IActionResult> ExportPdf(string? search = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search);
        var html = FeeListExporter.BuildExportHtml(result.Items.ToList(), "Late Fee Rules");
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", "late-fee-rules.pdf");
    }

}
