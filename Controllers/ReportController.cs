using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Controllers;

public class ReportController : GenericCrudController<AuditLog>
{
    public ReportController(SchoolDbContext db) : base(db, "Report / Audit Log") { }
}
