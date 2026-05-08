using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Services.Interfaces.Students;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers;

[Authorize]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;

    public StudentController(IStudentService studentService)
    {
        _studentService = studentService;
    }

    [RequirePermission("Student.View")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, CancellationToken cancellationToken = default)
    {
        if (User.IsInRole("Student"))
        {
            return RedirectToAction(nameof(Details), new { id = GetStudentIdSync() });
        }

        // Detect AJAX/Tabulator requests: check headers OR presence of pagination query params
        bool isAjax = Request.Headers["Accept"].ToString().Contains("application/json")
                    || Request.Headers["X-Requested-With"] == "XMLHttpRequest"
                    || Request.Query.ContainsKey("page");

        if (isAjax)
        {
            var result = await _studentService.GetPagedAsync(page, pageSize, search, cancellationToken);
            return Json(new { data = result.Items, last_page = Math.Ceiling((double)result.TotalItems / result.PageSize), total_records = result.TotalItems });
        }

        return View();
    }

    [HttpGet]
    [RequirePermission("Student.Create")]
    public IActionResult Create()
    {
        return RedirectToAction(nameof(CreateEdit));
    }

    [HttpGet]
    [RequirePermission("Student.Edit")]
    public IActionResult Edit(int id)
    {
        if (User.IsInRole("Student")) return Forbid();
        return RedirectToAction(nameof(CreateEdit), new { id });
    }

    [HttpGet]
    [Route("Student/Details/{id?}")]
    [Authorize] 
    public async Task<IActionResult> Details(string? id, CancellationToken cancellationToken)
    {
        // If ID is null, resolve the identity based on the user's role
        if (string.IsNullOrEmpty(id))
        {
            if (User.IsInRole("Student"))
            {
                var studentId = await GetStudentIdAsync(cancellationToken);
                if (studentId == null) return NotFound("Student record not found.");
                var dto = await _studentService.GetForEditAsync(studentId.Value, cancellationToken);
                return View(dto);
            }
            
            if (User.IsInRole("Teacher") || User.IsInRole("Senior Lecturer") || User.IsInRole("Lecturer"))
            {
                var teacherId = await GetTeacherIdAsync(cancellationToken);
                if (teacherId != null) return RedirectToAction("Details", "Teacher", new { id = teacherId });
            }

            return RedirectToAction("Index"); // Fallback for Admins
        }

        // Standard ID lookup (int or StudentNo)
        StudentUpsertDto? studentDto;
        if (int.TryParse(id, out var intId))
        {
            studentDto = await _studentService.GetForEditAsync(intId, cancellationToken);
        }
        else
        {
            studentDto = await _studentService.GetByStudentNoAsync(id, cancellationToken);
        }

        if (studentDto == null) return NotFound();

        // SECURITY CHECK
        if (User.IsInRole("Student"))
        {
            var loggedInStudentId = await GetStudentIdAsync(cancellationToken);
            if (loggedInStudentId != studentDto.Id) return Forbid(); 
        }
        else
        {
            if (!User.HasClaim("Permission", "Student.View") && !User.IsInRole("Super Admin")) return Forbid();
        }
        
        return View(studentDto);
    }

    private async Task<int?> GetTeacherIdAsync(CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return null;
        var db = HttpContext.RequestServices.GetRequiredService<SchoolManagementSystem.Data.SchoolDbContext>();
        var teacher = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            db.Teachers.AsNoTracking(), t => t.UserId == userId && !t.IsDeleted, ct);
        return teacher?.Id;
    }

    [HttpGet]
    [Route("Student/PrintIdCard/{id?}")]
    [Authorize]
    public async Task<IActionResult> PrintIdCard(string? id, CancellationToken cancellationToken)
    {
        StudentUpsertDto? dto;

        if (string.IsNullOrEmpty(id))
        {
            if (!User.IsInRole("Student")) return NotFound();
            
            var studentId = await GetStudentIdAsync(cancellationToken);
            if (studentId == null) return NotFound();
            
            dto = await _studentService.GetForEditAsync(studentId.Value, cancellationToken);
        }
        else if (int.TryParse(id, out var intId))
        {
            dto = await _studentService.GetForEditAsync(intId, cancellationToken);
        }
        else
        {
            dto = await _studentService.GetByStudentNoAsync(id, cancellationToken);
        }

        if (dto == null) return NotFound();

        // SECURITY CHECK
        if (User.IsInRole("Student"))
        {
            var loggedInStudentId = await GetStudentIdAsync(cancellationToken);
            if (loggedInStudentId != dto.Id) return Forbid();
        }
        else
        {
            if (!User.HasClaim("Permission", "Student.View") && !User.IsInRole("Super Admin")) return Forbid();
        }
        
        return View(dto);
    }


    private async Task<int?> GetStudentIdAsync(CancellationToken ct)
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return null;
        // This is a bit inefficient but safe; in production, you'd cache this or use a Claim
        // Accessing DB directly here for simplicity as we don't have a StudentId claim yet
        var db = HttpContext.RequestServices.GetRequiredService<SchoolManagementSystem.Data.SchoolDbContext>();
        var student = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.FirstOrDefaultAsync(
            db.Students.AsNoTracking(), s => s.UserId == userId && !s.IsDeleted, ct);
        return student?.Id;
    }

    private int? GetStudentIdSync()
    {
        var userIdStr = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdStr, out var userId)) return null;
        var db = HttpContext.RequestServices.GetRequiredService<SchoolManagementSystem.Data.SchoolDbContext>();
        return db.Students.AsNoTracking().FirstOrDefault(s => s.UserId == userId && !s.IsDeleted)?.Id;
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Student")) return Forbid();

        // 🔥 GET DB CONTEXT
        var db = HttpContext.RequestServices
            .GetRequiredService<SchoolManagementSystem.Data.SchoolDbContext>();

        var sectionData = await db.Sections
            .Select(s => new {
                s.Id,
                s.Name,
                s.Capacity,
                StudentCount = db.Students.Count(st => st.SectionId == s.Id && !st.IsDeleted && st.Status == SchoolManagementSystem.Models.Enums.StudentStatus.Active)
            })
            .ToListAsync(cancellationToken);

        var selectList = sectionData
            .Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.Name} ({s.StudentCount}/{s.Capacity}){(s.StudentCount >= s.Capacity ? " - FULL" : "")}",
                Disabled = s.StudentCount >= s.Capacity
            })
            .ToList();

        if (id.HasValue && id > 0)
        {
            if (!User.HasClaim("Permission", "Student.Edit") && !User.IsInRole("Super Admin"))
                return Forbid();

            var dto = await _studentService.GetForEditAsync(id.Value, cancellationToken);
            if (dto == null) return NotFound();

            dto.Sections = selectList;
            return View(dto);
        }

        if (!User.HasClaim("Permission", "Student.Create") && !User.IsInRole("Super Admin"))
            return Forbid();

        var model = new StudentUpsertDto
        {
            DateOfBirth = DateTime.Today.AddYears(-10),
            Sections = selectList
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(StudentUpsertDto model, CancellationToken cancellationToken)
    {
        if (User.IsInRole("Student")) return Forbid();

        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";

        if (model.Id == 0)
        {
            if (!User.HasClaim("Permission", "Student.Create") && !User.IsInRole("Super Admin")) return Forbid();
            await _studentService.CreateAsync(model, userId, cancellationToken);
            TempData["SuccessMessage"] = "Student created successfully.";
        }
        else
        {
            if (!User.HasClaim("Permission", "Student.Edit") && !User.IsInRole("Super Admin")) return Forbid();
            await _studentService.UpdateAsync(model, userId, cancellationToken);
            TempData["SuccessMessage"] = "Student updated successfully.";
        }

        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(StudentUpsertDto model, CancellationToken cancellationToken)
    {
        return CreateEdit(model, cancellationToken);
    }

    [HttpGet]
    [RequirePermission("Student.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var dto = await _studentService.GetForEditAsync(id, cancellationToken);
        return dto is null ? NotFound() : View("Delete", dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Student.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken cancellationToken)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _studentService.DeleteAsync(id, userId, cancellationToken);

            if (Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                return Json(new { success = true, message = "Student deleted successfully." });
            }

            TempData["SuccessMessage"] = "Student deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            if (Request.Headers["Accept"].ToString().Contains("application/json"))
            {
                return Json(new { success = false, message = ex.Message });
            }

            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}
