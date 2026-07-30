using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Reports;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FineRuleController : Controller
{
    private const string ViewPath = "~/Views/Fee/FineRule";
    private readonly IFineRuleService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    public FineRuleController(IFineRuleService service, IFeeSecurityService security, IPdfGenerator pdfGenerator) { _service = service; _security = security; _pdfGenerator = pdfGenerator; }

    [RequirePermission("FineRules.Read")]
    public IActionResult Index() { return View($"{ViewPath}/Index.cshtml"); }

    [HttpGet]
    [RequirePermission("FineRules.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FineRules.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FineRules.Read")]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FineRules.Update" : "FineRules.Create"))
            return Forbid();
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View($"{ViewPath}/CreateEdit.cshtml", new FineRuleViewModel { Id = dto.Id, Name = dto.Name, GraceDays = dto.GraceDays, FinePerDay = dto.FinePerDay });
        }
        return View($"{ViewPath}/CreateEdit.cshtml", new FineRuleViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FineRuleViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FineRules.Update" : "FineRules.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View($"{ViewPath}/CreateEdit.cshtml", vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Fine rule updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Fine rule created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FineRuleViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FineRules.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Details.cshtml", new FineRuleViewModel { Id = dto.Id, Name = dto.Name, GraceDays = dto.GraceDays, FinePerDay = dto.FinePerDay });
    }

    [HttpGet]
    [RequirePermission("FineRules.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Delete.cshtml", new FineRuleViewModel { Id = dto.Id, Name = dto.Name, GraceDays = dto.GraceDays, FinePerDay = dto.FinePerDay });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FineRules.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Fine rule deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FineRules.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Fine rule restored successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("FineRules.Read")]
    public async Task<IActionResult> ExportExcel(string? search = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search);
        var bytes = FeeListExporter.ExportToExcel(result.Items.ToList(), "Fine Rules");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "fine-rules.xlsx");
    }

    [HttpGet]
    [RequirePermission("FineRules.Read")]
    public async Task<IActionResult> ExportPdf(string? search = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search);
        var html = FeeListExporter.BuildExportHtml(result.Items.ToList(), "Fine Rules");
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", "fine-rules.pdf");
    }

}
