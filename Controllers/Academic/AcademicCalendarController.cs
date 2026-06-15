using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Helpers.Reports;
using SchoolManagementSystem.Helpers.Pdf;
using Microsoft.EntityFrameworkCore;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class AcademicCalendarController : Controller
{
    private readonly IAcademicCalendarService _service;
    private readonly IUnitOfWork _uow;
    private readonly ICalendarDashboardService _dashboardService;
    private readonly IPdfGenerator _pdfGenerator;

    public AcademicCalendarController(
        IAcademicCalendarService service,
        IUnitOfWork uow,
        ICalendarDashboardService dashboardService,
        IPdfGenerator pdfGenerator)
    {
        _service = service;
        _uow = uow;
        _dashboardService = dashboardService;
        _pdfGenerator = pdfGenerator;
    }

    [RequirePermission("Calendar.View")]
    public IActionResult Index()
    {
        return View();
    }

    [RequirePermission("Calendar.View")]
    public IActionResult WeekView(DateTime? date)
    {
        ViewBag.StartDate = date ?? DateTime.Today;
        return View();
    }

    [RequirePermission("Calendar.View")]
    public IActionResult Agenda()
    {
        return View();
    }

    [RequirePermission("Calendar.View")]
    public IActionResult YearView(int? year)
    {
        ViewBag.Year = year ?? DateTime.Today.Year;
        return View();
    }

    [HttpGet]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> GetEvents(DateTime start, DateTime end, CancellationToken ct = default)
    {
        if (start == default) start = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        if (end == default) end = start.AddMonths(1).AddDays(-1);

        var days = await _service.GetCalendarDaysAsync(start, end, ct);

        var events = days.Select(day => new
        {
            id = day.Id,
            title = day.Title,
            description = day.Description,
            date = day.Date.ToString("yyyy-MM-dd"),
            isHoliday = day.IsHoliday,
            isWorkingDay = day.IsWorkingDay,
            isExamDay = day.IsExamDay,
            isEventDay = day.IsEventDay,
            remarks = day.Remarks,
            holidayType = day.HolidayType
        }).ToList();

        return Json(events);
    }

    [HttpGet]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> GetWeekData(DateTime date, CancellationToken ct = default)
    {
        var weekStart = date.Date.AddDays(-(int)date.DayOfWeek);
        var weekEnd = weekStart.AddDays(6);
        var start = new DateTime(weekStart.Year, weekStart.Month, weekStart.Day);
        var end = new DateTime(weekEnd.Year, weekEnd.Month, weekEnd.Day);
        var days = await _service.GetCalendarDaysAsync(start, end, ct);

        var weekDays = days.Select(d => new
        {
            date = d.Date.ToString("yyyy-MM-dd"),
            dayName = d.Date.DayOfWeek.ToString(),
            dayNumber = d.Date.Day,
            title = d.Title,
            description = d.Description,
            isHoliday = d.IsHoliday,
            isWorkingDay = d.IsWorkingDay,
            isExamDay = d.IsExamDay,
            isEventDay = d.IsEventDay,
            holidayType = d.HolidayType
        }).ToList();

        return Json(new { weekStart = weekStart.ToString("yyyy-MM-dd"), weekEnd = weekEnd.ToString("yyyy-MM-dd"), days = weekDays });
    }

    [HttpGet]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> GetAgendaData(DateTime? start, int count = 20, CancellationToken ct = default)
    {
        var fromDate = start ?? DateTime.Today;
        var from = new DateTime(fromDate.Year, fromDate.Month, fromDate.Day);
        var end = from.AddDays(90);
        var days = await _service.GetCalendarDaysAsync(from, end, ct);

        var items = days
            .OrderBy(d => d.Date)
            .Take(count)
            .Select(d => new
            {
                date = d.Date.ToString("yyyy-MM-dd"),
                dayName = d.Date.DayOfWeek.ToString(),
                title = d.Title,
                description = d.Description,
                type = d.IsHoliday ? "holiday" : d.IsExamDay ? "exam" : d.IsEventDay ? "event" : "working",
                holidayType = d.HolidayType
            }).ToList();

        return Json(items);
    }

    [HttpGet]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> GetYearData(int year, CancellationToken ct = default)
    {
        var start = new DateTime(year, 1, 1);
        var end = new DateTime(year, 12, 31);
        var days = await _service.GetCalendarDaysAsync(start, end, ct);

        var monthly = Enumerable.Range(1, 12).Select(m => new
        {
            month = m,
            monthName = new DateTime(year, m, 1).ToString("MMMM"),
            totalDays = DateTime.DaysInMonth(year, m),
            holidays = days.Count(d => d.Date.Month == m && d.IsHoliday),
            examDays = days.Count(d => d.Date.Month == m && d.IsExamDay),
            events = days.Count(d => d.Date.Month == m && d.IsEventDay),
            workingDays = days.Count(d => d.Date.Month == m && d.IsWorkingDay)
        }).ToList();

        return Json(new { year, months = monthly });
    }

    // ── Dashboard Widget AJAX Endpoints ──

    [HttpGet]
    public async Task<IActionResult> WidgetUpcomingHolidays(int count = 5, CancellationToken ct = default)
    {
        var data = await _dashboardService.GetUpcomingHolidaysAsync(count, ct);
        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> WidgetUpcomingExams(int count = 5, CancellationToken ct = default)
    {
        var data = await _dashboardService.GetUpcomingExamsAsync(count, ct);
        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> WidgetUpcomingEvents(int count = 5, CancellationToken ct = default)
    {
        var data = await _dashboardService.GetUpcomingEventsAsync(count, ct);
        return Json(data);
    }

    [HttpGet]
    public async Task<IActionResult> WidgetMonthSummary(CancellationToken ct = default)
    {
        var data = await _dashboardService.GetCurrentMonthSummaryAsync(ct);
        return Json(data);
    }

    // ── CRUD (Permission-gated) ──

    [HttpGet]
    [RequirePermission("Calendar.Create")]
    public async Task<IActionResult> Create()
    {
        var activeYear = await _uow.Repository<AcademicYear>().Query()
            .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted);

        ViewBag.AcademicYears = await _uow.Repository<AcademicYear>().Query()
            .Where(y => !y.IsDeleted)
            .ToListAsync();

        return View(new AcademicCalendar
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            AcademicYearId = activeYear?.Id ?? 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Create")]
    public async Task<IActionResult> Create(AcademicCalendar entity, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AcademicYears = await _uow.Repository<AcademicYear>().Query()
                .Where(y => !y.IsDeleted)
                .ToListAsync(ct);
            return View(entity);
        }

        entity.CreatedBy = User.Identity?.Name ?? "system";
        entity.CreatedAt = DateTime.UtcNow;

        await _service.CreateAsync(entity, ct);
        TempData["Success"] = "Calendar entry created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Calendar.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var entity = await _service.GetByIdAsync(id, ct);
        if (entity == null) return NotFound();

        ViewBag.AcademicYears = await _uow.Repository<AcademicYear>().Query()
            .Where(y => !y.IsDeleted)
            .ToListAsync(ct);

        return View(entity);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Edit")]
    public async Task<IActionResult> Edit(AcademicCalendar entity, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AcademicYears = await _uow.Repository<AcademicYear>().Query()
                .Where(y => !y.IsDeleted)
                .ToListAsync(ct);
            return View(entity);
        }

        var existing = await _service.GetByIdAsync(entity.Id, ct);
        if (existing == null) return NotFound();

        existing.Date = entity.Date;
        existing.Title = entity.Title;
        existing.Description = entity.Description;
        existing.IsHoliday = entity.IsHoliday;
        existing.IsWorkingDay = entity.IsWorkingDay;
        existing.IsExamDay = entity.IsExamDay;
        existing.IsEventDay = entity.IsEventDay;
        existing.Remarks = entity.Remarks;
        existing.HolidayType = entity.HolidayType;
        existing.AcademicYearId = entity.AcademicYearId;
        existing.IsActive = entity.IsActive;
        existing.UpdatedBy = User.Identity?.Name ?? "system";
        existing.UpdatedAt = DateTime.UtcNow;

        await _service.UpdateAsync(existing, ct);
        TempData["Success"] = "Calendar entry updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, ct);
        return Json(new { success = true, message = "Calendar entry deleted successfully." });
    }

    // ── Exports ──

    [HttpGet]
    [RequirePermission("Calendar.Export")]
    public async Task<IActionResult> ExportPdf(int? year, CancellationToken ct = default)
    {
        var targetYear = year ?? DateTime.Today.Year;
        var start = new DateTime(targetYear, 1, 1);
        var end = new DateTime(targetYear, 12, 31);
        var days = await _service.GetCalendarDaysAsync(start, end, ct);

        var html = BuildCalendarPdfHtml(days, targetYear);
        var pdf = _pdfGenerator.GenerateFromHtml(html);
        return File(pdf, "application/pdf", $"AcademicCalendar_{targetYear}.pdf");
    }

    [HttpGet]
    [RequirePermission("Calendar.Export")]
    public async Task<IActionResult> ExportExcel(int? year, CancellationToken ct = default)
    {
        var targetYear = year ?? DateTime.Today.Year;
        var start = new DateTime(targetYear, 1, 1);
        var end = new DateTime(targetYear, 12, 31);
        var days = await _service.GetCalendarDaysAsync(start, end, ct);

        var rows = new List<string[]>
        {
            new[] { "Date", "Day", "Title", "Type", "IsHoliday", "IsExamDay", "IsEventDay", "Remarks" }
        };

        foreach (var d in days.OrderBy(x => x.Date))
        {
            rows.Add(new[]
            {
                d.Date.ToString("yyyy-MM-dd"),
                d.Date.DayOfWeek.ToString(),
                d.Title ?? "",
                d.HolidayType ?? "",
                d.IsHoliday ? "Yes" : "No",
                d.IsExamDay ? "Yes" : "No",
                d.IsEventDay ? "Yes" : "No",
                d.Remarks ?? ""
            });
        }

        var xlsx = SimpleExcelWriter.WriteWorkbook($"Calendar_{targetYear}", rows);
        return File(xlsx, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", $"AcademicCalendar_{targetYear}.xlsx");
    }

    [HttpGet]
    [RequirePermission("Calendar.Export")]
    public async Task<IActionResult> PrintView(int? year, CancellationToken ct = default)
    {
        var targetYear = year ?? DateTime.Today.Year;
        var start = new DateTime(targetYear, 1, 1);
        var end = new DateTime(targetYear, 12, 31);
        var days = await _service.GetCalendarDaysAsync(start, end, ct);

        var holidays = days.Where(d => d.IsHoliday).OrderBy(d => d.Date).ToList();
        var examDays = days.Where(d => d.IsExamDay).OrderBy(d => d.Date).ToList();
        var eventDays = days.Where(d => d.IsEventDay).OrderBy(d => d.Date).ToList();

        ViewBag.Year = targetYear;
        ViewBag.HolidayCount = holidays.Count;
        ViewBag.ExamDayCount = examDays.Count;
        ViewBag.EventDayCount = eventDays.Count;
        ViewBag.TotalDays = days.Count;
        ViewBag.WorkingDays = days.Count(d => d.IsWorkingDay);

        return View(days.OrderBy(d => d.Date).ToList());
    }

    private string BuildCalendarPdfHtml(List<AcademicCalendar> days, int year)
    {
        var holidays = days.Where(d => d.IsHoliday).OrderBy(d => d.Date).ToList();
        var examDays = days.Where(d => d.IsExamDay).OrderBy(d => d.Date).ToList();

        var html = $@"<!DOCTYPE html><html><head>
<meta charset='utf-8'/>
<style>
body {{ font-family: Arial, sans-serif; font-size: 10pt; color: #333; }}
h1 {{ text-align: center; color: #1B4D8C; font-size: 16pt; margin-bottom: 4px; }}
h2 {{ text-align: center; color: #666; font-size: 11pt; font-weight: normal; margin-top: 0; }}
table {{ width: 100%; border-collapse: collapse; margin-top: 12px; }}
th {{ background: #1B4D8C; color: #fff; padding: 6px 8px; text-align: left; font-size: 9pt; }}
td {{ padding: 4px 8px; border: 1px solid #ddd; font-size: 9pt; }}
tr:nth-child(even) {{ background: #f9fafb; }}
.badge-holiday {{ color: #b91c1c; font-weight: bold; }}
.badge-exam {{ color: #d97706; font-weight: bold; }}
.legend {{ margin-top: 16px; font-size: 9pt; }}
.legend span {{ display: inline-block; margin-right: 20px; }}
</style></head><body>
<h1>Academic Calendar {year}</h1>
<h2>School Management System</h2>
<table><thead><tr><th>Date</th><th>Day</th><th>Title</th><th>Type</th><th>Remarks</th></tr></thead><tbody>";

        foreach (var d in days.OrderBy(d => d.Date))
        {
            var type = d.IsHoliday ? "<span class='badge-holiday'>Holiday</span>" :
                       d.IsExamDay ? "<span class='badge-exam'>Exam</span>" : "Working";
            html += $"<tr><td>{d.Date:yyyy-MM-dd}</td><td>{d.Date.DayOfWeek}</td><td>{System.Net.WebUtility.HtmlEncode(d.Title ?? "")}</td><td>{type}</td><td>{System.Net.WebUtility.HtmlEncode(d.Remarks ?? "")}</td></tr>";
        }

        html += @"</tbody></table>
<div class='legend'>
<span style='color:#b91c1c;font-weight:bold'>■ Holiday</span>
<span style='color:#d97706;font-weight:bold'>■ Exam</span>
<span>■ Working Day</span>
</div>
</body></html>";

        return html;
    }
}
