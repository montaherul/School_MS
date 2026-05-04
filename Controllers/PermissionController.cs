using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Controllers;

public class PermissionController : GenericCrudController<Permission>
{
    public PermissionController(SchoolDbContext db) : base(db, "Permission") { }
}
