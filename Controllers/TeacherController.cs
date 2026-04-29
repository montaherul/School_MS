using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;

namespace SchoolManagementSystem.Controllers;

public class TeacherController : GenericCrudController<TeacherProfile>
{
    public TeacherController(SchoolDbContext db) : base(db, "Teacher") { }
}
