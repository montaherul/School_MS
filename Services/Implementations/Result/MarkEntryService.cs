using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

/// <summary>
/// Mark entry service with draft/submit workflow and teacher validation
/// </summary>
public class MarkEntryService : IMarkEntryService
{
    private readonly IUnitOfWork _uow;
    private readonly SchoolDbContext _db;

    public MarkEntryService(IUnitOfWork uow, SchoolDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    public async Task<MarkEntrySheet> GetMarkEntrySheetAsync(int examId, int subjectId, int teacherId)
    {
        // Validate teacher permission
        if (!await ValidateTeacherPermissionAsync(examId, subjectId, teacherId))
            throw new UnauthorizedAccessException("Teacher does not have permission to enter marks for this subject");

        // Get exam and subject details
        var exam = await _db.Exams.FindAsync(examId);
        var subject = await _db.Subjects.FindAsync(subjectId);
        var classSubject = await _db.ClassSubjects
            .Include(cs => cs.SchoolClass)
            .FirstOrDefaultAsync(cs => cs.SubjectId == subjectId);

        if (exam == null || subject == null || classSubject == null)
            throw new ArgumentException("Invalid exam, subject, or class configuration");

        // Get students for the class
        var students = await _db.Students
            .Where(s => s.ClassId == classSubject.SchoolClassId)
            .OrderBy(s => s.RollNumber)
            .ToListAsync();

        // Get existing marks if any
        var existingMarks = await _db.Marks
            .Where(m => m.ExamId == examId && m.SubjectId == subjectId)
            .ToDictionaryAsync(m => m.StudentId);

        // Check if marks are locked
        var isLocked = await _db.Marks
            .AnyAsync(m => m.ExamId == examId && m.SubjectId == subjectId && m.IsLocked);

        var isSubmitted = await _db.Marks
            .AnyAsync(m => m.ExamId == examId && m.SubjectId == subjectId && m.Status == ResultWorkflowStatus.Submitted);

        var sheet = new MarkEntrySheet
        {
            ExamId = examId,
            SubjectId = subjectId,
            ExamName = exam.Name,
            SubjectName = subject.Name,
            ClassName = classSubject.SchoolClass.Name,
            IsLocked = isLocked,
            IsSubmitted = isSubmitted,
            Config = new MarkEntryConfig
            {
                FullMarks = classSubject.FullMarks,
                PassMarks = classSubject.PassMarks,
                HasWritten = subject.HasWritten || classSubject.WrittenMarks.HasValue,
                HasMCQ = subject.HasMCQ || classSubject.MCQMarks.HasValue,
                HasCQ = subject.HasCQ || classSubject.CQMarks.HasValue,
                HasPractical = subject.HasPractical || classSubject.PracticalMarks.HasValue,
                HasViva = subject.HasViva || classSubject.VivaMarks.HasValue,
                HasLab = subject.HasLab || classSubject.LabMarks.HasValue,
                HasOral = subject.HasOral || classSubject.OralMarks.HasValue,
                HasAssignment = subject.HasAssignment || classSubject.AssignmentMarks.HasValue,
                HasContinuousAssessment = subject.HasContinuousAssessment || classSubject.ContinuousAssessmentMarks.HasValue,
                HasCompetency = classSubject.CompetencyMarks.HasValue,
                HasBehaviour = classSubject.BehaviourMarks.HasValue,
                HasParticipation = classSubject.ParticipationMarks.HasValue
            }
        };

        // Build student marks
        foreach (var student in students)
        {
            var studentMark = new StudentMarkEntry
            {
                StudentId = student.Id,
                StudentName = student.FullName,
                RollNumber = student.RollNumber
            };

            if (existingMarks.TryGetValue(student.Id, out var mark))
            {
                studentMark.WrittenMarks = mark.WrittenMarks;
                studentMark.MCQMarks = mark.MCQMarks;
                studentMark.CQMarks = mark.CQMarks;
                studentMark.PracticalMarks = mark.PracticalMarks;
                studentMark.VivaMarks = mark.VivaMarks;
                studentMark.LabMarks = mark.LabMarks;
                studentMark.OralMarks = mark.OralMarks;
                studentMark.AssignmentMarks = mark.AssignmentMarks;
                studentMark.ContinuousAssessmentMarks = mark.ContinuousAssessmentMarks;
                studentMark.CompetencyMarks = mark.CompetencyMarks;
                studentMark.BehaviourMarks = mark.BehaviourMarks;
                studentMark.ParticipationMarks = mark.ParticipationMarks;
                studentMark.TotalMarks = mark.MarksObtained;
            }

            sheet.StudentMarks.Add(studentMark);
        }

        return sheet;
    }

    public async Task SaveDraftMarksAsync(MarkEntrySheet sheet, int teacherId)
    {
        // Fetch all existing marks for this exam and subject to avoid N+1 queries
        var existingMarks = await _db.Marks
            .Where(m => m.ExamId == sheet.ExamId && m.SubjectId == sheet.SubjectId)
            .ToDictionaryAsync(m => m.StudentId);

        foreach (var studentMark in sheet.StudentMarks)
        {
            existingMarks.TryGetValue(studentMark.StudentId, out var existingMark);

            if (existingMark != null && existingMark.IsLocked) continue;

            if (existingMark == null)
            {
                existingMark = new MarkEntry
                {
                    ExamId = sheet.ExamId,
                    StudentId = studentMark.StudentId,
                    SubjectId = sheet.SubjectId,
                    EnteredByTeacherId = teacherId,
                    Status = ResultWorkflowStatus.Draft,
                    CreatedByUserId = teacherId,
                    UpdatedByUserId = teacherId
                };
                _db.Marks.Add(existingMark);
            }
            else
            {
                // Audit old marks - add to context without saving yet
                CreateMarkAuditLog(existingMark, teacherId, "Draft Update");
                existingMark.UpdatedByUserId = teacherId;
            }

            // Update marks
            existingMark.WrittenMarks = studentMark.WrittenMarks;
            existingMark.MCQMarks = studentMark.MCQMarks;
            existingMark.CQMarks = studentMark.CQMarks;
            existingMark.PracticalMarks = studentMark.PracticalMarks;
            existingMark.VivaMarks = studentMark.VivaMarks;
            existingMark.LabMarks = studentMark.LabMarks;
            existingMark.OralMarks = studentMark.OralMarks;
            existingMark.AssignmentMarks = studentMark.AssignmentMarks;
            existingMark.ContinuousAssessmentMarks = studentMark.ContinuousAssessmentMarks;
            existingMark.CompetencyMarks = studentMark.CompetencyMarks;
            existingMark.BehaviourMarks = studentMark.BehaviourMarks;
            existingMark.ParticipationMarks = studentMark.ParticipationMarks;

            // Recalculate total
            existingMark.MarksObtained = AggregateTotalMarks(existingMark);
            existingMark.SubmittedAt = null;
        }

        await _db.SaveChangesAsync();
    }

    public async Task SubmitMarksAsync(int examId, int subjectId, int teacherId)
    {
        var marks = await _db.Marks
            .Where(m => m.ExamId == examId && m.SubjectId == subjectId)
            .ToListAsync();

        foreach (var mark in marks)
        {
            if (!mark.IsLocked)
            {
                mark.Status = ResultWorkflowStatus.Submitted;
                mark.SubmittedAt = DateTime.Now;
                mark.UpdatedByUserId = teacherId;

                // Create audit log - add to context without saving yet
                CreateMarkAuditLog(mark, teacherId, "Submitted for approval");
            }
        }

        await _db.SaveChangesAsync();
    }

    public async Task<bool> ValidateTeacherPermissionAsync(int examId, int subjectId, int teacherId)
    {
        // Check if teacher is assigned to this subject
        var assignment = await _db.ClassSubjectTeachers
            .Include(cst => cst.ClassSubject)
            .AnyAsync(cst => cst.ClassSubject.SubjectId == subjectId && cst.TeacherId == teacherId);

        return assignment;
    }

    public async Task LockMarksAsync(int examId, int subjectId, int adminId)
    {
        var marks = await _db.Marks
            .Where(m => m.ExamId == examId && m.SubjectId == subjectId)
            .ToListAsync();

        foreach (var mark in marks)
        {
            mark.IsLocked = true;
            mark.LockedAt = DateTime.Now;
            mark.UpdatedByUserId = adminId;
        }

        // Create result lock record
        var resultLock = new ResultLock
        {
            ExamId = examId,
            LockedByUserId = adminId,
            LockedAt = DateTime.Now,
            Reason = $"Marks locked for subject {subjectId}"
        };
        _db.ResultLocks.Add(resultLock);

        await _db.SaveChangesAsync();
    }

    public async Task UnlockMarksAsync(int examId, int subjectId, int adminId, string reason)
    {
        var marks = await _db.Marks
            .Where(m => m.ExamId == examId && m.SubjectId == subjectId)
            .ToListAsync();

        foreach (var mark in marks)
        {
            mark.IsLocked = false;
            mark.LockedAt = null;
            mark.UpdatedByUserId = adminId;
        }

        // Update result lock record
        var resultLock = await _db.ResultLocks
            .FirstOrDefaultAsync(rl => rl.ExamId == examId);

        if (resultLock != null)
        {
            resultLock.CanUnlock = false; // Mark as unlocked
        }

        await _db.SaveChangesAsync();
    }

    public async Task<BulkImportResult> BulkImportMarksAsync(Stream fileStream, int examId, int subjectId, int teacherId)
    {
        // Implementation for Excel/CSV import would go here
        // For now, return empty result
        return new BulkImportResult
        {
            SuccessCount = 0,
            ErrorCount = 0,
            Errors = ["Bulk import not implemented yet"],
            Warnings = []
        };
    }

    public async Task<IEnumerable<MarkAuditEntry>> GetMarkAuditTrailAsync(int examId, int subjectId)
    {
        var auditLogs = await _db.MarkAuditLogs
            .Include(a => a.MarkEntry)
            .Where(a => a.MarkEntry.ExamId == examId && a.MarkEntry.SubjectId == subjectId)
            .OrderByDescending(a => a.CreatedAt)
            .Select(a => new MarkAuditEntry
            {
                ChangedAt = a.CreatedAt,
                ChangedBy = $"User {a.ChangedByUserId}",
                ChangeType = "Mark Change",
                OldMarks = a.OldMarks,
                NewMarks = a.NewMarks,
                Reason = a.Reason ?? ""
            })
            .ToListAsync();

        return auditLogs;
    }

    public async Task RecalculateMarksAsync(int examId, int subjectId)
    {
        var marks = await _db.Marks
            .Where(m => m.ExamId == examId && m.SubjectId == subjectId)
            .ToListAsync();

        foreach (var mark in marks)
        {
            mark.MarksObtained = AggregateTotalMarks(mark);
        }

        await _db.SaveChangesAsync();
    }

    private void CreateMarkAuditLog(MarkEntry markEntry, int changedByUserId, string reason)
    {
        var auditLog = new MarkAuditLog
        {
            MarkEntryId = markEntry.Id,
            OldMarks = markEntry.MarksObtained,
            NewMarks = markEntry.MarksObtained, // Will be updated with new value
            ChangedByUserId = changedByUserId,
            Reason = reason
        };

        _db.MarkAuditLogs.Add(auditLog);
    }

    private decimal AggregateTotalMarks(MarkEntry markEntry)
    {
        decimal total = 0;

        total += markEntry.WrittenMarks ?? 0;
        total += markEntry.MCQMarks ?? 0;
        total += markEntry.CQMarks ?? 0;
        total += markEntry.PracticalMarks ?? 0;
        total += markEntry.VivaMarks ?? 0;
        total += markEntry.LabMarks ?? 0;
        total += markEntry.OralMarks ?? 0;
        total += markEntry.AssignmentMarks ?? 0;
        total += markEntry.ContinuousAssessmentMarks ?? 0;
        total += markEntry.CompetencyMarks ?? 0;
        total += markEntry.BehaviourMarks ?? 0;
        total += markEntry.ParticipationMarks ?? 0;

        return total;
    }
}