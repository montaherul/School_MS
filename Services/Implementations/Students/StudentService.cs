using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Students;

namespace SchoolManagementSystem.Services.Implementations.Students;

public class StudentService : IStudentService
{
    private readonly SchoolDbContext _db;

    public StudentService(SchoolDbContext db)
    {
        _db = db;
    }

    // ✅ CREATE
    public async Task<int> CreateAsync(StudentUpsertDto dto, string createdBy, CancellationToken cancellationToken = default)
    {
        // 🔥 CAPACITY VALIDATION
        var section = await _db.Sections.FirstOrDefaultAsync(s => s.Id == dto.SectionId, cancellationToken);
        if (section != null)
        {
            var currentCount = await _db.Students.CountAsync(s => s.SectionId == dto.SectionId && !s.IsDeleted && s.Status == StudentStatus.Active, cancellationToken);
            if (currentCount >= section.Capacity)
                throw new InvalidOperationException($"Cannot assign student. Section '{section.Name}' capacity ({section.Capacity}) reached.");
        }

        // 🔥 FIRST: upload image
        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/students");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            var fileName = Guid.NewGuid() + Path.GetExtension(dto.ProfilePicture.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ProfilePicture.CopyToAsync(stream);
            }

            dto.ProfilePicturePath = "/uploads/students/" + fileName;
        }

        // 🔥 THEN create student
      
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

            // ✅ Address (IMPORTANT)
            PresentVillage = dto.PresentVillage,
            PresentPostOffice = dto.PresentPostOffice,
            PresentThana = dto.PresentThana,
            PresentDistrict = dto.PresentDistrict,

            PermanentVillage = dto.PermanentVillage,
            PermanentPostOffice = dto.PermanentPostOffice,
            PermanentThana = dto.PermanentThana,
            PermanentDistrict = dto.PermanentDistrict,

            PassportNo = dto.PassportNo,
            NationalIdNo = dto.NationalIdNo,
            BirthCertificateNo = dto.BirthCertificateNo,

            ProfilePicturePath = dto.ProfilePicturePath,

            ClassId = dto.ClassId,
            SectionId = dto.SectionId,
            RollNumber = dto.RollNumber,
            UserId = dto.UserId,

            CreatedAt = DateTime.UtcNow,
            CreatedBy = createdBy
        };

        // 🔹 Guardian
        if (!string.IsNullOrWhiteSpace(dto.FatherOrGuardianMobileNo))
        {
            var isGuardian = !string.IsNullOrWhiteSpace(dto.GuardianName);

            student.Guardians.Add(new Guardian
            {
                Name = isGuardian ? dto.GuardianName!.Trim() : dto.FatherName.Trim(),
                Phone = (dto.FatherOrGuardianMobileNo ?? "").Trim(),
                Occupation = dto.GuardianOccupation?.Trim(),
                Relation = isGuardian ? "Guardian" : "Father",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            });
        }

      

        _db.Students.Add(student);
        await _db.SaveChangesAsync(cancellationToken);

        return student.Id;
    }

    // ✅ UPDATE
    public async Task UpdateAsync(StudentUpsertDto dto, string updatedBy, CancellationToken cancellationToken = default)
    {
        var student = await _db.Students
            .Include(s => s.Guardians)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.Id == dto.Id && !s.IsDeleted, cancellationToken)
            ?? throw new Exception("Student not found");

        // 🔥 CAPACITY VALIDATION (Only if changing sections)
        if (student.SectionId != dto.SectionId)
        {
            var section = await _db.Sections.FirstOrDefaultAsync(s => s.Id == dto.SectionId, cancellationToken);
            if (section != null)
            {
                var currentCount = await _db.Students.CountAsync(s => s.SectionId == dto.SectionId && !s.IsDeleted && s.Status == StudentStatus.Active, cancellationToken);
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
        student.BloodGroup = dto.BloodGroup;

        // ✅ Address update
        student.PresentVillage = dto.PresentVillage;
        student.PresentPostOffice = dto.PresentPostOffice;
        student.PresentThana = dto.PresentThana;
        student.PresentDistrict = dto.PresentDistrict;

        student.PermanentVillage = dto.PermanentVillage;
        student.PermanentPostOffice = dto.PermanentPostOffice;
        student.PermanentThana = dto.PermanentThana;
        student.PermanentDistrict = dto.PermanentDistrict;

        student.PassportNo = dto.PassportNo;
        student.NationalIdNo = dto.NationalIdNo;
        student.BirthCertificateNo = dto.BirthCertificateNo;


        //student.ProfilePicturePath = dto.ProfilePicturePath;

        student.ClassId = dto.ClassId;
        student.SectionId = dto.SectionId;
        student.RollNumber = dto.RollNumber;

        student.UpdatedAt = DateTime.UtcNow;
        student.UpdatedBy = updatedBy;
        

        // 🔹 Guardian update
        var guardian = student.Guardians.FirstOrDefault();

        if (guardian == null && !string.IsNullOrWhiteSpace(dto.FatherOrGuardianMobileNo))
        {
            var isGuardian = !string.IsNullOrWhiteSpace(dto.GuardianName);

            student.Guardians.Add(new Guardian
            {
                Name = isGuardian ? dto.GuardianName!.Trim() : dto.FatherName.Trim(),
                Phone = (dto.FatherOrGuardianMobileNo ?? "").Trim(),
                Occupation = dto.GuardianOccupation?.Trim(),
                Relation = isGuardian ? "Guardian" : "Father",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = updatedBy
            });
        }
        else if (guardian != null)
        {
            var isGuardian = !string.IsNullOrWhiteSpace(dto.GuardianName);

            guardian.Name = isGuardian ? dto.GuardianName!.Trim() : dto.FatherName.Trim();
            guardian.Phone = (dto.FatherOrGuardianMobileNo ?? "").Trim();
            guardian.Occupation = dto.GuardianOccupation?.Trim();
            guardian.Relation = isGuardian ? "Guardian" : "Father";

            guardian.UpdatedAt = DateTime.UtcNow;
            guardian.UpdatedBy = updatedBy;
        }

        if (dto.ProfilePicture != null && dto.ProfilePicture.Length > 0)
        {
            var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads/students");

            if (!Directory.Exists(folderPath))
                Directory.CreateDirectory(folderPath);

            // 🔥 DELETE OLD IMAGE
            if (!string.IsNullOrEmpty(student.ProfilePicturePath))
            {
                var oldFilePath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot",
                    student.ProfilePicturePath.TrimStart('/')
                );

                if (File.Exists(oldFilePath))
                {
                    File.Delete(oldFilePath);
                }
            }

            // 🔥 SAVE NEW IMAGE
            var fileName = Guid.NewGuid() + Path.GetExtension(dto.ProfilePicture.FileName);
            var filePath = Path.Combine(folderPath, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await dto.ProfilePicture.CopyToAsync(stream);
            }

            // 🔥 UPDATE ENTITY DIRECTLY
            student.ProfilePicturePath = "/uploads/students/" + fileName;
        }

        await _db.SaveChangesAsync(cancellationToken);
    }

    // ✅ GET FOR EDIT
    public async Task<StudentUpsertDto?> GetForEditAsync(int id, CancellationToken cancellationToken = default)
    {
        var student = await _db.Students
            .Include(s => s.Guardians)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken)
            ?? throw new Exception("Student not found");

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

            // ✅ Address return
            PresentVillage = student.PresentVillage,
            PresentPostOffice = student.PresentPostOffice,
            PresentThana = student.PresentThana,
            PresentDistrict = student.PresentDistrict,

            PermanentVillage = student.PermanentVillage,
            PermanentPostOffice = student.PermanentPostOffice,
            PermanentThana = student.PermanentThana,
            PermanentDistrict = student.PermanentDistrict,

            PassportNo = student.PassportNo,
            NationalIdNo = student.NationalIdNo,
            BirthCertificateNo = student.BirthCertificateNo,

            ProfilePicturePath = student.ProfilePicturePath,

            ClassId = student.ClassId,
            SectionId = student.SectionId,
            RollNumber = student.RollNumber,
            SectionName = student.Section != null? student.Section.Name: "N/A",
            GuardianName = guardian?.Relation == "Guardian" ? guardian.Name : null,
            GuardianOccupation = guardian?.Occupation,
            FatherOrGuardianMobileNo = guardian?.Phone ?? string.Empty
        };
    }

    public async Task<StudentUpsertDto?> GetByStudentNoAsync(string studentNo, CancellationToken cancellationToken = default)
    {
        var student = await _db.Students
            .Include(s => s.Guardians)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.StudentNo == studentNo && !s.IsDeleted, cancellationToken);
            
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
            PassportNo = student.PassportNo,
            NationalIdNo = student.NationalIdNo,
            BirthCertificateNo = student.BirthCertificateNo,
            ProfilePicturePath = student.ProfilePicturePath,
            ClassId = student.ClassId,
            SectionId = student.SectionId,
            RollNumber = student.RollNumber,
            SectionName = student.Section != null ? student.Section.Name: "N/A",
            GuardianName = guardian?.Relation == "Guardian" ? guardian.Name : null,
            GuardianOccupation = guardian?.Occupation,
            FatherOrGuardianMobileNo = guardian?.Phone ?? string.Empty
        };
    }

    public async Task<PagedResult<StudentListItemDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var items = new List<StudentListItemDto>();
        int totalCount = 0;

        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetStudentList";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageNumber", page));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageSize", pageSize));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));

            await _db.Database.OpenConnectionAsync(cancellationToken);
            using (var reader = await command.ExecuteReaderAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new StudentListItemDto
                    {
                        Id = reader.GetInt32(0),
                        StudentNo = reader.GetString(1),
                        FullName = reader.GetString(2),
                        ClassName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                        SectionName = reader.IsDBNull(4) ? "" : reader.GetString(4),
                        RollNumber = reader.GetInt32(5),
                        Status = reader.IsDBNull(6) ? "" : reader.GetString(6),
                        FatherName = reader.IsDBNull(7) ? "" : reader.GetString(7),
                        FatherOccupation = reader.IsDBNull(8) ? "" : reader.GetString(8),
                        MotherName = reader.IsDBNull(9) ? "" : reader.GetString(9),
                        MotherOccupation = reader.IsDBNull(10) ? "" : reader.GetString(10),
                        MobileNumber = reader.IsDBNull(11) ? "" : reader.GetString(11),
                        EmailAddress = reader.IsDBNull(12) ? "" : reader.GetString(12),
                        PresentVillage = reader.IsDBNull(13) ? "" : reader.GetString(13),
                        PresentPostOffice = reader.IsDBNull(14) ? "" : reader.GetString(14),
                        PresentThana = reader.IsDBNull(15) ? "" : reader.GetString(15),
                        PresentDistrict = reader.IsDBNull(16) ? "" : reader.GetString(16),
                        PermanentVillage = reader.IsDBNull(17) ? "" : reader.GetString(17),
                        PermanentPostOffice = reader.IsDBNull(18) ? "" : reader.GetString(18),
                        PermanentThana = reader.IsDBNull(19) ? "" : reader.GetString(19),
                        PermanentDistrict = reader.IsDBNull(20) ? "" : reader.GetString(20),
                        BloodGroup = reader.IsDBNull(21) ? "" : reader.GetString(21),
                        Religion = reader.IsDBNull(22) ? "" : reader.GetString(22),
                        Nationality = reader.IsDBNull(23) ? "" : reader.GetString(23),
                        NationalIdNo = reader.IsDBNull(24) ? "" : reader.GetString(24),
                        BirthCertificateNo = reader.IsDBNull(25) ? "" : reader.GetString(25),
                        PassportNo = reader.IsDBNull(26) ? "" : reader.GetString(26),
                        ProfilePicturePath = reader.IsDBNull(27) ? "" : reader.GetString(27),
                        FatherOrGuardianMobileNo = reader.IsDBNull(28) ? "" : reader.GetString(28),
                        TotalRecords = reader.IsDBNull(29) ? 0 : reader.GetInt32(29)
                    });
                }
            }
            await _db.Database.CloseConnectionAsync();
        }

        totalCount = items.FirstOrDefault()?.TotalRecords ?? 0;

        return new PagedResult<StudentListItemDto>
        {
            Items = items,
            TotalItems = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    // ✅ DELETE
    public async Task DeleteAsync(int id, string updatedBy, CancellationToken cancellationToken = default)
    {
        var student = await _db.Students
            .FirstOrDefaultAsync(s => s.Id == id && !s.IsDeleted, cancellationToken)
            ?? throw new Exception("Student not found");

        student.IsDeleted = true;
        student.UpdatedBy = updatedBy;
        student.UpdatedAt = DateTime.UtcNow;

        await _db.SaveChangesAsync(cancellationToken);
    }

    // 🔧 Helper
 private async Task<string> GenerateStudentNoAsync(CancellationToken cancellationToken)
{
    var year = DateTime.UtcNow.Year;
    var count = await _db.Students
        .CountAsync(s => !s.IsDeleted && s.CreatedAt.Year == year, cancellationToken) + 1;
    return $"STU-{year}{count:D3}";
}
}
