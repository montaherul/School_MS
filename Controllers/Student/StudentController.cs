using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.DTOs.Academic;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.ViewModels.Student;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Website;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Student;

[Authorize]
public class StudentController : Controller
{
    private readonly IStudentService _studentService;
    private readonly ITeacherService _teacherService;
    private readonly ISectionService _sectionService;
    private readonly ISchoolWebsiteService _websiteService;
    private readonly IViewRendererService _viewRenderer;
    private readonly IPdfGenerator _pdfGenerator;

    public StudentController(
        IStudentService studentService,
        ITeacherService teacherService,
        ISectionService sectionService,
        ISchoolWebsiteService websiteService,
        IViewRendererService viewRenderer,
        IPdfGenerator pdfGenerator
           )
    {
        _studentService = studentService;
        _teacherService = teacherService;
        _sectionService = sectionService;
        _websiteService = websiteService;
        _viewRenderer = viewRenderer;
        _pdfGenerator = pdfGenerator;
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
    [Route("Student/PreviewIdCard/{id}")]
    public async Task<IActionResult> PreviewIdCard(string id, CancellationToken ct)
    {
        return await PrintIdCard(id, ct);
    }

    [HttpGet]
    [Route("Student/DownloadIdCard/{id}")]
    public async Task<IActionResult> DownloadIdCard(string id, CancellationToken ct)
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
        if (User.IsInRole("Student"))
        {
            var loggedInStudentId = await _studentService.GetStudentIdByUserIdAsync(currentUserId, ct);
            if (loggedInStudentId != dto.Id) return Forbid();
        }
        else if (!User.HasClaim("Permission", "Student.View") && !User.IsInRole("Super Admin"))
        {
            return Forbid();
        }

        var school = await _websiteService.GetSettingsAsync(ct);
        var viewModel = await BuildIdCardViewModelAsync([dto], school);

        var html = await _viewRenderer.RenderToStringAsync("PrintIdCard", viewModel);
        var pdfBytes = _pdfGenerator.GenerateStudentIdCardFromHtml(html);
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
        if (User.IsInRole("Student"))
        {
            var loggedInStudentId = await _studentService.GetStudentIdByUserIdAsync(currentUserId, ct);
            if (loggedInStudentId != dto.Id) return Forbid();
        }
        else if (!User.HasClaim("Permission", "Student.View") && !User.IsInRole("Super Admin"))
        {
            return Forbid();
        }

        var school = await _websiteService.GetSettingsAsync(ct);
        var viewModel = await BuildIdCardViewModelAsync([dto], school);

        return View(viewModel);
    }

    [HttpGet]
    [Route("Student/DownloadBulkIdCardPdf")]
    public async Task<IActionResult> DownloadBulkIdCardPdf(string ids, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        if (!User.HasClaim("Permission", "Student.View") && !User.IsInRole("Super Admin") && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var studentNos = ids.Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var students = new List<StudentUpsertDto>();

        foreach (var no in studentNos)
        {
            var s = await _studentService.GetByStudentNoAsync(no, ct);
            if (s != null) students.Add(s);
        }

        if (students.Count == 0) return NotFound();

        var school = await _websiteService.GetSettingsAsync(ct);
        var viewModel = await BuildIdCardViewModelAsync(students, school);

        var html = await _viewRenderer.RenderToStringAsync("PrintIdCard", viewModel);
        var pdfBytes = _pdfGenerator.GenerateBulkStudentIdCardPdfFromHtml(html);
        return File(pdfBytes, "application/pdf", $"Bulk_ID_Cards_{DateTime.Today:yyyyMMdd}.pdf");
    }

    [HttpGet]
    [Route("Student/BulkPrint")]
    public async Task<IActionResult> BulkPrint(int? classId, int? sectionId, CancellationToken ct)
    {
        if (!User.HasClaim("Permission", "Student.View") && !User.IsInRole("Super Admin") && !User.IsInRole("Admin"))
        {
            return Forbid();
        }

        var availableClasses = (await _sectionService.GetAvailableClassesAsync(ct))
            .Cast<dynamic>()
            .Select(c => new SchoolClassListItemDto
            {
                Id = (int)c.Id,
                Name = (string)c.Name
            })
            .ToList();

        if (classId == null)
        {
            return View("BulkPrintFilter", new IdCardBulkFilterViewModel
            {
                Classes = availableClasses
            });
        }

        var paged = await _studentService.GetPagedAsync(1, 5000, null, classId, sectionId, null, ct);
        var studentIds = paged.Items.Select(i => i.Id).ToList();
        var students = new List<StudentUpsertDto>();

        foreach (var sid in studentIds)
        {
            var s = await _studentService.GetForEditAsync(sid, ct);
            if (s != null) students.Add(s);
        }

        if (students.Count == 0)
        {
            TempData["ErrorMessage"] = "No students found for the selected filters.";
            return View("BulkPrintFilter", new IdCardBulkFilterViewModel
            {
                Classes = availableClasses,
                ClassId = classId,
                SectionId = sectionId
            });
        }

        var school = await _websiteService.GetSettingsAsync(ct);
        var viewModel = await BuildIdCardViewModelAsync(students, school);

        return View("PrintIdCard", viewModel);
    }

    private async Task<IdCardPrintViewModel> BuildIdCardViewModelAsync(List<StudentUpsertDto> students, Models.Entities.Website.SchoolSetting school)
    {
        return new IdCardPrintViewModel
        {
            Students = students,
            SchoolLogoPath = school.LogoPath ?? "",
            SchoolNameEn = school.SchoolName,
            SchoolNameBn = school.BanglaName ?? "",
            SchoolEIIN = school.EIIN,
            SchoolWebsite = school.Website,
            SchoolAddress = school.Address,
            SchoolPhone = school.Phone,
            SchoolEmail = school.Email,
            PrincipalName = school.PrincipalName ?? "",
            PrincipalSignaturePath = school.PrincipalSignaturePath ?? ""
        };
    }

    [HttpGet]
    public async Task<IActionResult> CreateEdit(int? id, CancellationToken ct)
    {
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

