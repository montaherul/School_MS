using System.Text.Encodings.Web;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Entities.System;
using SchoolManagementSystem.Services.Interfaces.Email;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using StudentEntity = SchoolManagementSystem.Models.Entities.Student.Student;

namespace SchoolManagementSystem.Services.Implementations.Attendance;

public class AttendanceNotificationService : IAttendanceNotificationService
{
    private const string AbsentNotificationType = "Absent";
    private const string LateStudentNotificationType = "LateStudent";
    private const string LateEmployeeNotificationType = "LateEmployee";
    private const string Channel = "Email";

    private readonly IUnitOfWork _uow;
    private readonly IEmailService _emailService;
    private readonly ILogger<AttendanceNotificationService> _logger;

    public AttendanceNotificationService(IUnitOfWork uow, IEmailService emailService, ILogger<AttendanceNotificationService> logger)
    {
        _uow = uow;
        _emailService = emailService;
        _logger = logger;
    }

    public async Task SendAbsentNotificationsAsync(IEnumerable<int> studentIds, DateOnly attendanceDate, string createdBy, CancellationToken ct = default)
    {
        foreach (var studentId in studentIds.Distinct())
        {
            await SendAbsentNotificationAsync(studentId, attendanceDate, createdBy, ct);
        }
    }

    public async Task SendLateStudentNotificationsAsync(IEnumerable<int> studentIds, DateOnly attendanceDate, string createdBy, CancellationToken ct = default)
    {
        foreach (var studentId in studentIds.Distinct())
        {
            await QueueStudentNotificationAsync(studentId, attendanceDate, LateStudentNotificationType, createdBy, ct);
        }
    }

    public async Task SendLateEmployeeNotificationsAsync(IEnumerable<int> employeeIds, DateOnly attendanceDate, string createdBy, CancellationToken ct = default)
    {
        foreach (var employeeId in employeeIds.Distinct())
        {
            await QueueEmployeeNotificationAsync(employeeId, attendanceDate, LateEmployeeNotificationType, createdBy, ct);
        }

        await _uow.SaveChangesAsync(ct);
    }

    public async Task SendAbsentNotificationAsync(int studentId, DateOnly attendanceDate, string createdBy, CancellationToken ct = default)
        => await QueueStudentNotificationAsync(studentId, attendanceDate, AbsentNotificationType, createdBy, ct);

    private async Task QueueStudentNotificationAsync(int studentId, DateOnly attendanceDate, string notificationType, string createdBy, CancellationToken ct)
    {
        var student = await _uow.Repository<StudentEntity>().Query()
            .AsNoTracking()
            .Include(s => s.StudentGuardians)
                .ThenInclude(sg => sg.Guardian)
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted, ct);

        if (student is null)
        {
            return;
        }

        var guardiansToNotify = student.StudentGuardians
            .Where(sg => sg.ReceivesAttendanceNotifications && sg.Guardian != null && !sg.Guardian.IsDeleted)
            .ToList();

        // If no guardians opted in, fall back to student email if available? 
        // User didn't specify, but for now let's focus on guardians.

        foreach (var mapping in guardiansToNotify)
        {
            if (mapping.ReceivesEmail && !string.IsNullOrWhiteSpace(mapping.Guardian!.Email))
            {
                await EnqueueStudentLog(studentId, mapping.GuardianId, mapping.Guardian.Email, attendanceDate, notificationType, "Email", createdBy, ct);
            }
            
            if (mapping.ReceivesSMS && !string.IsNullOrWhiteSpace(mapping.Guardian.MobileNumber))
            {
                await EnqueueStudentLog(studentId, mapping.GuardianId, mapping.Guardian.MobileNumber, attendanceDate, notificationType, "SMS", createdBy, ct);
            }
        }

        await _uow.SaveChangesAsync(ct);
    }

    private async Task QueueEmployeeNotificationAsync(int employeeId, DateOnly attendanceDate, string notificationType, string createdBy, CancellationToken ct)
    {
        var employee = await _uow.Repository<SchoolManagementSystem.Models.Entities.Employee.Employee>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == employeeId && !e.IsDeleted, ct);

        if (employee == null || string.IsNullOrWhiteSpace(employee.Email))
        {
            return;
        }

        await EnqueueEmployeeLog(employeeId, employee.Email, attendanceDate, notificationType, Channel, createdBy, ct);
    }

    private async Task EnqueueStudentLog(int studentId, int guardianId, string target, DateOnly date, string notificationType, string channel, string createdBy, CancellationToken ct)
    {
        var logRepo = _uow.Repository<AttendanceNotificationLog>();
        
        var log = await logRepo.FirstOrDefaultAsync(x =>
            x.StudentId == studentId &&
            x.GuardianId == guardianId &&
            x.AttendanceDate == date &&
            x.NotificationType == notificationType &&
            x.NotificationChannel == channel &&
            !x.IsDeleted, ct);

        if (log is null)
        {
            log = new AttendanceNotificationLog
            {
                StudentId = studentId,
                GuardianId = guardianId,
                AttendanceDate = date,
                NotificationType = notificationType,
                NotificationChannel = channel,
                NotificationStatus = "Queued",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                Email = target,
            };
            await logRepo.AddAsync(log, ct);
        }
        else if (!log.IsSent && log.NotificationStatus != "Queued")
        {
            log.NotificationStatus = "Queued";
            log.UpdatedAt = DateTime.UtcNow;
            log.UpdatedBy = createdBy;
            logRepo.Update(log);
        }
    }

    private async Task EnqueueEmployeeLog(int employeeId, string target, DateOnly date, string notificationType, string channel, string createdBy, CancellationToken ct)
    {
        var logRepo = _uow.Repository<AttendanceNotificationLog>();

        var log = await logRepo.FirstOrDefaultAsync(x =>
            x.EmployeeId == employeeId &&
            x.AttendanceDate == date &&
            x.NotificationType == notificationType &&
            x.NotificationChannel == channel &&
            !x.IsDeleted, ct);

        if (log is null)
        {
            log = new AttendanceNotificationLog
            {
                StudentId = 0,
                EmployeeId = employeeId,
                AttendanceDate = date,
                NotificationType = notificationType,
                NotificationChannel = channel,
                NotificationStatus = "Queued",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy,
                Email = target,
            };
            await logRepo.AddAsync(log, ct);
        }
        else if (!log.IsSent && log.NotificationStatus != "Queued")
        {
            log.NotificationStatus = "Queued";
            log.UpdatedAt = DateTime.UtcNow;
            log.UpdatedBy = createdBy;
            logRepo.Update(log);
        }
    }

    public async Task<IReadOnlyList<AttendanceNotificationLog>> GetLogsAsync(DateOnly attendanceDate, int? classId = null, int? sectionId = null, CancellationToken ct = default)
    {
        var query = _uow.Repository<AttendanceNotificationLog>().Query()
            .AsNoTracking()
            .Include(x => x.Student)
            .Where(x => x.AttendanceDate == attendanceDate && !x.IsDeleted);

        if (classId.HasValue)
        {
            query = query.Where(x => x.Student != null && x.Student.ClassId == classId.Value);
        }

        if (sectionId.HasValue)
        {
            query = query.Where(x => x.Student != null && x.Student.SectionId == sectionId.Value);
        }

        return await query.OrderByDescending(x => x.Id).ToListAsync(ct);
    }

}
