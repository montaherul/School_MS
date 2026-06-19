using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Reports;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeInvoiceController : Controller
{
    private readonly IFeeInvoiceService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    public FeeInvoiceController(IFeeInvoiceService service, IFeeSecurityService security, IPdfGenerator pdfGenerator) { _service = service; _security = security; _pdfGenerator = pdfGenerator; }

    [RequirePermission("FeeInvoices.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("FeeInvoices.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeeInvoices.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeeInvoices.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, int? studentId = null, int? status = null)
    {
        if (_security.HasStudentRole(User)) studentId = _security.GetCurrentStudentId(User);
        var result = await _service.GetPagedAsync(page, size, search, studentId, status);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("FeeInvoices.Read")]
    public async Task<IActionResult> ExportExcel(string? search = null, int? studentId = null, int? status = null)
    {
        if (_security.HasStudentRole(User)) studentId = _security.GetCurrentStudentId(User);
        var result = await _service.GetPagedAsync(1, 100000, search, studentId, status);
        var bytes = FeeListExporter.ExportToExcel(result.Items.ToList(), "Fee Invoices");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "fee-invoices.xlsx");
    }

    [HttpGet]
    [RequirePermission("FeeInvoices.Read")]
    public async Task<IActionResult> ExportPdf(string? search = null, int? studentId = null, int? status = null)
    {
        if (_security.HasStudentRole(User)) studentId = _security.GetCurrentStudentId(User);
        var result = await _service.GetPagedAsync(1, 100000, search, studentId, status);
        var html = FeeListExporter.BuildExportHtml(result.Items.ToList(), "Fee Invoices");
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", "fee-invoices.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FeeInvoices.Update" : "FeeInvoices.Create"))
            return Forbid();

        if (id.HasValue && id > 0)
        {
            var entity = await _service.GetByIdAsync(id.Value);
            if (entity == null) return NotFound();
            if (!_security.IsStudentScope(User, entity.StudentId)) return Forbid();
            var vm = new FeeInvoiceViewModel
            {
                Id = entity.Id, InvoiceNo = entity.InvoiceNo, StudentId = entity.StudentId,
                AcademicYearId = entity.AcademicYearId, DueDate = entity.DueDate,
                TotalAmount = entity.TotalAmount, PaidAmount = entity.PaidAmount,
                DiscountAmount = entity.DiscountAmount, LateFee = entity.LateFee,
                Status = (int)entity.Status, Remarks = entity.Remarks
            };
            return View(vm);
        }
        return View(new FeeInvoiceViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeeInvoiceViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeeInvoices.Update" : "FeeInvoices.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        if (vm.IsEditMode)
        {
            var entity = new FeeInvoice
            {
                Id = vm.Id, InvoiceNo = vm.InvoiceNo, StudentId = vm.StudentId,
                AcademicYearId = vm.AcademicYearId, DueDate = vm.DueDate,
                TotalAmount = vm.TotalAmount, PaidAmount = vm.PaidAmount,
                DiscountAmount = vm.DiscountAmount, LateFee = vm.LateFee,
                Status = (Models.Enums.PaymentStatus)vm.Status, Remarks = vm.Remarks
            };
            await _service.UpdateAsync(entity, userId);
            TempData["SuccessMessage"] = "Invoice updated.";
        }
        else
        {
            var entity = new FeeInvoice
            {
                InvoiceNo = vm.InvoiceNo, StudentId = vm.StudentId,
                AcademicYearId = vm.AcademicYearId, DueDate = vm.DueDate,
                TotalAmount = vm.TotalAmount, PaidAmount = vm.PaidAmount,
                DiscountAmount = vm.DiscountAmount, LateFee = vm.LateFee,
                Status = (Models.Enums.PaymentStatus)vm.Status, Remarks = vm.Remarks
            };
            await _service.CreateAsync(entity, userId);
            TempData["SuccessMessage"] = "Invoice created.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FeeInvoiceViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FeeInvoices.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();
        if (!_security.IsStudentScope(User, entity.StudentId)) return Forbid();
        return View(new FeeInvoiceViewModel
        {
            Id = entity.Id, InvoiceNo = entity.InvoiceNo, StudentId = entity.StudentId,
            AcademicYearId = entity.AcademicYearId, DueDate = entity.DueDate,
            TotalAmount = entity.TotalAmount, PaidAmount = entity.PaidAmount,
            DiscountAmount = entity.DiscountAmount, LateFee = entity.LateFee,
            Status = (int)entity.Status, Remarks = entity.Remarks
        });
    }

    [HttpGet]
    [RequirePermission("FeeInvoices.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();
        if (!_security.IsStudentScope(User, entity.StudentId)) return Forbid();
        return View(new FeeInvoiceViewModel
        {
            Id = entity.Id, InvoiceNo = entity.InvoiceNo, StudentId = entity.StudentId,
            AcademicYearId = entity.AcademicYearId, DueDate = entity.DueDate,
            TotalAmount = entity.TotalAmount, PaidAmount = entity.PaidAmount,
            DiscountAmount = entity.DiscountAmount, LateFee = entity.LateFee,
            Status = (int)entity.Status, Remarks = entity.Remarks
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeInvoices.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Invoice deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeInvoices.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var entity = await _service.GetByIdAsync(id);
        if (entity == null) return NotFound();
        if (!_security.IsStudentScope(User, entity.StudentId)) return Forbid();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Invoice restored successfully.";
        return RedirectToAction(nameof(Index));
    }

}
