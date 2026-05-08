using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Teacher;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.System;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Teachers;

namespace SchoolManagementSystem.Services.Implementations.Teachers;

public class TeacherService : ITeacherService
{
    private readonly SchoolDbContext _db;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IEmailService _emailService;

    public TeacherService(SchoolDbContext db, IPasswordHashService passwordHashService,IEmailService emailService)
    {
        _db = db;
        _passwordHashService = passwordHashService;
        _emailService = emailService;
    }

    public async Task<int> CreateAsync(TeacherUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        return await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                // 1. Create ApplicationUser
                var userName = await GenerateUserNameAsync(dto.FullName, ct);
                var email = dto.EmailAddress ?? $"{userName}@school.local";
                
                // Ensure email is unique
                if (await _db.Users.AnyAsync(u => u.Email == email, ct))
                {
                    if (dto.EmailAddress != null)
                        throw new Exception($"Email address '{dto.EmailAddress}' is already in use.");
                    
                    // If it was a generated email, we need to pick a different one
                    userName = $"{userName}.{Guid.NewGuid().ToString("N").Substring(0, 4)}";
                    email = $"{userName}@school.local";
                }

                var password = GenerateRandomPassword();
                
                var user = new ApplicationUser
                {
                    UserName = userName,
                    Email = email,
                    PhoneNumber = dto.MobileNumber,
                    PasswordHash = _passwordHashService.HashPassword(password),
                    IsEmailConfirmed = true,
                    Status = AccountStatus.Active,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Users.Add(user);
                await _db.SaveChangesAsync(ct);

                // 2. Assign "Lecturer" Role
                var lecturerRole = await _db.Roles.FirstOrDefaultAsync(r => r.Name == "Lecturer", ct);
                if (lecturerRole != null)
                {
                    _db.UserRoles.Add(new UserRole { UserId = user.Id, RoleId = lecturerRole.Id });
                }

                // 3. Create Teacher
                var teacher = new Teacher
                {
                    TeacherNo = await GenerateTeacherNoAsync(ct),
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
                    UserId = user.Id,
                    CreatedBy = createdBy,
                    CreatedAt = DateTime.UtcNow
                };

                _db.Teachers.Add(teacher);
                await _db.SaveChangesAsync(ct);

                // 4. Log the credentials (In real scenario, send email)
                // 4. Send teacher account email
            if (!string.IsNullOrWhiteSpace(email))
            {
                await _emailService.SendTeacherAccountAsync(
                    email,
                    dto.FullName,
                    userName,
                    password,
                    ct);
            }

            // Optional: keep system log
            _db.SystemLogs.Add(new SystemLog
            {
                Level = "Information",
                Message = $"Teacher account created for {dto.FullName} ({userName})",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            });

            await _db.SaveChangesAsync(ct);

                await transaction.CommitAsync(ct);
                return teacher.Id;
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }

    public async Task UpdateAsync(TeacherUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        var strategy = _db.Database.CreateExecutionStrategy();
        await strategy.ExecuteAsync(async () =>
        {
            using var transaction = await _db.Database.BeginTransactionAsync(ct);
            try
            {
                var teacher = await _db.Teachers.FindAsync(new object[] { dto.Id }, ct);
                if (teacher == null) return;

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
                if (!string.IsNullOrEmpty(dto.ProfilePicturePath))
                {
                    teacher.ProfilePicturePath = dto.ProfilePicturePath;
                }
                teacher.UpdatedBy = updatedBy;
                teacher.UpdatedAt = DateTime.UtcNow;

                // Also update the linked user email if changed
                // Also update/create linked user
                if (teacher.UserId.HasValue)
                {
                    var user = await _db.Users.FindAsync(new object[] { teacher.UserId.Value }, ct);

                    if (user != null)
                    {
                        // Check duplicate email
                        var emailExists = await _db.Users.AnyAsync(
                            u => u.Email == dto.EmailAddress && u.Id != user.Id,
                            ct);

                        if (emailExists)
                            throw new Exception("Email already exists.");

                        user.Email = dto.EmailAddress;
                        user.UserName = dto.EmailAddress;
                        user.PhoneNumber = dto.MobileNumber;
                        user.UpdatedBy = updatedBy;
                        user.UpdatedAt = DateTime.UtcNow;
                    }
                }
                else
                {
                    // Create user automatically if missing
                    var userName = await GenerateUserNameAsync(dto.FullName, ct);
                    var password = GenerateRandomPassword();

                    var email = dto.EmailAddress ?? $"{userName}@school.local";

                    // Ensure email is unique
                    if (await _db.Users.AnyAsync(u => u.Email == email, ct))
                    {
                        if (dto.EmailAddress != null)
                            throw new Exception($"Email address '{dto.EmailAddress}' is already in use.");

                        userName = $"{userName}.{Guid.NewGuid().ToString("N").Substring(0, 4)}";
                        email = $"{userName}@school.local";
                    }

                    var newUser = new ApplicationUser
                    {
                        UserName = userName,
                        Email = email,
                        PhoneNumber = dto.MobileNumber,
                        PasswordHash = _passwordHashService.HashPassword(password),
                        IsEmailConfirmed = true,
                        Status = AccountStatus.Active,
                        CreatedBy = updatedBy,
                        CreatedAt = DateTime.UtcNow
                    };

                    _db.Users.Add(newUser);
                    await _db.SaveChangesAsync(ct);

                    teacher.UserId = newUser.Id;

                    // Assign Lecturer role
                    var lecturerRole = await _db.Roles
                        .FirstOrDefaultAsync(r => r.Name == "Lecturer", ct);

                    if (lecturerRole != null)
                    {
                        _db.UserRoles.Add(new UserRole
                        {
                            UserId = newUser.Id,
                            RoleId = lecturerRole.Id
                        });
                    }

                    // Send account email
                    await _emailService.SendTeacherAccountAsync(
                        email,
                        dto.FullName,
                        userName,
                        password,
                        ct);
                     }

                await _db.SaveChangesAsync(ct);
                await transaction.CommitAsync(ct);
            }
            catch
            {
                await transaction.RollbackAsync(ct);
                throw;
            }
        });
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct)
            ?? throw new Exception("Teacher not found");

        teacher.IsDeleted = true;
        teacher.UpdatedBy = updatedBy;
        teacher.UpdatedAt = DateTime.UtcNow;

        // Also deactivate user account
        if (teacher.UserId.HasValue)
        {
            var user = await _db.Users.FindAsync(new object[] { teacher.UserId.Value }, ct);
            if (user != null)
            {
                user.Status = AccountStatus.Inactive;
                user.UpdatedBy = updatedBy;
                user.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task<TeacherUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        return await _db.Teachers
            .AsNoTracking()
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

    public async Task<PagedResult<TeacherListItemDto>> GetPagedAsync(int page, int pageSize, string? search, string? department, string? status, CancellationToken ct = default)
    {
        var items = new List<TeacherListItemDto>();
        int totalCount = 0;

        using (var command = _db.Database.GetDbConnection().CreateCommand())
        {
            command.CommandText = "sp_GetTeacherList";
            command.CommandType = System.Data.CommandType.StoredProcedure;
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageNumber", page));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageSize", pageSize));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Department", (object?)department ?? DBNull.Value));
            command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@Status", string.IsNullOrEmpty(status) ? DBNull.Value : (object?)status));

            await _db.Database.OpenConnectionAsync(ct);
            using (var reader = await command.ExecuteReaderAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    items.Add(new TeacherListItemDto
                    {
                        Id = reader.GetInt32(0),
                        TeacherNo = reader.GetString(1),
                        FullName = reader.GetString(2),
                        Designation = reader.GetString(3),
                        Department = reader.IsDBNull(4) ? null : reader.GetString(4),
                        MobileNumber = reader.GetString(5),
                        Status = ((TeacherStatus)reader.GetInt32(6)).ToString(),
                        ProfilePicturePath = reader.IsDBNull(7) ? null : reader.GetString(7)
                    });
                    totalCount = reader.IsDBNull(8) ? 0 : reader.GetInt32(8);
                }
            }
            await _db.Database.CloseConnectionAsync();
        }

        return new PagedResult<TeacherListItemDto>
        {
            Items = items,
            TotalItems = totalCount,
            Page = page,
            PageSize = pageSize
        };
    }

    public async Task DeactivateAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct)
            ?? throw new Exception("Teacher not found");

        teacher.Status = TeacherStatus.Inactive;
        teacher.UpdatedBy = updatedBy;
        teacher.UpdatedAt = DateTime.UtcNow;

        if (teacher.UserId.HasValue)
        {
            var user = await _db.Users.FindAsync(new object[] { teacher.UserId.Value }, ct);
            if (user != null)
            {
                user.Status = AccountStatus.Inactive;
                user.UpdatedBy = updatedBy;
                user.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    public async Task ActivateAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var teacher = await _db.Teachers.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct)
            ?? throw new Exception("Teacher not found");

        teacher.Status = TeacherStatus.Active;
        teacher.UpdatedBy = updatedBy;
        teacher.UpdatedAt = DateTime.UtcNow;

        if (teacher.UserId.HasValue)
        {
            var user = await _db.Users.FindAsync(new object[] { teacher.UserId.Value }, ct);
            if (user != null)
            {
                user.Status = AccountStatus.Active;
                user.UpdatedBy = updatedBy;
                user.UpdatedAt = DateTime.UtcNow;
            }
        }

        await _db.SaveChangesAsync(ct);
    }

    private async Task<string> GenerateTeacherNoAsync(CancellationToken ct)
    {
        // Find the highest existing teacher number to avoid collisions
        var lastTeacherNo = await _db.Teachers
            .Where(t => t.TeacherNo.StartsWith("T-"))
            .OrderByDescending(t => t.TeacherNo)
            .Select(t => t.TeacherNo)
            .FirstOrDefaultAsync(ct);

        int nextNum = 1;
        if (!string.IsNullOrEmpty(lastTeacherNo) && lastTeacherNo.Length >= 6)
        {
            if (int.TryParse(lastTeacherNo.Substring(2), out int lastNum))
            {
                nextNum = lastNum + 1;
            }
        }

        return $"T-{nextNum:D4}";
    }

    private async Task<string> GenerateUserNameAsync(string fullName, CancellationToken ct)
    {
        // Remove special characters and clean up the name for a valid username
        var cleanedName = new string(fullName.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
        var baseUserName = cleanedName.ToLower().Trim().Replace(" ", ".");
        
        // Avoid double dots if they were already there
        while (baseUserName.Contains("..")) baseUserName = baseUserName.Replace("..", ".");
        
        var userName = baseUserName;
        var suffix = 1;

        while (await _db.Users.AnyAsync(u => u.UserName == userName, ct))
        {
            userName = $"{baseUserName}{suffix++}";
        }

        return userName;
    }

    private string GenerateRandomPassword()
    {
        return Guid.NewGuid().ToString("N").Substring(0, 8);
    }
}
