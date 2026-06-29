namespace SchoolManagementSystem.Services.Interfaces.Admissions;

public interface ISectionAllocationService
{
    Task<bool> IsSectionAvailableAsync(int sectionId, CancellationToken ct = default);
    Task<int> GetSectionStudentCountAsync(int sectionId, CancellationToken ct = default);
}
