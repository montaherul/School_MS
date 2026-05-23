using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Entities.Teachers;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace SchoolManagementSystem.Services.Implementations.Teachers;

public class TeacherSynchronizationService : ITeacherSynchronizationService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<TeacherSynchronizationService> _logger;

    public TeacherSynchronizationService(IUnitOfWork unitOfWork, ILogger<TeacherSynchronizationService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    public async Task SyncEmployeeToTeacherAsync(int employeeId, CancellationToken ct = default)
    {
        try
        {
            var employeeRepo = _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>();
            var teacherRepo = _unitOfWork.Repository<Teacher>();

            var employee = await employeeRepo.Query()
                .Include(e => e.Designation)
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted, ct);

            if (employee == null)
            {
                _logger.LogWarning("Employee with ID {EmployeeId} not found or deleted. Skipping synchronization.", employeeId);
                return;
            }

            bool isTeaching = employee.IsTeachingStaff || (employee.Designation != null && employee.Designation.IsTeachingRole);

            var teacher = await teacherRepo.Query()
                .FirstOrDefaultAsync(t => t.EmployeeId == employee.Id, ct);

            if (isTeaching)
            {
                if (teacher == null)
                {
                    _logger.LogInformation("Creating synchronized Teacher profile for teaching employee: {Name} ({Code})", employee.FullName, employee.EmployeeCode);

                    teacher = new Teacher
                    {
                        EmployeeId = employee.Id,
                        TeacherCode = await GenerateTeacherCodeAsync(ct),
                        SubjectSpecialization = "General",
                        TeachingLevel = "Secondary",
                        IsClassTeacher = false,
                        IsExamController = false,
                        IsRoutineCoordinator = false,
                        TeachingExperienceYears = 0,
                        Remarks = "Auto-synchronized from Employee workforce onboarding",
                        CreatedBy = "system-sync",
                        CreatedAt = DateTime.UtcNow,
                        IsDeleted = false
                    };

                    await teacherRepo.AddAsync(teacher, ct);
                }
                else
                {
                    _logger.LogInformation("Updating synchronized Teacher profile for teaching employee: {Name} ({Code})", employee.FullName, employee.EmployeeCode);

                    if (string.IsNullOrWhiteSpace(teacher.TeacherCode) || !teacher.TeacherCode.StartsWith("T-", StringComparison.OrdinalIgnoreCase))
                    {
                        teacher.TeacherCode = await GenerateTeacherCodeAsync(ct);
                    }

                    teacher.IsDeleted = false; // Restore if it was soft-deleted
                    teacher.UpdatedBy = "system-sync";
                    teacher.UpdatedAt = DateTime.UtcNow;
                }
            }
            else
            {
                if (teacher != null)
                {
                    _logger.LogInformation("Employee {Name} ({Code}) is no longer marked as teaching staff. Deactivating academic Teacher role.", employee.FullName, employee.EmployeeCode);
                    teacher.Remarks = "Deactivated - Employee reassigned to non-teaching workforce";
                    teacher.IsDeleted = true; // Safe soft-delete to hide from active lists while preserving db relationships
                    teacher.UpdatedBy = "system-sync";
                    teacher.UpdatedAt = DateTime.UtcNow;
                }
            }

            await _unitOfWork.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to synchronize Employee ID {EmployeeId} to Teacher record.", employeeId);
            throw;
        }
    }

    private async Task<string> GenerateTeacherCodeAsync(CancellationToken ct)
    {
        var prefix = $"T-{DateTime.UtcNow.Year}-";
        var lastCode = await _unitOfWork.Repository<Teacher>().Query()
            .Where(t => t.TeacherCode.StartsWith(prefix))
            .OrderByDescending(t => t.TeacherCode)
            .Select(t => t.TeacherCode)
            .FirstOrDefaultAsync(ct);

        var nextNumber = 1;
        if (!string.IsNullOrEmpty(lastCode) && lastCode.Length > prefix.Length && int.TryParse(lastCode.Substring(prefix.Length), out var lastNumber))
        {
            nextNumber = lastNumber + 1;
        }

        return $"{prefix}{nextNumber:D4}";
    }

    public async Task SyncAllTeachingEmployeesAsync(CancellationToken ct = default)
    {
        try
        {
            _logger.LogInformation("Initiating full workforce sync for all teaching employees...");
            var employeeRepo = _unitOfWork.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>();
            
            var teachingEmployees = await employeeRepo.Query()
                .Include(e => e.Designation)
                .Where(e => !e.IsDeleted && (e.IsTeachingStaff || (e.Designation != null && e.Designation.IsTeachingRole)))
                .Select(e => e.Id)
                .ToListAsync(ct);

            foreach (var empId in teachingEmployees)
            {
                await SyncEmployeeToTeacherAsync(empId, ct);
            }

            _logger.LogInformation("Full workforce teaching sync completed successfully for {Count} profiles.", teachingEmployees.Count);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Critical failure during bulk teaching workforce sync.");
            throw;
        }
    }
}
