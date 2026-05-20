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


using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using Microsoft.AspNetCore.Http;


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
            PhotoPath = model.PhotoPath,   // will be overwritten below if a new file was uploaded
            IsActive = model.IsActive,
            DepartmentId = model.DepartmentId,
            DesignationId = model.DesignationId
        };

        await _employeeRepository.AddAsync(employee, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // Save uploaded photo now that we have an Id
        if (model.PhotoFile != null && model.PhotoFile.Length > 0)
        {
            employee.PhotoPath = await SaveFileAsync(model.PhotoFile, "employees/photos", ct);
            _employeeRepository.Update(employee);
            await _unitOfWork.SaveChangesAsync(ct);
        }

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

    private readonly IUserProvisionService _userProvisionService;
    private readonly IEmailService _emailService;
    private readonly SchoolManagementSystem.Services.Interfaces.Teachers.ITeacherSynchronizationService _teacherSynchronizationService;

    public EmployeeService(
        IUnitOfWork unitOfWork, 
        IUserProvisionService userProvisionService, 
        IEmailService emailService,
        SchoolManagementSystem.Services.Interfaces.Teachers.ITeacherSynchronizationService teacherSynchronizationService)
    {
        _unitOfWork = unitOfWork;
        _userProvisionService = userProvisionService;
        _emailService = emailService;
        _teacherSynchronizationService = teacherSynchronizationService;
    }

    public async Task<(List<EmployeeListItemDto> items, int totalRecords)> GetPagedAsync(
        int page, int pageSize, string? search, int? departmentId, int? designationId, bool? isTeachingStaff, string? status, CancellationToken ct)
    {
        return await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>()
            .Query()
            .AsNoTracking()
            .Where(e => !e.IsDeleted)
            .GetPagedAsync(page, pageSize, search, departmentId, designationId, isTeachingStaff, status, ct);
    }

    public async Task<EmployeeUpsertDto?> GetForEditAsync(int id, CancellationToken ct)
    {
        return await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>()
            .Query()
            .AsNoTracking()
            .GetForEditAsync(id, ct);
    }

    public async Task<EmployeeDetailsDto?> GetDetailsAsync(int id, CancellationToken ct)
    {
        return await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>()
            .Query()
            .AsNoTracking()
            .GetDetailsAsync(id, ct);
    }

    public async Task<EmployeeUpsertDto?> GetByUserIdAsync(int userId, CancellationToken ct)
    {
        return await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>()
            .Query()
            .AsNoTracking()
            .GetByUserIdAsync(userId, ct);
    }

    public async Task<int> SaveAsync(EmployeeUpsertDto dto, CancellationToken ct)
    {
        var employeeRepo = _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>();
        SchoolManagementSystem.Models.Entities.Employee.Employee? employee;

        // Validation Checks
        if (await IsPhoneExistsAsync(dto.Phone, dto.Id, ct))
            throw new InvalidOperationException("Phone number already exists in system.");
        if (!string.IsNullOrEmpty(dto.Email) && await IsEmailExistsAsync(dto.Email, dto.Id, ct))
            throw new InvalidOperationException("Email already exists in system.");
        if (!string.IsNullOrEmpty(dto.NIDNumber) && await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().AnyAsync(e => e.NIDNumber == dto.NIDNumber && e.Id != dto.Id && !e.IsDeleted, ct))
            throw new InvalidOperationException("NID Number already exists in system.");

        if (dto.Id == 0)
        {
            // Insert Mode
            employee = new SchoolManagementSystem.Models.Entities.Employee.Employee
            {
                EmployeeCode = await GenerateEmployeeCodeAsync(ct),
                FullName = dto.FullName,
                FatherName = dto.FatherName,
                MotherName = dto.MotherName,
                Gender = dto.Gender,
                DateOfBirth = dto.DateOfBirth,
                BloodGroup = dto.BloodGroup,
                Religion = dto.Religion,
                Nationality = dto.Nationality,
                NIDNumber = dto.NIDNumber,
                BirthCertificateNo = dto.BirthCertificateNo,
                Phone = dto.Phone,
                Email = dto.Email,
                PresentAddress = dto.PresentAddress,
                PermanentAddress = dto.PermanentAddress,
                JoiningDate = dto.JoiningDate,
                DepartmentId = dto.DepartmentId,
                DesignationId = dto.DesignationId,
                EmployeeType = dto.EmployeeType,
                IsTeachingStaff = dto.IsTeachingStaff,
                Status = dto.Status,
                EmergencyContactName = dto.EmergencyContactName,
                EmergencyContactPhone = dto.EmergencyContactPhone,
                Remarks = dto.Remarks,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = "System"
            };

            // File uploads
            if (dto.ProfilePictureFile != null && dto.ProfilePictureFile.Length > 0)
                employee.ProfilePicturePath = await SaveFileAsync(dto.ProfilePictureFile, "employees/photos", ct);
            if (dto.SignatureFile != null && dto.SignatureFile.Length > 0)
                employee.SignaturePath = await SaveFileAsync(dto.SignatureFile, "employees/signatures", ct);

            await employeeRepo.AddAsync(employee, ct);
            await _unitOfWork.SaveChangesAsync(ct);

            // Create connected user account automatically
            try
            {
                var (userId, username, password) = await _userProvisionService.ProvisionUserForEmployeeAsync(employee, ct);
                if (userId > 0)
                {
                    employee.UserId = userId;
                    
                    // Track generated username & password for the notification workflow
                    dto.Remarks = $"Account Auto-Provisioned: Username: {username}, Password: {password}. Please change password upon login.";
                    employee.Remarks = string.IsNullOrEmpty(employee.Remarks) ? dto.Remarks : $"{employee.Remarks}\n{dto.Remarks}";
                    
                    await _unitOfWork.SaveChangesAsync(ct);

                    // Send email to employee if email is provided
                    if (!string.IsNullOrWhiteSpace(employee.Email))
                    {
                        try
                        {
                            await _emailService.SendEmployeeAccountAsync(employee.Email, employee.FullName, username, password, ct);
                        }
                        catch
                        {
                            // Suppress email sending failures so employee saving still succeeds
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // In a production system, we would log this, but let's complete saving employee
                employee.Remarks = $"Employee saved but account provisioning failed: {ex.Message}";
                await _unitOfWork.SaveChangesAsync(ct);
            }
        }
        else
        {
            // Edit Mode
            employee = await employeeRepo.Query()
                .Include(e => e.Qualifications)
                .Include(e => e.Documents)
                .Include(e => e.Experiences)
                .FirstOrDefaultAsync(e => e.Id == dto.Id && !e.IsDeleted, ct);

            if (employee == null) throw new InvalidOperationException("Employee not found.");

            employee.FullName = dto.FullName;
            employee.FatherName = dto.FatherName;
            employee.MotherName = dto.MotherName;
            employee.Gender = dto.Gender;
            employee.DateOfBirth = dto.DateOfBirth;
            employee.BloodGroup = dto.BloodGroup;
            employee.Religion = dto.Religion;
            employee.Nationality = dto.Nationality;
            employee.NIDNumber = dto.NIDNumber;
            employee.BirthCertificateNo = dto.BirthCertificateNo;
            employee.Phone = dto.Phone;
            employee.Email = dto.Email;
            employee.PresentAddress = dto.PresentAddress;
            employee.PermanentAddress = dto.PermanentAddress;
            employee.JoiningDate = dto.JoiningDate;
            employee.DepartmentId = dto.DepartmentId;
            employee.DesignationId = dto.DesignationId;
            employee.EmployeeType = dto.EmployeeType;
            employee.IsTeachingStaff = dto.IsTeachingStaff;
            employee.Status = dto.Status;
            employee.EmergencyContactName = dto.EmergencyContactName;
            employee.EmergencyContactPhone = dto.EmergencyContactPhone;
            employee.Remarks = dto.Remarks;
            employee.UpdatedAt = DateTime.UtcNow;
            employee.UpdatedBy = "System";

            // Profile Picture
            if (dto.ProfilePictureFile != null && dto.ProfilePictureFile.Length > 0)
            {
                DeleteFile(employee.ProfilePicturePath);
                employee.ProfilePicturePath = await SaveFileAsync(dto.ProfilePictureFile, "employees/photos", ct);
            }

            // Signature
            if (dto.SignatureFile != null && dto.SignatureFile.Length > 0)
            {
                DeleteFile(employee.SignaturePath);
                employee.SignaturePath = await SaveFileAsync(dto.SignatureFile, "employees/signatures", ct);
            }

            // Update user account details if email/phone changed
            if (employee.UserId.HasValue)
            {
                var user = await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(employee.UserId.Value, ct);
                if (user != null)
                {
                    user.Email = employee.Email ?? $"{user.UserName}@school.local";
                    user.PhoneNumber = employee.Phone;
                    user.UpdatedAt = DateTime.UtcNow;
                }
            }
        }

        // Manage qualifications
        await ProcessQualificationsAsync(employee, dto.Qualifications, ct);

        // Manage documents
        await ProcessDocumentsAsync(employee, dto.Documents, ct);

        // Manage experiences
        await ProcessExperiencesAsync(employee, dto.Experiences, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        // Auto-synchronize teaching employee profile to Teacher extension layer
        try
        {
            await _teacherSynchronizationService.SyncEmployeeToTeacherAsync(employee.Id, ct);
        }
        catch
        {
            // Suppress secondary sync errors to ensure the primary employee saving transaction is successful

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

        // Update photo: replace the file if a new one was uploaded
        if (model.PhotoFile != null && model.PhotoFile.Length > 0)
        {
            DeleteFile(employee.PhotoPath);
            employee.PhotoPath = await SaveFileAsync(model.PhotoFile, "employees/photos", ct);
        }
        else
        {
            // Keep existing path if no new file uploaded (form might reset the field)
            // PhotoPath stays unchanged
        }

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

    public async Task<bool> DeleteAsync(int id, CancellationToken ct)
    {
        var employee = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);
        if (employee == null) return false;

        employee.IsDeleted = true;
        employee.Status = "Deleted";
        employee.UpdatedAt = DateTime.UtcNow;

        if (employee.UserId.HasValue)
        {
            var user = await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(employee.UserId.Value, ct);
            if (user != null)
            {
                user.IsDeleted = true;
                user.Status = AccountStatus.Inactive;
                user.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _unitOfWork.SaveChangesAsync(ct);

        // Auto-synchronize teaching employee deactivation to Teacher extension layer
        try
        {
            await _teacherSynchronizationService.SyncEmployeeToTeacherAsync(id, ct);
        }
        catch {}

        return true;
    }

    public async Task<bool> UpdateStatusAsync(int id, string status, CancellationToken ct)
    {
        var employee = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);
        if (employee == null) return false;

        employee.Status = status;
        employee.UpdatedAt = DateTime.UtcNow;

        if (employee.UserId.HasValue)
        {
            var user = await _unitOfWork.Repository<ApplicationUser>().GetByIdAsync(employee.UserId.Value, ct);
            if (user != null)
            {
                user.Status = (status == "Active") ? AccountStatus.Active : AccountStatus.Inactive;
                user.UpdatedAt = DateTime.UtcNow;

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

    // ── File helpers (mirrors TeacherService pattern) ─────────────────────────
    private async Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken ct)
    {
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", subFolder);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(folderPath, fileName);
        await using var stream = new FileStream(filePath, FileMode.Create);


        // Auto-synchronize teaching employee status updates to Teacher extension layer
        try
        {
            await _teacherSynchronizationService.SyncEmployeeToTeacherAsync(id, ct);
        }
        catch {}

        return true;
    }

    public async Task<bool> IsCodeExistsAsync(string code, int? excludeId, CancellationToken ct)
    {
        return await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>()
            .AnyAsync(e => e.EmployeeCode == code && e.Id != excludeId && !e.IsDeleted, ct);
    }

    public async Task<bool> IsEmailExistsAsync(string email, int? excludeId, CancellationToken ct)
    {
        return await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>()
            .AnyAsync(e => e.Email == email && e.Id != excludeId && !e.IsDeleted, ct);
    }

    public async Task<bool> IsPhoneExistsAsync(string phone, int? excludeId, CancellationToken ct)
    {
        return await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>()
            .AnyAsync(e => e.Phone == phone && e.Id != excludeId && !e.IsDeleted, ct);
    }

    private async Task<string> GenerateEmployeeCodeAsync(CancellationToken ct)
    {
        var prefix = $"EMP-{DateTime.Today.Year}-";
        var lastCode = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().Query()
            .Where(e => e.EmployeeCode.StartsWith(prefix))
            .OrderByDescending(e => e.EmployeeCode)
            .Select(e => e.EmployeeCode)
            .FirstOrDefaultAsync(ct);

        int nextNum = 1;
        if (!string.IsNullOrEmpty(lastCode) && lastCode.Length > prefix.Length)
        {
            if (int.TryParse(lastCode.Substring(prefix.Length), out int lastNum))
            {
                nextNum = lastNum + 1;
            }
        }
        return $"{prefix}{nextNum:D4}";
    }

    private async Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken ct)
    {
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", subFolder);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(folderPath, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);

        await file.CopyToAsync(stream, ct);
        return $"/uploads/{subFolder}/{fileName}";
    }

    private void DeleteFile(string? relativePath)
    {

        if (string.IsNullOrWhiteSpace(relativePath)) return;
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
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

        if (string.IsNullOrEmpty(relativePath)) return;
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }

    private async Task ProcessQualificationsAsync(SchoolManagementSystem.Models.Entities.Employee.Employee employee, List<EmployeeQualificationDto> list, CancellationToken ct)
    {
        var repo = _unitOfWork.Repository<EmployeeQualification>();
        var existingIds = list.Where(q => q.Id > 0).Select(q => q.Id).ToList();

        // Remove deleted ones
        var toDelete = employee.Qualifications.Where(q => !existingIds.Contains(q.Id) && !q.IsDeleted).ToList();
        foreach (var q in toDelete)
        {
            q.IsDeleted = true;
            q.UpdatedAt = DateTime.UtcNow;
        }

        // Add or Update
        foreach (var dto in list)
        {
            var certPath = dto.CertificateFilePath;
            if (dto.CertificateFile != null && dto.CertificateFile.Length > 0)
            {
                DeleteFile(dto.CertificateFilePath);
                certPath = await SaveFileAsync(dto.CertificateFile, "employees/certificates", ct);
            }

            if (dto.Id == 0)
            {
                var q = new EmployeeQualification
                {
                    EmployeeId = employee.Id,
                    ExamName = dto.ExamName,
                    BoardOrUniversity = dto.BoardOrUniversity,
                    InstituteName = dto.InstituteName,
                    GroupOrSubject = dto.GroupOrSubject,
                    PassingYear = dto.PassingYear,
                    Result = dto.Result,
                    CGPAOrDivision = dto.CGPAOrDivision,
                    CertificateFilePath = certPath,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await repo.AddAsync(q, ct);
            }
            else
            {
                var q = employee.Qualifications.FirstOrDefault(x => x.Id == dto.Id);
                if (q != null)
                {
                    q.ExamName = dto.ExamName;
                    q.BoardOrUniversity = dto.BoardOrUniversity;
                    q.InstituteName = dto.InstituteName;
                    q.GroupOrSubject = dto.GroupOrSubject;
                    q.PassingYear = dto.PassingYear;
                    q.Result = dto.Result;
                    q.CGPAOrDivision = dto.CGPAOrDivision;
                    q.CertificateFilePath = certPath;
                    q.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }

    private async Task ProcessDocumentsAsync(SchoolManagementSystem.Models.Entities.Employee.Employee employee, List<EmployeeDocumentDto> list, CancellationToken ct)
    {
        var repo = _unitOfWork.Repository<EmployeeDocument>();
        var existingIds = list.Where(d => d.Id > 0).Select(d => d.Id).ToList();

        // Remove deleted ones
        var toDelete = employee.Documents.Where(d => !existingIds.Contains(d.Id) && !d.IsDeleted).ToList();
        foreach (var d in toDelete)
        {
            d.IsDeleted = true;
            d.UpdatedAt = DateTime.UtcNow;
        }

        // Add or Update
        foreach (var dto in list)
        {
            var filePath = dto.FilePath;
            if (dto.DocumentFile != null && dto.DocumentFile.Length > 0)
            {
                DeleteFile(dto.FilePath);
                filePath = await SaveFileAsync(dto.DocumentFile, "employees/documents", ct);
            }

            if (dto.Id == 0)
            {
                if (string.IsNullOrEmpty(filePath)) continue; // Can't add doc without file
                var d = new EmployeeDocument
                {
                    EmployeeId = employee.Id,
                    DocumentType = dto.DocumentType,
                    DocumentName = dto.DocumentName,
                    FilePath = filePath,
                    ExpiryDate = dto.ExpiryDate,
                    Remarks = dto.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await repo.AddAsync(d, ct);
            }
            else
            {
                var d = employee.Documents.FirstOrDefault(x => x.Id == dto.Id);
                if (d != null)
                {
                    d.DocumentType = dto.DocumentType;
                    d.DocumentName = dto.DocumentName;
                    d.FilePath = filePath ?? d.FilePath;
                    d.ExpiryDate = dto.ExpiryDate;
                    d.Remarks = dto.Remarks;
                    d.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }

    private async Task ProcessExperiencesAsync(SchoolManagementSystem.Models.Entities.Employee.Employee employee, List<EmployeeExperienceDto> list, CancellationToken ct)
    {
        var repo = _unitOfWork.Repository<EmployeeExperience>();
        var existingIds = list.Where(ex => ex.Id > 0).Select(ex => ex.Id).ToList();

        // Remove deleted ones
        var toDelete = employee.Experiences.Where(ex => !existingIds.Contains(ex.Id) && !ex.IsDeleted).ToList();
        foreach (var ex in toDelete)
        {
            ex.IsDeleted = true;
            ex.UpdatedAt = DateTime.UtcNow;
        }

        // Add or Update
        foreach (var dto in list)
        {
            if (dto.Id == 0)
            {
                var ex = new EmployeeExperience
                {
                    EmployeeId = employee.Id,
                    OrganizationName = dto.OrganizationName,
                    Designation = dto.Designation,
                    StartDate = dto.StartDate,
                    EndDate = dto.EndDate,
                    Remarks = dto.Remarks,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = "System"
                };
                await repo.AddAsync(ex, ct);
            }
            else
            {
                var ex = employee.Experiences.FirstOrDefault(x => x.Id == dto.Id);
                if (ex != null)
                {
                    ex.OrganizationName = dto.OrganizationName;
                    ex.Designation = dto.Designation;
                    ex.StartDate = dto.StartDate;
                    ex.EndDate = dto.EndDate;
                    ex.Remarks = dto.Remarks;
                    ex.UpdatedAt = DateTime.UtcNow;
                }
            }
        }
    }
}

public static class EmployeeExtensions
{
    public static async Task<(List<EmployeeListItemDto> items, int totalRecords)> GetPagedAsync(
        this IQueryable<SchoolManagementSystem.Models.Entities.Employee.Employee> query,
        int page, int pageSize, string? search, int? departmentId, int? designationId, bool? isTeachingStaff, string? status, CancellationToken ct)
    {
        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(e => e.FullName.Contains(search) || e.EmployeeCode.Contains(search) || e.Phone.Contains(search));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        if (designationId.HasValue)
        {
            query = query.Where(e => e.DesignationId == designationId.Value);
        }

        if (isTeachingStaff.HasValue)
        {
            query = query.Where(e => e.IsTeachingStaff == isTeachingStaff.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(e => e.Status == status);
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(e => e.FullName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new EmployeeListItemDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FullName = e.FullName,
                Designation = e.Designation != null ? e.Designation.Name : string.Empty,
                Department = e.Department != null ? e.Department.Name : string.Empty,
                Phone = e.Phone,
                Email = e.Email,
                Status = e.Status,
                IsTeachingStaff = e.IsTeachingStaff,
                JoiningDate = e.JoiningDate,
                ProfilePicturePath = e.ProfilePicturePath
            }).ToListAsync(ct);

        return (items, totalCount);
    }

    public static async Task<EmployeeUpsertDto?> GetForEditAsync(
        this IQueryable<SchoolManagementSystem.Models.Entities.Employee.Employee> query, int id, CancellationToken ct)
    {
        var employee = await query
            .Include(e => e.Qualifications.Where(q => !q.IsDeleted))
            .Include(e => e.Documents.Where(d => !d.IsDeleted))
            .Include(e => e.Experiences.Where(ex => !ex.IsDeleted))
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);

        if (employee == null) return null;

        return new EmployeeUpsertDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FullName = employee.FullName,
            FatherName = employee.FatherName,
            MotherName = employee.MotherName,
            Gender = employee.Gender,
            DateOfBirth = employee.DateOfBirth,
            BloodGroup = employee.BloodGroup,
            Religion = employee.Religion,
            Nationality = employee.Nationality,
            NIDNumber = employee.NIDNumber,
            BirthCertificateNo = employee.BirthCertificateNo,
            Phone = employee.Phone,
            Email = employee.Email,
            PresentAddress = employee.PresentAddress,
            PermanentAddress = employee.PermanentAddress,
            JoiningDate = employee.JoiningDate,
            DepartmentId = employee.DepartmentId,
            DesignationId = employee.DesignationId,
            EmployeeType = employee.EmployeeType,
            IsTeachingStaff = employee.IsTeachingStaff,
            Status = employee.Status,
            ProfilePicturePath = employee.ProfilePicturePath,
            SignaturePath = employee.SignaturePath,
            EmergencyContactName = employee.EmergencyContactName,
            EmergencyContactPhone = employee.EmergencyContactPhone,
            Remarks = employee.Remarks,
            Qualifications = employee.Qualifications.Select(q => new EmployeeQualificationDto
            {
                Id = q.Id,
                EmployeeId = q.EmployeeId,
                ExamName = q.ExamName,
                BoardOrUniversity = q.BoardOrUniversity,
                InstituteName = q.InstituteName,
                GroupOrSubject = q.GroupOrSubject,
                PassingYear = q.PassingYear,
                Result = q.Result,
                CGPAOrDivision = q.CGPAOrDivision,
                CertificateFilePath = q.CertificateFilePath
            }).ToList(),
            Documents = employee.Documents.Select(d => new EmployeeDocumentDto
            {
                Id = d.Id,
                EmployeeId = d.EmployeeId,
                DocumentType = d.DocumentType,
                DocumentName = d.DocumentName,
                FilePath = d.FilePath,
                ExpiryDate = d.ExpiryDate,
                Remarks = d.Remarks
            }).ToList(),
            Experiences = employee.Experiences.Select(ex => new EmployeeExperienceDto
            {
                Id = ex.Id,
                EmployeeId = ex.EmployeeId,
                OrganizationName = ex.OrganizationName,
                Designation = ex.Designation,
                StartDate = ex.StartDate,
                EndDate = ex.EndDate,
                Remarks = ex.Remarks
            }).ToList()
        };
    }

    public static async Task<EmployeeDetailsDto?> GetDetailsAsync(
        this IQueryable<SchoolManagementSystem.Models.Entities.Employee.Employee> query, int id, CancellationToken ct)
    {
        var employee = await query
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .Include(e => e.User)
            .Include(e => e.Qualifications.Where(q => !q.IsDeleted))
            .Include(e => e.Documents.Where(d => !d.IsDeleted))
            .Include(e => e.Experiences.Where(ex => !ex.IsDeleted))
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);

        if (employee == null) return null;

        return new EmployeeDetailsDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FullName = employee.FullName,
            FatherName = employee.FatherName,
            MotherName = employee.MotherName,
            Gender = employee.Gender,
            DateOfBirth = employee.DateOfBirth,
            BloodGroup = employee.BloodGroup,
            Religion = employee.Religion,
            Nationality = employee.Nationality,
            NIDNumber = employee.NIDNumber,
            BirthCertificateNo = employee.BirthCertificateNo,
            Phone = employee.Phone,
            Email = employee.Email,
            PresentAddress = employee.PresentAddress,
            PermanentAddress = employee.PermanentAddress,
            JoiningDate = employee.JoiningDate,
            Department = employee.Department != null ? employee.Department.Name : string.Empty,
            Designation = employee.Designation != null ? employee.Designation.Name : string.Empty,
            EmployeeType = employee.EmployeeType,
            IsTeachingStaff = employee.IsTeachingStaff,
            Status = employee.Status,
            ProfilePicturePath = employee.ProfilePicturePath,
            SignaturePath = employee.SignaturePath,
            EmergencyContactName = employee.EmergencyContactName,
            EmergencyContactPhone = employee.EmergencyContactPhone,
            Remarks = employee.Remarks,
            Username = employee.User != null ? employee.User.UserName : null,
            Qualifications = employee.Qualifications.Select(q => new EmployeeQualificationDto
            {
                Id = q.Id,
                EmployeeId = q.EmployeeId,
                ExamName = q.ExamName,
                BoardOrUniversity = q.BoardOrUniversity,
                InstituteName = q.InstituteName,
                GroupOrSubject = q.GroupOrSubject,
                PassingYear = q.PassingYear,
                Result = q.Result,
                CGPAOrDivision = q.CGPAOrDivision,
                CertificateFilePath = q.CertificateFilePath
            }).ToList(),
            Documents = employee.Documents.Select(d => new EmployeeDocumentDto
            {
                Id = d.Id,
                EmployeeId = d.EmployeeId,
                DocumentType = d.DocumentType,
                DocumentName = d.DocumentName,
                FilePath = d.FilePath,
                ExpiryDate = d.ExpiryDate,
                Remarks = d.Remarks
            }).ToList(),
            Experiences = employee.Experiences.Select(ex => new EmployeeExperienceDto
            {
                Id = ex.Id,
                EmployeeId = ex.EmployeeId,
                OrganizationName = ex.OrganizationName,
                Designation = ex.Designation,
                StartDate = ex.StartDate,
                EndDate = ex.EndDate,
                Remarks = ex.Remarks
            }).ToList()
        };
    }

    public static async Task<EmployeeUpsertDto?> GetByUserIdAsync(
        this IQueryable<SchoolManagementSystem.Models.Entities.Employee.Employee> query, int userId, CancellationToken ct)
    {
        var employee = await query
            .Include(e => e.Qualifications.Where(q => !q.IsDeleted))
            .Include(e => e.Documents.Where(d => !d.IsDeleted))
            .Include(e => e.Experiences.Where(ex => !ex.IsDeleted))
            .FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted, ct);

        if (employee == null) return null;

        return new EmployeeUpsertDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FullName = employee.FullName,
            FatherName = employee.FatherName,
            MotherName = employee.MotherName,
            Gender = employee.Gender,
            DateOfBirth = employee.DateOfBirth,
            BloodGroup = employee.BloodGroup,
            Religion = employee.Religion,
            Nationality = employee.Nationality,
            NIDNumber = employee.NIDNumber,
            BirthCertificateNo = employee.BirthCertificateNo,
            Phone = employee.Phone,
            Email = employee.Email,
            PresentAddress = employee.PresentAddress,
            PermanentAddress = employee.PermanentAddress,
            JoiningDate = employee.JoiningDate,
            DepartmentId = employee.DepartmentId,
            DesignationId = employee.DesignationId,
            EmployeeType = employee.EmployeeType,
            IsTeachingStaff = employee.IsTeachingStaff,
            Status = employee.Status,
            ProfilePicturePath = employee.ProfilePicturePath,
            SignaturePath = employee.SignaturePath,
            EmergencyContactName = employee.EmergencyContactName,
            EmergencyContactPhone = employee.EmergencyContactPhone,
            Remarks = employee.Remarks,
            Qualifications = employee.Qualifications.Select(q => new EmployeeQualificationDto
            {
                Id = q.Id,
                EmployeeId = q.EmployeeId,
                ExamName = q.ExamName,
                BoardOrUniversity = q.BoardOrUniversity,
                InstituteName = q.InstituteName,
                GroupOrSubject = q.GroupOrSubject,
                PassingYear = q.PassingYear,
                Result = q.Result,
                CGPAOrDivision = q.CGPAOrDivision,
                CertificateFilePath = q.CertificateFilePath
            }).ToList(),
            Documents = employee.Documents.Select(d => new EmployeeDocumentDto
            {
                Id = d.Id,
                EmployeeId = d.EmployeeId,
                DocumentType = d.DocumentType,
                DocumentName = d.DocumentName,
                FilePath = d.FilePath,
                ExpiryDate = d.ExpiryDate,
                Remarks = d.Remarks
            }).ToList(),
            Experiences = employee.Experiences.Select(ex => new EmployeeExperienceDto
            {
                Id = ex.Id,
                EmployeeId = ex.EmployeeId,
                OrganizationName = ex.OrganizationName,
                Designation = ex.Designation,
                StartDate = ex.StartDate,
                EndDate = ex.EndDate,
                Remarks = ex.Remarks
            }).ToList()
        };

    }
}
