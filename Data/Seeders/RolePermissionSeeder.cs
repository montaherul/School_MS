using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Constants;

namespace SchoolManagementSystem.Data.Seeders;

public class RolePermissionSeeder : IDataSeeder
{
    public int Order => 1;
    public string Name => "RolePermissionSeeder";

    private readonly SchoolDbContext _db;
    private readonly ILogger<RolePermissionSeeder> _logger;

    public RolePermissionSeeder(SchoolDbContext db, ILogger<RolePermissionSeeder> logger)
    {
        _db = db;
        _logger = logger;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        var roleData = new[]
        {
            new { Name = Roles.SuperAdmin, Priority = 100, IsSystem = true },
            new { Name = Roles.Admin, Priority = 95, IsSystem = true },
            new { Name = Roles.Principal, Priority = 90, IsSystem = true },
            new { Name = Roles.HRManager, Priority = 80, IsSystem = true },
            new { Name = Roles.Accountant, Priority = 70, IsSystem = true },
            new { Name = Roles.Teacher, Priority = 60, IsSystem = true },
            new { Name = Roles.Librarian, Priority = 50, IsSystem = true },
            new { Name = Roles.Staff, Priority = 40, IsSystem = true },
            new { Name = Roles.Student, Priority = 10, IsSystem = true },
            new { Name = Roles.Parent, Priority = 5, IsSystem = true }
        };
        
        foreach (var r in roleData)
        {
            var role = await _db.Set<Role>().FirstOrDefaultAsync(x => x.Name == r.Name, ct);
            if (role == null)
            {
                role = new Role 
                { 
                    Name = r.Name, 
                    Description = $"{r.Name} Role",
                    Priority = r.Priority,
                    IsSystemRole = r.IsSystem,
                    IsActive = true,
                    CreatedBy = "system",
                    CreatedAt = DateTime.UtcNow
                };
                await _db.Set<Role>().AddAsync(role, ct);
            }
            else
            {
                role.Priority = r.Priority;
                role.IsSystemRole = r.IsSystem;
            }
        }
        await _db.SaveChangesAsync(ct);

        // Get all permissions from constants
        var permissionCodes = GetAllPermissionCodes();
        var existingPermissions = await _db.Set<Permission>().ToDictionaryAsync(p => p.Code, p => p.Id, ct);

        foreach (var code in permissionCodes)
        {
            if (!existingPermissions.ContainsKey(code))
            {
                var parts = code.Split('.');
                var module = parts.Length > 0 ? parts[0] : "General";
                var action = parts.Length > 1 ? parts[1] : code;

                var p = new Permission
                {
                    Code = code,
                    Module = module,
                    ModuleName = module,
                    Action = action,
                    Category = module,
                    CreatedBy = "system",
                    CreatedAt = DateTime.UtcNow
                };
                await _db.Set<Permission>().AddAsync(p, ct);
            }
        }
        await _db.SaveChangesAsync(ct);

        // Assign all to Super Admin
        var superAdminRole = await _db.Set<Role>().FirstAsync(r => r.Name == Roles.SuperAdmin, ct);
        var allPermissionIds = await _db.Set<Permission>().Select(p => p.Id).ToListAsync(ct);
        var existingRolePerms = await _db.Set<RolePermission>()
            .Where(rp => rp.RoleId == superAdminRole.Id)
            .Select(rp => rp.PermissionId)
            .ToListAsync(ct);

        var missingPermIds = allPermissionIds.Except(existingRolePerms).ToList();
        foreach (var pId in missingPermIds)
        {
            await _db.Set<RolePermission>().AddAsync(new RolePermission 
            { 
                RoleId = superAdminRole.Id, 
                PermissionId = pId 
            }, ct);
        }
        
        if (missingPermIds.Any())
        {
            await _db.SaveChangesAsync(ct);
            _logger.LogInformation("Seeded {Count} new permissions to Super Admin", missingPermIds.Count);
        }
    }

    private List<string> GetAllPermissionCodes()
    {
        var codes = new List<string>();
        var permissionType = typeof(Permissions);
        var nestedTypes = permissionType.GetNestedTypes();

        foreach (var type in nestedTypes)
        {
            var fields = type.GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.FlattenHierarchy);
            foreach (var field in fields)
            {
                var value = field.GetValue(null)?.ToString();
                if (!string.IsNullOrEmpty(value)) codes.Add(value);
            }
        }
        return codes;
    }
}
