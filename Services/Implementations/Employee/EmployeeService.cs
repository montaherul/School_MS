using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Models.Entities.Auth;


namespace SchoolManagementSystem.Services.Implementations.Employee;

public class EmployeeService : IEmployeeService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IAuditLogRepository _auditLogRepository;
    private readonly IPasswordHashService _passwordHashService;

    public EmployeeService(
        IUnitOfWork unitOfWork, 
        IEmployeeRepository employeeRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IAuditLogRepository auditLogRepository,
        IPasswordHashService passwordHashService)
    {
        _unitOfWork = unitOfWork;
        _employeeRepository = employeeRepository;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _auditLogRepository = auditLogRepository;
        _passwordHashService = passwordHashService;
    }


    public async Task<PagedResult<EmployeeListItemDto>> GetPagedAsync(int page, int pageSize, string? search, long? departmentId = null, long? designationId = null, bool? isActive = null, CancellationToken ct = default)
    {
        var (items, totalItems) = await _employeeRepository.GetPagedAsync(page, pageSize, search, departmentId, designationId, isActive, ct);
        return new PagedResult<EmployeeListItemDto> { Items = items, TotalItems = totalItems, Page = page, PageSize = pageSize };
    }

    public async Task<EmployeeViewModel?> GetForEditAsync(long id, CancellationToken ct = default)
    {
        var employee = await _employeeRepository.Query()
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
        if (employee == null) return null;

        var user = await _userRepository.FirstOrDefaultAsync(u => u.EmployeeId == id, ct);

        return new EmployeeViewModel
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FullName = employee.FullName,
            Gender = employee.Gender,
            DateOfBirth = employee.DateOfBirth,
            Phone = employee.Phone,
            Email = employee.Email,
            BloodGroup = employee.BloodGroup,
            Nationality = employee.Nationality,
            PresentVillage = employee.PresentVillage,
            PresentPostOffice = employee.PresentPostOffice,
            PresentThana = employee.PresentThana,
            PresentDistrict = employee.PresentDistrict,
            PermanentVillage = employee.PermanentVillage,
            PermanentPostOffice = employee.PermanentPostOffice,
            PermanentThana = employee.PermanentThana,
            PermanentDistrict = employee.PermanentDistrict,
            JoiningDate = employee.JoiningDate,
            Salary = employee.Salary,
            PhotoPath = employee.PhotoPath,
            IsActive = employee.IsActive,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name,
            DesignationId = employee.DesignationId,
            DesignationName = employee.Designation?.Name,
            
            // System Access Info
            HasLoginAccount = user != null,
            CreateLoginAccount = user != null,
            Username = user?.UserName,
            RoleId = user?.UserRoles.FirstOrDefault()?.RoleId
        };
    }


    public async Task<long> CreateAsync(EmployeeViewModel model, string createdBy, CancellationToken ct = default)
    {
        if (model.CreateLoginAccount && !string.IsNullOrWhiteSpace(model.Username))
        {
            var existingUser = await _userRepository.AnyAsync(u => u.UserName == model.Username, ct);
            if (existingUser) throw new Exception("Username already exists.");
        }

        var employee = new SchoolManagementSystem.Models.Entities.Employee.Employee
        {
            EmployeeCode = model.EmployeeCode,
            FullName = model.FullName,
            Gender = model.Gender,
            DateOfBirth = model.DateOfBirth,
            Phone = model.Phone,
            Email = model.Email,
            BloodGroup = model.BloodGroup,
            Nationality = model.Nationality,
            PresentVillage = model.PresentVillage,
            PresentPostOffice = model.PresentPostOffice,
            PresentThana = model.PresentThana,
            PresentDistrict = model.PresentDistrict,
            PermanentVillage = model.PermanentVillage,
            PermanentPostOffice = model.PermanentPostOffice,
            PermanentThana = model.PermanentThana,
            PermanentDistrict = model.PermanentDistrict,
            JoiningDate = model.JoiningDate,
            Salary = model.Salary,
            PhotoPath = model.PhotoPath,
            IsActive = model.IsActive,
            DepartmentId = model.DepartmentId,
            DesignationId = model.DesignationId
        };

        await _employeeRepository.AddAsync(employee, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        if (model.CreateLoginAccount && !string.IsNullOrWhiteSpace(model.Username) && !string.IsNullOrWhiteSpace(model.Password))
        {
            var user = new ApplicationUser
            {
                UserName = model.Username,
                Email = model.Email ?? $"{model.Username}@school.com",
                PhoneNumber = model.Phone,
                PasswordHash = _passwordHashService.HashPassword(model.Password),
                EmployeeId = employee.Id,
                Status = model.IsActive ? SchoolManagementSystem.Models.Enums.AccountStatus.Active : SchoolManagementSystem.Models.Enums.AccountStatus.Inactive,
                CreatedBy = createdBy
            };

            await _userRepository.AddAsync(user, ct);
            await _unitOfWork.SaveChangesAsync(ct);

                if (model.RoleId.HasValue)
                {
                    await _userRoleRepository.AddAsync(new UserRole { UserId = user.Id, RoleId = model.RoleId.Value }, ct);
                    await _unitOfWork.SaveChangesAsync(ct);
                }

                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Module = "Employee",
                    Action = "UserCreated",
                    Details = $"User account '{user.UserName}' created for Employee ID: {employee.Id}",
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                }, ct);
        }

        return employee.Id;
    }


    public async Task UpdateAsync(EmployeeViewModel model, string updatedBy, CancellationToken ct = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(model.Id, ct) ?? throw new Exception("Employee not found");

        employee.FullName = model.FullName;
        employee.Gender = model.Gender;
        employee.DateOfBirth = model.DateOfBirth;
        employee.Phone = model.Phone;
        employee.Email = model.Email;
        employee.BloodGroup = model.BloodGroup;
        employee.Nationality = model.Nationality;
        employee.PresentVillage = model.PresentVillage;
        employee.PresentPostOffice = model.PresentPostOffice;
        employee.PresentThana = model.PresentThana;
        employee.PresentDistrict = model.PresentDistrict;
        employee.PermanentVillage = model.PermanentVillage;
        employee.PermanentPostOffice = model.PermanentPostOffice;
        employee.PermanentThana = model.PermanentThana;
        employee.PermanentDistrict = model.PermanentDistrict;
        employee.JoiningDate = model.JoiningDate;
        employee.Salary = model.Salary;
        employee.PhotoPath = model.PhotoPath;
        employee.IsActive = model.IsActive;
        employee.DepartmentId = model.DepartmentId;
        employee.DesignationId = model.DesignationId;

        _employeeRepository.Update(employee);
        
        // Handle User Account
        var user = await _userRepository.FirstOrDefaultAsync(u => u.EmployeeId == employee.Id, ct);
        if (model.CreateLoginAccount)
        {
            if (user == null)
            {
                // Create new user if requested
                if (string.IsNullOrWhiteSpace(model.Username) || string.IsNullOrWhiteSpace(model.Password))
                    throw new Exception("Username and Password are required for login access.");

                var existingUser = await _userRepository.AnyAsync(u => u.UserName == model.Username, ct);
                if (existingUser) throw new Exception("Username already exists.");

                user = new ApplicationUser
                {
                    UserName = model.Username,
                    Email = model.Email ?? $"{model.Username}@school.com",
                    PhoneNumber = model.Phone,
                    PasswordHash = _passwordHashService.HashPassword(model.Password),
                    EmployeeId = employee.Id,
                    Status = model.IsActive ? SchoolManagementSystem.Models.Enums.AccountStatus.Active : SchoolManagementSystem.Models.Enums.AccountStatus.Inactive,
                    CreatedBy = updatedBy
                };
                await _userRepository.AddAsync(user, ct);
                await _unitOfWork.SaveChangesAsync(ct);

                if (model.RoleId.HasValue)
                {
                    await _userRoleRepository.AddAsync(new UserRole { UserId = user.Id, RoleId = model.RoleId.Value }, ct);
                }
            }
            else
            {
                // Update existing user
                user.Email = model.Email ?? user.Email;
                user.PhoneNumber = model.Phone;
                user.Status = model.IsActive ? SchoolManagementSystem.Models.Enums.AccountStatus.Active : SchoolManagementSystem.Models.Enums.AccountStatus.Inactive;
                user.UpdatedBy = updatedBy;
                user.UpdatedAt = DateTime.UtcNow;

                if (!string.IsNullOrWhiteSpace(model.Password))
                {
                    user.PasswordHash = _passwordHashService.HashPassword(model.Password);
                    await _auditLogRepository.AddAsync(new AuditLog
                    {
                        Module = "Employee",
                        Action = "PasswordReset",
                        Details = $"Password updated for User: {user.UserName}",
                        CreatedBy = updatedBy,
                        CreatedAt = DateTime.UtcNow
                    }, ct);
                }

                _userRepository.Update(user);

                if (model.RoleId.HasValue)
                {
                    var existingRole = await _userRoleRepository.FirstOrDefaultAsync(ur => ur.UserId == user.Id, ct);
                    if (existingRole != null)
                    {
                        if (existingRole.RoleId != model.RoleId.Value)
                        {
                            var oldRoleId = existingRole.RoleId;
                            _userRoleRepository.Remove(existingRole);
                            await _userRoleRepository.AddAsync(new UserRole { UserId = user.Id, RoleId = model.RoleId.Value }, ct);
                            
                            await _auditLogRepository.AddAsync(new AuditLog
                            {
                                Module = "Employee",
                                Action = "RoleChanged",
                                Details = $"Role changed from RoleID:{oldRoleId} to RoleID:{model.RoleId.Value} for User: {user.UserName}",
                                CreatedBy = updatedBy,
                                CreatedAt = DateTime.UtcNow
                            }, ct);
                        }
                    }
                    else
                    {
                        await _userRoleRepository.AddAsync(new UserRole { UserId = user.Id, RoleId = model.RoleId.Value }, ct);
                    }
                }
            }
        }
        else if (user != null)
        {
            // Disable access if unchecked
            if (user.Status != SchoolManagementSystem.Models.Enums.AccountStatus.Inactive)
            {
                user.Status = SchoolManagementSystem.Models.Enums.AccountStatus.Inactive;
                _userRepository.Update(user);

                await _auditLogRepository.AddAsync(new AuditLog
                {
                    Module = "Employee",
                    Action = "AccessDisabled",
                    Details = $"Login access disabled for User: {user.UserName}",
                    CreatedBy = updatedBy,
                    CreatedAt = DateTime.UtcNow
                }, ct);
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ToggleAccessAsync(long id, string updatedBy, CancellationToken ct = default)
    {
        var user = await _userRepository.FirstOrDefaultAsync(u => u.EmployeeId == id, ct);
        if (user == null) throw new Exception("User account not found for this employee.");

        user.Status = user.Status == SchoolManagementSystem.Models.Enums.AccountStatus.Active 
            ? SchoolManagementSystem.Models.Enums.AccountStatus.Inactive 
            : SchoolManagementSystem.Models.Enums.AccountStatus.Active;
        
        user.UpdatedBy = updatedBy;
        user.UpdatedAt = DateTime.UtcNow;

        _userRepository.Update(user);

        await _auditLogRepository.AddAsync(new AuditLog
        {
            Module = "Employee",
            Action = "AccessToggled",
            Details = $"Login access {(user.Status == SchoolManagementSystem.Models.Enums.AccountStatus.Active ? "enabled" : "disabled")} for User: {user.UserName}",
            CreatedBy = updatedBy,
            CreatedAt = DateTime.UtcNow
        }, ct);

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, string updatedBy, CancellationToken ct = default)
    {
        var employee = await _employeeRepository.GetByIdAsync(id, ct) ?? throw new Exception("Employee not found");
        _employeeRepository.Remove(employee);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<EmployeeViewModel?> GetDetailAsync(long id, CancellationToken ct = default)
    {
        var employee = await _employeeRepository.Query()
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .FirstOrDefaultAsync(e => e.Id == id, ct);
        if (employee == null) return null;

        var user = await _userRepository.Query()
            .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
            .FirstOrDefaultAsync(u => u.EmployeeId == id, ct);

        return new EmployeeViewModel
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FullName = employee.FullName,
            Gender = employee.Gender,
            DateOfBirth = employee.DateOfBirth,
            Phone = employee.Phone,
            Email = employee.Email,
            BloodGroup = employee.BloodGroup,
            Nationality = employee.Nationality,
            PresentVillage = employee.PresentVillage,
            PresentPostOffice = employee.PresentPostOffice,
            PresentThana = employee.PresentThana,
            PresentDistrict = employee.PresentDistrict,
            PermanentVillage = employee.PermanentVillage,
            PermanentPostOffice = employee.PermanentPostOffice,
            PermanentThana = employee.PermanentThana,
            PermanentDistrict = employee.PermanentDistrict,
            JoiningDate = employee.JoiningDate,
            Salary = employee.Salary,
            PhotoPath = employee.PhotoPath,
            IsActive = employee.IsActive,
            DepartmentId = employee.DepartmentId,
            DepartmentName = employee.Department?.Name,
            DesignationId = employee.DesignationId,
            DesignationName = employee.Designation?.Name,
            
            // System Access Info
            HasLoginAccount = user != null,
            Username = user?.UserName,
            RoleName = user?.UserRoles.FirstOrDefault()?.Role?.Name,
            LastLoginAt = user?.LastLoginAt,
            AccountStatus = user?.Status.ToString()
        };
    }
    public async Task<long?> GetEmployeeIdByUserIdAsync(long userId, CancellationToken ct = default)
    {
        var employee = await _employeeRepository.GetByUserIdAsync(userId, ct);
        return employee?.Id;
    }
}

public class DepartmentService : IDepartmentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDepartmentRepository _departmentRepository;

    public DepartmentService(IUnitOfWork unitOfWork, IDepartmentRepository departmentRepository)
    {
        _unitOfWork = unitOfWork;
        _departmentRepository = departmentRepository;
    }

    public async Task<IEnumerable<DepartmentDto>> GetAllAsync(CancellationToken ct = default)
    {
        var departments = await _departmentRepository.ListAsync(null, ct);
        return departments.Select(d => new DepartmentDto { Id = d.Id, Name = d.Name, Code = d.Code });
    }

    public async Task<DepartmentViewModel?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var d = await _departmentRepository.GetByIdAsync(id, ct);
        if (d == null) return null;
        return new DepartmentViewModel { Id = d.Id, Name = d.Name, Code = d.Code };
    }

    public async Task CreateAsync(DepartmentViewModel model, string createdBy, CancellationToken ct = default)
    {
        var d = new Department { Name = model.Name, Code = model.Code };
        await _departmentRepository.AddAsync(d, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(DepartmentViewModel model, string updatedBy, CancellationToken ct = default)
    {
        var d = await _departmentRepository.GetByIdAsync(model.Id, ct) ?? throw new Exception("Department not found");
        d.Name = model.Name;
        d.Code = model.Code;
        _departmentRepository.Update(d);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, string updatedBy, CancellationToken ct = default)
    {
        var d = await _departmentRepository.GetByIdAsync(id, ct) ?? throw new Exception("Department not found");
        _departmentRepository.Remove(d);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}

public class DesignationService : IDesignationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IDesignationRepository _designationRepository;

    public DesignationService(IUnitOfWork unitOfWork, IDesignationRepository designationRepository)
    {
        _unitOfWork = unitOfWork;
        _designationRepository = designationRepository;
    }

    public async Task<IEnumerable<DesignationDto>> GetAllAsync(CancellationToken ct = default)
    {
        var designations = await _designationRepository.ListAsync(null, ct);
        return designations.Select(d => new DesignationDto { Id = d.Id, Name = d.Name });
    }

    public async Task<DesignationViewModel?> GetByIdAsync(long id, CancellationToken ct = default)
    {
        var d = await _designationRepository.GetByIdAsync(id, ct);
        if (d == null) return null;
        return new DesignationViewModel { Id = d.Id, Name = d.Name };
    }

    public async Task CreateAsync(DesignationViewModel model, string createdBy, CancellationToken ct = default)
    {
        var d = new Designation { Name = model.Name };
        await _designationRepository.AddAsync(d, ct);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task UpdateAsync(DesignationViewModel model, string updatedBy, CancellationToken ct = default)
    {
        var d = await _designationRepository.GetByIdAsync(model.Id, ct) ?? throw new Exception("Designation not found");
        d.Name = model.Name;
        _designationRepository.Update(d);
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(long id, string updatedBy, CancellationToken ct = default)
    {
        var d = await _designationRepository.GetByIdAsync(id, ct) ?? throw new Exception("Designation not found");
        _designationRepository.Remove(d);
        await _unitOfWork.SaveChangesAsync(ct);
    }
}
