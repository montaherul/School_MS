using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.System;

namespace SchoolManagementSystem.Controllers;

public class SystemSettingsController : GenericCrudController<SchoolProfile>
{
    public SystemSettingsController(SchoolDbContext db) : base(db, "System Settings") { }
}
