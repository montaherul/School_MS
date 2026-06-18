using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Pdf;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Exam;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Exam;

public class ExamRoutineService : IExamRoutineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IViewRendererService _viewRenderer;
    private string? _schoolNameCache;
    private string? _schoolAddressCache;
    private string? _schoolLogoCache;
    private string? _academicYearNameCache;

    public ExamRoutineService(IUnitOfWork unitOfWork, IViewRendererService viewRenderer)
    {
        _unitOfWork = unitOfWork;
        _viewRenderer = viewRenderer;
    }

    public async Task<List<ExamRoutineDto>> GetStudentRoutineAsync(int studentId, CancellationToken ct = default)
    {
        var student = await _unitOfWork.Repository<Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted, ct);
        if (student == null) return new List<ExamRoutineDto>();

        var publishedStatus = ResultWorkflowStatus.Published;

        var schedules = await _unitOfWork.Repository<ExamSchedule>().Query()
            .AsNoTracking()
            .Include(s => s.Exam)
            .Include(s => s.Subject)
            .Include(s => s.Class)
            .Include(s => s.StudentGroup)
            .Include(s => s.Section)
            .Where(s => !s.IsDeleted
                && !s.Exam.IsDeleted
                && s.Exam.Status == publishedStatus
                && s.ClassId == student.ClassId
                && (s.StudentGroupId == null || s.StudentGroupId == student.StudentGroupId)
                && (s.SectionId == null || s.SectionId == student.SectionId))
            .OrderBy(s => s.ExamDate).ThenBy(s => s.StartsAt).ThenBy(s => s.Subject!.Name)
            .ToListAsync(ct);

        return schedules.Select(MapToDto).ToList();
    }

    public async Task<ExamRoutineViewModel> GetStudentRoutineViewAsync(int studentId, CancellationToken ct = default)
    {
        var student = await _unitOfWork.Repository<Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.StudentGroup)
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted, ct);

        if (student == null) return new ExamRoutineViewModel();

        var schedules = await GetStudentRoutineAsync(studentId, ct);

        Models.Entities.Exam.Exam? exam = null;
        if (schedules.Count > 0)
        {
            var minDate = schedules.Min(d => d.ExamDate);
            exam = await _unitOfWork.Repository<ExamSchedule>().Query()
                .AsNoTracking()
                .Include(s => s.Exam)
                .Where(s => s.ExamDate == minDate)
                .Select(s => s.Exam)
                .FirstOrDefaultAsync(ct);
        }

        var school = await LoadSchoolInfoAsync(ct);

        return new ExamRoutineViewModel
        {
            ExamName = exam?.Name ?? "N/A",
            ExamTerm = exam?.Term.ToString() ?? "",
            ExamStartsOn = exam?.StartsOn,
            ExamEndsOn = exam?.EndsOn,
            StudentName = student.FullName,
            StudentNo = student.StudentNo,
            ClassName = student.Class?.Name,
            GroupName = student.StudentGroup?.Name,
            SchoolName = school.SchoolName,
            SchoolAddress = school.SchoolAddress,
            SchoolLogo = school.SchoolLogo,
            AcademicYearName = school.AcademicYearName,
            Schedules = schedules
        };
    }

    public async Task<List<ExamRoutineDto>> GetGuardianRoutineAsync(int studentId, CancellationToken ct = default)
    {
        return await GetStudentRoutineAsync(studentId, ct);
    }

    public async Task<ExamRoutineViewModel> GetGuardianRoutineViewAsync(int studentId, CancellationToken ct = default)
    {
        return await GetStudentRoutineViewAsync(studentId, ct);
    }

    public async Task<List<ExamRoutineDto>> GetTeacherRoutineAsync(int teacherId, CancellationToken ct = default)
    {
        var activeStatuses = new[] { ResultWorkflowStatus.Published, ResultWorkflowStatus.Locked, (ResultWorkflowStatus)3 };

        var assignedSubjectSchedules = await _unitOfWork.Repository<ExamSchedule>().Query()
            .AsNoTracking()
            .Include(s => s.Exam)
            .Include(s => s.Subject)
            .Include(s => s.Class)
            .Include(s => s.StudentGroup)
            .Include(s => s.Section)
            .Where(s => !s.IsDeleted
                && !s.Exam.IsDeleted
                && activeStatuses.Contains(s.Exam.Status)
                && _unitOfWork.Repository<ExamSubject>().Query()
                    .Any(exs => exs.ExamId == s.ExamId
                        && exs.SubjectId == s.SubjectId
                        && exs.ClassId == s.ClassId
                        && exs.TeacherId == teacherId
                        && !exs.IsDeleted))
            .OrderBy(s => s.ExamDate).ThenBy(s => s.StartsAt).ThenBy(s => s.Subject!.Name)
            .ToListAsync(ct);

        var invigilationSchedules = await _unitOfWork.Repository<ExamSchedule>().Query()
            .AsNoTracking()
            .Include(s => s.Exam)
            .Include(s => s.Subject)
            .Include(s => s.Class)
            .Include(s => s.StudentGroup)
            .Include(s => s.Section)
            .Where(s => !s.IsDeleted
                && !s.Exam.IsDeleted
                && activeStatuses.Contains(s.Exam.Status)
                && _unitOfWork.Repository<TeacherClassAssignment>().Query()
                    .Any(tca => tca.TeacherId == teacherId
                        && tca.ClassId == s.ClassId
                        && tca.IsActive
                        && !tca.IsDeleted))
            .OrderBy(s => s.ExamDate).ThenBy(s => s.StartsAt).ThenBy(s => s.Subject!.Name)
            .ToListAsync(ct);

        var merged = assignedSubjectSchedules
            .Union(invigilationSchedules)
            .Distinct()
            .OrderBy(s => s.ExamDate).ThenBy(s => s.StartsAt).ThenBy(s => s.Subject!.Name)
            .ToList();

        return merged.Select(MapToDto).ToList();
    }

    public async Task<ExamRoutineViewModel> GetTeacherRoutineViewAsync(int teacherId, CancellationToken ct = default)
    {
        var teacher = await _unitOfWork.Repository<Teacher>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(t => t.Id == teacherId && !t.IsDeleted, ct);

        var schedules = await GetTeacherRoutineAsync(teacherId, ct);

        Models.Entities.Exam.Exam? exam = null;
        if (schedules.Count > 0)
        {
            var minDate = schedules.Min(d => d.ExamDate);
            exam = await _unitOfWork.Repository<ExamSchedule>().Query()
                .AsNoTracking()
                .Include(s => s.Exam)
                .Where(s => s.ExamDate == minDate)
                .Select(s => s.Exam)
                .FirstOrDefaultAsync(ct);
        }

        var school = await LoadSchoolInfoAsync(ct);

        return new ExamRoutineViewModel
        {
            ExamName = exam?.Name ?? "N/A",
            ExamTerm = exam?.Term.ToString() ?? "",
            ExamStartsOn = exam?.StartsOn,
            ExamEndsOn = exam?.EndsOn,
            StudentName = teacher?.FullName,
            SchoolName = school.SchoolName,
            SchoolAddress = school.SchoolAddress,
            SchoolLogo = school.SchoolLogo,
            AcademicYearName = school.AcademicYearName,
            Schedules = schedules
        };
    }

    public async Task<List<ExamRoutineDto>> GetClassRoutineAsync(int examId, int classId, int? groupId = null, CancellationToken ct = default)
    {
        var query = _unitOfWork.Repository<ExamSchedule>().Query()
            .AsNoTracking()
            .Include(s => s.Subject)
            .Include(s => s.Class)
            .Include(s => s.StudentGroup)
            .Include(s => s.Section)
            .Where(s => !s.IsDeleted
                && s.ExamId == examId
                && s.ClassId == classId
                && (groupId == null || s.StudentGroupId == groupId));

        var schedules = await query
            .OrderBy(s => s.ExamDate).ThenBy(s => s.StartsAt).ThenBy(s => s.Subject!.Name)
            .ToListAsync(ct);

        return schedules.Select(MapToDto).ToList();
    }

    public async Task<List<ExamRoutineDto>> GetPublishedExamsRoutineAsync(int classId, int? groupId = null, CancellationToken ct = default)
    {
        var publishedStatus = ResultWorkflowStatus.Published;

        var schedules = await _unitOfWork.Repository<ExamSchedule>().Query()
            .AsNoTracking()
            .Include(s => s.Exam)
            .Include(s => s.Subject)
            .Include(s => s.Class)
            .Include(s => s.StudentGroup)
            .Include(s => s.Section)
            .Where(s => !s.IsDeleted
                && !s.Exam.IsDeleted
                && s.Exam.Status == publishedStatus
                && s.ClassId == classId
                && (groupId == null || s.StudentGroupId == groupId))
            .OrderBy(s => s.ExamDate).ThenBy(s => s.StartsAt).ThenBy(s => s.Subject!.Name)
            .ToListAsync(ct);

        return schedules.Select(MapToDto).ToList();
    }

    public async Task<string> RenderRoutineHtmlAsync(List<ExamRoutineDto> schedules, string examName, string className, string? groupName, CancellationToken ct = default)
    {
        var school = await LoadSchoolInfoAsync(ct);
        var viewModel = new ExamRoutineViewModel
        {
            ExamName = examName,
            ClassName = className,
            GroupName = groupName,
            SchoolName = school.SchoolName,
            SchoolAddress = school.SchoolAddress,
            SchoolLogo = school.SchoolLogo,
            AcademicYearName = school.AcademicYearName,
            Schedules = schedules
        };
        return await _viewRenderer.RenderToStringAsync("~/Views/ExamRoutine/_RoutinePrint.cshtml", viewModel);
    }

    private async Task<(string SchoolName, string SchoolAddress, string? SchoolLogo, string AcademicYearName)> LoadSchoolInfoAsync(CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(_schoolNameCache))
            return (_schoolNameCache!, _schoolAddressCache ?? "", _schoolLogoCache, _academicYearNameCache ?? "");

        var setting = await _unitOfWork.Repository<SchoolSetting>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(ct);

        var year = await _unitOfWork.Repository<AcademicYear>().Query()
            .AsNoTracking()
            .Where(y => y.IsActive && !y.IsDeleted)
            .FirstOrDefaultAsync(ct);

        _schoolNameCache = setting?.SchoolName ?? "School Management System";
        _schoolAddressCache = setting?.Address ?? "";
        _schoolLogoCache = setting?.LogoPath;
        _academicYearNameCache = year?.Name ?? "";

        return (_schoolNameCache, _schoolAddressCache, _schoolLogoCache, _academicYearNameCache);
    }

    private static ExamRoutineDto MapToDto(ExamSchedule s)
    {
        return new ExamRoutineDto
        {
            ScheduleId = s.Id,
            SubjectName = s.Subject?.Name ?? "N/A",
            SubjectCode = s.Subject?.Code ?? "",
            ExamDate = s.ExamDate,
            StartsAt = s.StartsAt,
            EndsAt = s.EndsAt,
            RoomNo = s.RoomNo,
            Instructions = s.Instructions,
            ClassName = s.Class?.Name ?? "",
            GroupName = s.StudentGroup?.Name,
            SectionName = s.Section?.Name
        };
    }
}
