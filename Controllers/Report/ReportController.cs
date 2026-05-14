using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Services.Interfaces.Base;
using AuditLogEntity = SchoolManagementSystem.Models.Entities.Auth.AuditLog;

namespace SchoolManagementSystem.Controllers.Report;

public class ReportController : GenericCrudController<AuditLogEntity>
{
    public ReportController(IBaseService<AuditLogEntity> service) : base(service, "Audit Log") { }
}


