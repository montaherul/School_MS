using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
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

        var days = await _calendarService.GetCalendarDaysAsync(start, end, ct);

        var result = days.Where(d => d.IsActive).Select(d => new
        {
            date = d.Date.ToString("yyyy-MM-dd"),
            title = d.Title,
            description = d.Description,
            isHoliday = d.IsHoliday,
            isWorkingDay = d.IsWorkingDay,
            isExamDay = d.IsExamDay,
            isEventDay = d.IsEventDay,
            remarks = d.Remarks,
            holidayType = d.HolidayType
        }).ToList();

        return Json(result);
    }

    [HttpGet("/api/website/academic-calendar/upcoming")]
    public async Task<IActionResult> GetUpcomingEvents(int count = 10, CancellationToken ct = default)
    {
        var today = DateOnly.FromDateTime(DateTime.Today);
        var future = today.AddDays(180);

        var days = await _calendarService.GetCalendarDaysAsync(
            today.ToDateTime(TimeOnly.MinValue),
            future.ToDateTime(TimeOnly.MaxValue), ct);

        var upcoming = days
            .Where(d => d.IsActive && d.Date >= today && (d.IsHoliday || d.IsExamDay || d.IsEventDay))
            .OrderBy(d => d.Date)
            .Take(count)
            .Select(d => new
            {
                date = d.Date.ToString("dd MMM yyyy"),
                title = d.Title,
                description = d.Description,
                isHoliday = d.IsHoliday,
                isExamDay = d.IsExamDay,
                isEventDay = d.IsEventDay,
                holidayType = d.HolidayType,
                remarks = d.Remarks
            })
            .ToList();

        return Json(upcoming);
    }
}
