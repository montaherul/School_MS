using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Assignment;

namespace SchoolManagementSystem.Controllers;

public class AssignmentController : GenericCrudController<AssignmentTask>
{
    public AssignmentController(SchoolDbContext db) : base(db, "Assignment") { }
}
