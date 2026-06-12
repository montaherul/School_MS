using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ResultAuthorizationService : IResultAuthorizationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public ResultAuthorizationService(IUnitOfWork unitOfWork, IHttpContextAccessor httpContextAccessor)
    {
        _unitOfWork = unitOfWork;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<bool> IsAuthorizedToEnterMarksAsync(int teacherId, int subjectId, int classId, int sectionId, int academicYearId, int? groupId = null, CancellationToken ct = default)
    {
        if (academicYearId == 0)
        {
            var activeYear = await _unitOfWork.Repository<AcademicYear>().Query()
                .FirstOrDefaultAsync(y => y.IsActive && !y.IsDeleted, ct);
            if (activeYear == null) return false;
            academicYearId = activeYear.Id;
        }

        // Determine effective groupId: explicit > section-derived > null
        var studentGroupId = groupId ?? await GetSectionGroupIdAsync(sectionId, ct);

        var repo = _unitOfWork.Repository<TeacherSubjectAssignment>();

        // Check section-level assignment first
        var isAuthorized = await repo.AnyAsync(a => 
            a.TeacherId == teacherId && 
            a.SubjectId == subjectId && 
            a.ClassId == classId && 
            a.SectionId == sectionId && 
            a.AcademicYearId == academicYearId && 
            a.IsActive && 
            !a.IsDeleted, ct);

        if (isAuthorized) return true;

        // For group-based classes, also check group-level assignment
        if (studentGroupId.HasValue)
        {
            isAuthorized = await repo.AnyAsync(a =>
                a.TeacherId == teacherId &&
                a.SubjectId == subjectId &&
                a.ClassId == classId &&
                a.GroupId == studentGroupId.Value &&
                a.AcademicYearId == academicYearId &&
                a.IsActive &&
                !a.IsDeleted, ct);

            if (isAuthorized) return true;
        }

        // Log authorization failure
        var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
        var logRepo = _unitOfWork.Repository<TeacherAssignmentLog>();
        var log = new TeacherAssignmentLog
        {
            TeacherId = teacherId,
            Action = "AuthFailure",
            EntityName = "SubjectTeacherAssignment",
            Timestamp = DateTime.UtcNow,
            IPAddress = ipAddress,
            Remarks = $"Failed marks authorization for SubjectId: {subjectId}, ClassId: {classId}, SectionId: {sectionId}, AcademicYearId: {academicYearId}, GroupId: {groupId}"
        };
        await logRepo.AddAsync(log);
        await _unitOfWork.SaveChangesAsync();

        return false;
    }

    private async Task<int?> GetSectionGroupIdAsync(int sectionId, CancellationToken ct)
    {
        var section = await _unitOfWork.Repository<Section>()
            .Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.Id == sectionId && !s.IsDeleted, ct);

        if (section == null) return null;

        if (section.StudentGroupId.HasValue)
            return section.StudentGroupId.Value;

        if (section.ParentSectionId.HasValue)
        {
            var parent = await _unitOfWork.Repository<Section>()
                .Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.Id == section.ParentSectionId.Value && !s.IsDeleted, ct);

            return parent?.StudentGroupId;
        }

        return null;
    }
}
