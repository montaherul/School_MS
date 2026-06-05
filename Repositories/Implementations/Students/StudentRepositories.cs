using System;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Repositories.Interfaces.Students;

namespace SchoolManagementSystem.Repositories.Implementations.Students;

public class StudentRepository : BaseRepository<Student>, IStudentRepository
{
    public StudentRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<StudentListItemDto> items, int totalRecords)> GetPagedAsync(int page, int pageSize, string? search, int? classId, int? sectionId, int? status, CancellationToken ct)
    {
        var parameters = new[]
        {
            new Microsoft.Data.SqlClient.SqlParameter("@PageNumber", page),
            new Microsoft.Data.SqlClient.SqlParameter("@PageSize", pageSize),
            new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value),
            new Microsoft.Data.SqlClient.SqlParameter("@ClassId", classId ?? 0),
            new Microsoft.Data.SqlClient.SqlParameter("@SectionId", sectionId ?? 0),
            new Microsoft.Data.SqlClient.SqlParameter("@Status", (object?)status ?? DBNull.Value)
        };

        var items = await _db.Set<StudentListItemDto>()
            .FromSqlRaw("EXEC sp_GetStudentList @PageNumber, @PageSize, @SearchTerm, @ClassId, @SectionId, @Status", parameters)
            .ToListAsync(ct);

        int totalRecords = items.FirstOrDefault()?.TotalRecords ?? 0;
        return (items, totalRecords);
    }

    public async Task<StudentUpsertDto?> GetForEditAsync(int id, CancellationToken ct)
    {
        var student = await _db.Students.AsNoTracking()
            .Include(s => s.StudentGuardians)
                .ThenInclude(sg => sg.Guardian)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);

        if (student == null) return null;

        var sg = student.StudentGuardians.FirstOrDefault(x => x.IsPrimaryGuardian);
        var guardian = sg?.Guardian;
        return new StudentUpsertDto
        {
            Id = student.Id,
            StudentNo = student.StudentNo,
            FullName = student.FullName,
            FullNameBangla = student.FullNameBangla,
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            FatherName = student.FatherName,
            FatherOccupation = student.FatherOccupation,
            MotherName = student.MotherName,
            MotherOccupation = student.MotherOccupation,
            MobileNumber = student.MobileNumber,
            AlternativeNumber = student.AlternativeNumber,
            EmailAddress = student.EmailAddress,
            Nationality = student.Nationality,
            Country = student.Country,
            MaritalStatus = student.MaritalStatus,
            Religion = student.Religion,
            BloodGroup = student.BloodGroup,
            PresentVillage = student.PresentVillage,
            PresentPostOffice = student.PresentPostOffice,
            PresentThana = student.PresentThana,
            PresentDistrict = student.PresentDistrict,
            PermanentVillage = student.PermanentVillage,
            PermanentPostOffice = student.PermanentPostOffice,
            PermanentThana = student.PermanentThana,
            PermanentDistrict = student.PermanentDistrict,
            
            
            BirthCertificateNo = student.BirthCertificateNo,
            ProfilePicturePath = student.ProfilePicturePath,
            ClassId = student.ClassId,
            SectionId = student.SectionId,
            RollNumber = student.RollNumber,
            SectionName = student.Section?.Name ?? "N/A",
            GuardianName = sg?.Relationship == SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.LegalGuardian ? guardian?.FirstName : null,
            GuardianOccupation = guardian?.Occupation,
            FatherOrGuardianMobileNo = guardian?.MobileNumber ?? string.Empty
        };
    }

    public async Task<StudentUpsertDto?> GetByStudentNoAsync(string studentNo, CancellationToken ct)
    {
        var student = await _db.Students.AsNoTracking()
            .Include(s => s.StudentGuardians)
                .ThenInclude(sg => sg.Guardian)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.StudentNo == studentNo && !s.IsDeleted, ct);

        if (student == null) return null;

        var sg = student.StudentGuardians.FirstOrDefault(x => x.IsPrimaryGuardian);
        var guardian = sg?.Guardian;
        return new StudentUpsertDto
        {
            Id = student.Id,
            StudentNo = student.StudentNo,
            FullName = student.FullName,
            FullNameBangla = student.FullNameBangla,
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            FatherName = student.FatherName,
            FatherOccupation = student.FatherOccupation,
            MotherName = student.MotherName,
            MotherOccupation = student.MotherOccupation,
            MobileNumber = student.MobileNumber,
            AlternativeNumber = student.AlternativeNumber,
            EmailAddress = student.EmailAddress,
            Nationality = student.Nationality,
            Country = student.Country,
            MaritalStatus = student.MaritalStatus,
            Religion = student.Religion,
            BloodGroup = student.BloodGroup,
            PresentVillage = student.PresentVillage,
            PresentPostOffice = student.PresentPostOffice,
            PresentThana = student.PresentThana,
            PresentDistrict = student.PresentDistrict,
            PermanentVillage = student.PermanentVillage,
            PermanentPostOffice = student.PermanentPostOffice,
            PermanentThana = student.PermanentThana,
            PermanentDistrict = student.PermanentDistrict,
            
            
            BirthCertificateNo = student.BirthCertificateNo,
            ProfilePicturePath = student.ProfilePicturePath,
            ClassId = student.ClassId,
            SectionId = student.SectionId,
            RollNumber = student.RollNumber,
            SectionName = student.Section?.Name ?? "N/A",
            GuardianName = sg?.Relationship == SchoolManagementSystem.Models.Entities.Guardian.GuardianRelationshipType.LegalGuardian ? guardian?.FirstName : null,
            GuardianOccupation = guardian?.Occupation,
            FatherOrGuardianMobileNo = guardian?.MobileNumber ?? string.Empty
        };
    }
}
