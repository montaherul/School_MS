using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Guardian;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Repositories.Interfaces.Website;
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
    private readonly ISectionRepository _sectionRepository;
    private readonly IGuardianService _guardianService;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly ILogger<AdmissionService> _logger;
    private readonly IHttpContextAccessor _httpContextAccessor;

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
        ISectionRepository sectionRepository,
        IGuardianService guardianService,
        ISchoolSettingRepository settingRepo,
        ILogger<AdmissionService> logger,
        IHttpContextAccessor httpContextAccessor)
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
        _sectionRepository = sectionRepository;
        _guardianService = guardianService;
        _settingRepo = settingRepo;
        _logger = logger;
        _httpContextAccessor = httpContextAccessor;
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

        var query = _admissionRepository.Query().AsNoTracking()
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
        if (dto.GuardianPhoto != null && dto.GuardianPhoto.Length > 0)
        {
            dto.GuardianPhotoPath = await SaveFileAsync(dto.GuardianPhoto, "admissions/guardians", cancellationToken);
        }

        // Look up admission fee from fee structure
        decimal admissionFee = 0;
        var feeStructure = await _unitOfWork.Repository<AdmissionFeeStructure>()
            .FirstOrDefaultAsync(f => f.SchoolClassId == dto.AppliedClassId && f.IsActive && !f.IsDeleted, cancellationToken);
        if (feeStructure != null)
            admissionFee = feeStructure.AdmissionFee;

        string applicationNo;
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var year = DateTime.UtcNow.Year;
            var maxAppNo = await _admissionRepository.Query()
                .Where(x => x.CreatedAt.Year == year && !x.IsDeleted)
                .MaxAsync(x => (string?)x.ApplicationNo, cancellationToken);

            var nextSeq = 1;
            if (!string.IsNullOrEmpty(maxAppNo))
            {
                var lastDash = maxAppNo.LastIndexOf('-');
                if (lastDash >= 0 && int.TryParse(maxAppNo[(lastDash + 1)..], out var lastSeq))
                    nextSeq = lastSeq + 1;
            }

            applicationNo = $"APP-{year}-{nextSeq:0000}";

            var application = new AdmissionApplication
            {
                ApplicationNo = applicationNo,
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
            GuardianEmail = dto.GuardianEmail?.Trim(),
            GuardianMobileNumber = dto.GuardianMobileNumber?.Trim(),
            GuardianRelationship = dto.GuardianRelationship?.Trim(),
            GuardianNationalId = dto.GuardianNationalId?.Trim(),
            GuardianAddress = dto.GuardianAddress?.Trim(),
            GuardianPhoto = dto.GuardianPhotoPath,
            GuardianRemarks = dto.GuardianRemarks?.Trim(),
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
            AppliedStudentGroupId = dto.AppliedStudentGroupId,
            LinkedGuardianId = dto.LinkedGuardianId,
            AdmissionFee = admissionFee,
            AdmissionFeePaid = false,
            Status = AdmissionStatus.Pending,
            CreatedBy = createdBy
        };

            await _admissionRepository.AddAsync(application, cancellationToken);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await LogAuditAsync("Admission", "Admission.Apply", application.Id.ToString(), $"Application submitted: {application.ApplicantName} ({application.ApplicationNo})", cancellationToken);

            if (!string.IsNullOrWhiteSpace(application.ApplicantEmail))
            {
                try { await _emailService.SendAdmissionReceivedAsync(application.ApplicantEmail, application.ApplicantName, application.ApplicationNo, cancellationToken); }
                catch (Exception ex) { _logger.LogError(ex, "Admission confirmation email failed for application {ApplicationNo}", application.ApplicationNo); }
            }

            return application.ApplicationNo;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<int> ApproveAndConvertAsync(int applicationId, int sectionId, string approvedBy, CancellationToken cancellationToken = default)
    {
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Admission application not found.");

            if (application.Status == AdmissionStatus.Converted)
                throw new InvalidOperationException("Application has already been converted.");

            // Inner idempotency check: re-read status within the transaction to prevent race conditions
            var currentStatus = await _admissionRepository.Query()
                .Where(x => x.Id == applicationId)
                .Select(x => x.Status)
                .FirstOrDefaultAsync(cancellationToken);
            if (currentStatus == AdmissionStatus.Converted)
                throw new InvalidOperationException("Application was already converted by another admin.");

            if (currentStatus == AdmissionStatus.Rejected)
                throw new InvalidOperationException("Cannot convert a rejected application.");

            if (!application.AdmissionFeePaid)
                throw new InvalidOperationException($"Admission fee (BDT {application.AdmissionFee:N2}) must be paid before conversion.");

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

            // Derive group from section's StudentGroupId or application preference
            var section = await _sectionRepository.FirstOrDefaultAsync(x => x.Id == sectionId && !x.IsDeleted, cancellationToken)
                ?? throw new InvalidOperationException("Selected section not found or has been deleted.");
            var studentGroupId = section.StudentGroupId ?? application.AppliedStudentGroupId;

            // Fallback: if no group set and exactly one StudentGroup matches this class, auto-assign
            if (!studentGroupId.HasValue)
            {
                var schoolClass = await _classRepository.FirstOrDefaultAsync(x => x.Id == application.AppliedClassId && !x.IsDeleted, cancellationToken);
                if (schoolClass != null)
                {
                    var matchingGroups = await _unitOfWork.Repository<StudentGroup>().Query()
                        .Where(g => g.IsActive && !g.IsDeleted
                            && g.MinClass <= schoolClass.SortOrder
                            && g.MaxClass >= schoolClass.SortOrder)
                        .ToListAsync(cancellationToken);
                    if (matchingGroups.Count == 1)
                        studentGroupId = matchingGroups[0].Id;
                }
            }

            // PHASE 42H.2: Optional Guardian Portal — check settings before creating guardian
            var settings = await _settingRepo.GetCurrentSettingsAsync(cancellationToken);
            bool guardianPortalEnabled = settings?.EnableGuardianPortal ?? false;
            bool guardianActivationEnabled = settings?.EnableGuardianActivation ?? false;

            int? linkedGuardianId = null;
            string? guardianActivationToken = null;
            string? guardianEmail = null;
            string? guardianFullName = null;
            string? guardianCode = null;

            if (guardianPortalEnabled)
            {
                // PHASE 6: Ensure Guardian — with name-verification to prevent impersonation
                var guardian = await EnsureGuardianFromAdmissionSafeAsync(application, cancellationToken);
                application.LinkedGuardianId = guardian.Id;
                linkedGuardianId = guardian.Id;
                guardianEmail = guardian.Email;
                guardianFullName = guardian.FullName;
                guardianCode = guardian.GuardianCode;

                // PHASE 6/7: Create a Guardian portal user (gdn-{Code}) + activation link
                if (guardianActivationEnabled)
                {
                    try
                    {
                        guardianActivationToken = await _guardianService.EnsureGuardianUserAsync(guardian.Id, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Guardian user provisioning failed for admission {ApplicationNo}", application.ApplicationNo);
                    }
                }
            }
            else
            {
                // Guardian portal disabled — skip guardian creation entirely
                application.LinkedGuardianId = null;
            }

            // When guardian portal is off, prevent StudentService from creating inline guardians
            var studentDto = new StudentUpsertDto
            {
                StudentNo = candidateUserName, FullName = application.ApplicantName, FullNameBangla = application.ApplicantNameBangla,
                DateOfBirth = application.DateOfBirth, Gender = application.Gender, FatherName = application.FatherName,
                FatherOccupation = application.FatherOccupation, MotherName = application.MotherName, MotherOccupation = application.MotherOccupation,
                GuardianName = application.GuardianName, GuardianOccupation = application.GuardianOccupation,
                MobileNumber = application.ApplicantMobileNumber ?? string.Empty, AlternativeNumber = application.AlternativeNumber,
                FatherOrGuardianMobileNo = guardianPortalEnabled ? application.FatherOrGuardianMobileNo : null,
                EmailAddress = studentEmail,
                Nationality = application.Nationality, Country = application.Country, MaritalStatus = application.MaritalStatus,
                Religion = application.Religion, BloodGroup = application.BloodGroup,
                BirthCertificateNo = application.BirthCertificateNo,
                ProfilePicturePath = application.ProfilePicturePath, PresentVillage = application.PresentVillage,
                PresentPostOffice = application.PresentPostOffice, PresentThana = application.PresentThana,
                PresentDistrict = application.PresentDistrict, PermanentVillage = application.PermanentVillage,
                PermanentPostOffice = application.PermanentPostOffice, PermanentThana = application.PermanentThana,
                PermanentDistrict = application.PermanentDistrict, ClassId = application.AppliedClassId,
                SectionId = sectionId, StudentGroupId = studentGroupId, RollNumber = rollNumber, UserId = pendingUser.Id,
                LinkedGuardianId = linkedGuardianId
            };

            var studentId = await _studentService.CreateAsync(studentDto, approvedBy, cancellationToken);

            application.Status = AdmissionStatus.Converted;
            application.ReviewedAt = DateTime.UtcNow;
            if (int.TryParse(approvedBy, out var reviewerId)) application.ReviewedByUserId = reviewerId;
            application.UpdatedBy = approvedBy;
            application.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // --- Fee invoice creation for admission ---
            var feeStructure = await _unitOfWork.Repository<AdmissionFeeStructure>()
                .FirstOrDefaultAsync(f => f.SchoolClassId == application.AppliedClassId && f.IsActive && !f.IsDeleted, cancellationToken);

            var admissionFee = feeStructure?.AdmissionFee ?? application.AdmissionFee;
            var invoiceKey = $"AdmissionApp_{applicationId}";

            if (!await _unitOfWork.Repository<FeeInvoice>().AnyAsync(i => i.Remarks == invoiceKey && !i.IsDeleted, cancellationToken))
            {
                var invoiceNo = $"INV-ADM-{DateTime.UtcNow:yyyyMMdd}-{Random.Shared.Next(1000, 9999):D4}";
                var isPaid = application.AdmissionFeePaid;

                var invoice = new FeeInvoice
                {
                    InvoiceNo = invoiceNo,
                    StudentId = studentId,
                    DueDate = DateOnly.FromDateTime(DateTime.UtcNow.Date.AddDays(30)),
                    TotalAmount = admissionFee,
                    PaidAmount = isPaid ? admissionFee : 0,
                    Status = isPaid ? PaymentStatus.Paid : PaymentStatus.Unpaid,
                    Remarks = invoiceKey,
                    CreatedBy = approvedBy,
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.Repository<FeeInvoice>().AddAsync(invoice, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);

                var className = feeStructure?.ClassName ?? $"Class-{application.AppliedClassId}";

                await _unitOfWork.Repository<FeeInvoiceItem>().AddAsync(new FeeInvoiceItem
                {
                    FeeInvoiceId = invoice.Id,
                    Description = $"Admission Fee - {className}",
                    Amount = admissionFee,
                    NetAmount = admissionFee,
                    CreatedBy = approvedBy,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                await _unitOfWork.Repository<FeeLedger>().AddAsync(new FeeLedger
                {
                    StudentId = studentId,
                    FeeInvoiceId = invoice.Id,
                    TransactionType = FeeLedgerType.Invoice,
                    Debit = isPaid ? 0 : admissionFee,
                    Credit = 0,
                    Balance = isPaid ? 0 : admissionFee,
                    Description = $"Invoice created: {invoiceNo}",
                    TransactionDate = DateTime.UtcNow,
                    CreatedBy = approvedBy,
                    CreatedAt = DateTime.UtcNow
                }, cancellationToken);

                await _unitOfWork.SaveChangesAsync(cancellationToken);

                if (isPaid)
                {
                    await _unitOfWork.Repository<FeeLedger>().AddAsync(new FeeLedger
                    {
                        StudentId = studentId,
                        FeeInvoiceId = invoice.Id,
                        TransactionType = FeeLedgerType.Payment,
                        Debit = 0,
                        Credit = admissionFee,
                        Balance = 0,
                        Description = $"Payment for admission invoice: {invoiceNo}",
                        TransactionDate = DateTime.UtcNow,
                        CreatedBy = approvedBy,
                        CreatedAt = DateTime.UtcNow
                    }, cancellationToken);

                    await _unitOfWork.SaveChangesAsync(cancellationToken);
                }
            }

            await _unitOfWork.CommitTransactionAsync(cancellationToken);

            await LogAuditAsync("Admission", "Admission.Approve", applicationId.ToString(), $"Application approved & converted to student: {application.ApplicantName} ({application.ApplicationNo})", cancellationToken);

            try { await _emailService.SendStudentActivationAsync(studentEmail, application.ApplicantName, pendingUser.UserName, activationToken, cancellationToken); }
            catch (Exception ex) { _logger.LogError(ex, "Student activation email failed for application {ApplicationNo}", application.ApplicationNo); }

            if (!string.IsNullOrEmpty(guardianActivationToken) && !string.IsNullOrWhiteSpace(guardianEmail))
            {
                try
                {
                    // The EmailService falls back to its own configured BaseUrl/LocalUrl when an empty string is passed.
                    await _emailService.SendGuardianActivationAsync(
                        guardianEmail,
                        guardianFullName ?? application.GuardianName ?? application.FatherName ?? "Guardian",
                        $"gdn-{(guardianCode ?? "guardian").Replace("-", string.Empty)}",
                        guardianActivationToken,
                        string.Empty,
                        cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Guardian activation email failed for admission {ApplicationNo}", application.ApplicationNo);
                }
            }

            return studentId;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task UpdateAsync(int id, AdmissionCreateDto dto, string updatedBy, CancellationToken cancellationToken)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new Exception("Admission not found");

        if (application.Status == AdmissionStatus.Converted)
            throw new InvalidOperationException("Cannot update a converted application.");
        if (application.Status == AdmissionStatus.Rejected)
            throw new InvalidOperationException("Cannot update a rejected application.");

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
        if (dto.GuardianPhoto != null && dto.GuardianPhoto.Length > 0)
        {
            if (!string.IsNullOrEmpty(application.GuardianPhoto)) DeleteFile(application.GuardianPhoto);
            dto.GuardianPhotoPath = await SaveFileAsync(dto.GuardianPhoto, "admissions/guardians", cancellationToken);
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
        application.GuardianEmail = dto.GuardianEmail?.Trim();
        application.GuardianMobileNumber = dto.GuardianMobileNumber?.Trim();
        application.GuardianRelationship = dto.GuardianRelationship?.Trim();
        application.GuardianNationalId = dto.GuardianNationalId?.Trim();
        application.GuardianAddress = dto.GuardianAddress?.Trim();
        application.GuardianPhoto = dto.GuardianPhotoPath ?? application.GuardianPhoto;
        application.GuardianRemarks = dto.GuardianRemarks?.Trim();
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
        application.AppliedStudentGroupId = dto.AppliedStudentGroupId;
        application.LinkedGuardianId = dto.LinkedGuardianId;
        application.UpdatedAt = DateTime.UtcNow;
        application.UpdatedBy = updatedBy;

        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task RejectAsync(int applicationId, string rejectedBy, CancellationToken cancellationToken = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Admission application not found.");

        if (application.Status == AdmissionStatus.Converted)
            throw new InvalidOperationException("Cannot reject a converted application.");
        if (application.Status == AdmissionStatus.Rejected)
            throw new InvalidOperationException("Application has already been rejected.");

        application.Status = AdmissionStatus.Rejected;
        application.ReviewedAt = DateTime.UtcNow;
        if (int.TryParse(rejectedBy, out var reviewerId)) application.ReviewedByUserId = reviewerId;
        application.UpdatedBy = rejectedBy;
        application.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await LogAuditAsync("Admission", "Admission.Reject", applicationId.ToString(), $"Application rejected: {application.ApplicantName} ({application.ApplicationNo})", cancellationToken);
    }

    public async Task<AdmissionApplication?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        return await _admissionRepository.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Admission application not found.");

        if (application.Status == AdmissionStatus.Converted)
            throw new InvalidOperationException("Cannot delete a converted application.");

        // Cascade soft-delete to associated documents to prevent orphans
        var docs = await _unitOfWork.Repository<AdmissionDocument>().Query()
            .Where(d => d.AdmissionApplicationId == id && !d.IsDeleted)
            .ToListAsync(ct);
        foreach (var doc in docs)
        {
            doc.IsDeleted = true;
            DeleteFile(doc.FilePath);
        }

        // Also clean up uploaded files for this application
        DeleteFile(application.ProfilePicturePath);
        DeleteFile(application.BirthCertificatePath);
        DeleteFile(application.PaymentSlipPath);
        DeleteFile(application.GuardianPhoto);

        application.IsDeleted = true;
        application.UpdatedBy = updatedBy;
        application.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);

        await LogAuditAsync("Admission", "Admission.Delete", id.ToString(), $"Application deleted: {application.ApplicantName} ({application.ApplicationNo})", ct);
    }

    public async Task<IEnumerable<dynamic>> GetAvailableClassesAsync(CancellationToken ct = default)
    {
        return await _classRepository.Query().AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Select(c => new { c.Id, c.Name, c.IsGroupBased, c.SortOrder })
            .ToListAsync(ct);
    }

    public async Task<IEnumerable<dynamic>> GetActiveStudentGroupsAsync(CancellationToken ct = default)
    {
        return await _unitOfWork.Repository<StudentGroup>().Query().AsNoTracking()
            .Where(g => g.IsActive && !g.IsDeleted)
            .Select(g => new { g.Id, g.Name, g.MinClass, g.MaxClass })
            .ToListAsync(ct);
    }

    private static readonly HashSet<string> _allowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".jpg", ".jpeg", ".png", ".pdf" };
    private static readonly HashSet<string> _blockedExtensions = new(StringComparer.OrdinalIgnoreCase)
        { ".exe", ".dll", ".bat", ".cmd", ".ps1", ".aspx", ".cshtml", ".js", ".html", ".svg" };
    private const long _maxFileSize = 5 * 1024 * 1024; // 5 MB

    private async Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken ct)
    {
        var ext = Path.GetExtension(file.FileName);
        if (string.IsNullOrEmpty(ext) || !_allowedExtensions.Contains(ext) || _blockedExtensions.Contains(ext))
            throw new InvalidOperationException($"File type '{ext}' is not allowed. Allowed: jpg, jpeg, png, pdf.");

        if (file.Length == 0 || file.Length > _maxFileSize)
            throw new InvalidOperationException($"File size must be between 1 byte and 5 MB.");

        var contentType = file.ContentType?.ToLowerInvariant() ?? string.Empty;
        var allowedTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
            { "image/jpeg", "image/jpg", "image/png", "application/pdf" };
        if (!allowedTypes.Contains(contentType))
            throw new InvalidOperationException($"Content type '{contentType}' is not allowed.");

        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", subFolder);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        var safeName = Guid.NewGuid().ToString("N") + ext;
        var filePath = Path.Combine(folderPath, safeName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, ct);
        return $"/uploads/{subFolder}/{safeName}";
    }

    private void DeleteFile(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }

    private async Task<int> NextRollAsync(int classId, int sectionId, CancellationToken cancellationToken)
    {
        var maxRoll = await _studentRepository.Query().AsNoTracking()
            .Where(x => !x.IsDeleted && x.ClassId == classId && x.SectionId == sectionId)
            .Select(x => (int?)x.RollNumber)
            .MaxAsync(cancellationToken);
        return (maxRoll ?? 0) + 1;
    }

    private async Task<Models.Entities.Guardian.Guardian> EnsureGuardianFromAdmissionSafeAsync(
        AdmissionApplication application, CancellationToken ct)
    {
        var email = application.GuardianEmail?.Trim().ToLowerInvariant();
        var name = application.GuardianName?.Trim();
        var fallbackName = string.IsNullOrWhiteSpace(name) ? application.FatherName?.Trim() : name;
        if (string.IsNullOrWhiteSpace(fallbackName))
            throw new InvalidOperationException("Cannot create a Guardian: both GuardianName and FatherName are empty.");

        // 1) Direct link from admission form (admin explicitly picked a guardian)
        if (application.LinkedGuardianId.HasValue && application.LinkedGuardianId.Value > 0)
        {
            var linked = await _unitOfWork.Repository<Models.Entities.Guardian.Guardian>()
                .FirstOrDefaultAsync(g => g.Id == application.LinkedGuardianId.Value && !g.IsDeleted, ct);
            if (linked != null)
                return linked;
        }

        // 2) Find by email with NAME VERIFICATION — prevent impersonation
        if (!string.IsNullOrWhiteSpace(email))
        {
            var existing = await _unitOfWork.Repository<Models.Entities.Guardian.Guardian>()
                .FirstOrDefaultAsync(g => g.Email != null && g.Email.ToLower() == email && !g.IsDeleted, ct);
            if (existing != null)
            {
                var existingName = existing.FullName?.Trim().ToLowerInvariant() ?? "";
                var admissionName = fallbackName.ToLowerInvariant();
                if (!string.IsNullOrEmpty(admissionName) &&
                    (existingName.Contains(admissionName) || admissionName.Contains(existingName)))
                {
                    return existing;
                }
                _logger.LogWarning(
                    "Guardian email match but name mismatch for admission {AppNo}: " +
                    "found '{ExistingName}' vs expected '{AdmissionName}'. Creating new guardian.",
                    application.ApplicationNo, existingName, admissionName);
            }
        }

        // 3) No safe match — create a new guardian record
        var mobile = (application.GuardianMobileNumber ?? application.FatherOrGuardianMobileNo)?.Trim() ?? "";
        var guardianCode = await GenerateNextGuardianCodeAsync(ct);

        var guardian = new Models.Entities.Guardian.Guardian
        {
            GuardianCode = guardianCode,
            FirstName = fallbackName,
            LastName = string.Empty,
            FullName = fallbackName,
            Gender = string.Empty,
            RelationType = ResolveGuardianRelationship(application.GuardianRelationship, !string.IsNullOrWhiteSpace(name)),
            MobileNumber = mobile,
            Email = email,
            NationalId = application.GuardianNationalId?.Trim(),
            Occupation = application.GuardianOccupation?.Trim(),
            PresentAddress = application.GuardianAddress?.Trim(),
            PhotoPath = application.GuardianPhoto,
            Remarks = application.GuardianRemarks?.Trim(),
            PortalAccessEnabled = true,
            Status = string.IsNullOrWhiteSpace(email) && string.IsNullOrWhiteSpace(mobile)
                ? GuardianStatus.Inactive
                : GuardianStatus.PendingActivation
        };

        await _unitOfWork.Repository<Models.Entities.Guardian.Guardian>().AddAsync(guardian, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return guardian;
    }

    private async Task<string> GenerateNextGuardianCodeAsync(CancellationToken ct)
    {
        var lastCode = await _unitOfWork.Repository<Models.Entities.Guardian.Guardian>().Query()
            .OrderByDescending(g => g.GuardianCode)
            .Select(g => g.GuardianCode)
            .FirstOrDefaultAsync(ct);

        int nextNum = 1;
        if (lastCode != null && lastCode.StartsWith("GRD-", StringComparison.OrdinalIgnoreCase))
        {
            if (int.TryParse(lastCode.Substring(4), out int lastNum))
            {
                nextNum = lastNum + 1;
            }
        }

        return $"GRD-{nextNum:D5}";
    }

    private static GuardianRelationshipType ResolveGuardianRelationship(string? relationship, bool hasSeparateGuardian)
    {
        if (!string.IsNullOrWhiteSpace(relationship) &&
            Enum.TryParse<GuardianRelationshipType>(relationship.Replace(" ", string.Empty), true, out var parsed))
        {
            return parsed;
        }
        return hasSeparateGuardian
            ? GuardianRelationshipType.LegalGuardian
            : GuardianRelationshipType.Father;
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
}

