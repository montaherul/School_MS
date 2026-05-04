using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Controllers;

[Authorize(Roles = "Super Admin,Principal,Assistant Head,Senior Lecturer,Lecturer")]
public class ResultController : GenericCrudController<MarkEntry>
{
    private readonly SchoolDbContext _db;
    public ResultController(SchoolDbContext db) : base(db, "Result / Marks") { _db = db; }

    protected override IQueryable<MarkEntry> ApplySecurityFilters(IQueryable<MarkEntry> query)
    {
        if (User.IsInRole("Student"))
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                // In a real scenario, you'd cache the StudentId or use a claim
                // Here we join or subquery to filter by the Student linked to this User
                var student = _db.Students.FirstOrDefault(s => s.UserId == userId && !s.IsDeleted);
                if (student != null)
                {
                    return query.Where(m => m.StudentId == student.Id);
                }
                return query.Where(m => false); // No student linked? No data.
            }
        }
        return query;
    }
}
