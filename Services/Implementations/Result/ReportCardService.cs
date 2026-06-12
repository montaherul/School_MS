using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ReportCardService : IReportCardService
{
    private readonly IUnitOfWork _uow;
    private readonly IPdfGenerator _pdfGenerator;
    private readonly IStudentExamResultRepository _examResultRepository;
    private readonly IMarkEntryRepository _markEntryRepository;
    private readonly ISchoolSettingRepository _schoolSettingRepository;
    private readonly IStudentSubjectFilterService _subjectFilter;


    public ReportCardService(
        IUnitOfWork uow,
        IPdfGenerator pdfGenerator,
        IStudentExamResultRepository examResultRepository,
        IMarkEntryRepository markEntryRepository,
        ISchoolSettingRepository schoolSettingRepository,
        IStudentSubjectFilterService subjectFilter
        )
    {
        _uow = uow;
        _pdfGenerator = pdfGenerator;
        _examResultRepository = examResultRepository;
        _markEntryRepository = markEntryRepository;
        _schoolSettingRepository = schoolSettingRepository;
        _subjectFilter = subjectFilter;
    }

    public async Task<byte[]?> GenerateReportCardPdfAsync(int examId, int studentId, CancellationToken ct = default)
    {
        var result = await _examResultRepository.Query()
            .Include(r => r.Student)
            .Include(r => r.Exam)
            .FirstOrDefaultAsync(r => r.ExamId == examId && r.StudentId == studentId && !r.IsDeleted
                && (r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked), ct);

        if (result == null) return null;

        // Get the full student entity for subject filtering
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .Where(s => s.Id == studentId)
            .FirstOrDefaultAsync(ct);

        // Get valid subject IDs for this student based on curriculum
        var validSubjectIds = student != null
            ? await _subjectFilter.GetValidSubjectIdsForStudentAsync(student, ct)
            : new HashSet<int>();

        var marks = await _markEntryRepository.Query()
            .Include(m => m.Subject)
            .Where(m => m.ExamId == examId && m.StudentId == studentId && !m.IsDeleted)
            .ToListAsync(ct);

        // Filter marks to only show valid subjects for this student
        if (validSubjectIds.Count > 0)
            marks = marks.Where(m => validSubjectIds.Contains(m.SubjectId)).ToList();

        var school = await _schoolSettingRepository.Query().FirstOrDefaultAsync(ct);
 
        return _pdfGenerator.GenerateSchoolReportCard(result, marks, school);
    }
}

