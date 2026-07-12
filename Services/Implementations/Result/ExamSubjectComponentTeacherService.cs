using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.System;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class ExamSubjectComponentTeacherService : IExamSubjectComponentTeacherService
{
    private readonly IUnitOfWork _uow;

    public ExamSubjectComponentTeacherService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<bool> IsCustomizationEnabledAsync(CancellationToken ct = default)
    {
        var profile = await _uow.Repository<SchoolProfile>().QueryNoTracking()
            .FirstOrDefaultAsync(ct);
        return profile?.AllowTeacherComponentCustomization ?? false;
    }

    public async Task<List<TeacherExamSubjectDto>> GetTeacherExamSubjectsAsync(int teacherId, CancellationToken ct = default)
    {
        var isEnabled = await IsCustomizationEnabledAsync(ct);

        var assignments = await _uow.Repository<TeacherSubjectAssignment>().QueryNoTracking()
            .Where(a => a.TeacherId == teacherId && a.IsActive && !a.IsDeleted)
            .Select(a => new { a.SubjectId, a.ClassId, a.SectionId, a.StudentGroupId, a.AcademicYearId })
            .ToListAsync(ct);

        if (assignments.Count == 0)
            return [];

        var examSubjects = await _uow.Repository<ExamSubject>().QueryNoTracking()
            .Include(es => es.Exam)
            .Include(es => es.Subject)
            .Include(es => es.Class)
            .Include(es => es.Section)
            .Include(es => es.StudentGroup)
            .Where(es => es.IsActive && !es.IsDeleted
                && es.Exam != null && !es.Exam.IsDeleted
                && assignments.Any(a =>
                    a.SubjectId == es.SubjectId &&
                    a.ClassId == es.ClassId &&
                    a.SectionId == es.SectionId &&
                    (a.StudentGroupId == es.StudentGroupId || (a.StudentGroupId == null && es.StudentGroupId == null))
                    && a.AcademicYearId == es.Exam.AcademicYearId))
            .OrderBy(es => es.Exam!.Name)
            .ThenBy(es => es.Class!.Name)
            .ThenBy(es => es.Section!.Name)
            .ThenBy(es => es.Subject!.Name)
            .ToListAsync(ct);

        var examSubjectIds = examSubjects.Select(es => es.Id).ToList();
        var componentCounts = await _uow.Repository<ExamSubjectComponent>().QueryNoTracking()
            .Where(c => examSubjectIds.Contains(c.ExamSubjectId) && !c.IsDeleted)
            .GroupBy(c => c.ExamSubjectId)
            .Select(g => new { ExamSubjectId = g.Key, Count = g.Count() })
            .ToListAsync(ct);

        var componentCountLookup = componentCounts.ToDictionary(x => x.ExamSubjectId, x => x.Count);

        return examSubjects.Select(es => new TeacherExamSubjectDto
        {
            ExamSubjectId = es.Id,
            ExamId = es.ExamId,
            ExamName = es.Exam?.Name ?? string.Empty,
            SubjectId = es.SubjectId,
            SubjectName = es.Subject?.Name ?? string.Empty,
            ClassId = es.ClassId,
            ClassName = es.Class?.Name ?? string.Empty,
            SectionId = es.SectionId,
            SectionName = es.Section?.Name ?? string.Empty,
            StudentGroupId = es.StudentGroupId,
            StudentGroupName = es.StudentGroup?.Name ?? string.Empty,
            FullMarks = es.FullMarks,
            ExamStatus = es.Exam?.Status.ToString() ?? string.Empty,
            IsLocked = es.Exam?.IsLocked ?? false,
            CanCustomize = isEnabled && es.Exam?.Status == ResultWorkflowStatus.Draft && !(es.Exam?.IsLocked ?? false),
            ComponentCount = componentCountLookup.GetValueOrDefault(es.Id, 0)
        }).ToList();
    }

    public async Task<List<TeacherExamSubjectComponentDto>> GetExamSubjectComponentsAsync(int teacherId, int examSubjectId, CancellationToken ct = default)
    {
        var canCustomize = await CanCustomizeAsync(teacherId, examSubjectId, ct);

        var components = await _uow.Repository<ExamSubjectComponent>().QueryNoTracking()
            .Include(c => c.Component)
            .Where(c => c.ExamSubjectId == examSubjectId && !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Component!.DisplayOrder)
            .Select(c => new TeacherExamSubjectComponentDto
            {
                Id = c.Id,
                ExamSubjectId = c.ExamSubjectId,
                ComponentId = c.ComponentId,
                ComponentName = c.Component!.Name,
                ComponentCode = c.Component!.Code,
                MaxMarks = c.MaxMarks,
                PassMarks = c.PassMarks,
                DisplayOrder = c.DisplayOrder,
                IsCustomized = c.MaxMarks != c.Component.DefaultFullMarks || c.PassMarks != c.Component.DefaultPassMarks,
                OriginalMaxMarks = c.Component.DefaultFullMarks,
                OriginalPassMarks = c.Component.DefaultPassMarks
            })
            .ToListAsync(ct);

        return components;
    }

    public async Task<bool> CanCustomizeAsync(int teacherId, int examSubjectId, CancellationToken ct = default)
    {
        var isEnabled = await IsCustomizationEnabledAsync(ct);
        if (!isEnabled) return false;

        var examSubject = await _uow.Repository<ExamSubject>().QueryNoTracking()
            .Include(es => es.Exam)
            .FirstOrDefaultAsync(es => es.Id == examSubjectId && !es.IsDeleted, ct);

        if (examSubject == null || examSubject.Exam == null)
            return false;

        if (examSubject.Exam.Status != ResultWorkflowStatus.Draft || examSubject.Exam.IsLocked)
            return false;

        var assignment = await _uow.Repository<TeacherSubjectAssignment>().QueryNoTracking()
            .FirstOrDefaultAsync(a =>
                a.TeacherId == teacherId &&
                a.IsActive && !a.IsDeleted &&
                a.SubjectId == examSubject.SubjectId &&
                a.ClassId == examSubject.ClassId &&
                a.SectionId == examSubject.SectionId &&
                (a.StudentGroupId == examSubject.StudentGroupId || (a.StudentGroupId == null && examSubject.StudentGroupId == null)) &&
                a.AcademicYearId == examSubject.Exam.AcademicYearId, ct);

        return assignment != null;
    }

    public async Task<bool> UpdateComponentAsync(int teacherId, TeacherExamSubjectComponentUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        if (!await CanCustomizeAsync(teacherId, dto.ExamSubjectId, ct))
            throw new UnauthorizedAccessException("Teacher is not authorized to customize this exam subject's components.");

        var component = await _uow.Repository<ExamSubjectComponent>().GetByIdAsync(dto.Id);
        if (component == null || component.IsDeleted)
            throw new KeyNotFoundException("Component not found.");

        if (component.ExamSubjectId != dto.ExamSubjectId)
            throw new InvalidOperationException("Component does not belong to the specified exam subject.");

        if (dto.PassMarks > dto.MaxMarks)
            throw new InvalidOperationException("Pass marks cannot exceed max marks.");

        var examSubject = await _uow.Repository<ExamSubject>().QueryNoTracking()
            .FirstOrDefaultAsync(es => es.Id == dto.ExamSubjectId && !es.IsDeleted, ct);

        if (examSubject == null)
            throw new KeyNotFoundException("Exam subject not found.");

        var allComponents = await _uow.Repository<ExamSubjectComponent>().QueryNoTracking()
            .Where(c => c.ExamSubjectId == dto.ExamSubjectId && !c.IsDeleted)
            .ToListAsync(ct);

        var totalMaxMarks = allComponents
            .Where(c => c.Id != dto.Id)
            .Sum(c => c.MaxMarks) + dto.MaxMarks;

        if (totalMaxMarks != examSubject.FullMarks)
            throw new InvalidOperationException($"Total component max marks ({totalMaxMarks}) must equal exam subject full marks ({examSubject.FullMarks}).");

        component.MaxMarks = dto.MaxMarks;
        component.PassMarks = dto.PassMarks;
        component.DisplayOrder = dto.DisplayOrder;
        component.UpdatedBy = updatedBy;
        component.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<ExamSubjectComponent>().Update(component);
        await _uow.SaveChangesAsync();

        return true;
    }

    public async Task<bool> UpdateComponentsBulkAsync(int teacherId, int examSubjectId, List<TeacherExamSubjectComponentUpsertDto> components, string updatedBy, CancellationToken ct = default)
    {
        if (!await CanCustomizeAsync(teacherId, examSubjectId, ct))
            throw new UnauthorizedAccessException("Teacher is not authorized to customize this exam subject's components.");

        if (components == null || components.Count == 0)
            throw new ArgumentException("At least one component is required.");

        var examSubject = await _uow.Repository<ExamSubject>().QueryNoTracking()
            .FirstOrDefaultAsync(es => es.Id == examSubjectId && !es.IsDeleted, ct);

        if (examSubject == null)
            throw new KeyNotFoundException("Exam subject not found.");

        var existingComponents = await _uow.Repository<ExamSubjectComponent>().QueryNoTracking()
            .Where(c => c.ExamSubjectId == examSubjectId && !c.IsDeleted)
            .ToListAsync(ct);

        if (existingComponents.Count != components.Count)
            throw new InvalidOperationException("Component count mismatch. Cannot add/remove components via bulk update.");

        var componentIds = components.Select(c => c.Id).ToList();
        if (componentIds.Distinct().Count() != componentIds.Count)
            throw new InvalidOperationException("Duplicate component IDs in request.");

        var existingIds = existingComponents.Select(c => c.Id).ToHashSet();
        if (!componentIds.All(id => existingIds.Contains(id)))
            throw new InvalidOperationException("One or more component IDs do not exist for this exam subject.");

        var totalMaxMarks = components.Sum(c => c.MaxMarks);
        if (totalMaxMarks != examSubject.FullMarks)
            throw new InvalidOperationException($"Total component max marks ({totalMaxMarks}) must equal exam subject full marks ({examSubject.FullMarks}).");

        foreach (var dto in components)
        {
            if (dto.PassMarks > dto.MaxMarks)
                throw new InvalidOperationException($"Pass marks cannot exceed max marks for component {dto.ComponentId}.");

            var entity = existingComponents.First(c => c.Id == dto.Id);
            entity.MaxMarks = dto.MaxMarks;
            entity.PassMarks = dto.PassMarks;
            entity.DisplayOrder = dto.DisplayOrder;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;

            _uow.Repository<ExamSubjectComponent>().Update(entity);
        }

        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<TeacherMarksEntryGridConfigDto?> GetMarksEntryGridConfigAsync(int teacherId, int examSubjectId, CancellationToken ct = default)
    {
        var examSubject = await _uow.Repository<ExamSubject>().QueryNoTracking()
            .Include(es => es.Exam)
            .Include(es => es.Subject)
            .Include(es => es.Class)
            .Include(es => es.Section)
            .Include(es => es.StudentGroup)
            .FirstOrDefaultAsync(es => es.Id == examSubjectId && !es.IsDeleted, ct);

        if (examSubject == null)
            return null;

        var components = await _uow.Repository<ExamSubjectComponent>().QueryNoTracking()
            .Include(c => c.Component)
            .Where(c => c.ExamSubjectId == examSubjectId && !c.IsDeleted)
            .OrderBy(c => c.DisplayOrder)
            .ThenBy(c => c.Component!.DisplayOrder)
            .Select(c => new TeacherExamSubjectComponentDto
            {
                Id = c.Id,
                ExamSubjectId = c.ExamSubjectId,
                ComponentId = c.ComponentId,
                ComponentName = c.Component!.Name,
                ComponentCode = c.Component!.Code,
                MaxMarks = c.MaxMarks,
                PassMarks = c.PassMarks,
                DisplayOrder = c.DisplayOrder,
                IsCustomized = c.MaxMarks != c.Component.DefaultFullMarks || c.PassMarks != c.Component.DefaultPassMarks,
                OriginalMaxMarks = c.Component.DefaultFullMarks,
                OriginalPassMarks = c.Component.DefaultPassMarks
            })
            .ToListAsync(ct);

        return new TeacherMarksEntryGridConfigDto
        {
            ExamId = examSubject.ExamId,
            ExamSubjectId = examSubject.Id,
            SubjectId = examSubject.SubjectId,
            SubjectName = examSubject.Subject?.Name ?? string.Empty,
            ClassId = examSubject.ClassId,
            SectionId = examSubject.SectionId,
            StudentGroupId = examSubject.StudentGroupId,
            Components = components
        };
    }
}