using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Communication;

namespace SchoolManagementSystem.Controllers;

public class CommunicationController : GenericCrudController<Notice>
{
    public CommunicationController(SchoolDbContext db) : base(db, "Notice / Communication") { }
}
