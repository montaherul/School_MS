using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Fees;

namespace SchoolManagementSystem.Controllers;

public class PaymentController : GenericCrudController<Payment>
{
    public PaymentController(SchoolDbContext db) : base(db, "Payment") { }
}
