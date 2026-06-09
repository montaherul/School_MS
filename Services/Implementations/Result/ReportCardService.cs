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


    public ReportCardService(
        IUnitOfWork uow,
        IPdfGenerator pdfGenerator,
        IStudentExamResultRepository examResultRepository,
        IMarkEntryRepository markEntryRepository,
        ISchoolSettingRepository schoolSettingRepository
        )
    {
        _uow = uow;
        _pdfGenerator = pdfGenerator;
        _examResultRepository = examResultRepository;
        _markEntryRepository = markEntryRepository;
        _schoolSettingRepository = schoolSettingRepository;
    }

    public async Task<byte[]?> GenerateReportCardPdfAsync(int examId, int studentId, CancellationToken ct = default)
    {
        var result = await _examResultRepository.Query()
            .Include(r => r.Student)
            .Include(r => r.Exam)
            .FirstOrDefaultAsync(r => r.ExamId == examId && r.StudentId == studentId && !r.IsDeleted
                && (r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked), ct);

        if (result == null) return null;

        // Get student's assigned subjects for filtering
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .Where(s => s.Id == studentId)
            .Select(s => new
            {
                s.AssignedReligionSubjectId,
                s.OptionalSubjectId,
                s.StudentGroupId,
                s.ClassId
            })
            .FirstOrDefaultAsync(ct);

        // Get class-subject mappings to determine which subjects are valid for this student
        var validSubjectIds = new HashSet<int>();
        if (student != null)
        {
            var classSubjects = await _uow.Repository<ClassSubject>().Query()
                .AsNoTracking()
                .Where(cs => cs.SchoolClassId == student.ClassId && !cs.IsDeleted && cs.IsActive)
                .ToListAsync(ct);

            foreach (var cs in classSubjects)
            {
                // Skip religion subjects not matching student's religion
                if (cs.IsReligionSubject)
                {
                    if (student.AssignedReligionSubjectId.HasValue && cs.SubjectId == student.AssignedReligionSubjectId.Value)
                        validSubjectIds.Add(cs.SubjectId);
                    continue;
                }

                // Skip group subjects not matching student's group
                if (cs.IsGroupSubject)
                {
                    if (cs.StudentGroupId.HasValue && student.StudentGroupId.HasValue &&
                        cs.StudentGroupId.Value == student.StudentGroupId.Value)
                        validSubjectIds.Add(cs.SubjectId);
                    continue;
                }

                // Include common subjects
                validSubjectIds.Add(cs.SubjectId);
            }
        }

        var marks = await _markEntryRepository.Query()
            .Include(m => m.Subject)
            .Where(m => m.ExamId == examId && m.StudentId == studentId && !m.IsDeleted)
            .ToListAsync(ct);

        // Include the student's optional subject if assigned
        if (student != null && student.OptionalSubjectId.HasValue)
            validSubjectIds.Add(student.OptionalSubjectId.Value);

        // Filter marks to only show valid subjects for this student
        if (validSubjectIds.Count > 0)
            marks = marks.Where(m => validSubjectIds.Contains(m.SubjectId)).ToList();

        var school = await _schoolSettingRepository.Query().FirstOrDefaultAsync(ct);
 
        return _pdfGenerator.GenerateSchoolReportCard(result, marks, school);
    }
}

