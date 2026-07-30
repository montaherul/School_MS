using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Data;

public static class SchoolPayRbacSeeder
{
    private static readonly (string Code, string Action)[] Permissions = new[]
    {
        ("SchoolPay.ViewTransactions", "Read")
    };

    public static async Task SeedAsync(SchoolDbContext db)
    {
        var existing = await db.Permissions
            .Where(p => p.Module == "SchoolPay")
            .Select(p => p.Code)
            .ToListAsync();

        var toCreate = Permissions
            .Where(p => !existing.Contains(p.Code))
            .Select(p => new Permission
            {
                Module = "SchoolPay",
                ModuleName = "SSLCommerz Gateway",
                Action = p.Action,
                Code = p.Code,
                CanCreate = false,
                CanRead = true,
                CanUpdate = false,
                CanDelete = false
            })
            .ToList();

        if (toCreate.Count > 0)
        {
            db.Permissions.AddRange(toCreate);
            await db.SaveChangesAsync();
        }

        var permIds = await db.Permissions
            .Where(p => p.Module == "SchoolPay" && p.Code == "SchoolPay.ViewTransactions")
            .Select(p => p.Id)
            .ToListAsync();

        var adminRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Super Admin");
        var principalRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Principal");
        var accountantRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Accountant");
        var adminAppRole = await db.Roles.FirstOrDefaultAsync(r => r.Name == "Admin");

        var targetRoles = new[] { adminRole, principalRole, accountantRole, adminAppRole }
            .Where(r => r != null)
            .Select(r => r!.Id)
            .Distinct()
            .ToList();

        foreach (var roleId in targetRoles)
        {
            var existingRolePerms = await db.RolePermissions
                .Where(rp => rp.RoleId == roleId)
                .Select(rp => rp.PermissionId)
                .ToListAsync();

            var toAssign = permIds
                .Where(p => !existingRolePerms.Contains(p))
                .Select(p => new RolePermission
                {
                    RoleId = roleId,
                    PermissionId = p
                })
                .ToList();

            if (toAssign.Count > 0)
            {
                db.RolePermissions.AddRange(toAssign);
            }
        }

        await db.SaveChangesAsync();
    }
}
