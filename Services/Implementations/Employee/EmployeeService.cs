using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Employee;
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
    private readonly IUserProvisionService _userProvisionService;
    private readonly IEmailService _emailService;
    private readonly ILogger<EmployeeService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly SchoolManagementSystem.Services.Interfaces.Teachers.ITeacherSynchronizationService _teacherSynchronizationService;

    public EmployeeService(
        IUnitOfWork unitOfWork, 
        IUserProvisionService userProvisionService, 
        IEmailService emailService,
        ILogger<EmployeeService> logger,
        IHttpContextAccessor httpContextAccessor,
        SchoolManagementSystem.Services.Interfaces.Teachers.ITeacherSynchronizationService teacherSynchronizationService)
    {
        _unitOfWork = unitOfWork;
        _userProvisionService = userProvisionService;
        _emailService = emailService;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
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
                    
                    // Track generated username for the notification workflow
                    dto.Remarks = $"Account auto-provisioned. Username: {username}";
                    employee.Remarks = string.IsNullOrEmpty(employee.Remarks) ? dto.Remarks : $"{employee.Remarks}\n{dto.Remarks}";
                    
                    await _unitOfWork.SaveChangesAsync(ct);

                    // Send email to employee if email is provided
                    if (!string.IsNullOrWhiteSpace(employee.Email))
                    {
                        try
                        {
                            await _emailService.SendEmployeeAccountAsync(employee.Email, employee.FullName, username, password, ct);
                        }
                        catch (Exception ex)
                        {
                            _logger.LogError(ex, "Employee account email failed for {EmployeeEmail}", employee.Email);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // In a production system, we would log this, but let's complete saving employee
                _logger.LogError(ex, "Employee provisioning failed for {EmployeeEmail}", dto.Email);
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
        catch (Exception ex)
        {
            _logger.LogError(ex, "Teacher sync failed after saving employee {EmployeeId}", employee.Id);
        }

        var auditAction = dto.Id == 0 ? "Employee.Create" : "Employee.Update";
        var auditDetails = dto.Id == 0
            ? $"Created employee: {employee.FullName} ({employee.EmployeeCode})"
            : $"Updated employee: {employee.FullName} ({employee.EmployeeCode})";
        await LogAuditAsync("Employee", auditAction, employee.Id.ToString(), auditDetails, ct);

        return employee.Id;
    }

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

        await LogAuditAsync("Employee", "Employee.Delete", id.ToString(), $"Deleted employee: {employee.FullName} ({employee.EmployeeCode})", ct);

        // Auto-synchronize teaching employee deactivation to Teacher extension layer
        try
        {
            await _teacherSynchronizationService.SyncEmployeeToTeacherAsync(id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Teacher sync failed after deleting employee {EmployeeId}", id);
        }

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

        await LogAuditAsync("Employee", "Employee.StatusChange", id.ToString(), $"Changed status to '{status}' for employee: {employee.FullName} ({employee.EmployeeCode})", ct);

        // Auto-synchronize teaching employee status updates to Teacher extension layer
        try
        {
            await _teacherSynchronizationService.SyncEmployeeToTeacherAsync(id, ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Teacher sync failed after updating employee status {EmployeeId}", id);
        }

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
        if (string.IsNullOrEmpty(relativePath)) return;
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }

    private async Task LogAuditAsync(string module, string action, string entityId, string details, CancellationToken ct = default)
    {
        var httpContext = _httpContextAccessor?.HttpContext;
        var userIdStr = httpContext?.User?.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value;
        int? userId = userIdStr != null && int.TryParse(userIdStr, out var uid) ? uid : null;

        var log = new AuditLog
        {
            UserId = userId,
            Module = module,
            Action = action,
            IpAddress = httpContext?.Connection?.RemoteIpAddress?.ToString(),
            Details = details.Length > 1000 ? details[..1000] : details,
            CreatedBy = httpContext?.User?.Identity?.Name ?? "system",
            CreatedAt = DateTime.UtcNow
        };

        await _unitOfWork.Repository<AuditLog>().AddAsync(log, ct);
        await _unitOfWork.SaveChangesAsync(ct);
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
            EmployeeCardNumber = employee.EmployeeCardNumber,
            CardIssueDate = employee.CardIssueDate,
            CardExpiryDate = employee.CardExpiryDate,
            CardPrintedAt = employee.CardPrintedAt,
            CardVersion = employee.CardVersion,
            QRVerificationCode = employee.QRVerificationCode,
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
