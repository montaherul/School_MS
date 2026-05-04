using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Exam;

namespace SchoolManagementSystem.Controllers;

public class ExamController : GenericCrudController<Exam>
{
    public ExamController(SchoolDbContext db) : base(db, "Exam") { }
}
