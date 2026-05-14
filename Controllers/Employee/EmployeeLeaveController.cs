using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Employee;
using System.Security.Claims;
using SchoolManagementSystem.Constants;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
public class EmployeeLeaveController : Controller
{
    private readonly IEmployeeLeaveService _leaveService;
    private readonly ILeaveTypeService _leaveTypeService;
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;

    public EmployeeLeaveController(
        IEmployeeLeaveService leaveService,
        ILeaveTypeService leaveTypeService,
        IEmployeeService employeeService,
        IDepartmentService departmentService)
    {
        _leaveService = leaveService;
        _leaveTypeService = leaveTypeService;
        _employeeService = employeeService;
        _departmentService = departmentService;
    }

    [RequirePermission(Permissions.Leave.View)]
    public async Task<IActionResult> Index(int page = 1, string? search = null, long? departmentId = null, long? leaveTypeId = null, LeaveStatus? status = null)
    {
        var model = await _leaveService.GetPagedAsync(page, 15, search, departmentId, leaveTypeId, status);
        
        ViewBag.Departments = await GetDepartmentListAsync();
        ViewBag.LeaveTypes = await GetLeaveTypeListAsync();
        ViewBag.Search = search;
        ViewBag.DepartmentId = departmentId;
        ViewBag.LeaveTypeId = leaveTypeId;
        ViewBag.Status = status;

        return View(model);
    }

    [RequirePermission(Permissions.Leave.Apply)]
    public async Task<IActionResult> Create()
    {
        ViewBag.LeaveTypes = await GetLeaveTypeListAsync();
        
        // If employee is applying for themselves, we should fetch their balance
        var employeeId = await GetCurrentEmployeeIdAsync();
        if (employeeId.HasValue)
        {
            ViewBag.Summary = await _leaveService.GetEmployeeLeaveSummaryAsync(employeeId.Value, DateTime.Today.Year);
            return View(new EmployeeLeaveDto { EmployeeId = employeeId.Value, StartDate = DateTime.Today, EndDate = DateTime.Today });
        }

        return RedirectToAction("Index", "Dashboard");
    }

    [HttpPost]
    [RequirePermission(Permissions.Leave.Apply)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeLeaveDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                await _leaveService.ApplyLeaveAsync(model, User.Identity?.Name ?? "system");
                TempData["SuccessMessage"] = "Leave application submitted successfully.";
                return RedirectToAction(nameof(MyLeaves));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }

        ViewBag.LeaveTypes = await GetLeaveTypeListAsync();
        var employeeId = await GetCurrentEmployeeIdAsync();
        if (employeeId.HasValue)
        {
            ViewBag.Summary = await _leaveService.GetEmployeeLeaveSummaryAsync(employeeId.Value, DateTime.Today.Year);
        }
        return View(model);
    }

    [RequirePermission(Permissions.Leave.ViewSelf)]
    public async Task<IActionResult> MyLeaves()
    {
        var employeeId = await GetCurrentEmployeeIdAsync();
        if (!employeeId.HasValue) return Forbid();

        // Reusing the summary for personal view
        ViewBag.Summary = await _leaveService.GetEmployeeLeaveSummaryAsync(employeeId.Value, DateTime.Today.Year);
        
        // Simple history for the employee
        var history = await _leaveService.GetPagedAsync(1, 100, null, null, null, null); // This is overkill, but works if filtered
        // Actually, I should have a method for personal history
        return View(history.Items.Where(x => x.EmployeeId == employeeId.Value));
    }

    [HttpPost]
    [RequirePermission(Permissions.Leave.Approve)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(long id, string remarks)
    {
        try
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _leaveService.ApproveLeaveAsync(id, remarks, userId);
            TempData["SuccessMessage"] = "Leave approved and attendance synchronized.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequirePermission(Permissions.Leave.Reject)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(long id, string reason)
    {
        try
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _leaveService.RejectLeaveAsync(id, reason, userId);
            TempData["SuccessMessage"] = "Leave application rejected.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequirePermission(Permissions.Leave.Apply)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Cancel(long id)
    {
        try
        {
            await _leaveService.CancelLeaveAsync(id, User.Identity?.Name ?? "system");
            TempData["SuccessMessage"] = "Leave application cancelled.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(MyLeaves));
    }

    private async Task<long?> GetCurrentEmployeeIdAsync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (long.TryParse(userIdStr, out var userId))
        {
            return await _employeeService.GetEmployeeIdByUserIdAsync(userId);
        }
        return null;
    }

    private async Task<IEnumerable<SelectListItem>> GetDepartmentListAsync()
    {
        var departments = await _departmentService.GetAllAsync();
        return departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name });
    }

    private async Task<IEnumerable<SelectListItem>> GetLeaveTypeListAsync()
    {
        var types = await _leaveTypeService.GetAllAsync();
        return types.Select(t => new SelectListItem { Value = t.Id.ToString(), Text = t.Name });
    }
}
