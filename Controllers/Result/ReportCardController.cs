using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class ReportCardController : Controller
{
    private readonly IReportCardService _reportCardService;
    private readonly IUnitOfWork _uow;

    public ReportCardController(IReportCardService reportCardService, IUnitOfWork uow)
    {
        _reportCardService = reportCardService;
        _uow = uow;
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
    [Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller,Teacher,Senior Lecturer,Lecturer,Student")]
    public async Task<IActionResult> Download(int examId, int studentId, CancellationToken ct)
    {
        var pdfBytes = await _reportCardService.GenerateReportCardPdfAsync(examId, studentId, ct);
        if (pdfBytes == null)
            return NotFound("Report card has not been calculated or published yet.");

        return File(pdfBytes, "application/pdf", $"ReportCard_Student_{studentId}_Exam_{examId}.pdf");
    }
}
