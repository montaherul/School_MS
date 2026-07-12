using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;
using System.Text;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class MarksController : Controller
{
    private readonly IMarkEntryService _markEntryService;
    private readonly ITeacherService _teacherService;
    private readonly IResultAuthorizationService _resultAuthService;
    private readonly ITeacherResultRepository _teacherResultRepository;
    private readonly ISubjectMarkStructureService _markStructureService;
    private readonly IAcademicYearService _academicYearService;
    private readonly IExamService _examService;
    private readonly ISchoolClassService _schoolClassService;
    private readonly IStudentService _studentService;
    private readonly IResultPublicationService _publicationService;

    public MarksController(
        IMarkEntryService markEntryService,
        ITeacherService teacherService,
        IResultAuthorizationService resultAuthService,
        ITeacherResultRepository teacherResultRepository,
        ISubjectMarkStructureService markStructureService,
        IAcademicYearService academicYearService,
        IExamService examService,
        ISchoolClassService schoolClassService,
        IStudentService studentService,
        IResultPublicationService publicationService)
    {
        _markEntryService = markEntryService;
        _teacherService = teacherService;
        _resultAuthService = resultAuthService;
        _teacherResultRepository = teacherResultRepository;
        _markStructureService = markStructureService;
        _academicYearService = academicYearService;
        _examService = examService;
        _schoolClassService = schoolClassService;
        _studentService = studentService;
        _publicationService = publicationService;
    }

    [HttpGet]
    [RequirePermission("Marks.View")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var activeYear = await _academicYearService.GetActiveYearAsync(ct);
        var activeYearId = activeYear?.Id ?? 1;

        var exams = await _examService.GetExamsByYearAsync(activeYearId, ct);
        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);

        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);

        var model = new MarksIndexViewModel
        {
            Exams = exams.ToList(),
            Classes = classes.ToList(),
            IsTeacher = teacher != null,
            TeacherId = teacher?.Id ?? 0,
            Assignments = teacher != null
                ? await _teacherService.GetTeacherClassAssignmentsAsync(teacher.Id, ct)
                : null
        };

        return View(model);
    }

    [HttpGet]
    [RequirePermission("Marks.View")]
    public async Task<IActionResult> Entry(int examId, int classId, int sectionId, int subjectId, CancellationToken ct)
    {
        if (examId <= 0 || classId <= 0 || sectionId <= 0 || subjectId <= 0)
            return BadRequest("Invalid entry parameters.");

        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);

        if (teacher != null && !User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
        {
            var isAuthorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher.Id, subjectId, classId, sectionId, 0, null, ct);
            if (!isAuthorized) return Forbid();
        }

        var dto = await _markEntryService.GetMarkEntryDataAsync(examId, subjectId, classId, sectionId);
        var vm = new MarkEntryViewModel
        {
            ExamId = dto.ExamId,
            ExamName = dto.ExamName,
            SubjectId = dto.SubjectId,
            SubjectName = dto.SubjectName,
            ClassId = dto.ClassId,
            ClassName = dto.ClassName,
            SectionId = sectionId,
            Students = dto.Students.Select(s => new StudentMarkViewModel
            {
                StudentId = s.StudentId,
                StudentNo = s.StudentNo,
                StudentName = s.StudentName,
                RollNumber = s.RollNumber,
                MarksObtained = s.MarksObtained,
                Grade = s.Grade,
                IsLocked = s.IsLocked,
                ComponentMarks = s.ComponentMarks,
                EnteredByTeacherId = s.EnteredByTeacherId,
                EnteredByTeacherName = s.EnteredByTeacherName
            }).ToList()
        };

        var columns = await _markStructureService.GetGridColumnsAsync(subjectId, classId);
        ViewBag.ComponentColumns = columns;

        var exam = await _examService.GetExamEntityByIdAsync(examId, ct);
        if (exam != null && exam.Status == ResultWorkflowStatus.Published)
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
    [ValidateAntiForgeryToken]
    [RequirePermission("Marks.Edit")]
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
                    var studentIds = dto.Marks.Select(m => m.StudentId).Distinct().ToList();
                    var classSections = await _studentService.GetStudentClassSectionsAsync(studentIds, ct);

                    foreach (var cs in classSections)
                    {
                        var isAuthorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher.Id, dto.SubjectId, cs.ClassId, cs.SectionId, 0, null, ct);
                        if (!isAuthorized) return Forbid();
                    }
                }
            }

            foreach (var m in dto.Marks) m.Status = ResultWorkflowStatus.Submitted;
            var result = await _markEntryService.SubmitMarksBatchTrackedAsync(dto);
            return Json(new { success = true, savedCount = result.SavedCount, skippedStudentIds = result.SkippedStudentIds });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Marks.Edit")]
    public async Task<IActionResult> SaveRow([FromBody] MarkEntryDto dto, CancellationToken ct)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);

            if (teacher != null && !User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
            {
                var student = (await _studentService.GetStudentClassSectionsAsync([dto.StudentId], ct)).FirstOrDefault();

                if (student != null)
                {
                    var isAuthorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(
                        teacher.Id, dto.SubjectId, student.ClassId, student.SectionId, 0, null, ct);
                    if (!isAuthorized) return Forbid();
                }
            }

            var batch = new MarkBatchDto
            {
                ExamId = dto.ExamId,
                SubjectId = dto.SubjectId,
                TeacherId = teacher?.Id ?? 1,
                Marks = [dto]
            };

            await _markEntryService.SubmitMarksBatchAsync(batch);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Marks.Edit")]
    public async Task<IActionResult> SaveDraft([FromBody] MarkBatchDto dto, CancellationToken ct)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
            dto.TeacherId = teacher?.Id ?? 1;

            if (teacher != null && !User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
            {
                if (dto.Marks.Any())
                {
                    var studentIds = dto.Marks.Select(m => m.StudentId).Distinct().ToList();
                    var classSections = await _studentService.GetStudentClassSectionsAsync(studentIds, ct);

                    foreach (var cs in classSections)
                    {
                        var isAuthorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher.Id, dto.SubjectId, cs.ClassId, cs.SectionId, 0, null, ct);
                        if (!isAuthorized) return Forbid();
                    }
                }
            }

            foreach (var m in dto.Marks) m.Status = ResultWorkflowStatus.Draft;
            await _markEntryService.SubmitMarksBatchAsync(dto);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Marks.Import")]
    public async Task<IActionResult> ImportExcel(int examId, int subjectId, int classId, int sectionId, IFormFile file, bool saveAsDraft, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        var teacherId = teacher?.Id ?? 1;

        if (!User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
        {
            var authorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacherId, subjectId, classId, sectionId, 0, null, ct);
            if (!authorized) return Forbid();
        }

        if (file == null || file.Length == 0)
            return Json(new ImportResultDto { ErrorCount = 1, Errors = [new() { RowNumber = 0, Message = "No file uploaded" }] });

        var allowedExtensions = new[] { ".xlsx", ".xls" };
        var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
        if (!allowedExtensions.Contains(extension))
            return Json(new ImportResultDto { ErrorCount = 1, Errors = [new() { RowNumber = 0, Message = "Only Excel files (.xlsx, .xls) are allowed." }] });

        if (file.Length > 10 * 1024 * 1024)
            return Json(new ImportResultDto { ErrorCount = 1, Errors = [new() { RowNumber = 0, Message = "File size must be less than 10MB." }] });

        using var stream = file.OpenReadStream();
        var result = await _markEntryService.ImportMarksFromExcelAsync(stream, examId, subjectId, classId, sectionId, teacherId, saveAsDraft);
        return Json(result);
    }

    [HttpGet]
    [RequirePermission("Marks.View")]
    public async Task<IActionResult> DownloadTemplate(int examId, int subjectId, int classId, int sectionId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (!User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
        {
            var authorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher?.Id ?? 0, subjectId, classId, sectionId, 0, null, ct);
            if (!authorized) return Forbid();
        }
        var data = await _markEntryService.GenerateImportTemplateAsync(examId, subjectId, classId, sectionId);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"MarkEntryTemplate_Exam{examId}_Subject{subjectId}.xlsx");
    }

    [HttpGet]
    [RequirePermission("Marks.View")]
    public async Task<IActionResult> ExportExcel(int examId, int subjectId, int classId, int sectionId, int? groupId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (!User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
        {
            var authorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher?.Id ?? 0, subjectId, classId, sectionId, 0, groupId, ct);
            if (!authorized) return Forbid();
        }
        var data = await _markEntryService.ExportMarksToExcelAsync(examId, subjectId, classId, sectionId, groupId);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Marks_Exam{examId}_Subject{subjectId}.xlsx");
    }

    [HttpGet]
    [RequirePermission("Marks.View")]
    public async Task<IActionResult> ExportCsv(int examId, int subjectId, int classId, int sectionId, int? groupId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (!User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
        {
            var authorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher?.Id ?? 0, subjectId, classId, sectionId, 0, groupId, ct);
            if (!authorized) return Forbid();
        }
        var data = await _markEntryService.ExportMarksToCsvAsync(examId, subjectId, classId, sectionId, groupId);
        return File(Encoding.UTF8.GetBytes(data), "text/csv", $"Marks_Exam{examId}_Subject{subjectId}.csv");
    }

    [HttpGet]
    [RequirePermission("Marks.Dashboard")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return Unauthorized();

        var activeYear = await _academicYearService.GetActiveYearAsync(ct);
        var activeYearId = activeYear?.Id ?? 1;

        var assignedExams = await _teacherResultRepository.GetTeacherAssignedExamsAsync(teacher.Id, activeYearId, ct);
        var examIds = assignedExams.Select(e => e.ExamId).ToList();

        var assignments = await _teacherService.GetTeacherSubjectAssignmentsAsync(teacher.Id, activeYearId, ct);

        var allMarkEntries = await _markEntryService.GetMarkEntryStatusByExamIdsAsync(examIds, ct);

        int pendingEntries = 0;
        int submittedEntries = 0;
        int totalEntries = 0;
        var completions = new Dictionary<int, int>();

        foreach (var examId in examIds)
        {
            int examCount = 0;
            int examSubmitted = 0;
            foreach (var a in assignments)
            {
                totalEntries++;
                examCount++;
                var examMarks = allMarkEntries.Where(m =>
                    m.ExamId == examId && m.SubjectId == a.SubjectId && m.ClassId == a.ClassId).ToList();
                if (examMarks.Count == 0) pendingEntries++;
                else if (examMarks.Any(m => m.Status == ResultWorkflowStatus.Submitted))
                {
                    submittedEntries++;
                    examSubmitted++;
                }
            }
            completions[examId] = examCount > 0 ? (int)(examSubmitted * 100m / examCount) : 0;
        }

        var model = new TeacherMarksDashboardViewModel
        {
            AssignedExams = assignedExams.Count,
            AssignedSubjects = assignments.Count,
            PendingEntries = pendingEntries,
            SubmittedEntries = submittedEntries,
            TotalEntries = totalEntries,
            ExamNames = assignedExams.Select(e => e.ExamName).ToList(),
            CompletionPercentages = assignedExams.Select(e => completions.GetValueOrDefault(e.ExamId, 0)).ToList()
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Marks.Lock")]
    public async Task<IActionResult> Lock(int examId, int subjectId, int classId, int sectionId)
    {
        try
        {
            if (sectionId <= 0)
                await _markEntryService.LockMarksForClassAsync(examId, subjectId, classId);
            else
                await _markEntryService.LockMarksAsync(examId, subjectId, classId, sectionId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Marks.Lock")]
    public async Task<IActionResult> Unlock(int examId, int subjectId, int classId, int sectionId)
    {
        try
        {
            if (sectionId <= 0)
                await _markEntryService.UnlockMarksForClassAsync(examId, subjectId, classId);
            else
                await _markEntryService.UnlockMarksAsync(examId, subjectId, classId, sectionId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [RequirePermission("Marks.View")]
    public async Task<IActionResult> EntryStatus(int examId, int? classId, CancellationToken ct)
    {
        var dto = await _markEntryService.GetEntryStatusAsync(examId, classId);
        var exam = await _examService.GetExamEntityByIdAsync(examId, ct);
        ViewBag.ExamName = exam?.Name ?? "";
        return View(dto);
    }

    [HttpGet]
    [RequirePermission("Marks.Audit")]
    public async Task<IActionResult> AuditLog(int? examId, int? studentId, CancellationToken ct)
    {
        var logs = await _publicationService.GetAuditLogsAsync(examId, studentId, ct);

        var model = new MarksAuditLogViewModel
        {
            Logs = logs,
            Exams = (await _examService.GetAllExamsAsync(ct)).ToList(),
            SelectedExamId = examId
        };
        return View(model);
    }
}
