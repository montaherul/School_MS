using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Services.Implementations.Base;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;
using System.Text;
using ClosedXML.Excel;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class MarkEntryService : BaseService<MarkEntry>, IMarkEntryService
{
    private readonly IExamRepository _examRepository;
    private readonly IMarkEntryRepository _markRepository;
    private readonly IGradingRuleRepository _gradingRepository;
    private readonly ISubjectRepository _subjectRepository;
    private readonly ISchoolClassRepository _classRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ISubjectMarkStructureService _markStructureService;
    private readonly IAuditLogger _auditLogger;
    private readonly IGradeCalculator _gradeCalculator;

    public MarkEntryService(
        IUnitOfWork uow, 
        IExamRepository examRepository, 
        IMarkEntryRepository markRepository, 
        IGradingRuleRepository gradingRepository,
        ISubjectRepository subjectRepository,
        ISchoolClassRepository classRepository,
        ISectionRepository sectionRepository,
        IStudentRepository studentRepository,
        ISubjectMarkStructureService markStructureService,
        IAuditLogger auditLogger,
        IGradeCalculator gradeCalculator) : base(uow)
    {
        _examRepository = examRepository;
        _markRepository = markRepository;
        _gradingRepository = gradingRepository;
        _subjectRepository = subjectRepository;
        _classRepository = classRepository;
        _sectionRepository = sectionRepository;
        _studentRepository = studentRepository;
        _markStructureService = markStructureService;
        _auditLogger = auditLogger;
        _gradeCalculator = gradeCalculator;
    }

    public async Task<MarkEntryDataDto> GetMarkEntryDataAsync(int examId, int subjectId, int classId, int sectionId)
    {
        var exam = await _examRepository.GetByIdAsync(examId);
        var subject = await _subjectRepository.GetByIdAsync(subjectId);
        var schoolClass = await _classRepository.GetByIdAsync(classId);
        var section = await _sectionRepository.GetByIdAsync(sectionId);

        var sheetDtos = await _markRepository.GetMarkEntrySheetAsync(examId, classId, sectionId, subjectId, default);

        var students = sheetDtos.Select(dto =>
        {
            var d = new StudentMarkDataDto
            {
                StudentId = dto.StudentId,
                StudentNo = dto.StudentNo,
                StudentName = dto.StudentName,
                RollNumber = dto.RollNumber,
                MarksObtained = dto.MarksObtained,
                Grade = dto.Grade,
                IsLocked = dto.IsLocked,
                ComponentMarks = dto.ComponentMarks,
                EnteredByTeacherId = dto.EnteredByTeacherId,
                EnteredByTeacherName = dto.EnteredByTeacherName
            };

            return d;
        }).ToList();

        return new MarkEntryDataDto
        {
            ExamId = examId,
            ExamName = exam?.Name ?? "",
            SubjectId = subjectId,
            SubjectName = subject?.Name ?? "",
            ClassId = classId,
            ClassName = section != null
                ? $"{schoolClass?.Name} - {section.Name}"
                : (schoolClass?.Name ?? ""),
            Students = students
        };
    }

    public async Task SubmitMarksBatchAsync(MarkBatchDto dto)
    {
        await SubmitMarksBatchTrackedAsync(dto);
    }

    public async Task<BatchSaveResultDto> SubmitMarksBatchTrackedAsync(MarkBatchDto dto)
    {
        var result = new BatchSaveResultDto();

        var exam = await _examRepository.GetByIdAsync(dto.ExamId);
        if (exam == null)
            throw new InvalidOperationException("Exam not found.");
        if (exam.Status == ResultWorkflowStatus.Published || exam.Status == ResultWorkflowStatus.Locked || exam.Status == ResultWorkflowStatus.Unpublished || exam.IsLocked)
            throw new InvalidOperationException($"Cannot modify marks — exam is {exam.Status}.");

        var gradingRules = await _gradingRepository.ListAsync();

        var configuredComponents = await _markStructureService.GetGridColumnsAsync(
            dto.SubjectId);

        // Validate marks against full marks and negative values
        var subject = await _subjectRepository.GetByIdAsync(dto.SubjectId);
        var examSubject = await _unitOfWork.Repository<ExamSubject>().Query()
            .FirstOrDefaultAsync(es => es.ExamId == dto.ExamId && es.SubjectId == dto.SubjectId);

        foreach (var markDto in dto.Marks)
        {
            if (markDto.MarksObtained < 0)
                throw new InvalidOperationException($"Marks cannot be negative for student {markDto.StudentId}.");
            if (examSubject != null && markDto.MarksObtained > examSubject.FullMarks)
                throw new InvalidOperationException($"Marks ({markDto.MarksObtained}) exceed full marks ({examSubject.FullMarks}) for student {markDto.StudentId}.");
            if (subject != null && examSubject == null && markDto.MarksObtained > subject.DefaultFullMarks)
                throw new InvalidOperationException($"Marks ({markDto.MarksObtained}) exceed full marks ({subject.DefaultFullMarks}) for student {markDto.StudentId}.");

            ValidateComponentMarks(markDto, configuredComponents, markDto.StudentId);
        }

        foreach (var markDto in dto.Marks)
        {
            var existingMark = (await _markRepository
                .ListAsync(x => x.ExamId == dto.ExamId && x.StudentId == markDto.StudentId && x.SubjectId == dto.SubjectId))
                .FirstOrDefault();

            if (existingMark != null && (existingMark.IsLocked || existingMark.Status == ResultWorkflowStatus.Approved))
            {
                result.SkippedStudentIds.Add(markDto.StudentId);
                continue;
            }

            ApplyComponentValues(dto, markDto, configuredComponents,
                out var componentValuesJson, out var totalMarks);

            if (totalMarks == null)
                totalMarks = markDto.MarksObtained;

            var gradeResult = _gradeCalculator.CalculateGrade(totalMarks ?? 0, gradingRules);

            // Determine target status — never downgrade from Submitted to Draft
            var targetStatus = ResultWorkflowStatus.Submitted;
            if (markDto.Status == ResultWorkflowStatus.Draft
                && (existingMark == null || existingMark.Status < ResultWorkflowStatus.Submitted))
                targetStatus = ResultWorkflowStatus.Draft;

            if (existingMark == null)
            {
                var student = await _studentRepository.GetByIdAsync(markDto.StudentId);
                var newMark = new MarkEntry
                {
                    ExamId = dto.ExamId,
                    StudentId = markDto.StudentId,
                    SubjectId = dto.SubjectId,
                    AcademicYearId = exam?.AcademicYearId ?? 0,
                    ClassId = student?.ClassId ?? 0,
                    SectionId = student?.SectionId ?? 0,
                    StudentGroupId = student?.StudentGroupId,
                    MarksObtained = totalMarks ?? 0,
                    Grade = gradeResult.Grade,
                    GradePoint = gradeResult.GradePoint,
                    EnteredByTeacherId = dto.TeacherId,
                    Status = targetStatus,
                    ComponentValues = componentValuesJson
                };

                ApplyStandardFieldValues(newMark, markDto);
                await _markRepository.AddAsync(newMark);
            }
            else
            {
                if (existingMark.MarksObtained != (totalMarks ?? 0))
                {
                    await _auditLogger.LogMarkChangeAsync(
                        dto.ExamId, markDto.StudentId, dto.SubjectId,
                        existingMark.MarksObtained, totalMarks ?? 0,
                        dto.TeacherId, "Teacher update");
                }

                ApplyStandardFieldValues(existingMark, markDto);
                existingMark.ComponentValues = componentValuesJson;
                existingMark.MarksObtained = totalMarks ?? existingMark.MarksObtained;
                existingMark.Grade = gradeResult.Grade;
                existingMark.GradePoint = gradeResult.GradePoint;
                existingMark.EnteredByTeacherId = dto.TeacherId;
                existingMark.Status = targetStatus;
                _markRepository.Update(existingMark);
            }

            result.SavedCount++;
        }

        await _unitOfWork.SaveChangesAsync();
        return result;
    }

    public async Task<byte[]> GenerateImportTemplateAsync(int examId, int subjectId, int classId, int sectionId)
    {
        var components = await _markStructureService.GetGridColumnsAsync(subjectId, classId);
        var students = await _markRepository.GetMarkEntrySheetAsync(examId, classId, sectionId, subjectId, default);

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("MarkEntry");

        int col = 1;
        ws.Cell(1, col++).SetValue("Roll");
        ws.Cell(1, col++).SetValue("StudentName");
        ws.Cell(1, col++).SetValue("StudentNo");

        foreach (var c in components)
        {
            ws.Cell(1, col).SetValue(c.ComponentCode);
            col++;
        }

        int row = 2;
        foreach (var s in students)
        {
            col = 1;
            ws.Cell(row, col++).SetValue(s.RollNumber);
            ws.Cell(row, col++).SetValue(s.StudentName);
            ws.Cell(row, col++).SetValue(s.StudentNo);
            row++;
        }

        ws.RangeUsed().SetAutoFilter();
        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<ImportResultDto> ImportMarksFromExcelAsync(Stream stream, int examId, int subjectId, int classId, int sectionId, int teacherId, bool saveAsDraft)
    {
        var result = new ImportResultDto();
        using var wb = new XLWorkbook(stream);
        var ws = wb.Worksheet(1);
        var range = ws.RangeUsed();
        if (range == null)
        {
            result.ErrorCount = 1;
            result.Errors.Add(new ImportErrorItemDto { RowNumber = 0, Message = "Empty worksheet — no data found." });
            return result;
        }

        var rows = range.RowsUsed().Skip(1).ToList();
        result.TotalRows = rows.Count;

        if (rows.Count == 0)
        {
            result.ErrorCount = 1;
            result.Errors.Add(new ImportErrorItemDto { RowNumber = 0, Message = "No data rows found (header only)." });
            return result;
        }

        var components = await _markStructureService.GetGridColumnsAsync(subjectId, classId);
        var students = await _markRepository.GetMarkEntrySheetAsync(examId, classId, sectionId, subjectId, default);
        var studentLookup = students.ToDictionary(s => s.RollNumber, s => s.StudentId);

        int colRoll = 1;
        var headerCols = new Dictionary<string, int>();

        var headerRow = range.FirstRow().CellsUsed().ToList();
        for (int i = 0; i < headerRow.Count; i++)
            headerCols[headerRow[i].GetString().Trim().ToLowerInvariant()] = i + 1;

        var marks = new List<MarkEntryDto>();
        var seenStudentIds = new HashSet<int>();

        foreach (var row in rows)
        {
            int rowNum = row.RowNumber();
            var rollStr = row.Cell(colRoll).GetString().Trim();

            if (!int.TryParse(rollStr, out var roll))
            {
                result.Errors.Add(new ImportErrorItemDto { RowNumber = rowNum, Message = $"Invalid roll number: '{rollStr}'" });
                result.ErrorCount++;
                continue;
            }

            if (!studentLookup.TryGetValue(roll, out var studentId))
            {
                result.Errors.Add(new ImportErrorItemDto { RowNumber = rowNum, Message = $"Roll {roll} not found in this class/section" });
                result.ErrorCount++;
                continue;
            }

            if (!seenStudentIds.Add(studentId))
            {
                result.Errors.Add(new ImportErrorItemDto { RowNumber = rowNum, Message = $"Duplicate roll {roll} (student already processed on an earlier row)" });
                result.ErrorCount++;
                continue;
            }

            var md = new MarkEntryDto
            {
                StudentId = studentId,
                Status = saveAsDraft ? ResultWorkflowStatus.Draft : ResultWorkflowStatus.Submitted
            };

            foreach (var c in components)
            {
                var key = c.ComponentCode.ToLowerInvariant();
                if (!headerCols.TryGetValue(key, out var ci)) continue;

                var cellVal = row.Cell(ci).GetString().Trim();
                if (string.IsNullOrEmpty(cellVal)) continue;

                if (!decimal.TryParse(cellVal, out var val) || val < 0 || val > c.FullMarks)
                {
                    result.Errors.Add(new ImportErrorItemDto { RowNumber = rowNum, Message = $"Invalid {c.ComponentName}: '{cellVal}' (expected 0-{c.FullMarks})" });
                    result.ErrorCount++;
                    break;
                }

                SetMarkEntryComponentValue(md, c.ComponentCode, val);
            }

            if (result.Errors.Count(e => e.RowNumber == rowNum) == 0)
                marks.Add(md);
        }

        if (result.ErrorCount == 0 && marks.Count > 0)
        {
            var dto = new MarkBatchDto
            {
                ExamId = examId, SubjectId = subjectId, TeacherId = teacherId, Marks = marks
            };
            var batchResult = await SubmitMarksBatchTrackedAsync(dto);
            result.SuccessCount = batchResult.SavedCount;
            result.SkippedCount = batchResult.SkippedStudentIds.Count;
        }

        return result;
    }

    public async Task<byte[]> ExportMarksToExcelAsync(int examId, int subjectId, int classId, int sectionId, int? groupId)
    {
        var components = await _markStructureService.GetGridColumnsAsync(subjectId, classId);
        var students = await _markRepository.GetMarkEntrySheetAsync(examId, classId, sectionId, subjectId, default);

        using var wb = new XLWorkbook();
        var ws = wb.Worksheets.Add("Marks");

        int col = 1;
        ws.Cell(1, col++).SetValue("Roll");
        ws.Cell(1, col++).SetValue("StudentName");
        ws.Cell(1, col++).SetValue("StudentNo");

        foreach (var c in components)
        {
            ws.Cell(1, col).SetValue(c.ComponentName);
            col++;
        }
        ws.Cell(1, col++).SetValue("Total");
        ws.Cell(1, col++).SetValue("Grade");
        ws.Cell(1, col++).SetValue("GradePoint");
        ws.Cell(1, col).SetValue("PassStatus");

        int row = 2;
        foreach (var s in students)
        {
            col = 1;
            ws.Cell(row, col++).SetValue(s.RollNumber);
            ws.Cell(row, col++).SetValue(s.StudentName);
            ws.Cell(row, col++).SetValue(s.StudentNo);

            foreach (var c in components)
            {
                var val = s.ComponentMarks[c.ComponentCode]
                    ?? GetSheetDynamicValue(s.ComponentValues, c.ComponentCode);
                ws.Cell(row, col++).SetValue((double)(val ?? 0));
            }

            ws.Cell(row, col++).SetValue(s.MarksObtained ?? 0);
            ws.Cell(row, col++).SetValue(s.Grade ?? "");
            ws.Cell(row, col).SetValue(!string.IsNullOrEmpty(s.Grade) && s.Grade != "F" ? "Pass" : "Fail");
            row++;
        }

        ws.RangeUsed().SetAutoFilter();
        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        wb.SaveAs(ms);
        return ms.ToArray();
    }

    public async Task<string> ExportMarksToCsvAsync(int examId, int subjectId, int classId, int sectionId, int? groupId)
    {
        var components = await _markStructureService.GetGridColumnsAsync(subjectId, classId);
        var students = await _markRepository.GetMarkEntrySheetAsync(examId, classId, sectionId, subjectId, default);

        var sb = new StringBuilder();
        sb.Append("Roll,StudentName,StudentNo");
        foreach (var c in components)
            sb.Append($",{c.ComponentName}");
        sb.AppendLine(",Total,Grade,GradePoint,PassStatus");

        foreach (var s in students)
        {
            sb.Append($"{s.RollNumber},{EscapeCsv(s.StudentName)},{EscapeCsv(s.StudentNo)}");

            foreach (var c in components)
            {
                var val = s.ComponentMarks[c.ComponentCode]
                    ?? GetSheetDynamicValue(s.ComponentValues, c.ComponentCode);
                sb.Append($",{val ?? 0}");
            }

            sb.AppendLine($",{s.MarksObtained ?? 0},{s.Grade ?? ""},,{(!string.IsNullOrEmpty(s.Grade) && s.Grade != "F" ? "Pass" : "Fail")}");
        }

        return sb.ToString();
    }

    private static string EscapeCsv(string val) =>
        val.Contains(',') || val.Contains('"') || val.Contains('\n')
            ? $"\"{val.Replace("\"", "\"\"")}\"" : val;

    private static decimal? GetSheetDynamicValue(string? componentValuesJson, string componentCode)
    {
        if (string.IsNullOrEmpty(componentValuesJson)) return null;
        try
        {
            var parsed = System.Text.Json.JsonSerializer.Deserialize<Dictionary<string, decimal?>>(componentValuesJson);
            return parsed?.GetValueOrDefault(componentCode);
        }
        catch { return null; }
    }

    private static void ValidateComponentMarks(
        MarkEntryDto markDto,
        List<ComponentColumnDto> configuredComponents,
        int studentId)
    {
        foreach (var component in configuredComponents)
        {
            var value = ComponentFieldMapper.GetDtoValue(markDto, component.ComponentCode);
            if (value.HasValue && (value.Value < 0 || value.Value > component.FullMarks))
            {
                throw new InvalidOperationException(
                    $"Component '{component.ComponentName}' value ({value.Value}) exceeds limit (0-{component.FullMarks}) for student {studentId}.");
            }
        }
    }

    private static void SetMarkEntryComponentValue(MarkEntryDto dto, string code, decimal val)
        => dto.ComponentMarks[code] = val;

    private static void ApplyStandardFieldValues(MarkEntry entry, MarkEntryDto dto)
    {
        var dynamicJson = ComponentFieldMapper.ApplyToEntity(dto.ComponentMarks, entry);
        if (!string.IsNullOrEmpty(dynamicJson))
            entry.ComponentValues = dynamicJson;
    }

    private void ApplyComponentValues(
        MarkBatchDto batch,
        MarkEntryDto markDto,
        List<ComponentColumnDto> configuredComponents,
        out string? componentValuesJson,
        out decimal? totalMarks)
    {
        decimal sum = 0;
        var any = false;

        foreach (var component in configuredComponents)
        {
            decimal? value = GetComponentValue(markDto, component.ComponentCode);
            if (!value.HasValue) continue;

            any = true;
            sum += value.Value;
        }

        componentValuesJson = ComponentFieldMapper.SerializeDynamicComponents(markDto.ComponentMarks);
        totalMarks = any ? sum : null;
    }

    private static decimal? GetComponentValue(MarkEntryDto dto, string componentCode)
    {
        return ComponentFieldMapper.GetDtoValue(dto, componentCode);
    }

    protected override IQueryable<MarkEntry> ApplySecurityFilters(IQueryable<MarkEntry> query, System.Security.Claims.ClaimsPrincipal user)
    {
        if (user.IsInRole("Student"))
        {
            var userIdClaim = user.FindFirstValue(ClaimTypes.NameIdentifier);
            if (int.TryParse(userIdClaim, out var userId))
            {
                var studentIds = _studentRepository.Query()
                    .Where(s => s.UserId == userId && !s.IsDeleted)
                    .Select(s => s.Id);

                return query.Where(m => studentIds.Contains(m.StudentId));
            }
        }

        return query;
    }

    public async Task LockMarksAsync(int examId, int subjectId, int classId, int sectionId)
    {
        var entries = await _unitOfWork.Repository<MarkEntry>().Query()
            .Where(m => m.ExamId == examId && m.SubjectId == subjectId
                && m.ClassId == classId && m.SectionId == sectionId)
            .ToListAsync();

        foreach (var entry in entries)
        {
            if (!entry.IsLocked)
            {
                await _auditLogger.LogMarkChangeAsync(examId, entry.StudentId, subjectId,
                    entry.MarksObtained, entry.MarksObtained, entry.EnteredByTeacherId, "Marks locked");
            }
            entry.IsLocked = true;
            entry.LockedAt = DateTime.UtcNow;
            entry.Status = ResultWorkflowStatus.Locked;
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnlockMarksAsync(int examId, int subjectId, int classId, int sectionId)
    {
        var entries = await _unitOfWork.Repository<MarkEntry>().Query()
            .Where(m => m.ExamId == examId && m.SubjectId == subjectId
                && m.ClassId == classId && m.SectionId == sectionId)
            .ToListAsync();

        foreach (var entry in entries)
        {
            await _auditLogger.LogMarkChangeAsync(examId, entry.StudentId, subjectId,
                entry.MarksObtained, entry.MarksObtained, entry.EnteredByTeacherId, "Marks unlocked");
            entry.IsLocked = false;
            entry.LockedAt = null;
            entry.Status = ResultWorkflowStatus.Submitted;
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task LockMarksForClassAsync(int examId, int subjectId, int classId)
    {
        var entries = await _unitOfWork.Repository<MarkEntry>().Query()
            .Where(m => m.ExamId == examId && m.SubjectId == subjectId && m.ClassId == classId && !m.IsDeleted)
            .ToListAsync();

        foreach (var entry in entries)
        {
            if (!entry.IsLocked)
            {
                await _auditLogger.LogMarkChangeAsync(examId, entry.StudentId, subjectId,
                    entry.MarksObtained, entry.MarksObtained, entry.EnteredByTeacherId, "Marks locked");
            }
            entry.IsLocked = true;
            entry.LockedAt = DateTime.UtcNow;
            entry.Status = ResultWorkflowStatus.Locked;
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task UnlockMarksForClassAsync(int examId, int subjectId, int classId)
    {
        var entries = await _unitOfWork.Repository<MarkEntry>().Query()
            .Where(m => m.ExamId == examId && m.SubjectId == subjectId && m.ClassId == classId && !m.IsDeleted && m.IsLocked)
            .ToListAsync();

        foreach (var entry in entries)
        {
            await _auditLogger.LogMarkChangeAsync(examId, entry.StudentId, subjectId,
                entry.MarksObtained, entry.MarksObtained, entry.EnteredByTeacherId, "Marks unlocked");
            entry.IsLocked = false;
            entry.Status = ResultWorkflowStatus.Submitted;
            entry.LockedAt = null;
        }

        await _unitOfWork.SaveChangesAsync();
    }

    public async Task<EntryStatusSummaryDto> GetEntryStatusAsync(int examId, int? classId = null)
    {
        var exam = await _examRepository.GetByIdAsync(examId);
        var classSubjectsQuery = _unitOfWork.Repository<ClassSubject>().Query()
            .Include(cs => cs.Subject)
            .Include(cs => cs.SchoolClass)
            .Where(cs => !cs.IsDeleted);

        if (classId.HasValue)
            classSubjectsQuery = classSubjectsQuery.Where(cs => cs.SchoolClassId == classId.Value);

        var classSubjects = await classSubjectsQuery.ToListAsync();

        var markEntriesQuery = _unitOfWork.Repository<MarkEntry>().Query()
            .Where(m => m.ExamId == examId);

        if (classId.HasValue)
            markEntriesQuery = markEntriesQuery.Where(m => m.ClassId == classId.Value);

        var markEntries = await markEntriesQuery.ToListAsync();

        var studentsQuery = _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .Where(s => !s.IsDeleted);

        if (classId.HasValue)
            studentsQuery = studentsQuery.Where(s => s.ClassId == classId.Value);

        var students = await studentsQuery.ToListAsync();

        var classes = classSubjects
            .Select(cs => cs.SchoolClass)
            .Distinct()
            .Where(c => classId == null || c.Id == classId)
            .OrderBy(c => c.Name)
            .Select(c => new EntryStatusClassDto
            {
                ClassId = c.Id,
                ClassName = c.Name,
                StudentCount = students.Count(s => s.ClassId == c.Id && (classId == null || s.ClassId == classId)),
                Subjects = classSubjects
                    .Where(cs => cs.SchoolClassId == c.Id)
                    .Select(cs => cs.Subject)
                    .Distinct()
                    .OrderBy(s => s.Name)
                    .Select(s =>
                    {
                        var filtered = markEntries.Where(m =>
                            m.ClassId == c.Id && m.SubjectId == s.Id).ToList();
                        return new EntryStatusSubjectDto
                        {
                            SubjectId = s.Id,
                            SubjectName = s.Name,
                            TotalStudents = students.Count(st => st.ClassId == c.Id),
                            EnteredCount = filtered.Count(m => m.MarksObtained > 0),
                            LockedCount = filtered.Count(m => m.IsLocked),
                            IsLocked = filtered.Count > 0 && filtered.All(m => m.IsLocked),
                            EntryPercentage = filtered.Count > 0
                                ? Math.Round((decimal)filtered.Count(m => m.MarksObtained > 0) / students.Count(st => st.ClassId == c.Id) * 100, 1)
                                : 0
                        };
                    })
                    .ToList()
            })
            .ToList();

        return new EntryStatusSummaryDto
        {
            ExamId = examId,
            ExamName = exam?.Name ?? "",
            Classes = classes
        };
    }
}
