using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.ViewModels;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using EmpEntity = SchoolManagementSystem.Models.Entities.Employee.Employee;
using SchoolManagementSystem.Helpers;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly IDesignationService _designationService;
    private readonly IUnitOfWork _uow;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IViewRendererService _viewRenderer;

    public EmployeeController(
        IEmployeeService employeeService,
        IDepartmentService departmentService,
        IDesignationService designationService,
        IUnitOfWork uow,
        IPdfGenerator pdfGenerator,
        IViewRendererService viewRenderer)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
        _designationService = designationService;
        _uow = uow;
        _pdfGenerator = pdfGenerator;
        _viewRenderer = viewRenderer;
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
            var viewModels = items.Select(i => i.MapTo<EmployeeListItemViewModel>()).ToList();
            return Json(new { data = viewModels, last_page = totalPages, total_records = totalRecords });
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

        // Initialize ID Card fields if not present
        if (string.IsNullOrEmpty(dto.EmployeeCardNumber))
        {
            var employeeEntity = await _uow.Repository<EmpEntity>().GetByIdAsync(targetId, ct);
            if (employeeEntity != null)
            {
                employeeEntity.EmployeeCardNumber = $"CARD-{DateTime.Today.Year}-{targetId:D6}";
                employeeEntity.CardIssueDate = DateTime.Today;
                employeeEntity.CardExpiryDate = new DateTime(DateTime.Today.Year + 2, 12, 31);
                employeeEntity.CardVersion = 1;
                employeeEntity.QRVerificationCode = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
                await _uow.SaveChangesAsync(ct);

                // Update DTO
                dto.EmployeeCardNumber = employeeEntity.EmployeeCardNumber;
                dto.CardIssueDate = employeeEntity.CardIssueDate;
                dto.CardExpiryDate = employeeEntity.CardExpiryDate;
                dto.CardVersion = employeeEntity.CardVersion;
                dto.QRVerificationCode = employeeEntity.QRVerificationCode;
            }
        }

        ViewBag.SchoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct) ?? new SchoolSetting { SchoolName = "School Management ERP" };
        
        return View(dto.MapTo<EmployeeDetailsViewModel>());
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

    [HttpGet]
    public async Task<IActionResult> DownloadIdCardPdf(int id, CancellationToken ct)
    {
        if (!await CanViewCardAsync(id, ct))
        {
            return Forbid();
        }

        var employee = await _employeeService.GetDetailsAsync(id, ct);
        if (employee == null) return NotFound("Employee not found.");

        await InitializeCardFieldsAsync(employee, id, ct);

        var schoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct) ?? new SchoolSetting { SchoolName = "School Management ERP" };

        var viewModel = BuildEmployeeCardViewModel([employee], schoolSetting);
        var html = await _viewRenderer.RenderToStringAsync("PrintIdCard", viewModel);
        var pdfBytes = _pdfGenerator.GenerateEmployeeIdCardFromHtml(html);

        // Update tracking
        var currentEmpEntity = await _uow.Repository<EmpEntity>().GetByIdAsync(id, ct);
        if (currentEmpEntity != null)
        {
            currentEmpEntity.CardPrintedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync(ct);
        }

        // Log audit
        var userName = User.Identity?.Name ?? "Unknown";
        await LogAuditAsync("ID Card Downloaded", $"Employee ID Card downloaded for: {employee.FullName} (Code: {employee.EmployeeCode}) by {userName}", ct);

        return File(pdfBytes, "application/pdf", $"ID_Card_{employee.EmployeeCode}.pdf");
    }

    [HttpGet]
    public async Task<IActionResult> PreviewIdCard(int id, CancellationToken ct)
    {
        return await PrintIdCard(id, ct);
    }

    [HttpGet]
    public IActionResult QrCode(int id)
    {
        var png = IdCardQRHelper.GenerateQrCodePng(id.ToString());
        return File(png, "image/png");
    }

    [HttpGet]
    public async Task<IActionResult> PrintIdCard(int id, CancellationToken ct)
    {
        if (!CanManageIdCards())
        {
            return Forbid();
        }

        var employee = await _employeeService.GetDetailsAsync(id, ct);
        if (employee == null) return NotFound("Employee not found.");

        await InitializeCardFieldsAsync(employee, id, ct);

        var schoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct) ?? new SchoolSetting { SchoolName = "School Management ERP" };
        ViewBag.SchoolSetting = schoolSetting;

        // Update printed tracking
        var empToUpdate = await _uow.Repository<EmpEntity>().GetByIdAsync(id, ct);
        if (empToUpdate != null)
        {
            empToUpdate.CardPrintedAt = DateTime.UtcNow;
            await _uow.SaveChangesAsync(ct);
        }

        // Log audit
        var userName = User.Identity?.Name ?? "Unknown";
        await LogAuditAsync("ID Card Printed", $"Employee ID Card printed for: {employee.FullName} (Code: {employee.EmployeeCode}) by {userName}", ct);

        var viewModel = BuildEmployeeCardViewModel([employee], schoolSetting);
        return View("PrintIdCard", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ReissueCard(int id, CancellationToken ct)
    {
        if (!CanManageIdCards())
        {
            return Forbid();
        }

        var employeeEntity = await _uow.Repository<EmpEntity>().GetByIdAsync(id, ct);
        if (employeeEntity == null) return NotFound("Employee not found.");

        employeeEntity.CardVersion += 1;
        employeeEntity.CardIssueDate = DateTime.Today;
        employeeEntity.CardExpiryDate = new DateTime(DateTime.Today.Year + 2, 12, 31);
        employeeEntity.QRVerificationCode = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
        
        await _uow.SaveChangesAsync(ct);

        // Log audit
        var userName = User.Identity?.Name ?? "Unknown";
        await LogAuditAsync("ID Card Reissued", $"Employee ID Card reissued for: {employeeEntity.FullName} (Code: {employeeEntity.EmployeeCode}). New Version: {employeeEntity.CardVersion} by {userName}", ct);

        TempData["SuccessMessage"] = $"ID Card reissued successfully. Version is now {employeeEntity.CardVersion}.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [AllowAnonymous]
    [Route("Employee/Verify/{id}")]
    public async Task<IActionResult> Verify(int id, CancellationToken ct)
    {
        var schoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct) ?? new SchoolSetting { SchoolName = "School Management ERP" };
        ViewBag.SchoolSetting = schoolSetting;

        var dto = await _employeeService.GetDetailsAsync(id, ct);
        if (dto == null)
        {
            ViewBag.IsValid = false;
            return View("Verify", null);
        }

        bool isValid = dto.Status == "Active" && 
                        (dto.CardExpiryDate == null || dto.CardExpiryDate >= DateTime.Today);

        ViewBag.IsValid = isValid;

        if (dto.IsTeachingStaff)
        {
            var teacher = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.Teacher>().Query()
                .FirstOrDefaultAsync(t => t.EmployeeId == id && !t.IsDeleted, ct);
            if (teacher != null)
            {
                ViewBag.TeacherCode = teacher.TeacherCode;
                ViewBag.SubjectSpecialization = teacher.SubjectSpecialization;
                ViewBag.TeachingLevel = teacher.TeachingLevel;
                ViewBag.IsClassTeacher = teacher.IsClassTeacher;
                ViewBag.TeachingExperienceYears = teacher.TeachingExperienceYears;
            }
        }

        return View("Verify", dto);
    }

    [HttpGet]
    public async Task<IActionResult> BulkPrintIdCards(int[]? ids, CancellationToken ct)
    {
        if (!CanManageIdCards())
        {
            return Forbid();
        }

        var schoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct) ?? new SchoolSetting { SchoolName = "School Management ERP" };
        ViewBag.SchoolSetting = schoolSetting;

        if (ids != null && ids.Length > 0)
        {
            var employees = new List<EmployeeDetailsDto>();
            foreach (var id in ids)
            {
                var emp = await _employeeService.GetDetailsAsync(id, ct);
                if (emp != null)
                {
                    await InitializeCardFieldsAsync(emp, id, ct);

                    var empToUpdate = await _uow.Repository<EmpEntity>().GetByIdAsync(id, ct);
                    if (empToUpdate != null)
                    {
                        empToUpdate.CardPrintedAt = DateTime.UtcNow;
                        await _uow.SaveChangesAsync(ct);
                    }

                    employees.Add(emp);
                }
            }

            var userName = User.Identity?.Name ?? "Unknown";
            await LogAuditAsync("Bulk ID Cards Printed", $"Bulk printed ID Cards for {employees.Count} employees by {userName}", ct);

            return View("BulkPrint", employees);
        }

        await PopulateLookupListsAsync(ct);
        return View("BulkPrintFilter");
    }

    [HttpGet]
    public async Task<IActionResult> DownloadBulkIdCardPdf(string ids, CancellationToken ct)
    {
        if (!CanManageIdCards())
        {
            return Forbid();
        }

        var idList = ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
            .Select(int.Parse).ToArray();
        var employees = new List<EmployeeDetailsDto>();

        foreach (var id in idList)
        {
            var emp = await _employeeService.GetDetailsAsync(id, ct);
            if (emp != null)
            {
                await InitializeCardFieldsAsync(emp, id, ct);
                employees.Add(emp);
            }
        }

        if (employees.Count == 0) return NotFound();

        var schoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct) ?? new SchoolSetting { SchoolName = "School Management ERP" };
        var viewModel = BuildEmployeeCardViewModel(employees, schoolSetting);
        var html = await _viewRenderer.RenderToStringAsync("PrintIdCard", viewModel);
        var pdfBytes = _pdfGenerator.GenerateBulkEmployeeIdCardPdfFromHtml(html);

        var userName = User.Identity?.Name ?? "Unknown";
        await LogAuditAsync("Bulk ID Cards Downloaded", $"Bulk downloaded ID Cards for {employees.Count} employees by {userName}", ct);

        return File(pdfBytes, "application/pdf", $"Bulk_Employee_ID_Cards_{DateTime.Today:yyyyMMdd}.pdf");
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ProcessBulkPrint(int[] employeeIds, CancellationToken ct)
    {
        if (employeeIds == null || employeeIds.Length == 0)
        {
            TempData["ErrorMessage"] = "No employees selected for bulk printing.";
            return RedirectToAction(nameof(BulkPrintIdCards));
        }
        return RedirectToAction(nameof(BulkPrintIdCards), new { ids = employeeIds });
    }

    private bool CanManageIdCards()
    {
        return User.IsInRole("Super Admin") || 
               User.IsInRole("Admin") || 
               User.IsInRole("HR") || 
               User.IsInRole("Principal");
    }

    private async Task<bool> CanViewCardAsync(int employeeId, CancellationToken ct)
    {
        if (CanManageIdCards()) return true;

        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (int.TryParse(userIdStr, out var currentUserId))
        {
            var currentEmployee = await _employeeService.GetByUserIdAsync(currentUserId, ct);
            if (currentEmployee != null && currentEmployee.Id == employeeId)
            {
                return true;
            }
        }

        return false;
    }

    private async Task LogAuditAsync(string action, string details, CancellationToken ct)
    {
        try
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            int? userId = null;
            if (int.TryParse(userIdStr, out var parsedId))
            {
                userId = parsedId;
            }

            var ip = HttpContext.Connection.RemoteIpAddress?.ToString();

            var log = new AuditLog
            {
                UserId = userId,
                Module = "EmployeeIDCard",
                Action = action,
                IpAddress = ip,
                Details = details,
                CreatedAt = DateTime.UtcNow
            };

            await _uow.Repository<AuditLog>().AddAsync(log, ct);
            await _uow.SaveChangesAsync(ct);
        }
        catch
        {
            // Ignore audit log error
        }
    }

    private async Task PopulateLookupListsAsync(CancellationToken ct)
    {
        ViewBag.Departments = await _departmentService.GetAllAsync(ct);
        ViewBag.Designations = await _designationService.GetAllAsync(ct);
    }

    private async Task InitializeCardFieldsAsync(EmployeeDetailsDto employee, int id, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(employee.EmployeeCardNumber)) return;

        var employeeEntity = await _uow.Repository<EmpEntity>().GetByIdAsync(id, ct);
        if (employeeEntity == null) return;

        employeeEntity.EmployeeCardNumber = $"CARD-{DateTime.Today.Year}-{id:D6}";
        employeeEntity.CardIssueDate = DateTime.Today;
        employeeEntity.CardExpiryDate = new DateTime(DateTime.Today.Year + 2, 12, 31);
        employeeEntity.CardVersion = 1;
        employeeEntity.QRVerificationCode = Guid.NewGuid().ToString("N").Substring(0, 10).ToUpper();
        await _uow.SaveChangesAsync(ct);

        employee.EmployeeCardNumber = employeeEntity.EmployeeCardNumber;
        employee.CardIssueDate = employeeEntity.CardIssueDate;
        employee.CardExpiryDate = employeeEntity.CardExpiryDate;
        employee.CardVersion = employeeEntity.CardVersion;
        employee.QRVerificationCode = employeeEntity.QRVerificationCode;
    }

    private static EmployeeIdCardPrintViewModel BuildEmployeeCardViewModel(List<EmployeeDetailsDto> employees, SchoolSetting school)
    {
        return new EmployeeIdCardPrintViewModel
        {
            Employees = employees,
            SchoolLogoPath = school.LogoPath ?? "",
            SchoolNameEn = school.SchoolName,
            SchoolEIIN = school.EIIN,
            SchoolWebsite = school.Website,
            SchoolAddress = school.Address,
            SchoolPhone = school.Phone,
            SchoolEmail = school.Email,
            PrincipalName = school.PrincipalName ?? "",
            PrincipalSignaturePath = school.PrincipalSignaturePath ?? ""
        };
    }
}
