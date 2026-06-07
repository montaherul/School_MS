using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Auth;

namespace SchoolManagementSystem.Data;

/// <summary>
/// Runtime safety net for the Guardian RBAC model.
///
/// Even if a future migration or manual database change grants the Guardian
/// role more than the 9 portal-only permissions, this enforcer will detect
/// and remove the extras on every application startup. It runs in
/// <c>Program.cs</c> immediately after the database migration / seed step.
///
/// Single source of truth: <see cref="DbInitializer.GuardianPermissionCodes"/>.
/// </summary>
public static class GuardianRbacEnforcer
{
    /// <summary>
    /// Audit + repair the Guardian role's permission set.
    /// </summary>
    /// <returns>
    /// A tuple (wasCompliant, removedCount, addedCount). If <c>wasCompliant</c>
    /// is <c>true</c>, the role was already correctly configured.
    /// </returns>
    public static async Task<(bool WasCompliant, int RemovedCount, int AddedCount)> EnforceAsync(
        SchoolDbContext db,
        ILogger logger,
        CancellationToken cancellationToken = default)
    {
        var guardianRole = await db.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => !r.IsDeleted && r.Id == DbInitializer.GuardianRoleId, cancellationToken);

        if (guardianRole is null)
        {
            logger.LogWarning("GuardianRbacEnforcer: Guardian role (Id={RoleId}) not found. Skipping enforcement.", DbInitializer.GuardianRoleId);
            return (true, 0, 0);
        }

        // 1) Resolve target permission IDs
        var targetCodes = DbInitializer.GuardianPermissionCodes;
        var targetPermissionIds = await db.Permissions
            .AsNoTracking()
            .Where(p => targetCodes.Contains(p.Code))
            .Select(p => p.Id)
            .ToListAsync(cancellationToken);

        if (targetPermissionIds.Count != targetCodes.Count)
        {
            var foundCodes = await db.Permissions
                .AsNoTracking()
                .Where(p => targetCodes.Contains(p.Code))
                .Select(p => p.Code)
                .ToListAsync(cancellationToken);
            var missing = targetCodes.Except(foundCodes, StringComparer.Ordinal).ToList();
            logger.LogError("GuardianRbacEnforcer: missing permission rows in DB for codes: {Missing}", string.Join(", ", missing));
        }

        // 2) Read current role-permissions for the Guardian role
        var currentIds = await db.RolePermissions
            .AsNoTracking()
            .Where(rp => rp.RoleId == DbInitializer.GuardianRoleId)
            .Select(rp => rp.PermissionId)
            .ToListAsync(cancellationToken);

        // 3) Compute the diff
        var targetSet = new HashSet<int>(targetPermissionIds);
        var currentSet = new HashSet<int>(currentIds);

        var toRemove = currentSet.Except(targetSet).ToList();
        var toAdd = targetSet.Except(currentSet).ToList();

        if (toRemove.Count == 0 && toAdd.Count == 0)
        {
            logger.LogInformation("GuardianRbacEnforcer: Guardian role RBAC is compliant ({Count} permissions).", currentIds.Count);
            return (true, 0, 0);
        }

        // 4) Apply
        if (toRemove.Count > 0)
        {
            var rows = await db.RolePermissions
                .Where(rp => rp.RoleId == DbInitializer.GuardianRoleId && toRemove.Contains(rp.PermissionId))
                .ToListAsync(cancellationToken);
            db.RolePermissions.RemoveRange(rows);
        }

        foreach (var pid in toAdd)
        {
            db.RolePermissions.Add(new RolePermission
            {
                RoleId = DbInitializer.GuardianRoleId,
                PermissionId = pid
            });
        }

        await db.SaveChangesAsync(cancellationToken);

        logger.LogWarning(
            "GuardianRbacEnforcer: Guardian role RBAC was non-compliant. Removed {Removed} excessive permissions, added {Added} missing ones. Final count: {Final}.",
            toRemove.Count, toAdd.Count, targetSet.Count);

        return (false, toRemove.Count, toAdd.Count);
    }
}
