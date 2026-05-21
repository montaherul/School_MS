using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Website;

namespace SchoolManagementSystem.Controllers.Website;

public class EventController : Controller
{
    private readonly IEventService _eventService;

    public EventController(IEventService eventService)
    {
        _eventService = eventService;
    }

    [HttpGet("/events")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var events = await _eventService.GetAllEventsAsync(ct);
        return View(events);
    }

    [HttpGet("/event/details/{id}")]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var ev = await _eventService.GetEventByIdAsync(id, ct);
        if (ev == null || ev.IsDeleted || !ev.IsPublished)
        {
            return NotFound();
        }
        return View(ev);
    }
}
