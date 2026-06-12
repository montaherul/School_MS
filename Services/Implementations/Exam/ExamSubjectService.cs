using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Exam;
using SchoolManagementSystem.Services.Interfaces.Exam;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Services.Implementations.Exam;

public class ExamSubjectService : IExamSubjectService
{
    private readonly IUnitOfWork _uow;

    public ExamSubjectService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<ExamSubjectSetupViewModel> GetSubjectSetupAsync(int examId)
    {
        var exam = await _uow.Repository<ExamEntity>().Query()
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

        // Filter by SubjectGroup based on Bangladesh curriculum
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

        var subjects = classSubjects
            .Select(cs =>
            {
                existingSubjects.TryGetValue(cs.SubjectId, out var existing);
                return new ExamSubjectConfigDto
                {
                    Id = existing?.Id,
                    SubjectId = cs.SubjectId,
                    SubjectName = cs.Subject?.Name ?? "",
                    TeacherId = existing?.TeacherId,
                    TotalWrittenMarks = existing?.TotalWrittenMarks ?? 0,
                    TotalMCQMarks = existing?.TotalMCQMarks ?? 0,
                    TotalPracticalMarks = existing?.TotalPracticalMarks ?? 0,
                    TotalVivaMarks = existing?.TotalVivaMarks ?? 0,
                    TotalAssignmentMarks = existing?.TotalAssignmentMarks ?? 0,
                    PassMark = existing?.PassMarks ?? 33,
                    ExamDate = existing?.ExamDate,
                    ExamStartTime = existing?.ExamStartTime,
                    ExamDuration = existing?.ExamDuration,
                    RoomNumber = existing?.RoomNumber,
                    IsOptional = existing?.IsOptional ?? cs.IsOptional,
                    IsActive = existing?.IsActive ?? true
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

        var existingSubjects = await _uow.Repository<ExamSubject>().Query()
            .Where(es => es.ExamId == examId)
            .ToListAsync();

        var incomingSubjectIds = subjects.Where(s => s.IsActive).Select(s => s.SubjectId).ToHashSet();

        var toRemove = existingSubjects.Where(es => !incomingSubjectIds.Contains(es.SubjectId)).ToList();
        foreach (var item in toRemove)
            _uow.Repository<ExamSubject>().Remove(item);

        foreach (var dto in subjects.Where(s => s.IsActive))
        {
            if (dto.TotalWrittenMarks < 0 || dto.TotalMCQMarks < 0 || dto.TotalPracticalMarks < 0
                || dto.TotalVivaMarks < 0 || dto.TotalAssignmentMarks < 0)
                throw new InvalidOperationException($"Component marks for '{dto.SubjectName}' cannot be negative.");
            var total = dto.TotalWrittenMarks + dto.TotalMCQMarks + dto.TotalPracticalMarks
                        + dto.TotalVivaMarks + dto.TotalAssignmentMarks;
            if (total < 1 || total > 300)
                throw new InvalidOperationException($"Total marks for '{dto.SubjectName}' must be between 1 and 300.");
            if (!dto.TeacherId.HasValue)
                throw new InvalidOperationException($"Please assign a teacher for '{dto.SubjectName}'.");

            var existing = existingSubjects.FirstOrDefault(es => es.SubjectId == dto.SubjectId);
            if (existing != null)
            {
                existing.TeacherId = dto.TeacherId;
                existing.TotalWrittenMarks = dto.TotalWrittenMarks;
                existing.TotalMCQMarks = dto.TotalMCQMarks;
                existing.TotalPracticalMarks = dto.TotalPracticalMarks;
                existing.TotalVivaMarks = dto.TotalVivaMarks;
                existing.TotalAssignmentMarks = dto.TotalAssignmentMarks;
                existing.PassMarks = dto.PassMark;
                existing.ExamDate = dto.ExamDate;
                existing.ExamStartTime = dto.ExamStartTime;
                existing.ExamDuration = dto.ExamDuration;
                existing.RoomNumber = dto.RoomNumber;
                existing.IsOptional = dto.IsOptional;
                existing.IsActive = true;
                existing.FullMarks = total;
                _uow.Repository<ExamSubject>().Update(existing);
            }
            else
            {
                await _uow.Repository<ExamSubject>().AddAsync(new ExamSubject
                {
                    ExamId = examId,
                    SubjectId = dto.SubjectId,
                    TeacherId = dto.TeacherId,
                    TotalWrittenMarks = dto.TotalWrittenMarks,
                    TotalMCQMarks = dto.TotalMCQMarks,
                    TotalPracticalMarks = dto.TotalPracticalMarks,
                    TotalVivaMarks = dto.TotalVivaMarks,
                    TotalAssignmentMarks = dto.TotalAssignmentMarks,
                    FullMarks = total,
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

        return new ExamSubjectConfigDto
        {
            SubjectId = examSubject.SubjectId,
            SubjectName = examSubject.Subject?.Name ?? "",
            TeacherId = examSubject.TeacherId,
            TotalWrittenMarks = examSubject.TotalWrittenMarks,
            TotalMCQMarks = examSubject.TotalMCQMarks,
            TotalPracticalMarks = examSubject.TotalPracticalMarks,
            TotalVivaMarks = examSubject.TotalVivaMarks,
            TotalAssignmentMarks = examSubject.TotalAssignmentMarks,
            PassMark = examSubject.PassMarks,
            ExamDate = examSubject.ExamDate,
            ExamStartTime = examSubject.ExamStartTime,
            ExamDuration = examSubject.ExamDuration,
            RoomNumber = examSubject.RoomNumber,
            IsOptional = examSubject.IsOptional,
            IsActive = examSubject.IsActive,
            Teachers = teachers
        };
    }

    public async Task UpdateSubjectConfigAsync(int examSubjectId, ExamSubjectConfigDto dto)
    {
        var examSubject = await _uow.Repository<ExamSubject>().GetByIdAsync(examSubjectId)
            ?? throw new KeyNotFoundException($"ExamSubject with ID {examSubjectId} not found.");

        if (dto.TotalWrittenMarks < 0 || dto.TotalMCQMarks < 0 || dto.TotalPracticalMarks < 0
            || dto.TotalVivaMarks < 0 || dto.TotalAssignmentMarks < 0)
            throw new InvalidOperationException("Component marks cannot be negative.");
        if (!dto.TeacherId.HasValue)
            throw new InvalidOperationException("Teacher assignment is required.");

        examSubject.TeacherId = dto.TeacherId;
        examSubject.TotalWrittenMarks = dto.TotalWrittenMarks;
        examSubject.TotalMCQMarks = dto.TotalMCQMarks;
        examSubject.TotalPracticalMarks = dto.TotalPracticalMarks;
        examSubject.TotalVivaMarks = dto.TotalVivaMarks;
        examSubject.TotalAssignmentMarks = dto.TotalAssignmentMarks;
        examSubject.PassMarks = dto.PassMark;
        examSubject.ExamDate = dto.ExamDate;
        examSubject.ExamStartTime = dto.ExamStartTime;
        examSubject.ExamDuration = dto.ExamDuration;
        examSubject.RoomNumber = dto.RoomNumber;
        examSubject.IsOptional = dto.IsOptional;
        examSubject.FullMarks = dto.TotalWrittenMarks + dto.TotalMCQMarks + dto.TotalPracticalMarks
                                + dto.TotalVivaMarks + dto.TotalAssignmentMarks;

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

        var allSchedules = await _uow.Repository<ExamSchedule>().Query()
            .Where(s => s.ExamId != examId)
            .ToListAsync();

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
                    // If subject has a SubjectGroup, it must match the exam's group
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
                    var subjectName = dto.SubjectName;
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
