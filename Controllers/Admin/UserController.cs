using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.User;
using SchoolManagementSystem.Services.Interfaces.Admin;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Admin;

[Authorize]
public class UserController : Controller
{
    private readonly IUserService _userService;

    public UserController(IUserService userService)
    {
        _userService = userService;
    }

    [RequirePermission("Users.View")]
    public async Task<IActionResult> Index(
     int pageNumber = 1,
     int pageSize = 10,
     string? searchTerm = null,
     int? status = null,
     string? role = null,
     string? userType = null,
     CancellationToken ct = default)
    {
        // Tabulator sort params: sort[0][column]=UserName&sort[0][dir]=asc
        var sortColumn = Request.Query["sort[0][column]"].FirstOrDefault();
        var sortDirection = Request.Query["sort[0][dir]"].FirstOrDefault();

        var result = await _userService.GetPagedAsync(pageNumber, pageSize, searchTerm, status, role, userType, sortColumn, sortDirection, ct);

        // TABULATOR AJAX REQUEST
        if (Request.Headers["X-Requested-With"] == "XMLHttpRequest"
            || Request.Headers["Accept"].ToString().Contains("application/json"))
        {
            return Json(new
            {
                data = result.Items,
                last_page = (int)Math.Ceiling(result.TotalItems / (double)pageSize),
                current_page = pageNumber,
                total_records = result.TotalItems
            });
        }

        // NORMAL PAGE LOAD
        var model = new UserIndexViewModel
        {
            Items = result.Items,

            Page = pageNumber,

            PageSize = pageSize,

            TotalItems = result.TotalItems,

            Search = searchTerm
        };

        return View(model);
    }

    [RequirePermission("Users.Create")]
    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var roles = await _userService.GetAvailableRolesAsync(ct);
        return View("CreateEdit", new UserUpsertViewModel { Status = AccountStatus.Active, AvailableRoles = roles.ToList() });
    }

    [RequirePermission("Users.Edit")]
    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var model = await _userService.GetForEditAsync(id, ct);
        if (model == null) return NotFound();

        model.AvailableRoles = (await _userService.GetAvailableRolesAsync(ct)).ToList();
        return View("CreateEdit", model);
    }

    [RequirePermission("Users.Create")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(UserUpsertViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableRoles = (await _userService.GetAvailableRolesAsync(ct)).ToList();
            return View("CreateEdit", model);
        }

        try
        {
            await _userService.CreateAsync(model, User.Identity?.Name ?? "system", ct);
            TempData["SuccessMessage"] = "User created successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.AvailableRoles = (await _userService.GetAvailableRolesAsync(ct)).ToList();
            return View("CreateEdit", model);
        }
    }

    [RequirePermission("Users.Edit")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(UserUpsertViewModel model, CancellationToken ct)
    {
        if (!ModelState.IsValid)
        {
            model.AvailableRoles = (await _userService.GetAvailableRolesAsync(ct)).ToList();
            return View("CreateEdit", model);
        }

        try
        {
            await _userService.UpdateAsync(model, User.Identity?.Name ?? "system", ct);
            TempData["SuccessMessage"] = "User updated successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            model.AvailableRoles = (await _userService.GetAvailableRolesAsync(ct)).ToList();
            return View("CreateEdit", model);
        }
    }

    [RequirePermission("Users.View")]
    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var model = await _userService.GetDetailsAsync(id, ct);
        if (model == null) return NotFound();
        return View(model);
    }

    [RequirePermission("Users.Delete")]
    [HttpGet]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var model = await _userService.GetDetailsAsync(id, ct);
        if (model == null) return NotFound();
        return View(model);
    }

    [RequirePermission("Users.Delete")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        var user = await _userService.GetDetailsAsync(id, ct);
        if (user == null) return NotFound();

        try
        {
            await _userService.DeleteAsync(id, User.Identity?.Name ?? "system", ct);
            TempData["SuccessMessage"] = "User deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            return View("Delete", user);
        }
    }

    [RequirePermission("Users.Assign")]
    [HttpGet]
    public async Task<IActionResult> AssignRoles(int id, CancellationToken ct)
    {
        var user = await _userService.GetDetailsAsync(id, ct);
        if (user == null) return NotFound();

        var roles = await _userService.GetAvailableRolesAsync(ct);
        var selectedRoleIds = await _userService.GetAssignedRoleIdsAsync(id, ct);

        return View(new AssignRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            SelectedRoleIds = selectedRoleIds,
            AvailableRoles = roles.ToList()
        });
    }

    [RequirePermission("Users.Assign")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRoles(int id, List<int> selectedRoleIds, CancellationToken ct)
    {
        var user = await _userService.GetDetailsAsync(id, ct);
        if (user == null) return NotFound();

        try
        {
            var performedByUserId = int.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var uid) ? uid : (int?)null;
            await _userService.AssignRolesAsync(id, selectedRoleIds, performedByUserId, ct);
            TempData["SuccessMessage"] = "Roles updated successfully.";
            return RedirectToAction(nameof(Details), new { id });
        }
        catch (Exception ex)
        {
            ModelState.AddModelError("", ex.Message);
            var roles = await _userService.GetAvailableRolesAsync(ct);
            return View(new AssignRolesViewModel
            {
                UserId = id,
                UserName = user.UserName,
                SelectedRoleIds = selectedRoleIds,
                AvailableRoles = roles.ToList()
            });
        }
    }
}

