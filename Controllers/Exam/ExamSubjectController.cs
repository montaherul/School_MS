using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Services.Interfaces.Exam;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Controllers.Exam;

[Authorize]
public class ExamSubjectController : Controller
{
    private readonly IExamSubjectService _examSubjectService;
    private readonly IUnitOfWork _uow;

    public ExamSubjectController(IExamSubjectService examSubjectService, IUnitOfWork uow)
    {
        _examSubjectService = examSubjectService;
        _uow = uow;
    }

    [HttpGet]
    [RequirePermission("Exam.Update")]
    public async Task<IActionResult> Setup(int examId)
    {
        try
        {
            var vm = await _examSubjectService.GetSubjectSetupAsync(examId);
            return View(vm);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Exam.Update")]
    public async Task<IActionResult> SaveSetup([FromBody] SaveSetupRequest request)
    {
        try
        {
            await _examSubjectService.SetupSubjectsAsync(request.ExamId, request.Subjects);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [RequirePermission("Exam.Update")]
    public async Task<IActionResult> Schedule(int examId)
    {
        try
        {
            var schedules = await _examSubjectService.GetScheduleAsync(examId);
            ViewBag.ExamId = examId;
            return View(schedules);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

[HttpPost]
[ValidateAntiForgeryToken]
[RequirePermission("Exam.Update")]
public async Task<IActionResult> SaveSchedule([FromBody] SaveScheduleRequest request)
{
    try
    {
        await _examSubjectService.SaveScheduleAsync(request.ExamId, request.Schedules);
        return Json(new { success = true });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}

[HttpGet]
[RequirePermission("Exam.Update")]
public async Task<IActionResult> Edit(int id)
{
    try
    {
        var vm = await _examSubjectService.GetSubjectSetupAsyncBySubjectId(id);
        var examSubject = await _uow.Repository<ExamSubject>().Query()
            .Include(es => es.Exam)
            .FirstOrDefaultAsync(es => es.Id == id);
        ViewBag.ExamId = examSubject?.ExamId ?? 0;
        return View(vm);
    }
    catch (KeyNotFoundException)
    {
        return NotFound();
    }
}

[HttpPost]
[ValidateAntiForgeryToken]
[RequirePermission("Exam.Update")]
public async Task<IActionResult> Edit(int id, [FromBody] ExamSubjectConfigDto dto)
{
    try
    {
        await _examSubjectService.UpdateSubjectConfigAsync(id, dto);
        return Json(new { success = true });
    }
    catch (Exception ex)
    {
        return Json(new { success = false, message = ex.Message });
    }
}
}

public class SaveSetupRequest
{
    public int ExamId { get; set; }
    public List<ExamSubjectConfigDto> Subjects { get; set; } = [];
}

public class SaveScheduleRequest
{
    public int ExamId { get; set; }
    public List<ExamScheduleDto> Schedules { get; set; } = [];
}
