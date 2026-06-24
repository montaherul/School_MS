using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Admin;

[RequirePermission("Permission.Manage")]
public class PermissionController : GenericCrudController<Permission>
{
    public PermissionController(IBaseService<Permission> service) : base(service, "Permission") { }
}

