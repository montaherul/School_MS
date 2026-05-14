using System;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Repositories.Interfaces.Students;
using Dapper;
using System.Data;

namespace SchoolManagementSystem.Repositories.Implementations.Students;

public class StudentRepository : BaseRepository<Student>, IStudentRepository
{
    public StudentRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<StudentListItemDto> items, int totalRecords)> GetPagedAsync(int page, int pageSize, string? search, int? classId, int? sectionId, int? status, CancellationToken ct)
    {
        var connection = _db.Database.GetDbConnection();
        
        var parameters = new DynamicParameters();
        parameters.Add("@PageNumber", page);
        parameters.Add("@PageSize", pageSize);
        parameters.Add("@Search", search);
        parameters.Add("@ClassId", classId);
        parameters.Add("@SectionId", sectionId);
        parameters.Add("@Status", status);
        parameters.Add("@SortField", "StudentNo");
        parameters.Add("@SortDirection", "ASC");

        var result = (await connection.QueryAsync<dynamic>(
            "sp_Student_GetPaged",
            parameters,
            commandType: CommandType.StoredProcedure
        )).ToList();

        var data = result.Select(x => new StudentListItemDto
        {
            Id = Convert.ToInt32(x.Id),
            StudentNo = x.StudentNo,
            FullName = x.FullName,
            ClassName = x.ClassName,
            SectionName = x.SectionName,
            RollNumber = x.RollNumber,
            Status = Convert.ToInt32(x.Status).ToString(),
            ProfilePicturePath = x.ProfilePicturePath
        }).ToList();

        int totalRecords = data.Any() ? (int)result.First().TotalCount : 0;

        return (data, totalRecords);
    }

    public async Task<StudentUpsertDto?> GetForEditAsync(int id, CancellationToken ct)
    {
        var student = await _db.Students.AsNoTracking()
            .Include(s => s.Guardians)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, ct);

        if (student == null) return null;

        var guardian = student.Guardians.FirstOrDefault();
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
            GuardianName = guardian?.Relation == "Guardian" ? guardian.Name : null,
            GuardianOccupation = guardian?.Occupation,
            FatherOrGuardianMobileNo = guardian?.Phone ?? string.Empty
        };
    }

    public async Task<StudentUpsertDto?> GetByStudentNoAsync(string studentNo, CancellationToken ct)
    {
        var student = await _db.Students.AsNoTracking()
            .Include(s => s.Guardians)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.StudentNo == studentNo && !s.IsDeleted, ct);

        if (student == null) return null;

        var guardian = student.Guardians.FirstOrDefault();
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
            GuardianName = guardian?.Relation == "Guardian" ? guardian.Name : null,
            GuardianOccupation = guardian?.Occupation,
            FatherOrGuardianMobileNo = guardian?.Phone ?? string.Empty
        };
    }
}
