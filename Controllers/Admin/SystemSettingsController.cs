using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.System;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Admin;

public class SystemSettingsController : GenericCrudController<SchoolProfile>
{
    public SystemSettingsController(IBaseService<SchoolProfile> service) : base(service, "System Settings") { }
}

