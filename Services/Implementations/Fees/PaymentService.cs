using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Services.Implementations.Base;
using SchoolManagementSystem.Services.Interfaces.Fees;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Services.Implementations.Fees;

public class PaymentService : BaseService<Payment>, IPaymentService
{
    public PaymentService(IUnitOfWork unitOfWork) : base(unitOfWork)
    {
    }

    protected override IQueryable<Payment> ApplySecurityFilters(IQueryable<Payment> query, ClaimsPrincipal user)
    {
        return GetPaymentsForUser(query, user);
    }

    public IQueryable<Payment> GetPaymentsForUser(IQueryable<Payment> query, ClaimsPrincipal user)
    {
        if (user.IsInRole("Student"))
        {
            var userIdStr = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                var student = _unitOfWork.Repository<Student>().Query()
                    .AsNoTracking()
                    .FirstOrDefault(s => s.UserId == userId && !s.IsDeleted);

                if (student != null)
                {
                    return query.Where(p => _unitOfWork.Repository<FeeInvoice>().Query()
                        .Any(i => i.Id == p.FeeInvoiceId && i.StudentId == student.Id));
                }
                return query.Where(p => false);
            }
        }
        return query;
    }
}

