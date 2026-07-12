using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Repositories.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Services.Implementations.Teachers;

public class AutoTeacherAssignmentService : IAutoTeacherAssignmentService
{
    private readonly IUnitOfWork _uow;
    private readonly ITeacherSubjectAssignmentRepository _teacherSubjectAssignmentRepo;

    public AutoTeacherAssignmentService(
        IUnitOfWork uow,
        ITeacherSubjectAssignmentRepository teacherSubjectAssignmentRepo)
    {
        _uow = uow;
        _teacherSubjectAssignmentRepo = teacherSubjectAssignmentRepo;
    }

    public async Task<AutoTeacherAssignmentResultDto> AutoAssignTeachersAsync(int examId, CancellationToken ct = default)
    {
        var exam = await _uow.Repository<ExamEntity>().Query()
            .Include(e => e.ExamSubjects).ThenInclude(es => es.Subject)
            .Include(e => e.ExamSubjects).ThenInclude(es => es.Teacher).ThenInclude(t => t!.Employee)
            .FirstOrDefaultAsync(e => e.Id == examId && !e.IsDeleted, ct);

        if (exam == null)
        {
            return new AutoTeacherAssignmentResultDto
            {
                ExamId = examId,
                Success = false,
                Message = $"Exam with ID {examId} not found."
            };
        }

        var activeSubjects = exam.ExamSubjects.Where(es => !es.IsDeleted && es.IsActive).ToList();
        var details = new List<AutoAssignmentDetailDto>();
        var assignedCount = 0;
        var skippedCount = 0;

        foreach (var examSubject in activeSubjects)
        {
            var teacherAssignment = await _teacherSubjectAssignmentRepo.Query()
                .AsNoTracking()
                .Include(tsa => tsa.Teacher).ThenInclude(t => t!.Employee)
                .Where(tsa =>
                    tsa.AcademicYearId == exam.AcademicYearId &&
                    tsa.ClassId == examSubject.ClassId &&
                    tsa.SubjectId == examSubject.SubjectId &&
                    tsa.SectionId == (exam.SectionId ?? 0) &&
                    tsa.GroupId == exam.StudentGroupId &&
                    tsa.IsActive &&
                    !tsa.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (teacherAssignment != null)
            {
                examSubject.TeacherId = teacherAssignment.TeacherId;
                _uow.Repository<ExamSubject>().Update(examSubject);

                assignedCount++;
                details.Add(new AutoAssignmentDetailDto
                {
                    SubjectId = examSubject.SubjectId,
                    SubjectName = examSubject.Subject?.Name ?? "",
                    WasAssigned = true,
                    TeacherId = teacherAssignment.TeacherId,
                    TeacherName = teacherAssignment.Teacher?.FullName ?? "",
                    Reason = "Matched via TeacherSubjectAssignment"
                });
            }
            else
            {
                skippedCount++;
                details.Add(new AutoAssignmentDetailDto
                {
                    SubjectId = examSubject.SubjectId,
                    SubjectName = examSubject.Subject?.Name ?? "",
                    WasAssigned = false,
                    Reason = "No matching TeacherSubjectAssignment found"
                });
            }
        }

        await _uow.SaveChangesAsync(ct);

        return new AutoTeacherAssignmentResultDto
        {
            ExamId = examId,
            ExamName = exam.Name,
            TotalSubjects = activeSubjects.Count,
            Assigned = assignedCount,
            Skipped = skippedCount,
            Success = true,
            Message = $"Auto-assigned {assignedCount} teacher(s), skipped {skippedCount} subject(s) with no matching assignment.",
            Details = details
        };
    }
}
