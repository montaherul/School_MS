using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.Website;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Data;

namespace SchoolManagementSystem.Services.Implementations.Students;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentRepository _studentRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ISchoolClassRepository _classRepository;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public StudentService(
        IUnitOfWork unitOfWork, 
        IStudentRepository studentRepository,
        ISectionRepository sectionRepository,
        ISchoolClassRepository classRepository,
        ISchoolSettingRepository settingRepo,
        IHttpContextAccessor httpContextAccessor) 
    { 
        _unitOfWork = unitOfWork;
        _studentRepository = studentRepository;
        _sectionRepository = sectionRepository;
        _classRepository = classRepository;
        _settingRepo = settingRepo;
        _httpContextAccessor = httpContextAccessor;
    }

    public async Task<int> CreateAsync(StudentUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        var section = await _sectionRepository.GetByIdAsync(dto.SectionId, cancellationToken);
        if (section != null)
        {
            var currentCount = await _studentRepository.CountAsync(s => s.SectionId == dto.SectionId && !s.IsDeleted && s.Status == StudentStatus.Active, cancellationToken);
            if (currentCount >= section.Capacity)
                throw new InvalidOperationException($"Cannot assign student. Section '{section.Name}' capacity ({section.Capacity}) reached.");
        }

        var groupId = dto.StudentGroupId ?? section?.StudentGroupId;

        // Validate group against GroupStartsFromClassId setting
        var schoolClassForGroup = await _classRepository.GetByIdAsync(dto.ClassId, cancellationToken);
        if (schoolClassForGroup != null)
        {
            var settings = await _settingRepo.GetCurrentSettingsAsync(cancellationToken);
            if (settings != null)
            {
                bool classRequiresGroup = schoolClassForGroup.SortOrder >= settings.GroupStartsFromClassId;
                if (classRequiresGroup && !groupId.HasValue)
                    throw new InvalidOperationException("An academic group is required for the selected class.");
                if (!classRequiresGroup)
                    groupId = null;
            }
        }

        // Fallback: if no group set and exactly one StudentGroup matches this class, auto-assign
        if (!groupId.HasValue)
        {
            var schoolClass = await _classRepository.GetByIdAsync(dto.ClassId, cancellationToken);
            if (schoolClass != null)
            {
                var matchingGroups = await _unitOfWork.Repository<StudentGroup>().Query()
                    .Where(g => g.IsActive && !g.IsDeleted
                        && g.MinClass <= schoolClass.SortOrder
                        && g.MaxClass >= schoolClass.SortOrder)
                    .ToListAsync(cancellationToken);
                if (matchingGroups.Count == 1)
                    groupId = matchingGroups[0].Id;
            }
        }

        var student = new Student
        {
            StudentNo = !string.IsNullOrWhiteSpace(dto.StudentNo) ? dto.StudentNo : await GenerateStudentNoAsync(cancellationToken),
            FullName = dto.FullName,
            FullNameBangla = dto.FullNameBangla,
            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,
            FatherName = dto.FatherName,
            FatherOccupation = dto.FatherOccupation,
            MotherName = dto.MotherName,
            MotherOccupation = dto.MotherOccupation,
            MobileNumber = dto.MobileNumber,
            AlternativeNumber = dto.AlternativeNumber,
            EmailAddress = dto.EmailAddress,
            Nationality = dto.Nationality,
            Country = dto.Country,
            MaritalStatus = dto.MaritalStatus,
            Religion = dto.Religion,
            BloodGroup = dto.BloodGroup,
            PresentVillage = dto.PresentVillage,
            PresentPostOffice = dto.PresentPostOffice,
            PresentThana = dto.PresentThana,
            PresentDistrict = dto.PresentDistrict,
            PermanentVillage = dto.PermanentVillage,
            PermanentPostOffice = dto.PermanentPostOffice,
            PermanentThana = dto.PermanentThana,
            PermanentDistrict = dto.PermanentDistrict,
            BirthCertificateNo = dto.BirthCertificateNo,
            ProfilePicturePath = dto.ProfilePicturePath,
            ClassId = dto.ClassId,
            SectionId = dto.SectionId,
            StudentGroupId = groupId,
            RollNumber = dto.RollNumber,
            UserId = dto.UserId,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        student.OptionalSubjectId = dto.OptionalSubjectId;
        student.AssignedReligionSubjectId = await GetReligionSubjectIdAsync(dto.Religion, cancellationToken);

        if (dto.LinkedGuardianId.HasValue && dto.LinkedGuardianId > 0)
        {

        student.StudentGuardians.Add(new SchoolManagementSystem.Models.Entities.Guardian.StudentGuardian
            {
                GuardianId = dto.LinkedGuardianId.Value,
                Relationship = !string.IsNullOrWhiteSpace(dto.GuardianName) 
                    ? SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.LegalGuardian 
                    : SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.Father,
                IsPrimaryGuardian = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            });
        }
        else if (!string.IsNullOrWhiteSpace(dto.FatherOrGuardianMobileNo))
        {
            var isGuardian = !string.IsNullOrWhiteSpace(dto.GuardianName);
            var name = isGuardian ? dto.GuardianName?.Trim() : dto.FatherName?.Trim();
            if (string.IsNullOrWhiteSpace(name))
                name = isGuardian ? "Guardian" : "Father";
            student.StudentGuardians.Add(new SchoolManagementSystem.Models.Entities.Guardian.StudentGuardian
            {
                Guardian = new SchoolManagementSystem.Models.Entities.Guardian.Guardian
                {
                    FirstName = name,
                    LastName = "",
                    FullName = name,
                    MobileNumber = dto.FatherOrGuardianMobileNo.Trim(),
                    Occupation = dto.GuardianOccupation?.Trim(),
                    RelationType = isGuardian ? SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.LegalGuardian : SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.Father,
                    Status = SchoolManagementSystem.Models.Entities.Guardian.GuardianStatus.Active,
                    CreatedAt = DateTime.UtcNow,
                    CreatedBy = createdBy
                },
                Relationship = isGuardian 
                    ? SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.LegalGuardian 
                    : SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.Father,
                IsPrimaryGuardian = true,
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            });
        }

        await _studentRepository.AddAsync(student, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        if (groupId.HasValue)
        {
            var activeYear = await _unitOfWork.Repository<AcademicYear>().FirstOrDefaultAsync(x => x.IsActive, cancellationToken);
            if (activeYear != null)
            {
                var assignment = new StudentGroupAssignment
                {
                    StudentId = student.Id,
                    StudentGroupId = groupId.Value,
                    SchoolClassId = dto.ClassId,
                    AcademicYearId = activeYear.Id,
                    AssignedDate = DateTime.UtcNow
                };
                var assignmentRepo = _unitOfWork.Repository<StudentGroupAssignment>();
                await assignmentRepo.AddAsync(assignment, cancellationToken);
                await _unitOfWork.SaveChangesAsync(cancellationToken);
            }
        }

        await LogAuditAsync("Student", "Student.Create", student.Id.ToString(), $"Created student: {student.FullName} ({student.StudentNo})", cancellationToken);

        return student.Id;
    }

    public async Task UpdateAsync(StudentUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.Query()
            .Include(s => s.StudentGuardians)
                .ThenInclude(sg => sg.Guardian)
            .FirstOrDefaultAsync(s => s.Id == dto.Id && !s.IsDeleted, cancellationToken)
            ?? throw new Exception("Student not found");

        if (student.SectionId != dto.SectionId)
        {
            var section = await _sectionRepository.GetByIdAsync(dto.SectionId, cancellationToken);
            if (section != null)
            {
                var currentCount = await _studentRepository.CountAsync(s => s.SectionId == dto.SectionId && !s.IsDeleted && s.Status == StudentStatus.Active, cancellationToken);
                if (currentCount >= section.Capacity)
                    throw new InvalidOperationException($"Cannot assign student. Section '{section.Name}' capacity ({section.Capacity}) reached.");
            }
        }

        student.FullName = dto.FullName;
        student.FullNameBangla = dto.FullNameBangla;
        student.DateOfBirth = dto.DateOfBirth;
        student.Gender = dto.Gender;
        student.FatherName = dto.FatherName;
        student.FatherOccupation = dto.FatherOccupation;
        student.MotherName = dto.MotherName;
        student.MotherOccupation = dto.MotherOccupation;
        student.MobileNumber = dto.MobileNumber;
        student.AlternativeNumber = dto.AlternativeNumber;
        student.EmailAddress = dto.EmailAddress;
        student.Nationality = dto.Nationality;
        student.Country = dto.Country;
        student.MaritalStatus = dto.MaritalStatus;
        student.Religion = dto.Religion;
        student.AssignedReligionSubjectId = await GetReligionSubjectIdAsync(dto.Religion, cancellationToken);
        student.OptionalSubjectId = dto.OptionalSubjectId;
        student.BloodGroup = dto.BloodGroup;
        student.PresentVillage = dto.PresentVillage;
        student.PresentPostOffice = dto.PresentPostOffice;
        student.PresentThana = dto.PresentThana;
        student.PresentDistrict = dto.PresentDistrict;
        student.PermanentVillage = dto.PermanentVillage;
        student.PermanentPostOffice = dto.PermanentPostOffice;
        student.PermanentThana = dto.PermanentThana;
        student.PermanentDistrict = dto.PermanentDistrict;
        student.BirthCertificateNo = dto.BirthCertificateNo;
        var classChanged = student.ClassId != dto.ClassId;
        student.ClassId = dto.ClassId;
        student.SectionId = dto.SectionId;

        // Revalidate group when class changes
        if (classChanged)
        {
            var schoolClassForGroup = await _classRepository.GetByIdAsync(dto.ClassId, cancellationToken);
            if (schoolClassForGroup != null)
            {
                var settings = await _settingRepo.GetCurrentSettingsAsync(cancellationToken);
                if (settings != null)
                {
                    bool classRequiresGroup = schoolClassForGroup.SortOrder >= settings.GroupStartsFromClassId;
                    if (classRequiresGroup && !dto.StudentGroupId.HasValue && !student.StudentGroupId.HasValue)
                        throw new InvalidOperationException("An academic group is required for the selected class.");
                    if (!classRequiresGroup)
                        dto.StudentGroupId = null;
                }
            }
        }

        student.StudentGroupId = dto.StudentGroupId ?? (dto.SectionId > 0
            ? (await _sectionRepository.GetByIdAsync(dto.SectionId, cancellationToken))?.StudentGroupId
            : null);
        student.RollNumber = dto.RollNumber;
        student.UpdatedAt = DateTime.UtcNow;
        student.UpdatedBy = updatedBy;

        var sg = student.StudentGuardians.FirstOrDefault(x => x.IsPrimaryGuardian);
        if (sg != null && sg.Guardian != null)
        {
            var isGuardian = !string.IsNullOrWhiteSpace(dto.GuardianName);
            sg.Guardian.FirstName = isGuardian ? dto.GuardianName!.Trim() : dto.FatherName.Trim();
            sg.Guardian.LastName = "";
            sg.Guardian.FullName = sg.Guardian.FirstName;
            sg.Guardian.MobileNumber = (dto.FatherOrGuardianMobileNo ?? "").Trim();
            sg.Guardian.Occupation = dto.GuardianOccupation?.Trim();
            sg.Guardian.RelationType = isGuardian 
                ? SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.LegalGuardian 
                : SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.Father;
            
            sg.Relationship = sg.Guardian.RelationType;
            sg.UpdatedAt = DateTime.UtcNow;
            sg.UpdatedBy = updatedBy;
        }
        
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // Update StudentGroupAssignment
        var assignmentRepo = _unitOfWork.Repository<StudentGroupAssignment>();
        var existingAssignment = await assignmentRepo.Query()
            .FirstOrDefaultAsync(a => a.StudentId == student.Id, cancellationToken);
        if (student.StudentGroupId.HasValue)
        {
            var activeYear = await _unitOfWork.Repository<AcademicYear>().FirstOrDefaultAsync(x => x.IsActive, cancellationToken);
            if (existingAssignment != null)
            {
                existingAssignment.StudentGroupId = student.StudentGroupId.Value;
                existingAssignment.SchoolClassId = dto.ClassId;
                existingAssignment.AcademicYearId = activeYear?.Id ?? existingAssignment.AcademicYearId;
                existingAssignment.AssignedDate = DateTime.UtcNow;
            }
            else if (activeYear != null)
            {
                await assignmentRepo.AddAsync(new StudentGroupAssignment
                {
                    StudentId = student.Id,
                    StudentGroupId = student.StudentGroupId.Value,
                    SchoolClassId = dto.ClassId,
                    AcademicYearId = activeYear.Id,
                    AssignedDate = DateTime.UtcNow
                });
            }
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }
        else if (existingAssignment != null)
        {
            assignmentRepo.Remove(existingAssignment);
            await _unitOfWork.SaveChangesAsync(cancellationToken);
        }

        await LogAuditAsync("Student", "Student.Update", student.Id.ToString(), $"Updated student: {student.FullName} ({student.StudentNo})", cancellationToken);
    }

    public async Task<StudentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        return await _studentRepository.GetForEditAsync(id, cancellationToken);
    }

    public async Task<StudentUpsertDto?> GetByStudentNoAsync(string studentNo, CancellationToken cancellationToken = default)
    {
        return await _studentRepository.GetByStudentNoAsync(studentNo, cancellationToken);
    }

    public async Task<PagedResult<StudentListItemDto>> GetPagedAsync(int page, int pageSize, string? search, int? classId = null, int? sectionId = null, int? status = null, CancellationToken cancellationToken = default)
    {
        var (items, totalItems) = await _studentRepository.GetPagedAsync(page, pageSize, search, classId, sectionId, status, cancellationToken);
        return new PagedResult<StudentListItemDto> { Items = items, TotalItems = totalItems, Page = page, PageSize = pageSize };
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.GetByIdAsync(id, cancellationToken)
            ?? throw new Exception("Student not found");
        student.IsDeleted = true;
        student.UpdatedBy = updatedBy;
        student.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        await LogAuditAsync("Student", "Student.Delete", id.ToString(), $"Deleted student: {student.FullName} ({student.StudentNo})", cancellationToken);
    }

    public async Task<StudentUpsertDto?> GetByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var studentId = await GetStudentIdByUserIdAsync(userId, cancellationToken);
        if (studentId == null) return null;
        return await GetForEditAsync(studentId.Value, cancellationToken);
    }

    public async Task<int?> GetStudentIdByUserIdAsync(int userId, CancellationToken cancellationToken = default)
    {
        var student = await _studentRepository.FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, cancellationToken);
        return student?.Id;
    }

    private async Task<int?> GetReligionSubjectIdAsync(string religion, CancellationToken cancellationToken)
    {
        var code = religion?.Trim().ToLowerInvariant() switch
        {
            "islam" => "IRE",
            "hindu" => "HRE",
            "buddhist" => "BRE",
            "christian" => "CRE",
            _ => null
        };
        if (code == null) return null;
        var subject = await _unitOfWork.Repository<Subject>()
            .FirstOrDefaultAsync(s => s.Code == code && !s.IsDeleted, cancellationToken);
        return subject?.Id;
    }

    public async Task<List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>> GetOptionalSubjectsAsync(int classId, CancellationToken cancellationToken = default)
    {
        var optionalSubjectIds = await _unitOfWork.Repository<ClassSubject>()
            .QueryNoTracking()
            .Where(cs => cs.SchoolClassId == classId && cs.IsOptional && !cs.IsDeleted)
            .Select(cs => cs.SubjectId)
            .Distinct()
            .ToListAsync(cancellationToken);

        if (optionalSubjectIds.Count == 0)
            return new List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem>();

        var subjects = await _unitOfWork.Repository<Subject>()
            .Query()
            .Where(s => optionalSubjectIds.Contains(s.Id) && !s.IsDeleted)
            .ToListAsync(cancellationToken);

        return subjects
            .Select(s => new Microsoft.AspNetCore.Mvc.Rendering.SelectListItem
            {
                Value = s.Id.ToString(),
                Text = $"{s.Code} — {s.Name}"
            })
            .ToList();
    }

    public async Task<int?> GetStudentGroupIdByClassSectionAsync(int classId, int sectionId, CancellationToken ct = default)
    {
        var studentGroupId = await _studentRepository.Query()
            .Where(s => s.ClassId == classId && s.SectionId == sectionId)
            .Select(s => s.StudentGroupId)
            .FirstOrDefaultAsync(ct);
        return studentGroupId;
    }

    public async Task<List<StudentClassSectionDto>> GetStudentClassSectionsAsync(List<int> studentIds, CancellationToken ct = default)
    {
        return await _studentRepository.Query()
            .Where(s => studentIds.Contains(s.Id))
            .Select(s => new StudentClassSectionDto { ClassId = s.ClassId, SectionId = s.SectionId })
            .Distinct()
            .ToListAsync(ct);
    }

    private async Task<string> GenerateStudentNoAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _studentRepository.CountAsync(s => !s.IsDeleted && s.CreatedAt.Year == year, cancellationToken) + 1;
        return $"STU-{year}{count:D3}";
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

