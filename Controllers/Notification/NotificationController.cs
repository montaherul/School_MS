using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.Notification;
using SchoolManagementSystem.Services.Interfaces.Base;

namespace SchoolManagementSystem.Controllers.Notification;

public class NotificationController : GenericCrudController<NotificationMessage>
{
    public NotificationController(IBaseService<NotificationMessage> service) : base(service, "Notification") { }
}

