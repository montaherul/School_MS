using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Controllers.Website;

public class WebsiteAcademicCalendarController : Controller
{
    private readonly IAcademicCalendarService _calendarService;

    public WebsiteAcademicCalendarController(IAcademicCalendarService calendarService)
    {
        _calendarService = calendarService;
    }

    [HttpGet("/academic-calendar")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("/api/website/academic-calendar")]
    public async Task<IActionResult> GetCalendarData(int year, int month, CancellationToken ct)
    {
        if (year < 2000 || year > 2100) year = DateTime.Today.Year;
        if (month < 1 || month > 12) month = DateTime.Today.Month;

        var start = new DateTime(year, month, 1).AddMonths(-2);
        var end = new DateTime(year, month, 1).AddMonths(3).AddDays(-1);
        var startDateOnly = DateOnly.FromDateTime(start);
        var endDateOnly = DateOnly.FromDateTime(end);

        var days = await _calendarService.GetCalendarDaysAsync(start, end, ct);
        var webEvents = await _calendarService.GetPublishedEventsAsync(start, end, ct);
        var examSchedules = await _calendarService.GetExamSchedulesAsync(startDateOnly, endDateOnly, ct);

        var result = new List<(string date, object item)>();

        foreach (var d in days.Where(d => d.IsActive))
        {
            var dateStr = d.Date.ToString("yyyy-MM-dd");
            result.Add((dateStr, new
            {
                date = dateStr,
                title = d.Title ?? "",
                description = d.Description ?? "",
                isHoliday = d.IsHoliday,
                isWorkingDay = d.IsWorkingDay,
                isExamDay = d.IsExamDay,
                isEventDay = d.IsEventDay,
                isWebsiteEvent = false,
                remarks = d.Remarks ?? "",
                holidayType = d.HolidayType,
                venue = (string?)null,
                source = "academic"
            }));
        }

        foreach (var ev in webEvents)
        {
            var dateStr = ev.EventDate.ToString("yyyy-MM-dd");
            result.Add((dateStr, new
            {
                date = dateStr,
                title = ev.Title ?? "",
                description = ev.Description ?? "",
                isHoliday = false,
                isWorkingDay = false,
                isExamDay = false,
                isEventDay = true,
                isWebsiteEvent = true,
                remarks = "",
                holidayType = (string?)null,
                venue = ev.EventLocation,
                source = "website_event"
            }));
        }

        foreach (var g in examSchedules.GroupBy(es => new { es.ExamId, es.ExamDate }))
        {
            var first = g.First();
            var subjects = g
                .Select(es => es.SubjectName)
                .Where(n => !string.IsNullOrEmpty(n));
            var classNames = g
                .Select(es => es.ClassName)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct();
            var dateStr = g.Key.ExamDate.ToString("yyyy-MM-dd");
            result.Add((dateStr, new
            {
                date = dateStr,
                title = first.ExamName ?? "Exam",
                description = $"{g.Count()} subject(s)",
                isHoliday = false,
                isWorkingDay = false,
                isExamDay = true,
                isEventDay = false,
                isWebsiteEvent = false,
                remarks = string.Join(", ", subjects),
                holidayType = (string?)null,
                venue = (string?)null,
                source = "exam",
                examId = g.Key.ExamId,
                totalSubjects = g.Count(),
                classes = classNames.ToList()
            }));
        }

        return Json(result.OrderBy(r => r.date).Select(r => r.item).ToList());
    }

    [HttpGet("/api/website/academic-calendar/upcoming")]
    public async Task<IActionResult> GetUpcomingEvents(int count = 10, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var future = today.AddDays(180);
        var todayDt = today.ToDateTime(TimeOnly.MinValue);
        var futureDt = future.ToDateTime(TimeOnly.MaxValue);

        var days = await _calendarService.GetCalendarDaysAsync(todayDt, futureDt, ct);
        var webEvents = await _calendarService.GetPublishedEventsAsync(todayDt, futureDt, ct);
        var examSchedules = await _calendarService.GetExamSchedulesAsync(today, future, ct);

        var itemsWithSortKey = new List<(string sortDate, object item)>();

        foreach (var d in days.Where(d => d.IsActive && d.Date >= today && (d.IsHoliday || d.IsExamDay || d.IsEventDay)))
        {
            var sortKey = d.Date.ToString("yyyy-MM-dd");
            itemsWithSortKey.Add((sortKey, new
            {
                date = d.Date.ToString("dd MMM yyyy"),
                sortDate = sortKey,
                title = d.Title ?? "",
                description = d.Description ?? "",
                isHoliday = d.IsHoliday,
                isExamDay = d.IsExamDay,
                isEventDay = d.IsEventDay,
                isWebsiteEvent = false,
                holidayType = d.HolidayType,
                remarks = d.Remarks ?? "",
                venue = (string?)null,
                source = "academic"
            }));
        }

        foreach (var ev in webEvents)
        {
            var sortKey = ev.EventDate.ToString("yyyy-MM-dd");
            itemsWithSortKey.Add((sortKey, new
            {
                date = ev.EventDate.ToString("dd MMM yyyy"),
                sortDate = sortKey,
                title = ev.Title ?? "",
                description = ev.Description ?? "",
                isHoliday = false,
                isExamDay = false,
                isEventDay = true,
                isWebsiteEvent = true,
                holidayType = (string?)null,
                remarks = "",
                venue = ev.EventLocation,
                source = "website_event"
            }));
        }

        foreach (var g in examSchedules.GroupBy(es => es.ExamId))
        {
            var first = g.First();
            var minDate = g.Min(es => es.ExamDate);
            var maxDate = g.Max(es => es.ExamDate);
            var classNames = g
                .Select(es => es.ClassName)
                .Where(n => !string.IsNullOrEmpty(n))
                .Distinct()
                .ToList();
            var groupNames = g
                .Where(es => !string.IsNullOrEmpty(es.StudentGroupName))
                .Select(es => es.StudentGroupName!)
                .Distinct()
                .ToList();
            var allSubjects = g
                .Select(es => es.SubjectName)
                .Where(n => !string.IsNullOrEmpty(n))
                .ToList();

            var sortKey = minDate.ToString("yyyy-MM-dd");
            itemsWithSortKey.Add((sortKey, new
            {
                sortDate = sortKey,
                date = minDate.ToString("dd MMM yyyy"),
                startDate = minDate.ToString("dd MMM yyyy"),
                endDate = maxDate.ToString("dd MMM yyyy"),
                title = first.ExamName ?? "Exam",
                description = $"{g.Count()} subjects across {(maxDate.DayNumber - minDate.DayNumber + 1)} day(s)",
                isHoliday = false,
                isExamDay = true,
                isEventDay = false,
                isWebsiteEvent = false,
                holidayType = (string?)null,
                remarks = $"Total Subjects: {g.Count()}",
                venue = (string?)null,
                source = "exam",
                examId = g.Key,
                totalSubjects = g.Count(),
                classes = classNames,
                groups = groupNames,
                subjects = allSubjects
            }));
        }

        return Json(itemsWithSortKey
            .OrderBy(i => i.sortDate)
            .Take(count)
            .Select(i => i.item)
            .ToList());
    }
}
