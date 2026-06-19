using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Helpers.Reports;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeLedgerController : Controller
{
    private readonly IFeeLedgerService _service;
    private readonly IFeeSecurityService _security;
    private readonly IPdfGenerator _pdfGenerator;
    public FeeLedgerController(IFeeLedgerService service, IFeeSecurityService security, IPdfGenerator pdfGenerator) { _service = service; _security = security; _pdfGenerator = pdfGenerator; }

    [RequirePermission("FeeLedger.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("FeeLedger.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null, int? studentId = null, int? transactionType = null)
    {
        if (_security.HasStudentRole(User)) studentId = _security.GetCurrentStudentId(User);
        var result = await _service.GetPagedAsync(page, size, search, studentId, transactionType);
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    [RequirePermission("FeeLedger.Read")]
    public async Task<IActionResult> ExportExcel(string? search = null, int? studentId = null, int? transactionType = null)
    {
        if (_security.HasStudentRole(User)) studentId = _security.GetCurrentStudentId(User);
        var result = await _service.GetPagedAsync(1, 100000, search, studentId, transactionType);
        var bytes = FeeListExporter.ExportToExcel(result.Items.ToList(), "Fee Ledger");
        return File(bytes, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "fee-ledger.xlsx");
    }

    [HttpGet]
    [RequirePermission("FeeLedger.Read")]
    public async Task<IActionResult> ExportPdf(string? search = null, int? studentId = null, int? transactionType = null)
    {
        if (_security.HasStudentRole(User)) studentId = _security.GetCurrentStudentId(User);
        var result = await _service.GetPagedAsync(1, 100000, search, studentId, transactionType);
        var html = FeeListExporter.BuildExportHtml(result.Items.ToList(), "Fee Ledger");
        var bytes = _pdfGenerator.GenerateFromHtml(html);
        return File(bytes, "application/pdf", "fee-ledger.pdf");
    }
}
