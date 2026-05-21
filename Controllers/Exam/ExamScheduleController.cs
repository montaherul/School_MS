using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Models.ViewModels.Shared;

namespace SchoolManagementSystem.Controllers.Exam;

[Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]
public class ExamScheduleController : Controller
{
    private readonly IUnitOfWork _uow;

    public ExamScheduleController(IUnitOfWork uow)
    {
        _uow = uow;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? examId, int? classId, CancellationToken ct)
    {
        var exams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted);
        var classes = await _uow.Repository<SchoolClass>().ListAsync(x => !x.IsDeleted);

        ViewBag.Exams = exams;
        ViewBag.Classes = classes;
        ViewBag.SelectedExamId = examId;
        ViewBag.SelectedClassId = classId;

        var query = _uow.Repository<ExamSchedule>().Query()
            .Include(s => s.Exam)
            .Include(s => s.Subject).ThenInclude(sub => sub.ClassSubjects).ThenInclude(cs => cs.SchoolClass)
            .Where(s => !s.IsDeleted);

        if (examId.HasValue && examId > 0)
        {
            query = query.Where(s => s.ExamId == examId.Value);
        }

        if (classId.HasValue && classId > 0)
        {
            query = query.Where(s => s.Subject.ClassSubjects.Any(cs => cs.SchoolClassId == classId.Value));
        }

        var schedules = await query.OrderBy(s => s.ExamDate).ThenBy(s => s.StartsAt).ToListAsync(ct);
        return View(schedules);
    }

    [HttpGet]
    public async Task<IActionResult> Routine(int examId, int classId, CancellationToken ct)
    {
        var exam = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().GetByIdAsync(examId);
        var schoolClass = await _uow.Repository<SchoolClass>().GetByIdAsync(classId);

        if (exam == null || schoolClass == null)
            return NotFound("Exam or Class not found.");

        var schedules = await _uow.Repository<ExamSchedule>().Query()
            .Include(s => s.Subject).ThenInclude(sub => sub.ClassSubjects).ThenInclude(cs => cs.SchoolClass)
            .Where(s => s.ExamId == examId && s.Subject.ClassSubjects.Any(cs => cs.SchoolClassId == classId) && !s.IsDeleted)
            .OrderBy(s => s.ExamDate)
            .ThenBy(s => s.StartsAt)
            .ToListAsync(ct);

        ViewBag.Exam = exam;
        ViewBag.Class = schoolClass;

        return View(schedules);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var exams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted);
        var subjects = await _uow.Repository<ClassSubject>().Query()
            .Include(cs => cs.Subject)
            .Include(cs => cs.SchoolClass)
            .Where(cs => !cs.IsDeleted)
            .ToListAsync(ct);

        ViewBag.Exams = exams;
        ViewBag.Subjects = subjects;

        return View(new ExamSchedule());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamSchedule model, CancellationToken ct)
    {
        if (ModelState.IsValid)
        {
            // Validate Schedule Conflict
            var conflict = await _uow.Repository<ExamSchedule>().Query()
                .AnyAsync(s => s.ExamDate == model.ExamDate && 
                               s.RoomNo == model.RoomNo && 
                               s.StartsAt < model.EndsAt && 
                               s.EndsAt > model.StartsAt && 
                               !s.IsDeleted, ct);

            if (conflict)
            {
                ModelState.AddModelError("", "Room conflict detected! Another exam is scheduled in this room at the same time.");
                var exams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted);
                var subjects = await _uow.Repository<ClassSubject>().Query().Include(cs => cs.Subject).Include(cs => cs.SchoolClass).Where(cs => !cs.IsDeleted).ToListAsync(ct);
                ViewBag.Exams = exams;
                ViewBag.Subjects = subjects;
                return View(model);
            }

            model.CreatedAt = DateTime.UtcNow;
            await _uow.Repository<ExamSchedule>().AddAsync(model);
            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        var listExams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted);
        var listSubjects = await _uow.Repository<ClassSubject>().Query().Include(cs => cs.Subject).Include(cs => cs.SchoolClass).Where(cs => !cs.IsDeleted).ToListAsync(ct);
        ViewBag.Exams = listExams;
        ViewBag.Subjects = listSubjects;
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var schedule = await _uow.Repository<ExamSchedule>().GetByIdAsync(id);
        if (schedule == null) return NotFound();

        var exams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted);
        var subjects = await _uow.Repository<ClassSubject>().Query().Include(cs => cs.Subject).Include(cs => cs.SchoolClass).Where(cs => !cs.IsDeleted).ToListAsync(ct);

        ViewBag.Exams = exams;
        ViewBag.Subjects = subjects;

        return View(schedule);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ExamSchedule model, CancellationToken ct)
    {
        if (ModelState.IsValid)
        {
            var existing = await _uow.Repository<ExamSchedule>().GetByIdAsync(model.Id);
            if (existing == null) return NotFound();

            // Validate Schedule Conflict
            var conflict = await _uow.Repository<ExamSchedule>().Query()
                .AnyAsync(s => s.Id != model.Id &&
                               s.ExamDate == model.ExamDate && 
                               s.RoomNo == model.RoomNo && 
                               s.StartsAt < model.EndsAt && 
                               s.EndsAt > model.StartsAt && 
                               !s.IsDeleted, ct);

            if (conflict)
            {
                ModelState.AddModelError("", "Room conflict detected! Another exam is scheduled in this room at the same time.");
                var exams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted);
                var subjects = await _uow.Repository<ClassSubject>().Query().Include(cs => cs.Subject).Include(cs => cs.SchoolClass).Where(cs => !cs.IsDeleted).ToListAsync(ct);
                ViewBag.Exams = exams;
                ViewBag.Subjects = subjects;
                return View(model);
            }

            existing.ExamId = model.ExamId;
            existing.SubjectId = model.SubjectId;
            existing.ExamDate = model.ExamDate;
            existing.StartsAt = model.StartsAt;
            existing.EndsAt = model.EndsAt;
            existing.RoomNo = model.RoomNo;
            existing.Instructions = model.Instructions;
            existing.UpdatedAt = DateTime.UtcNow;

            _uow.Repository<ExamSchedule>().Update(existing);
            await _uow.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        var listExams = await _uow.Repository<SchoolManagementSystem.Models.Entities.Exam.Exam>().ListAsync(x => !x.IsDeleted);
        var listSubjects = await _uow.Repository<ClassSubject>().Query().Include(cs => cs.Subject).Include(cs => cs.SchoolClass).Where(cs => !cs.IsDeleted).ToListAsync(ct);
        ViewBag.Exams = listExams;
        ViewBag.Subjects = listSubjects;
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var schedule = await _uow.Repository<ExamSchedule>().GetByIdAsync(id);
        if (schedule == null) return Json(new { success = false, message = "Schedule not found" });

        schedule.IsDeleted = true;
        _uow.Repository<ExamSchedule>().Update(schedule);
        await _uow.SaveChangesAsync();

        return Json(new { success = true });
    }
}
