using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Notification;

namespace SchoolManagementSystem.Controllers;

public class NotificationController : GenericCrudController<NotificationMessage>
{
    public NotificationController(SchoolDbContext db) : base(db, "Notification") { }
}
