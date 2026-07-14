using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Services.Interfaces.Accounting;
using System.Security.Claims;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Reports;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeWaiverController : Controller
{
    private readonly IFeeWaiverService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IFinancePostingService _postingService;
    public FeeWaiverController(IFeeWaiverService service, IFeeSecurityService security, IPdfGenerator pdfGenerator, IFinancePostingService postingService) { _service = service; _security = security; _pdfGenerator = pdfGenerator; _postingService = postingService; }

    [RequirePermission("FeeWaivers.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("FeeWaivers.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeeWaivers.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeeWaivers.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, int? studentId = null)
    {
        if (_security.HasStudentRole(User)) studentId = _security.GetCurrentStudentId(User);
        var result = await _service.GetPagedAsync(page, size, search, studentId);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FeeWaivers.Update" : "FeeWaivers.Create"))
            return Forbid();
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            if (!_security.IsStudentScope(User, dto.StudentId)) return Forbid();
            return View(new FeeWaiverViewModel { Id = dto.Id, StudentId = dto.StudentId, FeeInvoiceId = dto.FeeInvoiceId, FeeCategoryId = dto.FeeCategoryId, FeeStructureId = dto.FeeStructureId, WaiverType = dto.WaiverType, WaiverValue = dto.WaiverValue, WaiverAmount = dto.WaiverAmount, Reason = dto.Reason, IsApproved = dto.IsApproved, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
        }
        return View(new FeeWaiverViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeeWaiverViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeeWaivers.Update" : "FeeWaivers.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Waiver updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Waiver created."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FeeWaiverViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FeeWaivers.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (!_security.IsStudentScope(User, dto.StudentId)) return Forbid();
        return View(new FeeWaiverViewModel { Id = dto.Id, StudentId = dto.StudentId, FeeInvoiceId = dto.FeeInvoiceId, FeeCategoryId = dto.FeeCategoryId, FeeStructureId = dto.FeeStructureId, WaiverType = dto.WaiverType, WaiverValue = dto.WaiverValue, WaiverAmount = dto.WaiverAmount, Reason = dto.Reason, IsApproved = dto.IsApproved, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
    }

    [HttpGet]
    [RequirePermission("FeeWaivers.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (!_security.IsStudentScope(User, dto.StudentId)) return Forbid();
        return View(new FeeWaiverViewModel { Id = dto.Id, StudentId = dto.StudentId, FeeInvoiceId = dto.FeeInvoiceId, FeeCategoryId = dto.FeeCategoryId, FeeStructureId = dto.FeeStructureId, WaiverType = dto.WaiverType, WaiverValue = dto.WaiverValue, WaiverAmount = dto.WaiverAmount, Reason = dto.Reason, IsApproved = dto.IsApproved, ValidFrom = dto.ValidFrom, ValidTo = dto.ValidTo });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeWaivers.Approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (!_security.IsStudentScope(User, dto.StudentId)) return Forbid();
        await _service.ApproveAsync(id, userId);
        await _postingService.PostFeeWaiverAsync(dto.StudentId, dto.WaiverAmount, dto.Reason ?? "Waiver approved", userId);
        TempData["SuccessMessage"] = "Waiver approved and posted to General Ledger.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeWaivers.Approve")]
    public async Task<IActionResult> Reject(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RejectAsync(id, userId);
        TempData["SuccessMessage"] = "Waiver rejected.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeWaivers.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Waiver deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeWaivers.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (!_security.IsStudentScope(User, dto.StudentId)) return Forbid();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Waiver restored successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("FeeWaivers.Read")]
    public async Task<IActionResult> ExportExcel(string? search = null, int? studentId = null)
    {
        if (_security.HasStudentRole(User)) studentId = _security.GetCurrentStudentId(User);
        var result = await _service.GetPagedAsync(1, 100000, search, studentId);
        var bytes = FeeListExporter.ExportToExcel(result.Items.ToList(), "Fee Waivers");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "fee-waivers.xlsx");
    }

    [HttpGet]
    [RequirePermission("FeeWaivers.Read")]
    public async Task<IActionResult> ExportPdf(string? search = null, int? studentId = null)
    {
        if (_security.HasStudentRole(User)) studentId = _security.GetCurrentStudentId(User);
        var result = await _service.GetPagedAsync(1, 100000, search, studentId);
        var html = FeeListExporter.BuildExportHtml(result.Items.ToList(), "Fee Waivers");
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", "fee-waivers.pdf");
    }
}
