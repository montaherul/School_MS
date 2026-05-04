using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Student;
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
            StudentNo = GenerateStudentNo(),

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

    // ✅ PAGINATION
    public async Task<PagedResult<StudentListItemDto>> GetPagedAsync(
        int page,
        int pageSize,
        string? search,
        CancellationToken cancellationToken = default)
    {
        var query = _db.Students.Where(s => !s.IsDeleted);

        if (!string.IsNullOrWhiteSpace(search))
        {
            query = query.Where(s =>
                s.FullName.Contains(search) ||
                s.StudentNo.Contains(search));
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var items = await (
            from s in query
            join c in _db.Classes on s.ClassId equals c.Id
            join sec in _db.Sections on s.SectionId equals sec.Id
            orderby s.Id descending
            select new StudentListItemDto
            {
                Id = s.Id,
                StudentNo = s.StudentNo,
                FullName = s.FullName,

                ClassName = c.Name,
                SectionName = sec.Name,


                RollNumber = s.RollNumber,
                Status = s.Status.ToString(),

                FatherName = s.FatherName,
                FatherOccupation = s.FatherOccupation,
                MotherName = s.MotherName,
                MotherOccupation = s.MotherOccupation,

                MobileNumber = s.MobileNumber,
                EmailAddress = s.EmailAddress,

                PresentVillage = s.PresentVillage,
                PresentPostOffice = s.PresentPostOffice,
                PresentThana = s.PresentThana,
                PresentDistrict = s.PresentDistrict,

                PermanentVillage = s.PermanentVillage,
                PermanentPostOffice = s.PermanentPostOffice,
                PermanentThana = s.PermanentThana,
                PermanentDistrict = s.PermanentDistrict,

                BloodGroup = s.BloodGroup,
                Religion = s.Religion,
                Nationality = s.Nationality,

                NationalIdNo = s.NationalIdNo,
                BirthCertificateNo = s.BirthCertificateNo,
                PassportNo = s.PassportNo,

                ProfilePicturePath = s.ProfilePicturePath,

                FatherOrGuardianMobileNo = s.Guardians
                    .Select(g => g.Phone)
                    .FirstOrDefault()
            }
        )
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync(cancellationToken);
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
    private string GenerateStudentNo()
    {
        var year = DateTime.UtcNow.Year.ToString().Substring(2);
        var count = _db.Students.Count(s => !s.IsDeleted) + 1;
        return $"STU-{year}{count:D3}";
    }
}
