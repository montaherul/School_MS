using Microsoft.AspNetCore.Authorization;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Fees;

namespace SchoolManagementSystem.Controllers.Fees;

public class PaymentController : GenericCrudController<Payment>
{
    public PaymentController(IPaymentService service) : base(service, "Payment") { }

    protected override IQueryable<Payment> ApplySecurityFilters(IQueryable<Payment> query)
    {
        return ((IPaymentService)_service).GetPaymentsForUser(query, User);
    }
}

