using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System;
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
    {
        if (academicYearId == 0)
        {
            var activeYear = await _unitOfWork.Repository<AcademicYear>().Query()
                .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);
            if (activeYear == null) return false;
            academicYearId = activeYear.Id;
        }

        var repo = _unitOfWork.Repository<TeacherClassAssignment>();
        var isAuthorized = await repo.AnyAsync(a => 
            a.TeacherId == teacherId && 
            a.ClassId == classId && 
            a.SectionId == sectionId && 
            a.AcademicYearId == academicYearId && 
            a.IsActive && 
            !a.IsDeleted, ct);

        if (!isAuthorized)
        {
            // Log authorization failure
            var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
            var logRepo = _unitOfWork.Repository<TeacherAssignmentLog>();
            var log = new TeacherAssignmentLog
            {
                TeacherId = teacherId,
                Action = "AuthFailure",
                EntityName = "ClassTeacherAssignment",
                Timestamp = DateTime.UtcNow,
                IPAddress = ipAddress,
                Remarks = $"Failed attendance authorization for ClassId: {classId}, SectionId: {sectionId}, AcademicYearId: {academicYearId}"
            };
            await logRepo.AddAsync(log);
            await _unitOfWork.SaveChangesAsync(); // Save the log
        }

        return isAuthorized;
    }
}
