using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Services.Interfaces.Base;
using System.Security.Claims;

namespace SchoolManagementSystem.Services.Interfaces.Fees;

public interface IPaymentService : IBaseService<Payment>
{
    IQueryable<Payment> GetPaymentsForUser(IQueryable<Payment> query, ClaimsPrincipal user);
}

