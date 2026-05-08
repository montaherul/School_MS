using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.DTOs.Admission.StoredProcedures;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Students;
using System.Linq;

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
              var year = DateTime.UtcNow.Year;
                var count = await _db.Students.CountAsync(x => x.CreatedAt.Year == year, cancellationToken) + 1;
                var candidateUserName = $"STU-{year}{count:D3}";


               while (await _db.Users.AnyAsync(u => u.UserName == candidateUserName, cancellationToken))
                {
                    count++;
                    candidateUserName = $"STU-{year}{count:D3}";
                }

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
                    StudentNo = candidateUserName,   
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

    /// <summary>
    /// Get admission list using stored procedure with pagination, search, and filtering
    /// </summary>
    public async Task<(List<AdmissionListResultDto> items, int totalRecords, object counts)> GetListByStoredProcedureAsync(
        int pageNumber = 1,
        int pageSize = 10,
        string? searchTerm = null,
        int classId = 0,
        CancellationToken cancellationToken = default,
        int? status= null
       )
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetAdmissionList";
        command.CommandType = System.Data.CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@PageNumber", pageNumber));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
        command.Parameters.Add(new SqlParameter("@SearchTerm", (object?)searchTerm ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@ClassId", classId));
        command.Parameters.Add(new SqlParameter("@Status", status ?? (object)DBNull.Value));

        await _db.Database.OpenConnectionAsync(cancellationToken);
        try
        {
            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            var items = new List<AdmissionListResultDto>();

            while (await reader.ReadAsync(cancellationToken))
            {
                items.Add(new AdmissionListResultDto
                {
                    Id = reader.GetInt32(0),
                    ApplicationNo = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    ApplicantName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    DateOfBirth = reader.IsDBNull(3) ? DateTime.MinValue : reader.GetDateTime(3),
                    Gender = reader.IsDBNull(4) ? string.Empty : reader.GetString(4),
                    AppliedClassId = reader.IsDBNull(5) ? 0 : reader.GetInt32(5),
                    ClassName = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    Status = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    FatherName = reader.IsDBNull(8) ? string.Empty : reader.GetString(8),
                    FatherOccupation = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                    MotherName = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                    MotherOccupation = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                    GuardianName = reader.IsDBNull(12) ? string.Empty : reader.GetString(12),
                    GuardianOccupation = reader.IsDBNull(13) ? string.Empty : reader.GetString(13),
                    FatherOrGuardianMobileNo = reader.IsDBNull(14) ? string.Empty : reader.GetString(14),
                    ApplicantMobileNumber = reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                    AlternativeNumber = reader.IsDBNull(16) ? string.Empty : reader.GetString(16),
                    ApplicantEmail = reader.IsDBNull(17) ? string.Empty : reader.GetString(17),
                    Nationality = reader.IsDBNull(18) ? string.Empty : reader.GetString(18),
                    Religion = reader.IsDBNull(19) ? string.Empty : reader.GetString(19),
                    BloodGroup = reader.IsDBNull(20) ? string.Empty : reader.GetString(20),
                    NationalIdNo = reader.IsDBNull(21) ? string.Empty : reader.GetString(21),
                    BirthCertificateNo = reader.IsDBNull(22) ? string.Empty : reader.GetString(22),
                    PassportNo = reader.IsDBNull(23) ? string.Empty : reader.GetString(23),
                    PaymentMethod = reader.IsDBNull(24) ? string.Empty : reader.GetString(24),
                    TransactionDetails = reader.IsDBNull(25) ? string.Empty : reader.GetString(25),
                    PresentVillage = reader.IsDBNull(26) ? string.Empty : reader.GetString(26),
                    PresentPostOffice = reader.IsDBNull(27) ? string.Empty : reader.GetString(27),
                    PresentThana = reader.IsDBNull(28) ? string.Empty : reader.GetString(28),
                    PresentDistrict = reader.IsDBNull(29) ? string.Empty : reader.GetString(29),
                    PermanentVillage = reader.IsDBNull(30) ? string.Empty : reader.GetString(30),
                    PermanentPostOffice = reader.IsDBNull(31) ? string.Empty : reader.GetString(31),
                    PermanentThana = reader.IsDBNull(32) ? string.Empty : reader.GetString(32),
                    PermanentDistrict = reader.IsDBNull(33) ? string.Empty : reader.GetString(33),
                    ProfilePicturePath = reader.IsDBNull(34) ? string.Empty : reader.GetString(34),
                    CreatedBy = reader.IsDBNull(35) ? string.Empty : reader.GetString(35),
                    CreatedAt = reader.IsDBNull(36) ? DateTime.MinValue : reader.GetDateTime(36),
                    TotalRecords = reader.IsDBNull(37) ? 0 : reader.GetInt32(37)
                });
            }

            // Calculate total records for the current filter (including status)
            int totalRecords = await _db.Admissions
                .Where(a => !a.IsDeleted)
                .Where(a => classId == 0 || a.AppliedClassId == classId)
                .Where(a => status == null || (int)a.Status == status)
                .Where(a => string.IsNullOrEmpty(searchTerm) ||
                    a.ApplicantName.Contains(searchTerm) ||
                    a.ApplicationNo.Contains(searchTerm) ||
                    a.FatherOrGuardianMobileNo.Contains(searchTerm) ||
                    a.ApplicantMobileNumber.Contains(searchTerm))
                .CountAsync(cancellationToken);

            // Calculate status counts (tabs) - ignore the status filter for these
            var counts = await _db.Admissions
                .Where(a => !a.IsDeleted)
                .Where(a => classId == 0 || a.AppliedClassId == classId)
                .Where(a => string.IsNullOrEmpty(searchTerm) ||
                    a.ApplicantName.Contains(searchTerm) ||
                    a.ApplicationNo.Contains(searchTerm) ||
                    a.FatherOrGuardianMobileNo.Contains(searchTerm) ||
                    a.ApplicantMobileNumber.Contains(searchTerm))
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            // Convert to UI format
            var countsObj = new
            {
                Pending = counts.FirstOrDefault(x => x.Status == AdmissionStatus.Pending)?.Count ?? 0,
                Approved = counts.FirstOrDefault(x => x.Status == AdmissionStatus.Approved)?.Count ?? 0,
                Rejected = counts.FirstOrDefault(x => x.Status == AdmissionStatus.Rejected)?.Count ?? 0,
                Converted = counts.FirstOrDefault(x => x.Status == AdmissionStatus.Converted)?.Count ?? 0
            };
            return (items, totalRecords, countsObj);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }
}
