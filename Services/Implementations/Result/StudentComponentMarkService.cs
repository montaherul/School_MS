using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Result;

public class StudentComponentMarkService : IStudentComponentMarkService
{
    private readonly IUnitOfWork _uow;

    public StudentComponentMarkService(IUnitOfWork uow)
    {
        _uow = uow;
    }

    public async Task<StudentComponentMark?> GetAsync(int examId, int studentId, int examSubjectComponentId)
    {
        return await _uow.Repository<StudentComponentMark>()
            .Query()
            .FirstOrDefaultAsync(x => x.ExamId == examId && x.StudentId == studentId && x.ExamSubjectComponentId == examSubjectComponentId && !x.IsDeleted);
    }

    public async Task<List<StudentComponentMark>> GetByStudentAsync(int examId, int studentId)
    {
        return await _uow.Repository<StudentComponentMark>()
            .Query()
            .Where(x => x.ExamId == examId && x.StudentId == studentId && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<StudentComponentMark>> GetByExamSubjectAsync(int examSubjectId)
    {
        return await _uow.Repository<StudentComponentMark>()
            .Query()
            .Where(x => x.ExamSubjectId == examSubjectId && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task<List<StudentComponentMark>> GetByExamAsync(int examId)
    {
        return await _uow.Repository<StudentComponentMark>()
            .Query()
            .Where(x => x.ExamId == examId && !x.IsDeleted)
            .ToListAsync();
    }

    public async Task UpsertAsync(StudentComponentMark mark, string updatedBy)
    {
        var repo = _uow.Repository<StudentComponentMark>();
        var existing = await repo.Query()
            .FirstOrDefaultAsync(x => x.ExamId == mark.ExamId
                                   && x.StudentId == mark.StudentId
                                   && x.ExamSubjectComponentId == mark.ExamSubjectComponentId
                                   && !x.IsDeleted);

        if (existing != null)
        {
            existing.ObtainedMarks = mark.ObtainedMarks;
            existing.UpdatedBy = updatedBy;
            existing.UpdatedAt = DateTime.UtcNow;
            repo.Update(existing);
        }
        else
        {
            mark.CreatedBy = updatedBy;
            mark.CreatedAt = DateTime.UtcNow;
            await repo.AddAsync(mark);
        }

        await _uow.SaveChangesAsync();
    }

    public async Task UpsertBatchAsync(List<StudentComponentMark> marks, string updatedBy)
    {
        if (marks.Count == 0) return;

        var repo = _uow.Repository<StudentComponentMark>();
        var now = DateTime.UtcNow;

        foreach (var mark in marks)
        {
            var existing = await repo.Query()
                .FirstOrDefaultAsync(x => x.ExamId == mark.ExamId
                                       && x.StudentId == mark.StudentId
                                       && x.ExamSubjectComponentId == mark.ExamSubjectComponentId
                                       && !x.IsDeleted);

            if (existing != null)
            {
                existing.ObtainedMarks = mark.ObtainedMarks;
                existing.UpdatedBy = updatedBy;
                existing.UpdatedAt = now;
                repo.Update(existing);
            }
            else
            {
                mark.CreatedBy = updatedBy;
                mark.CreatedAt = now;
                await repo.AddAsync(mark);
            }
        }

        await _uow.SaveChangesAsync();
    }

    public async Task<bool> DeleteAsync(int id)
    {
        var repo = _uow.Repository<StudentComponentMark>();
        var mark = await repo.GetByIdAsync(id);
        if (mark == null) return false;

        mark.IsDeleted = true;
        repo.Update(mark);
        await _uow.SaveChangesAsync();
        return true;
    }

    public async Task<bool> HasMarksAsync(int examSubjectComponentId)
    {
        return await _uow.Repository<StudentComponentMark>()
            .Query()
            .AnyAsync(x => x.ExamSubjectComponentId == examSubjectComponentId && !x.IsDeleted && x.ObtainedMarks != null);
    }
}
