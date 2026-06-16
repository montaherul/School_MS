using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Data;

namespace SchoolManagementSystem.Controllers.Website;

public class WebsiteAcademicCalendarController : Controller
{
    private readonly IAcademicCalendarService _calendarService;
    private readonly SchoolDbContext _db;

    public WebsiteAcademicCalendarController(IAcademicCalendarService calendarService, SchoolDbContext db)
    {
        _calendarService = calendarService;
        _db = db;
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

        var webEvents = await _db.Events.AsNoTracking()
            .Where(e => e.IsPublished && e.EventDate >= start && e.EventDate <= end)
            .ToListAsync(ct);

        var examSchedules = await _db.ExamSchedules.AsNoTracking()
            .Include(es => es.Exam)
            .Include(es => es.Subject)
            .Include(es => es.Class)
            .Where(es => es.ExamDate >= startDateOnly && es.ExamDate <= endDateOnly)
            .ToListAsync(ct);

        var result = new List<object>();

        foreach (var d in days.Where(d => d.IsActive))
        {
            result.Add(new
            {
                date = d.Date.ToString("yyyy-MM-dd"),
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
            });
        }

        foreach (var ev in webEvents)
        {
            result.Add(new
            {
                date = ev.EventDate.ToString("yyyy-MM-dd"),
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
            });
        }

        var examDayGroups = examSchedules
            .GroupBy(es => new { es.ExamId, es.ExamDate })
            .Select(g =>
            {
                var first = g.First();
                var exam = first.Exam;
                var subjects = g
                    .Select(es => es.Subject?.Name)
                    .Where(n => !string.IsNullOrEmpty(n));
                var classNames = g
                    .Select(es => es.Class?.Name)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct();
                return new
                {
                    date = g.Key.ExamDate.ToString("yyyy-MM-dd"),
                    title = exam?.Name ?? "Exam",
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
                };
            });

        foreach (var eg in examDayGroups)
        {
            result.Add(eg);
        }

        return Json(result.OrderBy(r => ((dynamic)r).date).ToList());
    }

    [HttpGet("/api/website/academic-calendar/upcoming")]
    public async Task<IActionResult> GetUpcomingEvents(int count = 10, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var future = today.AddDays(180);
        var todayDt = today.ToDateTime(TimeOnly.MinValue);
        var futureDt = future.ToDateTime(TimeOnly.MaxValue);

        var days = await _calendarService.GetCalendarDaysAsync(todayDt, futureDt, ct);

        var webEvents = await _db.Events.AsNoTracking()
            .Where(e => e.IsPublished && e.EventDate >= todayDt && e.EventDate <= futureDt)
            .ToListAsync(ct);

        var examSchedules = await _db.ExamSchedules.AsNoTracking()
            .Include(es => es.Exam)
            .Include(es => es.Subject)
            .Include(es => es.Class)
            .Include(es => es.StudentGroup)
            .Where(es => es.ExamDate >= today && es.ExamDate <= future)
            .ToListAsync(ct);

        var itemsWithSortKey = new List<(string sortDate, object item)>();

        foreach (var d in days.Where(d => d.IsActive && d.Date >= today && (d.IsHoliday || d.IsExamDay || d.IsEventDay)))
        {
            itemsWithSortKey.Add((d.Date.ToString("yyyy-MM-dd"), new
            {
                date = d.Date.ToString("dd MMM yyyy"),
                sortDate = d.Date.ToString("yyyy-MM-dd"),
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
            itemsWithSortKey.Add((ev.EventDate.ToString("yyyy-MM-dd"), new
            {
                date = ev.EventDate.ToString("dd MMM yyyy"),
                sortDate = ev.EventDate.ToString("yyyy-MM-dd"),
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

        var examGroups = examSchedules
            .GroupBy(es => es.ExamId)
            .Select(g =>
            {
                var first = g.First();
                var exam = first.Exam;
                var minDate = g.Min(es => es.ExamDate);
                var maxDate = g.Max(es => es.ExamDate);
                var classNames = g
                    .Select(es => es.Class?.Name)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .Distinct()
                    .ToList();
                var groupNames = g
                    .Where(es => es.StudentGroup != null)
                    .Select(es => es.StudentGroup!.Name)
                    .Distinct()
                    .ToList();
                var allSubjects = g
                    .Select(es => es.Subject?.Name)
                    .Where(n => !string.IsNullOrEmpty(n))
                    .ToList();

                return new
                {
                    sortDate = minDate.ToString("yyyy-MM-dd"),
                    date = minDate.ToString("dd MMM yyyy"),
                    startDate = minDate.ToString("dd MMM yyyy"),
                    endDate = maxDate.ToString("dd MMM yyyy"),
                    title = exam?.Name ?? "Exam",
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
                };
            });

        foreach (var eg in examGroups)
        {
            itemsWithSortKey.Add((((dynamic)eg).sortDate, eg));
        }

        var ordered = itemsWithSortKey
            .OrderBy(i => i.sortDate)
            .Take(count)
            .Select(i => i.item)
            .ToList();

        return Json(ordered);
    }
}
