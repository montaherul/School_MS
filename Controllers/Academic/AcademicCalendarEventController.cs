using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class AcademicCalendarEventController : Controller
{
    private readonly IAcademicCalendarEventService _service;

    public AcademicCalendarEventController(IAcademicCalendarEventService service)
    {
        _service = service;
    }

    [RequirePermission("Calendar.View")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Calendar.View")]
    public async Task<IActionResult> GetList(int calendarId, CancellationToken ct)
    {
        var events = await _service.GetEventsByCalendarAsync(calendarId, ct);
        return Json(new { data = events });
    }

    [HttpGet]
    [RequirePermission("Calendar.Create")]
    public async Task<IActionResult> Create(int calendarId, CancellationToken ct)
    {
        var calendar = await _service.GetCalendarByIdAsync(calendarId, ct);
        if (calendar == null) return NotFound();

        ViewBag.Calendar = calendar;
        ViewBag.EventTypes = new[] { "Holiday", "WeeklyOff", "Exam", "Vacation", "Event" };
        return View(new AcademicCalendarEventDto { AcademicCalendarId = calendarId, IsActive = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Create")]
    public async Task<IActionResult> Create(AcademicCalendarEventDto dto, int calendarId, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            var calendar = await _service.GetCalendarByIdAsync(calendarId, ct);
            ViewBag.Calendar = calendar;
            ViewBag.EventTypes = new[] { "Holiday", "WeeklyOff", "Exam", "Vacation", "Event" };
            return View(dto);
        }

        dto.AcademicCalendarId = calendarId;
        await _service.CreateAsync(dto, User.Identity?.Name ?? "system", ct);
        TempData["Success"] = "Event created.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Calendar.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        if (dto == null) return NotFound();

        ViewBag.EventTypes = new[] { "Holiday", "WeeklyOff", "Exam", "Vacation", "Event" };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Edit")]
    public async Task<IActionResult> Edit(AcademicCalendarEventDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.EventTypes = new[] { "Holiday", "WeeklyOff", "Exam", "Vacation", "Event" };
            return View(dto);
        }

        await _service.UpdateAsync(dto, User.Identity?.Name ?? "system", ct);
        TempData["Success"] = "Event updated.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Calendar.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        try
        {
            await _service.DeleteAsync(id, User.Identity?.Name ?? "system", ct);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
