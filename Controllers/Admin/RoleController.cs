using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Services.Interfaces.Admin;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Admin;

public class RoleController : GenericCrudController<Role>
{
    private readonly IRoleService _roleService;

    public RoleController(IBaseService<Role> service, IRoleService roleService) : base(service, "Role")
    {
        _roleService = roleService;
    }

    public override async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken ct = default)
    {
        var result = await _roleService.GetPagedAsync(page, pageSize, search, ct);

        if (Request.Headers["Accept"].ToString().Contains("application/json") || Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Query.ContainsKey("page"))
        {
            return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / pageSize), total_records = result.TotalItems });
        }

        return View();
    }
}

