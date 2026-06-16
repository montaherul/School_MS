using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Helpers.Files;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.ViewModels.Student;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Student;

[Authorize]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;
    private readonly ITeacherService _teacherService;
    private readonly ISectionService _sectionService;
    private readonly ISchoolClassService _classService;
    private readonly IFileStorageService _fileStorage;
    public StudentController(
        IStudentService studentService,
        ITeacherService teacherService,
        ISectionService sectionService,
        ISchoolClassService classService,
        IFileStorageService fileStorage)
    {
        _studentService = studentService;
        _teacherService = teacherService;
        _sectionService = sectionService;
        _classService = classService;
        _fileStorage = fileStorage;
    }

    [RequirePermission("Student.View")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, int? classId = null, int? sectionId = null, CancellationToken ct = default)
    {
        if (User.IsInRole("Student"))
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var studentId = await _studentService.GetStudentIdByUserIdAsync(userId, ct);
            return RedirectToAction(nameof(Details), new { id = studentId });
        }

        bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest" || Request.Headers["Accept"].ToString().Contains("application/json") || Request.Query.ContainsKey("page");

        if (isAjax)
        {
            var result = await _studentService.GetPagedAsync(page, pageSize, search, classId, sectionId, null, ct);
            return Json(new { 
                data = result.Items, 
                last_page = Math.Ceiling((double)result.TotalItems / pageSize), 
                total_records = result.TotalItems 
            });
        }

        var availableClasses = (await _sectionService.GetAvailableClassesAsync(ct)).Cast<dynamic>().Select(c => new SchoolManagementSystem.Models.DTOs.Academic.SchoolClassListItemDto { Id = (int)c.Id, Name = (string)c.Name }).ToList();
        var model = new SchoolManagementSystem.Models.ViewModels.Student.StudentIndexViewModel
        {
            Classes = availableClasses
        };
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> GetList(int page = 1, int pageSize = 10, string? search = null, int? classId = null, int? sectionId = null, CancellationToken ct = default)
    {
        var result = await _studentService.GetPagedAsync(page, pageSize, search, classId, sectionId, null, ct);
        return Json(new {
            data = result.Items,
            last_page = Math.Ceiling((double)result.TotalItems / pageSize)
        });
    }

    [HttpGet]
    [RequirePermission("Student.Create")]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission("Student.Edit")]
    public IActionResult Edit(int id)
    {
        if (User.IsInRole("Student")) return Forbid();
        return RedirectToAction(nameof(CreateEdit), new { id });
    }

    [HttpGet]
    [Route("Student/Details/{id?}")]
    public async Task<IActionResult> Details(string? id, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        if (string.IsNullOrEmpty(id))
        {
            if (User.IsInRole("Student"))
            {
                var studentId = await _studentService.GetStudentIdByUserIdAsync(currentUserId, ct);
                if (studentId == null) return NotFound("Student record not found.");
                var dto = await _studentService.GetForEditAsync(studentId.Value, ct);
                return View(dto);
            }
            
            if (User.IsInRole("Teacher") || User.IsInRole("Senior Lecturer") || User.IsInRole("Lecturer"))
            {
                var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
                if (teacher != null) return RedirectToAction("Details", "Teacher", new { id = teacher.Id });
            }

            return RedirectToAction(nameof(Index));
        }

        StudentUpsertDto? studentDto;
        if (int.TryParse(id, out var intId))
        {
            studentDto = await _studentService.GetForEditAsync(intId, ct);
        }
        else
        {
            studentDto = await _studentService.GetByStudentNoAsync(id, ct);
        }

        if (studentDto == null) return NotFound();

        // SECURITY CHECK
        if (User.IsInRole("Student"))
        {
            var loggedInStudentId = await _studentService.GetStudentIdByUserIdAsync(currentUserId, ct);
            if (loggedInStudentId != studentDto.Id) return Forbid(); 
        }
        else if (!User.HasClaim("Permission", "Student.View") && !User.IsInRole("Super Admin"))
        {
            return Forbid();
        }
        
        return View(studentDto);
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id, CancellationToken ct)
    {
        if (User.IsInRole("Student")) return Forbid();

        var classes = await _classService.GetAllAsync(ct);
        ViewBag.Classes = classes;

        var sections = await _sectionService.GetByClassIdAsync(0, null, ct); // Get all or handle by class
        var selectList = sections.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = s.Id.ToString(),
            Text = $"{s.Name} ({s.StudentCount}/{s.Capacity})",
            Disabled = s.StudentCount >= s.Capacity
        }).ToList();

        if (id.HasValue && id > 0)
        {
            if (!User.HasClaim("Permission", "Student.Edit") && !User.IsInRole("Super Admin")) return Forbid();
            var dto = await _studentService.GetForEditAsync(id.Value, ct);
            if (dto == null) return NotFound();
            dto.Sections = selectList;
            dto.OptionalSubjectList = await _studentService.GetOptionalSubjectsAsync(dto.ClassId, ct);
            return View(dto);
        }

        if (!User.HasClaim("Permission", "Student.Create") && !User.IsInRole("Super Admin")) return Forbid();
        return View(new StudentUpsertDto { DateOfBirth = DateTime.Today.AddYears(-10), Sections = selectList });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(StudentUpsertDto model, CancellationToken ct)
    {
        if (User.IsInRole("Student")) return Forbid();
        if (!ModelState.IsValid) return View(model);

        if (model.ProfilePicture != null && model.ProfilePicture.Length > 0)
        {
            model.ProfilePicturePath = await _fileStorage.SaveAsync(model.ProfilePicture, "students/photos", ct);
        }

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (model.Id == 0)
        {
            if (!User.HasClaim("Permission", "Student.Create") && !User.IsInRole("Super Admin")) return Forbid();
            await _studentService.CreateAsync(model, userId, ct);
            TempData["SuccessMessage"] = "Student created successfully.";
        }
        else
        {
            if (!User.HasClaim("Permission", "Student.Edit") && !User.IsInRole("Super Admin")) return Forbid();
            await _studentService.UpdateAsync(model, userId, ct);
            TempData["SuccessMessage"] = "Student updated successfully.";
        }
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public Task<IActionResult> Save(StudentUpsertDto model, CancellationToken ct) => CreateEdit(model, ct);

    [HttpGet]
    [RequirePermission("Student.Delete")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var dto = await _studentService.GetForEditAsync(id, ct);
        return dto == null ? NotFound() : View("Delete", dto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [RequirePermission("Student.Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _studentService.DeleteAsync(id, userId, ct);
            return Json(new { success = true, message = "Student deleted successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}

