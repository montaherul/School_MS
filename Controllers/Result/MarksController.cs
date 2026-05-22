using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class MarksController : Controller
{
    private readonly IMarkEntryService _markEntryService;
    private readonly ITeacherService _teacherService;
    private readonly IUnitOfWork _uow;
    private readonly SchoolManagementSystem.Services.Interfaces.Result.IResultAuthorizationService _resultAuthService;

    public MarksController(
        IMarkEntryService markEntryService,
        ITeacherService teacherService,
        IUnitOfWork uow,
        SchoolManagementSystem.Services.Interfaces.Result.IResultAuthorizationService resultAuthService)
    {
        _markEntryService = markEntryService;
        _teacherService = teacherService;
        _uow = uow;
        _resultAuthService = resultAuthService;
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer,Admin,Super Admin,Principal")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var activeYear = await _uow.Repository<AcademicYear>().FirstOrDefaultAsync(x => x.IsActive, ct);
        var activeYearId = activeYear?.Id ?? 1;

        var exams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => x.AcademicYearId == activeYearId && !x.IsDeleted);
        var classes = await _uow.Repository<SchoolClass>().ListAsync(x => !x.IsDeleted);

        ViewBag.Exams = exams;
        ViewBag.Classes = classes;

        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        ViewBag.IsTeacher = teacher != null;

        if (teacher != null)
        {
            ViewBag.TeacherId = teacher.Id;
            var assignments = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherClassAssignment>().Query()
                .Include(a => a.Class)
                .Include(a => a.Section)
                .Include(a => a.Group)
                .Where(a => a.TeacherId == teacher.Id && !a.IsDeleted)
                .ToListAsync(ct);
            ViewBag.Assignments = assignments;
        }

        return View();
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer,Admin,Super Admin,Principal")]
    public async Task<IActionResult> Entry(int examId, int classId, int sectionId, int subjectId, CancellationToken ct)
    {
        if (examId <= 0 || classId <= 0 || sectionId <= 0 || subjectId <= 0)
            return BadRequest("Invalid entry parameters.");

        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);

        if (teacher != null && !User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
        {
            var isAuthorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher.Id, subjectId, classId, sectionId, 0, ct);
            if (!isAuthorized) return Forbid();
        }

        var vm = await _markEntryService.GetMarkEntryDataAsync(examId, subjectId, classId, sectionId);
        
        var exam = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().GetByIdAsync(examId);
        if (exam != null && exam.Status == SchoolManagementSystem.Models.Enums.ResultWorkflowStatus.Published)
        {
            ViewBag.IsReadOnly = true;
            ViewBag.Message = "Exam marks have already been published and cannot be modified.";
        }
        else
        {
            ViewBag.IsReadOnly = false;
        }

        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer,Admin,Super Admin,Principal")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> Save([FromBody] MarkBatchDto dto, CancellationToken ct)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
            
            dto.TeacherId = teacher?.Id ?? 1; // Default to admin teacher if system user
            
            if (teacher != null && !User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
            {
                if (dto.Marks.Any())
                {
                    var firstStudentId = dto.Marks.First().StudentId;
                    var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().GetByIdAsync(firstStudentId);
                    if (student != null)
                    {
                        var isAuthorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher.Id, dto.SubjectId, student.ClassId, student.SectionId, 0, ct);
                        if (!isAuthorized) return Forbid();
                    }
                }
            }
            
            await _markEntryService.SubmitMarksBatchAsync(dto);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
    public async Task<IActionResult> AuditLog(int? examId, int? studentId, CancellationToken ct)
    {
        var query = _uow.Repository<ResultAuditLog>().Query()
            .Include(l => l.Exam)
            .Include(l => l.Student)
            .Include(l => l.Subject)
            .Where(l => !l.IsDeleted);

        if (examId.HasValue && examId > 0)
            query = query.Where(l => l.ExamId == examId.Value);

        if (studentId.HasValue && studentId > 0)
            query = query.Where(l => l.StudentId == studentId.Value);

        var logs = await query.OrderByDescending(l => l.CreatedAt).Take(200).ToListAsync(ct);
        
        ViewBag.Exams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted);
        return View(logs);
    }
}
