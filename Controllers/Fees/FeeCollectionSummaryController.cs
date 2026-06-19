using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Reports;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeCollectionSummaryController : Controller
{
    private readonly IFeeCollectionSummaryService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    public FeeCollectionSummaryController(IFeeCollectionSummaryService service, IFeeSecurityService security, IPdfGenerator pdfGenerator) { _service = service; _security = security; _pdfGenerator = pdfGenerator; }

    [RequirePermission("FeeCollectionSummaries.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("FeeCollectionSummaries.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeeCollectionSummaries.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeeCollectionSummaries.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        var result = await _service.GetPagedAsync(page, size, search, fromDate, toDate);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FeeCollectionSummaries.Update" : "FeeCollectionSummaries.Create"))
            return Forbid();
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            return View(new FeeCollectionSummaryViewModel { Id = dto.Id, CollectionDate = dto.CollectionDate, TotalCollected = dto.TotalCollected, TotalDiscounted = dto.TotalDiscounted, TotalRefunded = dto.TotalRefunded, TotalTransactions = dto.TotalTransactions, PaymentMethod = dto.PaymentMethod, IsDailySummary = dto.IsDailySummary });
        }
        return View(new FeeCollectionSummaryViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeeCollectionSummaryViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeeCollectionSummaries.Update" : "FeeCollectionSummaries.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Summary updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Summary created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FeeCollectionSummaryViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FeeCollectionSummaries.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View(new FeeCollectionSummaryViewModel { Id = dto.Id, CollectionDate = dto.CollectionDate, TotalCollected = dto.TotalCollected, TotalDiscounted = dto.TotalDiscounted, TotalRefunded = dto.TotalRefunded, TotalTransactions = dto.TotalTransactions, PaymentMethod = dto.PaymentMethod, IsDailySummary = dto.IsDailySummary });
    }

    [HttpGet]
    [RequirePermission("FeeCollectionSummaries.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        return View(new FeeCollectionSummaryViewModel { Id = dto.Id, CollectionDate = dto.CollectionDate, TotalCollected = dto.TotalCollected, TotalDiscounted = dto.TotalDiscounted, TotalRefunded = dto.TotalRefunded, TotalTransactions = dto.TotalTransactions, PaymentMethod = dto.PaymentMethod, IsDailySummary = dto.IsDailySummary });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeCollectionSummaries.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Summary deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("FeeCollectionSummaries.Read")]
    public async Task<IActionResult> ExportExcel(string? search = null, DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search, fromDate, toDate);
        var bytes = FeeListExporter.ExportToExcel(result.Items.ToList(), "Fee Collection Summaries");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "fee-collection-summaries.xlsx");
    }

    [HttpGet]
    [RequirePermission("FeeCollectionSummaries.Read")]
    public async Task<IActionResult> ExportPdf(string? search = null, DateOnly? fromDate = null, DateOnly? toDate = null)
    {
        var result = await _service.GetPagedAsync(1, 100000, search, fromDate, toDate);
        var html = FeeListExporter.BuildExportHtml(result.Items.ToList(), "Fee Collection Summaries");
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", "fee-collection-summaries.pdf");
    }
}
