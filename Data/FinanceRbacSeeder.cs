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
        await EnsureRoleAsync(db, "Admin", "Administrator", now, cancellationToken);
        await EnsureRoleAsync(db, "Accountant", "Accounts and finance", now, cancellationToken);

        foreach (var module in FinanceModules)
        {
            foreach (var action in FinanceActions)
            {
                var code = $"{module}.{action}";
                if (await db.Permissions.AnyAsync(p => p.Code == code, cancellationToken))
                {
                    continue;
                }

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

        foreach (var roleName in FullFinanceRoles)
        {
            await GrantAsync(db, roleName, FinanceModules, FinanceActions, cancellationToken);
        }

        await GrantAsync(
            db,
            "Principal",
            ["FinanceDashboard", "FinanceReports", "Payments", "Invoices", "StudentDues", "Scholarships", "Waivers"],
            ["View", "Read", "Approve", "Export", "Print"],
            cancellationToken);

        await GrantAsync(
            db,
            "Student",
            ["Invoices", "Payments", "StudentDues", "Receipts"],
            ["View", "Read", "Print", "Export"],
            cancellationToken);

        await db.SaveChangesAsync(cancellationToken);
    }

    private static async Task EnsureRoleAsync(SchoolDbContext db, string name, string description, DateTime now, CancellationToken cancellationToken)
    {
        if (await db.Roles.AnyAsync(r => r.Name == name, cancellationToken))
        {
            return;
        }

        db.Roles.Add(new Role
        {
            Name = name,
            Description = description,
            CreatedAt = now,
            CreatedBy = "finance-rbac-seeder"
        });
    }

    private static async Task GrantAsync(SchoolDbContext db, string roleName, IEnumerable<string> modules, IEnumerable<string> actions, CancellationToken cancellationToken)
    {
        var role = await db.Roles.FirstOrDefaultAsync(r => r.Name == roleName, cancellationToken);
        if (role is null)
        {
            return;
        }

        var codes = modules.SelectMany(module => actions.Select(action => $"{module}.{action}")).ToArray();
        var permissionIds = await db.Permissions
            .Where(p => codes.Contains(p.Code))
            .Select(p => p.Id)
            .ToArrayAsync(cancellationToken);

        var existingPermissionIds = await db.RolePermissions
            .Where(rp => rp.RoleId == role.Id && permissionIds.Contains(rp.PermissionId))
            .Select(rp => rp.PermissionId)
            .ToArrayAsync(cancellationToken);

        foreach (var permissionId in permissionIds.Except(existingPermissionIds))
        {
            db.RolePermissions.Add(new RolePermission
            {
                RoleId = role.Id,
                PermissionId = permissionId
            });
        }
    }
}
