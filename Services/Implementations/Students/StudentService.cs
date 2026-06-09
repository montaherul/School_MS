using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Repositories.Interfaces.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Data;

namespace SchoolManagementSystem.Services.Implementations.Students;

public class StudentService : IStudentService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IStudentRepository _studentRepository;
    private readonly ISectionRepository _sectionRepository;
    private readonly ISchoolClassRepository _classRepository;

    public StudentService(
        IUnitOfWork unitOfWork, 
        IStudentRepository studentRepository,
        ISectionRepository sectionRepository,
        ISchoolClassRepository classRepository) 
    { 
        _unitOfWork = unitOfWork;
        _studentRepository = studentRepository;
        _sectionRepository = sectionRepository;
        _classRepository = classRepository;
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

        var student = new Student
        {
            StudentNo = await GenerateStudentNoAsync(cancellationToken),
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
            student.StudentGuardians.Add(new SchoolManagementSystem.Models.Entities.Guardian.StudentGuardian
            {
                Guardian = new SchoolManagementSystem.Models.Entities.Guardian.Guardian
                {
                    FirstName = isGuardian ? dto.GuardianName!.Trim() : dto.FatherName.Trim(),
                    LastName = "",
                    FullName = isGuardian ? dto.GuardianName!.Trim() : dto.FatherName.Trim(),
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
        student.ClassId = dto.ClassId;
        student.SectionId = dto.SectionId;
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
            .Query()
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

    private async Task<string> GenerateStudentNoAsync(CancellationToken cancellationToken)
    {
        var year = DateTime.UtcNow.Year;
        var count = await _studentRepository.CountAsync(s => !s.IsDeleted && s.CreatedAt.Year == year, cancellationToken) + 1;
        return $"STU-{year}{count:D3}";
    }
}

