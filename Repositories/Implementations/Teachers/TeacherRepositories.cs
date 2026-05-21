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
        var query = _db.Teachers.AsNoTracking()
            .Include(t => t.Employee)
            .ThenInclude(e => e.Designation)
            .Include(t => t.Employee.Department)
            .Where(t => !t.IsDeleted && !t.Employee.IsDeleted && (t.Employee.IsTeachingStaff || t.Employee.Designation.IsTeachingRole));

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(t => t.Employee.FullName.Contains(search) || 
                                     t.TeacherCode.Contains(search) || 
                                     t.Employee.Phone.Contains(search) || 
                                     t.Employee.Email.Contains(search));
        }

        if (!string.IsNullOrEmpty(department))
        {
            query = query.Where(t => t.Employee.Department.Name == department);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(t => t.Employee.Status == status);
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(t => t.Employee.FullName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(t => new TeacherListItemDto
            {
                Id = t.Id,
                TeacherNo = t.TeacherCode,
                FullName = t.Employee.FullName,
                Designation = t.Employee.Designation != null ? t.Employee.Designation.Name : string.Empty,
                Department = t.Employee.Department != null ? t.Employee.Department.Name : string.Empty,
                MobileNumber = t.Employee.Phone,
                EmailAddress = t.Employee.Email,
                Status = t.Employee.Status,
                ProfilePicturePath = t.Employee.ProfilePicturePath
            }).ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<TeacherUpsertDto?> GetForEditAsync(int id, CancellationToken ct)
    {
        return await _db.Teachers.AsNoTracking()
            .Include(t => t.Employee)
            .ThenInclude(e => e.Designation)
            .Include(t => t.Employee.Department)
            .Where(t => t.Id == id && !t.IsDeleted)
            .Select(t => new TeacherUpsertDto
            {
                Id = t.Id,
                TeacherNo = t.TeacherCode,
                FullName = t.Employee.FullName,
                DateOfBirth = t.Employee.DateOfBirth,
                Gender = t.Employee.Gender,
                MobileNumber = t.Employee.Phone,
                EmailAddress = t.Employee.Email,
                Nationality = t.Employee.Nationality,
                Designation = t.Employee.Designation != null ? t.Employee.Designation.Name : string.Empty,
                Department = t.Employee.Department != null ? t.Employee.Department.Name : string.Empty,
                Specialization = t.SubjectSpecialization,
                JoiningDate = t.Employee.JoiningDate,
                FatherName = t.Employee.FatherName,
                MotherName = t.Employee.MotherName,
                PresentVillage = t.Employee.PresentAddress,
                PermanentVillage = t.Employee.PermanentAddress,
                ProfilePicturePath = t.Employee.ProfilePicturePath,
                Status = t.Employee.Status ?? "Active"
            })
            .FirstOrDefaultAsync(ct);
    }

    public async Task<TeacherUpsertDto?> GetByUserIdAsync(int userId, CancellationToken ct)
    {
        return await _db.Teachers.AsNoTracking()
            .Include(t => t.Employee)
            .ThenInclude(e => e.Designation)
            .Include(t => t.Employee.Department)
            .Where(t => t.Employee.UserId == userId && !t.IsDeleted)
            .Select(t => new TeacherUpsertDto
            {
                Id = t.Id,
                TeacherNo = t.TeacherCode,
                FullName = t.Employee.FullName,
                DateOfBirth = t.Employee.DateOfBirth,
                Gender = t.Employee.Gender,
                MobileNumber = t.Employee.Phone,
                EmailAddress = t.Employee.Email,
                Nationality = t.Employee.Nationality,
                Designation = t.Employee.Designation != null ? t.Employee.Designation.Name : string.Empty,
                Department = t.Employee.Department != null ? t.Employee.Department.Name : string.Empty,
                Specialization = t.SubjectSpecialization,
                JoiningDate = t.Employee.JoiningDate,
                FatherName = t.Employee.FatherName,
                MotherName = t.Employee.MotherName,
                PresentVillage = t.Employee.PresentAddress,
                PermanentVillage = t.Employee.PermanentAddress,
                ProfilePicturePath = t.Employee.ProfilePicturePath,
                Status = t.Employee.Status ?? "Active"
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
