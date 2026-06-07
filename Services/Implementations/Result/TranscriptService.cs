using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class TranscriptService : ITranscriptService
{
    private readonly IUnitOfWork _uow;
    private readonly SchoolDbContext _db;

    public TranscriptService(IUnitOfWork uow, SchoolDbContext db)
    {
        _uow = uow;
        _db = db;
    }

    public async Task<StudentTranscriptDto?> GetStudentTranscriptAsync(int studentId, int academicYearId)
    {
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
            .Query()
            .Include(s => s.Class)
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted);
        if (student == null) return null;

        var academicYear = await _uow.Repository<AcademicYear>()
            .FirstOrDefaultAsync(y => y.Id == academicYearId && !y.IsDeleted);
        if (academicYear == null) return null;

        var schoolProfile = await _db.SchoolProfiles.FirstOrDefaultAsync();

        var examResults = await _uow.Repository<StudentExamResult>()
            .Query()
            .Include(r => r.Exam)
            .Where(r => r.StudentId == studentId && r.Exam.AcademicYearId == academicYearId
                && (r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked))
            .OrderBy(r => r.Exam.StartsOn)
            .ToListAsync();

        var subjectResults = await _uow.Repository<StudentSubjectResult>()
            .Query()
            .Include(r => r.Subject)
            .Include(r => r.Exam)
            .Where(r => r.StudentId == studentId && r.Exam.AcademicYearId == academicYearId)
            .ToListAsync();

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
                        TotalMarks = g.Average(s => s.MarksObtained),
                        FullMarks = g.Average(s => s.FullMarks),
                        Grade = g.OrderByDescending(s => s.GradePoint).First().Grade,
                        GradePoint = g.Average(s => s.GradePoint),
                        IsPassed = g.All(s => s.IsPassed),
                        SubjectGroup = first.Subject?.SubjectGroup ?? ""
                    };
                }).ToList()
        };

        return transcript;
    }

    public async Task<byte[]?> GenerateTranscriptPdfAsync(int studentId, int academicYearId)
    {
        return null;
    }
}
