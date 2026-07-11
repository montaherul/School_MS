using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Exam;
using SchoolManagementSystem.Services.Interfaces.Exam;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;

namespace SchoolManagementSystem.Services.Implementations.Exam;

public class AdmitCardService : IAdmitCardService
{
    private readonly IUnitOfWork _uow;
    private readonly IViewRendererService _viewRenderer;
    private readonly IPdfGenerator _pdfGenerator;

    public AdmitCardService(
        IUnitOfWork uow,
        IViewRendererService viewRenderer,
        IPdfGenerator pdfGenerator)
    {
        _uow = uow;
        _viewRenderer = viewRenderer;
        _pdfGenerator = pdfGenerator;
    }

    public async Task GenerateAdmitCardsAsync(int examId)
    {
        var exam = await _uow.Repository<ExamEntity>().GetByIdAsync(examId)
            ?? throw new KeyNotFoundException($"Exam with ID {examId} not found");

        var classIds = await _uow.Repository<ExamSchedule>().Query()
            .Where(es => es.ExamId == examId)
            .Select(es => es.ClassId)
            .Distinct()
            .ToListAsync();

        if (classIds.Count == 0)
        {
            var subjectIds = await _uow.Repository<ExamSubject>().Query()
                .Where(es => es.ExamId == examId)
                .Select(es => es.SubjectId)
                .ToListAsync();

            classIds = await _uow.Repository<ClassSubject>().Query()
                .Where(cs => subjectIds.Contains(cs.SubjectId) && !cs.IsDeleted)
                .Select(cs => cs.SchoolClassId)
                .Distinct()
                .ToListAsync();
        }

        if (classIds.Count == 0)
            throw new InvalidOperationException("Cannot determine which classes take this exam. Configure exam subjects first.");

        var studentsQuery = _uow.Repository<Student>().Query()
            .Where(s => classIds.Contains(s.ClassId) && !s.IsDeleted && s.Status == StudentStatus.Active);

        if (exam.StudentGroupId.HasValue)
            studentsQuery = studentsQuery.Where(s => s.StudentGroupId == exam.StudentGroupId.Value);

        var students = await studentsQuery
            .OrderBy(s => s.RollNumber)
            .ThenBy(s => s.FullName)
            .ToListAsync();

        var existingCards = await _uow.Repository<AdmitCard>().Query()
            .Where(a => a.ExamId == examId)
            .ToListAsync();

        var existingStudentIds = existingCards.Select(a => a.StudentId).ToHashSet();

        int rollSeq = existingCards.Any()
            ? existingCards.Max(a => a.RollNumber) ?? 0
            : 0;

        var academicYear = await _uow.Repository<AcademicYear>().GetByIdAsync(exam.AcademicYearId);
        var yearShort = academicYear?.Name?.Length >= 2 ? academicYear.Name[^2..] : "00";

        var newCards = new List<AdmitCard>();
        foreach (var student in students)
        {
            if (existingStudentIds.Contains(student.Id))
                continue;

            rollSeq++;
            newCards.Add(new AdmitCard
            {
                ExamId = examId,
                StudentId = student.Id,
                CardNo = $"AC-{examId}-{student.Id}",
                AdmitCardNumber = $"AC-{yearShort}-{examId}-{student.Id:D6}",
                RollNumber = rollSeq,
                IsGenerated = true,
                IsIssued = true,
                IssuedAt = DateTime.UtcNow
            });
        }

        if (newCards.Count > 0)
        {
            await _uow.Repository<AdmitCard>().AddRangeAsync(newCards);
            await _uow.SaveChangesAsync();
        }
    }

    public async Task<AdmitCardViewModel> GetAdmitCardAsync(int examId, int studentId)
    {
        var admitCard = await _uow.Repository<AdmitCard>().Query()
            .AsNoTracking()
            .Include(a => a.Exam)
            .Include(a => a.Student)
                .ThenInclude(s => s.Class)
            .Include(a => a.Student)
                .ThenInclude(s => s.Section)
            .Include(a => a.Student)
                .ThenInclude(s => s.StudentGroup)
            .FirstOrDefaultAsync(a => a.ExamId == examId && a.StudentId == studentId && !a.IsDeleted)
            ?? throw new KeyNotFoundException("Admit card not found for this student and exam.");

        var academicYear = await _uow.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(y => y.Id == admitCard.Exam.AcademicYearId);

        var schoolSetting = await _uow.Repository<SchoolSetting>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync();

        var examSubjects = await _uow.Repository<ExamSubject>().Query()
            .AsNoTracking()
            .Include(es => es.Subject)
            .Where(es => es.ExamId == examId)
            .ToListAsync();

        var schedules = await _uow.Repository<ExamSchedule>().Query()
            .AsNoTracking()
            .Where(es => es.ExamId == examId)
            .ToListAsync();

        return new AdmitCardViewModel
        {
            ExamId = examId,
            StudentId = studentId,
            StudentName = admitCard.Student.FullName,
            StudentPhotoPath = admitCard.Student.ProfilePicturePath,
            RollNumber = admitCard.RollNumber,
            AdmitCardNumber = admitCard.AdmitCardNumber,
            SeatNumber = admitCard.SeatNumber,
            ClassName = admitCard.Student.Class?.Name ?? "",
            SectionName = admitCard.Student.Section?.Name ?? "",
            GroupName = admitCard.Student.StudentGroup?.Name,
            AcademicYear = academicYear?.Name ?? "",
            ExamName = admitCard.Exam.Name,
            ExamType = admitCard.Exam.Term.ToString(),
            ExamStartDate = admitCard.Exam.StartsOn,
            ExamEndDate = admitCard.Exam.EndsOn,
            SchoolName = schoolSetting?.SchoolName ?? "School Management System",
            SchoolAddress = schoolSetting?.Address ?? "",
            SchoolLogo = schoolSetting?.LogoPath,
            EIIN = schoolSetting?.EIIN ?? "",
            IsIssued = admitCard.IsIssued,
            IssuedAt = admitCard.IssuedAt,
            SubjectSchedules = examSubjects
                .Select(es =>
                {
                    var schedule = schedules.FirstOrDefault(s => s.SubjectId == es.SubjectId);
                    return new AdmitCardSubjectRow
                    {
                        SubjectName = es.Subject?.Name ?? "",
                        ExamDate = schedule?.ExamDate ?? default,
                        StartTime = schedule?.StartsAt.ToString(@"hh\:mm") ?? "",
                        Duration = "",
                        RoomNumber = schedule?.RoomNo ?? ""
                    };
                })
                .OrderBy(s => s.ExamDate)
                .ThenBy(s => s.StartTime)
                .ToList()
        };
    }

    public async Task<byte[]> GenerateAdmitCardPdfAsync(int examId, int studentId)
    {
        var vm = await GetAdmitCardAsync(examId, studentId);
        var html = await _viewRenderer.RenderToStringAsync("AdmitCardPdf", vm);
        return _pdfGenerator.GenerateFromHtml(html);
    }

    public async Task<byte[]> GenerateBulkAdmitCardsPdfAsync(int examId, int? sectionId)
    {
        var query = _uow.Repository<AdmitCard>().Query()
            .AsNoTracking()
            .Include(a => a.Student)
                .ThenInclude(s => s.Class)
            .Include(a => a.Student)
                .ThenInclude(s => s.Section)
            .Include(a => a.Student)
                .ThenInclude(s => s.StudentGroup)
            .Include(a => a.Exam)
            .Where(a => a.ExamId == examId && !a.IsDeleted && a.IsGenerated);

        if (sectionId.HasValue)
            query = query.Where(a => a.Student.SectionId == sectionId.Value);

        var admitCards = await query
            .OrderBy(a => a.RollNumber)
            .ToListAsync();

        if (admitCards.Count == 0)
            throw new InvalidOperationException("No admit cards found for this exam.");

        var academicYear = await _uow.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(y => y.Id == admitCards[0].Exam.AcademicYearId);

        var schoolSetting = await _uow.Repository<SchoolSetting>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync();

        var examSubjects = await _uow.Repository<ExamSubject>().Query()
            .AsNoTracking()
            .Include(es => es.Subject)
            .Where(es => es.ExamId == examId)
            .ToListAsync();

        var schedules = await _uow.Repository<ExamSchedule>().Query()
            .AsNoTracking()
            .Where(es => es.ExamId == examId)
            .ToListAsync();

        var htmlParts = new List<string>();
        foreach (var card in admitCards)
        {
            var vm = new AdmitCardViewModel
            {
                ExamId = examId,
                StudentId = card.StudentId,
                StudentName = card.Student.FullName,
                StudentPhotoPath = card.Student.ProfilePicturePath,
                RollNumber = card.RollNumber,
                AdmitCardNumber = card.AdmitCardNumber,
                SeatNumber = card.SeatNumber,
                ClassName = card.Student.Class?.Name ?? "",
                SectionName = card.Student.Section?.Name ?? "",
                GroupName = card.Student.StudentGroup?.Name,
                AcademicYear = academicYear?.Name ?? "",
                ExamName = card.Exam.Name,
                ExamType = card.Exam.Term.ToString(),
                ExamStartDate = card.Exam.StartsOn,
                ExamEndDate = card.Exam.EndsOn,
                SchoolName = schoolSetting?.SchoolName ?? "School Management System",
                SchoolAddress = schoolSetting?.Address ?? "",
                SchoolLogo = schoolSetting?.LogoPath,
                EIIN = schoolSetting?.EIIN ?? "",
                IsIssued = card.IsIssued,
                IssuedAt = card.IssuedAt,
                SubjectSchedules = examSubjects
                    .Select(es =>
                    {
                        var schedule = schedules.FirstOrDefault(s => s.SubjectId == es.SubjectId);
                        return new AdmitCardSubjectRow
                        {
                            SubjectName = es.Subject?.Name ?? "",
                            ExamDate = schedule?.ExamDate ?? default,
                            StartTime = schedule?.StartsAt.ToString(@"hh\:mm") ?? "",
                            Duration = "",
                            RoomNumber = schedule?.RoomNo ?? ""
                        };
                    })
                    .OrderBy(s => s.ExamDate)
                    .ThenBy(s => s.StartTime)
                    .ToList()
            };
            var html = await _viewRenderer.RenderToStringAsync("AdmitCardPdf", vm);
            htmlParts.Add(html);
        }

        var combinedHtml = string.Join("<div style='page-break-after: always;'></div>", htmlParts);
        return _pdfGenerator.GenerateFromHtml(combinedHtml);
    }

    public async Task<bool> IsAdmitCardGeneratedAsync(int examId)
    {
        return await _uow.Repository<AdmitCard>().Query()
            .AnyAsync(a => a.ExamId == examId && a.IsGenerated && !a.IsDeleted);
    }
}
