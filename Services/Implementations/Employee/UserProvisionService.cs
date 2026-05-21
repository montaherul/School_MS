using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Employee;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System.Security.Cryptography;

namespace SchoolManagementSystem.Services.Implementations.Employee;

public class UserProvisionService : IUserProvisionService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly IPasswordHashService _passwordHashService;

    public UserProvisionService(IUnitOfWork unitOfWork, IPasswordHashService passwordHashService)
    {
        _unitOfWork = unitOfWork;
        _passwordHashService = passwordHashService;
    }

    public async Task<(int userId, string username, string password)> ProvisionUserForEmployeeAsync(
        SchoolManagementSystem.Models.Entities.Employee.Employee employee, CancellationToken ct)
    {
        var userRepo = _unitOfWork.Repository<ApplicationUser>();
        var roleRepo = _unitOfWork.Repository<Role>();
        var userRoleRepo = _unitOfWork.Repository<UserRole>();
        var mappingRepo = _unitOfWork.Repository<DesignationRoleMapping>();

        // 0. Check if designation requires login
        var designation = await _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Designation>()
            .GetByIdAsync(employee.DesignationId, ct);

        if (designation == null || !designation.RequiresLogin)
        {
            return (0, string.Empty, string.Empty); // No user created
        }

        // 1. Generate standard username: e.g. emp1023 (lowercased employee code without separators)
        var username = employee.EmployeeCode.Replace("-", "").Replace("_", "").ToLower();
        
        // Ensure username uniqueness
        var count = 1;
        var finalUsername = username;
        while (await userRepo.AnyAsync(u => u.UserName == finalUsername && !u.IsDeleted, ct))
        {
            finalUsername = $"{username}{count++}";
        }

        // 2. Generate secure random password
        var password = GenerateRandomPassword();
        var passwordHash = _passwordHashService.HashPassword(password);

        // 3. Create ApplicationUser
        var user = new ApplicationUser
        {
            UserName = finalUsername,
            Email = employee.Email ?? $"{finalUsername}@school.local",
            PhoneNumber = employee.Phone,
            Status = AccountStatus.Active,
            PasswordHash = passwordHash,
            IsEmailConfirmed = true,
            MustChangePassword = true, // Force change on first login
            EmployeeId = employee.Id,
            CreatedAt = DateTime.UtcNow,
            CreatedBy = "System-Provision"
        };

        await userRepo.AddAsync(user, ct);
        await _unitOfWork.SaveChangesAsync(ct);

        // 4. Determine and Assign Role based on DesignationRoleMapping
        int assignedRoleId = 6; // Default fallback role (e.g., Office Staff)

        var mapping = await mappingRepo.Query()
            .FirstOrDefaultAsync(m => m.DesignationId == employee.DesignationId && m.IsActive, ct);

        if (mapping != null)
        {
            assignedRoleId = mapping.RoleId;
        }
        else
        {
            // Fallback to name matching if no database mapping exists
            var matchedRole = await roleRepo.FirstOrDefaultAsync(r => r.Name.ToLower() == designation.Name.ToLower() && !r.IsDeleted, ct);
            if (matchedRole != null)
            {
                assignedRoleId = matchedRole.Id;
            }
        }

        // Add to UserRole
        await userRoleRepo.AddAsync(new UserRole
        {
            UserId = user.Id,
            RoleId = assignedRoleId
        }, ct);

        await _unitOfWork.SaveChangesAsync(ct);

        return (user.Id, finalUsername, password);
    }

    private static string GenerateRandomPassword()
    {
        const string upperCase = "ABCDEFGHJKLMNOPQRSTUVWXYZ";
        const string lowerCase = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string specials = "!@$?_-";

        var charSet = upperCase + lowerCase + digits + specials;
        var chars = new char[10];
        
        using (var rng = RandomNumberGenerator.Create())
        {
            var bytes = new byte[10];
            rng.GetBytes(bytes);

            // Ensure we have at least one character from each set
            chars[0] = upperCase[bytes[0] % upperCase.Length];
            chars[1] = lowerCase[bytes[1] % lowerCase.Length];
            chars[2] = digits[bytes[2] % digits.Length];
            chars[3] = specials[bytes[3] % specials.Length];

            for (int i = 4; i < 10; i++)
            {
                chars[i] = charSet[bytes[i] % charSet.Length];
            }

            // Shuffle the characters
            for (int i = chars.Length - 1; i > 0; i--)
            {
                int j = bytes[i] % (i + 1);
                var temp = chars[i];
                chars[i] = chars[j];
                chars[j] = temp;
            }
        }

        return new string(chars);
    }
}
