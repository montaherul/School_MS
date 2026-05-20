using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Employee;

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;

    public DepartmentService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken ct)
    {
        return await _unitOfWork.Repository<Department>().Query()
            .AsNoTracking()
            .Where(d => !d.IsDeleted)
            .OrderBy(d => d.Name)
            .Select(d => new DepartmentDto
            {
                Id = d.Id,
                Name = d.Name
            }).ToListAsync(ct);
    }
}

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
