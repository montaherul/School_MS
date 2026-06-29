namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface IStudentRollGenerationService
{
    Task<int> GenerateNextRollAsync(int classId, int sectionId, CancellationToken ct = default);
    Task<List<int>> GenerateBulkRollsAsync(int classId, int sectionId, int count, CancellationToken ct = default);
}
