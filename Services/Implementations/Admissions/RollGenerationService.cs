using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Admissions;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class StudentRollGenerationService : IStudentRollGenerationService
{
    private readonly IStudentRepository _studentRepository;

    public StudentRollGenerationService(IStudentRepository studentRepository)
    {
        _studentRepository = studentRepository;
    }

    public async Task<int> GenerateNextRollAsync(int classId, int sectionId, CancellationToken ct = default)
    {
        var maxRoll = await _studentRepository.Query().AsNoTracking()
            .Where(x => !x.IsDeleted && x.ClassId == classId && x.SectionId == sectionId)
            .Select(x => (int?)x.RollNumber)
            .MaxAsync(ct);
        return (maxRoll ?? 0) + 1;
    }

    public async Task<List<int>> GenerateBulkRollsAsync(int classId, int sectionId, int count, CancellationToken ct = default)
    {
        var maxRoll = await GenerateNextRollAsync(classId, sectionId, ct);
        return Enumerable.Range(maxRoll, count).ToList();
    }
}
