using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace SchoolManagementSystem.Services.Implementations.Employee
{
    public class EmployeeModuleSeeder
    {
        private readonly SchoolDbContext _db;
        private readonly ILogger<EmployeeModuleSeeder> _logger;

        public EmployeeModuleSeeder(SchoolDbContext db, ILogger<EmployeeModuleSeeder> logger)
        {
            _db = db;
            _logger = logger;
        }

        private class LegacyTeacher
        {
            public int Id { get; set; }
            public string Name { get; set; } = string.Empty;
            public string Mobile { get; set; } = string.Empty;
            public string? Email { get; set; }
            public string Designation { get; set; } = string.Empty;
            public int? UserId { get; set; }
            public string? ProfilePic { get; set; }
        }

        public async Task SeedAsync()
        {
            try
            {
                _logger.LogInformation("Starting Employee workforce architecture schema verification...");

                // 1. Check if Employees table exists
                bool employeesTableExists = false;
                try
                {
                    // Query sys.tables directly to see if table exists
                    var conn = _db.Database.GetDbConnection();
                    if (conn.State != System.Data.ConnectionState.Open)
                        await conn.OpenAsync();

                    var tableNames = new List<string>();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT name FROM sys.tables ORDER BY name";
                        using (var r = cmd.ExecuteReader())
                        {
                            while (r.Read()) tableNames.Add(r.GetString(0));
                        }
                    }
                    _logger.LogInformation("Existing Tables in Database: {Tables}", string.Join(", ", tableNames));

                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name = 'Employees'";
                        var count = (int)(cmd.ExecuteScalar() ?? 0);
                        employeesTableExists = (count > 0);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogWarning("Error checking for Employees table, assuming it does not exist: {Message}", ex.Message);
                }

                if (!employeesTableExists)
                {
                    _logger.LogWarning("Workforce module database tables not found. Please run Entity Framework migrations first to generate the tables.");
                    return;
                }

                // 2. Seed Departments
                var conn2 = _db.Database.GetDbConnection();
                if (conn2.State != System.Data.ConnectionState.Open)
                    await conn2.OpenAsync();

                using (var cmd = conn2.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Departments WHERE IsDeleted = 0";
                    var depCount = (int)(cmd.ExecuteScalar() ?? 0);
                    if (depCount == 0)
                    {
                        _logger.LogInformation("Seeding default workforce departments...");
                        var seedDepsSql = @"
                            INSERT INTO Departments (Name, CreatedBy, CreatedAt, IsDeleted) VALUES 
                            ('Academic', 'seeder', GETDATE(), 0),
                            ('Administration', 'seeder', GETDATE(), 0),
                            ('Accounts & Finance', 'seeder', GETDATE(), 0),
                            ('Information Technology', 'seeder', GETDATE(), 0),
                            ('Library Services', 'seeder', GETDATE(), 0),
                            ('Security & Safety', 'seeder', GETDATE(), 0),
                            ('Support & Services', 'seeder', GETDATE(), 0);
                        ";
                        await _db.Database.ExecuteSqlRawAsync(seedDepsSql);
                    }
                }

                // 3. Seed Designations
                using (var cmd = conn2.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM Designations WHERE IsDeleted = 0";
                    var desCount = (int)(cmd.ExecuteScalar() ?? 0);
                    if (desCount == 0)
                    {
                        _logger.LogInformation("Seeding default workforce designations...");
                        var seedDesSql = @"
                            INSERT INTO Designations (Name, RoleLevel, IsTeachingRole, IsAdministrativeRole, RequiresLogin, IsActive, CreatedBy, CreatedAt, IsDeleted) VALUES 
                            ('Principal', 1, 1, 1, 1, 1, 'seeder', GETDATE(), 0),
                            ('Vice Principal', 2, 1, 1, 1, 1, 'seeder', GETDATE(), 0),
                            ('Assistant Head', 3, 1, 1, 1, 1, 'seeder', GETDATE(), 0),
                            ('Senior Teacher', 4, 1, 0, 1, 1, 'seeder', GETDATE(), 0),
                            ('Lecturer', 4, 1, 0, 1, 1, 'seeder', GETDATE(), 0),
                            ('Teacher', 5, 1, 0, 1, 1, 'seeder', GETDATE(), 0),
                            ('Assistant Teacher', 6, 1, 0, 1, 1, 'seeder', GETDATE(), 0),
                            ('Office Staff', 7, 0, 0, 1, 1, 'seeder', GETDATE(), 0),
                            ('Accountant', 7, 0, 1, 1, 1, 'seeder', GETDATE(), 0),
                            ('Librarian', 7, 0, 0, 1, 1, 'seeder', GETDATE(), 0),
                            ('Lab Assistant', 8, 0, 0, 1, 1, 'seeder', GETDATE(), 0),
                            ('Driver', 9, 0, 0, 0, 1, 'seeder', GETDATE(), 0),
                            ('Guard', 10, 0, 0, 0, 1, 'seeder', GETDATE(), 0),
                            ('Cleaner', 11, 0, 0, 0, 1, 'seeder', GETDATE(), 0),
                            ('Aya / Helper', 11, 0, 0, 0, 1, 'seeder', GETDATE(), 0);
                        ";
                        await _db.Database.ExecuteSqlRawAsync(seedDesSql);
                    }
                }

                // 4. Seed DesignationRoleMappings
                using (var cmd = conn2.CreateCommand())
                {
                    cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE name = 'DesignationRoleMappings'";
                    var tableExists = (int)(cmd.ExecuteScalar() ?? 0) > 0;

                    if (tableExists)
                    {
                        cmd.CommandText = "SELECT COUNT(*) FROM DesignationRoleMappings WHERE IsDeleted = 0";
                        var mappingCount = (int)(cmd.ExecuteScalar() ?? 0);
                        if (mappingCount == 0)
                        {
                            _logger.LogInformation("Seeding default designation-role mappings...");
                            var seedMappingSql = @"
                                INSERT INTO DesignationRoleMappings (DesignationId, RoleId, IsActive, CreatedBy, CreatedAt, IsDeleted) VALUES 
                                (1, 2, 1, 'seeder', GETDATE(), 0), -- Principal -> Principal
                                (2, 2, 1, 'seeder', GETDATE(), 0), -- Vice Principal -> Principal
                                (3, 3, 1, 'seeder', GETDATE(), 0), -- Assistant Head -> AssistantHead
                                (4, 4, 1, 'seeder', GETDATE(), 0), -- Senior Teacher -> SeniorLecturer
                                (5, 5, 1, 'seeder', GETDATE(), 0), -- Lecturer -> Lecturer
                                (6, 5, 1, 'seeder', GETDATE(), 0), -- Teacher -> Lecturer
                                (7, 5, 1, 'seeder', GETDATE(), 0), -- Assistant Teacher -> Lecturer
                                (8, 6, 1, 'seeder', GETDATE(), 0), -- Office Staff -> OfficeStaff
                                (9, 20, 1, 'seeder', GETDATE(), 0), -- Accountant -> Accountant
                                (10, 21, 1, 'seeder', GETDATE(), 0), -- Librarian -> Librarian
                                (11, 22, 1, 'seeder', GETDATE(), 0), -- Lab Assistant -> LabAssistant
                                (12, 23, 1, 'seeder', GETDATE(), 0), -- Driver -> TransportStaff
                                (13, 24, 1, 'seeder', GETDATE(), 0), -- Guard -> SupportStaff
                                (14, 24, 1, 'seeder', GETDATE(), 0), -- Cleaner -> SupportStaff
                                (15, 24, 1, 'seeder', GETDATE(), 0); -- Aya / Helper -> SupportStaff
                            ";
                            await _db.Database.ExecuteSqlRawAsync(seedMappingSql);
                        }
                    }
                }

                // Legacy teacher migration disabled since schema has been permanently updated to Employee root architecture.
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Critical failure during Employee module schema initialization & migration seeding.");
            }
        }
    }
}
