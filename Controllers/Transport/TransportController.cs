using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.Transport;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Transport;

public class TransportController : GenericCrudController<TransportRoute>
{
    public TransportController(IBaseService<TransportRoute> service) : base(service, "Transport Route") { }
}

