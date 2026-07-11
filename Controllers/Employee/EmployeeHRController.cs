using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;

namespace SchoolManagementSystem.Controllers.Employee;

[Authorize]
[Route("EmployeeHR")]
public class EmployeeHRController : Controller
{
    private readonly IEmployeeHrService _hrService;
    private readonly IEmployeeService _employeeService;

    public EmployeeHRController(IEmployeeHrService hrService, IEmployeeService employeeService)
    {
        _hrService = hrService;
        _employeeService = employeeService;
    }

    private async Task<string?> GetEmployeeNameAsync(int employeeId, CancellationToken ct)
    {
        var emp = await _employeeService.GetForEditAsync(employeeId, ct);
        return emp?.FullName;
    }

    // ── Bank Accounts ──

    [HttpGet("BankAccounts/{employeeId}")]
    [RequirePermission("Employee.BankAccount.View")]
    public async Task<IActionResult> BankAccounts(int employeeId, CancellationToken ct)
    {
        if (employeeId <= 0) return RedirectToAction("Index", "Employee");
        ViewBag.EmployeeName = await GetEmployeeNameAsync(employeeId, ct) ?? "Unknown";
        ViewBag.EmployeeId = employeeId;
        var items = await _hrService.GetBankAccountsAsync(employeeId, ct);
        return View(items);
    }

    [HttpPost("BankAccount/Save")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.BankAccount.Edit")]
    public async Task<IActionResult> SaveBankAccount(EmployeeBankAccountDto dto, CancellationToken ct)
    {
        await _hrService.SaveBankAccountAsync(dto, ct);
        TempData["SuccessMessage"] = "Bank account saved.";
        return RedirectToAction("BankAccounts", new { employeeId = dto.EmployeeId });
    }

    [HttpPost("BankAccount/Delete")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.BankAccount.Delete")]
    public async Task<IActionResult> DeleteBankAccount(int id, int employeeId, CancellationToken ct)
    {
        await _hrService.DeleteBankAccountAsync(id, ct);
        TempData["SuccessMessage"] = "Bank account deleted.";
        return RedirectToAction("BankAccounts", new { employeeId });
    }

    // ── Promotions ──

    [HttpGet("Promotions/{employeeId}")]
    [RequirePermission("Employee.Promotion.View")]
    public async Task<IActionResult> Promotions(int employeeId, CancellationToken ct)
    {
        if (employeeId <= 0) return RedirectToAction("Index", "Employee");
        ViewBag.EmployeeName = await GetEmployeeNameAsync(employeeId, ct) ?? "Unknown";
        ViewBag.EmployeeId = employeeId;
        var items = await _hrService.GetPromotionsAsync(employeeId, ct);
        return View(items);
    }

    [HttpPost("Promotion/Save")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Promotion.Edit")]
    public async Task<IActionResult> SavePromotion(EmployeePromotionDto dto, CancellationToken ct)
    {
        await _hrService.SavePromotionAsync(dto, ct);
        TempData["SuccessMessage"] = "Promotion saved.";
        return RedirectToAction("Promotions", new { employeeId = dto.EmployeeId });
    }

    [HttpPost("Promotion/Delete")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Promotion.Delete")]
    public async Task<IActionResult> DeletePromotion(int id, int employeeId, CancellationToken ct)
    {
        await _hrService.DeletePromotionAsync(id, ct);
        TempData["SuccessMessage"] = "Promotion deleted.";
        return RedirectToAction("Promotions", new { employeeId });
    }

    // ── Transfers ──

    [HttpGet("Transfers/{employeeId}")]
    [RequirePermission("Employee.Transfer.View")]
    public async Task<IActionResult> Transfers(int employeeId, CancellationToken ct)
    {
        if (employeeId <= 0) return RedirectToAction("Index", "Employee");
        ViewBag.EmployeeName = await GetEmployeeNameAsync(employeeId, ct) ?? "Unknown";
        ViewBag.EmployeeId = employeeId;
        var items = await _hrService.GetTransfersAsync(employeeId, ct);
        return View(items);
    }

    [HttpPost("Transfer/Save")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Transfer.Edit")]
    public async Task<IActionResult> SaveTransfer(EmployeeTransferDto dto, CancellationToken ct)
    {
        await _hrService.SaveTransferAsync(dto, ct);
        TempData["SuccessMessage"] = "Transfer saved.";
        return RedirectToAction("Transfers", new { employeeId = dto.EmployeeId });
    }

    [HttpPost("Transfer/Delete")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Transfer.Delete")]
    public async Task<IActionResult> DeleteTransfer(int id, int employeeId, CancellationToken ct)
    {
        await _hrService.DeleteTransferAsync(id, ct);
        TempData["SuccessMessage"] = "Transfer deleted.";
        return RedirectToAction("Transfers", new { employeeId });
    }

    // ── Training ──

    [HttpGet("Training/{employeeId}")]
    [RequirePermission("Employee.Training.View")]
    public async Task<IActionResult> Training(int employeeId, CancellationToken ct)
    {
        if (employeeId <= 0) return RedirectToAction("Index", "Employee");
        ViewBag.EmployeeName = await GetEmployeeNameAsync(employeeId, ct) ?? "Unknown";
        ViewBag.EmployeeId = employeeId;
        var items = await _hrService.GetTrainingsAsync(employeeId, ct);
        return View(items);
    }

    [HttpPost("Training/Save")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Training.Edit")]
    public async Task<IActionResult> SaveTraining(EmployeeTrainingDto dto, CancellationToken ct)
    {
        await _hrService.SaveTrainingAsync(dto, ct);
        TempData["SuccessMessage"] = "Training saved.";
        return RedirectToAction("Training", new { employeeId = dto.EmployeeId });
    }

    [HttpPost("Training/Delete")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Training.Delete")]
    public async Task<IActionResult> DeleteTraining(int id, int employeeId, CancellationToken ct)
    {
        await _hrService.DeleteTrainingAsync(id, ct);
        TempData["SuccessMessage"] = "Training deleted.";
        return RedirectToAction("Training", new { employeeId });
    }

    // ── Awards ──

    [HttpGet("Awards/{employeeId}")]
    [RequirePermission("Employee.Award.View")]
    public async Task<IActionResult> Awards(int employeeId, CancellationToken ct)
    {
        if (employeeId <= 0) return RedirectToAction("Index", "Employee");
        ViewBag.EmployeeName = await GetEmployeeNameAsync(employeeId, ct) ?? "Unknown";
        ViewBag.EmployeeId = employeeId;
        var items = await _hrService.GetAwardsAsync(employeeId, ct);
        return View(items);
    }

    [HttpPost("Award/Save")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Award.Edit")]
    public async Task<IActionResult> SaveAward(EmployeeAwardDto dto, CancellationToken ct)
    {
        await _hrService.SaveAwardAsync(dto, ct);
        TempData["SuccessMessage"] = "Award saved.";
        return RedirectToAction("Awards", new { employeeId = dto.EmployeeId });
    }

    [HttpPost("Award/Delete")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Award.Delete")]
    public async Task<IActionResult> DeleteAward(int id, int employeeId, CancellationToken ct)
    {
        await _hrService.DeleteAwardAsync(id, ct);
        TempData["SuccessMessage"] = "Award deleted.";
        return RedirectToAction("Awards", new { employeeId });
    }

    // ── Disciplinary Actions ──

    [HttpGet("Disciplinary/{employeeId}")]
    [RequirePermission("Employee.Disciplinary.View")]
    public async Task<IActionResult> Disciplinary(int employeeId, CancellationToken ct)
    {
        if (employeeId <= 0) return RedirectToAction("Index", "Employee");
        ViewBag.EmployeeName = await GetEmployeeNameAsync(employeeId, ct) ?? "Unknown";
        ViewBag.EmployeeId = employeeId;
        var items = await _hrService.GetDisciplinaryActionsAsync(employeeId, ct);
        return View(items);
    }

    [HttpPost("Disciplinary/Save")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Disciplinary.Edit")]
    public async Task<IActionResult> SaveDisciplinary(EmployeeDisciplinaryActionDto dto, CancellationToken ct)
    {
        await _hrService.SaveDisciplinaryActionAsync(dto, ct);
        TempData["SuccessMessage"] = "Disciplinary action saved.";
        return RedirectToAction("Disciplinary", new { employeeId = dto.EmployeeId });
    }

    [HttpPost("Disciplinary/Delete")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Disciplinary.Delete")]
    public async Task<IActionResult> DeleteDisciplinary(int id, int employeeId, CancellationToken ct)
    {
        await _hrService.DeleteDisciplinaryActionAsync(id, ct);
        TempData["SuccessMessage"] = "Disciplinary action deleted.";
        return RedirectToAction("Disciplinary", new { employeeId });
    }

    [HttpPost("Disciplinary/Resolve")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Employee.Disciplinary.Edit")]
    public async Task<IActionResult> ResolveDisciplinary(int id, int employeeId, string resolutionRemarks, CancellationToken ct)
    {
        await _hrService.ResolveDisciplinaryActionAsync(id, resolutionRemarks, ct);
        TempData["SuccessMessage"] = "Disciplinary action resolved.";
        return RedirectToAction("Disciplinary", new { employeeId });
    }
}
