using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Fees;
using SchoolManagementSystem.Models.ViewModels.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Fees;

[Authorize]
public class FeeRefundController : Controller
{
    private readonly IFeeRefundService _service;
    private readonly IFeeSecurityService _security;
    public FeeRefundController(IFeeRefundService service, IFeeSecurityService security) { _service = service; _security = security; }

    [RequirePermission("FeeRefunds.Read")]
    public IActionResult Index() { return View(); }

    [HttpGet]
    [RequirePermission("FeeRefunds.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("FeeRefunds.Update")]
    public IActionResult Edit(int id) => RedirectToAction(nameof(CreateEdit), new { id });

    [HttpGet]
    [RequirePermission("FeeRefunds.Read")]
    public async Task<IActionResult> GetList(int page = 1, int size = 10, string? search = null)
    {
        var result = await _service.GetPagedAsync(page, size, search);
        if (_security.HasStudentRole(User))
        {
            var myId = _security.GetCurrentStudentId(User);
            if (myId.HasValue)
                result = new PagedResult<FeeRefundListItemDto> { Items = result.Items.Where(i => i.StudentId == myId.Value).ToList(), Page = result.Page, PageSize = result.PageSize, TotalItems = result.TotalItems };
        }
        return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize) });
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id)
    {
        if (!_security.Can(User, id.HasValue && id > 0 ? "FeeRefunds.Update" : "FeeRefunds.Create"))
            return Forbid();
        if (id.HasValue && id > 0)
        {
            var dto = await _service.GetForEditAsync(id.Value);
            if (dto == null) return NotFound();
            if (_security.HasStudentRole(User) && !await _security.CanAccessRefundAsync(User, id.Value))
                return Forbid();
            return View(new FeeRefundViewModel { Id = dto.Id, FeePaymentId = dto.FeePaymentId, RefundAmount = dto.RefundAmount, RefundMethod = dto.RefundMethod, ReferenceNo = dto.ReferenceNo, Reason = dto.Reason, IsApproved = dto.IsApproved, RefundDate = dto.RefundDate });
        }
        return View(new FeeRefundViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(FeeRefundViewModel vm)
    {
        if (!_security.Can(User, vm.IsEditMode ? "FeeRefunds.Update" : "FeeRefunds.Create"))
            return Forbid();
        if (!ModelState.IsValid) return View(vm);
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (vm.IsEditMode) { await _service.UpdateAsync(vm, userId); TempData["SuccessMessage"] = "Refund updated."; }
        else { await _service.CreateAsync(vm, userId); TempData["SuccessMessage"] = "Refund recorded."; }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(FeeRefundViewModel vm) => CreateEdit(vm);

    [HttpGet]
    [RequirePermission("FeeRefunds.Read")]
    public async Task<IActionResult> Details(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (_security.HasStudentRole(User) && !await _security.CanAccessRefundAsync(User, id))
            return Forbid();
        return View(new FeeRefundViewModel { Id = dto.Id, FeePaymentId = dto.FeePaymentId, RefundAmount = dto.RefundAmount, RefundMethod = dto.RefundMethod, ReferenceNo = dto.ReferenceNo, Reason = dto.Reason, IsApproved = dto.IsApproved, RefundDate = dto.RefundDate });
    }

    [HttpGet]
    [RequirePermission("FeeRefunds.Delete")]
    public async Task<IActionResult> Delete(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (_security.HasStudentRole(User) && !await _security.CanAccessRefundAsync(User, id))
            return Forbid();
        return View(new FeeRefundViewModel { Id = dto.Id, FeePaymentId = dto.FeePaymentId, RefundAmount = dto.RefundAmount, RefundMethod = dto.RefundMethod, ReferenceNo = dto.ReferenceNo, Reason = dto.Reason, IsApproved = dto.IsApproved, RefundDate = dto.RefundDate });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeRefunds.Approve")]
    public async Task<IActionResult> Approve(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.ApproveAsync(id, userId);
        TempData["SuccessMessage"] = "Refund approved.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeRefunds.Approve")]
    public async Task<IActionResult> Reject(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RejectAsync(id, userId);
        TempData["SuccessMessage"] = "Refund rejected.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeRefunds.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.DeleteAsync(id, userId);
        TempData["SuccessMessage"] = "Refund deleted.";
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("FeeRefunds.Delete")]
    public async Task<IActionResult> Restore(int id)
    {
        var dto = await _service.GetForEditAsync(id);
        if (dto == null) return NotFound();
        if (_security.HasStudentRole(User) && !await _security.CanAccessRefundAsync(User, id))
            return Forbid();
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        await _service.RestoreAsync(id, userId);
        TempData["SuccessMessage"] = "Refund restored successfully.";
        return RedirectToAction(nameof(Index));
    }
}
