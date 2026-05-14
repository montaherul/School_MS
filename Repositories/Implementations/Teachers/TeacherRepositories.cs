using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.DTOs.Teacher;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Teachers;

namespace SchoolManagementSystem.Repositories.Implementations.Teachers;

public class TeacherRepository : BaseRepository<Teacher>, ITeacherRepository
{
    public TeacherRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<TeacherListItemDto> items, int totalRecords)> GetPagedAsync(int page, int pageSize, string? search, string? department, string? status, CancellationToken ct)
    {
        var query = _db.Teachers.AsNoTracking().Where(t => !t.IsDeleted);
        if (!string.IsNullOrEmpty(search)) query = query.Where(t => t.FullName.Contains(search) || t.TeacherNo.Contains(search));
        if (!string.IsNullOrEmpty(department)) query = query.Where(t => t.Department == department);
        if (!string.IsNullOrEmpty(status))
        {
            if (Enum.TryParse<TeacherStatus>(status, out var s)) query = query.Where(t => t.Status == s);
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(t => t.FullName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(t => new TeacherListItemDto
            {
                Id = t.Id,
                TeacherNo = t.TeacherNo,
                FullName = t.FullName,
                Designation = t.Designation,
                Department = t.Department,
                MobileNumber = t.MobileNumber,
                Status = t.Status.ToString(),
                ProfilePicturePath = t.ProfilePicturePath
            }).ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<TeacherUpsertDto?> GetForEditAsync(int id, CancellationToken ct)
    {
        return await _db.Teachers.AsNoTracking()
            .Where(t => t.Id == id && !t.IsDeleted)
            .Select(t => new TeacherUpsertDto
            {
                Id = t.Id,
                TeacherNo = t.TeacherNo,
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
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<TeacherUpsertDto?> GetByUserIdAsync(int userId, CancellationToken ct)
    {
        return await _db.Teachers.AsNoTracking()
            .Where(t => t.UserId == userId && !t.IsDeleted)
            .Select(t => new TeacherUpsertDto
            {
                Id = t.Id,
                TeacherNo = t.TeacherNo,
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
            })
            .FirstOrDefaultAsync(ct);
    }
}

public class TeacherClassAssignmentRepository : BaseRepository<TeacherClassAssignment>, ITeacherClassAssignmentRepository 
{ 
    public TeacherClassAssignmentRepository(SchoolDbContext db) : base(db) { } 
}

public class TeacherSubjectAssignmentRepository : BaseRepository<TeacherSubjectAssignment>, ITeacherSubjectAssignmentRepository 
{ 
    public TeacherSubjectAssignmentRepository(SchoolDbContext db) : base(db) { } 
}
