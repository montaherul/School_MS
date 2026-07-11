using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.ViewModels;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;
using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using EmpEntity = SchoolManagementSystem.Models.Entities.Employee.Employee;
using SalaryEntity = SchoolManagementSystem.Models.Entities.Employee.EmployeeSalary;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
public class EmployeeController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IDepartmentService _departmentService;
    private readonly IDesignationService _designationService;
    private readonly IUnitOfWork _uow;
    private readonly IViewRendererService _viewRenderer;
    private readonly IPdfGenerator _pdfGenerator;

    public EmployeeController(
        IEmployeeService employeeService,
        IDepartmentService departmentService,
        IDesignationService designationService,
        IUnitOfWork uow,
        IViewRendererService viewRenderer,
        IPdfGenerator pdfGenerator)
    {
        _employeeService = employeeService;
        _departmentService = departmentService;
        _designationService = designationService;
        _uow = uow;
        _viewRenderer = viewRenderer;
        _pdfGenerator = pdfGenerator;
    }

    [RequirePermission("Employees.View")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        var model = await _employeeService.GetDashboardAsync(ct);
        return View(model);
    }

    [RequirePermission("Employees.View")]
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

    [RequirePermission("Employees.Create")]
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

    [RequirePermission("Employees.Edit")]
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
        // Runtime permission check (Create vs Edit based on Id)
        var requiredPerm = dto.Id == 0 ? "Employees.Create" : "Employees.Edit";
        if (!User.HasClaim("Permission", requiredPerm) && !User.IsInRole("Super Admin"))
            return Forbid();

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
            ModelState.AddModelError(string.Empty, $"An unexpected error occurred.");
            await PopulateLookupListsAsync(ct);
            return View("CreateEdit", dto);
        }
    }

    [Route("Employee/Details/{id?}")]
    [RequirePermission("Employees.View")]
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

        // Security Check: own profile bypasses Employees.View
        if (!isOwnProfile && !User.HasClaim("Permission", "Employees.View") && !User.IsInRole("Super Admin"))
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

        // Load additional profile data
        var attendanceQuery = _uow.Repository<SchoolManagementSystem.Models.Entities.Attendance.EmployeeAttendance>().Query()
            .Where(a => a.EmployeeId == targetId && a.AttendanceDate.Year == DateTime.Today.Year && !a.IsDeleted);
        var attendanceStats = await attendanceQuery
            .GroupBy(a => 1)
            .Select(g => new
            {
                Present = g.Count(a => a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Present),
                Absent = g.Count(a => a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Absent),
                Leave = g.Count(a => a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Leave),
                Late = g.Count(a => a.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Late)
            })
            .FirstOrDefaultAsync(ct);
        ViewBag.TotalPresent = attendanceStats?.Present ?? 0;
        ViewBag.TotalAbsent = attendanceStats?.Absent ?? 0;
        ViewBag.TotalLeave = attendanceStats?.Leave ?? 0;
        ViewBag.TotalLate = attendanceStats?.Late ?? 0;

        var leaveQuery = _uow.Repository<SchoolManagementSystem.Models.Entities.Attendance.LeaveApplication>().Query()
            .Where(l => l.EmployeeId == targetId && l.FromDate.Year == DateTime.Today.Year);
        ViewBag.TotalLeaveDays = await leaveQuery.SumAsync(l => l.TotalDays, ct);

        var salary = await _uow.Repository<SalaryEntity>().Query()
            .Where(s => s.EmployeeId == targetId && !s.IsDeleted)
            .OrderByDescending(s => s.EffectiveFrom)
            .FirstOrDefaultAsync(ct);
        ViewBag.CurrentSalary = salary?.TotalSalary ?? 0;

        // Teaching profile
        var teacher = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.Teacher>().Query()
            .Include(t => t.ClassAssignments.Where(ca => !ca.IsDeleted && ca.IsActive))
            .Include(t => t.SubjectAssignments.Where(sa => !sa.IsDeleted && sa.IsActive))
            .FirstOrDefaultAsync(t => t.EmployeeId == targetId && !t.IsDeleted, ct);
        ViewBag.TeacherProfile = teacher;

        // Generate QR code for ID Card
        if (!string.IsNullOrEmpty(dto.QRVerificationCode))
        {
            ViewBag.QRCodeBase64 = IdCardQRHelper.GenerateQrCodeBase64(dto.QRVerificationCode);
        }

        // Load audit log history for employee's user account
        var empEntity = await _uow.Repository<EmpEntity>().Query()
            .Where(e => e.Id == targetId && !e.IsDeleted)
            .Select(e => e.UserId)
            .FirstOrDefaultAsync(ct);
        if (empEntity.HasValue)
        {
            ViewBag.AuditLogs = await _uow.Repository<SchoolManagementSystem.Models.Entities.Auth.AuditLog>().Query()
                .Where(al => al.UserId == empEntity.Value)
                .OrderByDescending(al => al.CreatedAt)
                .Take(200)
                .ToListAsync(ct);
        }
        else
        {
            ViewBag.AuditLogs = new List<SchoolManagementSystem.Models.Entities.Auth.AuditLog>();
        }

        return View(dto.MapTo<EmployeeDetailsViewModel>());
    }

    [HttpPost]
    [RequirePermission("Employees.Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var success = await _employeeService.DeleteAsync(id, ct);
        if (!success) return NotFound("Employee not found");

        TempData["SuccessMessage"] = "Employee records soft-deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [HttpGet]
    [RequirePermission("Employees.View")]
    public async Task<IActionResult> DownloadServiceBookPdf(int id, CancellationToken ct)
    {
        var dto = await _employeeService.GetDetailsAsync(id, ct);
        if (dto == null) return NotFound("Employee not found");

        var schoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct) ?? new SchoolSetting { SchoolName = "School Management ERP" };
        ViewBag.SchoolSetting = schoolSetting;

        var salary = await _uow.Repository<SalaryEntity>().Query()
            .Where(s => s.EmployeeId == id && !s.IsDeleted)
            .OrderByDescending(s => s.EffectiveFrom)
            .FirstOrDefaultAsync(ct);
        ViewBag.CurrentSalary = salary?.TotalSalary ?? 0;

        var vm = dto.MapTo<EmployeeDetailsViewModel>();
        var html = await _viewRenderer.RenderToStringAsync("ServiceBookPdf", vm);
        var pdf = _pdfGenerator.GenerateFromHtml(html);
        return File(pdf, "application/pdf", $"ServiceBook_{dto.EmployeeCode}_{DateTime.Today:yyyyMMdd}.pdf");
    }

    [HttpPost]
    [RequirePermission("Employees.Edit")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateStatus(int id, string status, CancellationToken ct)
    {
        var success = await _employeeService.UpdateStatusAsync(id, status, ct);
        if (!success) return NotFound("Employee not found");

        TempData["SuccessMessage"] = $"Employee status updated to {status} successfully.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpGet]
    [RequirePermission("Employees.View")]
    public async Task<IActionResult> VerifyCode(string code, int? id, CancellationToken ct)
    {
        var exists = await _employeeService.IsCodeExistsAsync(code, id, ct);
        return Json(!exists);
    }

    [HttpGet]
    [RequirePermission("Employees.View")]
    public async Task<IActionResult> VerifyEmail(string email, int? id, CancellationToken ct)
    {
        var exists = await _employeeService.IsEmailExistsAsync(email, id, ct);
        return Json(!exists);
    }

    [HttpGet]
    [RequirePermission("Employees.View")]
    public async Task<IActionResult> VerifyPhone(string phone, int? id, CancellationToken ct)
    {
        var exists = await _employeeService.IsPhoneExistsAsync(phone, id, ct);
        return Json(!exists);
    }

    [HttpGet]
    [RequirePermission("Employees.View")]
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

    private async Task PopulateLookupListsAsync(CancellationToken ct)
    {
        ViewBag.Departments = await _departmentService.GetAllAsync(ct);
        ViewBag.Designations = await _designationService.GetAllAsync(ct);
    }
}
