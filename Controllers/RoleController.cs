using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Controllers;

public class RoleController : GenericCrudController<Role>
{
    public RoleController(SchoolDbContext db) : base(db, "Role") { }
}
