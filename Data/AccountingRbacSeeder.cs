using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Data;

public static class AccountingRbacSeeder
{
    private static readonly string[] AccountingPermissions =
    [
        "Accounting.View",
        "Accounting.Post",
        "Accounting.ClosePeriod",
        "Accounting.Reconcile",
        "Accounting.Export"
    ];

    private static readonly string[] FullAccessRoles = ["Super Admin", "Admin", "Accountant"];

    public static async Task SeedAsync(SchoolDbContext db, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var existingCodes = await db.Permissions
            .Where(p => AccountingPermissions.Contains(p.Code))
            .Select(p => p.Code)
            .ToListAsync(cancellationToken);

        var missingCodes = AccountingPermissions.Except(existingCodes).ToArray();

        foreach (var code in missingCodes)
        {
            var parts = code.Split('.');
            db.Permissions.Add(new Permission
            {
                Module = "Accounting",
                ModuleName = "Accounting",
                Action = parts[1],
                Code = code,
                CanRead = parts[1] is "View" or "Export",
                CanCreate = parts[1] is "Post",
                CanUpdate = parts[1] is "ClosePeriod" or "Reconcile",
                CanDelete = false,
                CreatedAt = now,
                CreatedBy = "accounting-rbac-seeder"
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        var allPermissionIds = await db.Permissions
            .Where(p => AccountingPermissions.Contains(p.Code))
            .Select(p => p.Id)
            .ToArrayAsync(cancellationToken);

        var roles = await db.Roles
            .Where(r => FullAccessRoles.Contains(r.Name))
            .ToListAsync(cancellationToken);

        foreach (var role in roles)
        {
            var existing = (await db.RolePermissions
                .Where(rp => rp.RoleId == role.Id && allPermissionIds.Contains(rp.PermissionId))
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken)).ToHashSet();

            foreach (var pid in allPermissionIds)
            {
                if (!existing.Contains(pid))
                {
                    db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = pid
                    });
                }
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
