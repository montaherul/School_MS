using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Guardian;
using SchoolManagementSystem.Models.Entities.Admission;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Guardian;
using SchoolManagementSystem.Services.Guardian;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Guardian;

public class GuardianService : IGuardianService
{
    private readonly IUnitOfWork _uow;
    private readonly IGuardianRepository _guardianRepo;
    private readonly SchoolDbContext _db;
    private readonly IPasswordHashService _passwordHashService;
    private readonly ILogger<GuardianService> _logger;

    public GuardianService(IUnitOfWork uow, IGuardianRepository guardianRepo, SchoolDbContext db, IPasswordHashService passwordHashService, ILogger<GuardianService> logger)
    {
        _uow = uow;
        _guardianRepo = guardianRepo;
        _db = db;
        _passwordHashService = passwordHashService;
        _logger = logger;
    }

    public async Task UpdateGuardianProfileAsync(int userId, GuardianProfileUpdateDto dto, CancellationToken ct = default)
    {
        var guardian = await _guardianRepo.Query()
            .FirstOrDefaultAsync(g => g.UserId == userId && !g.IsDeleted, ct)
            ?? throw new KeyNotFoundException("Guardian profile not found.");

        if (!string.IsNullOrWhiteSpace(dto.FirstName))
        {
            guardian.FirstName = dto.FirstName.Trim();
            guardian.FullName = $"{dto.FirstName.Trim()} {dto.LastName?.Trim() ?? guardian.LastName}".Trim();
        }
        if (!string.IsNullOrWhiteSpace(dto.LastName))
        {
            guardian.LastName = dto.LastName.Trim();
            guardian.FullName = $"{guardian.FirstName} {dto.LastName.Trim()}".Trim();
        }
        if (!string.IsNullOrWhiteSpace(dto.Email)) guardian.Email = dto.Email.Trim();
        if (!string.IsNullOrWhiteSpace(dto.MobileNumber)) guardian.MobileNumber = dto.MobileNumber.Trim();
        if (!string.IsNullOrWhiteSpace(dto.NationalId)) guardian.NationalId = dto.NationalId.Trim();
        if (!string.IsNullOrWhiteSpace(dto.Occupation)) guardian.Occupation = dto.Occupation.Trim();
        if (!string.IsNullOrWhiteSpace(dto.PresentAddress)) guardian.PresentAddress = dto.PresentAddress.Trim();
        if (!string.IsNullOrWhiteSpace(dto.PermanentAddress)) guardian.PermanentAddress = dto.PermanentAddress.Trim();
        if (!string.IsNullOrWhiteSpace(dto.EmergencyContactName)) guardian.EmergencyContactName = dto.EmergencyContactName.Trim();
        if (!string.IsNullOrWhiteSpace(dto.EmergencyContactNumber)) guardian.EmergencyContactNumber = dto.EmergencyContactNumber.Trim();

        if (dto.PhotoFile != null && dto.PhotoFile.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/guardians");
            if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
            var fileName = $"guardian_{guardian.Id}_{Guid.NewGuid()}{Path.GetExtension(dto.PhotoFile.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);
            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.PhotoFile.CopyToAsync(stream, ct);
            guardian.PhotoPath = $"/uploads/guardians/{fileName}";
        }

        if (!string.IsNullOrWhiteSpace(dto.NewPassword))
        {
            if (string.IsNullOrWhiteSpace(dto.CurrentPassword))
                throw new InvalidOperationException("Current password is required to set a new password.");

            var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == userId && !u.IsDeleted, ct);
            if (user == null) throw new KeyNotFoundException("User account not found.");

            if (!_passwordHashService.VerifyPassword(dto.CurrentPassword, user.PasswordHash))
                throw new InvalidOperationException("Current password is incorrect.");

            user.PasswordHash = _passwordHashService.HashPassword(dto.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;
        }

        _guardianRepo.Update(guardian);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<(IEnumerable<GuardianListItemDto> Items, int TotalCount)> GetGuardianListAsync(string? searchTerm, string? status, int pageNumber, int pageSize)
    {
        return await _guardianRepo.GetListAsync(searchTerm, status, pageNumber, pageSize);
    }

    public async Task<GuardianDetailsDto?> GetGuardianByIdAsync(int id)
    {
        return await _guardianRepo.GetDetailsAsync(id);
    }

    public async Task<int> CreateGuardianAsync(GuardianUpsertDto dto)
    {
        string guardianCode = await GenerateGuardianCode();

        var guardian = new SchoolManagementSystem.Models.Entities.Guardian.Guardian
        {
            GuardianCode = guardianCode,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            FullName = BuildFullName(dto.FirstName, dto.LastName),
            Gender = dto.Gender,
            RelationType = dto.RelationType,
            MobileNumber = dto.MobileNumber,
            Email = dto.Email,
            NationalId = dto.NationalId,
            Occupation = dto.Occupation,
            PresentAddress = dto.PresentAddress,
            PermanentAddress = dto.PermanentAddress,
            PortalAccessEnabled = dto.PortalAccessEnabled,
            Status = GuardianStatus.Active
        };

        await _guardianRepo.AddAsync(guardian);
        await _uow.SaveChangesAsync();

        return guardian.Id;
    }

    public async Task UpdateGuardianAsync(GuardianUpsertDto dto)
    {
        var guardian = await _guardianRepo.GetByIdAsync(dto.Id);
        if (guardian == null) throw new KeyNotFoundException("Guardian not found");

        guardian.FirstName = dto.FirstName;
        guardian.LastName = dto.LastName;
        guardian.FullName = BuildFullName(dto.FirstName, dto.LastName);
        guardian.Gender = dto.Gender;
        guardian.RelationType = dto.RelationType;
        guardian.MobileNumber = dto.MobileNumber;
        guardian.Email = dto.Email;
        guardian.NationalId = dto.NationalId;
        guardian.Occupation = dto.Occupation;
        guardian.PresentAddress = dto.PresentAddress;
        guardian.PermanentAddress = dto.PermanentAddress;
        guardian.PortalAccessEnabled = dto.PortalAccessEnabled;

        _guardianRepo.Update(guardian);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteGuardianAsync(int id)
    {
        var guardian = await _guardianRepo.GetByIdAsync(id);
        if (guardian != null)
        {
            _guardianRepo.Remove(guardian);
            await _uow.SaveChangesAsync();
        }
    }

    public async Task SetGuardianStatusAsync(int id, bool active)
    {
        var guardian = await _guardianRepo.GetByIdAsync(id);
        if (guardian == null) throw new KeyNotFoundException("Guardian not found");

        guardian.Status = active ? GuardianStatus.Active : GuardianStatus.Inactive;
        guardian.PortalAccessEnabled = active;
        _guardianRepo.Update(guardian);
        await _uow.SaveChangesAsync();
    }

    public async Task LinkStudentAsync(int guardianId, int studentId, string relation)
    {
        await _guardianRepo.LinkStudentAsync(guardianId, studentId, relation);
        await _uow.SaveChangesAsync();
    }

    public async Task<GuardianDashboardDataDto> GetDashboardAsync(int guardianId)
    {
        return await _guardianRepo.GetDashboardDataAsync(guardianId);
    }

    public async Task<GuardianDashboardDataDto> GetDashboardByUserIdAsync(int userId)
    {
        var guardian = await _guardianRepo.Query()
            .FirstOrDefaultAsync(g => g.UserId == userId);

        if (guardian == null) throw new KeyNotFoundException("Guardian profile not found for this user.");

        return await GetDashboardAsync(guardian.Id);
    }

    // =====================================================================
    // PHASE 6/7: Admission-driven Guardian onboarding (auto-create or link)
    // =====================================================================
    public async Task<Models.Entities.Guardian.Guardian> EnsureGuardianFromAdmissionAsync(AdmissionApplication application, CancellationToken ct = default)
    {
        if (application == null) throw new ArgumentNullException(nameof(application));

        var email = application.GuardianEmail?.Trim().ToLowerInvariant();
        var mobile = NormalizePhone(application.GuardianMobileNumber ?? application.FatherOrGuardianMobileNo);
        var name = application.GuardianName?.Trim();
        var fallbackName = string.IsNullOrWhiteSpace(name) ? application.FatherName?.Trim() : name;
        if (string.IsNullOrWhiteSpace(fallbackName))
            throw new InvalidOperationException("Cannot create a Guardian: both GuardianName and FatherName are empty.");

        // 1) Direct link from admission form (admin picked an existing guardian)
        if (application.LinkedGuardianId.HasValue && application.LinkedGuardianId.Value > 0)
        {
            var linked = await _guardianRepo.GetByIdAsync(application.LinkedGuardianId.Value, ct);
            if (linked != null)
            {
                linked = ApplyAdmissionDetailsToGuardian(linked, application, email, mobile, fallbackName!);
                _guardianRepo.Update(linked);
                await _uow.SaveChangesAsync(ct);
                return linked;
            }
        }

        // 2) Find by email
        if (!string.IsNullOrWhiteSpace(email))
        {
            var existing = await _guardianRepo.Query()
                .FirstOrDefaultAsync(g => g.Email != null && g.Email.ToLower() == email, ct);
            if (existing != null)
            {
                existing = ApplyAdmissionDetailsToGuardian(existing, application, email, mobile, fallbackName!);
                _guardianRepo.Update(existing);
                await _uow.SaveChangesAsync(ct);
                return existing;
            }
        }

        // 3) Find by mobile
        if (!string.IsNullOrWhiteSpace(mobile))
        {
            var existing = await _guardianRepo.Query()
                .FirstOrDefaultAsync(g => g.MobileNumber == mobile, ct);
            if (existing != null)
            {
                existing = ApplyAdmissionDetailsToGuardian(existing, application, email, mobile, fallbackName!);
                _guardianRepo.Update(existing);
                await _uow.SaveChangesAsync(ct);
                return existing;
            }
        }

        // 4) Create new
        var guardian = new Models.Entities.Guardian.Guardian
        {
            GuardianCode = await GenerateGuardianCode(ct),
            FirstName = fallbackName!,
            LastName = string.Empty,
            FullName = fallbackName!,
            Gender = string.Empty,
            RelationType = ResolveRelationship(application.GuardianRelationship, hasSeparateGuardian: !string.IsNullOrWhiteSpace(name)),
            MobileNumber = mobile ?? string.Empty,
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

        await _guardianRepo.AddAsync(guardian, ct);
        await _uow.SaveChangesAsync(ct);
        return guardian;
    }

    public async Task<string> EnsureGuardianUserAsync(int guardianId, CancellationToken ct = default)
    {
        var guardian = await _guardianRepo.Query()
            .FirstOrDefaultAsync(g => g.Id == guardianId, ct)
            ?? throw new KeyNotFoundException($"Guardian {guardianId} not found");

        if (string.IsNullOrWhiteSpace(guardian.Email))
            throw new InvalidOperationException("Guardian does not have an email address \u2014 cannot create portal user.");

        // Already linked
        if (guardian.UserId.HasValue)
        {
            var existingUser = await _db.Users.FirstOrDefaultAsync(u => u.Id == guardian.UserId.Value, ct);
            if (existingUser != null) return string.Empty;
            guardian.UserId = null;
        }

        // Username: gdn-{GuardianCode} (e.g. gdn-GRD-00001) \u2014 strip dashes from code per spec
        var codeNoDashes = (guardian.GuardianCode ?? $"G{guardian.Id:D5}").Replace("-", string.Empty);
        var candidateUserName = $"gdn-{codeNoDashes}";
        while (await _db.Users.AnyAsync(u => u.UserName == candidateUserName, ct))
        {
            candidateUserName = $"{candidateUserName}_{guardian.Id}";
        }

        // Conflict: another user already has this email
        var existingByEmail = await _db.Users.FirstOrDefaultAsync(u => u.Email == guardian.Email, ct);
        if (existingByEmail != null)
        {
            guardian.UserId = existingByEmail.Id;
            guardian.Status = existingByEmail.Status == AccountStatus.Active
                ? GuardianStatus.Active
                : GuardianStatus.PendingActivation;
            _guardianRepo.Update(guardian);
            await _uow.SaveChangesAsync(ct);
            return string.Empty;
        }

        var guardianRole = await _db.Roles.FirstOrDefaultAsync(r => !r.IsDeleted && r.Name == "Guardian", ct)
            ?? throw new InvalidOperationException("Guardian role not found. Seed it first.");

        var activationToken = Guid.NewGuid().ToString("N");
        var user = new ApplicationUser
        {
            UserName = candidateUserName,
            Email = guardian.Email,
            PhoneNumber = guardian.MobileNumber,
            Status = AccountStatus.Pending,
            PasswordHash = string.Empty,
            IsEmailConfirmed = false,
            ActivationToken = activationToken,
            ActivationTokenExpiry = DateTime.UtcNow.AddHours(24),
            CreatedBy = "admission-approval",
            CreatedAt = DateTime.UtcNow
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync(ct);

        _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = guardianRole.Id });
        await _db.SaveChangesAsync(ct);

        guardian.UserId = user.Id;
        guardian.Status = GuardianStatus.PendingActivation;
        _guardianRepo.Update(guardian);
        await _uow.SaveChangesAsync(ct);

        _logger.LogInformation("Guardian user created. GuardianId={GuardianId}, UserName={UserName}", guardian.Id, candidateUserName);
        return activationToken;
    }

    // =====================================================================
    // Notification helpers
    // =====================================================================
    public async Task CreateNotificationAsync(int guardianId, string title, string message, string? category = null, CancellationToken ct = default)
    {
        _db.GuardianNotifications.Add(new GuardianNotification
        {
            GuardianId = guardianId,
            Title = title,
            Message = message,
            Category = category ?? "General",
            CreatedAt = DateTime.UtcNow
        });
        await _db.SaveChangesAsync(ct);
    }

    public async Task CreateAttendanceNotificationAsync(int studentId, string studentName, string status, DateTime date, CancellationToken ct = default)
    {
        var mappings = await _db.StudentGuardians
            .Where(sg => sg.StudentId == studentId && !sg.IsDeleted && sg.ReceivesAttendanceNotifications)
            .ToListAsync(ct);

        foreach (var sg in mappings)
        {
            _db.GuardianNotifications.Add(new GuardianNotification
            {
                GuardianId = sg.GuardianId,
                Title = $"{studentName} - {status}",
                Message = $"{studentName} was marked {status} on {date:dd MMM yyyy}.",
                Category = "Attendance",
                CreatedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task CreateFeeDueNotificationAsync(int studentId, string studentName, decimal amount, CancellationToken ct = default)
    {
        var mappings = await _db.StudentGuardians
            .Where(sg => sg.StudentId == studentId && !sg.IsDeleted && sg.ReceivesFeeNotifications)
            .ToListAsync(ct);

        foreach (var sg in mappings)
        {
            var alreadySent = await _db.GuardianNotifications.AnyAsync(n => n.GuardianId == sg.GuardianId
                && n.Category == "Fee"
                && n.Message.Contains(studentName)
                && n.CreatedAt.Date == DateTime.UtcNow.Date, ct);
            if (alreadySent) continue;

            _db.GuardianNotifications.Add(new GuardianNotification
            {
                GuardianId = sg.GuardianId,
                Title = "Fee Due Reminder",
                Message = $"Fee due for {studentName}: ৳{amount:N2} remaining.",
                Category = "Fee",
                CreatedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task CreateResultPublishedNotificationAsync(int studentId, string studentName, string examName, CancellationToken ct = default)
    {
        var mappings = await _db.StudentGuardians
            .Where(sg => sg.StudentId == studentId && !sg.IsDeleted && sg.ReceivesResultNotifications)
            .ToListAsync(ct);

        foreach (var sg in mappings)
        {
            _db.GuardianNotifications.Add(new GuardianNotification
            {
                GuardianId = sg.GuardianId,
                Title = "Result Published",
                Message = $"Result for {examName} has been published for {studentName}.",
                Category = "Result",
                CreatedAt = DateTime.UtcNow
            });
        }
        await _db.SaveChangesAsync(ct);
    }

    public async Task<bool> UserHasAccessToStudentAsync(int userId, int studentId, CancellationToken ct = default)
    {
        var guardian = await _guardianRepo.Query().AsNoTracking()
            .FirstOrDefaultAsync(g => g.UserId == userId && !g.IsDeleted, ct);
        if (guardian == null) return false;
        return await _db.Set<StudentGuardian>()
            .AsNoTracking()
            .AnyAsync(sg => sg.GuardianId == guardian.Id && sg.StudentId == studentId && !sg.IsDeleted, ct);
    }

    public async Task<List<GuardianChildCardDto>> GetChildrenByUserIdAsync(int userId, CancellationToken ct = default)
    {
        var guardian = await _guardianRepo.Query().AsNoTracking()
            .FirstOrDefaultAsync(g => g.UserId == userId && !g.IsDeleted, ct);
        if (guardian == null) return new List<GuardianChildCardDto>();

        return await _guardianRepo.GetChildrenAsync(guardian.Id, ct);
    }

    public async Task<GuardianChildDetailDto?> GetChildDetailAsync(int userId, int studentId, CancellationToken ct = default)
    {
        if (!await UserHasAccessToStudentAsync(userId, studentId, ct)) return null;

        var mapping = await _db.Set<StudentGuardian>().AsNoTracking()
            .Include(sg => sg.Student)!.ThenInclude(s => s!.Class)
            .Include(sg => sg.Student)!.ThenInclude(s => s!.Section)
            .FirstOrDefaultAsync(sg => sg.StudentId == studentId
                && sg.Guardian!.UserId == userId
                && !sg.IsDeleted
                && sg.Student != null
                && !sg.Student!.IsDeleted, ct);

        if (mapping?.Student == null) return null;
        var s = mapping.Student;

        var attendance = await _db.Attendance.AsNoTracking()
            .Where(a => a.StudentId == s.Id && !a.IsDeleted)
            .GroupBy(_ => 1)
            .Select(g => new
            {
                Total = g.Count(),
                Present = g.Count(x => x.Status == AttendanceStatus.Present),
                Absent = g.Count(x => x.Status == AttendanceStatus.Absent),
                Late = g.Count(x => x.Status == AttendanceStatus.Late),
                Leave = g.Count(x => x.Status == AttendanceStatus.Leave)
            })
            .FirstOrDefaultAsync(ct);

        var outstanding = await _db.FeeInvoices.AsNoTracking()
            .Where(fi => fi.StudentId == s.Id && (int)fi.Status != 3)
            .SumAsync(fi => (decimal?)(fi.TotalAmount - fi.PaidAmount), ct) ?? 0m;

        var latestGpa = await _db.StudentExamResults.AsNoTracking()
            .Where(r => r.StudentId == s.Id && !r.IsDeleted && (r.Status == ResultWorkflowStatus.Published || r.Status == ResultWorkflowStatus.Locked))
            .OrderByDescending(r => r.Id)
            .Select(r => (decimal?)r.Gpa)
            .FirstOrDefaultAsync(ct);

        double pct = attendance == null || attendance.Total == 0 ? 0
            : Math.Round((double)(attendance.Present + attendance.Late) / attendance.Total * 100, 2);

        return new GuardianChildDetailDto
        {
            StudentId = s.Id,
            StudentNo = s.StudentNo,
            FullName = s.FullName,
            ClassName = s.Class?.Name ?? string.Empty,
            SectionName = s.Section?.Name ?? string.Empty,
            RollNumber = s.RollNumber,
            ProfilePicturePath = s.ProfilePicturePath ?? string.Empty,
            EmailAddress = s.EmailAddress ?? string.Empty,
            MobileNumber = s.MobileNumber ?? string.Empty,
            Relationship = mapping.Relationship.ToString(),
            IsPrimaryGuardian = mapping.IsPrimaryGuardian,
            TotalAttendanceDays = attendance?.Total ?? 0,
            PresentCount = attendance?.Present ?? 0,
            AbsentCount = attendance?.Absent ?? 0,
            LateCount = attendance?.Late ?? 0,
            LeaveCount = attendance?.Leave ?? 0,
            AttendancePercentage = pct,
            OutstandingFees = outstanding,
            LatestGPA = latestGpa
        };
    }

    public async Task<List<StudentAttendanceDto>> GetChildAttendanceAsync(int userId, int studentId, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        if (!await UserHasAccessToStudentAsync(userId, studentId, ct))
            throw new UnauthorizedAccessException("You do not have access to this student's data.");

        var fromDate = (from ?? DateTime.Today.AddMonths(-1)).Date;
        var toDate = (to ?? DateTime.Today).Date;

        var student = await _db.Students.AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted, ct)
            ?? throw new KeyNotFoundException("Student not found");

        var records = await _db.Attendance.AsNoTracking()
            .Where(a => a.StudentId == studentId
                && !a.IsDeleted
                && a.AttendanceDate >= DateOnly.FromDateTime(fromDate)
                && a.AttendanceDate <= DateOnly.FromDateTime(toDate))
            .OrderByDescending(a => a.AttendanceDate)
            .ToListAsync(ct);

        return records.Select(a => new StudentAttendanceDto
        {
            Id = a.Id,
            StudentId = a.StudentId,
            StudentNo = student.StudentNo,
            StudentName = student.FullName,
            RollNumber = student.RollNumber.ToString(),
            ClassId = a.SchoolClassId,
            ClassName = student.Class?.Name ?? string.Empty,
            SectionId = a.SectionId,
            SectionName = student.Section?.Name ?? string.Empty,
            AttendanceDate = a.AttendanceDate.ToDateTime(TimeOnly.MinValue),
            Status = a.Status,
            StatusName = a.Status.ToString(),
            Remarks = a.Remarks ?? string.Empty
        }).ToList();
    }

    // =====================================================================
    // Private helpers
    // =====================================================================
    private static Models.Entities.Guardian.Guardian ApplyAdmissionDetailsToGuardian(
        Models.Entities.Guardian.Guardian guardian,
        AdmissionApplication application,
        string? email,
        string? mobile,
        string fallbackName)
    {
        if (string.IsNullOrWhiteSpace(guardian.FullName)) guardian.FullName = fallbackName;
        if (string.IsNullOrWhiteSpace(guardian.FirstName)) guardian.FirstName = fallbackName;
        if (string.IsNullOrWhiteSpace(guardian.MobileNumber) && !string.IsNullOrWhiteSpace(mobile)) guardian.MobileNumber = mobile!;
        if (string.IsNullOrWhiteSpace(guardian.Email) && !string.IsNullOrWhiteSpace(email)) guardian.Email = email!;
        if (string.IsNullOrWhiteSpace(guardian.NationalId) && !string.IsNullOrWhiteSpace(application.GuardianNationalId))
            guardian.NationalId = application.GuardianNationalId.Trim();
        if (string.IsNullOrWhiteSpace(guardian.Occupation) && !string.IsNullOrWhiteSpace(application.GuardianOccupation))
            guardian.Occupation = application.GuardianOccupation.Trim();
        if (string.IsNullOrWhiteSpace(guardian.PresentAddress) && !string.IsNullOrWhiteSpace(application.GuardianAddress))
            guardian.PresentAddress = application.GuardianAddress.Trim();
        if (string.IsNullOrWhiteSpace(guardian.PhotoPath) && !string.IsNullOrWhiteSpace(application.GuardianPhoto))
            guardian.PhotoPath = application.GuardianPhoto;
        if (string.IsNullOrWhiteSpace(guardian.Remarks) && !string.IsNullOrWhiteSpace(application.GuardianRemarks))
            guardian.Remarks = application.GuardianRemarks.Trim();
        guardian.PortalAccessEnabled = true;
        return guardian;
    }

    private static string NormalizePhone(string? phone)
    {
        if (string.IsNullOrWhiteSpace(phone)) return string.Empty;
        var trimmed = phone.Trim();
        return trimmed.Replace(" ", string.Empty).Replace("-", string.Empty);
    }

    private static GuardianRelationshipType ResolveRelationship(string? raw, bool hasSeparateGuardian)
    {
        if (!string.IsNullOrWhiteSpace(raw) &&
            Enum.TryParse<GuardianRelationshipType>(raw.Replace(" ", string.Empty), true, out var parsed))
        {
            return parsed;
        }
        return hasSeparateGuardian
            ? GuardianRelationshipType.LegalGuardian
            : GuardianRelationshipType.Father;
    }

    private async Task<string> GenerateGuardianCode(CancellationToken ct)
    {
        var lastCode = await _guardianRepo.Query()
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

    private Task<string> GenerateGuardianCode() => GenerateGuardianCode(CancellationToken.None);

    private static string BuildFullName(string firstName, string lastName)
    {
        return string.Join(' ', new[] { firstName, lastName }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
    }
}
