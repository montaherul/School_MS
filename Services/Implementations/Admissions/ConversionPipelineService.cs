using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Admission;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Guardian;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class ConversionPipelineService : IConversionPipelineService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAdmissionRepository _admissionRepository;
    private readonly IStudentService _studentService;
    private readonly IGuardianService _guardianService;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly ISchoolClassRepository _classRepository;
    private readonly IStudentRepository _studentRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly IStudentRollGenerationService _rollGenerationService;
    private readonly ISectionAllocationService _sectionAllocationService;
    private readonly IAdmissionFinanceService _admissionFinanceService;
    private readonly IWorkflowService _workflowService;
    private readonly IEmailService _emailService;
    private readonly IMemoryCache _cache;
    private readonly ILogger<ConversionPipelineService> _logger;
    private const string CacheKeySettings = "SchoolSettings";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public ConversionPipelineService(
        IUnitOfWork unitOfWork,
        IAdmissionRepository admissionRepository,
        IStudentService studentService,
        IGuardianService guardianService,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        ISchoolClassRepository classRepository,
        IStudentRepository studentRepository,
        ISectionRepository sectionRepository,
        ISchoolSettingRepository settingRepo,
        IStudentRollGenerationService rollGenerationService,
        ISectionAllocationService sectionAllocationService,
        IAdmissionFinanceService admissionFinanceService,
        IWorkflowService workflowService,
        IEmailService emailService,
        IMemoryCache cache,
        ILogger<ConversionPipelineService> logger)
    {
        _unitOfWork = unitOfWork;
        _admissionRepository = admissionRepository;
        _studentService = studentService;
        _guardianService = guardianService;
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _classRepository = classRepository;
        _studentRepository = studentRepository;
        _sectionRepository = sectionRepository;
        _settingRepo = settingRepo;
        _rollGenerationService = rollGenerationService;
        _sectionAllocationService = sectionAllocationService;
        _admissionFinanceService = admissionFinanceService;
        _workflowService = workflowService;
        _emailService = emailService;
        _cache = cache;
        _logger = logger;
    }

    public async Task<ConversionResult> ExecuteAsync(int applicationId, int sectionId, string approvedBy, CancellationToken ct = default)
    {
        var result = new ConversionResult();

        var application = await ValidateAsync(applicationId, ct);
        if (application == null)
        {
            result.Success = false;
            result.ErrorMessage = "Application not found.";
            return result;
        }

        // State captured inside transaction, needed after commit
        string? pendingUserEmail = null;
        string? pendingUserName = null;
        string? applicantName = null;
        string? applicationNo = null;
        string? activationToken = null;
        bool guardianPortalEnabled = false;
        bool guardianActivationEnabled = false;
        string? guardianActivationTokenResult = null;
        string? guardianEmailResult = null;
        string? guardianFullNameResult = null;
        string? guardianCodeResult = null;
        string? guardianName = null;
        string? fatherName = null;
        string? className = null;
        string? sectionName = null;
        int capturedStudentId = 0;
        int? capturedGuardianId = null;

        try
        {
            await _unitOfWork.ExecuteInTransactionAsync(async () =>
            {
                var settings = await GetCachedSettingsAsync(ct);
                int groupStartClass = settings.GroupStartsFromClassId;
                bool allowDirectClass10 = settings.AllowDirectAdmissionToClass10;
                guardianPortalEnabled = settings.EnableGuardianPortal;
                guardianActivationEnabled = settings.EnableGuardianActivation;

                if (application.AppliedClassId >= 10 && !allowDirectClass10)
                    throw new InvalidOperationException("Direct admission to Class 10 is not allowed. Students are promoted from Class 9.");

                var section = await _sectionRepository.Query().AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == sectionId && !x.IsDeleted, ct)
                    ?? throw new InvalidOperationException("Selected section not found.");
                var schoolClass = await _classRepository.Query().AsNoTracking()
                    .FirstOrDefaultAsync(x => x.Id == application.AppliedClassId && !x.IsDeleted, ct);

                int? studentGroupId = await ResolveGroupAsync(applicationId, sectionId, schoolClass?.SortOrder ?? 0, groupStartClass, ct);

                if (!await _sectionAllocationService.IsSectionAvailableAsync(sectionId, ct))
                    throw new InvalidOperationException("Selected section has reached its capacity.");

                int rollNumber = await GenerateRollNumberAsync(application.AppliedClassId, sectionId, ct);
                result.RollNumber = rollNumber.ToString();

                var (pendingUser, activationTokenInner) = await CreateUserAsync(applicationId, approvedBy, ct);
                result.UserId = pendingUser.Id;
                result.UserName = pendingUser.UserName;
                pendingUserEmail = pendingUser.Email;
                pendingUserName = pendingUser.UserName;
                activationToken = activationTokenInner;

                await LogWorkflowTransitionAsync(applicationId, WorkflowState.UserProvisioning, approvedBy, $"User {pendingUser.UserName} created", ct);

                int? linkedGuardianId = null;

                if (guardianPortalEnabled)
                {
                    var (guardian, activationTokenResult2) = await CreateGuardianAsync(applicationId, guardianPortalEnabled, guardianActivationEnabled, approvedBy, ct);
                    if (guardian != null)
                    {
                        linkedGuardianId = guardian.Id;
                        application.LinkedGuardianId = guardian.Id;
                        guardianEmailResult = guardian.Email;
                        guardianFullNameResult = guardian.FullName;
                        guardianCodeResult = guardian.GuardianCode;
                        guardianActivationTokenResult = activationTokenResult2;
                    }
                }
                else
                {
                    application.LinkedGuardianId = null;
                }

                result.GuardianId = linkedGuardianId;
                capturedGuardianId = linkedGuardianId;

                if (linkedGuardianId.HasValue)
                {
                    await LogWorkflowTransitionAsync(applicationId, WorkflowState.GuardianCreation, approvedBy, $"Guardian {linkedGuardianId} linked", ct);
                }

                int studentId = await CreateStudentAsync(applicationId, sectionId, studentGroupId, rollNumber, pendingUser.Id, approvedBy, ct);
                result.StudentId = studentId;
                result.StudentNo = pendingUser.UserName;
                capturedStudentId = studentId;
                className = schoolClass?.Name ?? $"Class {application.AppliedClassId}";
                sectionName = section?.Name ?? "N/A";

                await LogWorkflowTransitionAsync(applicationId, WorkflowState.StudentCreation, approvedBy, $"Student {studentId} created", ct);

                application.Status = AdmissionStatus.Converted;
                application.ReviewedAt = DateTime.UtcNow;
                if (int.TryParse(approvedBy, out var reviewerId)) application.ReviewedByUserId = reviewerId;
                application.UpdatedBy = approvedBy;
                application.UpdatedAt = DateTime.UtcNow;
                _admissionRepository.Update(application);
                await _unitOfWork.SaveChangesAsync(ct);

                await CreateFeeInvoiceAsync(applicationId, studentId, approvedBy, ct);

                await LogWorkflowTransitionAsync(applicationId, WorkflowState.AdmissionCompleted, approvedBy, $"Conversion complete: student {studentId}", ct);

                applicantName = application.ApplicantName;
                applicationNo = application.ApplicationNo;
                guardianName = application.GuardianName;
                fatherName = application.FatherName;
            }, ct);

            result.Success = true;

            if (!string.IsNullOrEmpty(pendingUserEmail))
            {
                try
                {
                    await _emailService.SendStudentActivationAsync(
                        pendingUserEmail, applicantName ?? "", pendingUserName ?? "", activationToken ?? "", ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Student activation email failed for {AppNo}", applicationNo);
                }
            }

            if (guardianPortalEnabled && guardianActivationEnabled && !string.IsNullOrEmpty(guardianActivationTokenResult) && !string.IsNullOrWhiteSpace(guardianEmailResult))
            {
                try
                {
                    await _emailService.SendGuardianActivationAsync(
                        guardianEmailResult,
                        guardianFullNameResult ?? guardianName ?? fatherName ?? "Guardian",
                        $"gdn-{(guardianCodeResult ?? "guardian").Replace("-", string.Empty)}",
                        guardianActivationTokenResult,
                        string.Empty,
                        ct);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Guardian activation email failed for admission {AppNo}", applicationNo);
                }
            }

            // Send Welcome email (fire-and-forget)
            if (!string.IsNullOrEmpty(pendingUserEmail) && capturedStudentId > 0)
            {
                try
                {
                    await _emailService.SendWelcomeEmailAsync(
                        pendingUserEmail,
                        applicantName ?? "Student",
                        pendingUserName ?? "",
                        capturedStudentId,
                        className ?? "N/A",
                        sectionName ?? "N/A",
                        "",
                        ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Welcome email failed for {AppNo}", applicationNo);
                }
            }

            // Create Guardian notification (fire-and-forget)
            if (capturedGuardianId.HasValue && capturedGuardianId.Value > 0)
            {
                try
                {
                    await _guardianService.CreateNotificationAsync(
                        capturedGuardianId.Value,
                        "Admission Approved",
                        $"Your child {applicantName ?? "Student"} has been admitted to {className ?? "N/A"} - {sectionName ?? "N/A"}. Student ID: {pendingUserName ?? "N/A"}",
                        "Admission",
                        ct);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Guardian notification failed for admission {AppNo}", applicationNo);
                }
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.ErrorMessage = ex.Message;
        }

        return result;
    }

    public async Task<AdmissionApplication?> ValidateAsync(int applicationId, CancellationToken ct = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct);
        if (application == null)
            throw new InvalidOperationException("Admission application not found.");
        if (application.Status == AdmissionStatus.Converted)
            throw new InvalidOperationException("Application has already been converted.");
        if (application.Status == AdmissionStatus.Rejected)
            throw new InvalidOperationException("Cannot convert a rejected application.");

        var currentStatus = await _admissionRepository.Query().AsNoTracking()
            .Where(x => x.Id == applicationId)
            .Select(x => x.Status)
            .FirstOrDefaultAsync(ct);
        if (currentStatus == AdmissionStatus.Converted)
            throw new InvalidOperationException("Application was already converted by another admin.");
        if (currentStatus == AdmissionStatus.Rejected)
            throw new InvalidOperationException("Cannot convert a rejected application.");

        if (!application.AdmissionFeePaid)
            throw new InvalidOperationException($"Admission fee (BDT {application.AdmissionFee:N2}) must be paid before conversion.");

        return application;
    }

    public async Task<(ApplicationUser user, string activationToken)> CreateUserAsync(int applicationId, string approvedBy, CancellationToken ct = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Application not found.");

        var studentEmail = application.ApplicantEmail?.Trim();
        if (string.IsNullOrWhiteSpace(studentEmail))
        {
            var sanitizedName = application.ApplicantName.Replace(" ", ".").ToLower();
            studentEmail = $"{sanitizedName}.{application.Id}@school.com";
        }

        if (await _userRepository.AnyAsync(u => u.Email == studentEmail, ct))
            throw new InvalidOperationException($"A user account already exists for '{studentEmail}'.");

        var studentRole = await _roleRepository.Query().AsNoTracking()
            .FirstOrDefaultAsync(r => !r.IsDeleted && r.Name == "Student", ct)
            ?? throw new InvalidOperationException("Student role not found.");

        var year = DateTime.UtcNow.Year;
        var lastUserName = await _userRepository.Query().AsNoTracking()
            .Where(u => u.UserName.StartsWith($"STU-{year}"))
            .OrderByDescending(u => u.UserName)
            .Select(u => u.UserName)
            .FirstOrDefaultAsync(ct);

        var nextSeq = 1;
        if (!string.IsNullOrEmpty(lastUserName) && int.TryParse(lastUserName.AsSpan(8), out var lastSeq))
            nextSeq = lastSeq + 1;

        var candidateUserName = $"STU-{year}{nextSeq:D3}";

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

        await _userRepository.AddAsync(pendingUser, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        await _userRoleRepository.AddAsync(new UserRole { UserId = pendingUser.Id, RoleId = studentRole.Id }, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        return (pendingUser, activationToken);
    }

    public async Task<int> GenerateRollNumberAsync(int classId, int sectionId, CancellationToken ct = default)
    {
        return await _rollGenerationService.GenerateNextRollAsync(classId, sectionId, ct);
    }

    public async Task<int?> ResolveGroupAsync(int applicationId, int sectionId, int classSortOrder, int groupStartClass, CancellationToken ct = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Application not found.");

        var section = await _sectionRepository.Query().AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == sectionId && !x.IsDeleted, ct);

        bool classRequiresGroup = classSortOrder >= groupStartClass;

        int? studentGroupId = section?.StudentGroupId ?? application.AppliedStudentGroupId;

        if (classRequiresGroup && !studentGroupId.HasValue)
        {
            var matchingGroups = await _unitOfWork.Repository<StudentGroup>().Query()
                .Where(g => g.IsActive && !g.IsDeleted
                    && g.MinClass <= classSortOrder
                    && g.MaxClass >= classSortOrder)
                .ToListAsync(ct);
            if (matchingGroups.Count == 1)
                studentGroupId = matchingGroups[0].Id;
        }

        if (classRequiresGroup && !studentGroupId.HasValue)
            throw new InvalidOperationException("An academic group (Science, Humanities, Business Studies) is required for this class. Please select a group before approving.");

        if (!classRequiresGroup)
            studentGroupId = null;

        return studentGroupId;
    }

    public async Task<(Models.Entities.Guardian.Guardian? guardian, string? activationToken)> CreateGuardianAsync(
        int applicationId, bool portalEnabled, bool activationEnabled, string approvedBy, CancellationToken ct = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Application not found.");

        if (!portalEnabled)
            return (null, null);

        var guardian = await EnsureGuardianFromAdmissionSafeAsync(application, ct);

        string? activationToken = null;
        if (activationEnabled)
        {
            try
            {
                activationToken = await _guardianService.EnsureGuardianUserAsync(guardian.Id, ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Guardian user provisioning failed for admission {AppNo}", application.ApplicationNo);
            }
        }

        return (guardian, activationToken);
    }

    public async Task<int> CreateStudentAsync(int applicationId, int sectionId, int? groupId, int rollNumber, int userId, string approvedBy, CancellationToken ct = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct)
            ?? throw new InvalidOperationException("Application not found.");

        var settings = await GetCachedSettingsAsync(ct);
        bool guardianPortalEnabled = settings.EnableGuardianPortal;

        var studentDto = new StudentUpsertDto
        {
            StudentNo = null,
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
            FatherOrGuardianMobileNo = guardianPortalEnabled ? application.FatherOrGuardianMobileNo : null,
            EmailAddress = application.ApplicantEmail ?? string.Empty,
            Nationality = application.Nationality,
            Country = application.Country,
            MaritalStatus = application.MaritalStatus,
            Religion = application.Religion,
            BloodGroup = application.BloodGroup,
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
            StudentGroupId = groupId,
            RollNumber = rollNumber,
            UserId = userId,
            LinkedGuardianId = application.LinkedGuardianId
        };

        return await _studentService.CreateAsync(studentDto, approvedBy, ct);
    }

    public async Task CreateFeeInvoiceAsync(int applicationId, int studentId, string approvedBy, CancellationToken ct = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct);
        if (application == null) return;

        var feeStructure = await _unitOfWork.Repository<AdmissionFeeStructure>().Query().AsNoTracking()
            .FirstOrDefaultAsync(f => f.SchoolClassId == application.AppliedClassId && f.IsActive && !f.IsDeleted, ct);

        var admissionFee = feeStructure?.AdmissionFee ?? application.AdmissionFee;
        var className = feeStructure?.ClassName ?? $"Class-{application.AppliedClassId}";

        await _admissionFinanceService.CreateAdmissionInvoiceAsync(
            applicationId, studentId, admissionFee,
            application.AdmissionFeePaid, className,
            application.PaymentMethod, application.TransactionDetails,
            approvedBy, ct);
    }

    public async Task SendEmailsAsync(int applicationId, int userId, bool portalEnabled, bool activationEnabled,
        string? guardianActivationToken, string? guardianEmail, string? guardianFullName, string? guardianCode, CancellationToken ct = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, ct);
        if (application == null) return;

        var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == userId, ct);
        if (user == null) return;

        if (!string.IsNullOrEmpty(user.Email))
        {
            try
            {
                await _emailService.SendStudentActivationAsync(
                    user.Email, application.ApplicantName, user.UserName, user.ActivationToken ?? string.Empty, ct);
            }
            catch (Exception ex)
            {
                _logger.LogWarning(ex, "Student activation email failed for {AppNo}", application.ApplicationNo);
            }
        }

        if (portalEnabled && activationEnabled && !string.IsNullOrEmpty(guardianActivationToken) && !string.IsNullOrWhiteSpace(guardianEmail))
        {
            try
            {
                await _emailService.SendGuardianActivationAsync(
                    guardianEmail,
                    guardianFullName ?? application.GuardianName ?? application.FatherName ?? "Guardian",
                    $"gdn-{(guardianCode ?? "guardian").Replace("-", string.Empty)}",
                    guardianActivationToken,
                    string.Empty,
                    ct);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Guardian activation email failed for admission {AppNo}", application.ApplicationNo);
            }
        }
    }

    private async Task<SchoolSetting> GetCachedSettingsAsync(CancellationToken ct)
    {
        if (_cache.TryGetValue(CacheKeySettings, out SchoolSetting? cached) && cached != null)
            return cached;

        var settings = await _settingRepo.GetCurrentSettingsAsync(ct)
            ?? throw new InvalidOperationException("School settings not configured.");

        _cache.Set(CacheKeySettings, settings, CacheDuration);
        return settings;
    }

    private async Task LogWorkflowTransitionAsync(int applicationId, WorkflowState state, string actionedBy, string? remarks, CancellationToken ct)
    {
        try
        {
            await _workflowService.LogPipelineStepAsync(applicationId, state.ToString(), actionedBy, remarks, ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Workflow pipeline step {State} failed for application {AppId}", state, applicationId);
        }
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
                ? Models.Entities.Guardian.GuardianStatus.Inactive
                : Models.Entities.Guardian.GuardianStatus.PendingActivation
        };

        await _unitOfWork.Repository<Models.Entities.Guardian.Guardian>().AddAsync(guardian, ct);
        await _unitOfWork.SaveChangesAsync(ct);
        return guardian;
    }

    private async Task<string> GenerateNextGuardianCodeAsync(CancellationToken ct)
    {
        var lastCode = await _unitOfWork.Repository<Models.Entities.Guardian.Guardian>().Query()
            .Where(g => g.GuardianCode.StartsWith("GRD-"))
            .OrderByDescending(g => g.GuardianCode.Length)
            .ThenByDescending(g => g.GuardianCode)
            .Select(g => g.GuardianCode)
            .FirstOrDefaultAsync(ct);

        int nextNum = 1;
        if (lastCode != null && lastCode.Length > 4 && int.TryParse(lastCode.AsSpan(4), out int lastNum))
            nextNum = lastNum + 1;

        return $"GRD-{nextNum:D5}";
    }

    private static Models.Entities.Guardian.GuardianRelationshipType ResolveGuardianRelationship(string? relationship, bool hasSeparateGuardian)
    {
        if (!string.IsNullOrWhiteSpace(relationship) &&
            Enum.TryParse<Models.Entities.Guardian.GuardianRelationshipType>(relationship.Replace(" ", string.Empty), true, out var parsed))
        {
            return parsed;
        }
        return hasSeparateGuardian
            ? Models.Entities.Guardian.GuardianRelationshipType.LegalGuardian
            : Models.Entities.Guardian.GuardianRelationshipType.Father;
    }
}
