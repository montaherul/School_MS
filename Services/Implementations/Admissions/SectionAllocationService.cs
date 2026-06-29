using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Admissions;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class SectionAllocationService : ISectionAllocationService
{
    private readonly ISectionRepository _sectionRepository;
    private readonly IStudentRepository _studentRepository;

    public SectionAllocationService(ISectionRepository sectionRepository, IStudentRepository studentRepository)
    {
        _sectionRepository = sectionRepository;
        _studentRepository = studentRepository;
    }

    public async Task<bool> IsSectionAvailableAsync(int sectionId, CancellationToken ct = default)
    {
        var section = await _sectionRepository.FirstOrDefaultAsync(x => x.Id == sectionId && !x.IsDeleted, ct);
        if (section == null) return false;

        var count = await GetSectionStudentCountAsync(sectionId, ct);
        return count < section.Capacity;
    }

    public async Task<int> GetSectionStudentCountAsync(int sectionId, CancellationToken ct = default)
    {
        return await _studentRepository.Query().AsNoTracking()
            .CountAsync(x => x.SectionId == sectionId && !x.IsDeleted, ct);
    }
}
