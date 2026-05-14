using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.Library;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Library;

public class LibraryController : GenericCrudController<Book>
{
    public LibraryController(IBaseService<Book> service) : base(service, "Library Book") { }
}

