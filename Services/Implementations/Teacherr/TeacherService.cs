using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Teacher;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Teachers;

public class TeacherService : ITeacherService
{
    private readonly SchoolDbContext _db;

    public TeacherService(SchoolDbContext db)
    {
        _db = db;
    }

    // ✅ CREATE
    public async Task<int> CreateAsync(
        TeacherUpsertDto dto,
        string createdBy,
        CancellationToken cancellationToken = default)
    {
        // 🔍 Email duplicate check
        if (!string.IsNullOrWhiteSpace(dto.EmailAddress))
        {
            var exists = await _db.Teachers.AnyAsync(t =>
                t.EmailAddress == dto.EmailAddress &&
                !t.IsDeleted,
                cancellationToken);

            if (exists)
                throw new Exception($"Teacher with email '{dto.EmailAddress}' already exists.");
        }

        // 📸 Upload image
        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/teachers");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.ProfilePicture.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.ProfilePicture.CopyToAsync(stream, cancellationToken);

            dto.ProfilePicturePath = "/uploads/teachers/" + fileName;
        }

        var teacher = new Teacher
        {
            TeacherNo = GenerateTeacherNo(),

            FullName = dto.FullName,
            FullNameBangla = dto.FullNameBangla,

            DateOfBirth = dto.DateOfBirth,
            Gender = dto.Gender,

            MobileNumber = dto.MobileNumber,
            AlternativeNumber = dto.AlternativeNumber,
            EmailAddress = dto.EmailAddress,

            Nationality = dto.Nationality,
            Country = dto.Country,
            MaritalStatus = dto.MaritalStatus,
            Religion = dto.Religion,
            BloodGroup = dto.BloodGroup,

            PassportNo = dto.PassportNo,
            NationalIdNo = dto.NationalIdNo,

            Designation = dto.Designation,
            Department = dto.Department,
            Qualification = dto.Qualification,
            Specialization = dto.Specialization,
            JoiningDate = dto.JoiningDate,

            FatherName = dto.FatherName,
            MotherName = dto.MotherName,
            SpouseName = dto.SpouseName,

            PresentVillage = dto.PresentVillage,
            PresentPostOffice = dto.PresentPostOffice,
            PresentThana = dto.PresentThana,
            PresentDistrict = dto.PresentDistrict,

            PermanentVillage = dto.PermanentVillage,
            PermanentPostOffice = dto.PermanentPostOffice,
            PermanentThana = dto.PermanentThana,
            PermanentDistrict = dto.PermanentDistrict,

            ProfilePicturePath = dto.ProfilePicturePath,
            Status = TeacherStatus.Active,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        _db.Teachers.Add(teacher);
        await _db.SaveChangesAsync(cancellationToken);

        return teacher.Id;
    }

    // ✅ UPDATE
    public async Task UpdateAsync(
        TeacherUpsertDto dto,
        string updatedBy,
        CancellationToken cancellationToken = default)
    {
        var teacher = await _db.Teachers
            .FirstOrDefaultAsync(t => t.Id == dto.Id && !t.IsDeleted, cancellationToken)
            ?? throw new Exception("Teacher not found");

        // 🔍 Email check
        if (!string.IsNullOrWhiteSpace(dto.EmailAddress))
        {
            var exists = await _db.Teachers.AnyAsync(t =>
                t.EmailAddress == dto.EmailAddress &&
                t.Id != dto.Id &&
                !t.IsDeleted,
                cancellationToken);

            if (exists)
                throw new Exception("Email already used by another teacher");
        }

        // 📸 Update image
        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/teachers");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // delete old
            if (!string.IsNullOrEmpty(teacher.ProfilePicturePath))
            {
                var oldPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    teacher.ProfilePicturePath.TrimStart('/')
                );

                if (File.Exists(oldPath))
                    File.Delete(oldPath);
            }

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.ProfilePicture.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using var stream = new FileStream(filePath, FileMode.Create);
            await dto.ProfilePicture.CopyToAsync(stream, cancellationToken);

            teacher.ProfilePicturePath = "/uploads/teachers/" + fileName;
        }

        // 🔄 Update fields
        teacher.FullName = dto.FullName;
        teacher.FullNameBangla = dto.FullNameBangla;
        teacher.DateOfBirth = dto.DateOfBirth;
        teacher.Gender = dto.Gender;

        teacher.MobileNumber = dto.MobileNumber;
        teacher.AlternativeNumber = dto.AlternativeNumber;
        teacher.EmailAddress = dto.EmailAddress;

        teacher.Nationality = dto.Nationality;
        teacher.Country = dto.Country;
        teacher.MaritalStatus = dto.MaritalStatus;
        teacher.Religion = dto.Religion;
        teacher.BloodGroup = dto.BloodGroup;

        teacher.PassportNo = dto.PassportNo;
        teacher.NationalIdNo = dto.NationalIdNo;

        teacher.Designation = dto.Designation;
        teacher.Department = dto.Department;
        teacher.Qualification = dto.Qualification;
        teacher.Specialization = dto.Specialization;
        teacher.JoiningDate = dto.JoiningDate;

        teacher.FatherName = dto.FatherName;
        teacher.MotherName = dto.MotherName;
        teacher.SpouseName = dto.SpouseName;

        teacher.PresentVillage = dto.PresentVillage;
        teacher.PresentPostOffice = dto.PresentPostOffice;
        teacher.PresentThana = dto.PresentThana;
        teacher.PresentDistrict = dto.PresentDistrict;

        teacher.PermanentVillage = dto.PermanentVillage;
        teacher.PermanentPostOffice = dto.PermanentPostOffice;
        teacher.PermanentThana = dto.PermanentThana;
        teacher.PermanentDistrict = dto.PermanentDistrict;

        teacher.UpdatedAt = DateTime.UtcNow;
        teacher.UpdatedBy = updatedBy;

        await _db.SaveChangesAsync(cancellationToken);
    }

    // ✅ GET FOR EDIT
    public async Task<TeacherUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var t = await _db.Teachers
            .FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, cancellationToken)
            ?? throw new Exception("Teacher not found");

        return new TeacherUpsertDto
        {
            Id = t.Id,
            FullName = t.FullName,
            FullNameBangla = t.FullNameBangla,
            DateOfBirth = t.DateOfBirth,
            Gender = t.Gender,
            MobileNumber = t.MobileNumber,
            AlternativeNumber = t.AlternativeNumber,
            EmailAddress = t.EmailAddress,
            Nationality = t.Nationality,
            Country = t.Country,
            MaritalStatus = t.MaritalStatus,
            Religion = t.Religion,
            BloodGroup = t.BloodGroup,
            PassportNo = t.PassportNo,
            NationalIdNo = t.NationalIdNo,
            Designation = t.Designation,
            Department = t.Department,
            Qualification = t.Qualification,
            Specialization = t.Specialization,
            JoiningDate = t.JoiningDate,
            FatherName = t.FatherName,
            MotherName = t.MotherName,
            SpouseName = t.SpouseName,
            PresentVillage = t.PresentVillage,
            PresentPostOffice = t.PresentPostOffice,
            PresentThana = t.PresentThana,
            PresentDistrict = t.PresentDistrict,
            PermanentVillage = t.PermanentVillage,
            PermanentPostOffice = t.PermanentPostOffice,
            PermanentThana = t.PermanentThana,
            PermanentDistrict = t.PermanentDistrict,
            ProfilePicturePath = t.ProfilePicturePath
        };
    }

    // ✅ PAGINATION
    public async Task<PagedResult<TeacherListItemDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Teachers.Where(t => !t.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(t =>
                t.FullName.Contains(search) ||
                t.TeacherNo.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await query
            .OrderByDescending(t => t.Id)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .Select(t => new TeacherListItemDto
            {
                Id = t.Id,
                TeacherNo = t.TeacherNo,
                FullName = t.FullName,
                MobileNumber = t.MobileNumber,
                EmailAddress = t.EmailAddress,
                Designation = t.Designation,
                Department = t.Department,
                Status = t.Status.ToString(),
                ProfilePicturePath = t.ProfilePicturePath
            })
            .ToListAsync(cancellationToken);

        return new PagedResult<TeacherListItemDto>
        {
            Items = items,
            TotalItems = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // ✅ DELETE (soft)
    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var teacher = await _db.Teachers
            .FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, cancellationToken)
            ?? throw new Exception("Teacher not found");

        teacher.IsDeleted = true;
        teacher.UpdatedAt = DateTime.UtcNow;
        teacher.UpdatedBy = updatedBy;

        await _db.SaveChangesAsync(cancellationToken);
    }

    // 🔧 Helper
    private string GenerateTeacherNo()
    {
        return "TCH-" + DateTime.UtcNow.Ticks;
    }
}