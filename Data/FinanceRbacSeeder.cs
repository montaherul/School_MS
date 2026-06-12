using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Data;

public static class FinanceRbacSeeder
{
    private static readonly string[] FinanceModules =
    [
        "FeeStructures",
        "Invoices",
        "Payments",
        "Scholarships",
        "Waivers",
        "StudentDues",
        "FinancialTransactions",
        "FinanceReports",
        "FinanceConfiguration",
        "FinanceDashboard",
        "Receipts"
    ];

    private static readonly string[] FinanceActions =
    [
        "View",
        "Read",
        "Create",
        "Edit",
        "Update",
        "Delete",
        "Approve",
        "Export",
        "Print",
        "Generate",
        "Manage"
    ];

    private static readonly string[] FullFinanceRoles = ["Super Admin", "Admin", "Accountant"];

    public static async Task SeedAsync(SchoolDbContext db, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;

        var existingRoleNames = (await db.Roles.Select(r => r.Name).ToListAsync(cancellationToken)).ToHashSet();
        if (!existingRoleNames.Contains("Admin"))
            db.Roles.Add(new Role { Name = "Admin", Description = "Administrator", CreatedAt = now, CreatedBy = "finance-rbac-seeder" });
        if (!existingRoleNames.Contains("Accountant"))
            db.Roles.Add(new Role { Name = "Accountant", Description = "Accounts and finance", CreatedAt = now, CreatedBy = "finance-rbac-seeder" });

        var existingCodes = (await db.Permissions.Select(p => p.Code).ToListAsync(cancellationToken)).ToHashSet();

        foreach (var module in FinanceModules)
        {
            foreach (var action in FinanceActions)
            {
                var code = $"{module}.{action}";
                if (existingCodes.Contains(code))
                    continue;

                db.Permissions.Add(new Permission
                {
                    Module = module,
                    ModuleName = module,
                    Action = action,
                    Code = code,
                    CanCreate = action is "Create" or "Generate" or "Manage",
                    CanRead = action is "View" or "Read" or "Export" or "Print" or "Generate" or "Manage",
                    CanUpdate = action is "Edit" or "Update" or "Approve" or "Manage",
                    CanDelete = action is "Delete" or "Manage",
                    CreatedAt = now,
                    CreatedBy = "finance-rbac-seeder"
                });
            }
        }

        await db.SaveChangesAsync(cancellationToken);

        await GrantBatchAsync(db, FullFinanceRoles, FinanceModules, FinanceActions, cancellationToken);
        await GrantBatchAsync(db, ["Principal"],
            ["FinanceDashboard", "FinanceReports", "Payments", "Invoices", "StudentDues", "Scholarships", "Waivers"],
            ["View", "Read", "Approve", "Export", "Print"], cancellationToken);
        await GrantBatchAsync(db, ["Student"],
            ["Invoices", "Payments", "StudentDues", "Receipts"],
            ["View", "Read", "Print", "Export"], cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task GrantBatchAsync(SchoolDbContext db, string[] roleNames, string[] modules, string[] actions, CancellationToken cancellationToken)
    {
        var roles = await db.Roles.Where(r => roleNames.Contains(r.Name)).ToListAsync(cancellationToken);
        if (roles.Count == 0) return;

        var codes = modules.SelectMany(m => actions.Select(a => $"{m}.{a}")).ToArray();
        var allPermissionIds = await db.Permissions
            .Where(p => codes.Contains(p.Code))
            .Select(p => p.Id)
            .ToArrayAsync(cancellationToken);

        var roleIds = roles.Select(r => r.Id).ToArray();
        var existingByRole = await db.RolePermissions
            .Where(rp => roleIds.Contains(rp.RoleId) && allPermissionIds.Contains(rp.PermissionId))
            .GroupBy(rp => rp.RoleId)
            .ToDictionaryAsync(g => g.Key, g => g.Select(rp => rp.PermissionId).ToHashSet(), cancellationToken);

        foreach (var role in roles)
        {
            var existing = existingByRole.GetValueOrDefault(role.Id, []);
            foreach (var permissionId in allPermissionIds)
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
