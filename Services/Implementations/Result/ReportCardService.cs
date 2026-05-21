using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ReportCardService : IReportCardService
{
    private readonly IUnitOfWork _uow;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IStudentExamResultRepository _examResultRepository;
    private readonly IMarkEntryRepository _markEntryRepository;

    public ReportCardService(
        IUnitOfWork uow,
        IPdfGenerator pdfGenerator,
        IStudentExamResultRepository examResultRepository,
        IMarkEntryRepository markEntryRepository)
    {
        _uow = uow;
        _pdfGenerator = pdfGenerator;
        _examResultRepository = examResultRepository;
        _markEntryRepository = markEntryRepository;
    }

    public async Task<byte[]?> GenerateReportCardPdfAsync(int examId, int studentId, CancellationToken ct = default)
    {
        var result = await _examResultRepository.Query()
            .Include(r => r.Student)
            .Include(r => r.Exam)
            .FirstOrDefaultAsync(r => r.ExamId == examId && r.StudentId == studentId && !r.IsDeleted, ct);

        if (result == null) return null;

        var marks = await _markEntryRepository.Query()
            .Include(m => m.Subject)
            .Where(m => m.ExamId == examId && m.StudentId == studentId && !m.IsDeleted)
            .ToListAsync(ct);

        return _pdfGenerator.GenerateSchoolReportCard(result, marks);
    }
}

