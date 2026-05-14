using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Employee;
using System.Security.Claims;
using SchoolManagementSystem.Constants;
using SchoolManagementSystem.Models.DTOs.Common;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
public class EmployeePayrollController : Controller
{
    private readonly IEmployeePayrollService _payrollService;
    private readonly ISalaryStructureService _structureService;
    private readonly IDepartmentService _departmentService;
    private readonly IEmployeeService _employeeService;

    public EmployeePayrollController(
        IEmployeePayrollService payrollService,
        ISalaryStructureService structureService,
        IDepartmentService departmentService,
        IEmployeeService employeeService)
    {
        _payrollService = payrollService;
        _structureService = structureService;
        _departmentService = departmentService;
        _employeeService = employeeService;
    }

    [RequirePermission(Permissions.Payroll.View)]
    public async Task<IActionResult> Index(int month = 0, int year = 0, long? departmentId = null, PayrollPaymentStatus? status = null, CancellationToken ct = default)
    {
        if (month == 0) month = DateTime.Today.Month;
        if (year == 0) year = DateTime.Today.Year;

        ViewBag.Departments = await GetDepartmentListAsync();
        ViewBag.Month = month;
        ViewBag.Year = year;
        ViewBag.DepartmentId = departmentId;
        ViewBag.Status = status;
        ViewBag.Summary = await _payrollService.GetDashboardSummaryAsync(month, year, ct);

        return View();
    }

    [HttpPost]
    [RequirePermission(Permissions.Payroll.View)]
    public async Task<IActionResult> GetPaged([FromBody] GridRequestDto request, CancellationToken ct = default)
    {
        try
        {
            // Extract month/year from filters if possible, or use defaults
            int month = DateTime.Today.Month;
            int year = DateTime.Today.Year;
            
            var result = await _payrollService.GetPagedAsync(request.Page, request.Size, month, year, null, null, ct);
            
            return Json(new PagedApiResponse<SchoolManagementSystem.Models.DTOs.Employee.PayrollRecordListItemDto>
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

    [HttpPost]
    [RequirePermission(Permissions.Payroll.Generate)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Generate(int month, int year, long? departmentId)
    {
        try
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            int count = await _payrollService.GeneratePayrollAsync(month, year, departmentId, userId);
            TempData["SuccessMessage"] = $"{count} payroll records generated successfully.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index), new { month, year, departmentId });
    }

    [HttpPost]
    [RequirePermission(Permissions.Payroll.Approve)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(long id)
    {
        try
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _payrollService.ApprovePayrollAsync(id, userId);
            TempData["SuccessMessage"] = "Payroll approved.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [RequirePermission(Permissions.Payroll.Pay)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> MarkPaid(long id, DateTime paymentDate, string? remarks)
    {
        try
        {
            var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            await _payrollService.MarkAsPaidAsync(id, paymentDate, remarks, userId);
            TempData["SuccessMessage"] = "Salary marked as paid.";
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
        }
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission(Permissions.Payroll.ViewSelf)]
    public async Task<IActionResult> MyPayslips()
    {
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var employeeId = await _employeeService.GetEmployeeIdByUserIdAsync(userId);
        if (!employeeId.HasValue) return Forbid();

        var history = await _payrollService.GetEmployeeHistoryAsync(employeeId.Value);
        return View(history);
    }

    [RequirePermission(Permissions.Payroll.ViewSelf)]
    public async Task<IActionResult> Payslip(long id)
    {
        var payslip = await _payrollService.GetByIdAsync(id);
        if (payslip == null) return NotFound();

        // Security check: If not admin, can only view own payslip
        var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        var employeeId = await _employeeService.GetEmployeeIdByUserIdAsync(userId);
        if (!User.IsInRole(Roles.Admin) && payslip.EmployeeId != employeeId) return Forbid();

        return View(payslip);
    }

    [RequirePermission(Permissions.Payroll.Configure)]
    public async Task<IActionResult> SalaryStructure(long employeeId)
    {
        var employee = await _employeeService.GetDetailAsync(employeeId);
        if (employee == null) return NotFound();

        var activeStructure = await _structureService.GetActiveByEmployeeIdAsync(employeeId);
        var history = await _structureService.GetHistoryByEmployeeIdAsync(employeeId);

        ViewBag.Employee = employee;
        ViewBag.History = history;

        return View(activeStructure ?? new SalaryStructureDto { EmployeeId = employeeId, EffectiveFrom = DateTime.Today });
    }

    [HttpPost]
    [RequirePermission(Permissions.Payroll.Configure)]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> UpdateSalaryStructure(SalaryStructureDto model)
    {
        if (ModelState.IsValid)
        {
            try
            {
                var userId = long.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
                await _structureService.CreateAsync(model, userId);
                TempData["SuccessMessage"] = "Salary structure updated successfully.";
                return RedirectToAction(nameof(SalaryStructure), new { employeeId = model.EmployeeId });
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
            }
        }
        return View(nameof(SalaryStructure), model);
    }

    private async Task<IEnumerable<SelectListItem>> GetDepartmentListAsync()
    {
        var departments = await _departmentService.GetAllAsync();
        return departments.Select(d => new SelectListItem { Value = d.Id.ToString(), Text = d.Name });
    }
}
