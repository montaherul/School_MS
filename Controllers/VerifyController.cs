using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Controllers;

[AllowAnonymous]
[Route("Verify")]
public class VerifyController : Controller
{
    private readonly IEmployeeService _employeeService;
    private readonly IUnitOfWork _uow;

    public VerifyController(IEmployeeService employeeService, IUnitOfWork uow)
    {
        _employeeService = employeeService;
        _uow = uow;
    }

    [HttpGet("Student/{id}")]
    public async Task<IActionResult> Student(string id, CancellationToken ct)
    {
        var schoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct)
            ?? new SchoolSetting { SchoolName = "School Management ERP" };
        ViewBag.SchoolSetting = schoolSetting;

        if (string.IsNullOrEmpty(id))
        {
            ViewBag.IsValid = false;
            return View("Student", model: null);
        }

        return View("Student", model: id);
    }

    [HttpGet("Employee/{id}")]
    public async Task<IActionResult> Employee(int id, CancellationToken ct)
    {
        var schoolSetting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(ct)
            ?? new SchoolSetting { SchoolName = "School Management ERP" };
        ViewBag.SchoolSetting = schoolSetting;

        var dto = await _employeeService.GetDetailsAsync(id, ct);
        if (dto == null)
        {
            ViewBag.IsValid = false;
            return View("Employee", model: null);
        }

        bool isValid = dto.Status == "Active" &&
                        (dto.CardExpiryDate == null || dto.CardExpiryDate >= DateTime.Today);
        ViewBag.IsValid = isValid;

        return View("Employee", dto);
    }
}
