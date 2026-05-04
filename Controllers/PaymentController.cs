using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Fees;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers;

public class PaymentController : GenericCrudController<Payment>
{
    private readonly SchoolDbContext _db;
    public PaymentController(SchoolDbContext db) : base(db, "Payment") { _db = db; }

    protected override IQueryable<Payment> ApplySecurityFilters(IQueryable<Payment> query)
    {
        if (User.IsInRole("Student"))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                var student = _db.Students.AsNoTracking().FirstOrDefault(s => s.UserId == userId && !s.IsDeleted);
                if (student != null)
                {
                    return query.Where(p => _db.FeeInvoices.Any(i => i.Id == p.FeeInvoiceId && i.StudentId == student.Id));
                }
                return query.Where(p => false);
            }
        }
        return query;
    }
}
