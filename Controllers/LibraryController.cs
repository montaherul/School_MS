using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Library;

namespace SchoolManagementSystem.Controllers;

public class LibraryController : GenericCrudController<Book>
{
    public LibraryController(SchoolDbContext db) : base(db, "Library Book") { }
}
