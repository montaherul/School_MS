using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;

namespace SchoolManagementSystem.Controllers;

public class ResultController : GenericCrudController<MarkEntry>
{
    public ResultController(SchoolDbContext db) : base(db, "Result / Marks") { }
}
