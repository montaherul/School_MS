using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using System.Data;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class AdmissionService : IAdmissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAdmissionRepository _admissionRepository;
    private readonly IStudentService _studentService;
    private readonly IEmailService _emailService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ISchoolClassRepository _classRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ILogger<AdmissionService> _logger;

    public AdmissionService(
        IUnitOfWork unitOfWork, 
        IAdmissionRepository admissionRepository, 
        IStudentService studentService, 
        IEmailService emailService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        ISchoolClassRepository classRepository,
        IStudentRepository studentRepository,
        ILogger<AdmissionService> logger)
    {
        _unitOfWork = unitOfWork;
        _admissionRepository = admissionRepository;
        _studentService = studentService;
        _emailService = emailService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _classRepository = classRepository;
        _studentRepository = studentRepository;
        _logger = logger;
    }

    public async Task<(List<AdmissionListResultDto> items, int totalRecords, object counts)> GetListByStoredProcedureAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        int classId = 0,
        CancellationToken cancellationToken = default,
        int? status = null)
    {
        var (items, totalCount) = await _admissionRepository.GetListByStoredProcedureAsync(pageNumber, pageSize, searchTerm, classId, status, cancellationToken);

        var query = _admissionRepository.Query()
            .Where(a => !a.IsDeleted)
            .Where(a => classId == 0 || a.AppliedClassId == classId)
            .Where(a => string.IsNullOrEmpty(searchTerm) ||
                a.ApplicantName.Contains(searchTerm) ||
                a.ApplicationNo.Contains(searchTerm) ||
                a.FatherOrGuardianMobileNo.Contains(searchTerm) ||
                a.ApplicantMobileNumber.Contains(searchTerm));

        var counts = await query
            .GroupBy(a => a.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToListAsync(cancellationToken);

        var countsObj = new
        {
            Pending = counts.FirstOrDefault(x => x.Status == AdmissionStatus.Pending)?.Count ?? 0,
            Approved = counts.FirstOrDefault(x => x.Status == AdmissionStatus.Approved)?.Count ?? 0,
            Rejected = counts.FirstOrDefault(x => x.Status == AdmissionStatus.Rejected)?.Count ?? 0,
            Converted = counts.FirstOrDefault(x => x.Status == AdmissionStatus.Converted)?.Count ?? 0
        };

        return (items, totalCount, countsObj);
    }

    public async Task<string> SubmitAsync(AdmissionCreateDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            dto.ProfilePicturePath = await SaveFileAsync(dto.ProfilePicture, "admissions/profiles", cancellationToken);
        }
        if (dto.BirthCertificateFile != null && dto.BirthCertificateFile.Length > 0)
        {
            dto.BirthCertificatePath = await SaveFileAsync(dto.BirthCertificateFile, "admissions/documents", cancellationToken);
        }
        if (dto.PaymentSlipFile != null && dto.PaymentSlipFile.Length > 0)
        {
            dto.PaymentSlipPath = await SaveFileAsync(dto.PaymentSlipFile, "admissions/payments", cancellationToken);
        }

        var count = await _admissionRepository.CountAsync(x => x.CreatedAt.Year == DateTime.UtcNow.Year, cancellationToken) + 1;

        var application = new AdmissionApplication
        {
            ApplicationNo = $"APP-{DateTime.UtcNow.Year}-{count:0000}",
            ApplicantName = dto.ApplicantName.Trim(),
            ApplicantNameBangla = dto.ApplicantNameBangla?.Trim(),
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender.Trim(),
            FatherName = dto.FatherName.Trim(),
            FatherOccupation = dto.FatherOccupation?.Trim(),
            MotherName = dto.MotherName.Trim(),
            MotherOccupation = dto.MotherOccupation?.Trim(),
            GuardianName = dto.GuardianName?.Trim(),
            GuardianOccupation = dto.GuardianOccupation?.Trim(),
            ApplicantMobileNumber = dto.ApplicantMobileNumber.Trim(),
            AlternativeNumber = dto.AlternativeNumber?.Trim(),
            FatherOrGuardianMobileNo = dto.FatherOrGuardianMobileNo.Trim(),
            ApplicantEmail = dto.ApplicantEmail?.Trim(),
            Nationality = dto.Nationality.Trim(),
            Country = dto.Country.Trim(),
            MaritalStatus = dto.MaritalStatus.Trim(),
            Religion = dto.Religion.Trim(),
            BloodGroup = dto.BloodGroup?.Trim(),
            BirthCertificateNo = dto.BirthCertificateNo?.Trim(),
            BirthCertificatePath = dto.BirthCertificatePath,
            PaymentSlipPath = dto.PaymentSlipPath,
            PaymentMethod = dto.PaymentMethod?.Trim(),
            TransactionDetails = dto.TransactionDetails?.Trim(),
            PresentVillage = dto.PresentVillage?.Trim(),
            PresentPostOffice = dto.PresentPostOffice?.Trim(),
            PresentThana = dto.PresentThana?.Trim(),
            PresentDistrict = dto.PresentDistrict?.Trim(),
            PermanentVillage = dto.PermanentVillage?.Trim(),
            PermanentPostOffice = dto.PermanentPostOffice?.Trim(),
            PermanentThana = dto.PermanentThana?.Trim(),
            PermanentDistrict = dto.PermanentDistrict?.Trim(),
            ProfilePicturePath = dto.ProfilePicturePath,
            AppliedClassId = dto.AppliedClassId,
            LinkedGuardianId = dto.LinkedGuardianId,
            Status = AdmissionStatus.Pending,
            CreatedBy = createdBy
        };

        await _admissionRepository.AddAsync(application, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (!string.IsNullOrWhiteSpace(application.ApplicantEmail))
        {
            try { await _emailService.SendAdmissionReceivedAsync(application.ApplicantEmail, application.ApplicantName, application.ApplicationNo, cancellationToken); }
            catch (Exception ex) { _logger.LogError(ex, "Admission confirmation email failed for application {ApplicationNo}", application.ApplicationNo); }
        }

        return application.ApplicationNo;
    }

    public async Task<int> ApproveAndConvertAsync(int applicationId, int sectionId, string approvedBy, CancellationToken cancellationToken = default)
    {

        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Admission application not found.");

        if (application.Status == AdmissionStatus.Converted)
            throw new InvalidOperationException("Application has already been converted.");

        var studentEmail = application.ApplicantEmail?.Trim();
        if (string.IsNullOrWhiteSpace(studentEmail))
        {
            var sanitizedName = application.ApplicantName.Replace(" ", ".").ToLower();
            studentEmail = $"{sanitizedName}.{application.Id}@school.com";
        }

        if (await _userRepository.AnyAsync(u => u.Email == studentEmail, cancellationToken))
            throw new InvalidOperationException($"A user account already exists for '{studentEmail}'.");

        var studentRole = await _roleRepository.FirstOrDefaultAsync(r => !r.IsDeleted && r.Name == "Student", cancellationToken)
            ?? throw new InvalidOperationException("Student role not found.");

        var year = DateTime.UtcNow.Year;
        var count = await _studentRepository.CountAsync(x => x.CreatedAt.Year == year, cancellationToken) + 1;
        var candidateUserName = $"STU-{year}{count:D3}";

        while (await _userRepository.AnyAsync(u => u.UserName == candidateUserName, cancellationToken))
        {
            count++;
            candidateUserName = $"STU-{year}{count:D3}";
        }

        var activationToken = Guid.NewGuid().ToString("N");
        var pendingUser = new ApplicationUser
        {
            UserName = candidateUserName,
            Email = studentEmail,
            PhoneNumber = application.ApplicantMobileNumber?.Trim(),
            Status = AccountStatus.Pending,
            PasswordHash = string.Empty,
            IsEmailConfirmed = false,
            ActivationToken = activationToken,
            ActivationTokenExpiry = DateTime.UtcNow.AddHours(24),
            CreatedBy = approvedBy,
            CreatedAt = DateTime.UtcNow
        };

        await _userRepository.AddAsync(pendingUser, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await _userRoleRepository.AddAsync(new UserRole { UserId = pendingUser.Id, RoleId = studentRole.Id }, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        var rollNumber = await NextRollAsync(application.AppliedClassId, sectionId, cancellationToken);

        var studentId = await _studentService.CreateAsync(new StudentUpsertDto
        {
            StudentNo = candidateUserName, FullName = application.ApplicantName, FullNameBangla = application.ApplicantNameBangla,
            DateOfBirth = application.DateOfBirth, Gender = application.Gender, FatherName = application.FatherName,
            FatherOccupation = application.FatherOccupation, MotherName = application.MotherName, MotherOccupation = application.MotherOccupation,
            GuardianName = application.GuardianName, GuardianOccupation = application.GuardianOccupation,
            MobileNumber = application.ApplicantMobileNumber ?? string.Empty, AlternativeNumber = application.AlternativeNumber,
            FatherOrGuardianMobileNo = application.FatherOrGuardianMobileNo, EmailAddress = studentEmail,
            Nationality = application.Nationality, Country = application.Country, MaritalStatus = application.MaritalStatus,
            Religion = application.Religion, BloodGroup = application.BloodGroup,
            BirthCertificateNo = application.BirthCertificateNo,
            ProfilePicturePath = application.ProfilePicturePath, PresentVillage = application.PresentVillage,
            PresentPostOffice = application.PresentPostOffice, PresentThana = application.PresentThana,
            PresentDistrict = application.PresentDistrict, PermanentVillage = application.PermanentVillage,
            PermanentPostOffice = application.PermanentPostOffice, PermanentThana = application.PermanentThana,
            PermanentDistrict = application.PermanentDistrict, ClassId = application.AppliedClassId,
            SectionId = sectionId, RollNumber = rollNumber, UserId = pendingUser.Id
        }, approvedBy, cancellationToken);

        application.Status = AdmissionStatus.Converted;
        application.ReviewedAt = DateTime.UtcNow;
        application.UpdatedBy = approvedBy;
        application.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        try { await _emailService.SendStudentActivationAsync(studentEmail, application.ApplicantName, pendingUser.UserName, activationToken, cancellationToken); }
        catch (Exception ex) { _logger.LogError(ex, "Student activation email failed for application {ApplicationNo}", application.ApplicationNo); }

        return studentId;
    }

    public async Task UpdateAsync(int id, AdmissionCreateDto dto, string updatedBy, CancellationToken cancellationToken)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new Exception("Admission not found");

        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            if (!string.IsNullOrEmpty(application.ProfilePicturePath)) DeleteFile(application.ProfilePicturePath);
            application.ProfilePicturePath = await SaveFileAsync(dto.ProfilePicture, "admissions/profiles", cancellationToken);
        }
        if (dto.BirthCertificateFile != null && dto.BirthCertificateFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(application.BirthCertificatePath)) DeleteFile(application.BirthCertificatePath);
            application.BirthCertificatePath = await SaveFileAsync(dto.BirthCertificateFile, "admissions/documents", cancellationToken);
        }
        if (dto.PaymentSlipFile != null && dto.PaymentSlipFile.Length > 0)
        {
            if (!string.IsNullOrEmpty(application.PaymentSlipPath)) DeleteFile(application.PaymentSlipPath);
            application.PaymentSlipPath = await SaveFileAsync(dto.PaymentSlipFile, "admissions/payments", cancellationToken);
        }

        application.ApplicantName = dto.ApplicantName?.Trim() ?? string.Empty;
        application.ApplicantNameBangla = dto.ApplicantNameBangla?.Trim();
        application.DateOfBirth = dto.DateOfBirth;
        application.Gender = dto.Gender?.Trim() ?? string.Empty;
        application.FatherName = dto.FatherName?.Trim() ?? string.Empty;
        application.FatherOccupation = dto.FatherOccupation?.Trim();
        application.MotherName = dto.MotherName?.Trim() ?? string.Empty;
        application.MotherOccupation = dto.MotherOccupation?.Trim();
        application.GuardianName = dto.GuardianName?.Trim();
        application.GuardianOccupation = dto.GuardianOccupation?.Trim();
        application.ApplicantMobileNumber = dto.ApplicantMobileNumber?.Trim() ?? string.Empty;
        application.AlternativeNumber = dto.AlternativeNumber?.Trim();
        application.FatherOrGuardianMobileNo = dto.FatherOrGuardianMobileNo?.Trim() ?? string.Empty;
        application.ApplicantEmail = dto.ApplicantEmail?.Trim();
        application.Nationality = dto.Nationality?.Trim() ?? string.Empty;
        application.Country = dto.Country?.Trim() ?? string.Empty;
        application.MaritalStatus = dto.MaritalStatus?.Trim() ?? string.Empty;
        application.Religion = dto.Religion?.Trim() ?? string.Empty;
        application.BloodGroup = dto.BloodGroup?.Trim();
        application.BirthCertificateNo = dto.BirthCertificateNo?.Trim();
        application.PresentVillage = dto.PresentVillage?.Trim();
        application.PresentPostOffice = dto.PresentPostOffice?.Trim();
        application.PresentThana = dto.PresentThana?.Trim();
        application.PresentDistrict = dto.PresentDistrict?.Trim();
        application.PermanentVillage = dto.PermanentVillage?.Trim();
        application.PermanentPostOffice = dto.PermanentPostOffice?.Trim();
        application.PermanentThana = dto.PermanentThana?.Trim();
        application.PermanentDistrict = dto.PermanentDistrict?.Trim();
        application.PaymentMethod = dto.PaymentMethod?.Trim();
        application.TransactionDetails = dto.TransactionDetails?.Trim();
        application.AppliedClassId = dto.AppliedClassId;
        application.LinkedGuardianId = dto.LinkedGuardianId;
        application.UpdatedAt = DateTime.UtcNow;
        application.UpdatedBy = updatedBy;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectAsync(int applicationId, string rejectedBy, CancellationToken cancellationToken = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Admission application not found.");

        application.Status = AdmissionStatus.Rejected;
        application.ReviewedAt = DateTime.UtcNow;
        application.UpdatedBy = rejectedBy;
        application.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<AdmissionApplication?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _admissionRepository.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Admission application not found.");

        application.IsDeleted = true;
        application.UpdatedBy = updatedBy;
        application.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<IEnumerable<dynamic>> GetAvailableClassesAsync(CancellationToken ct = default)
    {
        return await _classRepository.Query()
            .Where(c => c.Name != "Class Ten" && !c.IsDeleted)
            .Select(c => new { c.Id, c.Name })
            .ToListAsync(ct);
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

    private async Task<int> NextRollAsync(int classId, int sectionId, CancellationToken cancellationToken)
    {
        var maxRoll = await _studentRepository.Query()
            .Where(x => !x.IsDeleted && x.ClassId == classId && x.SectionId == sectionId)
            .Select(x => (int?)x.RollNumber)
            .MaxAsync(cancellationToken);
        return (maxRoll ?? 0) + 1;
    }
}

