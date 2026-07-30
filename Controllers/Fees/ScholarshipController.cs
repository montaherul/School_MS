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
public class ScholarshipController : Controller
{
    private const string ViewPath = "~/Views/Fee/Scholarship";
    private readonly IScholarshipService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    public ScholarshipController(IScholarshipService service, IFeeSecurityService security, IPdfGenerator pdfGenerator) { _service = service; _security = security; _pdfGenerator = pdfGenerator; }

    [RequirePermission("Scholarships.Read")]
    public IActionResult Index() { return View($"{ViewPath}/Index.cshtml"); }

    [HttpGet]
    [RequirePermission("Scholarships.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("Scholarships.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("Scholarships.Read")]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, pageSize, search);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "Scholarships.Update" : "Scholarships.Create"))
            return Forbid();

        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View($"{ViewPath}/CreateEdit.cshtml", new ScholarshipViewModel
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                DiscountType = dto.DiscountType,
                Value = dto.Value,
                SchoolClassId = dto.SchoolClassId,
                FeeCategoryId = dto.FeeCategoryId,
                FeeTypeId = dto.FeeTypeId,
                IsActive = dto.IsActive,
                ValidFrom = dto.ValidFrom,
                ValidTo = dto.ValidTo
            });
        }
        return View($"{ViewPath}/CreateEdit.cshtml", new ScholarshipViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(ScholarshipViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "Scholarships.Update" : "Scholarships.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View($"{ViewPath}/CreateEdit.cshtml", vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Scholarship updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Scholarship created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(ScholarshipViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("Scholarships.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Details.cshtml", new ScholarshipViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            DiscountType = dto.DiscountType,
            Value = dto.Value,
            SchoolClassId = dto.SchoolClassId,
            FeeCategoryId = dto.FeeCategoryId,
            FeeTypeId = dto.FeeTypeId,
            IsActive = dto.IsActive,
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo
        });
    }

    [HttpGet]
    [RequirePermission("Scholarships.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View($"{ViewPath}/Delete.cshtml", new ScholarshipViewModel
        {
            Id = dto.Id,
            Name = dto.Name,
            Description = dto.Description,
            DiscountType = dto.DiscountType,
            Value = dto.Value,
            SchoolClassId = dto.SchoolClassId,
            FeeCategoryId = dto.FeeCategoryId,
            FeeTypeId = dto.FeeTypeId,
            IsActive = dto.IsActive,
            ValidFrom = dto.ValidFrom,
            ValidTo = dto.ValidTo
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Scholarships.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Scholarship deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Scholarships.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Scholarship restored successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Scholarships.Read")]
    public async Task<IActionResult> ExportExcel(string? search = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search);
        var bytes = FeeListExporter.ExportToExcel(result.Items.ToList(), "Scholarships");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "scholarships.xlsx");
    }

    [HttpGet]
    [RequirePermission("Scholarships.Read")]
    public async Task<IActionResult> ExportPdf(string? search = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search);
        var html = FeeListExporter.BuildExportHtml(result.Items.ToList(), "Scholarships");
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", "scholarships.pdf");
    }
}
