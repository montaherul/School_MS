using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.System;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Admin;

[RequirePermission("Settings.Manage")]
public class SystemSettingsController : GenericCrudController<SchoolProfile>
{
    public SystemSettingsController(IBaseService<SchoolProfile> service) : base(service, "System Settings") { }
}

