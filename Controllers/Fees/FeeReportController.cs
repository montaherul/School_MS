using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeReportController : Controller
{
    private readonly IFeeReportService _service;
    private readonly IFeeSecurityService _security;
    private readonly IUnitOfWork _unitOfWork;
    public FeeReportController(IFeeReportService service, IFeeSecurityService security, IUnitOfWork unitOfWork) { _service = service; _security = security; _unitOfWork = unitOfWork; }

    [RequirePermission("FinanceReports.Read")]
    public IActionResult Index() => View();

    // ── Student Ledger ──────────────────────────────────────────────
    [RequirePermission("FinanceReports.Read")]
    public IActionResult StudentLedgerView() => View();

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> StudentLedger(int studentId, int page = 1, int size = 50)
    {
        var result = await _service.GetStudentLedgerReportAsync(studentId, page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportStudentLedgerExcel(int studentId)
    {
        var result = await _service.GetStudentLedgerReportAsync(studentId, 1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Student Ledger"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"student-ledger-{studentId}.xlsx");
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportStudentLedgerPdf(int studentId)
    {
        var result = await _service.GetStudentLedgerReportAsync(studentId, 1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Student Ledger", ""), "application/pdf", $"student-ledger-{studentId}.pdf");
    }

    // ── Daily Collection ────────────────────────────────────────────
    [RequirePermission("FinanceReports.Read")]
    public IActionResult DailyCollectionView() => View();

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> DailyCollection(DateOnly date, int page = 1, int size = 50)
    {
        if (date == default) date = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await _service.GetDailyCollectionReportAsync(date, page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportDailyCollectionExcel(DateOnly date)
    {
        if (date == default) date = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await _service.GetDailyCollectionReportAsync(date, 1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Daily Collection"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"daily-collection-{date:yyyyMMdd}.xlsx");
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportDailyCollectionPdf(DateOnly date)
    {
        if (date == default) date = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await _service.GetDailyCollectionReportAsync(date, 1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Daily Collection", ""), "application/pdf", $"daily-collection-{date:yyyyMMdd}.pdf");
    }

    // ── Monthly Collection ──────────────────────────────────────────
    [RequirePermission("FinanceReports.Read")]
    public IActionResult MonthlyCollectionView() => View();

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> MonthlyCollection(int year, int page = 1, int size = 50)
    {
        if (year <= 0) year = DateTime.UtcNow.Year;
        var result = await _service.GetMonthlyCollectionReportAsync(year, page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportMonthlyCollectionExcel(int year)
    {
        if (year <= 0) year = DateTime.UtcNow.Year;
        var result = await _service.GetMonthlyCollectionReportAsync(year, 1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Monthly Collection"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"monthly-collection-{year}.xlsx");
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportMonthlyCollectionPdf(int year)
    {
        if (year <= 0) year = DateTime.UtcNow.Year;
        var result = await _service.GetMonthlyCollectionReportAsync(year, 1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Monthly Collection", ""), "application/pdf", $"monthly-collection-{year}.pdf");
    }

    // ── Due Report ──────────────────────────────────────────────────
    [RequirePermission("FinanceReports.Read")]
    public IActionResult DueView() => View();

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> Due(int page = 1, int size = 50, int classId = 0)
    {
        var result = await _service.GetDueReportAsync(page, size, classId);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportDueExcel(int classId = 0)
    {
        var result = await _service.GetDueReportAsync(1, 10000, classId);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Due Report"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "due-report.xlsx");
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportDuePdf(int classId = 0)
    {
        var result = await _service.GetDueReportAsync(1, 10000, classId);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Due Report", ""), "application/pdf", "due-report.pdf");
    }

    // ── Discount Report ─────────────────────────────────────────────
    [RequirePermission("FinanceReports.Read")]
    public IActionResult DiscountView() => View();

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> Discount(int page = 1, int size = 50)
    {
        var result = await _service.GetDiscountReportAsync(page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportDiscountExcel()
    {
        var result = await _service.GetDiscountReportAsync(1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Discount Report"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "discount-report.xlsx");
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportDiscountPdf()
    {
        var result = await _service.GetDiscountReportAsync(1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Discount Report", ""), "application/pdf", "discount-report.pdf");
    }

    // ── Waiver Report ───────────────────────────────────────────────
    [RequirePermission("FinanceReports.Read")]
    public IActionResult WaiverView() => View();

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> Waiver(int page = 1, int size = 50)
    {
        var result = await _service.GetWaiverReportAsync(page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportWaiverExcel()
    {
        var result = await _service.GetWaiverReportAsync(1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Waiver Report"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "waiver-report.xlsx");
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportWaiverPdf()
    {
        var result = await _service.GetWaiverReportAsync(1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Waiver Report", ""), "application/pdf", "waiver-report.pdf");
    }

    // ── Refund Report ───────────────────────────────────────────────
    [RequirePermission("FinanceReports.Read")]
    public IActionResult RefundView() => View();

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> Refund(int page = 1, int size = 50)
    {
        var result = await _service.GetRefundReportAsync(page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportRefundExcel()
    {
        var result = await _service.GetRefundReportAsync(1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Refund Report"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "refund-report.xlsx");
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportRefundPdf()
    {
        var result = await _service.GetRefundReportAsync(1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Refund Report", ""), "application/pdf", "refund-report.pdf");
    }

    // ── Class Collection Summary ────────────────────────────────────
    [RequirePermission("FinanceReports.Read")]
    public IActionResult ClassSummaryView() => View();

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ClassSummary(int academicYearId = 0, int page = 1, int size = 50)
    {
        var result = await _service.GetClassCollectionSummaryAsync(academicYearId, page, size);
        return Json(new { data = result.Items, total_records = result.TotalItems, last_page = (int)Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportClassSummaryExcel(int academicYearId = 0)
    {
        var result = await _service.GetClassCollectionSummaryAsync(academicYearId, 1, 10000);
        return File(await _service.ExportToExcelAsync(result.Items.ToList(), "Class Collection Summary"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", "class-collection-summary.xlsx");
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportClassSummaryPdf(int academicYearId = 0)
    {
        var result = await _service.GetClassCollectionSummaryAsync(academicYearId, 1, 10000);
        return File(await _service.ExportToPdfAsync(result.Items.ToList(), "Class Collection Summary", ""), "application/pdf", "class-collection-summary.pdf");
    }

    // ── Cash Book ────────────────────────────────────────────────────
    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> CashBookView()
    {
        var years = await _unitOfWork.Repository<AcademicYear>().Query().AsNoTracking()
            .Where(y => !y.IsDeleted)
            .OrderByDescending(y => y.StartsOn)
            .ToListAsync();
        ViewBag.AcademicYears = years;
        return View();
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> CashBook(DateOnly fromDate, DateOnly toDate, int? academicYearId = null)
    {
        if (fromDate == default) fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        if (toDate == default) toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await _service.GetCashBookAsync(fromDate, toDate, academicYearId);
        return Json(result);
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportCashBookExcel(DateOnly fromDate, DateOnly toDate, int? academicYearId = null)
    {
        if (fromDate == default) fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        if (toDate == default) toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await _service.GetCashBookAsync(fromDate, toDate, academicYearId);
        return File(await _service.ExportToExcelAsync(result.Days.ToList(), "Cash Book"), "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"cash-book-{fromDate:yyyyMMdd}-{toDate:yyyyMMdd}.xlsx");
    }

    [RequirePermission("FinanceReports.Read")]
    public async Task<IActionResult> ExportCashBookPdf(DateOnly fromDate, DateOnly toDate, int? academicYearId = null)
    {
        if (fromDate == default) fromDate = DateOnly.FromDateTime(DateTime.UtcNow.AddDays(-30));
        if (toDate == default) toDate = DateOnly.FromDateTime(DateTime.UtcNow);
        var result = await _service.GetCashBookAsync(fromDate, toDate, academicYearId);
        return File(await _service.ExportToPdfAsync(result.Days.ToList(), "Cash Book", ""), "application/pdf", $"cash-book-{fromDate:yyyyMMdd}-{toDate:yyyyMMdd}.pdf");
    }
}
