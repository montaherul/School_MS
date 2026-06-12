using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Admin;

[Authorize(Roles = "Admin,Super Admin")]
public class PermissionController : GenericCrudController<Permission>
{
    public PermissionController(IBaseService<Permission> service) : base(service, "Permission") { }
}

