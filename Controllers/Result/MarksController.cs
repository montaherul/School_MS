using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;
using System.Text;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Models.Enums;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class MarksController : Controller
{
    private readonly IMarkEntryService _markEntryService;
    private readonly ITeacherService _teacherService;
    private readonly IUnitOfWork _uow;
    private readonly SchoolManagementSystem.Services.Interfaces.Result.IResultAuthorizationService _resultAuthService;
    private readonly ITeacherResultRepository _teacherResultRepository;
    private readonly ISubjectMarkStructureService _markStructureService;

    public MarksController(
        IMarkEntryService markEntryService,
        ITeacherService teacherService,
        IUnitOfWork uow,
        SchoolManagementSystem.Services.Interfaces.Result.IResultAuthorizationService resultAuthService,
        ITeacherResultRepository teacherResultRepository,
        ISubjectMarkStructureService markStructureService)
    {
        _markEntryService = markEntryService;
        _teacherService = teacherService;
        _uow = uow;
        _resultAuthService = resultAuthService;
        _teacherResultRepository = teacherResultRepository;
        _markStructureService = markStructureService;
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer,Admin,Super Admin,Principal")]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var activeYear = await _uow.Repository<AcademicYear>().FirstOrDefaultAsync(x => x.IsActive, ct);
        var activeYearId = activeYear?.Id ?? 1;

        var exams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => x.AcademicYearId == activeYearId && !x.IsDeleted);
        var classes = await _uow.Repository<SchoolClass>().ListAsync(x => !x.IsDeleted);

        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);

        var model = new SchoolManagementSystem.Models.ViewModels.Result.MarksIndexViewModel
        {
            Exams = exams.ToList(),
            Classes = classes.ToList(),
            IsTeacher = teacher != null,
            TeacherId = teacher?.Id ?? 0,
            Assignments = teacher != null
                ? (await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherClassAssignment>().Query()
                    .Include(a => a.Class)
                    .Include(a => a.Section)
                    .Include(a => a.Group)
                    .Where(a => a.TeacherId == teacher.Id && !a.IsDeleted)
                    .ToListAsync(ct))
                : null
        };

        return View(model);
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

        var dto = await _markEntryService.GetMarkEntryDataAsync(examId, subjectId, classId, sectionId);
        var vm = new MarkEntryViewModel
        {
            ExamId = dto.ExamId,
            ExamName = dto.ExamName,
            SubjectId = dto.SubjectId,
            SubjectName = dto.SubjectName,
            ClassId = dto.ClassId,
            ClassName = dto.ClassName,
            Students = dto.Students.Select(s => new StudentMarkViewModel
            {
                StudentId = s.StudentId,
                StudentNo = s.StudentNo,
                StudentName = s.StudentName,
                RollNumber = s.RollNumber,
                MarksObtained = s.MarksObtained,
                Grade = s.Grade,
                IsLocked = s.IsLocked,
                WrittenMarks = s.WrittenMarks,
                MCQMarks = s.MCQMarks,
                CQMarks = s.CQMarks,
                PracticalMarks = s.PracticalMarks,
                VivaMarks = s.VivaMarks,
                LabMarks = s.LabMarks,
                OralMarks = s.OralMarks,
                AssignmentMarks = s.AssignmentMarks,
                ContinuousAssessmentMarks = s.ContinuousAssessmentMarks,
                CompetencyMarks = s.CompetencyMarks,
                BehaviourMarks = s.BehaviourMarks,
                ParticipationMarks = s.ParticipationMarks,
                ComponentValues = s.ComponentValues,
                EnteredByTeacherId = s.EnteredByTeacherId,
                EnteredByTeacherName = s.EnteredByTeacherName
            }).ToList()
        };

        var columns = await _markStructureService.GetGridColumnsAsync(subjectId, classId);
        ViewBag.ComponentColumns = columns;

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
                    var studentIds = dto.Marks.Select(m => m.StudentId).Distinct().ToList();
                    var classSections = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
                        .Query()
                        .Where(s => studentIds.Contains(s.Id))
                        .Select(s => new { s.ClassId, s.SectionId })
                        .Distinct()
                        .ToListAsync(ct);

                    foreach (var cs in classSections)
                    {
                        var isAuthorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher.Id, dto.SubjectId, cs.ClassId, cs.SectionId, 0, ct);
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

    [HttpPost]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer,Admin,Super Admin,Principal")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SaveDraft([FromBody] MarkBatchDto dto, CancellationToken ct)
    {
        try
        {
            var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
            dto.TeacherId = teacher?.Id ?? 1;
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
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer,Admin,Super Admin,Principal")]
    public async Task<IActionResult> ImportExcel(int examId, int subjectId, int classId, int sectionId, IFormFile file, bool saveAsDraft, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        var teacherId = teacher?.Id ?? 1;

        if (!User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
        {
            var authorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacherId, subjectId, classId, sectionId, 0, ct);
            if (!authorized) return Forbid();
        }

        if (file == null || file.Length == 0)
            return Json(new ImportResultDto { ErrorCount = 1, Errors = [new() { RowNumber = 0, Message = "No file uploaded" }] });

        using var stream = file.OpenReadStream();
        var result = await _markEntryService.ImportMarksFromExcelAsync(stream, examId, subjectId, classId, sectionId, teacherId, saveAsDraft);
        return Json(result);
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer,Admin,Super Admin,Principal")]
    public async Task<IActionResult> DownloadTemplate(int examId, int subjectId, int classId, int sectionId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (!User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
        {
            var authorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher?.Id ?? 0, subjectId, classId, sectionId, 0, ct);
            if (!authorized) return Forbid();
        }
        var data = await _markEntryService.GenerateImportTemplateAsync(examId, subjectId, classId, sectionId);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"MarkEntryTemplate_Exam{examId}_Subject{subjectId}.xlsx");
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer,Admin,Super Admin,Principal")]
    public async Task<IActionResult> ExportExcel(int examId, int subjectId, int classId, int sectionId, int? groupId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (!User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
        {
            var authorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher?.Id ?? 0, subjectId, classId, sectionId, groupId ?? 0, ct);
            if (!authorized) return Forbid();
        }
        var data = await _markEntryService.ExportMarksToExcelAsync(examId, subjectId, classId, sectionId, groupId);
        return File(data, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"Marks_Exam{examId}_Subject{subjectId}.xlsx");
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer,Admin,Super Admin,Principal")]
    public async Task<IActionResult> ExportCsv(int examId, int subjectId, int classId, int sectionId, int? groupId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (!User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
        {
            var authorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher?.Id ?? 0, subjectId, classId, sectionId, groupId ?? 0, ct);
            if (!authorized) return Forbid();
        }
        var data = await _markEntryService.ExportMarksToCsvAsync(examId, subjectId, classId, sectionId, groupId);
        return File(Encoding.UTF8.GetBytes(data), "text/csv", $"Marks_Exam{examId}_Subject{subjectId}.csv");
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> Dashboard(CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return Unauthorized();

        var activeYear = await _uow.Repository<AcademicYear>().FirstOrDefaultAsync(x => x.IsActive, ct);
        var activeYearId = activeYear?.Id ?? 1;

        var assignedExams = await _teacherResultRepository.GetTeacherAssignedExamsAsync(teacher.Id, activeYearId, ct);
        var examIds = assignedExams.Select(e => e.ExamId).ToList();

        var assignments = await _uow.Repository<SchoolManagementSystem.Models.Entities.Teachers.TeacherSubjectAssignment>().Query()
            .Where(a => a.TeacherId == teacher.Id && a.IsActive && !a.IsDeleted && a.AcademicYearId == activeYearId)
            .ToListAsync(ct);

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
                var count = await _uow.Repository<MarkEntry>().Query()
                    .CountAsync(m => m.ExamId == examId && m.SubjectId == a.SubjectId && m.ClassId == a.ClassId, ct);
                if (count == 0) pendingEntries++;
                else if (await _uow.Repository<MarkEntry>().Query()
                    .AnyAsync(m => m.ExamId == examId && m.SubjectId == a.SubjectId && m.ClassId == a.ClassId
                        && m.Status == ResultWorkflowStatus.Submitted, ct))
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
        
        var model = new SchoolManagementSystem.Models.ViewModels.Result.MarksAuditLogViewModel
        {
            Logs = logs,
            Exams = (await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted)).ToList(),
            SelectedExamId = examId
        };
        return View(model);
    }
}
