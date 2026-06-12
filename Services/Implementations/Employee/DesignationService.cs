using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Employee;

public class DesignationService : IDesignationService
{
    private readonly IUnitOfWork _unitOfWork;

    public DesignationService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<DesignationDto>> GetAllAsync(CancellationToken ct)
    {
        return await _unitOfWork.Repository<Designation>().Query()
            .AsNoTracking()
            .Where(d => !d.IsDeleted && d.IsActive)
            .OrderBy(d => d.RoleLevel)
            .ThenBy(d => d.Name)
            .Select(d => new DesignationDto
            {
                Id = d.Id,
                Name = d.Name,
                RoleLevel = d.RoleLevel,
                IsTeachingRole = d.IsTeachingRole,
                IsActive = d.IsActive
            }).ToListAsync(ct);
    }
}
