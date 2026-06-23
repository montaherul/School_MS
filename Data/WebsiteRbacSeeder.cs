using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Data;

public static class WebsiteRbacSeeder
{
    private static readonly string[] WebsitePermissions =
    [
        "Website.View",
        "Website.Edit",
        "Website.Notices",
        "Website.Events",
        "Website.Gallery",
        "Website.Pages",
        "Website.EmailTemplates",
        "Website.AdmissionFees"
    ];

    private static readonly string[] GrantedRoles = ["Super Admin", "Admin", "Principal"];

    public static async Task SeedAsync(SchoolDbContext db, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var existingCodes = (await db.Permissions.Select(p => p.Code).ToListAsync(cancellationToken)).ToHashSet();

        foreach (var code in WebsitePermissions)
        {
            if (existingCodes.Contains(code))
                continue;

            var parts = code.Split('.');
            var module = parts[0];
            var action = parts[1];

            db.Permissions.Add(new Permission
            {
                Module = module,
                ModuleName = module,
                Action = action,
                Code = code,
                CanCreate = action is "Edit" or "Create",
                CanRead = true,
                CanUpdate = action is "Edit",
                CanDelete = false,
                CreatedAt = now,
                CreatedBy = "website-rbac-seeder"
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        await GrantToRolesAsync(db, WebsitePermissions, GrantedRoles, cancellationToken);
        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task GrantToRolesAsync(SchoolDbContext db, string[] codes, string[] roleNames, CancellationToken cancellationToken)
    {
        var roles = await db.Roles.Where(r => roleNames.Contains(r.Name)).ToListAsync(cancellationToken);
        if (roles.Count == 0) return;

        var permissionIds = await db.Permissions
            .Where(p => codes.Contains(p.Code))
            .Select(p => p.Id)
            .ToArrayAsync(cancellationToken);

        var roleIds = roles.Select(r => r.Id).ToArray();
        var existingByRole = await db.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId) && permissionIds.Contains(rp.PermissionId))
            .GroupBy(rp => rp.RoleId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(rp => rp.PermissionId).ToHashSet(), cancellationToken);

        foreach (var role in roles)
        {
            var existing = existingByRole.GetValueOrDefault(role.Id, []);
            foreach (var permissionId in permissionIds)
            {
                if (!existing.Contains(permissionId))
                {
                    db.RolePermissions.Add(new RolePermission
                    {
                        RoleId = role.Id,
                        PermissionId = permissionId
                    });
                }
            }
        }
    }
}
