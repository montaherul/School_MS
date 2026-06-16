using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Services.Interfaces.Academic;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Helpers.Reports;
using SchoolManagementSystem.Helpers.Pdf;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class AcademicCalendarController : Controller
{
    private readonly IAcademicCalendarService _service;
    private readonly IUnitOfWork _uow;
    private readonly ICalendarDashboardService _dashboardService;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly SchoolDbContext _db;

    public AcademicCalendarController(
        IAcademicCalendarService service,
        IUnitOfWork uow,
        ICalendarDashboardService dashboardService,
        IPdfGenerator pdfGenerator,
        SchoolDbContext db)
    {
        _service = service;
        _uow = uow;
        _dashboardService = dashboardService;
        _pdfGenerator = pdfGenerator;
        _db = db;
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

        var endDateOnly = DateOnly.FromDateTime(end);
        var startDateOnly = DateOnly.FromDateTime(start);

        // 1. Academic Calendar entries
        var academicDays = await _service.GetCalendarDaysAsync(start, end, ct);

        // 2. Website Events (published, within range)
        var webEvents = await _db.Events
            .Where(e => e.IsPublished && e.EventDate >= start && e.EventDate <= end)
            .ToListAsync(ct);

        // 3. Exam Schedules (within range)
        var examSchedules = await _db.ExamSchedules
            .Include(es => es.Exam)
            .Include(es => es.Subject)
            .Where(es => es.ExamDate >= startDateOnly && es.ExamDate <= endDateOnly)
            .ToListAsync(ct);

        // Merge all sources
        var results = new List<object>();

        foreach (var day in academicDays)
        {
            results.Add(new
            {
                id = $"ac-{day.Id}",
                title = day.Title ?? "",
                description = day.Description ?? "",
                date = day.Date.ToString("yyyy-MM-dd"),
                dayName = day.Date.DayOfWeek.ToString(),
                isHoliday = day.IsHoliday,
                isWorkingDay = day.IsWorkingDay,
                isExamDay = day.IsExamDay,
                isEventDay = day.IsEventDay,
                isWebsiteEvent = false,
                remarks = day.Remarks ?? "",
                holidayType = day.HolidayType,
                venue = (string?)null,
                source = "academic",
                sourceId = day.Id
            });
        }

        foreach (var ev in webEvents)
        {
            results.Add(new
            {
                id = $"web-{ev.Id}",
                title = ev.Title ?? "",
                description = ev.Description ?? "",
                date = ev.EventDate.ToString("yyyy-MM-dd"),
                dayName = ev.EventDate.DayOfWeek.ToString(),
                isHoliday = false,
                isWorkingDay = false,
                isExamDay = false,
                isEventDay = true,
                isWebsiteEvent = true,
                remarks = "",
                holidayType = (string?)null,
                venue = ev.EventLocation,
                source = "website_event",
                sourceId = ev.Id
            });
        }

        foreach (var es in examSchedules)
        {
            var examName = es.Exam?.Name ?? "Exam";
            var subName = es.Subject?.Name ?? "Subject";
            var timeStr = $"{es.StartsAt:hh\\:mm}–{es.EndsAt:hh\\:mm}";
            results.Add(new
            {
                id = $"exam-{es.Id}",
                title = $"{subName} - {examName}",
                description = es.Instructions ?? "",
                date = es.ExamDate.ToString("yyyy-MM-dd"),
                dayName = es.ExamDate.DayOfWeek.ToString(),
                isHoliday = false,
                isWorkingDay = false,
                isExamDay = true,
                isEventDay = false,
                isWebsiteEvent = false,
                remarks = timeStr,
                holidayType = (string?)null,
                venue = string.IsNullOrEmpty(es.RoomNo) ? null : $"Room: {es.RoomNo}",
                source = "exam_schedule",
                sourceId = es.Id
            });
        }

        return Json(results.OrderBy(r => ((dynamic)r).date).ToList());
    }

    [HttpGet]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> GetWeekData(DateTime date, CancellationToken ct = default)
    {
        var weekStart = date.Date.AddDays(-(int)date.DayOfWeek);
        var weekEnd = weekStart.AddDays(6);
        var start = new DateTime(weekStart.Year, weekStart.Month, weekStart.Day);
        var end = new DateTime(weekEnd.Year, weekEnd.Month, weekEnd.Day);
        var startDateOnly = DateOnly.FromDateTime(start);
        var endDateOnly = DateOnly.FromDateTime(end);

        // 1. Academic Calendar entries
        var academicDays = await _service.GetCalendarDaysAsync(start, end, ct);

        // 2. Website Events
        var webEvents = await _db.Events
            .Where(e => e.IsPublished && e.EventDate >= start && e.EventDate <= end)
            .ToListAsync(ct);

        // 3. Exam Schedules
        var examSchedules = await _db.ExamSchedules
            .Include(es => es.Exam)
            .Include(es => es.Subject)
            .Where(es => es.ExamDate >= startDateOnly && es.ExamDate <= endDateOnly)
            .ToListAsync(ct);

        // Build a per-date lookup
        var dayMap = new Dictionary<string, List<object>>();
        for (var d = weekStart; d <= weekEnd; d = d.AddDays(1))
        {
            dayMap[d.ToString("yyyy-MM-dd")] = new List<object>();
        }

        foreach (var day in academicDays)
        {
            var key = day.Date.ToString("yyyy-MM-dd");
            if (dayMap.ContainsKey(key))
            {
                dayMap[key].Add(new
                {
                    title = day.Title ?? "",
                    description = day.Description ?? "",
                    isHoliday = day.IsHoliday,
                    isWorkingDay = day.IsWorkingDay,
                    isExamDay = day.IsExamDay,
                    isEventDay = day.IsEventDay,
                    isWebsiteEvent = false,
                    holidayType = day.HolidayType,
                    venue = (string?)null,
                    source = "academic"
                });
            }
        }

        foreach (var ev in webEvents)
        {
            var key = ev.EventDate.ToString("yyyy-MM-dd");
            if (dayMap.ContainsKey(key))
            {
                dayMap[key].Add(new
                {
                    title = ev.Title ?? "",
                    description = ev.Description ?? "",
                    isHoliday = false,
                    isWorkingDay = false,
                    isExamDay = false,
                    isEventDay = true,
                    isWebsiteEvent = true,
                    holidayType = (string?)null,
                    venue = ev.EventLocation,
                    source = "website_event"
                });
            }
        }

        foreach (var es in examSchedules)
        {
            var key = es.ExamDate.ToString("yyyy-MM-dd");
            if (dayMap.ContainsKey(key))
            {
                var examName = es.Exam?.Name ?? "Exam";
                var subName = es.Subject?.Name ?? "Subject";
                dayMap[key].Add(new
                {
                    title = $"{subName} - {examName}",
                    description = es.Instructions ?? "",
                    isHoliday = false,
                    isWorkingDay = false,
                    isExamDay = true,
                    isEventDay = false,
                    isWebsiteEvent = false,
                    holidayType = (string?)null,
                    venue = string.IsNullOrEmpty(es.RoomNo) ? null : $"Room: {es.RoomNo}",
                    source = "exam_schedule"
                });
            }
        }

        var weekDays = dayMap.Select(kvp =>
        {
            var dt = DateTime.Parse(kvp.Key);
            var items = kvp.Value;
            // Determine aggregate flags for the day
            var hasHoliday = items.Any(i => (bool)((dynamic)i).isHoliday);
            var hasExam = items.Any(i => (bool)((dynamic)i).isExamDay);
            var hasEvent = items.Any(i => (bool)((dynamic)i).isEventDay || (bool)((dynamic)i).isWebsiteEvent);
            var isWorking = !hasHoliday && items.All(i => (bool)((dynamic)i).isWorkingDay || !(bool)((dynamic)i).isHoliday);

            var primaryItem = items.FirstOrDefault();
            return new
            {
                date = kvp.Key,
                dayName = dt.DayOfWeek.ToString(),
                dayNumber = dt.Day,
                title = primaryItem != null ? ((dynamic)primaryItem).title : (string?)null,
                description = primaryItem != null ? ((dynamic)primaryItem).description : (string?)null,
                isHoliday = hasHoliday,
                isWorkingDay = isWorking && !hasHoliday,
                isExamDay = hasExam,
                isEventDay = hasEvent,
                holidayType = (string?)null,
                items = items
            };
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
        var fromDateOnly = DateOnly.FromDateTime(from);
        var endDateOnly = DateOnly.FromDateTime(end);

        // 1. Academic Calendar entries
        var academicDays = await _service.GetCalendarDaysAsync(from, end, ct);

        // 2. Website Events
        var webEvents = await _db.Events
            .Where(e => e.IsPublished && e.EventDate >= from && e.EventDate <= end)
            .ToListAsync(ct);

        // 3. Exam Schedules
        var examSchedules = await _db.ExamSchedules
            .Include(es => es.Exam)
            .Include(es => es.Subject)
            .Where(es => es.ExamDate >= fromDateOnly && es.ExamDate <= endDateOnly)
            .ToListAsync(ct);

        var items = new List<object>();

        foreach (var d in academicDays)
        {
            // FIX 2: Skip working days in agenda event lists
            var isWorking = !d.IsHoliday && !d.IsExamDay && !d.IsEventDay;
            if (isWorking) continue;

            items.Add(new
            {
                date = d.Date.ToString("yyyy-MM-dd"),
                dayName = d.Date.DayOfWeek.ToString(),
                title = d.Title ?? "",
                description = d.Description ?? "",
                type = d.IsHoliday ? "holiday" : d.IsExamDay ? "exam" : "event",
                holidayType = d.HolidayType,
                venue = (string?)null,
                source = "academic"
            });
        }

        foreach (var ev in webEvents)
        {
            items.Add(new
            {
                date = ev.EventDate.ToString("yyyy-MM-dd"),
                dayName = ev.EventDate.DayOfWeek.ToString(),
                title = ev.Title ?? "",
                description = ev.Description ?? "",
                type = "event",
                holidayType = (string?)null,
                venue = ev.EventLocation,
                source = "website_event"
            });
        }

        foreach (var es in examSchedules)
        {
            var examName = es.Exam?.Name ?? "Exam";
            var subName = es.Subject?.Name ?? "Subject";
            var timeStr = $"{es.StartsAt:hh\\:mm}–{es.EndsAt:hh\\:mm}";
            items.Add(new
            {
                date = es.ExamDate.ToString("yyyy-MM-dd"),
                dayName = es.ExamDate.DayOfWeek.ToString(),
                title = $"{subName} - {examName} ({timeStr})",
                description = es.Instructions ?? "",
                type = "exam",
                holidayType = (string?)null,
                venue = string.IsNullOrEmpty(es.RoomNo) ? null : $"Room: {es.RoomNo}",
                source = "exam_schedule"
            });
        }

        var ordered = items
            .OrderBy(i => ((dynamic)i).date)
            .ThenBy(i => ((dynamic)i).title)
            .Take(count)
            .ToList();

        return Json(ordered);
    }

    [HttpGet]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> GetYearData(int year, CancellationToken ct = default)
    {
        var start = new DateTime(year, 1, 1);
        var end = new DateTime(year, 12, 31);
        var startDateOnly = DateOnly.FromDateTime(start);
        var endDateOnly = DateOnly.FromDateTime(end);

        // 1. Academic Calendar entries
        var days = await _service.GetCalendarDaysAsync(start, end, ct);

        // 2. Website Events in this year
        var webEvents = await _db.Events
            .Where(e => e.IsPublished && e.EventDate >= start && e.EventDate <= end)
            .ToListAsync(ct);

        // 3. Exam Schedules in this year
        var examSchedules = await _db.ExamSchedules
            .Where(es => es.ExamDate >= startDateOnly && es.ExamDate <= endDateOnly)
            .ToListAsync(ct);

        // FIX 1: Mutually exclusive counts from academic calendar only.
        // Exam schedules are already reflected in IsExamDay via SyncExamDaysAsync.
        // Priority: Holiday > Exam > Event > Working
        var monthly = Enumerable.Range(1, 12).Select(m =>
        {
            return new
            {
                month = m,
                monthName = new DateTime(year, m, 1).ToString("MMMM"),
                totalDays = DateTime.DaysInMonth(year, m),
                holidays = days.Count(d => d.Date.Month == m && d.IsHoliday),
                examDays = days.Count(d => d.Date.Month == m && d.IsExamDay && !d.IsHoliday),
                events = days.Count(d => d.Date.Month == m && d.IsEventDay && !d.IsHoliday && !d.IsExamDay),
                websiteEvents = webEvents.Count(ev => ev.EventDate.Month == m),
                workingDays = days.Count(d => d.Date.Month == m && d.IsWorkingDay && !d.IsHoliday && !d.IsExamDay && !d.IsEventDay)
            };
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
