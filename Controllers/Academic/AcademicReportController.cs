using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
[Route("Academic/[controller]")]
public class AcademicReportController : Controller
{
    private readonly IAcademicReportService _service;

    public AcademicReportController(IAcademicReportService service)
    {
        _service = service;
    }

    [RequirePermission("Reports.View")]
    public IActionResult Index()
    {
        return View("~/Views/Academic/AcademicReport/Index.cshtml", new AcademicReportViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Reports.View")]
    public async Task<IActionResult> Generate(AcademicReportFilterDto filter, CancellationToken ct)
    {
        var model = await _service.GetReportAsync(filter, ct);
        model.Filter = filter;
        return View("~/Views/Academic/AcademicReport/Index.cshtml", model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Reports.Export")]
    public async Task<IActionResult> ExportPdf(AcademicReportFilterDto filter, CancellationToken ct)
    {
        var pdf = await _service.ExportPdfAsync(filter, ct);
        return File(pdf, "application/pdf", $"AcademicReport_{filter.ReportType}_{DateTime.Today:yyyyMMdd}.pdf");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Reports.Export")]
    public async Task<IActionResult> ExportExcel(AcademicReportFilterDto filter, CancellationToken ct)
    {
        var xlsx = await _service.ExportExcelAsync(filter, ct);
        return File(xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"AcademicReport_{filter.ReportType}_{DateTime.Today:yyyyMMdd}.xlsx");
    }
}
