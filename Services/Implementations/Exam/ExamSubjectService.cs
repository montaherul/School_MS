using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Exam;
using SchoolManagementSystem.Services.Interfaces.Exam;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Services.Implementations.Exam;

public class ExamSubjectService : IExamSubjectService
{
    private readonly IUnitOfWork _uow;
    private readonly ISubjectMarkStructureService _markStructureService;
    private readonly IExamValidationService _examValidation;

    public ExamSubjectService(IUnitOfWork uow, ISubjectMarkStructureService markStructureService, IExamValidationService examValidation)
    {
        _uow = uow;
        _markStructureService = markStructureService;
        _examValidation = examValidation;
    }

    public async Task<ExamSubjectSetupViewModel> GetSubjectSetupAsync(int examId)
    {
        var exam = await _uow.Repository<ExamEntity>().Query().AsNoTracking()
            .Include(e => e.ExamSubjects)
                .ThenInclude(es => es.Subject)
            .FirstOrDefaultAsync(e => e.Id == examId)
            ?? throw new KeyNotFoundException($"Exam with ID {examId} not found.");

        var classSubjects = await _uow.Repository<ClassSubject>().Query()
            .AsNoTracking()
            .Include(cs => cs.Subject)
            .Where(cs => !cs.IsDeleted && cs.IsActive && cs.SchoolClassId == exam.ClassId)
            .ToListAsync();

        if (exam.StudentGroupId.HasValue)
            classSubjects = classSubjects.Where(cs =>
                cs.StudentGroupId == null || cs.StudentGroupId == exam.StudentGroupId.Value).ToList();

        var classNumber = ExtractClassNumber(exam.ClassId);
        if (classNumber >= 1 && classNumber <= 8)
        {
            classSubjects = classSubjects.Where(cs => cs.Subject == null ||
                string.IsNullOrEmpty(cs.Subject.SubjectGroup) ||
                cs.Subject.SubjectGroup == "General").ToList();
        }
        else if (classNumber >= 9 && classNumber <= 10 && exam.StudentGroupId.HasValue)
        {
            var group = await _uow.Repository<StudentGroup>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == exam.StudentGroupId.Value);
            if (group != null)
            {
                classSubjects = classSubjects.Where(cs => cs.Subject == null ||
                    cs.Subject.SubjectGroup == group.Name ||
                    cs.Subject.SubjectGroup == "Common" ||
                    string.IsNullOrEmpty(cs.Subject.SubjectGroup)).ToList();
            }
        }

        var groupName = exam.StudentGroupId.HasValue
            ? (await _uow.Repository<StudentGroup>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == exam.StudentGroupId.Value))?.Name
            : null;

        var existingSubjects = exam.ExamSubjects.ToDictionary(es => es.SubjectId);

        var teachers = await _uow.Repository<Teacher>().Query()
            .AsNoTracking()
            .Include(t => t.Employee)
            .Where(t => !t.IsDeleted && t.Status == TeacherStatus.Active)
            .Select(t => new TeacherOption
            {
                Id = t.Id,
                Name = t.Employee != null ? t.Employee.FullName : t.TeacherCode
            })
            .ToListAsync();

        // Load component previews from SubjectMarkStructure for each subject
        var subjectIds = classSubjects.Select(cs => cs.SubjectId).ToList();
        var componentPreviews = await _markStructureService.GetComponentPreviewsAsync(subjectIds);

        var previewLookup = componentPreviews.ToDictionary(p => p.SubjectId);

        var subjects = classSubjects
            .Select(cs =>
            {
                existingSubjects.TryGetValue(cs.SubjectId, out var existing);
                previewLookup.TryGetValue(cs.SubjectId, out var preview);
                var totalFullMarks = preview?.Components?.Sum(c => c.FullMarks) ?? existing?.FullMarks ?? 100;
                var passMarks = existing?.PassMarks ?? 33;
                return new ExamSubjectConfigDto
                {
                    Id = existing?.Id,
                    SubjectId = cs.SubjectId,
                    SubjectName = cs.Subject?.Name ?? "",
                    TeacherId = existing?.TeacherId,
                    PassMark = passMarks,
                    FullMarks = totalFullMarks,
                    ExamDate = existing?.ExamDate,
                    ExamStartTime = existing?.ExamStartTime,
                    ExamDuration = existing?.ExamDuration,
                    RoomNumber = existing?.RoomNumber,
                    IsOptional = existing?.IsOptional ?? cs.IsOptional,
                    IsActive = existing?.IsActive ?? true,
                    ComponentPreview = preview?.Components ?? []
                };
            })
            .ToList();

        return new ExamSubjectSetupViewModel
        {
            ExamId = examId,
            ExamName = exam.Name,
            ExamType = exam.Term.ToString(),
            GroupName = groupName,
            Subjects = subjects,
            Teachers = teachers
        };
    }

    public async Task SetupSubjectsAsync(int examId, List<ExamSubjectConfigDto> subjects)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId)
            ?? throw new KeyNotFoundException($"Exam with ID {examId} not found.");

        var classId = exam.ClassId;
        var studentGroupId = exam.StudentGroupId;

        // Validate that all active subjects have SubjectMarkStructure configured
        var activeSubjectIds = subjects.Where(s => s.IsActive).Select(s => s.SubjectId).ToList();
        await _examValidation.ThrowIfSubjectMarkStructureMissingAsync(activeSubjectIds);

        var existingSubjects = await _uow.Repository<ExamSubject>().Query()
            .Where(es => es.ExamId == examId)
            .ToListAsync();

        var incomingSubjectIds = subjects.Where(s => s.IsActive).Select(s => s.SubjectId).ToHashSet();

        var toRemove = existingSubjects.Where(es => !incomingSubjectIds.Contains(es.SubjectId)).ToList();
        foreach (var item in toRemove)
            _uow.Repository<ExamSubject>().Remove(item);

        // Load component previews for validation
        var componentPreviews = await _markStructureService.GetComponentPreviewsAsync(incomingSubjectIds.ToList());
        var previewLookup = componentPreviews.ToDictionary(p => p.SubjectId);

        foreach (var dto in subjects.Where(s => s.IsActive))
        {
            if (!dto.TeacherId.HasValue)
                throw new InvalidOperationException($"Please assign a teacher for '{dto.SubjectName}'.");

            // FullMarks come from SubjectMarkStructure — never from manual entry
            previewLookup.TryGetValue(dto.SubjectId, out var preview);
            var fullMarks = preview?.Components?.Sum(c => c.FullMarks) ?? dto.FullMarks;

            var existing = existingSubjects.FirstOrDefault(es => es.SubjectId == dto.SubjectId);
            if (existing != null)
            {
                existing.TeacherId = dto.TeacherId;
                existing.PassMarks = dto.PassMark;
                existing.FullMarks = fullMarks;
                existing.ExamDate = dto.ExamDate;
                existing.ExamStartTime = dto.ExamStartTime;
                existing.ExamDuration = dto.ExamDuration;
                existing.RoomNumber = dto.RoomNumber;
                existing.IsOptional = dto.IsOptional;
                existing.IsActive = true;
                _uow.Repository<ExamSubject>().Update(existing);
            }
            else
            {
                await _uow.Repository<ExamSubject>().AddAsync(new ExamSubject
                {
                    ExamId = examId,
                    SubjectId = dto.SubjectId,
                    ClassId = classId,
                    StudentGroupId = studentGroupId,
                    TeacherId = dto.TeacherId,
                    FullMarks = fullMarks,
                    PassMarks = dto.PassMark,
                    ExamDate = dto.ExamDate,
                    ExamStartTime = dto.ExamStartTime,
                    ExamDuration = dto.ExamDuration,
                    RoomNumber = dto.RoomNumber,
                    IsOptional = dto.IsOptional,
                    IsActive = true
                });
            }
        }

        await _uow.SaveChangesAsync();
    }

    public async Task<ExamSubjectConfigDto> GetSubjectSetupAsyncBySubjectId(int examSubjectId)
    {
        var examSubject = await _uow.Repository<ExamSubject>().Query()
            .Include(es => es.Subject)
            .Include(es => es.Exam)
            .FirstOrDefaultAsync(es => es.Id == examSubjectId)
            ?? throw new KeyNotFoundException($"ExamSubject with ID {examSubjectId} not found.");

        var teachers = await _uow.Repository<Teacher>().Query()
            .AsNoTracking()
            .Include(t => t.Employee)
            .Where(t => !t.IsDeleted && t.Status == TeacherStatus.Active)
            .Select(t => new TeacherOption
            {
                Id = t.Id,
                Name = t.Employee != null ? t.Employee.FullName : t.TeacherCode
            })
            .ToListAsync();

        var previews = await _markStructureService.GetComponentPreviewsAsync([examSubject.SubjectId]);
        var preview = previews.FirstOrDefault();

        return new ExamSubjectConfigDto
        {
            SubjectId = examSubject.SubjectId,
            SubjectName = examSubject.Subject?.Name ?? "",
            TeacherId = examSubject.TeacherId,
            FullMarks = examSubject.FullMarks,
            PassMark = examSubject.PassMarks,
            ExamDate = examSubject.ExamDate,
            ExamStartTime = examSubject.ExamStartTime,
            ExamDuration = examSubject.ExamDuration,
            RoomNumber = examSubject.RoomNumber,
            IsOptional = examSubject.IsOptional,
            IsActive = examSubject.IsActive,
            Teachers = teachers,
            ComponentPreview = preview?.Components
        };
    }

    public async Task UpdateSubjectConfigAsync(int examSubjectId, ExamSubjectConfigDto dto)
    {
        var examSubject = await _uow.Repository<ExamSubject>().GetByIdAsync(examSubjectId)
            ?? throw new KeyNotFoundException($"ExamSubject with ID {examSubjectId} not found.");

        if (!dto.TeacherId.HasValue)
            throw new InvalidOperationException("Teacher assignment is required.");

        examSubject.TeacherId = dto.TeacherId;
        examSubject.PassMarks = dto.PassMark;
        examSubject.FullMarks = dto.FullMarks;
        examSubject.ExamDate = dto.ExamDate;
        examSubject.ExamStartTime = dto.ExamStartTime;
        examSubject.ExamDuration = dto.ExamDuration;
        examSubject.RoomNumber = dto.RoomNumber;
        examSubject.IsOptional = dto.IsOptional;

        _uow.Repository<ExamSubject>().Update(examSubject);
        await _uow.SaveChangesAsync();
    }

    public async Task RemoveSubjectAsync(int examSubjectId)
    {
        var existing = await _uow.Repository<ExamSubject>().GetByIdAsync(examSubjectId)
            ?? throw new KeyNotFoundException($"ExamSubject with ID {examSubjectId} not found.");

        var hasMarks = await _uow.Repository<MarkEntry>().Query()
            .AnyAsync(m => m.ExamId == existing.ExamId && m.SubjectId == existing.SubjectId);

        if (hasMarks)
            throw new InvalidOperationException("Cannot remove a subject that already has marks entered.");

        _uow.Repository<ExamSubject>().Remove(existing);
        await _uow.SaveChangesAsync();
    }

    public async Task<List<ExamScheduleDto>> GetScheduleAsync(int examId)
    {
        var schedules = await _uow.Repository<ExamSchedule>().Query()
            .AsNoTracking()
            .Include(s => s.Subject)
            .Where(s => s.ExamId == examId)
            .OrderBy(s => s.ExamDate)
            .ThenBy(s => s.StartsAt)
            .ToListAsync();

        return schedules.Select(s => new ExamScheduleDto
        {
            SubjectId = s.SubjectId,
            SubjectName = s.Subject?.Name ?? "",
            ExamDate = s.ExamDate,
            StartTime = s.StartsAt,
            EndTime = s.EndsAt,
            RoomNumber = s.RoomNo
        }).ToList();
    }

    public async Task SaveScheduleAsync(int examId, List<ExamScheduleDto> schedules)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId);

        var existingSchedules = await _uow.Repository<ExamSchedule>().Query()
            .Where(s => s.ExamId == examId)
            .ToListAsync();

        var examSubjects = await _uow.Repository<ExamSubject>().Query()
            .Where(es => es.ExamId == examId)
            .ToListAsync();

        var examSubjectLookup = examSubjects.ToDictionary(es => es.SubjectId, es => es);

        // Validate schedule subjects match exam's group (cross-group prevention)
        if (exam != null && exam.StudentGroupId.HasValue)
        {
            var classSubjects = await _uow.Repository<ClassSubject>().Query()
                .AsNoTracking()
                .Include(cs => cs.Subject)
                .Where(cs => cs.SchoolClassId == exam.ClassId && !cs.IsDeleted && cs.IsActive)
                .ToListAsync();

            var group = await _uow.Repository<StudentGroup>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(g => g.Id == exam.StudentGroupId.Value);

            foreach (var dto in schedules.Where(s => s.ExamDate != default))
            {
                var cs = classSubjects.FirstOrDefault(c => c.SubjectId == dto.SubjectId);
                if (cs?.Subject != null && group != null)
                {
                    var subjectGroup = cs.Subject.SubjectGroup ?? "";
                    if (!string.IsNullOrEmpty(subjectGroup) &&
                        subjectGroup != "Common" &&
                        !subjectGroup.Equals(group.Name, StringComparison.OrdinalIgnoreCase) &&
                        !subjectGroup.Equals("General", StringComparison.OrdinalIgnoreCase))
                    {
                        throw new InvalidOperationException(
                            $"Subject '{cs.Subject.Name}' belongs to '{subjectGroup}' group, " +
                            $"but this exam is for '{group.Name}' group. " +
                            "Cross-group subject scheduling is not allowed.");
                    }
                }
            }
        }

        foreach (var dto in schedules.Where(s => s.ExamDate != default))
        {
            if (!string.IsNullOrEmpty(dto.RoomNumber))
            {
                var roomConflict = existingSchedules.Any(s =>
                    s.ExamDate == dto.ExamDate && s.RoomNo == dto.RoomNumber
                    && s.StartsAt < dto.EndTime && s.EndsAt > dto.StartTime
                    && s.SubjectId != dto.SubjectId);
                if (roomConflict)
                {
                    throw new InvalidOperationException(
                        $"Room '{dto.RoomNumber}' is already scheduled for another subject on {dto.ExamDate:dd MMM yyyy} during the same time slot.");
                }
            }

            if (examSubjectLookup.TryGetValue(dto.SubjectId, out var examSubj) && examSubj.TeacherId.HasValue)
            {
                var teacherConflict = existingSchedules.Any(s =>
                    s.ExamDate == dto.ExamDate
                    && s.StartsAt < dto.EndTime && s.EndsAt > dto.StartTime
                    && s.SubjectId != dto.SubjectId
                    && examSubjectLookup.TryGetValue(s.SubjectId, out var sExamSubj)
                    && sExamSubj.TeacherId == examSubj.TeacherId);
                if (teacherConflict)
                    throw new InvalidOperationException(
                        $"Teacher is already assigned to another subject during the same time slot on {dto.ExamDate:dd MMM yyyy}.");
            }

            var overlap = existingSchedules.Any(s =>
                s.ExamDate == dto.ExamDate
                && s.StartsAt < dto.EndTime && s.EndsAt > dto.StartTime
                && s.SubjectId == dto.SubjectId);
            if (overlap)
                throw new InvalidOperationException(
                    $"Duplicate schedule entry for subject '{dto.SubjectName}' on {dto.ExamDate:dd MMM yyyy}.");
        }

        foreach (var old in existingSchedules)
            _uow.Repository<ExamSchedule>().Remove(old);

        foreach (var dto in schedules)
        {
            var classId = dto.ClassId ?? 0;
            if (classId == 0)
            {
                classId = examSubjectLookup.TryGetValue(dto.SubjectId, out var es)
                    ? await _uow.Repository<ClassSubject>().Query()
                        .Where(cs => cs.SubjectId == dto.SubjectId)
                        .Select(cs => cs.SchoolClassId)
                        .FirstOrDefaultAsync()
                    : 0;
            }

            await _uow.Repository<ExamSchedule>().AddAsync(new ExamSchedule
            {
                ExamId = examId,
                SubjectId = dto.SubjectId,
                ClassId = classId,
                StudentGroupId = dto.StudentGroupId,
                SectionId = dto.SectionId,
                ExamDate = dto.ExamDate,
                StartsAt = dto.StartTime,
                EndsAt = dto.EndTime,
                RoomNo = dto.RoomNumber
            });
        }

        await _uow.SaveChangesAsync();
    }

    /// <summary>
    /// Get sibling exams (same group key) for the same academic year
    /// </summary>
    public async Task<List<ExamListDto>> GetSiblingExamsAsync(int examId)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId)
            ?? throw new KeyNotFoundException($"Exam with ID {examId} not found.");

        var allExams = await _uow.Repository<ExamEntity>().Query()
            .AsNoTracking()
            .Where(e => e.AcademicYearId == exam.AcademicYearId
                && e.Name == exam.Name && e.Id != examId && !e.IsDeleted)
            .Select(e => new ExamListDto
            {
                Id = e.Id,
                Name = e.Name,
                Term = e.Term,
                AcademicYearId = e.AcademicYearId,
                ClassId = e.ClassId,
                SubjectCount = e.ExamSubjects.Count(es => !es.IsDeleted),
                Status = e.Status,
                CreatedAt = e.CreatedAt
            })
            .ToListAsync();

        return allExams;
    }

    /// <summary>
    /// Copy subject structure (ExamSubject configs) from source exam to one or more target exams.
    /// Skips subjects that already exist in the target.
    /// Returns number of subjects copied.
    /// </summary>
    public async Task<int> CopySubjectStructureAsync(int sourceExamId, List<int> targetExamIds)
    {
        var sourceSubjects = await _uow.Repository<ExamSubject>().Query()
            .Where(es => es.ExamId == sourceExamId && !es.IsDeleted)
            .ToListAsync();

        if (sourceSubjects.Count == 0)
            throw new InvalidOperationException("Source exam has no subjects configured.");

        var copied = 0;

        foreach (var targetExamId in targetExamIds)
        {
            var targetExam = await _uow.Repository<ExamEntity>().GetByIdAsync(targetExamId)
                ?? throw new KeyNotFoundException($"Target exam with ID {targetExamId} not found.");

            var existingSubjectIds = await _uow.Repository<ExamSubject>().Query()
                .Where(es => es.ExamId == targetExamId && !es.IsDeleted)
                .Select(es => es.SubjectId)
                .ToListAsync();

            var toAdd = sourceSubjects
                .Where(ss => !existingSubjectIds.Contains(ss.SubjectId))
                .ToList();

            foreach (var source in toAdd)
            {
                await _uow.Repository<ExamSubject>().AddAsync(new ExamSubject
                {
                    ExamId = targetExamId,
                    SubjectId = source.SubjectId,
                    ClassId = targetExam.ClassId,
                    StudentGroupId = targetExam.StudentGroupId,
                    TeacherId = source.TeacherId,
                    FullMarks = source.FullMarks,
                    PassMarks = source.PassMarks,
                    ExamDate = source.ExamDate,
                    ExamStartTime = source.ExamStartTime,
                    ExamDuration = source.ExamDuration,
                    RoomNumber = source.RoomNumber,
                    IsOptional = source.IsOptional,
                    IsActive = source.IsActive
                });
                copied++;
            }

            // Copy schedules for newly added subjects
            var sourceSchedule = await _uow.Repository<ExamSchedule>().Query()
                .Where(s => s.ExamId == sourceExamId)
                .ToListAsync();

            var existingScheduleSubjectIds = await _uow.Repository<ExamSchedule>().Query()
                .Where(s => s.ExamId == targetExamId)
                .Select(s => s.SubjectId)
                .ToListAsync();

            var scheduleToAdd = sourceSchedule
                .Where(ss => toAdd.Any(a => a.SubjectId == ss.SubjectId)
                    && !existingScheduleSubjectIds.Contains(ss.SubjectId))
                .ToList();

            foreach (var sched in scheduleToAdd)
            {
                await _uow.Repository<ExamSchedule>().AddAsync(new ExamSchedule
                {
                    ExamId = targetExamId,
                    SubjectId = sched.SubjectId,
                    ClassId = targetExam.ClassId,
                    StudentGroupId = targetExam.StudentGroupId,
                    SectionId = sched.SectionId,
                    ExamDate = sched.ExamDate,
                    StartsAt = sched.StartsAt,
                    EndsAt = sched.EndsAt,
                    RoomNo = sched.RoomNo
                });
            }
        }

        await _uow.SaveChangesAsync();
        return copied;
    }

    private int ExtractClassNumber(int classId)
    {
        var cls = _uow.Repository<SchoolClass>().Query()
            .AsNoTracking()
            .FirstOrDefault(c => c.Id == classId);
        if (cls == null || string.IsNullOrEmpty(cls.Name)) return 0;
        var digits = new string(cls.Name.Where(char.IsDigit).ToArray());
        if (int.TryParse(digits, out var number) && number > 0 && number <= 12)
            return number;
        return 0;
    }
}
