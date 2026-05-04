using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Students;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class AdmissionService : IAdmissionService
{
    private readonly SchoolDbContext _db;
    private readonly IStudentService _studentService;
    private readonly IEmailService _emailService;

    public AdmissionService(SchoolDbContext db, IStudentService studentService, IEmailService emailService)
    {
        _db = db;
        _studentService = studentService;
        _emailService = emailService;
    }

    public async Task<string> SubmitAsync(AdmissionCreateDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/admissions");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.ProfilePicture.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.ProfilePicture.CopyToAsync(stream, cancellationToken);

            dto.ProfilePicturePath = "/uploads/admissions/" + fileName;
        }

        var count = await _db.Admissions.CountAsync(x => x.CreatedAt.Year == DateTime.UtcNow.Year, cancellationToken) + 1;

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

            PassportNo = dto.PassportNo?.Trim(),
            NationalIdNo = dto.NationalIdNo?.Trim(),
            BirthCertificateNo = dto.BirthCertificateNo?.Trim(),

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
            Status = AdmissionStatus.Pending,
            CreatedBy = createdBy
        };

        _db.Admissions.Add(application);
        await _db.SaveChangesAsync(cancellationToken);

        // ── Send admission received email ──────────────────────────────────────
        if (!string.IsNullOrWhiteSpace(application.ApplicantEmail))
        {
            try
            {
                await _emailService.SendAdmissionReceivedAsync(application.ApplicantEmail, application.ApplicantName, application.ApplicationNo, cancellationToken);
            }
            catch
            {
                // Log and ignore email failures for submission
            }
        }

        return application.ApplicationNo;
    }

    public async Task<int> ApproveAndConvertAsync(
        int applicationId,
        int sectionId,
        string approvedBy,
        CancellationToken cancellationToken = default)
    {
        var strategy = _db.Database.CreateExecutionStrategy();

        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _db.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                var application = await _db.Admissions
                    .FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, cancellationToken)
                    ?? throw new InvalidOperationException("Admission application not found.");

                if (application.Status == AdmissionStatus.Converted)
                    throw new InvalidOperationException("Application has already been converted.");

                // ── Fallback Email Logic ─────────────────────────────────────────
                var studentEmail = application.ApplicantEmail?.Trim();
                if (string.IsNullOrWhiteSpace(studentEmail))
                {
                    // Fallback: name_id@school.com
                    var sanitizedName = application.ApplicantName.Replace(" ", ".").ToLower();
                    studentEmail = $"{sanitizedName}.{application.Id}@school.com";
                }

                // ── Guard: don't create a duplicate user for the same email ──────────
                var existingUser = await _db.Users
                    .FirstOrDefaultAsync(u => u.Email == studentEmail, cancellationToken);
                if (existingUser != null)
                    throw new InvalidOperationException($"A user account already exists for '{studentEmail}'.");

                var studentRole = await _db.Roles
                    .AsNoTracking()
                    .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name == "Student", cancellationToken)
                    ?? throw new InvalidOperationException("Student role not found.");

                // ── Build a collision-safe UserName ──────────────────────────────────
                // Pattern: name.id
                var candidateUserName = application.ApplicantName.Replace(" ", ".").ToLower();
                if (candidateUserName.Length > 20) candidateUserName = candidateUserName[..20];
                candidateUserName = $"{candidateUserName}.{application.Id}";

                var userNameExists = await _db.Users
                    .AnyAsync(u => u.UserName == candidateUserName, cancellationToken);
                if (userNameExists)
                    candidateUserName = $"{candidateUserName}.{Guid.NewGuid():N[..4]}";

                // ── Activation token ─────────────────────────────────────────────────
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

                // ── Persist user + role ─────────────────────────────────────────────
                _db.Users.Add(pendingUser);
                await _db.SaveChangesAsync(cancellationToken);          // get pendingUser.Id

                _db.UserRoles.Add(new UserRole { UserId = pendingUser.Id, RoleId = studentRole.Id });
                await _db.SaveChangesAsync(cancellationToken);

                // ── Compute roll number (locked to this class+section) ───────────────
                var rollNumber = await NextRollAsync(application.AppliedClassId, sectionId, cancellationToken);

                // ── Create student profile ───────────────────────────────────────────
                var studentId = await _studentService.CreateAsync(new StudentUpsertDto
                {
                    FullName = application.ApplicantName,
                    FullNameBangla = application.ApplicantNameBangla,
                    DateOfBirth = application.DateOfBirth,
                    Gender = application.Gender,

                    FatherName = application.FatherName,
                    FatherOccupation = application.FatherOccupation,
                    MotherName = application.MotherName,
                    MotherOccupation = application.MotherOccupation,
                    GuardianName = application.GuardianName,
                    GuardianOccupation = application.GuardianOccupation,

                    MobileNumber = application.ApplicantMobileNumber ?? string.Empty,
                    AlternativeNumber = application.AlternativeNumber,
                    FatherOrGuardianMobileNo = application.FatherOrGuardianMobileNo,
                    EmailAddress = studentEmail,

                    Nationality = application.Nationality,
                    Country = application.Country,
                    MaritalStatus = application.MaritalStatus,
                    Religion = application.Religion,
                    BloodGroup = application.BloodGroup,

                    PassportNo = application.PassportNo,
                    NationalIdNo = application.NationalIdNo,
                    BirthCertificateNo = application.BirthCertificateNo,

                    ProfilePicturePath = application.ProfilePicturePath,

                    PresentVillage = application.PresentVillage,
                    PresentPostOffice = application.PresentPostOffice,
                    PresentThana = application.PresentThana,
                    PresentDistrict = application.PresentDistrict,

                    PermanentVillage = application.PermanentVillage,
                    PermanentPostOffice = application.PermanentPostOffice,
                    PermanentThana = application.PermanentThana,
                    PermanentDistrict = application.PermanentDistrict,

                    ClassId = application.AppliedClassId,
                    SectionId = sectionId,
                    RollNumber = rollNumber,
                    UserId = pendingUser.Id
                }, approvedBy, cancellationToken);

                // ── Mark application Converted ──────────────────────────────────────
                application.Status = AdmissionStatus.Converted;
                application.ReviewedAt = DateTime.UtcNow;
                application.UpdatedBy = approvedBy;
                application.UpdatedAt = DateTime.UtcNow;

                await _db.SaveChangesAsync(cancellationToken);

                // ── Commit Transaction ──────────────────────────────────────────────
                await transaction.CommitAsync(cancellationToken);

                // ── Send activation email (after DB is consistent) ───────────────────
                try
                {
                    await _emailService.SendStudentActivationAsync(studentEmail,application.ApplicantName, pendingUser.UserName,activationToken,cancellationToken);
                }
                catch
                {
                    // Log but don't fail the conversion if only email fails
                }

                return studentId;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        });
    }

    public async Task UpdateAsync(int id, AdmissionCreateDto dto, string updatedBy, CancellationToken cancellationToken)
    {
        var application = await _db.Admissions
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new Exception("Admission not found");

        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/admissions");
            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            if (!string.IsNullOrEmpty(application.ProfilePicturePath))
            {
                var oldPath = Path.Combine(
                    Directory.GetCurrentDirectory(), "wwwroot",
                    application.ProfilePicturePath.TrimStart('/'));
                if (File.Exists(oldPath)) File.Delete(oldPath);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.ProfilePicture.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.ProfilePicture.CopyToAsync(stream, cancellationToken);

            application.ProfilePicturePath = "/uploads/admissions/" + fileName;
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

        application.PassportNo = dto.PassportNo?.Trim();
        application.NationalIdNo = dto.NationalIdNo?.Trim();
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
        application.UpdatedAt = DateTime.UtcNow;
        application.UpdatedBy = updatedBy;

        await _db.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectAsync(int applicationId, string rejectedBy, CancellationToken cancellationToken = default)
    {
        var application = await _db.Admissions
            .FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Admission application not found.");

        application.Status = AdmissionStatus.Rejected;
        application.ReviewedAt = DateTime.UtcNow;
        application.UpdatedBy = rejectedBy;
        application.UpdatedAt = DateTime.UtcNow;
        await _db.SaveChangesAsync(cancellationToken);
    }

    private async Task<int> NextRollAsync(int classId, int sectionId, CancellationToken cancellationToken)
    {
        var maxRoll = await _db.Students
            .Where(x => !x.IsDeleted && x.ClassId == classId && x.SectionId == sectionId)
            .Select(x => (int?)x.RollNumber)
            .MaxAsync(cancellationToken);
        return (maxRoll ?? 0) + 1;
    }
}
