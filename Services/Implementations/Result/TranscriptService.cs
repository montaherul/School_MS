using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.System;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class TranscriptService : ITranscriptService
{
    private readonly IUnitOfWork _uow;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IStudentSubjectFilterService _subjectFilter;

    public TranscriptService(IUnitOfWork uow, IPdfGenerator pdfGenerator, IStudentSubjectFilterService subjectFilter)
    {
        _uow = uow;
        _pdfGenerator = pdfGenerator;
        _subjectFilter = subjectFilter;
    }

    public async Task<StudentTranscriptDto?> GetStudentTranscriptAsync(int studentId, int academicYearId)
    {
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
            .QueryNoTracking()
            .Include(s => s.Class)
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);
        if (student == null) return null;

        var academicYear = await _uow.Repository<AcademicYear>()
            .FirstOrDefaultAsync(y => y.Id == academicYearId && !y.IsDeleted);
        if (academicYear == null) return null;

        var schoolProfile = await _uow.Repository<SchoolProfile>().Query().FirstOrDefaultAsync();

        var examResults = await _uow.Repository<StudentExamResult>()
            .QueryNoTracking()
            .Include(r => r.Exam)
            .Where(r => r.StudentId == studentId && r.Exam.AcademicYearId == academicYearId
                && (r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked))
            .OrderBy(r => r.Exam.StartsOn)
            .ToListAsync();

        var subjectResults = await _uow.Repository<StudentSubjectResult>()
            .QueryNoTracking()
            .Include(r => r.Subject)
            .Include(r => r.Exam)
            .Where(r => r.StudentId == studentId && r.Exam.AcademicYearId == academicYearId)
            .ToListAsync();

        // Filter subject results based on student's curriculum
        var validSubjectIds = await _subjectFilter.GetValidSubjectIdsForStudentAsync(student);
        if (validSubjectIds.Count > 0)
        {
            subjectResults = subjectResults.Where(sr => validSubjectIds.Contains(sr.SubjectId)).ToList();
        }

        var finalResult = await _uow.Repository<FinalResult>()
            .FirstOrDefaultAsync(f => f.StudentId == studentId && f.AcademicYearId == academicYearId);

        var transcript = new StudentTranscriptDto
        {
            StudentId = student.Id,
            StudentName = student.FullName,
            StudentNameBn = student.FullName,
            FatherName = student.FatherName ?? "",
            MotherName = student.MotherName ?? "",
            DateOfBirth = student.DateOfBirth,
            RollNumber = student.RollNumber,
            RegistrationNumber = student.StudentNo,
            SchoolName = schoolProfile?.Name ?? "",
            SchoolAddress = schoolProfile?.Address ?? "",
            AcademicYearId = academicYearId,
            AcademicYear = academicYear.Name,
            FinalGPA = (finalResult?.FinalGpa) ?? (examResults.Any() ? examResults.Average(r => r.Gpa) : 0),
            FinalGrade = finalResult?.FinalGrade ?? "",
            MeritPosition = finalResult?.FinalPosition ?? 0,
            FinalClassPosition = finalResult?.FinalClassPosition ?? 0,
            FinalSectionPosition = finalResult?.FinalSectionPosition ?? 0,
            FinalGroupPosition = finalResult?.FinalGroupPosition ?? 0,
            WeightedTotalMarks = finalResult?.WeightedTotalMarks ?? 0,
            TotalPassedSubjects = finalResult?.TotalPassedSubjects ?? 0,
            TotalFailedSubjects = finalResult?.TotalFailedSubjects ?? 0,
            AttendancePercentage = finalResult?.AttendancePercentage ?? 0,
            ExamResults = examResults.Select(r => new StudentExamResultDto
            {
                ExamId = r.ExamId,
                ExamName = r.Exam?.Name ?? "",
                Term = r.Exam?.Term ?? ExamTerm.Other,
                Status = r.Status,
                TotalMarks = r.TotalMarks,
                TotalFullMarks = r.TotalFullMarks,
                Gpa = r.Gpa,
                Grade = r.Grade,
                Position = r.Position,
                ClassPosition = r.ClassPosition,
                GroupPosition = r.GroupPosition,
                IsPassed = r.IsPassed,
                FailedSubjectCount = r.FailedSubjectCount,
                PassedSubjectCount = r.PassedSubjectCount,
                PublishedAt = r.PublishedAt,
                Subjects = subjectResults
                    .Where(s => s.ExamId == r.ExamId)
                    .Select(s => new StudentSubjectResultDto
                    {
                        SubjectId = s.SubjectId,
                        SubjectName = s.Subject?.Name ?? "",
                        SubjectNameBn = s.Subject?.NameBn ?? "",
                        SubjectGroup = s.Subject?.SubjectGroup ?? "",
                        MarksObtained = s.MarksObtained,
                        FullMarks = s.FullMarks,
                        PassMarks = s.PassMarks,
                        Grade = s.Grade,
                        GradePoint = s.GradePoint,
                        IsPassed = s.IsPassed
                    }).ToList()
            }).ToList(),
            SubjectWiseResults = subjectResults
                .GroupBy(s => s.SubjectId)
                .Select(g =>
                {
                    var first = g.First();
                    return new SubjectTranscriptDto
                    {
                        SubjectName = first.Subject?.Name ?? "",
                        SubjectNameBn = first.Subject?.NameBn ?? "",
                        TotalMarks = g.Sum(s => s.MarksObtained),
                        FullMarks = g.Sum(s => s.FullMarks),
                        Grade = g.OrderByDescending(s => s.GradePoint).First().Grade,
                        GradePoint = g.Average(s => s.GradePoint),
                        IsPassed = g.All(s => s.IsPassed),
                        SubjectGroup = first.Subject?.SubjectGroup ?? ""
                    };
                }).ToList()
        };

        return transcript;
    }

    public async Task<bool> IsResultBlockedForStudentAsync(int studentId, CancellationToken cancellationToken = default)
    {
        if (await IsResultBlockedAsync(cancellationToken))
            return await HasFeeDueAsync(studentId, cancellationToken);
        return false;
    }

    public async Task<byte[]?> GenerateTranscriptPdfAsync(int studentId, int academicYearId)
    {
        if (await IsResultBlockedForStudentAsync(studentId))
            return null;

        var transcript = await GetStudentTranscriptAsync(studentId, academicYearId);
        if (transcript == null) return null;

        return _pdfGenerator.GenerateTranscript(transcript);
    }

    private async Task<bool> HasFeeDueAsync(int studentId, CancellationToken cancellationToken)
    {
        return await _uow.Repository<FeeInvoice>().AnyAsync(
            x => x.StudentId == studentId && !x.IsDeleted && x.Status != PaymentStatus.Paid && x.Status != PaymentStatus.Waived,
            cancellationToken);
    }

    private async Task<bool> IsResultBlockedAsync(CancellationToken cancellationToken)
    {
        var setting = await _uow.Repository<SchoolSetting>().Query().FirstOrDefaultAsync(cancellationToken);
        if (setting == null) return false;
        return !setting.AllowResultWithDue;
    }
}
