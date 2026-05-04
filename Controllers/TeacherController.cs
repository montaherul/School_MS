using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Controllers.Common;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Teachers;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers;

public class TeacherController : GenericCrudController<Teacher>
{
    private readonly SchoolDbContext _db;
    public TeacherController(SchoolDbContext db) : base(db, "Teacher") { _db = db; }

    [HttpGet]
    public async Task<IActionResult> Details(int? id, CancellationToken cancellationToken = default)
    {
        // SELF-DISCOVERY: If no ID, find the logged-in teacher's ID
        if (!id.HasValue || id <= 0)
        {
            var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdStr, out var userId))
            {
                var self = await _db.Teachers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.UserId == userId && !t.IsDeleted, cancellationToken);
                
                if (self != null) id = self.Id;
            }
        }

        if (!id.HasValue || id <= 0) return NotFound("Teacher record not found for your account.");

        var teacher = await _db.Teachers
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken);

        if (teacher == null) return NotFound();

        return View(teacher);
    }
}
