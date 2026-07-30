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
public class FeeDiscountController : Controller
{
    private const string ViewPath = "~/Views/Fee/FeeDiscount";
    private readonly IFeeDiscountService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    public FeeDiscountController(IFeeDiscountService service, IFeeSecurityService security, IPdfGenerator pdfGenerator) { _service = service; _security = security; _pdfGenerator = pdfGenerator; }

    [RequirePermission("FeeDiscounts.Read")]
    public IActionResult Index() { return View($"{ViewPath}/Index.cshtml"); }

    [HttpGet]
    [RequirePermission("FeeDiscounts.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeeDiscounts.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeeDiscounts.Read")]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FeeDiscounts.Update" : "FeeDiscounts.Create"))
            return Forbid();
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View($"{ViewPath}/CreateEdit.cshtml", new FeeDiscountViewModel { Id = dto.Id, Name = dto.Name, Description = dto.Description, DiscountType = dto.DiscountType, Value = dto.Value, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId, FeeStructureId = dto.FeeStructureId, IsActive = dto.IsActive, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
        }
        return View($"{ViewPath}/CreateEdit.cshtml", new FeeDiscountViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeeDiscountViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeeDiscounts.Update" : "FeeDiscounts.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View($"{ViewPath}/CreateEdit.cshtml", vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Discount updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Discount created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FeeDiscountViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FeeDiscounts.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Details.cshtml", new FeeDiscountViewModel { Id = dto.Id, Name = dto.Name, Description = dto.Description, DiscountType = dto.DiscountType, Value = dto.Value, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId, FeeStructureId = dto.FeeStructureId, IsActive = dto.IsActive, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
    }

    [HttpGet]
    [RequirePermission("FeeDiscounts.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Delete.cshtml", new FeeDiscountViewModel { Id = dto.Id, Name = dto.Name, Description = dto.Description, DiscountType = dto.DiscountType, Value = dto.Value, SchoolClassId = dto.SchoolClassId, FeeCategoryId = dto.FeeCategoryId, FeeStructureId = dto.FeeStructureId, IsActive = dto.IsActive, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeDiscounts.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Discount deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeDiscounts.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Discount restored successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("FeeDiscounts.Read")]
    public async Task<IActionResult> ExportExcel(string? search = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search);
        var bytes = FeeListExporter.ExportToExcel(result.Items.ToList(), "Fee Discounts");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "fee-discounts.xlsx");
    }

    [HttpGet]
    [RequirePermission("FeeDiscounts.Read")]
    public async Task<IActionResult> ExportPdf(string? search = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search);
        var html = FeeListExporter.BuildExportHtml(result.Items.ToList(), "Fee Discounts");
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", "fee-discounts.pdf");
    }

}
