using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
public class EmployeePayrollController : Controller
{
    private readonly IEmployeePayrollService _payrollService;
    private readonly IEmployeeService _employeeService;

    public EmployeePayrollController(IEmployeePayrollService payrollService, IEmployeeService employeeService)
    {
        _payrollService = payrollService;
        _employeeService = employeeService;
    }

    [HttpGet]
    [RequirePermission("Employee.Salary.View")]
    public async Task<IActionResult> Index(int employeeId, CancellationToken ct)
    {
        if (employeeId <= 0) return RedirectToAction("Index", "Employee");
        var employee = await _employeeService.GetForEditAsync(employeeId, ct);
        if (employee == null) return NotFound();
        ViewBag.EmployeeName = employee.FullName;
        ViewBag.EmployeeCode = employee.EmployeeCode;
        var salaries = await _payrollService.GetSalariesByEmployeeIdAsync(employeeId, ct);
        return View(salaries);
    }

    [HttpGet]
    [RequirePermission("Employee.Salary.Create")]
    public async Task<IActionResult> Create(int employeeId, CancellationToken ct)
    {
        if (employeeId <= 0) return RedirectToAction("Index", "Employee");
        var employee = await _employeeService.GetForEditAsync(employeeId, ct);
        if (employee == null) return NotFound();
        ViewBag.EmployeeName = employee.FullName;
        return View(new EmployeeSalaryDto { EmployeeId = employeeId, EffectiveFrom = DateTime.Today });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Salary.Create")]
    public async Task<IActionResult> Create(EmployeeSalaryDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);
        await _payrollService.SaveSalaryAsync(dto, ct);
        TempData["SuccessMessage"] = "Salary record saved successfully.";
        return RedirectToAction("Index", new { employeeId = dto.EmployeeId });
    }

    [HttpGet]
    [RequirePermission("Employee.Salary.Edit")]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var salary = await _payrollService.GetSalaryByIdAsync(id, ct);
        if (salary == null) return NotFound();
        return View(salary);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Salary.Edit")]
    public async Task<IActionResult> Edit(EmployeeSalaryDto dto, CancellationToken ct)
    {
        if (!ModelState.IsValid) return View(dto);
        await _payrollService.SaveSalaryAsync(dto, ct);
        TempData["SuccessMessage"] = "Salary record updated.";
        return RedirectToAction("Index", new { employeeId = dto.EmployeeId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Salary.Delete")]
    public async Task<IActionResult> Delete(int id, int employeeId, CancellationToken ct)
    {
        await _payrollService.DeleteSalaryAsync(id, ct);
        TempData["SuccessMessage"] = "Salary record deleted.";
        return RedirectToAction("Index", new { employeeId });
    }
}
