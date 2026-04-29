using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Transport;

namespace SchoolManagementSystem.Controllers;

public class TransportController : GenericCrudController<TransportRoute>
{
    public TransportController(SchoolDbContext db) : base(db, "Transport Route") { }
}
