using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Employee;

namespace SchoolManagementSystem.Data.Seeders;

public class HrReferenceDataSeeder : IDataSeeder
{
    public int Order => 2;
    public string Name => "HrReferenceDataSeeder";

    private readonly SchoolDbContext _db;

    public HrReferenceDataSeeder(SchoolDbContext db)
    {
        _db = db;
    }

    public async Task SeedAsync(CancellationToken ct = default)
    {
        // 1. Departments
        var departments = new[]
        {
            new Department { Name = "Academic", Code = "ACAD" },
            new Department { Name = "Administration", Code = "ADMIN" },
            new Department { Name = "Accounts", Code = "ACC" },
            new Department { Name = "Library", Code = "LIB" },
            new Department { Name = "Maintenance", Code = "MAINT" }
        };

        foreach (var d in departments)
        {
            if (!await _db.Set<Department>().AnyAsync(x => x.Code == d.Code, ct))
            {
                await _db.Set<Department>().AddAsync(d, ct);
            }
        }

        // 2. Designations
        var designations = new[] { "Principal", "Vice Principal", "Senior Lecturer", "Lecturer", "Teacher", "Accountant", "Librarian", "Clerk", "Security Guard" };
        foreach (var desig in designations)
        {
            if (!await _db.Set<Designation>().AnyAsync(x => x.Name == desig, ct))
            {
                await _db.Set<Designation>().AddAsync(new Designation { Name = desig }, ct);
            }
        }

        // 3. Leave Types
        var leaveTypes = new[]
        {
            new LeaveType { Name = "Sick Leave", DefaultDaysPerYear = 15, ColorCode = "#dc3545", IsPaid = true, IsActive = true },
            new LeaveType { Name = "Casual Leave", DefaultDaysPerYear = 10, ColorCode = "#ffc107", IsPaid = true, IsActive = true },
            new LeaveType { Name = "Maternity Leave", DefaultDaysPerYear = 120, ColorCode = "#e83e8c", IsPaid = true, IsActive = true },
            new LeaveType { Name = "Unpaid Leave", DefaultDaysPerYear = 0, ColorCode = "#6c757d", IsPaid = false, IsActive = true }
        };

        foreach (var lt in leaveTypes)
        {
            if (!await _db.Set<LeaveType>().AnyAsync(x => x.Name == lt.Name, ct))
            {
                await _db.Set<LeaveType>().AddAsync(lt, ct);
            }
        }

        // 4. Holidays (Sample)
        if (!await _db.Set<Holiday>().AnyAsync(ct))
        {
            await _db.Set<Holiday>().AddAsync(new Holiday 
            { 
                Name = "Victory Day", 
                StartDate = new DateTime(DateTime.Today.Year, 12, 16), 
                EndDate = new DateTime(DateTime.Today.Year, 12, 16), 
                Description = "National Holiday",
                IsRecurring = true
            }, ct);
        }

        await _db.SaveChangesAsync(ct);
    }
}
