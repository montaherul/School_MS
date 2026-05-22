using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Files;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Models.ViewModels.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Cryptography;

namespace SchoolManagementSystem.Services.Implementations.Employee;

public class EmployeeInvitationService : IEmployeeInvitationService
{
    private readonly IUnitOfWork _uow;
    private readonly IEmailService _emailService;
    private readonly IFileStorageService _fileStorage;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IUserProvisionService _userProvisionService;
    private readonly SchoolManagementSystem.Services.Interfaces.Teachers.ITeacherSynchronizationService _teacherSync;

    public EmployeeInvitationService(
        IUnitOfWork uow,
        IEmailService emailService,
        IFileStorageService fileStorage,
        IPasswordHashService passwordHashService,
        IUserProvisionService userProvisionService,
        SchoolManagementSystem.Services.Interfaces.Teachers.ITeacherSynchronizationService teacherSync)
    {
        _uow = uow;
        _emailService = emailService;
        _fileStorage = fileStorage;
        _passwordHashService = passwordHashService;
        _userProvisionService = userProvisionService;
        _teacherSync = teacherSync;
    }

    public async Task<(List<EmployeeInvitationDto> items, int totalRecords)> GetPagedInvitationsAsync(int page, int pageSize, string? search, CancellationToken ct)
    {
        var repo = (IEmployeeInvitationRepository)_uow.Repository<EmployeeInvitation>();
        return await repo.GetPagedBySpAsync(page, pageSize, search, ct);
    }

    public async Task<EmployeeInvitationDto?> GetInvitationByIdAsync(int id, CancellationToken ct)
    {
        var i = await _uow.Repository<EmployeeInvitation>().Query()
            .Include(i => i.Department)
            .Include(i => i.Designation)
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);

        if (i == null) return null;

        return MapToDto(i);
    }

    public async Task<EmployeeInvitationDto?> GetInvitationByTokenAsync(string token, CancellationToken ct)
    {
        var i = await _uow.Repository<EmployeeInvitation>().Query()
            .Include(i => i.Department)
            .Include(i => i.Designation)
            .FirstOrDefaultAsync(x => x.InvitationToken == token && !x.IsDeleted, ct);

        if (i == null) return null;

        return MapToDto(i);
    }

    public async Task<int> CreateInvitationAsync(EmployeeInvitationUpsertDto dto, int createdByUserId, CancellationToken ct)
    {
        // 1. Check if email already used in active invitation or employee record
        var existingInvite = await _uow.Repository<EmployeeInvitation>().FirstOrDefaultAsync(i => i.Email == dto.Email && !i.IsUsed && !i.IsDeleted && i.ExpiresAt > DateTime.UtcNow, ct);
        if (existingInvite != null) throw new InvalidOperationException("An active invitation already exists for this email.");

        var existingEmp = await _uow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().AnyAsync(e => e.Email == dto.Email && !e.IsDeleted, ct);
        if (existingEmp) throw new InvalidOperationException("An employee with this email already exists.");

        // 2. Create Invitation
        var invitation = new EmployeeInvitation
        {
            FullName = dto.FullName,
            Email = dto.Email,
            Mobile = dto.Mobile,
            InvitationToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N"),
            DepartmentId = dto.DepartmentId,
            DesignationId = dto.DesignationId,
            JoiningDate = dto.JoiningDate,
            EmploymentType = dto.EmploymentType,
            Status = dto.Status,
            IsTeachingStaff = dto.IsTeachingStaff,
            Remarks = dto.Remarks,
            ExpiresAt = DateTime.UtcNow.AddHours(72),
            InvitationStatus = "Sent",
            CreatedBy = createdByUserId.ToString()
        };

        await _uow.Repository<EmployeeInvitation>().AddAsync(invitation, ct);
        await _uow.SaveChangesAsync(ct);

        // 3. Send Email (Extend existing service or use direct if not available)
        // Note: We'll implement SendEmployeeInvitationAsync in IEmailService later
        // For now, let's assume it works or use direct sender as fallback
        try 
        {
             // TO BE IMPLEMENTED in IEmailService extension step
        }
        catch { /* Log error but invitation created */ }

        return invitation.Id;
    }

    public async Task<bool> ResendInvitationAsync(int id, CancellationToken ct)
    {
        var invite = await _uow.Repository<EmployeeInvitation>().GetByIdAsync(id, ct);
        if (invite == null || invite.IsUsed || invite.IsDeleted) return false;

        invite.InvitationToken = Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N");
        invite.ExpiresAt = DateTime.UtcNow.AddHours(72);
        invite.InvitationStatus = "Sent";
        invite.UpdatedAt = DateTime.UtcNow;

        _uow.Repository<EmployeeInvitation>().Update(invite);
        await _uow.SaveChangesAsync(ct);

        return true;
    }

    public async Task<bool> CancelInvitationAsync(int id, CancellationToken ct)
    {
        var invite = await _uow.Repository<EmployeeInvitation>().GetByIdAsync(id, ct);
        if (invite == null || invite.IsDeleted) return false;

        invite.InvitationStatus = "Cancelled";
        invite.IsDeleted = true;
        _uow.Repository<EmployeeInvitation>().Update(invite);
        await _uow.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> ValidateTokenAsync(string token, CancellationToken ct)
    {
        return await _uow.Repository<EmployeeInvitation>().AnyAsync(i => 
            i.InvitationToken == token && 
            !i.IsUsed && 
            !i.IsDeleted && 
            i.ExpiresAt > DateTime.UtcNow, ct);
    }

    public async Task<(bool success, string message)> CompleteOnboardingAsync(EmployeeOnboardingViewModel model, CancellationToken ct)
    {
        // 1. Validate Token
        var invite = await _uow.Repository<EmployeeInvitation>().Query()
            .Include(i => i.Department)
            .Include(i => i.Designation)
            .FirstOrDefaultAsync(i => i.InvitationToken == model.Token && !i.IsUsed && !i.IsDeleted && i.ExpiresAt > DateTime.UtcNow, ct);

        if (invite == null) return (false, "Invalid or expired invitation.");

        try
        {
            // 2. Start Transaction implicitly via UoW or explicitly
            // Standard Employee Logic usually generates a code. We'll use the existing EmployeeService logic if possible or replicate

            var employee = new SchoolManagementSystem.Models.Entities.Employee.Employee
            {
                EmployeeCode = "PENDING-" + Guid.NewGuid().ToString("N").Substring(0, 8).ToUpper(), // Temporary code
                FullName = model.FullName,
                FatherName = model.FatherName,
                MotherName = model.MotherName,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                BloodGroup = model.BloodGroup,
                Religion = model.Religion,
                Nationality = model.Nationality,
                NIDNumber = model.NIDNumber,
                BirthCertificateNo = model.BirthCertificateNo,
                Phone = model.MobileNumber,
                Email = model.PersonalEmail,
                PresentAddress = model.PresentAddress,
                PermanentAddress = model.PermanentAddress,
                EmergencyContactName = model.EmergencyContactName,
                EmergencyContactPhone = model.EmergencyContactPhone,

                // Admin Controlled Fields (Locked)
                JoiningDate = invite.JoiningDate,
                DepartmentId = invite.DepartmentId,
                DesignationId = invite.DesignationId,
                EmployeeType = invite.EmploymentType,
                IsTeachingStaff = invite.IsTeachingStaff,
                Status = invite.Status,
                Remarks = invite.Remarks,
                CreatedBy = "Onboarding"
            };

            // Handle Files
            if (model.ProfilePhoto != null)
                employee.ProfilePicturePath = await _fileStorage.SaveAsync(model.ProfilePhoto, "employees/photos", ct);
            if (model.Signature != null)
                employee.SignaturePath = await _fileStorage.SaveAsync(model.Signature, "employees/signatures", ct);

            await _uow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().AddAsync(employee, ct);
            await _uow.SaveChangesAsync(ct); // Get Employee ID

            // Add Qualifications
            if (model.Qualifications.Any())
            {
                foreach (var q in model.Qualifications)
                {
                    var qual = new EmployeeQualification
                    {
                        EmployeeId = employee.Id,
                        ExamName = q.ExamName,
                        BoardOrUniversity = q.BoardOrUniversity,
                        InstituteName = q.InstituteName,
                        GroupOrSubject = q.GroupOrSubject,
                        PassingYear = q.PassingYear,
                        Result = q.Result,
                        CGPAOrDivision = q.CGPAOrDivision
                    };
                    if (q.CertificateFile != null)
                        qual.CertificateFilePath = await _fileStorage.SaveAsync(q.CertificateFile, "employees/qualifications", ct);

                    await _uow.Repository<EmployeeQualification>().AddAsync(qual, ct);
                }
            }

            // Add Experiences
            if (model.Experiences.Any())
            {
                foreach (var ex in model.Experiences)
                {
                    await _uow.Repository<EmployeeExperience>().AddAsync(new EmployeeExperience
                    {
                        EmployeeId = employee.Id,
                        OrganizationName = ex.OrganizationName,
                        Designation = ex.Designation,
                        StartDate = ex.StartDate,
                        EndDate = ex.EndDate,
                        Remarks = ex.Remarks
                    }, ct);
                }
            }

            // Create User & Assign Roles via Designation Mapping
            var user = new ApplicationUser
            {
                UserName = employee.Email.Split('@')[0].ToLower() + employee.Id,
                Email = employee.Email,
                PasswordHash = _passwordHashService.HashPassword(model.Password),
                Status = AccountStatus.Active,
                EmployeeId = employee.Id,
                CreatedBy = "Onboarding"
            };

            await _uow.Repository<ApplicationUser>().AddAsync(user, ct);
            await _uow.SaveChangesAsync(ct);

            // RBAC Mapping
            var mappings = await _uow.Repository<DesignationRoleMapping>().ListAsync(m => m.DesignationId == employee.DesignationId && m.IsActive, ct);
            foreach (var m in mappings)
            {
                await _uow.Repository<UserRole>().AddAsync(new UserRole { UserId = user.Id, RoleId = m.RoleId }, ct);
            }

            // Teacher Sync
            if (employee.IsTeachingStaff || (invite.Designation != null && invite.Designation.IsTeachingRole))
            {
               await _teacherSync.SyncEmployeeToTeacherAsync(employee.Id, ct);
            }

            // Mark Invite Used
            invite.IsUsed = true;
            invite.InvitationStatus = "Completed";
            invite.OnboardedAt = DateTime.UtcNow;
            _uow.Repository<EmployeeInvitation>().Update(invite);

            await _uow.SaveChangesAsync(ct);
            return (true, "Onboarding successful.");
        }
        catch (Exception ex)
        {
            return (false, "Error during onboarding: " + ex.Message);
        }
    }

    public async Task<bool> ApproveOnboardingAsync(int id, int approvedByUserId, CancellationToken ct)
    {
        var invite = await _uow.Repository<EmployeeInvitation>().GetByIdAsync(id, ct);
        if (invite == null || invite.IsDeleted || invite.IsApproved) return false;

        invite.IsApproved = true;
        invite.InvitationStatus = "Approved";
        _uow.Repository<EmployeeInvitation>().Update(invite);
        await _uow.SaveChangesAsync(ct);

        return true;
    }

    private EmployeeInvitationDto MapToDto(EmployeeInvitation i)
    {
        return new EmployeeInvitationDto
        {
            Id = i.Id,
            FullName = i.FullName,
            Email = i.Email,
            Mobile = i.Mobile,
            InvitationToken = i.InvitationToken,
            DepartmentId = i.DepartmentId,
            DepartmentName = i.Department?.Name ?? "",
            DesignationId = i.DesignationId,
            DesignationName = i.Designation?.Name ?? "",
            JoiningDate = i.JoiningDate,
            EmploymentType = i.EmploymentType,
            Status = i.Status,
            IsTeachingStaff = i.IsTeachingStaff,
            Remarks = i.Remarks,
            ExpiresAt = i.ExpiresAt,
            IsUsed = i.IsUsed,
            IsApproved = i.IsApproved,
            InvitationStatus = i.InvitationStatus,
            CreatedAt = i.CreatedAt
        };
    }
}
