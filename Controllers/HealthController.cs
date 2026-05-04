using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Health;

namespace SchoolManagementSystem.Controllers;

public class HealthController : GenericCrudController<MedicalRecord>
{
    public HealthController(SchoolDbContext db) : base(db, "Health Record") { }
}
