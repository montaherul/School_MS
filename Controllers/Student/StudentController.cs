using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using System.Security.Claims;

using SchoolManagementSystem.Constants;
using SchoolManagementSystem.Models.DTOs.Common;



namespace SchoolManagementSystem.Controllers.Student;

[Authorize]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;
    private readonly ITeacherService _teacherService;
    private readonly ISectionService _sectionService;

    public StudentController(
        IStudentService studentService,
        ITeacherService teacherService,
        ISectionService sectionService)
    {
        _studentService = studentService;
        _teacherService = teacherService;
        _sectionService = sectionService;
    }


    [RequirePermission(Permissions.Student.View)]
    public async Task<IActionResult> Index(int? classId = null, int? sectionId = null, CancellationToken ct = default)
    {
        if (User.IsInRole(Roles.Student))

    [RequirePermission("Student.View")]
    public async Task<IActionResult> Index(int page = 1, int pageSize = 10, string? search = null, int? classId = null, int? sectionId = null, CancellationToken ct = default)
    {
        if (User.IsInRole("Student"))

        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var studentId = await _studentService.GetStudentIdByUserIdAsync(userId, ct);
            return RedirectToAction(nameof(Details), new { id = studentId });
        }


        ViewBag.Classes = await _sectionService.GetAvailableClassesAsync(ct);
        ViewBag.ClassId = classId;
        ViewBag.SectionId = sectionId;
        return View();
    }

    [HttpPost]
    [RequirePermission(Permissions.Student.View)]
    public async Task<IActionResult> GetPaged([FromBody] GridRequestDto request, CancellationToken ct = default)
    {
        try
        {
            // Extract filters from JSON if provided
            int? classId = null;
            int? sectionId = null;

            if (!string.IsNullOrEmpty(request.Filters))
            {
                try
                {
                    var filters = System.Text.Json.JsonDocument.Parse(request.Filters).RootElement;
                    if (filters.TryGetProperty("classId", out var classVal) && classVal.ValueKind != System.Text.Json.JsonValueKind.Null)
                        classId = classVal.GetInt32();
                    if (filters.TryGetProperty("sectionId", out var sectVal) && sectVal.ValueKind != System.Text.Json.JsonValueKind.Null)
                        sectionId = sectVal.GetInt32();
                }
                catch { /* Ignore filter parsing errors */ }
            }

            var result = await _studentService.GetPagedAsync(request.Page, request.Size, request.Search, classId, sectionId, null, ct);
            
            return Json(new PagedApiResponse<SchoolManagementSystem.Models.DTOs.Student.StudentListItemDto>
            { 
                data = result.Items, 
                last_page = (int)Math.Ceiling((double)result.TotalItems / request.Size),
                total = result.TotalItems 
            });
        }
        catch (Exception ex)
        {
            return Json(new PagedApiResponse<object>
            {
                data = new List<object>(),
                last_page = 1,
                total = 0,
                error = ex.Message
            });
        }
    }

    [HttpGet]
    [RequirePermission(Permissions.Student.Create)]
    public IActionResult Create() => RedirectToAction(nameof(CreateEdit));

    [HttpGet]
    [RequirePermission(Permissions.Student.Update)]
    public IActionResult Edit(int id)
    {
        if (User.IsInRole(Roles.Student)) return Forbid();

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

        ViewBag.Classes = await _sectionService.GetAvailableClassesAsync(ct);
        return View();
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

            if (User.IsInRole(Roles.Student))

            if (User.IsInRole("Student"))

            {
                var studentId = await _studentService.GetStudentIdByUserIdAsync(currentUserId, ct);
                if (studentId == null) return NotFound("Student record not found.");
                var dto = await _studentService.GetForEditAsync(studentId.Value, ct);
                return View(dto);
            }
            

            if (User.IsInRole(Roles.Teacher))

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

        if (User.IsInRole(Roles.Student))

        if (User.IsInRole("Student"))

        {
            var loggedInStudentId = await _studentService.GetStudentIdByUserIdAsync(currentUserId, ct);
            if (loggedInStudentId != studentDto.Id) return Forbid(); 
        }

        else if (!User.HasClaim("Permission", Permissions.Student.View) && !User.IsInRole(Roles.SuperAdmin))

        else if (!User.HasClaim("Permission", "Student.View") && !User.IsInRole("Super Admin"))

        {
            return Forbid();
        }
        
        return View(studentDto);
    }

    [HttpGet]
    [Route("Student/DownloadIdCard/{id}")]
    public async Task<IActionResult> DownloadIdCard(string id, [FromServices] SchoolManagementSystem.Helpers.Pdf.IPdfGenerator pdfGenerator, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        StudentUpsertDto? dto;

        if (int.TryParse(id, out var intId))
        {
            dto = await _studentService.GetForEditAsync(intId, ct);
        }
        else
        {
            dto = await _studentService.GetByStudentNoAsync(id, ct);
        }

        if (dto == null) return NotFound();

        // SECURITY CHECK

        if (User.IsInRole(Roles.Student))

        if (User.IsInRole("Student"))

        {
            var loggedInStudentId = await _studentService.GetStudentIdByUserIdAsync(currentUserId, ct);
            if (loggedInStudentId != dto.Id) return Forbid();
        }

        else if (!User.HasClaim("Permission", Permissions.Student.View) && !User.IsInRole(Roles.SuperAdmin))

        else if (!User.HasClaim("Permission", "Student.View") && !User.IsInRole("Super Admin"))

        {
            return Forbid();
        }

        var pdfBytes = pdfGenerator.GenerateStudentIdCard(dto);
        return File(pdfBytes, "application/pdf", $"ID_Card_{dto.StudentNo}.pdf");
    }

    [HttpGet]
    [Route("Student/PrintIdCard/{id?}")]
    public async Task<IActionResult> PrintIdCard(string? id, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        StudentUpsertDto? dto;

        if (string.IsNullOrEmpty(id))
        {

            if (!User.IsInRole(Roles.Student)) return NotFound();

            if (!User.IsInRole("Student")) return NotFound();

            var studentId = await _studentService.GetStudentIdByUserIdAsync(currentUserId, ct);
            if (studentId == null) return NotFound();
            dto = await _studentService.GetForEditAsync(studentId.Value, ct);
        }
        else if (int.TryParse(id, out var intId))
        {
            dto = await _studentService.GetForEditAsync(intId, ct);
        }
        else
        {
            dto = await _studentService.GetByStudentNoAsync(id, ct);
        }

        if (dto == null) return NotFound();

        // SECURITY CHECK

        if (User.IsInRole(Roles.Student))

        if (User.IsInRole("Student"))

        {
            var loggedInStudentId = await _studentService.GetStudentIdByUserIdAsync(currentUserId, ct);
            if (loggedInStudentId != dto.Id) return Forbid();
        }

        else if (!User.HasClaim("Permission", Permissions.Student.View) && !User.IsInRole(Roles.SuperAdmin))

        else if (!User.HasClaim("Permission", "Student.View") && !User.IsInRole("Super Admin"))

        {
            return Forbid();
        }
        
        return View(dto);
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id, CancellationToken ct)
    {

        if (User.IsInRole(Roles.Student)) return Forbid();

        if (User.IsInRole("Student")) return Forbid();


        var sections = await _sectionService.GetByClassIdAsync(0, ct); // Get all or handle by class
        var selectList = sections.Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
        {
            Value = s.Id.ToString(),
            Text = $"{s.Name} ({s.StudentCount}/{s.Capacity})",
            Disabled = s.StudentCount >= s.Capacity
        }).ToList();

        if (id.HasValue && id > 0)
        {

            if (!User.HasClaim("Permission", Permissions.Student.Update) && !User.IsInRole(Roles.SuperAdmin)) return Forbid();

            if (!User.HasClaim("Permission", "Student.Edit") && !User.IsInRole("Super Admin")) return Forbid();

            var dto = await _studentService.GetForEditAsync(id.Value, ct);
            if (dto == null) return NotFound();
            dto.Sections = selectList;
            return View(dto);
        }


        if (!User.HasClaim("Permission", Permissions.Student.Create) && !User.IsInRole(Roles.SuperAdmin)) return Forbid();

        if (!User.HasClaim("Permission", "Student.Create") && !User.IsInRole("Super Admin")) return Forbid();

        return View(new StudentUpsertDto { DateOfBirth = DateTime.Today.AddYears(-10), Sections = selectList });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateEdit(StudentUpsertDto model, CancellationToken ct)
    {

        if (User.IsInRole(Roles.Student)) return Forbid();

        if (User.IsInRole("Student")) return Forbid();

        if (!ModelState.IsValid) return View(model);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
        if (model.Id == 0)
        {

            if (!User.HasClaim("Permission", Permissions.Student.Create) && !User.IsInRole(Roles.SuperAdmin)) return Forbid();

            if (!User.HasClaim("Permission", "Student.Create") && !User.IsInRole("Super Admin")) return Forbid();

            await _studentService.CreateAsync(model, userId, ct);
            TempData["SuccessMessage"] = "Student created successfully.";
        }
        else
        {

            if (!User.HasClaim("Permission", Permissions.Student.Update) && !User.IsInRole(Roles.SuperAdmin)) return Forbid();

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

    [RequirePermission(Permissions.Student.Delete)]

    [RequirePermission("Student.Delete")]

    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var dto = await _studentService.GetForEditAsync(id, ct);
        return dto == null ? NotFound() : View("Delete", dto);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]

    [RequirePermission(Permissions.Student.Delete)]

    [RequirePermission("Student.Delete")]

    public async Task<IActionResult> DeleteConfirmed(int id, CancellationToken ct)
    {
        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            await _studentService.DeleteAsync(id, userId, ct);
            TempData["SuccessMessage"] = "Student deleted successfully.";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return RedirectToAction(nameof(Index));
        }
    }
}

