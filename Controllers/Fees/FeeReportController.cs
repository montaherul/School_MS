using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeReportController : Controller
{
    private readonly IFeeReportService _service;
    private readonly IFeeSecurityService _security;
    public FeeReportController(IFeeReportService service, IFeeSecurityService security) { _service = service; _security = security; }

    [RequirePermission("Fee.Report")]
    public IActionResult Index() => View();

    // ── Student Ledger ──────────────────────────────────────────────
    [RequirePermission("Fee.Report")]
    public IActionResult StudentLedgerView() => View();

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> StudentLedger(int studentId, int page = 1, int size = 50)
    {
        var result = await _service.GetStudentLedgerReportAsync(studentId, page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportStudentLedgerExcel(int studentId)
    {
        var result = await _service.GetStudentLedgerReportAsync(studentId, 1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Student Ledger"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"student-ledger-{studentId}.xlsx");
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportStudentLedgerPdf(int studentId)
    {
        var result = await _service.GetStudentLedgerReportAsync(studentId, 1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Student Ledger", ""), "application/pdf", $"student-ledger-{studentId}.pdf");
    }

    // ── Daily Collection ────────────────────────────────────────────
    [RequirePermission("Fee.Report")]
    public IActionResult DailyCollectionView() => View();

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> DailyCollection(DateOnly date, int page = 1, int size = 50)
    {
        if (date == default) date = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await _service.GetDailyCollectionReportAsync(date, page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportDailyCollectionExcel(DateOnly date)
    {
        if (date == default) date = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await _service.GetDailyCollectionReportAsync(date, 1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Daily Collection"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"daily-collection-{date:yyyyMMdd}.xlsx");
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportDailyCollectionPdf(DateOnly date)
    {
        if (date == default) date = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await _service.GetDailyCollectionReportAsync(date, 1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Daily Collection", ""), "application/pdf", $"daily-collection-{date:yyyyMMdd}.pdf");
    }

    // ── Monthly Collection ──────────────────────────────────────────
    [RequirePermission("Fee.Report")]
    public IActionResult MonthlyCollectionView() => View();

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> MonthlyCollection(int year, int page = 1, int size = 50)
    {
        if (year <= 0) year = DateTime.UtcNow.Year;
        var result = await _service.GetMonthlyCollectionReportAsync(year, page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportMonthlyCollectionExcel(int year)
    {
        if (year <= 0) year = DateTime.UtcNow.Year;
        var result = await _service.GetMonthlyCollectionReportAsync(year, 1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Monthly Collection"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"monthly-collection-{year}.xlsx");
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportMonthlyCollectionPdf(int year)
    {
        if (year <= 0) year = DateTime.UtcNow.Year;
        var result = await _service.GetMonthlyCollectionReportAsync(year, 1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Monthly Collection", ""), "application/pdf", $"monthly-collection-{year}.pdf");
    }

    // ── Due Report ──────────────────────────────────────────────────
    [RequirePermission("Fee.Report")]
    public IActionResult DueView() => View();

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> Due(int page = 1, int size = 50, int classId = 0)
    {
        var result = await _service.GetDueReportAsync(page, size, classId);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportDueExcel(int classId = 0)
    {
        var result = await _service.GetDueReportAsync(1, 10000, classId);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Due Report"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "due-report.xlsx");
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportDuePdf(int classId = 0)
    {
        var result = await _service.GetDueReportAsync(1, 10000, classId);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Due Report", ""), "application/pdf", "due-report.pdf");
    }

    // ── Discount Report ─────────────────────────────────────────────
    [RequirePermission("Fee.Report")]
    public IActionResult DiscountView() => View();

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> Discount(int page = 1, int size = 50)
    {
        var result = await _service.GetDiscountReportAsync(page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportDiscountExcel()
    {
        var result = await _service.GetDiscountReportAsync(1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Discount Report"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "discount-report.xlsx");
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportDiscountPdf()
    {
        var result = await _service.GetDiscountReportAsync(1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Discount Report", ""), "application/pdf", "discount-report.pdf");
    }

    // ── Waiver Report ───────────────────────────────────────────────
    [RequirePermission("Fee.Report")]
    public IActionResult WaiverView() => View();

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> Waiver(int page = 1, int size = 50)
    {
        var result = await _service.GetWaiverReportAsync(page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportWaiverExcel()
    {
        var result = await _service.GetWaiverReportAsync(1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Waiver Report"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "waiver-report.xlsx");
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportWaiverPdf()
    {
        var result = await _service.GetWaiverReportAsync(1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Waiver Report", ""), "application/pdf", "waiver-report.pdf");
    }

    // ── Refund Report ───────────────────────────────────────────────
    [RequirePermission("Fee.Report")]
    public IActionResult RefundView() => View();

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> Refund(int page = 1, int size = 50)
    {
        var result = await _service.GetRefundReportAsync(page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportRefundExcel()
    {
        var result = await _service.GetRefundReportAsync(1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Refund Report"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "refund-report.xlsx");
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportRefundPdf()
    {
        var result = await _service.GetRefundReportAsync(1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Refund Report", ""), "application/pdf", "refund-report.pdf");
    }

    // ── Class Collection Summary ────────────────────────────────────
    [RequirePermission("Fee.Report")]
    public IActionResult ClassSummaryView() => View();

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ClassSummary(int academicYearId = 0, int page = 1, int size = 50)
    {
        var result = await _service.GetClassCollectionSummaryAsync(academicYearId, page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportClassSummaryExcel(int academicYearId = 0)
    {
        var result = await _service.GetClassCollectionSummaryAsync(academicYearId, 1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Class Collection Summary"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "class-collection-summary.xlsx");
    }

    [RequirePermission("Fee.Report")]
    public async Task<IActionResult> ExportClassSummaryPdf(int academicYearId = 0)
    {
        var result = await _service.GetClassCollectionSummaryAsync(academicYearId, 1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Class Collection Summary", ""), "application/pdf", "class-collection-summary.pdf");
    }
}
