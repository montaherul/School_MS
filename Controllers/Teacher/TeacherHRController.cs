using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Teacher;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Teacher;

[Authorize]
public class TeacherHRController : Controller
{
    private readonly ITeacherHRService _hrService;
    private readonly ITeacherService _teacherService;

    public TeacherHRController(ITeacherHRService hrService, ITeacherService teacherService)
    {
        _hrService = hrService;
        _teacherService = teacherService;
    }

    // ── Attendance ───────────────────────────────────────────────────────────

    [RequirePermission("Teachers.Attendance")]
    public async Task<IActionResult> Attendance(DateTime? date, string? department)
    {
        var targetDate = date ?? DateTime.Today;
        var attendance = await _hrService.GetAttendanceAsync(targetDate, department);
        ViewBag.Date = targetDate;
        ViewBag.Department = department;
        return View(attendance);
    }

    [HttpPost]
    [RequirePermission("Teachers.Attendance")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkAttendance(List<TeacherAttendanceDto> attendanceList, DateTime date)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        foreach (var item in attendanceList) item.AttendanceDate = date;
        
        await _hrService.MarkAttendanceAsync(attendanceList, userId);
        TempData["SuccessMessage"] = "Attendance marked successfully.";
        return RedirectToAction(nameof(Attendance), new { date = date.ToString("yyyy-MM-dd") });
    }

    // ── Leaves ───────────────────────────────────────────────────────────────

    [RequirePermission("Teachers.Leaves.View")]
    public async Task<IActionResult> Leaves(int page = 1, string? status = null)
    {
        var result = await _hrService.GetLeavesPagedAsync(page, 10, status);
        return View(result);
    }

    [HttpPost]
    [RequirePermission("Teachers.Leaves.Manage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ApproveLeave(int id, string remarks)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _hrService.ApproveLeaveAsync(id, remarks, userId);
        TempData["SuccessMessage"] = "Leave approved.";
        return RedirectToAction(nameof(Leaves));
    }

    [HttpPost]
    [RequirePermission("Teachers.Leaves.Manage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> RejectLeave(int id, string remarks)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _hrService.RejectLeaveAsync(id, remarks, userId);
        TempData["SuccessMessage"] = "Leave rejected.";
        return RedirectToAction(nameof(Leaves));
    }

    // ── Payroll ──────────────────────────────────────────────────────────────

    [RequirePermission("Teachers.Payroll.View")]
    public async Task<IActionResult> Payroll(int page = 1, DateTime? monthYear = null)
    {
        var result = await _hrService.GetPayrollPagedAsync(page, 10, monthYear);
        ViewBag.MonthYear = monthYear;
        return View(result);
    }

    [HttpPost]
    [RequirePermission("Teachers.Payroll.Manage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GeneratePayroll(DateTime monthYear)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _hrService.GenerateMonthlyPayrollAsync(monthYear, userId);
        TempData["SuccessMessage"] = "Payroll generated for " + monthYear.ToString("MMMM yyyy");
        return RedirectToAction(nameof(Payroll), new { monthYear = monthYear.ToString("yyyy-MM-dd") });
    }

    [HttpPost]
    [RequirePermission("Teachers.Payroll.Manage")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdatePayrollStatus(int id, string status)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _hrService.UpdatePayrollStatusAsync(id, status, userId);
        TempData["SuccessMessage"] = "Payroll status updated to " + status;
        return RedirectToAction(nameof(Payroll));
    }
}
