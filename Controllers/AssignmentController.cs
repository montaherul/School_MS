using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Assignment;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Controllers;

[Authorize(Roles = "Super Admin,Principal,Assistant Head,Senior Lecturer,Lecturer")]
public class AssignmentController : GenericCrudController<AssignmentTask>
{
    private readonly SchoolDbContext _db;
    public AssignmentController(SchoolDbContext db) : base(db, "Assignment") { _db = db; }

    protected override IQueryable<AssignmentTask> ApplySecurityFilters(IQueryable<AssignmentTask> query)
    {
        if (User.IsInRole("Student"))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                var student = _db.Students.AsNoTracking().FirstOrDefault(s => s.UserId == userId && !s.IsDeleted);
                if (student != null)
                {
                    return query.Where(a => a.SchoolClassId == student.ClassId && a.SectionId == student.SectionId);
                }
                return query.Where(a => false);
            }
        }
        return query;
    }
}
