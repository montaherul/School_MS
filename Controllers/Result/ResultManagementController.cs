using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;
using System.Text.Json;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class ResultManagementController : Controller
{
    private readonly IExamService _examService;
    private readonly IResultPublicationService _publicationService;
    private readonly IMarkEntryService _markEntryService;
    private readonly IReEvaluationService _reEvaluationService;
    private readonly IReportCardService _reportCardService;
    private readonly ITeacherService _teacherService;
    private readonly IStudentService _studentService;
    private readonly IAcademicYearService _academicYearService;
    private readonly ISectionService _sectionService;
    private readonly ITeacherAssignmentService _assignmentService;
    private readonly ITranscriptService _transcriptService;
    private readonly IUnitOfWork _uow;
    private readonly IResultAuthorizationService _resultAuthService;
    private readonly ISubjectMarkStructureService _markStructureService;

    public ResultManagementController(
        IExamService examService,
        IResultPublicationService publicationService,
        IMarkEntryService markEntryService,
        IReEvaluationService reEvaluationService,
        IReportCardService reportCardService,
        ITeacherService teacherService,
        IStudentService studentService,
        IAcademicYearService academicYearService,
        ISectionService sectionService,
        ITeacherAssignmentService assignmentService,
        ITranscriptService transcriptService,
        IUnitOfWork uow,
        IResultAuthorizationService resultAuthService,
        ISubjectMarkStructureService markStructureService)
    {
        _examService = examService;
        _publicationService = publicationService;
        _markEntryService = markEntryService;
        _reEvaluationService = reEvaluationService;
        _reportCardService = reportCardService;
        _teacherService = teacherService;
        _studentService = studentService;
        _academicYearService = academicYearService;
        _sectionService = sectionService;
        _assignmentService = assignmentService;
        _transcriptService = transcriptService;
        _uow = uow;
        _resultAuthService = resultAuthService;
        _markStructureService = markStructureService;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    public async Task<IActionResult> AdminIndex(CancellationToken ct)
    {
        var exams = (await _examService.GetExamsAsync(0, ct)).ToList();
        var model = new SchoolManagementSystem.Models.ViewModels.Exam.ExamListViewModel
        {
            Exams = exams
        };
        return View(model);
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> TeacherEntry(CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return NotFound("Teacher profile not found.");

        // Get active academic year
        var academicYears = await _academicYearService.GetPagedAsync(1, 100, string.Empty, ct);
        var activeYear = academicYears.Items.FirstOrDefault(ay => ay.IsActive);
        var academicYearId = activeYear?.Id ?? 1; // Default to 1 if no active year found

        var model = new SchoolManagementSystem.Models.ViewModels.Result.TeacherEntryViewModel
        {
            TeacherId = teacher.Id,
            Assignments = (await _assignmentService.GetTeacherClassAssignmentsAsync(teacher.Id, ct)).ToList(),
            Exams = (await _examService.GetExamsAsync(academicYearId)).ToList()
        };
        return View(model);
    }

    [HttpGet]
    [Authorize(Roles = "Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> GetSubjectsForTeacher(int classId, int? groupId, int? sectionId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return Json(new List<object>());

        var subjects = await _assignmentService.GetTeacherAssignedSubjectsAsync(teacher.Id, classId, groupId, sectionId, ct);
        return Json(subjects.Select(s => new { subjectId = s.Id, subjectName = s.Name }));
    }

    [HttpGet]
    public async Task<IActionResult> GetSectionsForClass(int classId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return Json(new List<object>());

        var assignments = await _assignmentService.GetTeacherClassAssignmentsAsync(teacher.Id, ct);
        var sections = assignments.Where(a => a.ClassId == classId).Select(a => new { id = a.SectionId, name = a.SectionName }).Distinct();
        return Json(sections);
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> StudentIndex(CancellationToken ct)
    {
        return RedirectToAction(nameof(StudentDashboard));
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> StudentDashboard(CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
        if (student == null)
        {
            TempData["Error"] = "Student profile not found.";
            return RedirectToAction("Index", "Home");
        }

        var dto = await _publicationService.GetStudentResultsAsync(student.Id);
        if (dto == null)
        {
            TempData["Error"] = "Result data not found.";
            return RedirectToAction("Index", "Home");
        }

        return View(ToViewModel(dto));
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> StudentReportCards(int? academicYearId, int? examId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
        if (student == null) return RedirectToAction("StudentDashboard");

        var dto = await _publicationService.GetStudentResultsAsync(student.Id);
        if (dto == null) return RedirectToAction("StudentDashboard");

        var academicYears = await _academicYearService.GetPagedAsync(1, 100, string.Empty, ct);
        ViewBag.AcademicYears = academicYears.Items;
        ViewBag.SelectedAcademicYearId = academicYearId ?? 0;
        ViewBag.SelectedExamId = examId ?? 0;

            if (academicYearId.HasValue && academicYearId > 0)
            {
                ViewBag.Exams = await _uow.Repository<ExamEntity>().ListAsync(e => e.AcademicYearId == academicYearId && !e.IsDeleted, ct);
                var yearExamIds = (await _uow.Repository<ExamEntity>().ListAsync(e => e.AcademicYearId == academicYearId && !e.IsDeleted, ct))
                    .Select(e => e.Id).ToHashSet();
                dto.ExamResults = dto.ExamResults.Where(e => yearExamIds.Contains(e.ExamId)).ToList();
            }
            else
            {
                ViewBag.Exams = await _uow.Repository<ExamEntity>().ListAsync(e => !e.IsDeleted, ct);
            }

            if (examId.HasValue && examId > 0)
                dto.ExamResults = dto.ExamResults.Where(e => e.ExamId == examId).ToList();

            return View(ToViewModel(dto));
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> StudentTranscript(int? academicYearId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
        if (student == null) return RedirectToAction("StudentDashboard");

        var academicYears = await _academicYearService.GetPagedAsync(1, 100, string.Empty, ct);
        var activeYear = academicYears.Items.FirstOrDefault(y => y.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        if (yearId == 0)
        {
            TempData["Error"] = "No academic year selected.";
            return RedirectToAction("StudentDashboard");
        }

        var transcript = await _transcriptService.GetStudentTranscriptAsync(student.Id, yearId);
        if (transcript == null)
        {
            TempData["Error"] = "Transcript not found for the selected academic year.";
            return RedirectToAction("StudentDashboard");
        }

        ViewBag.AcademicYears = academicYears.Items;
        ViewBag.SelectedAcademicYearId = yearId;
        return View(transcript);
    }

    [HttpGet]
    public async Task<IActionResult> GetExams(CancellationToken ct)
    {
        var exams = await _examService.GetExamsAsync(0);
        return Json(exams);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetSubjectsForClass(int classId, int? groupId, int? sectionId, CancellationToken ct)
    {
        var subjects = await _examService.GetSubjectsByClassIdAsync(classId, groupId, sectionId, ct);
        return Json(subjects);
    }

    [HttpGet]
    [Authorize(Roles = "Principal,Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> MarkEntry(int examId, int classId, int sectionId, int subjectId, int? groupId, CancellationToken ct)
    {
        if (examId <= 0 || classId <= 0 || sectionId <= 0 || subjectId <= 0) return BadRequest();

        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return Unauthorized();

        if (!User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
        {
            var exam = await _examService.GetExamByIdAsync(examId, ct);
            var acYearId = exam is SchoolManagementSystem.Models.Entities.Exam.Exam e ? e.AcademicYearId : 0;
            var authorized = await _resultAuthService.IsAuthorizedToEnterMarksAsync(teacher.Id, subjectId, classId, sectionId, acYearId, ct);
            if (!authorized) return Forbid();
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

        var columns = await _markStructureService.GetGridColumnsAsync(subjectId, classId, groupId);

        ViewBag.ComponentColumns = columns.Select(c => new {
            name = c.ComponentName,
            field = c.FieldName,
            max = c.FullMarks
        }).ToList();

        return View(vm);
    }

    [HttpPost]
    [Authorize(Roles = "Principal,Teacher,Senior Lecturer,Lecturer")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> SaveMarks([FromBody] MarkBatchDto dto, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return Json(new { success = false, message = "Teacher not found" });
        
        if (!User.IsInRole("Admin") && !User.IsInRole("Super Admin") && !User.IsInRole("Principal"))
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
                    if (!isAuthorized) return Json(new { success = false, message = "Unauthorized to enter marks for one or more students" });
                }
            }
        }
        
        dto.TeacherId = teacher.Id;
        await _markEntryService.SubmitMarksBatchAsync(dto);
        return Json(new { success = true });
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> PublishResults([FromBody] ResultPublishDto dto)
    {
        try 
        {
            await _publicationService.PublishResultsAsync(dto);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    public async Task<IActionResult> ReEvaluationDashboard()
    {
        var dto = await _reEvaluationService.GetReEvaluationDashboardAsync();
        var model = new ReEvaluationDashboardViewModel
        {
            PendingRequests = dto.PendingRequests.Select(r => new ReEvaluationRequestViewModel
            {
                Id = r.Id,
                StudentId = r.StudentId,
                SubjectId = r.SubjectId,
                ExamId = r.ExamId,
                ExamName = r.ExamName,
                StudentName = r.StudentName,
                SubjectName = r.SubjectName,
                OldMarks = r.OldMarks,
                NewMarks = r.NewMarks,
                Status = r.Status,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt
            }).ToList(),
            CompletedRequests = dto.CompletedRequests.Select(r => new ReEvaluationRequestViewModel
            {
                Id = r.Id,
                StudentId = r.StudentId,
                SubjectId = r.SubjectId,
                ExamId = r.ExamId,
                ExamName = r.ExamName,
                StudentName = r.StudentName,
                SubjectName = r.SubjectName,
                OldMarks = r.OldMarks,
                NewMarks = r.NewMarks,
                Status = r.Status,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt
            }).ToList()
        };
        return View(model);
    }

    [HttpPost]
    [Authorize(Roles = "Admin,Super Admin,Principal")]
    [IgnoreAntiforgeryToken]
    public async Task<IActionResult> ProcessReEvaluation([FromBody] ReEvaluationProcessDto dto)
    {
        var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        await _reEvaluationService.ProcessReEvaluationAsync(dto, adminId);
        return Json(new { success = true });
    }

    [HttpGet]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> RequestReEvaluation(int examId, int subjectId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
        if (student == null) return RedirectToAction("StudentIndex");

        var dto = new ReEvaluationRequestDto { ExamId = examId, SubjectId = subjectId, StudentId = student.Id };
        return View(dto);
    }

    [HttpPost]
    [Authorize(Roles = "Student")]
    public async Task<IActionResult> RequestReEvaluation(ReEvaluationRequestDto dto)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        await _reEvaluationService.RequestReEvaluationAsync(dto, currentUserId);
        return RedirectToAction("StudentIndex");
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> DownloadReportCard(int examId, int studentId)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        bool isAdmin = User.IsInRole("Admin") || User.IsInRole("Super Admin") || User.IsInRole("Principal");

        if (!isAdmin)
        {
            if (User.IsInRole("Student"))
            {
                var student = await _studentService.GetByUserIdAsync(currentUserId, default);
                if (student == null || student.Id != studentId)
                    return Forbid();
            }
            else if (User.IsInRole("Guardian"))
            {
                var guardianRepo = _uow.Repository<SchoolManagementSystem.Models.Entities.Guardian.StudentGuardian>();
                var hasAccess = await guardianRepo.AnyAsync(sg => sg.Guardian!.UserId == currentUserId && sg.StudentId == studentId);
                if (!hasAccess)
                    return Forbid();
            }
            else if (User.IsInRole("Teacher") || User.IsInRole("Senior Lecturer") || User.IsInRole("Lecturer"))
            {
                var teacher = await _teacherService.GetByUserIdAsync(currentUserId, default);
                if (teacher == null) return Forbid();
                var assignments = await _assignmentService.GetTeacherClassAssignmentsAsync(teacher.Id, default);
                var studentObj = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().GetByIdAsync(studentId);
                if (studentObj == null || !assignments.Any(a => a.ClassId == studentObj.ClassId))
                    return Forbid();
            }
            else
            {
                return Forbid();
            }
        }

        var result = await _publicationService.GetStudentResultsAsync(studentId);
        if (result == null || !result.ExamResults.Any())
            return NotFound("No published results found");

        var pdf = await _reportCardService.GenerateReportCardPdfAsync(examId, studentId);
        if (pdf == null) return NotFound();
        return File(pdf, "application/pdf", $"ReportCard_{studentId}_{examId}.pdf");
    }

    private static StudentPortalResultViewModel ToViewModel(StudentPortalResultDto dto) => new()
    {
        StudentId = dto.StudentId,
        StudentName = dto.StudentName,
        ClassName = dto.ClassName,
        SectionName = dto.SectionName,
        RollNumber = dto.RollNumber,
        ExamResults = dto.ExamResults,
        FinalResult = dto.FinalResult,
        Transcript = dto.Transcript
    };
}

