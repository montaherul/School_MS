using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Data;

public static class SchoolPayRbacSeeder
{
    private static readonly (string Code, string Action)[] Permissions = new[]
    {
        ("SchoolPay.Manage", "Manage"),
        ("SchoolPay.ViewTransactions", "Read"),
        ("SchoolPay.ProcessRefund", "Update"),
        ("SchoolPay.ViewSettlements", "Read"),
        ("SchoolPay.Reconcile", "Update"),
        ("SchoolPay.Analytics", "Read"),
        ("SchoolPay.Operations", "Read"),
        ("SchoolPay.Failover", "Read"),
        ("SchoolPay.Monitoring", "Read"),
        ("SchoolPay.Security", "Manage")
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
                ModuleName = "SchoolPay Gateway",
                Action = p.Action,
                Code = p.Code,
                CanCreate = p.Action == "Manage",
                CanRead = true,
                CanUpdate = p.Action == "Manage" || p.Action == "Update",
                CanDelete = p.Action == "Manage"
            })
            .ToList();

        if (toCreate.Count > 0)
        {
            db.Permissions.AddRange(toCreate);
            await db.SaveChangesAsync();
        }

        var allSchoolPayPerms = await db.Permissions
            .Where(p => p.Module == "SchoolPay")
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

            var toAssign = allSchoolPayPerms
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
