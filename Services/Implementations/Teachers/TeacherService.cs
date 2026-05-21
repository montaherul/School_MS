using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Helpers.Security;
using SchoolManagementSystem.Models.DTOs.Common;
using SchoolManagementSystem.Models.DTOs.Teacher;
using SchoolManagementSystem.Models.Entities.Auth;
using SchoolManagementSystem.Models.Entities.System;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Repositories.Interfaces.Auth;
using System.Data;
using SchoolManagementSystem.Repositories.Interfaces.Teachers;

namespace SchoolManagementSystem.Services.Implementations.Teachers;

public class TeacherService : ITeacherService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ITeacherRepository _teacherRepository;
    private readonly IUserRepository _userRepository;
    private readonly IRoleRepository _roleRepository;
    private readonly IUserRoleRepository _userRoleRepository;
    private readonly IPasswordHashService _passwordHashService;
    private readonly IEmailService _emailService;

    public TeacherService(
        IUnitOfWork unitOfWork,
        ITeacherRepository teacherRepository,
        IUserRepository userRepository,
        IRoleRepository roleRepository,
        IUserRoleRepository userRoleRepository,
        IPasswordHashService passwordHashService,
        IEmailService emailService)
    {
        _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        _teacherRepository = teacherRepository ?? throw new ArgumentNullException(nameof(teacherRepository));
        _userRepository = userRepository;
        _roleRepository = roleRepository;
        _userRoleRepository = userRoleRepository;
        _passwordHashService = passwordHashService ?? throw new ArgumentNullException(nameof(passwordHashService));
        _emailService = emailService ?? throw new ArgumentNullException(nameof(emailService));
    }

    public async Task<int> CreateAsync(TeacherUpsertDto dto, string createdBy, CancellationToken ct = default)
    {
        throw new InvalidOperationException("Workforce Architecture Update: Teachers can no longer be created manually as isolated entities. Please navigate to the Employee module to onboard a new staff member and assign them a teaching designation. The system will automatically synchronize their academic teaching profile.");
    }

    public async Task UpdateAsync(TeacherUpsertDto dto, string updatedBy, CancellationToken ct = default)
    {
        var teacher = await _teacherRepository.FirstOrDefaultAsync(t => t.Id == dto.Id && !t.IsDeleted, ct)
            ?? throw new Exception("Teacher not found");

        // We only update the academic extension fields, personal workforce fields are managed in Employee module.
        if (!string.IsNullOrEmpty(dto.TeacherNo))
        {
            teacher.TeacherCode = dto.TeacherNo;
        }
        teacher.SubjectSpecialization = dto.Specialization;

        teacher.UpdatedBy = updatedBy;
        teacher.UpdatedAt = DateTime.UtcNow;

        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var teacher = await _teacherRepository.FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct) ?? throw new Exception("Teacher not found");
        teacher.IsDeleted = true;
        teacher.UpdatedBy = updatedBy;
        teacher.UpdatedAt = DateTime.UtcNow;

        if (teacher.UserId.HasValue)
        {
            var user = await _userRepository.FirstOrDefaultAsync(u => u.Id == teacher.UserId.Value, ct);
            if (user != null)
            {
                user.Status = AccountStatus.Inactive;
                user.UpdatedBy = updatedBy;
                user.UpdatedAt = DateTime.UtcNow;
            }
        }
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<TeacherUpsertDto?> GetForEditAsync(int id, CancellationToken ct = default)
    {
        return await _teacherRepository.GetForEditAsync(id, ct);
    }

    public async Task<PagedResult<TeacherListItemDto>> GetPagedAsync(int page, int pageSize, string? search, string? department, string? status, CancellationToken ct = default)
    {
        var (items, totalItems) = await _teacherRepository.GetPagedAsync(page, pageSize, search, department, status, ct);
        return new PagedResult<TeacherListItemDto> { Items = items, TotalItems = totalItems, Page = page, PageSize = pageSize };
    }

    public async Task DeactivateAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var teacher = await _teacherRepository.Query().Include(t => t.Employee).FirstOrDefaultAsync(t => t.Id == id && !t.IsDeleted, ct) ?? throw new Exception("Teacher not found");
        teacher.IsDeleted = true; // Soft delete the academic metadata link
        teacher.UpdatedBy = updatedBy; 
        teacher.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task ActivateAsync(int id, string updatedBy, CancellationToken ct = default)
    {
        var teacher = await _teacherRepository.Query().Include(t => t.Employee).FirstOrDefaultAsync(t => t.Id == id, ct) ?? throw new Exception("Teacher not found");
        teacher.IsDeleted = false; // Restore the academic metadata link
        teacher.UpdatedBy = updatedBy; 
        teacher.UpdatedAt = DateTime.UtcNow;
        await _unitOfWork.SaveChangesAsync(ct);
    }

    public async Task<TeacherUpsertDto?> GetByUserIdAsync(int userId, CancellationToken ct = default)
    {
        return await _teacherRepository.GetByUserIdAsync(userId, ct);
    }

    private async Task<string> SaveFileAsync(IFormFile file, string subFolder, CancellationToken ct)
    {
        var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", subFolder);
        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);
        var fileName = Guid.NewGuid() + Path.GetExtension(file.FileName);
        var filePath = Path.Combine(folderPath, fileName);
        using var stream = new FileStream(filePath, FileMode.Create);
        await file.CopyToAsync(stream, ct);
        return $"/uploads/{subFolder}/{fileName}";
    }

    private void DeleteFile(string? relativePath)
    {
        if (string.IsNullOrEmpty(relativePath)) return;
        var fullPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", relativePath.TrimStart('/'));
        if (File.Exists(fullPath)) File.Delete(fullPath);
    }

    private async Task<string> GenerateTeacherNoAsync(CancellationToken ct)
    {
        var lastTeacherNo = await _unitOfWork.Repository<Teacher>().Query()
            .Where(t => t.TeacherNo.StartsWith("T-"))
            .OrderByDescending(t => t.TeacherNo)
            .Select(t => t.TeacherNo)
            .FirstOrDefaultAsync(ct);
        int nextNum = 1;
        if (!string.IsNullOrEmpty(lastTeacherNo) && lastTeacherNo.Length >= 6)
        {
            if (int.TryParse(lastTeacherNo.Substring(2), out int lastNum)) nextNum = lastNum + 1;
        }
        return $"T-{nextNum:D4}";
    }

    private async Task<string> GenerateUserNameAsync(string fullName, CancellationToken ct)
    {
        var cleanedName = new string(fullName.Where(c => char.IsLetterOrDigit(c) || char.IsWhiteSpace(c)).ToArray());
        var baseUserName = cleanedName.ToLower().Trim().Replace(" ", ".");
        while (baseUserName.Contains("..")) baseUserName = baseUserName.Replace("..", ".");
        var userName = baseUserName;
        var suffix = 1;
        while (await _userRepository.AnyAsync(u => u.UserName == userName, ct)) userName = $"{baseUserName}{suffix++}";
        return userName;
    }

    private string GenerateRandomPassword() => Guid.NewGuid().ToString("N").Substring(0, 8);
}

