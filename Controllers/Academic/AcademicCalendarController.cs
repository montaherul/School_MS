using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Helpers.Reports;
using SchoolManagementSystem.Helpers.Pdf;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class AcademicCalendarController : Controller
{
    private readonly IAcademicCalendarService _service;
    private readonly IAcademicYearService _yearService;
    private readonly ICalendarDashboardService _dashboardService;
    private readonly IPdfGenerator _pdfGenerator;

    public AcademicCalendarController(
        IAcademicCalendarService service,
        IAcademicYearService yearService,
        ICalendarDashboardService dashboardService,
        IPdfGenerator pdfGenerator)
    {
        _service = service;
        _yearService = yearService;
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

        var endDateOnly = DateOnly.FromDateTime(end);
        var startDateOnly = DateOnly.FromDateTime(start);

        var academicDays = await _service.GetCalendarDaysAsync(start, end, ct);
        var webEvents = await _service.GetPublishedEventsAsync(start, end, ct);
        var examSchedules = await _service.GetExamSchedulesAsync(startDateOnly, endDateOnly, ct);

        var results = new List<CalendarEventDto>();

        foreach (var day in academicDays)
        {
            results.Add(new CalendarEventDto
            {
                Id = $"ac-{day.Id}",
                Title = day.Title ?? "",
                Description = day.Description ?? "",
                Date = day.Date.ToString("yyyy-MM-dd"),
                DayName = day.Date.DayOfWeek.ToString(),
                IsHoliday = day.IsHoliday,
                IsWorkingDay = day.IsWorkingDay,
                IsExamDay = day.IsExamDay,
                IsEventDay = day.IsEventDay,
                IsWebsiteEvent = false,
                Remarks = day.Remarks ?? "",
                HolidayType = day.HolidayType,
                Venue = null,
                Source = "academic",
                SourceId = day.Id
            });
        }

        foreach (var ev in webEvents)
        {
            results.Add(new CalendarEventDto
            {
                Id = $"web-{ev.Id}",
                Title = ev.Title ?? "",
                Description = ev.Description ?? "",
                Date = ev.EventDate.ToString("yyyy-MM-dd"),
                DayName = ev.EventDate.DayOfWeek.ToString(),
                IsHoliday = false,
                IsWorkingDay = false,
                IsExamDay = false,
                IsEventDay = true,
                IsWebsiteEvent = true,
                Remarks = "",
                HolidayType = null,
                Venue = ev.EventLocation,
                Source = "website_event",
                SourceId = ev.Id
            });
        }

        foreach (var es in examSchedules)
        {
            var timeStr = $"{es.StartsAt:hh\\:mm}\u2013{es.EndsAt:hh\\:mm}";
            results.Add(new CalendarEventDto
            {
                Id = $"exam-{es.Id}",
                Title = $"{es.SubjectName} - {es.ExamName}",
                Description = es.Instructions ?? "",
                Date = es.ExamDate.ToString("yyyy-MM-dd"),
                DayName = es.ExamDate.DayOfWeek.ToString(),
                IsHoliday = false,
                IsWorkingDay = false,
                IsExamDay = true,
                IsEventDay = false,
                IsWebsiteEvent = false,
                Remarks = timeStr,
                HolidayType = null,
                Venue = string.IsNullOrEmpty(es.RoomNo) ? null : $"Room: {es.RoomNo}",
                Source = "exam_schedule",
                SourceId = es.Id
            });
        }

        return Json(results.OrderBy(r => r.Date).ToList());
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

        var academicDays = await _service.GetCalendarDaysAsync(start, end, ct);
        var webEvents = await _service.GetPublishedEventsAsync(start, end, ct);
        var examSchedules = await _service.GetExamSchedulesAsync(startDateOnly, endDateOnly, ct);

        var dayMap = new Dictionary<string, List<CalendarEventDto>>();
        for (var d = weekStart; d <= weekEnd; d = d.AddDays(1))
        {
            dayMap[d.ToString("yyyy-MM-dd")] = new List<CalendarEventDto>();
        }

        foreach (var day in academicDays)
        {
            var key = day.Date.ToString("yyyy-MM-dd");
            if (dayMap.ContainsKey(key))
            {
                dayMap[key].Add(new CalendarEventDto
                {
                    Title = day.Title ?? "",
                    Description = day.Description ?? "",
                    IsHoliday = day.IsHoliday,
                    IsWorkingDay = day.IsWorkingDay,
                    IsExamDay = day.IsExamDay,
                    IsEventDay = day.IsEventDay,
                    IsWebsiteEvent = false,
                    HolidayType = day.HolidayType,
                    Venue = null,
                    Source = "academic"
                });
            }
        }

        foreach (var ev in webEvents)
        {
            var key = ev.EventDate.ToString("yyyy-MM-dd");
            if (dayMap.ContainsKey(key))
            {
                dayMap[key].Add(new CalendarEventDto
                {
                    Title = ev.Title ?? "",
                    Description = ev.Description ?? "",
                    IsHoliday = false,
                    IsWorkingDay = false,
                    IsExamDay = false,
                    IsEventDay = true,
                    IsWebsiteEvent = true,
                    HolidayType = null,
                    Venue = ev.EventLocation,
                    Source = "website_event"
                });
            }
        }

        foreach (var es in examSchedules)
        {
            var key = es.ExamDate.ToString("yyyy-MM-dd");
            if (dayMap.ContainsKey(key))
            {
                dayMap[key].Add(new CalendarEventDto
                {
                    Title = $"{es.SubjectName} - {es.ExamName}",
                    Description = es.Instructions ?? "",
                    IsHoliday = false,
                    IsWorkingDay = false,
                    IsExamDay = true,
                    IsEventDay = false,
                    IsWebsiteEvent = false,
                    HolidayType = null,
                    Venue = string.IsNullOrEmpty(es.RoomNo) ? null : $"Room: {es.RoomNo}",
                    Source = "exam_schedule"
                });
            }
        }

        var weekDays = dayMap.Select(kvp =>
        {
            var dt = DateTime.Parse(kvp.Key);
            var items = kvp.Value;
            var hasHoliday = items.Any(i => i.IsHoliday);
            var hasExam = items.Any(i => i.IsExamDay);
            var hasEvent = items.Any(i => i.IsEventDay || i.IsWebsiteEvent);
            var isWorking = !hasHoliday && items.All(i => i.IsWorkingDay || !i.IsHoliday);

            var primaryItem = items.FirstOrDefault();
            return new
            {
                date = kvp.Key,
                dayName = dt.DayOfWeek.ToString(),
                dayNumber = dt.Day,
                title = primaryItem?.Title,
                description = primaryItem?.Description,
                isHoliday = hasHoliday,
                isWorkingDay = isWorking && !hasHoliday,
                isExamDay = hasExam,
                isEventDay = hasEvent,
                holidayType = (string?)null,
                items
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

        var academicDays = await _service.GetCalendarDaysAsync(from, end, ct);
        var webEvents = await _service.GetPublishedEventsAsync(from, end, ct);
        var examSchedules = await _service.GetExamSchedulesAsync(fromDateOnly, endDateOnly, ct);

        var items = new List<AgendaItemDto>();

        foreach (var d in academicDays)
        {
            var isWorking = !d.IsHoliday && !d.IsExamDay && !d.IsEventDay;
            if (isWorking) continue;

            items.Add(new AgendaItemDto
            {
                Date = d.Date.ToString("yyyy-MM-dd"),
                DayName = d.Date.DayOfWeek.ToString(),
                Title = d.Title ?? "",
                Description = d.Description ?? "",
                Type = d.IsHoliday ? "holiday" : d.IsExamDay ? "exam" : "event",
                HolidayType = d.HolidayType,
                Venue = null,
                Source = "academic"
            });
        }

        foreach (var ev in webEvents)
        {
            items.Add(new AgendaItemDto
            {
                Date = ev.EventDate.ToString("yyyy-MM-dd"),
                DayName = ev.EventDate.DayOfWeek.ToString(),
                Title = ev.Title ?? "",
                Description = ev.Description ?? "",
                Type = "event",
                HolidayType = null,
                Venue = ev.EventLocation,
                Source = "website_event"
            });
        }

        foreach (var es in examSchedules)
        {
            var timeStr = $"{es.StartsAt:hh\\:mm}\u2013{es.EndsAt:hh\\:mm}";
            items.Add(new AgendaItemDto
            {
                Date = es.ExamDate.ToString("yyyy-MM-dd"),
                DayName = es.ExamDate.DayOfWeek.ToString(),
                Title = $"{es.SubjectName} - {es.ExamName} ({timeStr})",
                Description = es.Instructions ?? "",
                Type = "exam",
                HolidayType = null,
                Venue = string.IsNullOrEmpty(es.RoomNo) ? null : $"Room: {es.RoomNo}",
                Source = "exam_schedule"
            });
        }

        var ordered = items
            .OrderBy(i => i.Date)
            .ThenBy(i => i.Title)
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

        var days = await _service.GetCalendarDaysAsync(start, end, ct);
        var webEvents = await _service.GetPublishedEventsAsync(start, end, ct);
        var examSchedules = await _service.GetExamSchedulesAsync(startDateOnly, endDateOnly, ct);

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

    [HttpGet]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> WidgetUpcomingHolidays(int count = 5, CancellationToken ct = default)
    {
        var data = await _dashboardService.GetUpcomingHolidaysAsync(count, ct);
        return Json(data);
    }

    [HttpGet]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> WidgetUpcomingExams(int count = 5, CancellationToken ct = default)
    {
        var data = await _dashboardService.GetUpcomingExamsAsync(count, ct);
        return Json(data);
    }

    [HttpGet]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> WidgetUpcomingEvents(int count = 5, CancellationToken ct = default)
    {
        var data = await _dashboardService.GetUpcomingEventsAsync(count, ct);
        return Json(data);
    }

    [HttpGet]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> WidgetMonthSummary(CancellationToken ct = default)
    {
        var data = await _dashboardService.GetCurrentMonthSummaryAsync(ct);
        return Json(data);
    }

    [HttpGet]
    [RequirePermission("Calendar.Create")]
    public async Task<IActionResult> Create(CancellationToken ct = default)
    {
        var activeYear = await _yearService.GetActiveYearAsync(ct);
        var allYears = await _yearService.GetAllYearsAsync(ct);

        ViewBag.AcademicYears = allYears;

        return View(new AcademicCalendarUpsertDto
        {
            Date = DateOnly.FromDateTime(DateTime.Today),
            AcademicYearId = activeYear?.Id ?? 0
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Create")]
    public async Task<IActionResult> Create(AcademicCalendarUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AcademicYears = await _yearService.GetAllYearsAsync(ct);
            return View(dto);
        }

        await _service.CreateAsync(dto, User.Identity?.Name ?? "system", ct);
        TempData["Success"] = "Calendar entry created successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Calendar.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var dto = await _service.GetForEditAsync(id, ct);
        if (dto == null) return NotFound();

        ViewBag.AcademicYears = await _yearService.GetAllYearsAsync(ct);

        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Edit")]
    public async Task<IActionResult> Edit(AcademicCalendarUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.AcademicYears = await _yearService.GetAllYearsAsync(ct);
            return View(dto);
        }

        await _service.UpdateAsync(dto, User.Identity?.Name ?? "system", ct);
        TempData["Success"] = "Calendar entry updated successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        await _service.DeleteAsync(id, User.Identity?.Name ?? "system", ct);
        return Json(new { success = true, message = "Calendar entry deleted successfully." });
    }

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

    private string BuildCalendarPdfHtml(List<AcademicCalendarDto> days, int year)
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
<span style='color:#b91c1c;font-weight:bold'>\u25a0 Holiday</span>
<span style='color:#d97706;font-weight:bold'>\u25a0 Exam</span>
<span>\u25a0 Working Day</span>
</div>
</body></html>";

        return html;
    }
}
