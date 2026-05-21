using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.User;
using SchoolManagementSystem.Services.Interfaces.Admin;

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
     CancellationToken ct = default)
    {
        var result = await _userService.GetPagedAsync( pageNumber, pageSize,searchTerm,status,role,ct);

        // TABULATOR AJAX REQUEST
        if (Request.Headers["Accept"].ToString().Contains("application/json")
            || Request.Headers["X-Requested-With"] == "XMLHttpRequest"
            || Request.Query.ContainsKey("pageNumber"))
        {
            return Json(new
            {
                data = result.Items,

                last_page = (int)Math.Ceiling(
                    result.TotalItems / (double)pageSize),

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
        await _userService.DeleteAsync(id, User.Identity?.Name ?? "system", ct);
        TempData["SuccessMessage"] = "User deleted successfully.";
        return RedirectToAction(nameof(Index));
    }

    [RequirePermission("Users.Assign")]
    [HttpGet]
    public async Task<IActionResult> AssignRoles(int id, CancellationToken ct)
    {
        var user = await _userService.GetDetailsAsync(id, ct);
        if (user == null) return NotFound();

        var roles = await _userService.GetAvailableRolesAsync(ct);
        var selectedRoleIds = await _userService.GetForEditAsync(id, ct); // To get selected roles

        return View(new AssignRolesViewModel
        {
            UserId = user.Id,
            UserName = user.UserName,
            SelectedRoleIds = selectedRoleIds?.SelectedRoleIds ?? new List<int>(),
            AvailableRoles = roles.ToList()
        });
    }

    [RequirePermission("Users.Assign")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignRoles(int id, List<int> selectedRoleIds, CancellationToken ct)
    {
        await _userService.AssignRolesAsync(id, selectedRoleIds, ct);
        TempData["SuccessMessage"] = "Roles updated successfully.";
        return RedirectToAction(nameof(Details), new { id = id });
    }
}

