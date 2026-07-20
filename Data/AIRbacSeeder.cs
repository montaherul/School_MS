using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Data;

public static class AIRbacSeeder
{
    private static readonly string[] AiPermissionCodes =
    [
        "AI.View", "AI.Manage",
        "AI.Chat", "AI.Quiz", "AI.Homework", "AI.RAG", "AI.Export",
        "AI.Analytics", "AI.Configuration", "AI.Prompts", "AI.Models", "AI.Security"
    ];

    private static readonly int[] TargetRoleIds = [1, 2, 27]; // SuperAdmin, Admin, Student

    public static async Task SeedAsync(SchoolDbContext db, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var existingCodes = await db.Permissions
            .Where(p => AiPermissionCodes.Contains(p.Code))
            .Select(p => p.Code)
            .ToListAsync(cancellationToken);

        var missingCodes = AiPermissionCodes.Except(existingCodes).ToArray();

        foreach (var code in missingCodes)
        {
            var parts = code.Split('.');
            db.Permissions.Add(new Permission
            {
                Module = parts[0],
                ModuleName = "AI",
                Action = parts.Length > 1 ? parts[1] : "View",
                Code = code,
                CanRead = true,
                CreatedAt = now
            });
        }

        if (missingCodes.Length > 0)
            await db.SaveChangesAsync(cancellationToken);

        var permissionIds = await db.Permissions
            .Where(p => AiPermissionCodes.Contains(p.Code))
            .Select(p => new { p.Id, p.Code })
            .ToListAsync(cancellationToken);

        foreach (var roleId in TargetRoleIds)
        {
            var existingRolePerms = await db.RolePermissions
                .Where(rp => rp.RoleId == roleId && permissionIds.Select(x => x.Id).Contains(rp.PermissionId))
                .Select(rp => rp.PermissionId)
                .ToListAsync(cancellationToken);

            var newPerms = permissionIds.Where(p => !existingRolePerms.Contains(p.Id)).ToList();

            foreach (var perm in newPerms)
            {
                db.RolePermissions.Add(new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = perm.Id
                });
            }
        }

        await db.SaveChangesAsync(cancellationToken);
    }
}
