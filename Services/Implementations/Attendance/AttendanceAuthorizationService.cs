using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Claims;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Implementations.Attendance;

public class AttendanceAuthorizationService : IAttendanceAuthorizationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AttendanceAuthorizationService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> IsAuthorizedToMarkAttendanceAsync(int teacherId, int classId, int sectionId, int academicYearId, CancellationToken ct = default)
        => await IsAuthorizedToMarkAttendanceAsync(teacherId, classId, sectionId, null, academicYearId, ct);

    public async Task EnsureCurrentUserCanManageAttendanceAsync(
        int classId,
        int sectionId,
        int? studentGroupId,
        int academicYearId = 0,
        CancellationToken ct = default)
    {
        var user = _httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            return;
        }

        if (user.IsInRole("Super Admin") ||
            user.IsInRole("Admin") ||
            user.IsInRole("Principal") ||
            user.IsInRole("Assistant Head"))
        {
            return;
        }

        var userIdValue = user.FindFirstValue(ClaimTypes.NameIdentifier);
        if (!int.TryParse(userIdValue, out var userId))
        {
            throw new UnauthorizedAccessException("Current user cannot be resolved for attendance authorization.");
        }

        var teacher = await _unitOfWork.Repository<Teacher>().Query()
            .AsNoTracking()
            .Include(t => t.Employee)
            .FirstOrDefaultAsync(t => t.Employee != null && t.Employee.UserId == userId && !t.IsDeleted, ct);

        if (teacher == null)
        {
            throw new UnauthorizedAccessException("Only authorized teachers can manage student attendance.");
        }

        var isAuthorized = await IsAuthorizedToMarkAttendanceAsync(
            teacher.Id,
            classId,
            sectionId,
            studentGroupId,
            academicYearId,
            ct);

        if (!isAuthorized)
        {
            throw new UnauthorizedAccessException("Teacher is not assigned to this class, section and group.");
        }
    }

    public async Task<bool> IsAuthorizedToMarkAttendanceAsync(
        int teacherId,
        int classId,
        int sectionId,
        int? studentGroupId,
        int academicYearId,
        CancellationToken ct = default)
    {
        if (academicYearId == 0)
        {
            var activeYear = await _unitOfWork.Repository<AcademicYear>().Query()
                .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);
            if (activeYear == null) return false;
            academicYearId = activeYear.Id;
        }

        var schoolClass = await _unitOfWork.Repository<SchoolClass>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == classId && !c.IsDeleted, ct);

        if (schoolClass == null)
        {
            await LogAuthorizationFailureAsync(teacherId, classId, sectionId, studentGroupId, academicYearId, ct);
            return false;
        }

        var requiresGroup = IsGroupClass(schoolClass) || studentGroupId.HasValue;
        if (requiresGroup && !studentGroupId.HasValue)
        {
            await LogAuthorizationFailureAsync(teacherId, classId, sectionId, studentGroupId, academicYearId, ct);
            return false;
        }

        var repo = _unitOfWork.Repository<TeacherClassAssignment>();
        var query = repo.Query().Where(a =>
            a.TeacherId == teacherId && 
            a.ClassId == classId && 
            a.SectionId == sectionId && 
            a.AcademicYearId == academicYearId && 
            a.IsActive && 
            !a.IsDeleted);

        query = requiresGroup
            ? query.Where(a => a.GroupId == studentGroupId)
            : query.Where(a => a.GroupId == null);

        var isAuthorized = await query.AnyAsync(ct);

        if (!isAuthorized)
        {
            await LogAuthorizationFailureAsync(teacherId, classId, sectionId, studentGroupId, academicYearId, ct);
        }

        return isAuthorized;
    }

    private async Task LogAuthorizationFailureAsync(
        int teacherId,
        int classId,
        int sectionId,
        int? studentGroupId,
        int academicYearId,
        CancellationToken ct)
    {
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var logRepo = _unitOfWork.Repository<TeacherAssignmentLog>();
        var log = new TeacherAssignmentLog
        {
            TeacherId = teacherId,
            Action = "AuthFailure",
            EntityName = "ClassTeacherAssignment",
            Timestamp = DateTime.UtcNow,
            IPAddress = ipAddress,
            Remarks = $"Failed attendance authorization for ClassId: {classId}, SectionId: {sectionId}, GroupId: {studentGroupId?.ToString() ?? "NULL"}, AcademicYearId: {academicYearId}"
        };
        await logRepo.AddAsync(log, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    private static bool IsGroupClass(SchoolClass schoolClass)
    {
        if (schoolClass.SortOrder >= 9 && schoolClass.SortOrder <= 10)
        {
            return true;
        }

        var name = schoolClass.Name.Replace("Class ", "", StringComparison.OrdinalIgnoreCase).Trim();
        var match = System.Text.RegularExpressions.Regex.Match(name, "\\d+");
        if (match.Success && int.TryParse(match.Value, out var classNumber))
        {
            return classNumber >= 9 && classNumber <= 10;
        }

        return name.Equals("IX", StringComparison.OrdinalIgnoreCase) ||
               name.Equals("X", StringComparison.OrdinalIgnoreCase);
    }
}
