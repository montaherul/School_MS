using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.Health;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Health;

public class HealthController : GenericCrudController<MedicalRecord>
{
    public HealthController(IBaseService<MedicalRecord> service) : base(service, "Medical Record") { }
}

