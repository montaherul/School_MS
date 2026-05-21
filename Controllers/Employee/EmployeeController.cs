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

    public EmployeeController(
        IEmployeeService employeeService,
        IDepartmentService departmentService,
        IDesignationService designationService)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
        _designationService = designationService;
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
