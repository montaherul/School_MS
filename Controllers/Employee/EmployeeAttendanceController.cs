using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Constants;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
public class EmployeeAttendanceController : Controller
{
    private readonly IEmployeeAttendanceService _attendanceService;
    private readonly IDepartmentService _departmentService;

    public EmployeeAttendanceController(
        IEmployeeAttendanceService attendanceService,
        IDepartmentService departmentService)
    {
        _attendanceService = attendanceService;
        _departmentService = departmentService;
    }

    [RequirePermission(Permissions.Attendance.View)]
    public async Task<IActionResult> Index(DateTime? date, long? departmentId, CancellationToken ct = default)
    {
        ViewBag.Departments = await GetDepartmentListAsync();
        ViewBag.TargetDate = date ?? DateTime.Today;
        return View();
    }

    [HttpPost]
    [RequirePermission(Permissions.Attendance.View)]
    public async Task<IActionResult> GetPaged([FromBody] GridRequestDto request, CancellationToken ct = default)
    {
        try
        {
            // For attendance, we might need date range from JSON Filters or standardized fields
            // For now, using nulls to show the pattern
            var result = await _attendanceService.GetPagedAsync(request.Page, request.Size, request.Search, null, null, null, null, ct);
            
            return Json(new PagedApiResponse<SchoolManagementSystem.Models.DTOs.Employee.EmployeeAttendanceDto>
            { 
                data = result.Items, 
                last_page = (int)Math.Ceiling((double)result.TotalItems / request.Size),
                total = result.TotalItems 
            });
        }
        catch (Exception ex)
        {
            return Json(new PagedApiResponse<object>
            {
                data = new List<object>(),
                last_page = 1,
                total = 0,
                error = ex.Message
            });
        }
    }

    [RequirePermission(Permissions.Attendance.Mark)]
    public async Task<IActionResult> MarkDailyAttendance(DateTime? date, long? departmentId)
    {
        var targetDate = date ?? DateTime.Today;
        var model = await _attendanceService.GetDailyAttendanceAsync(targetDate, departmentId);
        
        ViewBag.Departments = await GetDepartmentListAsync();
        ViewBag.TargetDate = targetDate;
        ViewBag.DepartmentId = departmentId;

        return View(model);
    }

    [HttpPost]
    [RequirePermission(Permissions.Attendance.Mark)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveAttendance(List<EmployeeAttendanceDto> attendanceList)
    {
        if (attendanceList == null || !attendanceList.Any())
        {
            TempData["ErrorMessage"] = "No attendance data to save.";
            return RedirectToAction(nameof(MarkDailyAttendance));
        }

        try
        {
            await _attendanceService.MarkAttendanceAsync(attendanceList, User.Identity?.Name ?? "system");
            TempData["SuccessMessage"] = "Attendance saved successfully.";
            return RedirectToAction(nameof(Index), new { date = attendanceList.First().AttendanceDate });
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(MarkDailyAttendance), new { date = attendanceList.First().AttendanceDate });
        }
    }

    [RequirePermission(Permissions.Attendance.View)]
    public async Task<IActionResult> EmployeeHistory(long id, int page = 1, DateTime? startDate = null, DateTime? endDate = null)
    {
        var model = await _attendanceService.GetEmployeeHistoryPagedAsync(id, page, 20, startDate, endDate);
        ViewBag.EmployeeId = id;
        ViewBag.StartDate = startDate;
        ViewBag.EndDate = endDate;
        return View(model);
    }

    [RequirePermission(Permissions.Reports.View)]
    public async Task<IActionResult> MonthlyReport(long? employeeId, DateTime? month)
    {
        var targetMonth = month ?? DateTime.Today;
        if (employeeId.HasValue)
        {
            var summary = await _attendanceService.GetEmployeeSummaryAsync(employeeId.Value, targetMonth);
            return View("EmployeeMonthlyReport", summary);
        }
        
        // General monthly report logic could go here
        return View();
    }

    private async Task<IEnumerable<SelectListItem>> GetDepartmentListAsync()
    {
        var departments = await _departmentService.GetAllAsync();
        return departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name });
    }
}
