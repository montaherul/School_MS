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
        // IMAGE UPLOAD FIRST
        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/admissions");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.ProfilePicture.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ProfilePicture.CopyToAsync(stream);
            }

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

            // ADDRESS
            PresentVillage = dto.PresentVillage?.Trim(),
            PresentPostOffice = dto.PresentPostOffice?.Trim(),
            PresentThana = dto.PresentThana?.Trim(),
            PresentDistrict = dto.PresentDistrict?.Trim(),

            PermanentVillage = dto.PermanentVillage?.Trim(),
            PermanentPostOffice = dto.PermanentPostOffice?.Trim(),
            PermanentThana = dto.PermanentThana?.Trim(),
            PermanentDistrict = dto.PermanentDistrict?.Trim(),

            // IMAGE
            ProfilePicturePath = dto.ProfilePicturePath,

            AppliedClassId = dto.AppliedClassId,
            Status = AdmissionStatus.Pending,
            CreatedBy = createdBy
        };

        _db.Admissions.Add(application);
        await _db.SaveChangesAsync(cancellationToken);

        return application.ApplicationNo;
    }

    public async Task<int> ApproveAndConvertAsync(int applicationId, int sectionId, string approvedBy, CancellationToken cancellationToken = default)
    {
        // FIX 1: Removed duplicate ?? throw
        var application = await _db.Admissions
            .FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Admission application not found.");

        if (application.Status == AdmissionStatus.Converted)
            throw new InvalidOperationException("Application has already been converted.");

        // FIX 2: Email guard kept once, cleanly — no duplicate commented block
        if (string.IsNullOrWhiteSpace(application.ApplicantEmail))
            throw new InvalidOperationException("Applicant email is required for activation onboarding.");

        // FIX 3: Single clean email variable — no dead fallback logic
        var studentEmail = application.ApplicantEmail.Trim();

        application.Status = AdmissionStatus.Approved;
        application.ReviewedAt = DateTime.UtcNow;
        application.UpdatedBy = approvedBy;
        application.UpdatedAt = DateTime.UtcNow;

        var studentRole = await _db.Roles
            .AsNoTracking()
            .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name == "Student", cancellationToken)
            ?? throw new InvalidOperationException("Student role not found.");

        // 1) Create an unconfirmed user (no password yet) + activation token.
        var activationToken = Guid.NewGuid().ToString("N");
        var activationExpiry = DateTime.UtcNow.AddHours(24);

        // FIX 4: Removed duplicate UserName and Email assignments on object initializer
        var pendingUser = new ApplicationUser
        {
            UserName = "S"+applicationId,
            Email = studentEmail,
            PhoneNumber = application.ApplicantMobileNumber?.Trim(),
            Status = AccountStatus.Pending,
            PasswordHash = string.Empty,
            IsEmailConfirmed = false,
            ActivationToken = activationToken,
            ActivationTokenExpiry = activationExpiry,
            CreatedBy = approvedBy,
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(pendingUser);
        await _db.SaveChangesAsync(cancellationToken);

        // 2) Assign the existing Student role.
        _db.UserRoles.Add(new UserRole { UserId = pendingUser.Id, RoleId = studentRole.Id });
        await _db.SaveChangesAsync(cancellationToken);

        var studentId = await _studentService.CreateAsync(new StudentUpsertDto
        {
            FullName = application.ApplicantName,
            FullNameBangla = application.ApplicantNameBangla,
            DateOfBirth = application.DateOfBirth,
            Gender = application.Gender,

            // Family
            FatherName = application.FatherName,
            FatherOccupation = application.FatherOccupation,
            MotherName = application.MotherName,
            MotherOccupation = application.MotherOccupation,
            GuardianName = application.GuardianName,
            GuardianOccupation = application.GuardianOccupation,

            // Contact
            MobileNumber = application.ApplicantMobileNumber ?? string.Empty,
            AlternativeNumber = application.AlternativeNumber,
            FatherOrGuardianMobileNo = application.FatherOrGuardianMobileNo,
            EmailAddress = application.ApplicantEmail,

            // Demographics
            Nationality = application.Nationality,
            Country = application.Country,
            MaritalStatus = application.MaritalStatus,
            Religion = application.Religion,
            BloodGroup = application.BloodGroup,

            // IDs
            PassportNo = application.PassportNo,
            NationalIdNo = application.NationalIdNo,
            BirthCertificateNo = application.BirthCertificateNo,

            // Picture
            ProfilePicturePath = application.ProfilePicturePath,

            // Present Address
            PresentVillage = application.PresentVillage,
            PresentPostOffice = application.PresentPostOffice,
            PresentThana = application.PresentThana,
            PresentDistrict = application.PresentDistrict,

            // Permanent Address
            PermanentVillage = application.PermanentVillage,
            PermanentPostOffice = application.PermanentPostOffice,
            PermanentThana = application.PermanentThana,
            PermanentDistrict = application.PermanentDistrict,

            // Academic
            ClassId = application.AppliedClassId,
            SectionId = sectionId,
            RollNumber = await NextRollAsync(application.AppliedClassId, sectionId, cancellationToken)
        }, approvedBy, cancellationToken);

        // 3) Link the student profile to the pending activation user.
        var student = await _db.Students.FirstOrDefaultAsync(s => s.Id == studentId, cancellationToken)
            ?? throw new InvalidOperationException("Student profile was not created.");

        student.UserId = pendingUser.Id;
        application.Status = AdmissionStatus.Converted;
        await _db.SaveChangesAsync(cancellationToken);

        // 4) Send activation email — email is guaranteed non-null here
        await _emailService.SendStudentActivationAsync(studentEmail, activationToken, cancellationToken);

        return studentId;
    }

    public async Task UpdateAsync(int id, AdmissionCreateDto dto, string updatedBy, CancellationToken cancellationToken)
    {
        var application = await _db.Admissions
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new Exception("Admission not found");

        // IMAGE UPDATE + DELETE OLD
        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/admissions");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // DELETE OLD
            if (!string.IsNullOrEmpty(application.ProfilePicturePath))
            {
                var oldPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    application.ProfilePicturePath.TrimStart('/')
                );

                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            // SAVE NEW
            var fileName = Guid.NewGuid() + Path.GetExtension(dto.ProfilePicture.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ProfilePicture.CopyToAsync(stream);
            }

            application.ProfilePicturePath = "/uploads/admissions/" + fileName;
        }

        // BASIC
        application.ApplicantName = dto.ApplicantName?.Trim() ?? string.Empty;
        application.ApplicantNameBangla = dto.ApplicantNameBangla?.Trim();
        application.DateOfBirth = dto.DateOfBirth;
        application.Gender = dto.Gender?.Trim() ?? string.Empty;

        // FAMILY
        application.FatherName = dto.FatherName?.Trim() ?? string.Empty;
        application.FatherOccupation = dto.FatherOccupation?.Trim();
        application.MotherName = dto.MotherName?.Trim() ?? string.Empty;
        application.MotherOccupation = dto.MotherOccupation?.Trim();
        application.GuardianName = dto.GuardianName?.Trim();
        application.GuardianOccupation = dto.GuardianOccupation?.Trim();

        // CONTACT
        application.ApplicantMobileNumber = dto.ApplicantMobileNumber?.Trim() ?? string.Empty;
        application.AlternativeNumber = dto.AlternativeNumber?.Trim();
        application.FatherOrGuardianMobileNo = dto.FatherOrGuardianMobileNo?.Trim() ?? string.Empty;
        application.ApplicantEmail = dto.ApplicantEmail?.Trim();

        // DEMOGRAPHICS
        application.Nationality = dto.Nationality?.Trim() ?? string.Empty;
        application.Country = dto.Country?.Trim() ?? string.Empty;
        application.MaritalStatus = dto.MaritalStatus?.Trim() ?? string.Empty;
        application.Religion = dto.Religion?.Trim() ?? string.Empty;
        application.BloodGroup = dto.BloodGroup?.Trim();

        // IDs
        application.PassportNo = dto.PassportNo?.Trim();
        application.NationalIdNo = dto.NationalIdNo?.Trim();
        application.BirthCertificateNo = dto.BirthCertificateNo?.Trim();

        // ADDRESS
        application.PresentVillage = dto.PresentVillage?.Trim();
        application.PresentPostOffice = dto.PresentPostOffice?.Trim();
        application.PresentThana = dto.PresentThana?.Trim();
        application.PresentDistrict = dto.PresentDistrict?.Trim();

        application.PermanentVillage = dto.PermanentVillage?.Trim();
        application.PermanentPostOffice = dto.PermanentPostOffice?.Trim();
        application.PermanentThana = dto.PermanentThana?.Trim();
        application.PermanentDistrict = dto.PermanentDistrict?.Trim();

        // PAYMENT
        application.PaymentMethod = dto.PaymentMethod?.Trim();
        application.TransactionDetails = dto.TransactionDetails?.Trim();

        // ACADEMIC
        application.AppliedClassId = dto.AppliedClassId;

        // AUDIT
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