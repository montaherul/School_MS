
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using SchoolManagementSystem.Filters;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Constants;
using SchoolManagementSystem.Models.Common;
using SchoolManagementSystem.Models.DTOs.Common;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;
using System.Security.Claims;


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

        IDesignationService designationService)

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

    [RequirePermission(Permissions.Employee.View)]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        ViewBag.Departments = await _departmentService.GetAllAsync(ct);
        ViewBag.Designations = await _designationService.GetAllAsync(ct);
        return View();
    }

    [HttpPost]
    [RequirePermission(Permissions.Employee.View)]
    public async Task<IActionResult> GetPaged([FromBody] GridRequestDto request, CancellationToken ct = default)
    {
        try
        {
            // Extract filters from JSON if provided
            int? departmentId = null;
            int? designationId = null;

            if (!string.IsNullOrEmpty(request.Filters))
            {
                try
                {
                    var filters = System.Text.Json.JsonDocument.Parse(request.Filters).RootElement;
                    if (filters.TryGetProperty("departmentId", out var deptVal) && deptVal.ValueKind != System.Text.Json.JsonValueKind.Null)
                        departmentId = deptVal.GetInt32();
                    if (filters.TryGetProperty("designationId", out var desigVal) && desigVal.ValueKind != System.Text.Json.JsonValueKind.Null)
                        designationId = desigVal.GetInt32();
                }
                catch { /* Ignore filter parsing errors */ }
            }

            var result = await _employeeService.GetPagedAsync(request.Page, request.Size, request.Search, departmentId, designationId, null, ct);
            
            return Json(new PagedApiResponse<SchoolManagementSystem.Models.DTOs.Employee.EmployeeListItemDto>
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

    [RequirePermission(Permissions.Employee.Create)]
    public async Task<IActionResult> Create()
    {
        var model = new EmployeeViewModel
        {
            Departments = await GetDepartmentListAsync(),
            Designations = await GetDesignationListAsync(),
            Roles = await GetRoleListAsync()

    }

    [RequirePermission("Users.View")] // Fallback to Users permission or specialized if desired
    public async Task<IActionResult> Index(
        int page = 1, int size = 10, string? search = null, 
        int? departmentId = null, int? designationId = null, 
        bool? isTeachingStaff = null, string? status = null, CancellationToken ct = default)
    {
        var (items, totalRecords) = await _employeeService.GetPagedAsync(page, size, search, departmentId, designationId, isTeachingStaff, status, ct);
        
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest")
        {
            var totalPages = (int)Math.Ceiling((double)totalRecords / size);
            return Json(new { data = items, last_page = totalPages, total_records = totalRecords });
        }

        await PopulateLookupListsAsync(ct);
        return View();
    }

    [RequirePermission("Users.Create")]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        await PopulateLookupListsAsync(ct);
        var model = new EmployeeUpsertDto
        {
            JoiningDate = DateTime.Today,
            DateOfBirth = DateTime.Today.AddYears(-25),
            Status = "Active",
            IsTeachingStaff = false

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



    [RequirePermission(Permissions.Employee.Update)]
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


    [RequirePermission(Permissions.Employee.View)]
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
    [RequirePermission(Permissions.Employee.Update)]
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
    [RequirePermission(Permissions.Employee.Delete)]
    public async Task<IActionResult> Delete(long id)

    {
        try
        {
            await _employeeService.DeleteAsync(id, User.Identity?.Name ?? "system");
            return Json(ApiResponse.Ok("Employee deleted successfully"));
        }
        catch (Exception ex)
        {
            return Json(ApiResponse.Fail(ex.Message));
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
        
        if (employeeId != model.Id && !User.HasClaim("Permission", Permissions.Employee.Update))

    [RequirePermission("Users.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var dto = await _employeeService.GetForEditAsync(id, ct);
        if (dto == null) return NotFound("Employee not found");
        
        await PopulateLookupListsAsync(ct);
        return View("CreateEdit", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Save(EmployeeUpsertDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            await PopulateLookupListsAsync(ct);
            return View("CreateEdit", dto);
        }

        try
        {
            var empId = await _employeeService.SaveAsync(dto, ct);
            TempData["SuccessMessage"] = dto.Id == 0 
                ? "Employee created and system user account provisioned successfully." 
                : "Employee records updated successfully.";

            return RedirectToAction(nameof(Details), new { id = empId });
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            await PopulateLookupListsAsync(ct);
            return View("CreateEdit", dto);
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, $"An unexpected error occurred: {ex.Message}");
            await PopulateLookupListsAsync(ct);
            return View("CreateEdit", dto);
        }
    }

    [Route("Employee/Details/{id?}")]
    public async Task<IActionResult> Details(int? id, CancellationToken ct)
    {
        int targetId;
        bool isOwnProfile = false;

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        int.TryParse(userIdStr, out var currentUserId);
        
        var currentEmployeeDto = await _employeeService.GetByUserIdAsync(currentUserId, ct);

        if (id == null || id == 0)
        {
            if (currentEmployeeDto == null) return NotFound("Employee profile not found.");
            targetId = currentEmployeeDto.Id;
            isOwnProfile = true;
        }
        else
        {
            targetId = id.Value;
            if (currentEmployeeDto != null && currentEmployeeDto.Id == targetId)
            {
                isOwnProfile = true;
            }
        }

        // Security Check: Users.View allows viewing any employee, or they can view their own profile.
        if (!isOwnProfile && !User.HasClaim("Permission", "Users.View") && !User.IsInRole("Super Admin"))

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


        var dto = await _employeeService.GetDetailsAsync(targetId, ct);
        if (dto == null) return NotFound("Employee details not found.");
        
        return View(dto);
    }

    [HttpPost]
    [RequirePermission("Users.Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await _employeeService.DeleteAsync(id, ct);
        if (!success) return NotFound("Employee not found");

        TempData["SuccessMessage"] = "Employee records soft-deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequirePermission("Users.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status, CancellationToken ct)
    {
        var success = await _employeeService.UpdateStatusAsync(id, status, ct);
        if (!success) return NotFound("Employee not found");

        TempData["SuccessMessage"] = $"Employee status updated to {status} successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    public async Task<IActionResult> VerifyCode(string code, int? id, CancellationToken ct)
    {
        var exists = await _employeeService.IsCodeExistsAsync(code, id, ct);
        return Json(!exists);
    }

    [HttpGet]
    public async Task<IActionResult> VerifyEmail(string email, int? id, CancellationToken ct)
    {
        var exists = await _employeeService.IsEmailExistsAsync(email, id, ct);
        return Json(!exists);
    }

    [HttpGet]
    public async Task<IActionResult> VerifyPhone(string phone, int? id, CancellationToken ct)
    {
        var exists = await _employeeService.IsPhoneExistsAsync(phone, id, ct);
        return Json(!exists);
    }

    private async Task PopulateLookupListsAsync(CancellationToken ct)
    {
        ViewBag.Departments = await _departmentService.GetAllAsync(ct);
        ViewBag.Designations = await _designationService.GetAllAsync(ct);
    }
}

