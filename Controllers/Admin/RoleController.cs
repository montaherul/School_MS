using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Admin;

[RequirePermission("Role.Manage")]
public class RoleController : GenericCrudController<Role>
{
    private readonly IRoleService _roleService;

    public RoleController(IBaseService<Role> service, IRoleService roleService) : base(service, "Role")
    {
        _roleService = roleService;
    }

    public override async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken ct = default)
    {
        var sortColumn = Request.Query["sort[0][column]"].FirstOrDefault();
        var sortDirection = Request.Query["sort[0][dir]"].FirstOrDefault();
        var result = await _roleService.GetPagedAsync(page, pageSize, search, sortColumn, sortDirection, ct);

        if (Request.Headers["Accept"].ToString().Contains("application/json") || Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Query.ContainsKey("page"))
        {
            return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / pageSize), total_records = result.TotalItems });
        }

        return View();
    }

    [HttpGet("AllPermissions")]
    public async Task<IActionResult> GetAllPermissions(CancellationToken ct)
    {
        var permissions = await _roleService.GetAllPermissionsAsync(ct);
        return Ok(permissions);
    }

    [HttpGet("{id}/Permissions")]
    public async Task<IActionResult> GetPermissions(int id, CancellationToken ct)
    {
        var permissions = await _roleService.GetPermissionsByRoleIdAsync(id, ct);
        return Ok(permissions);
    }

    [HttpPost("{id}/Permissions")]
    [RequirePermission("Roles.AssignPermissions")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AssignPermissions(int id, [FromBody] List<int> permissionIds, CancellationToken ct)
    {
        try
        {
            var result = await _roleService.AssignPermissionsToRoleAsync(id, permissionIds, ct);
            return Ok(new { success = result });
        }
        catch (Exception ex)
        {
            return BadRequest(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken = default)
    {
        try
        {
            await _roleService.DeleteAsync(id);
            TempData["SuccessMessage"] = "Role deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}

