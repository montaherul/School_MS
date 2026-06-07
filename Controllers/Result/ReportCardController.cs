using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class ReportCardController : Controller
{
    private readonly IReportCardService _reportCardService;
    private readonly IStudentService _studentService;
    private readonly IUnitOfWork _uow;
    private readonly IStudentExamResultRepository _studentExamResultRepository;

    public ReportCardController(IReportCardService reportCardService, IStudentService studentService, IUnitOfWork uow, IStudentExamResultRepository studentExamResultRepository)
    {
        _reportCardService = reportCardService;
        _studentService = studentService;
        _uow = uow;
        _studentExamResultRepository = studentExamResultRepository;
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller,Teacher,Senior Lecturer,Lecturer")]
    public async Task<IActionResult> Index(int? examId, int? classId, int? sectionId, CancellationToken ct)
    {
        var exams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted);
        var classes = await _uow.Repository<SchoolClass>().ListAsync(x => !x.IsDeleted);

        ViewBag.Exams = exams;
        ViewBag.Classes = classes;
        ViewBag.SelectedExamId = examId;
        ViewBag.SelectedClassId = classId;
        ViewBag.SelectedSectionId = sectionId;

        if (classId.HasValue)
        {
            var sections = await _uow.Repository<Section>().ListAsync(x => x.SchoolClassId == classId.Value && !x.IsDeleted);
            ViewBag.Sections = sections;
        }

        var studentResults = new List<StudentExamResult>();

        if (examId.HasValue && classId.HasValue)
        {
            var query = _uow.Repository<StudentExamResult>().Query()
                .Include(r => r.Student)
                .Include(r => r.Exam)
                .Where(r => r.ExamId == examId.Value && r.Student.ClassId == classId.Value && !r.IsDeleted);

            if (sectionId.HasValue && sectionId > 0)
            {
                query = query.Where(r => r.Student.SectionId == sectionId.Value);
            }

            studentResults = await query.OrderBy(r => r.Student.RollNumber).ToListAsync(ct);
        }

        return View(studentResults);
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller,Teacher,Senior Lecturer,Lecturer,Student,Guardian")]
    public async Task<IActionResult> Download(int examId, int studentId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        bool isAdmin = User.IsInRole("Admin") || User.IsInRole("Super Admin") || User.IsInRole("Principal") || User.IsInRole("Exam Controller");

        if (!isAdmin)
        {
            if (User.IsInRole("Student"))
            {
                var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
                if (student == null || student.Id != studentId)
                    return Forbid();
            }
            else if (User.IsInRole("Guardian"))
            {
                var guardianRepo = _uow.Repository<SchoolManagementSystem.Models.Entities.Guardian.StudentGuardian>();
                var hasAccess = await guardianRepo.AnyAsync(sg => sg.Guardian!.UserId == currentUserId && sg.StudentId == studentId);
                if (!hasAccess) return Forbid();
            }
            else if (User.IsInRole("Teacher") || User.IsInRole("Senior Lecturer") || User.IsInRole("Lecturer"))
            {
                var studentObj = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().GetByIdAsync(studentId);
                if (studentObj == null)
                    return Forbid();
            }
            else
            {
                return Forbid();
            }
        }

        var examResult = await _uow.Repository<StudentExamResult>()
            .FirstOrDefaultAsync(r => r.ExamId == examId && r.StudentId == studentId && !r.IsDeleted
                && (r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked), ct);

        if (examResult == null)
            return NotFound("Report card has not been calculated or published yet.");

        var pdfBytes = await _reportCardService.GenerateReportCardPdfAsync(examId, studentId, ct);
        if (pdfBytes == null)
            return NotFound("Report card has not been calculated or published yet.");

        return File(pdfBytes, "application/pdf", $"ReportCard_Student_{studentId}_Exam_{examId}.pdf");
    }

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller,Teacher,Senior Lecturer,Lecturer,Student,Guardian")]
    public async Task<IActionResult> PrintFormat(int examId, int studentId, CancellationToken ct)
        => await BangladeshFormat(examId, studentId, ct);

    [HttpGet]
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller,Teacher,Senior Lecturer,Lecturer,Student,Guardian")]
    public async Task<IActionResult> BangladeshFormat(int examId, int studentId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        bool isAdmin = User.IsInRole("Admin") || User.IsInRole("Super Admin") || User.IsInRole("Principal") || User.IsInRole("Exam Controller");

        if (!isAdmin)
        {
            if (User.IsInRole("Student"))
            {
                var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
                if (student == null || student.Id != studentId)
                    return Forbid();
            }
            else if (User.IsInRole("Guardian"))
            {
                var guardianRepo = _uow.Repository<SchoolManagementSystem.Models.Entities.Guardian.StudentGuardian>();
                var hasAccess = await guardianRepo.AnyAsync(sg => sg.Guardian!.UserId == currentUserId && sg.StudentId == studentId);
                if (!hasAccess) return Forbid();
            }
            else if (User.IsInRole("Teacher") || User.IsInRole("Senior Lecturer") || User.IsInRole("Lecturer"))
            {
                var studentObj = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().GetByIdAsync(studentId);
                if (studentObj == null)
                    return Forbid();
            }
            else
            {
                return Forbid();
            }
        }

        var dto = await _studentExamResultRepository.GetReportCardAsync(examId, studentId, ct);
        if (dto == null)
            return NotFound("Report card has not been calculated or published yet.");

        return View(dto);
    }
}
