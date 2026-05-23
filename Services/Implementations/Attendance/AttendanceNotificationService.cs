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
    private const string NotificationType = "Absent";
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

    public async Task SendAbsentNotificationAsync(int studentId, DateOnly attendanceDate, string createdBy, CancellationToken ct = default)
    {
        var logRepo = _uow.Repository<AttendanceNotificationLog>();
        var existingSent = await logRepo.AnyAsync(x =>
            x.StudentId == studentId &&
            x.AttendanceDate == attendanceDate &&
            x.NotificationType == NotificationType &&
            x.NotificationChannel == Channel &&
            x.IsSent &&
            !x.IsDeleted, ct);

        if (existingSent)
        {
            return;
        }

        var student = await _uow.Repository<Student>().Query()
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .Include(s => s.Guardians)
            .FirstOrDefaultAsync(s => s.Id == studentId && !s.IsDeleted, ct);

        if (student is null)
        {
            return;
        }

        var email = student.Guardians
            .Select(g => g.Email)
            .FirstOrDefault(e => !string.IsNullOrWhiteSpace(e))
            ?? student.EmailAddress;

        var log = await logRepo.FirstOrDefaultAsync(x =>
            x.StudentId == studentId &&
            x.AttendanceDate == attendanceDate &&
            x.NotificationType == NotificationType &&
            x.NotificationChannel == Channel &&
            !x.IsDeleted, ct);

        if (log is null)
        {
            log = new AttendanceNotificationLog
            {
                StudentId = studentId,
                AttendanceDate = attendanceDate,
                NotificationType = NotificationType,
                NotificationChannel = Channel,
                NotificationStatus = "Pending",
                CreatedAt = DateTime.UtcNow,
                CreatedBy = createdBy
            };
            await logRepo.AddAsync(log, ct);
        }

        log.Email = email ?? string.Empty;

        if (string.IsNullOrWhiteSpace(email))
        {
            log.IsSent = false;
            log.NotificationStatus = "Failed";
            log.ErrorMessage = "Guardian email is missing.";
            log.UpdatedAt = DateTime.UtcNow;
            log.UpdatedBy = createdBy;
            await _uow.SaveChangesAsync(ct);
            return;
        }

        var schoolName = await _uow.Repository<SchoolProfile>().Query()
            .AsNoTracking()
            .Select(s => s.Name)
            .FirstOrDefaultAsync(ct) ?? "School";

        try
        {
            await _emailService.SendAttendanceNotificationAsync(
                email,
                student.FullName,
                student.RollNumber.ToString(),
                student.Class?.Name ?? string.Empty,
                student.Section?.Name ?? string.Empty,
                attendanceDate,
                schoolName,
                ct);

            log.IsSent = true;
            log.SentAt = DateTime.UtcNow;
            log.NotificationStatus = "Sent";
            log.ErrorMessage = null;
            log.UpdatedAt = DateTime.UtcNow;
            log.UpdatedBy = createdBy;
            await _uow.SaveChangesAsync(ct);
        }
        catch (Exception ex)
        {
            log.IsSent = false;
            log.NotificationStatus = "Failed";
            log.ErrorMessage = ex.ToString();
            log.UpdatedAt = DateTime.UtcNow;
            log.UpdatedBy = createdBy;
            await _uow.SaveChangesAsync(ct);
            _logger.LogError(ex, "Attendance email failed for student {StudentId} on {AttendanceDate}", studentId, attendanceDate);
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
