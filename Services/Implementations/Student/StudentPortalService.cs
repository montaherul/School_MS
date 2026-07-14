using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Dashboard;
using SchoolManagementSystem.Models.DTOs.Student;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Entities.Communication;
using SchoolManagementSystem.Models.Entities.Fees;
using SchoolManagementSystem.Models.Entities.Notification;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Entities.Student;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Website;
using SchoolManagementSystem.Services.Interfaces.Student;
using SchoolManagementSystem.UnitOfWork.Interfaces;

namespace SchoolManagementSystem.Services.Implementations.Student;

public class StudentPortalService : IStudentPortalService
{
    private readonly IUnitOfWork _uow;
    private readonly ISchoolSettingRepository _settingRepo;
    private readonly ILogger<StudentPortalService> _logger;

    public StudentPortalService(
        IUnitOfWork uow,
        ISchoolSettingRepository settingRepo,
        ILogger<StudentPortalService> logger)
    {
        _uow = uow;
        _settingRepo = settingRepo;
        _logger = logger;
    }

    public async Task<StudentPortalDashboardDto> GetDashboardAsync(int userId, CancellationToken ct = default)
    {
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);

        if (student == null)
            return new StudentPortalDashboardDto();

        var dto = new StudentPortalDashboardDto
        {
            StudentName = student.FullName,
            ClassName = student.Class?.Name ?? "",
            SectionName = student.Section?.Name ?? "",
            RollNumber = student.RollNumber,
            ProfilePicturePath = student.ProfilePicturePath
        };

        // Attendance
        var from = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        var to = from.AddMonths(1).AddDays(-1);
        var attendanceRecords = await _uow.Repository<StudentAttendance>().Query()
            .AsNoTracking()
            .Where(a => a.StudentId == student.Id && a.AttendanceDate >= from && a.AttendanceDate <= to)
            .ToListAsync(ct);

        dto.PresentCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Present);
        dto.AbsentCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Absent);
        dto.LateCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Late);
        dto.LeaveCount = attendanceRecords.Count(a => a.Status == AttendanceStatus.Leave);
        var totalDays = attendanceRecords.Count;
        dto.AttendancePercentage = totalDays > 0 ? (double)(totalDays - dto.AbsentCount) / totalDays * 100 : 0;

        // Finance
        var settings = await _settingRepo.GetCurrentSettingsAsync(ct);
        var invoices = await _uow.Repository<FeeInvoice>().Query()
            .AsNoTracking()
            .Where(i => i.StudentId == student.Id && !i.IsDeleted)
            .ToListAsync(ct);

        dto.InvoiceCount = invoices.Count;
        dto.TotalInvoiced = invoices.Sum(i => i.TotalAmount + i.LateFee);
        dto.TotalPaid = invoices.Sum(i => i.PaidAmount);
        dto.OutstandingFees = invoices.Where(i => i.Status != PaymentStatus.Paid).Sum(i => i.TotalAmount - i.PaidAmount);
        dto.TotalDue = dto.TotalInvoiced - dto.TotalPaid;

        dto.IsResultBlocked = !(settings?.AllowResultWithDue == true) && dto.OutstandingFees > 0;

        // Latest result
        if (!dto.IsResultBlocked)
        {
            var latestResult = await _uow.Repository<StudentExamResult>().Query()
                .AsNoTracking()
                .Where(r => r.StudentId == student.Id && !r.IsDeleted)
                .OrderByDescending(r => r.ExamId)
                .FirstOrDefaultAsync(ct);

            if (latestResult != null)
            {
                dto.LatestGPA = latestResult.Gpa;
                dto.LatestGrade = latestResult.Grade ?? string.Empty;
                dto.LatestPassed = latestResult.IsPassed;
            }
        }

        // Leave counts
        dto.LeaveApplicationCount = await _uow.Repository<StudentLeaveApplication>().Query()
            .CountAsync(l => l.StudentId == student.Id, ct);
        dto.PendingLeaveCount = await _uow.Repository<StudentLeaveApplication>().Query()
            .CountAsync(l => l.StudentId == student.Id
                && l.ApprovalStatus == StudentLeaveApplication.ApprovalStatusEnum.Pending, ct);

        // Notification count
        dto.UnreadNotificationCount = await _uow.Repository<NotificationMessage>().Query()
            .CountAsync(n => n.UserId == userId && !n.IsRead && !n.IsDeleted, ct);

        return dto;
    }

    public async Task<StudentProfileDto?> GetProfileAsync(int userId, CancellationToken ct = default)
    {
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .Include(s => s.Class)
            .Include(s => s.Section)
            .Include(s => s.StudentGroup)
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);

        if (student == null) return null;

        return new StudentProfileDto
        {
            Id = student.Id,
            StudentNo = student.StudentNo,
            FullName = student.FullName,
            FullNameBangla = student.FullNameBangla,
            DateOfBirth = student.DateOfBirth,
            Gender = student.Gender,
            FatherName = student.FatherName,
            MotherName = student.MotherName,
            MobileNumber = student.MobileNumber,
            EmailAddress = student.EmailAddress,
            ProfilePicturePath = student.ProfilePicturePath,
            PresentAddress = student.PresentVillage,
            PermanentAddress = student.PermanentVillage,
            BloodGroup = student.BloodGroup,
            Religion = student.Religion,
            ClassName = student.Class?.Name ?? "",
            SectionName = student.Section?.Name ?? "",
            RollNumber = student.RollNumber,
            StudentGroupName = student.StudentGroup?.Name
        };
    }

    public async Task UpdateProfileAsync(int userId, StudentProfileUpdateDto dto, CancellationToken ct = default)
    {
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);

        if (student == null)
            throw new InvalidOperationException("Student not found.");

        if (!string.IsNullOrEmpty(dto.EmailAddress))
            student.EmailAddress = dto.EmailAddress;
        if (!string.IsNullOrEmpty(dto.MobileNumber))
            student.MobileNumber = dto.MobileNumber;
        if (!string.IsNullOrEmpty(dto.PresentVillage))
            student.PresentVillage = dto.PresentVillage;
        if (!string.IsNullOrEmpty(dto.PermanentVillage))
            student.PermanentVillage = dto.PermanentVillage;

        student.UpdatedAt = DateTime.UtcNow;
        student.UpdatedBy = userId.ToString();

        _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Update(student);
        await _uow.SaveChangesAsync(ct);
    }

    public async Task<List<StudentAttendanceDto>> GetAttendanceAsync(int userId, DateTime? from, DateTime? to, CancellationToken ct = default)
    {
        var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
            .AsNoTracking()
            .FirstOrDefaultAsync(s => s.UserId == userId && !s.IsDeleted, ct);

        if (student == null) return new();

        var fromDate = (from ?? new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1)).Date;
        var toDate = (to ?? DateTime.Today).Date;

        var records = await _uow.Repository<StudentAttendance>().Query()
            .AsNoTracking()
            .Include(a => a.Class)
            .Include(a => a.Section)
            .Where(a => a.StudentId == student.Id && a.AttendanceDate >= fromDate && a.AttendanceDate <= toDate)
            .OrderByDescending(a => a.AttendanceDate)
            .ToListAsync(ct);

        return records.Select(a => new StudentAttendanceDto
        {
            AttendanceDate = a.AttendanceDate,
            StatusName = a.Status.ToString(),
            ClassName = a.Class?.Name ?? "",
            SectionName = a.Section?.Name ?? "",
            Remarks = a.Remarks
        }).ToList();
    }

    public async Task<List<StudentNotificationItemDto>> GetNotificationsAsync(int userId, CancellationToken ct = default)
    {
        var notifications = await _uow.Repository<NotificationMessage>().Query()
            .AsNoTracking()
            .Where(n => n.UserId == userId && !n.IsDeleted)
            .OrderByDescending(n => n.Id)
            .Take(100)
            .ToListAsync(ct);

        return notifications.Select(n => new StudentNotificationItemDto
        {
            Id = n.Id,
            Title = n.Title,
            Body = n.Body,
            Channel = (int)n.Channel,
            IsRead = n.IsRead,
            SentAt = n.SentAt,
            CreatedAt = n.CreatedAt
        }).ToList();
    }

    public async Task MarkNotificationReadAsync(int userId, int notificationId, CancellationToken ct = default)
    {
        var n = await _uow.Repository<NotificationMessage>().Query()
            .FirstOrDefaultAsync(x => x.Id == notificationId && x.UserId == userId && !x.IsDeleted, ct);

        if (n == null) return;

        n.IsRead = true;
        n.SentAt ??= DateTime.UtcNow;
        _uow.Repository<NotificationMessage>().Update(n);
        await _uow.SaveChangesAsync(ct);
    }
}
