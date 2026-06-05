using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Guardian;
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Guardian;
using SchoolManagementSystem.Services.Guardian;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Guardian;

public class GuardianService : IGuardianService
{
    private readonly IUnitOfWork _uow;
    private readonly IGuardianRepository _guardianRepo;

    public GuardianService(IUnitOfWork uow, IGuardianRepository guardianRepo)
    {
        _uow = uow;
        _guardianRepo = guardianRepo;
    }

    public async Task<(IEnumerable<GuardianListItemDto> Items, int TotalCount)> GetGuardianListAsync(string? searchTerm, string? status, int pageNumber, int pageSize)
    {
        return await _guardianRepo.GetListAsync(searchTerm, status, pageNumber, pageSize);
    }

    public async Task<GuardianDetailsDto?> GetGuardianByIdAsync(int id)
    {
        return await _guardianRepo.GetDetailsAsync(id);
    }

    public async Task<int> CreateGuardianAsync(GuardianUpsertDto dto)
    {
        string guardianCode = await GenerateGuardianCode();
        
        var guardian = new SchoolManagementSystem.Models.Entities.Guardian.Guardian
        {
            GuardianCode = guardianCode,
            FirstName = dto.FirstName,
            LastName = dto.LastName,
            FullName = BuildFullName(dto.FirstName, dto.LastName),
            Gender = dto.Gender,
            RelationType = dto.RelationType,
            MobileNumber = dto.MobileNumber,
            Email = dto.Email,
            NationalId = dto.NationalId,
            Occupation = dto.Occupation,
            PresentAddress = dto.PresentAddress,
            PermanentAddress = dto.PermanentAddress,
            PortalAccessEnabled = dto.PortalAccessEnabled,
            Status = GuardianStatus.Active
        };

        await _guardianRepo.AddAsync(guardian);
        await _uow.SaveChangesAsync();

        return guardian.Id;
    }

    public async Task UpdateGuardianAsync(GuardianUpsertDto dto)
    {
        var guardian = await _guardianRepo.GetByIdAsync(dto.Id);
        if (guardian == null) throw new KeyNotFoundException("Guardian not found");

        guardian.FirstName = dto.FirstName;
        guardian.LastName = dto.LastName;
        guardian.FullName = BuildFullName(dto.FirstName, dto.LastName);
        guardian.Gender = dto.Gender;
        guardian.RelationType = dto.RelationType;
        guardian.MobileNumber = dto.MobileNumber;
        guardian.Email = dto.Email;
        guardian.NationalId = dto.NationalId;
        guardian.Occupation = dto.Occupation;
        guardian.PresentAddress = dto.PresentAddress;
        guardian.PermanentAddress = dto.PermanentAddress;
        guardian.PortalAccessEnabled = dto.PortalAccessEnabled;

        _guardianRepo.Update(guardian);
        await _uow.SaveChangesAsync();
    }

    public async Task DeleteGuardianAsync(int id)
    {
        var guardian = await _guardianRepo.GetByIdAsync(id);
        if (guardian != null)
        {
            _guardianRepo.Remove(guardian);
            await _uow.SaveChangesAsync();
        }
    }

    public async Task SetGuardianStatusAsync(int id, bool active)
    {
        var guardian = await _guardianRepo.GetByIdAsync(id);
        if (guardian == null) throw new KeyNotFoundException("Guardian not found");

        guardian.Status = active ? GuardianStatus.Active : GuardianStatus.Inactive;
        guardian.PortalAccessEnabled = active;
        _guardianRepo.Update(guardian);
        await _uow.SaveChangesAsync();
    }

    public async Task LinkStudentAsync(int guardianId, int studentId, string relation)
    {
        await _guardianRepo.LinkStudentAsync(guardianId, studentId, relation);
        await _uow.SaveChangesAsync();
    }

    public async Task<GuardianDashboardDataDto> GetDashboardAsync(int guardianId)
    {
        return await _guardianRepo.GetDashboardDataAsync(guardianId);
    }

    public async Task<GuardianDashboardDataDto> GetDashboardByUserIdAsync(int userId)
    {
        var guardian = await _guardianRepo.Query()
            .FirstOrDefaultAsync(g => g.UserId == userId);
            
        if (guardian == null) throw new KeyNotFoundException("Guardian profile not found for this user.");
        
        return await GetDashboardAsync(guardian.Id);
    }

    private async Task<string> GenerateGuardianCode()
    {
        var lastCode = await _guardianRepo.Query()
            .OrderByDescending(g => g.GuardianCode)
            .Select(g => g.GuardianCode)
            .FirstOrDefaultAsync();

        int nextNum = 1;
        if (lastCode != null && lastCode.StartsWith("GRD-"))
        {
            if (int.TryParse(lastCode.Substring(4), out int lastNum))
            {
                nextNum = lastNum + 1;
            }
        }

        return $"GRD-{nextNum:D5}";
    }

    private static string BuildFullName(string firstName, string lastName)
    {
        return string.Join(' ', new[] { firstName, lastName }.Where(x => !string.IsNullOrWhiteSpace(x))).Trim();
    }
}
