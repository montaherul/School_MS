using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Caching.Memory;
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
using SchoolManagementSystem.Helpers.Email;
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
using SchoolManagementSystem.Services.Interfaces.Admissions;
using SchoolManagementSystem.Models.DTOs.Admission;
using SchoolManagementSystem.Models.Entities.Admission;
using System.Data;

namespace SchoolManagementSystem.Services.Implementations.Admissions;

public class AdmissionService : IAdmissionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IAdmissionRepository _admissionRepository;
    private readonly IStudentService _studentService;
    private readonly IEmailService _emailService;
    private readonly IEmailSender _emailSender;
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
    private readonly IWorkflowService _workflowService;
    private readonly IAdmissionFinanceService _admissionFinanceService;
    private readonly IConversionPipelineService _conversionPipeline;
    private readonly IMemoryCache _cache;
    private const string CacheKeyClasses = "AdmissionClasses";
    private const string CacheKeyGroups = "AdmissionGroups";
    private static readonly TimeSpan CacheDuration = TimeSpan.FromMinutes(5);

    public AdmissionService(
        IUnitOfWork unitOfWork,
        IAdmissionRepository admissionRepository,
        IStudentService studentService,
        IEmailService emailService,
        IEmailSender emailSender,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        ISchoolClassRepository classRepository,
        IStudentRepository studentRepository,
        ISectionRepository sectionRepository,
        IGuardianService guardianService,
        ISchoolSettingRepository settingRepo,
        ILogger<AdmissionService> logger,
        IHttpContextAccessor httpContextAccessor,
        IWorkflowService workflowService,
        IAdmissionFinanceService admissionFinanceService,
        IConversionPipelineService conversionPipeline,
        IMemoryCache cache)
    {
        _unitOfWork = unitOfWork;
        _admissionRepository = admissionRepository;
        _studentService = studentService;
        _emailService = emailService;
        _emailSender = emailSender;
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
        _workflowService = workflowService;
        _admissionFinanceService = admissionFinanceService;
        _conversionPipeline = conversionPipeline;
        _cache = cache;
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
        const int maxRetries = 3;
        for (int attempt = 1; attempt <= maxRetries; attempt++)
        {
            await _unitOfWork.BeginTransactionAsync(cancellationToken);
            try
            {
                var year = DateTime.UtcNow.Year;
                var prefix = $"APP-{year}-";
                var maxAppNo = await _admissionRepository.Query().AsNoTracking()
                    .Where(x => x.ApplicationNo.StartsWith(prefix) && !x.IsDeleted)
                    .OrderByDescending(x => x.ApplicationNo.Length)
                    .ThenByDescending(x => x.ApplicationNo)
                    .Select(x => x.ApplicationNo)
                    .FirstOrDefaultAsync(cancellationToken);

                var nextSeq = 1;
                if (!string.IsNullOrEmpty(maxAppNo))
                {
                    var dash = maxAppNo.LastIndexOf('-');
                    if (dash >= 0 && int.TryParse(maxAppNo.AsSpan(dash + 1), out var lastSeq))
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

            try { await _workflowService.InitializeWorkflowAsync(application.Id, cancellationToken); }
            catch (Exception ex) { _logger.LogWarning(ex, "Workflow initialization failed for application {AppNo}", applicationNo); }

            await LogAuditAsync("Admission", "Admission.Apply", application.Id.ToString(), $"Application submitted: {application.ApplicantName} ({application.ApplicationNo})", cancellationToken);

            if (!string.IsNullOrWhiteSpace(application.ApplicantEmail))
            {
                try { await _emailService.SendAdmissionReceivedAsync(application.ApplicantEmail, application.ApplicantName, application.ApplicationNo, cancellationToken); }
                catch (Exception ex) { _logger.LogError(ex, "Admission confirmation email failed for application {ApplicationNo}", application.ApplicationNo); }
            }

            return application.ApplicationNo;
        }
        catch (DbUpdateException ex) when (attempt < maxRetries && ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2601 || sqlEx.Number == 2627))
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            continue;
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    throw new InvalidOperationException("Failed to generate unique application number after multiple retries.");
    }

    public async Task<int> ApproveAndConvertAsync(int applicationId, int sectionId, string approvedBy, CancellationToken cancellationToken = default)
    {
        var pipelineResult = await _conversionPipeline.ExecuteAsync(applicationId, sectionId, approvedBy, cancellationToken);

        if (!pipelineResult.Success)
            throw new InvalidOperationException(pipelineResult.ErrorMessage ?? "Conversion failed.");

        await LogAuditAsync("Admission", "Admission.Approve", applicationId.ToString(),
            $"Application approved & converted to student: {pipelineResult.StudentNo} ({pipelineResult.StudentId})", cancellationToken);

        return pipelineResult.StudentId ?? throw new InvalidOperationException("Student ID not returned from pipeline.");
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

        _admissionRepository.Update(application);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await LogAuditAsync("Admission", "Admission.Update", id.ToString(), $"Admission updated: {application.ApplicationNo} ({application.ApplicantName})", cancellationToken);
    }

    public async Task RejectAsync(int applicationId, string rejectedBy, string? rejectionReason = null, CancellationToken cancellationToken = default)
    {
        var application = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == applicationId && !x.IsDeleted, cancellationToken)
            ?? throw new InvalidOperationException("Admission application not found.");

        if (application.Status == AdmissionStatus.Converted)
            throw new InvalidOperationException("Cannot reject a converted application.");
        if (application.Status == AdmissionStatus.Rejected)
            throw new InvalidOperationException("Application has already been rejected.");

        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            application.Status = AdmissionStatus.Rejected;
            application.ReviewedAt = DateTime.UtcNow;
            if (int.TryParse(rejectedBy, out var reviewerId)) application.ReviewedByUserId = reviewerId;
            application.UpdatedBy = rejectedBy;
            application.UpdatedAt = DateTime.UtcNow;
            _admissionRepository.Update(application);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            await LogAuditAsync("Admission", "Admission.Reject", applicationId.ToString(), $"Application rejected: {application.ApplicantName} ({application.ApplicationNo})", cancellationToken);

            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }

        if (!string.IsNullOrWhiteSpace(application.ApplicantEmail))
        {
            try
            {
                var subject = $"Admission Application {application.ApplicationNo} - Status Update";
                var htmlBody = $@"<h2>Admission Application Status</h2>
<p>Dear {application.ApplicantName},</p>
<p>Thank you for your interest in our school.</p>
<p>After careful review, we regret to inform you that your admission application ({application.ApplicationNo}) has been <strong>rejected</strong>.</p>
<p>Reason: {rejectionReason ?? "Not specified"}</p>
<p>We encourage you to apply again in the future.</p>
<p>Regards,<br/>Admission Office</p>";
                await _emailSender.SendAsync(application.ApplicantEmail, subject, htmlBody, cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Rejection email failed for application {ApplicationNo}", application.ApplicationNo);
            }
        }
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

        await _unitOfWork.BeginTransactionAsync(ct);
        try
        {
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
            _admissionRepository.Update(application);
            await _unitOfWork.SaveChangesAsync(ct);

            await LogAuditAsync("Admission", "Admission.Delete", id.ToString(), $"Admission deleted: {application.ApplicationNo} ({application.ApplicantName})", ct);

            await _unitOfWork.CommitTransactionAsync(ct);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(ct);
            throw;
        }
    }

    public async Task<IEnumerable<dynamic>> GetAvailableClassesAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CacheKeyClasses, out IEnumerable<dynamic>? cached) && cached != null)
            return cached;

        var classes = await _classRepository.Query().AsNoTracking()
            .Where(c => !c.IsDeleted)
            .Select(c => new { c.Id, c.Name, c.IsGroupBased, c.SortOrder })
            .ToListAsync(ct);

        _cache.Set(CacheKeyClasses, classes, CacheDuration);
        return classes;
    }

    public async Task<WorkflowInstance> InitializeWorkflowAsync(int applicationId, CancellationToken ct = default)
    {
        return await _workflowService.InitializeWorkflowAsync(applicationId, ct);
    }

    public async Task<AdmissionTimelineDto> GetTimelineAsync(int applicationId, CancellationToken ct = default)
    {
        return await _workflowService.GetTimelineAsync(applicationId, ct);
    }

    public async Task<IEnumerable<dynamic>> GetActiveStudentGroupsAsync(CancellationToken ct = default)
    {
        if (_cache.TryGetValue(CacheKeyGroups, out IEnumerable<dynamic>? cached) && cached != null)
            return cached;

        var groups = await _unitOfWork.Repository<StudentGroup>().Query().AsNoTracking()
            .Where(g => g.IsActive && !g.IsDeleted)
            .Select(g => new { g.Id, g.Name, g.MinClass, g.MaxClass })
            .ToListAsync(ct);

        _cache.Set(CacheKeyGroups, groups, CacheDuration);
        return groups;
    }

    public async Task<BulkOperationProgress> BulkApproveAsync(List<int> ids, int sectionId, string approvedBy, CancellationToken ct = default)
    {
        var progress = new BulkOperationProgress { Total = ids.Count };
        foreach (var id in ids)
        {
            progress.CurrentItem = $"Processing ID: {id}";
            try
            {
                await ApproveAndConvertAsync(id, sectionId, approvedBy, ct);
                progress.Succeeded++;
            }
            catch (Exception ex)
            {
                progress.Failed++;
                progress.Errors.Add($"ID {id}: {ex.Message}");
            }
            progress.Processed++;
        }
        progress.IsCompleted = true;
        return progress;
    }

    public async Task<BulkOperationProgress> BulkRejectAsync(List<int> ids, string rejectedBy, string? reason = null, CancellationToken ct = default)
    {
        var progress = new BulkOperationProgress { Total = ids.Count };
        foreach (var id in ids)
        {
            progress.CurrentItem = $"Processing ID: {id}";
            try
            {
                await RejectAsync(id, rejectedBy, reason, ct);
                progress.Succeeded++;
            }
            catch (Exception ex)
            {
                progress.Failed++;
                progress.Errors.Add($"ID {id}: {ex.Message}");
            }
            progress.Processed++;
        }
        progress.IsCompleted = true;
        return progress;
    }

    public async Task<BulkOperationProgress> BulkDeleteAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        var progress = new BulkOperationProgress { Total = ids.Count };
        foreach (var id in ids)
        {
            progress.CurrentItem = $"Processing ID: {id}";
            try
            {
                await DeleteAsync(id, updatedBy, ct);
                progress.Succeeded++;
            }
            catch (Exception ex)
            {
                progress.Failed++;
                progress.Errors.Add($"ID {id}: {ex.Message}");
            }
            progress.Processed++;
        }
        progress.IsCompleted = true;
        return progress;
    }

    public async Task<BulkOperationProgress> BulkRestoreAsync(List<int> ids, string updatedBy, CancellationToken ct = default)
    {
        var progress = new BulkOperationProgress { Total = ids.Count };
        foreach (var id in ids)
        {
            progress.CurrentItem = $"Processing ID: {id}";
            try
            {
                var app = await _admissionRepository.FirstOrDefaultAsync(x => x.Id == id && x.IsDeleted, ct);
                if (app != null)
                {
                    app.IsDeleted = false;
                    app.UpdatedBy = updatedBy;
                    app.UpdatedAt = DateTime.UtcNow;
                    _admissionRepository.Update(app);
                    await _unitOfWork.SaveChangesAsync(ct);
                    progress.Succeeded++;
                }
            }
            catch (Exception ex)
            {
                progress.Failed++;
                progress.Errors.Add($"ID {id}: {ex.Message}");
            }
            progress.Processed++;
        }
        progress.IsCompleted = true;
        return progress;
    }

    public async Task<BulkOperationProgress> BulkExportAsync(List<int> ids, CancellationToken ct = default)
    {
        var progress = new BulkOperationProgress { Total = ids.Count };
        foreach (var id in ids)
        {
            progress.CurrentItem = $"Exporting ID: {id}";
            try
            {
                var app = await GetByIdAsync(id, ct);
                if (app != null) progress.Succeeded++;
            }
            catch (Exception ex)
            {
                progress.Failed++;
                progress.Errors.Add($"ID {id}: {ex.Message}");
            }
            progress.Processed++;
        }
        progress.IsCompleted = true;
        return progress;
    }

    public async Task<byte[]> BulkExportExcelAsync(List<int>? ids = null, CancellationToken ct = default)
    {
        var query = _admissionRepository.Query().AsNoTracking().Where(a => !a.IsDeleted);
        if (ids != null && ids.Any())
            query = query.Where(a => ids.Contains(a.Id));

        var apps = await query.OrderByDescending(a => a.CreatedAt).ToListAsync(ct);

        using var workbook = new ClosedXML.Excel.XLWorkbook();
        var ws = workbook.Worksheets.Add("Admissions");

        // Headers
        ws.Cell(1, 1).Value = "Application No";
        ws.Cell(1, 2).Value = "Applicant Name";
        ws.Cell(1, 3).Value = "Name (Bangla)";
        ws.Cell(1, 4).Value = "Date of Birth";
        ws.Cell(1, 5).Value = "Gender";
        ws.Cell(1, 6).Value = "Father Name";
        ws.Cell(1, 7).Value = "Mother Name";
        ws.Cell(1, 8).Value = "Mobile";
        ws.Cell(1, 9).Value = "Email";
        ws.Cell(1, 10).Value = "Religion";
        ws.Cell(1, 11).Value = "Status";
        ws.Cell(1, 12).Value = "Applied Class";
        ws.Cell(1, 13).Value = "Submitted At";

        var headerRange = ws.Range(1, 1, 1, 13);
        headerRange.Style.Font.Bold = true;
        headerRange.Style.Fill.BackgroundColor = ClosedXML.Excel.XLColor.FromArgb(15, 118, 110);
        headerRange.Style.Font.FontColor = ClosedXML.Excel.XLColor.White;

        int row = 2;
        foreach (var app in apps)
        {
            ws.Cell(row, 1).Value = app.ApplicationNo;
            ws.Cell(row, 2).Value = app.ApplicantName;
            ws.Cell(row, 3).Value = app.ApplicantNameBangla ?? string.Empty;
            ws.Cell(row, 4).Value = app.DateOfBirth.ToString("dd-MMM-yyyy");
            ws.Cell(row, 5).Value = app.Gender;
            ws.Cell(row, 6).Value = app.FatherName;
            ws.Cell(row, 7).Value = app.MotherName;
            ws.Cell(row, 8).Value = app.ApplicantMobileNumber ?? string.Empty;
            ws.Cell(row, 9).Value = app.ApplicantEmail ?? string.Empty;
            ws.Cell(row, 10).Value = app.Religion;
            ws.Cell(row, 11).Value = app.Status.ToString();
            ws.Cell(row, 12).Value = app.AppliedClassId.ToString();
            ws.Cell(row, 13).Value = app.CreatedAt.ToString("dd-MMM-yyyy HH:mm");
            row++;
        }

        ws.Columns().AdjustToContents();

        using var ms = new MemoryStream();
        workbook.SaveAs(ms);
        return ms.ToArray();
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
        try
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
                Details = (details ?? string.Empty).Length > 1000 ? (details ?? string.Empty)[..1000] : details ?? string.Empty,
                CreatedBy = httpContext?.User?.Identity?.Name ?? "system",
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.Repository<AuditLog>().AddAsync(log, ct);
            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to log audit: {Module}/{Action} for {EntityId}", module, action, entityId);
        }
    }
}

