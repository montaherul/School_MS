using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Assignment;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;

namespace SchoolManagementSystem.Controllers;

[Authorize(Roles = "Super Admin,Principal,Assistant Head,Senior Lecturer,Lecturer")]
public class StudentClassAssignmentController : GenericCrudController<AssignmentTask>
{
    private readonly SchoolDbContext _db;
    public StudentClassAssignmentController(SchoolDbContext db) : base(db, "StudentClassAssignment") { _db = db; }

    protected override IQueryable<AssignmentTask> ApplySecurityFilters(IQueryable<AssignmentTask> query)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return query.Where(a => false);

        if (User.IsInRole("Student"))
        {
            var student = _db.Students.AsNoTracking().FirstOrDefault(s => s.UserId == userId && !s.IsDeleted);
            if (student != null)
            {
                return query.Where(a => a.SchoolClassId == student.ClassId && a.SectionId == student.SectionId);
            }
            return query.Where(a => false);
        }

        bool isTeacherLike = User.IsInRole("Teacher") || User.IsInRole("Senior Lecturer") || User.IsInRole("Lecturer");
        bool isAdminLike = User.IsInRole("Super Admin") || User.IsInRole("Principal") || User.IsInRole("Assistant Head");

        if (isTeacherLike && !isAdminLike)
        {
            var teacher = _db.Teachers.AsNoTracking().FirstOrDefault(t => t.UserId == userId && !t.IsDeleted);
            if (teacher != null)
            {
                // Teachers can see assignments for classes they are assigned to
                var assignedClassIds = _db.TeacherClassAssignments
                    .Where(a => a.TeacherId == teacher.Id && !a.IsDeleted)
                    .Select(a => a.ClassId)
                    .ToList();
                
                return query.Where(a => assignedClassIds.Contains(a.SchoolClassId));
            }
            return query.Where(a => false);
        }

        return query;
    }
}
