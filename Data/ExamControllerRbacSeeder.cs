using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Data;

public static class ExamControllerRbacSeeder
{
    private static readonly string[] AllowedCodes =
    [
        "Dashboard.View", "Dashboard.Read",
        "Academic.View", "Academic.Read",
        "Classes.View", "Classes.Read",
        "Sections.View", "Sections.Read",
        "Subjects.View", "Subjects.Read",
        "Students.View", "Students.Read",
        "Student.View", "Student.Read",
        "Attendance.View",
        "Assignments.View",
        "Exams.View", "Exams.Read", "Exams.Create", "Exams.Edit",
        "Exam.View", "Exam.Read", "Exam.Create", "Exam.Edit",
        "Marks.View", "Marks.Read", "Marks.Create", "Marks.Edit", "Marks.Approve", "Marks.Publish",
        "Results.View", "Results.Read", "Results.Approve", "Results.Publish",
        "Result.View", "Result.Read",
        "Reports.View", "Reports.Read"
    ];

    public static async Task SeedAsync(SchoolDbContext db, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        // Ensure "Exam Controller" role exists
        var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Exam Controller", cancellationToken);
        if (role == null)
        {
            role = new Role
            {
                Name = "Exam Controller",
                Description = "Exam and result management operations",
                CreatedAt = now,
                CreatedBy = "exam-controller-rbac-seeder"
            };
            db.Roles.Add(role);
            await db.SaveChangesAsync(cancellationToken);
        }

        // Re-fetch to get Id after save
        role = await db.Roles.FirstAsync(r => r.Name == "Exam Controller", cancellationToken);

        var existingCodes = await db.RolePermissions
            .Where(rp => rp.RoleId == role.Id && rp.Permission != null)
            .Select(rp => rp.Permission!.Code)
            .ToListAsync(cancellationToken);

        var missingCodes = AllowedCodes.Except(existingCodes).ToArray();

        if (missingCodes.Length > 0)
        {
            var permissionIds = await db.Permissions
                .Where(p => missingCodes.Contains(p.Code))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            foreach (var permissionId in permissionIds)
            {
                db.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId
                });
            }

            await db.SaveChangesAsync(cancellationToken);
        }
    }
}
