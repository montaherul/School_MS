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
public class FeeTypeController : Controller
{
    private readonly IFeeTypeService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    public FeeTypeController(IFeeTypeService service, IFeeSecurityService security, IPdfGenerator pdfGenerator) { _service = service; _security = security; _pdfGenerator = pdfGenerator; }

    private const string ViewPath = "~/Views/Fee/FeeType";

    [RequirePermission("FeeTypes.Read")]
    public IActionResult Index() { return View($"{ViewPath}/Index.cshtml"); }

    [HttpGet]
    [RequirePermission("FeeTypes.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeeTypes.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeeTypes.Read")]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FeeTypes.Update" : "FeeTypes.Create"))
            return Forbid();

        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View($"{ViewPath}/CreateEdit.cshtml", new FeeTypeViewModel { Id = dto.Id, Name = dto.Name, Description = dto.Description, DisplayOrder = dto.DisplayOrder, IsActive = dto.IsActive });
        }
        return View($"{ViewPath}/CreateEdit.cshtml", new FeeTypeViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeeTypeViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeeTypes.Update" : "FeeTypes.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View($"{ViewPath}/CreateEdit.cshtml", vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Fee type updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Fee type created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FeeTypeViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FeeTypes.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Details.cshtml", new FeeTypeViewModel { Id = dto.Id, Name = dto.Name, Description = dto.Description, DisplayOrder = dto.DisplayOrder, IsActive = dto.IsActive });
    }

    [HttpGet]
    [RequirePermission("FeeTypes.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Delete.cshtml", new FeeTypeViewModel { Id = dto.Id, Name = dto.Name, Description = dto.Description, DisplayOrder = dto.DisplayOrder, IsActive = dto.IsActive });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeTypes.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Fee type deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeTypes.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Fee type restored successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("FeeTypes.Read")]
    public async Task<IActionResult> ExportExcel(string? search = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search);
        var bytes = FeeListExporter.ExportToExcel(result.Items.ToList(), "Fee Types");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "fee-types.xlsx");
    }

    [HttpGet]
    [RequirePermission("FeeTypes.Read")]
    public async Task<IActionResult> ExportPdf(string? search = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search);
        var html = FeeListExporter.BuildExportHtml(result.Items.ToList(), "Fee Types");
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", "fee-types.pdf");
    }

}
