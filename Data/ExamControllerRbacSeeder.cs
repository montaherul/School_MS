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
        "Reports.View", "Reports.Read",
        "ClassSubjectMappings.View",
        "CashierCollection.View"
    ];

    public static async Task SeedAsync(SchoolDbContext db, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

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

        role = await db.Roles.FirstAsync(r => r.Name == "Exam Controller", cancellationToken);

        // Ensure ClassSubjectMappings.View permission exists (missing from DbInitializer matrix)
        if (!await db.Permissions.AnyAsync(p => p.Code == "ClassSubjectMappings.View", cancellationToken))
        {
            db.Permissions.Add(new Permission
            {
                Module = "ClassSubjectMappings",
                ModuleName = "ClassSubjectMappings",
                Action = "View",
                Code = "ClassSubjectMappings.View",
                CanRead = true,
                CreatedAt = now
            });
            await db.SaveChangesAsync(cancellationToken);
        }

        var permission = await db.Permissions.FirstAsync(p => p.Code == "ClassSubjectMappings.View", cancellationToken);

        // Assign to Exam Controller role
        var examControllerCodes = await db.RolePermissions
            .Where(rp => rp.RoleId == role.Id && rp.Permission != null)
            .Select(rp => rp.Permission!.Code)
            .ToListAsync(cancellationToken);

        var missingExamControllerCodes = AllowedCodes.Except(examControllerCodes).ToArray();

        if (missingExamControllerCodes.Length > 0)
        {
            var permissionIds = await db.Permissions
                .Where(p => missingExamControllerCodes.Contains(p.Code))
                .Select(p => p.Id)
                .ToListAsync(cancellationToken);

            foreach (var permissionId1 in permissionIds)
            {
                db.RolePermissions.Add(new RolePermission
                {
                    RoleId = role.Id,
                    PermissionId = permissionId1
                });
            }
        }

        // Assign ClassSubjectMappings.View to Admin role (Id=26) since it needs it for exam wizard
        var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.Id == 26, cancellationToken);
        var adminAdded = false;
        if (adminRole != null)
        {
            var adminHasIt = await db.RolePermissions
                .AnyAsync(rp => rp.RoleId == 26 && rp.PermissionId == permission.Id, cancellationToken);
            if (!adminHasIt)
            {
                db.RolePermissions.Add(new RolePermission
                {
                    RoleId = 26,
                    PermissionId = permission.Id
                });
                adminAdded = true;
            }
        }

        if (missingExamControllerCodes.Length > 0 || adminAdded)
            await db.SaveChangesAsync(cancellationToken);
    }
}
