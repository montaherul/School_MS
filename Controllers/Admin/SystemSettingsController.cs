using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.System;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Admin;

[Authorize(Roles = "Admin,Super Admin,Principal")]
public class SystemSettingsController : GenericCrudController<SchoolProfile>
{
    public SystemSettingsController(IBaseService<SchoolProfile> service) : base(service, "System Settings") { }
}

