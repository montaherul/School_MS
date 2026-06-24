using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;
using ExamScheduleEntity = SchoolManagementSystem.Models.Entities.Exam.ExamSchedule;

namespace SchoolManagementSystem.Controllers.Exam;

using SchoolManagementSystem.Filters;

[RequirePermission("ExamSchedule.Manage")]
public class ExamScheduleController : Controller
{
    private readonly IUnitOfWork _uow;
    private readonly ICalendarGenerationService _calendarGen;

    public ExamScheduleController(IUnitOfWork uow, ICalendarGenerationService calendarGen)
    {
        _uow = uow;
        _calendarGen = calendarGen;
    }

    [HttpGet]
    public async Task<IActionResult> Index(int? examId, int? classId, int? groupId, CancellationToken ct)
    {
        var exams = await _uow.Repository<ExamEntity>().ListAsync(x => !x.IsDeleted, ct);
        var classes = await _uow.Repository<SchoolClass>().ListAsync(x => !x.IsDeleted, ct);
        var groups = await _uow.Repository<StudentGroup>().ListAsync(x => x.IsActive && !x.IsDeleted, ct);

        ViewBag.Exams = exams;
        ViewBag.Classes = classes;
        ViewBag.Groups = groups;
        ViewBag.SelectedExamId = examId;
        ViewBag.SelectedClassId = classId;
        ViewBag.SelectedGroupId = groupId;

        var query = _uow.Repository<ExamScheduleEntity>().Query()
            .Include(s => s.Exam)
            .Include(s => s.Subject)
            .Include(s => s.Class)
            .Include(s => s.StudentGroup)
            .Include(s => s.Section)
            .Where(s => !s.IsDeleted);

        if (examId.HasValue && examId > 0)
            query = query.Where(s => s.ExamId == examId.Value);
        if (classId.HasValue && classId > 0)
            query = query.Where(s => s.ClassId == classId.Value);
        if (groupId.HasValue && groupId > 0)
            query = query.Where(s => s.StudentGroupId == groupId.Value);

        var schedules = await query.OrderBy(s => s.ExamDate).ThenBy(s => s.StartsAt).ToListAsync(ct);
        return View(schedules);
    }

    [HttpGet]
    public async Task<IActionResult> Create(CancellationToken ct)
    {
        var exams = await _uow.Repository<ExamEntity>().ListAsync(x => !x.IsDeleted, ct);
        var classes = await _uow.Repository<SchoolClass>().ListAsync(x => !x.IsDeleted, ct);
        var groups = await _uow.Repository<StudentGroup>().ListAsync(x => x.IsActive && !x.IsDeleted, ct);
        var subjects = await _uow.Repository<ClassSubject>().Query()
            .Include(cs => cs.Subject)
            .Include(cs => cs.SchoolClass)
            .Where(cs => !cs.IsDeleted)
            .ToListAsync(ct);

        ViewBag.Exams = exams;
        ViewBag.Classes = classes;
        ViewBag.Groups = groups;
        ViewBag.Subjects = subjects;

        return View(new ExamScheduleEntity());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(ExamScheduleEntity model, CancellationToken ct)
    {
        if (ModelState.IsValid)
        {
            // Validate: same class + same subject + same group cannot be double-booked
            var duplicate = await _uow.Repository<ExamScheduleEntity>().Query()
                .AnyAsync(s => s.ExamId == model.ExamId &&
                               s.ClassId == model.ClassId &&
                               s.SubjectId == model.SubjectId &&
                               s.StudentGroupId == model.StudentGroupId &&
                               s.ExamDate == model.ExamDate &&
                               !s.IsDeleted, ct);

            if (duplicate)
            {
                ModelState.AddModelError("", "This subject is already scheduled for this class and group on this date.");
                await LoadFormViewBags(ct);
                return View(model);
            }

            // Validate Room Conflict: same date + same room + overlapping time
            var roomConflict = await _uow.Repository<ExamScheduleEntity>().Query()
                .AnyAsync(s => s.ExamDate == model.ExamDate &&
                               s.RoomNo == model.RoomNo &&
                               s.StartsAt < model.EndsAt &&
                               s.EndsAt > model.StartsAt &&
                               !s.IsDeleted, ct);

            if (roomConflict)
            {
                ModelState.AddModelError("", "Room conflict detected! Another exam is scheduled in this room at this time.");
                await LoadFormViewBags(ct);
                return View(model);
            }

            model.CreatedAt = DateTime.UtcNow;
            await _uow.Repository<ExamScheduleEntity>().AddAsync(model, ct);
            await _uow.SaveChangesAsync(ct);

            await SyncCalendarForExamDate(model, ct);
        }

        await LoadFormViewBags(ct);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Edit(int id, CancellationToken ct)
    {
        var schedule = await _uow.Repository<ExamScheduleEntity>().GetByIdAsync(id, ct);
        if (schedule == null) return NotFound();

        await LoadFormViewBags(ct);
        return View(schedule);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(ExamScheduleEntity model, CancellationToken ct)
    {
        if (ModelState.IsValid)
        {
            var existing = await _uow.Repository<ExamScheduleEntity>().GetByIdAsync(model.Id, ct);
            if (existing == null) return NotFound();

            // Validate duplicate
            var duplicate = await _uow.Repository<ExamScheduleEntity>().Query()
                .AnyAsync(s => s.Id != model.Id &&
                               s.ExamId == model.ExamId &&
                               s.ClassId == model.ClassId &&
                               s.SubjectId == model.SubjectId &&
                               s.StudentGroupId == model.StudentGroupId &&
                               s.ExamDate == model.ExamDate &&
                               !s.IsDeleted, ct);

            if (duplicate)
            {
                ModelState.AddModelError("", "This subject is already scheduled for this class and group on this date.");
                await LoadFormViewBags(ct);
                return View(model);
            }

            // Validate Room Conflict
            var roomConflict = await _uow.Repository<ExamScheduleEntity>().Query()
                .AnyAsync(s => s.Id != model.Id &&
                               s.ExamDate == model.ExamDate &&
                               s.RoomNo == model.RoomNo &&
                               s.StartsAt < model.EndsAt &&
                               s.EndsAt > model.StartsAt &&
                               !s.IsDeleted, ct);

            if (roomConflict)
            {
                ModelState.AddModelError("", "Room conflict detected! Another exam is scheduled in this room at this time.");
                await LoadFormViewBags(ct);
                return View(model);
            }

            existing.ExamId = model.ExamId;
            existing.SubjectId = model.SubjectId;
            existing.ClassId = model.ClassId;
            existing.StudentGroupId = model.StudentGroupId;
            existing.SectionId = model.SectionId;
            existing.ExamDate = model.ExamDate;
            existing.StartsAt = model.StartsAt;
            existing.EndsAt = model.EndsAt;
            existing.RoomNo = model.RoomNo;
            existing.Instructions = model.Instructions;
            existing.UpdatedAt = DateTime.UtcNow;

            _uow.Repository<ExamScheduleEntity>().Update(existing);
            await _uow.SaveChangesAsync(ct);

            await SyncCalendarForExamDate(existing, ct);
        }

        await LoadFormViewBags(ct);
        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Details(int id, CancellationToken ct)
    {
        var schedule = await _uow.Repository<ExamScheduleEntity>().Query()
            .Include(s => s.Exam)
            .Include(s => s.Subject)
            .Include(s => s.Class)
            .Include(s => s.Section)
            .Include(s => s.StudentGroup)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);

        if (schedule == null) return NotFound();

        return View(schedule);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id)
    {
        var schedule = await _uow.Repository<ExamScheduleEntity>().GetByIdAsync(id);
        if (schedule == null) return Json(new { success = false, message = "Schedule not found" });

        var examDate = schedule.ExamDate;
        var examId = schedule.ExamId;

        schedule.IsDeleted = true;
        _uow.Repository<ExamScheduleEntity>().Update(schedule);
        await _uow.SaveChangesAsync();

        await SyncCalendarForExamDate(examDate, examId, CancellationToken.None);

        return Json(new { success = true });
    }

    private async Task LoadFormViewBags(CancellationToken ct)
    {
        ViewBag.Exams = await _uow.Repository<ExamEntity>().ListAsync(x => !x.IsDeleted, ct);
        ViewBag.Classes = await _uow.Repository<SchoolClass>().ListAsync(x => !x.IsDeleted, ct);
        ViewBag.Groups = await _uow.Repository<StudentGroup>().ListAsync(x => x.IsActive && !x.IsDeleted, ct);
        ViewBag.Subjects = await _uow.Repository<ClassSubject>().Query()
            .Include(cs => cs.Subject)
            .Include(cs => cs.SchoolClass)
            .Where(cs => !cs.IsDeleted)
            .ToListAsync(ct);
    }

    private async Task SyncCalendarForExamDate(ExamScheduleEntity schedule, CancellationToken ct)
    {
        try
        {
            var academicYear = await _uow.Repository<AcademicYear>().Query()
                .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);

            if (academicYear == null) return;

            var entry = await _uow.Repository<AcademicCalendar>().Query()
                .FirstOrDefaultAsync(x => x.AcademicYearId == academicYear.Id && x.Date == schedule.ExamDate && !x.IsDeleted, ct);

            if (entry == null) return;

            entry.IsExamDay = true;
            entry.IsEventDay = false;
            entry.Title = $"Exam: {schedule.Exam?.Name ?? "Exam"}";
            entry.Description = $"Subject scheduled on {schedule.ExamDate:dd MMM yyyy}";
            entry.UpdatedAt = DateTime.UtcNow;
            entry.UpdatedBy = "system";
        }
        catch
        {
            // Best-effort sync
        }

        await _uow.SaveChangesAsync(ct);
    }

    private async Task SyncCalendarForExamDate(DateOnly examDate, int examId, CancellationToken ct)
    {
        try
        {
            var remaining = await _uow.Repository<ExamScheduleEntity>().Query()
                .AnyAsync(x => x.ExamId == examId && x.ExamDate == examDate && !x.IsDeleted, ct);

            if (remaining) return;

            var academicYear = await _uow.Repository<AcademicYear>().Query()
                .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);

            if (academicYear == null) return;

            var entry = await _uow.Repository<AcademicCalendar>().Query()
                .FirstOrDefaultAsync(x => x.AcademicYearId == academicYear.Id && x.Date == examDate && !x.IsDeleted, ct);

            if (entry == null) return;

            entry.IsExamDay = false;
            entry.IsEventDay = false;
            entry.Title = "Working Day";
            entry.Description = "";
            entry.UpdatedAt = DateTime.UtcNow;
            entry.UpdatedBy = "system";

            await _uow.SaveChangesAsync(ct);
        }
        catch
        {
            // Best-effort sync
        }
    }
}
