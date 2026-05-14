using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using SchoolManagementSystem.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SchoolManagementSystem.Services.Interfaces.Academic;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly IDesignationService _designationService;
    private readonly IRoleRepository _roleRepository;
    private readonly IEmployeeAttendanceService _attendanceService;
    private readonly IEmployeeLeaveService _leaveService;
    private readonly ISalaryStructureService _structureService;
    private readonly SchoolManagementSystem.Services.Interfaces.Academic.ITeacherAcademicService _teacherAcademicService;

    public EmployeeController(
        IEmployeeService employeeService,
        IDepartmentService departmentService,
        IDesignationService designationService,
        IRoleRepository roleRepository,
        IEmployeeAttendanceService attendanceService,
        IEmployeeLeaveService leaveService,
        ISalaryStructureService structureService,
        SchoolManagementSystem.Services.Interfaces.Academic.ITeacherAcademicService teacherAcademicService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
        _designationService = designationService;
        _roleRepository = roleRepository;
        _attendanceService = attendanceService;
        _leaveService = leaveService;
        _structureService = structureService;
        _teacherAcademicService = teacherAcademicService;
    }

    [RequirePermission("Employee.View")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, long? departmentId = null, long? designationId = null, bool? isActive = null, CancellationToken ct = default)

    {
        bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json") || Request.Query.ContainsKey("page");

        if (isAjax)
        {
            var result = await _employeeService.GetPagedAsync(page, pageSize, search, departmentId, designationId, isActive, ct);
            return Json(new { 
                data = result.Items, 
                last_page = Math.Ceiling((double)result.TotalItems / pageSize), 
                total_records = result.TotalItems 
            });
        }

        ViewBag.Departments = await _departmentService.GetAllAsync(ct);
        ViewBag.Designations = await _designationService.GetAllAsync(ct);
        
        return View();
    }

    [RequirePermission("Employee.Create")]
    public async Task<IActionResult> Create()
    {
        var model = new EmployeeViewModel
        {
            Departments = await GetDepartmentListAsync(),
            Designations = await GetDesignationListAsync(),
            Roles = await GetRoleListAsync()
        };
        return View("CreateEdit", model);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(EmployeeViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _employeeService.CreateAsync(model, User.Identity?.Name ?? "system");
            return RedirectToAction(nameof(Index));
        }
        model.Departments = await GetDepartmentListAsync();
        model.Designations = await GetDesignationListAsync();
        model.Roles = await GetRoleListAsync();
        return View("CreateEdit", model);
    }



    [RequirePermission("Employee.Edit")]
    public async Task<IActionResult> Edit(long id)
    {
        var model = await _employeeService.GetForEditAsync(id);
        if (model == null) return NotFound();
        
        model.Departments = await GetDepartmentListAsync();
        model.Designations = await GetDesignationListAsync();
        model.Roles = await GetRoleListAsync();
        return View("CreateEdit", model);
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(EmployeeViewModel model)
    {
        if (ModelState.IsValid)
        {
            await _employeeService.UpdateAsync(model, User.Identity?.Name ?? "system");
            return RedirectToAction(nameof(Index));
        }
        model.Departments = await GetDepartmentListAsync();
        model.Designations = await GetDesignationListAsync();
        model.Roles = await GetRoleListAsync();
        return View("CreateEdit", model);
    }


    [RequirePermission("Employee.View")]
    public async Task<IActionResult> Details(long id)

    {
        var model = await _employeeService.GetDetailAsync(id);
        if (model == null) return NotFound();
        
        ViewBag.AttendanceSummary = await _attendanceService.GetEmployeeSummaryAsync(id);
        ViewBag.LeaveSummary = await _leaveService.GetEmployeeLeaveSummaryAsync(id, DateTime.Today.Year);
        ViewBag.SalaryStructure = await _structureService.GetActiveByEmployeeIdAsync(id);

        if (model.DesignationName != null && model.DesignationName.Contains("Teacher", StringComparison.OrdinalIgnoreCase))
        {
            ViewBag.IsTeacher = true;
            ViewBag.TeacherAssignments = await _teacherAcademicService.GetAssignmentsByTeacherAsync(id);
            ViewBag.TeacherWorkload = await _teacherAcademicService.GetWorkloadAsync(id);
        }
        else
        {
            ViewBag.IsTeacher = false;
        }

        return View(model);
    }

    [HttpPost]
    [RequirePermission("Employee.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ToggleAccess(long id)
    {
        try
        {
            await _employeeService.ToggleAccessAsync(id, User.Identity?.Name ?? "system");
            TempData["SuccessMessage"] = "System access updated successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [RequirePermission("Employee.Delete")]
    public async Task<IActionResult> Delete(long id)

    {
        try
        {
            await _employeeService.DeleteAsync(id, User.Identity?.Name ?? "system");
            return Json(new { success = true, message = "Employee deleted successfully" });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    private async Task<IEnumerable<SelectListItem>> GetDepartmentListAsync()
    {
        var list = await _departmentService.GetAllAsync();
        return list.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
    }

    private async Task<IEnumerable<SelectListItem>> GetDesignationListAsync()
    {
        var list = await _designationService.GetAllAsync();
        return list.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
    }

    private async Task<IEnumerable<SelectListItem>> GetRoleListAsync()
    {
        var list = await _roleRepository.ListAsync();
        return list.Select(x => new SelectListItem { Value = x.Id.ToString(), Text = x.Name });
    }

    [Authorize]
    public async Task<IActionResult> MyProfile()
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var employeeId = await _employeeService.GetEmployeeIdByUserIdAsync(userId);
        if (!employeeId.HasValue) return NotFound("Employee record not found for this user.");
        
        var model = await _employeeService.GetDetailAsync(employeeId.Value);
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateContact(EmployeeViewModel model)
    {
        // Security check: ensure user is updating their own profile or has permission
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var employeeId = await _employeeService.GetEmployeeIdByUserIdAsync(userId);
        
        if (employeeId != model.Id && !User.HasClaim("Permission", "Employee.Update"))
        {
            return Forbid();
        }

        // Only update contact fields
        var existing = await _employeeService.GetDetailAsync(model.Id);
        if (existing == null) return NotFound();

        existing.Phone = model.Phone;
        existing.Email = model.Email;
        existing.PresentVillage = model.PresentVillage;
        existing.PresentPostOffice = model.PresentPostOffice;
        existing.PresentThana = model.PresentThana;
        existing.PresentDistrict = model.PresentDistrict;

        await _employeeService.UpdateAsync(existing, User.Identity!.Name!);
        TempData["SuccessMessage"] = "Profile updated successfully.";
        return RedirectToAction(nameof(MyProfile));
    }
}

