using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class MeritListController : Controller
{
    private readonly IMeritCalculationService _meritService;
    private readonly IUnitOfWork _uow;

    public MeritListController(IMeritCalculationService meritService, IUnitOfWork uow)
    {
        _meritService = meritService;
        _uow = uow;
    }

    [HttpGet]
    [RequirePermission("MeritList.View")]
    public async Task<IActionResult> Index(int? examId, MeritCategory category = MeritCategory.Class, CancellationToken ct = default)
    {
        var exams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted);
        ViewBag.Exams = exams;
        ViewBag.SelectedExamId = examId;
        ViewBag.SelectedCategory = category;

        var meritItems = new List<MeritListItem>();

        if (examId.HasValue && examId > 0)
        {
            var items = await _meritService.GetMeritListAsync(examId.Value, category);
            meritItems = items.ToList();
        }

        return View(meritItems);
    }

    [HttpPost]
    [RequirePermission("MeritList.Recalculate")]
    public async Task<IActionResult> Recalculate(int examId)
    {
        try
        {
            await _meritService.RecalculateMeritPositionsAsync(examId);
            return Json(new { success = true, message = "Merit positions recalculated successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
