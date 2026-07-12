using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Routine;

namespace SchoolManagementSystem.Controllers.Academic;

[Authorize]
public class TeacherWorkloadController : Controller
{
    private readonly IRoutineEngineService _engineService;

    public TeacherWorkloadController(IRoutineEngineService engineService)
    {
        _engineService = engineService;
    }

    [RequirePermission("Routine.View")]
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetList(int academicYearId, CancellationToken ct)
    {
        var data = await _engineService.GetWorkloadSummaryAsync(academicYearId, ct);
        return Json(new { data });
    }

    [RequirePermission("Routine.View")]
    public async Task<IActionResult> Details(int id, int academicYearId, CancellationToken ct)
    {
        var dto = await _engineService.GetTeacherWorkloadDetailAsync(id, academicYearId, ct);
        if (dto == null) return NotFound();
        return View(dto);
    }

    [HttpGet]
    [RequirePermission("Routine.View")]
    public async Task<IActionResult> GetOverloadCount(int academicYearId, CancellationToken ct)
    {
        var count = await _engineService.GetOverloadedTeacherCountAsync(academicYearId, ct: ct);
        return Json(new { count });
    }
}
