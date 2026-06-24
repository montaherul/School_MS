using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Services.Interfaces.Attendance;

namespace SchoolManagementSystem.Controllers.Attendance
{
    using SchoolManagementSystem.Filters;

    [RequirePermission("Attendance.AutoAbsent")]
    [Route("AutoAbsent")]
    public class AutoAbsentController : Controller
    {
        private readonly IAutoAbsentService _autoAbsentService;

        public AutoAbsentController(IAutoAbsentService autoAbsentService)
        {
            _autoAbsentService = autoAbsentService;
        }

        [HttpGet("")]
        public async Task<IActionResult> Index(CancellationToken ct)
        {
            var executions = await _autoAbsentService.GetRecentExecutionsAsync(20, ct);
            var lastRun = await _autoAbsentService.GetLastExecutionAsync(ct);
            ViewBag.LastRun = lastRun;
            return View(executions);
        }

        [HttpPost("RunForToday")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunForToday(CancellationToken ct)
        {
            try
            {
                var result = await _autoAbsentService.RunForTodayAsync(User.Identity?.Name ?? "system", ct);
                if (result == null)
                {
                    return Json(new { success = false, message = "Auto-Absent is disabled in settings." });
                }
                return Json(new
                {
                    success = result.Status == "Success",
                    status = result.Status,
                    message = result.Message,
                    studentsMarked = result.StudentsMarkedAbsent,
                    employeesMarked = result.EmployeesMarkedAbsent,
                    durationMs = result.DurationMs
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost("RunForDate")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RunForDate(DateTime targetDate, CancellationToken ct)
        {
            try
            {
                var result = await _autoAbsentService.RunForDateAsync(targetDate, User.Identity?.Name ?? "system", ct);
                return Json(new
                {
                    success = result.Status == "Success",
                    status = result.Status,
                    message = result.Message,
                    studentsMarked = result.StudentsMarkedAbsent,
                    employeesMarked = result.EmployeesMarkedAbsent,
                    durationMs = result.DurationMs
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpGet("History")]
        public async Task<IActionResult> History(int count = 30, CancellationToken ct = default)
        {
            var logs = await _autoAbsentService.GetRecentExecutionsAsync(count, ct);
            return Json(new { data = logs });
        }
    }
}
