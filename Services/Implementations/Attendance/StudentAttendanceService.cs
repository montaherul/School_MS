using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Admin;
using System.Reflection;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Enums;

using SchoolManagementSystem.Models.Entities.Academic;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using SchoolManagementSystem.Services.Interfaces.Attendance;
using SchoolManagementSystem.UnitOfWork.Interfaces;
using SchoolManagementSystem.Services.Interfaces.Admin;
using System.Reflection;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Enums;

using SchoolManagementSystem.Models.Entities.Academic;
using Microsoft.Extensions.Logging;

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class StudentAttendanceService : IStudentAttendanceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IStudentAttendanceRepository _repo;
        private readonly IAttendanceLogRepository _auditLog;
        private readonly IAttendanceNotificationService _notificationService;
        private readonly IAttendanceAuthorizationService _authorizationService;
        private readonly IAttendanceValidationService _validationService;
        private readonly ILogger<StudentAttendanceService> _logger;

        public StudentAttendanceService(
            IUnitOfWork uow, 
            IStudentAttendanceRepository repo, 
            IAttendanceLogRepository auditLog,
            IAttendanceNotificationService notificationService,
            IAttendanceAuthorizationService authorizationService,
            IAttendanceValidationService validationService,
            ILogger<StudentAttendanceService> logger)
        {
            _uow = uow;
            _repo = repo;
            _auditLog = auditLog;
            _notificationService = notificationService;
            _authorizationService = authorizationService;
            _validationService = validationService;
            _logger = logger;
        }

        public async Task<int> MarkAttendanceAsync(StudentAttendanceItemDto dto, int classId, int sectionId, DateTime date, string recordedBy, CancellationToken ct = default)
        {
            var dateOnly = DateOnly.FromDateTime(date);
            var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>()
                .Query()
                .FirstOrDefaultAsync(s => s.Id == dto.StudentId && !s.IsDeleted, ct)
                ?? throw new InvalidOperationException("Student not found.");

            var result = await SaveAttendanceAsync(new StudentAttendanceBulkDto
            {
                ClassId = classId,
                SectionId = sectionId,
                StudentGroupId = student.StudentGroupId,
                AttendanceDate = date.Date,
                SendNotifications = true,
                Attendances = new List<StudentAttendanceItemDto>
                {
                    dto
                }
            }, recordedBy, ct);

            if (!result.Success)
            {
                throw new InvalidOperationException(result.Message);
            }

            var saved = await _uow.Repository<AttendanceRecord>().Query()
                .AsNoTracking()
                .FirstOrDefaultAsync(a => a.StudentId == dto.StudentId && a.AttendanceDate == dateOnly && !a.IsDeleted, ct)
                ?? throw new InvalidOperationException("Attendance record was not saved.");

            return saved.Id;
        }

        public async Task<bool> BulkMarkAsync(StudentAttendanceBulkDto dto, string recordedBy, CancellationToken ct = default)
        {
            var response = await SaveAttendanceAsync(dto, recordedBy, ct);
            return response.Success;
        }

        public async Task UpdateAttendanceAsync(int id, SchoolManagementSystem.Models.Enums.AttendanceStatus status, string? remarks, string updatedBy, CancellationToken ct = default)
        {
            var repo = _uow.Repository<AttendanceRecord>();
            var entity = await repo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct) ?? throw new KeyNotFoundException("Attendance record not found.");
            
            entity.Status = status;
            entity.Remarks = remarks;
            entity.UpdatedBy = updatedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            
            repo.Update(entity);
            await _uow.SaveChangesAsync(ct);
            await _auditLog.AddAsync(new AttendanceLog { UserId = updatedBy, Action = "Updated Student Attendance", EntityName = "AttendanceRecord", EntityId = id }, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task DeleteAttendanceAsync(int id, string deletedBy, CancellationToken ct = default)
        {
            var repo = _uow.Repository<AttendanceRecord>();
            var entity = await repo.FirstOrDefaultAsync(x => x.Id == id && !x.IsDeleted, ct) ?? throw new KeyNotFoundException("Attendance record not found.");
            
            entity.IsDeleted = true;
            entity.UpdatedBy = deletedBy;
            entity.UpdatedAt = DateTime.UtcNow;
            
            repo.Update(entity);
            await _uow.SaveChangesAsync(ct);
            await _auditLog.AddAsync(new AttendanceLog { UserId = deletedBy, Action = "Deleted Student Attendance", EntityName = "AttendanceRecord", EntityId = id }, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task<(List<StudentAttendanceDto> Data, int TotalRecords)> GetPagedAsync(
            int page, 
            int size, 
            int? classId = null, 
            int? sectionId = null, 
            int? studentGroupId = null,
            DateTime? date = null, 
            CancellationToken ct = default)
        {
            // Business Rule: Class 9 & 10 must have a group selected in Bangladesh curriculum
            if (classId.HasValue && !studentGroupId.HasValue)
            {
                var schoolClass = await _uow.Repository<SchoolClass>().GetByIdAsync(classId.Value, ct);
                if (schoolClass != null && (schoolClass.Name.Contains("Class 9", StringComparison.OrdinalIgnoreCase) || schoolClass.Name.Contains("Class 10", StringComparison.OrdinalIgnoreCase)))
                {
                    // If Class 9/10 is selected but no group, and it's not a global view, 
                    // we should ideally enforce group selection in the UI.
                    // For now, we allow the fetch but log it.
                    _logger.LogInformation("Class 9/10 attendance fetched without group filter.");
                }
            }

            var filter = new StudentAttendanceFilterDto
            {
                ClassId = classId,
                SectionId = sectionId,
                StudentGroupId = studentGroupId,
                AttendanceDate = date ?? DateTime.Today,
                Page = page,
                PageSize = size
            };

            return await _repo.GetAttendanceGridAsync(filter, page, size, ct);
        }

        /// <summary>
        /// Bulk save attendance with duplicate prevention and validation - matches EmployeeAttendanceService pattern
        /// </summary>
        public async Task<BulkAttendanceSaveResponse> SaveAttendanceAsync(StudentAttendanceBulkDto dto, string recordedBy, CancellationToken ct = default)
        {
            var response = new BulkAttendanceSaveResponse { Success = true };
            var date = DateOnly.FromDateTime(dto.AttendanceDate);
            var repo = _uow.Repository<AttendanceRecord>();
            var sessionRepo = _uow.Repository<AttendanceSession>();

            var validationError = await _validationService.ValidateAttendanceDateAsync(date, ct);
            if (validationError != null)
            {
                response.Success = false;
                response.Message = validationError;
                return response;
            }

            // Ensure an attendance session exists or is checked for locking
            await EnsureNoDuplicateSessionsAsync(dto.ClassId, dto.SectionId, dto.StudentGroupId, date, ct);
            var existingSession = await FindSessionAsync(dto.ClassId, dto.SectionId, dto.StudentGroupId, date, ct);

            if (existingSession != null &&
                (existingSession.Status == AttendanceSessionStatus.Locked ||
                 existingSession.Status == AttendanceSessionStatus.Approved))
            {
                response.Success = false;
                response.Message = "Attendance already submitted for this class and date.";
                return response;
            }

            try
            {
                await RequireGroupForUpperClassesAsync(dto.ClassId, dto.StudentGroupId, ct);
                await _authorizationService.EnsureCurrentUserCanManageAttendanceAsync(dto.ClassId, dto.SectionId, dto.StudentGroupId, 0, ct);

                // Validate input
                if (dto.Attendances == null || !dto.Attendances.Any())
                {
                    response.Success = false;
                    response.Message = "No attendance records to save.";
                    return response;
                }

                await ValidateStudentRosterAsync(dto.ClassId, dto.SectionId, dto.StudentGroupId, dto.Attendances.Select(a => a.StudentId), ct);

                // Check for duplicates in the request
                var duplicateStudents = dto.Attendances
                    .GroupBy(a => a.StudentId)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();

                if (duplicateStudents.Any())
                {
                    response.Success = false;
                    response.Errors.Add($"Duplicate student IDs in request: {string.Join(", ", duplicateStudents)}");
                    return response;
                }

                // Get existing records for this date/class/section — filter group via Students join
                var existingQuery = repo.Query()
                    .Include(a => a.Student)
                    .Where(a => a.SchoolClassId == dto.ClassId
                        && a.SectionId == dto.SectionId
                        && a.AttendanceDate == date
                        && !a.IsDeleted);

                if (dto.StudentGroupId.HasValue)
                    existingQuery = existingQuery.Where(a => a.Student != null && a.Student.StudentGroupId == dto.StudentGroupId);

                var existingRecords = await existingQuery.ToListAsync(ct);

                var existingDict = existingRecords.ToDictionary(a => a.StudentId);
                var recordsToAdd = new List<AttendanceRecord>();
                var recordsToUpdate = new List<AttendanceRecord>();
                var statusChanges = new List<(int StudentId, SchoolManagementSystem.Models.Enums.AttendanceStatus OldStatus, SchoolManagementSystem.Models.Enums.AttendanceStatus NewStatus)>();

                // Process each attendance item
                foreach (var item in dto.Attendances)
                {
                    if (existingDict.TryGetValue(item.StudentId, out var existingRecord))
                    {
                        // Track status changes for notification
                        if (existingRecord.Status != item.Status)
                        {
                            statusChanges.Add((item.StudentId, existingRecord.Status, item.Status));
                        }

                        // Update existing
                        existingRecord.Status = item.Status;
                        existingRecord.Remarks = item.Remarks;
                        existingRecord.UpdatedBy = recordedBy;
                        existingRecord.UpdatedAt = DateTime.UtcNow;
                        recordsToUpdate.Add(existingRecord);
                    }
                    else
                    {
                        // Track new absents for notification
                        if (item.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Absent)
                        {
                            statusChanges.Add((item.StudentId, SchoolManagementSystem.Models.Enums.AttendanceStatus.Present, item.Status));
                        }

                        // Add new
                        recordsToAdd.Add(new AttendanceRecord
                        {
                            StudentId = item.StudentId,
                            SchoolClassId = dto.ClassId,
                            SectionId = dto.SectionId,
                            AttendanceDate = date,
                            Status = item.Status,
                            Remarks = item.Remarks,
                            CreatedBy = recordedBy,
                            CreatedAt = DateTime.UtcNow
                        });
                    }
                }


                // Use UnitOfWork transaction methods now exposed on IUnitOfWork
                await _uow.ExecuteInTransactionAsync(async () =>
                {
                    try
                    {
                        if (recordsToAdd.Any())
                        {
                            await repo.AddRangeAsync(recordsToAdd, ct);
                        }

                        if (recordsToUpdate.Any())
                        {
                            foreach (var record in recordsToUpdate)
                            {
                                // Create revision entries for any status changes
                                var trackedOld = record; // existing record already modified in-memory
                                // Note: we captured status changes earlier in statusChanges list
                                repo.Update(record);
                            }
                        }

                        await _uow.SaveChangesAsync(ct);

                        // Create or update attendance session and lock it as Submitted->Locked
                        if (existingSession == null)
                        {
                            existingSession = new AttendanceSession
                            {
                                SchoolClassId = dto.ClassId,
                                SectionId = dto.SectionId,
                                StudentGroupId = dto.StudentGroupId,
                                AttendanceDate = date,
                                Status = AttendanceSessionStatus.Submitted,
                                SubmittedBy = recordedBy,
                                SubmittedAt = DateTime.UtcNow,
                                CreatedBy = recordedBy,
                                CreatedAt = DateTime.UtcNow
                            };
                            await sessionRepo.AddAsync(existingSession, ct);
                        }
                        else
                        {
                            existingSession.Status = AttendanceSessionStatus.Submitted;
                            existingSession.SubmittedBy = recordedBy;
                            existingSession.SubmittedAt = DateTime.UtcNow;
                            existingSession.UpdatedBy = recordedBy;
                            existingSession.UpdatedAt = DateTime.UtcNow;
                            sessionRepo.Update(existingSession);
                        }

                        await _uow.SaveChangesAsync(ct);

                        // Immediately lock the session to prevent further submissions
                        existingSession.Status = AttendanceSessionStatus.Locked;
                        existingSession.LockedBy = recordedBy;
                        existingSession.LockedAt = DateTime.UtcNow;
                        existingSession.UpdatedBy = recordedBy;
                        existingSession.UpdatedAt = DateTime.UtcNow;
                        sessionRepo.Update(existingSession);
                        await _uow.SaveChangesAsync(ct);

                        // Refresh record IDs for revision entries on newly inserted rows
                        if (recordsToAdd.Any())
                        {
                            var newIds = await repo.Query()
                                .Where(a => recordsToAdd.Select(r => r.StudentId).Contains(a.StudentId)
                                    && a.AttendanceDate == date && !a.IsDeleted)
                                .ToListAsync(ct);
                            foreach (var record in newIds)
                                existingDict[record.StudentId] = record;
                        }

                        // Create attendance revision logs for changed statuses
                        var revisionRepo = _uow.Repository<AttendanceRevision>();
                        foreach (var sc in statusChanges)
                        {
                            var rev = new AttendanceRevision
                            {
                                AttendanceRecordId = existingDict.TryGetValue(sc.StudentId, out var rec) ? rec.Id : 0,
                                StudentId = sc.StudentId,
                                AttendanceDate = date,
                                OldStatus = sc.OldStatus.ToString(),
                                NewStatus = sc.NewStatus.ToString(),
                                Reason = null,
                                ChangedBy = recordedBy,
                                ChangedAt = DateTime.UtcNow,
                                CreatedBy = recordedBy,
                                CreatedAt = DateTime.UtcNow
                            };
                            await revisionRepo.AddAsync(rev, ct);
                        }

                        await _uow.SaveChangesAsync(ct);
                        await _uow.CommitTransactionAsync(ct);
                    }
                    catch (Exception)
                    {
                        await _uow.RollbackTransactionAsync(ct);
                        throw;
                    }
                }, ct);

                response.RecordsSaved = recordsToAdd.Count + recordsToUpdate.Count;

                // Log audit
                await _auditLog.AddAsync(new AttendanceLog 
                { 
                    UserId = recordedBy, 
                    Action = $"Bulk marked {response.RecordsSaved} attendance records", 
                    EntityName = "AttendanceRecord", 
                    EntityId = 0 
                }, ct);
                await _uow.SaveChangesAsync(ct);

                // Send notifications for absent students if enabled
                if (dto.SendNotifications && statusChanges.Any())
                {
                    var absentStudentIds = statusChanges
                        .Where(sc => sc.NewStatus == SchoolManagementSystem.Models.Enums.AttendanceStatus.Absent)
                        .Select(sc => sc.StudentId)
                        .Distinct()
                        .ToList();

                    var lateStudentIds = statusChanges
                        .Where(sc => sc.NewStatus == SchoolManagementSystem.Models.Enums.AttendanceStatus.Late)
                        .Select(sc => sc.StudentId)
                        .Distinct()
                        .ToList();

                    if (absentStudentIds.Any())
                    {
                        try
                        {
                            await _notificationService.SendAbsentNotificationsAsync(absentStudentIds, date, recordedBy, ct);
                            response.NotificationsSent = absentStudentIds.Count;
                        }
                        catch (Exception notifyEx)
                        {
                            // Log notification error but don't fail the attendance save
                            await _auditLog.AddAsync(new AttendanceLog 
                            { 
                                UserId = recordedBy, 
                                Action = $"Notification error: {notifyEx.Message}", 
                                EntityName = "AttendanceNotificationLog", 
                                EntityId = 0 
                            }, ct);
                            await _uow.SaveChangesAsync(ct);
                        }
                    }

                    if (lateStudentIds.Any())
                    {
                        try
                        {
                            await _notificationService.SendLateStudentNotificationsAsync(lateStudentIds, date, recordedBy, ct);
                            response.NotificationsSent += lateStudentIds.Count;
                        }
                        catch (Exception notifyEx)
                        {
                            await _auditLog.AddAsync(new AttendanceLog
                            {
                                UserId = recordedBy,
                                Action = $"Late notification error: {notifyEx.Message}",
                                EntityName = "AttendanceNotificationLog",
                                EntityId = 0
                            }, ct);
                            await _uow.SaveChangesAsync(ct);
                        }
                    }
                }

                response.Message = $"Successfully saved {response.RecordsSaved} attendance records." +
                    (response.NotificationsSent > 0 ? $" {response.NotificationsSent} absent notification(s) queued." : "");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = ex.Message;
                response.Errors.Add(ex.Message);
            }

            return response;
        }

        /// <summary>
        /// Get all students in a class/section for attendance marking
        /// </summary>
        public async Task<(List<StudentAttendanceDto> Students, int Total)> GetStudentsForAttendanceAsync(
            int classId, 
            int sectionId, 
            int? studentGroupId,
            DateTime attendanceDate,
            int page = 1,
            int pageSize = 50,
            CancellationToken ct = default)
        {
            var dateOnly = DateOnly.FromDateTime(attendanceDate);
            var studentRepo = _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>();
            var attendanceRepo = _uow.Repository<AttendanceRecord>();

            // Get active students
            var studentsQuery = studentRepo.Query()
                .Include(s => s.Class)
                .Include(s => s.Section)
                .Include(s => s.StudentGroup)
                .Where(s => s.ClassId == classId 
                    && s.SectionId == sectionId 
                    && s.Status == SchoolManagementSystem.Models.Enums.StudentStatus.Active 
                    && !s.IsDeleted);

            if (studentGroupId.HasValue)
            {
                studentsQuery = studentsQuery.Where(s => s.StudentGroupId == studentGroupId);
            }

            var totalStudents = await studentsQuery.CountAsync(ct);

            var students = await studentsQuery
                .OrderBy(s => s.RollNumber)
                .ThenBy(s => s.FullName)
                .Skip((Math.Max(page, 1) - 1) * pageSize)
                .Take(Math.Clamp(pageSize, 1, 10000))
                .ToListAsync(ct);

            var studentIds = students.Select(s => s.Id).ToList();

            // Get existing attendance records
            var attendanceRecords = await attendanceRepo.Query()
                .Where(a => studentIds.Contains(a.StudentId) 
                    && a.AttendanceDate == dateOnly 
                    && !a.IsDeleted)
                .ToListAsync(ct);

            var attendanceDict = attendanceRecords.ToDictionary(a => a.StudentId);

            // Build DTOs
            var dtos = students.Select(s =>
            {
                var attendance = attendanceDict.ContainsKey(s.Id) ? attendanceDict[s.Id] : null;
                return new StudentAttendanceDto
                {
                    Id = attendance?.Id ?? 0,
                    StudentId = s.Id,
                    StudentNo = s.StudentNo ?? "",
                    StudentName = s.FullName ?? "",
                    RollNumber = s.RollNumber.ToString(),
                    ClassId = classId,
                    ClassName = s.Class?.Name ?? "",
                    SectionId = sectionId,
                    SectionName = s.Section?.Name ?? "",
                    StudentGroupId = s.StudentGroupId,
                    StudentGroupName = s.StudentGroup?.Name ?? "",
                    AttendanceDate = attendanceDate.Date,
                    Status = attendance?.Status ?? SchoolManagementSystem.Models.Enums.AttendanceStatus.Present,
                    StatusName = (attendance?.Status ?? SchoolManagementSystem.Models.Enums.AttendanceStatus.Present).ToString(),
                    Remarks = attendance?.Remarks ?? ""
                };
            }).ToList();

            return (dtos, totalStudents);
        }

        public async Task UnlockAttendanceSessionAsync(int classId, int sectionId, int? studentGroupId, DateTime attendanceDate, string unlockedBy, string reason, CancellationToken ct = default)
        {
            var dateOnly = DateOnly.FromDateTime(attendanceDate.Date);
            var sessionRepo = _uow.Repository<AttendanceSession>();
            var session = await FindSessionAsync(classId, sectionId, studentGroupId, dateOnly, ct)
                ?? throw new KeyNotFoundException("Attendance session not found.");

            if (session.Status != AttendanceSessionStatus.Locked && session.Status != AttendanceSessionStatus.Approved)
                throw new InvalidOperationException("Only locked or approved sessions can be unlocked.");

            session.Status = AttendanceSessionStatus.Revised;
            session.RevisedBy = unlockedBy;
            session.RevisedAt = DateTime.UtcNow;
            session.UpdatedBy = unlockedBy;
            session.UpdatedAt = DateTime.UtcNow;
            session.LockedBy = null;
            session.LockedAt = null;
            session.Notes = reason;
            sessionRepo.Update(session);
            await _uow.SaveChangesAsync(ct);

            await _auditLog.AddAsync(new AttendanceLog { UserId = unlockedBy, Action = "Unlocked Attendance Session", EntityName = "AttendanceSession", EntityId = session.Id }, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ReviseAttendanceSessionAsync(int classId, int sectionId, int? studentGroupId, DateTime attendanceDate, string revisedBy, string? notes, CancellationToken ct = default)
        {
            var dateOnly = DateOnly.FromDateTime(attendanceDate.Date);
            var sessionRepo = _uow.Repository<AttendanceSession>();
            var session = await FindSessionAsync(classId, sectionId, studentGroupId, dateOnly, ct)
                ?? throw new KeyNotFoundException("Attendance session not found.");

            if (session.Status != AttendanceSessionStatus.Locked)
                throw new InvalidOperationException("Only locked sessions can be revised.");

            session.Status = AttendanceSessionStatus.Revised;
            session.RevisedBy = revisedBy;
            session.RevisedAt = DateTime.UtcNow;
            session.UpdatedBy = revisedBy;
            session.UpdatedAt = DateTime.UtcNow;
            session.Notes = notes;
            sessionRepo.Update(session);
            await _uow.SaveChangesAsync(ct);

            await _auditLog.AddAsync(new AttendanceLog { UserId = revisedBy, Action = "Revised Attendance Session", EntityName = "AttendanceSession", EntityId = session.Id }, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ApproveAttendanceSessionAsync(int classId, int sectionId, int? studentGroupId, DateTime attendanceDate, string approvedBy, CancellationToken ct = default)
        {
            var dateOnly = DateOnly.FromDateTime(attendanceDate.Date);
            var sessionRepo = _uow.Repository<AttendanceSession>();
            var session = await FindSessionAsync(classId, sectionId, studentGroupId, dateOnly, ct)
                ?? throw new KeyNotFoundException("Attendance session not found.");

            if (session.Status != AttendanceSessionStatus.Locked && session.Status != AttendanceSessionStatus.Revised)
                throw new InvalidOperationException("Only locked or revised sessions can be approved.");

            session.Status = AttendanceSessionStatus.Approved;
            session.ApprovedBy = approvedBy;
            session.ApprovedAt = DateTime.UtcNow;
            session.UpdatedBy = approvedBy;
            session.UpdatedAt = DateTime.UtcNow;
            sessionRepo.Update(session);
            await _uow.SaveChangesAsync(ct);

            await _auditLog.AddAsync(new AttendanceLog { UserId = approvedBy, Action = "Approved Attendance Session", EntityName = "AttendanceSession", EntityId = session.Id }, ct);
            await _uow.SaveChangesAsync(ct);
        }

        private async Task<AttendanceSession?> FindSessionAsync(int classId, int sectionId, int? studentGroupId, DateOnly date, CancellationToken ct)
        {
            var sessionRepo = _uow.Repository<AttendanceSession>();
            var query = sessionRepo.Query()
                .Where(s => s.SchoolClassId == classId
                    && s.SectionId == sectionId
                    && s.AttendanceDate == date
                    && !s.IsDeleted);

            query = studentGroupId.HasValue
                ? query.Where(s => s.StudentGroupId == studentGroupId)
                : query.Where(s => s.StudentGroupId == null);

            return await query.FirstOrDefaultAsync(ct);
        }

        private async Task EnsureNoDuplicateSessionsAsync(int classId, int sectionId, int? studentGroupId, DateOnly date, CancellationToken ct)
        {
            var query = _uow.Repository<AttendanceSession>().Query()
                .Where(s => s.SchoolClassId == classId
                    && s.SectionId == sectionId
                    && s.AttendanceDate == date
                    && !s.IsDeleted);

            query = studentGroupId.HasValue
                ? query.Where(s => s.StudentGroupId == studentGroupId)
                : query.Where(s => s.StudentGroupId == null);

            if (await query.CountAsync(ct) > 1)
            {
                throw new InvalidOperationException("Duplicate attendance sessions exist for this class, section, group and date. Resolve duplicates before saving attendance.");
            }
        }

        private async Task EnsureSessionWritableAsync(int classId, int sectionId, int? studentGroupId, DateOnly date, CancellationToken ct)
        {
            var session = await FindSessionAsync(classId, sectionId, studentGroupId, date, ct);
            if (session == null) return;

            if (session.Status == AttendanceSessionStatus.Submitted ||
                session.Status == AttendanceSessionStatus.Locked ||
                session.Status == AttendanceSessionStatus.Approved)
            {
                throw new InvalidOperationException("Attendance session is locked and cannot be modified.");
            }
        }

        private static int ParseClassNumber(string className)
        {
            if (string.IsNullOrEmpty(className)) return 0;
            var trimmed = className.Replace("Class ", "", StringComparison.OrdinalIgnoreCase).Trim();
            var match = System.Text.RegularExpressions.Regex.Match(trimmed, "\\d+");
            if (match.Success && int.TryParse(match.Value, out var num)) return num;
            return trimmed.ToUpperInvariant() switch
            {
                "IX" => 9,
                "X" => 10,
                _ => 0
            };
        }

        private async Task RequireGroupForUpperClassesAsync(int classId, int? studentGroupId, CancellationToken ct)
        {
            var schoolClass = await _uow.Repository<SchoolClass>().GetByIdAsync(classId, ct);
            if (schoolClass == null) return;
            var classNum = ParseClassNumber(schoolClass.Name);
            if (classNum >= 9 && classNum <= 10 && !studentGroupId.HasValue)
                throw new InvalidOperationException("Student group is required for Class 9 and 10 attendance.");
        }

        private async Task ValidateStudentRosterAsync(int classId, int sectionId, int? studentGroupId, IEnumerable<int> studentIds, CancellationToken ct)
        {
            var ids = studentIds.Distinct().ToList();
            if (!ids.Any()) return;

            var query = _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
                .Where(s => ids.Contains(s.Id)
                    && s.ClassId == classId
                    && s.SectionId == sectionId
                    && s.Status == StudentStatus.Active
                    && !s.IsDeleted);

            if (studentGroupId.HasValue)
                query = query.Where(s => s.StudentGroupId == studentGroupId);

            var validIds = await query.Select(s => s.Id).ToListAsync(ct);
            var invalid = ids.Except(validIds).ToList();
            if (invalid.Any())
                throw new InvalidOperationException($"Students not in this class/section/group: {string.Join(", ", invalid)}");
        }

        /// <summary>
        /// Load attendance with filter and summary - matches EmployeeAttendanceService pattern
        /// </summary>
        public async Task<(List<StudentAttendanceDto> Data, int TotalRecords, StudentAttendanceSummaryDto Summary)> LoadAttendanceAsync(
            StudentAttendanceFilterDto filter,
            int page,
            int size,
            CancellationToken ct = default)
        {
            // Use SP-based repository methods
            var (data, totalRecords) = await _repo.GetAttendanceGridAsync(filter, page, size, ct);
            var summary = await _repo.GetAttendanceSummaryAsync(filter, ct);

            return (data, totalRecords, summary);
        }

        /// <summary>
        /// Get attendance history for a student in a specific month - matches EmployeeAttendanceService pattern
        /// </summary>
        public async Task<List<StudentAttendanceDto>> GetAttendanceHistoryAsync(int studentId, int year, int month, CancellationToken ct = default)
        {
            var repo = _uow.Repository<AttendanceRecord>();
            var startDate = new DateOnly(year, month, 1);
            var endDate = startDate.AddMonths(1).AddDays(-1);

            var records = await repo.Query()
                .Include(a => a.Student)
                .Where(a => a.StudentId == studentId 
                    && a.AttendanceDate >= startDate 
                    && a.AttendanceDate <= endDate
                    && !a.IsDeleted)
                .OrderBy(a => a.AttendanceDate)
                .ToListAsync(ct);

            return records.Select(a => new StudentAttendanceDto
            {
                Id = a.Id,
                StudentId = a.StudentId,
                StudentNo = a.Student?.StudentNo ?? "",
                StudentName = a.Student?.FullName ?? "",
                RollNumber = a.Student?.RollNumber.ToString() ?? "",
                ClassId = a.SchoolClassId,
                ClassName = a.Student?.Class?.Name ?? "",
                SectionId = a.SectionId,
                SectionName = a.Student?.Section?.Name ?? "",
                AttendanceDate = a.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                Status = a.Status,
                StatusName = a.Status.ToString(),
                Remarks = a.Remarks ?? ""
            }).ToList();
        }

        /// <summary>
        /// Get monthly summary for a student - matches EmployeeAttendanceService pattern
        /// </summary>
        public async Task<StudentAttendanceMonthlySummaryDto> GetMonthlySummaryAsync(int studentId, int year, int month, CancellationToken ct = default)
        {
            var history = await GetAttendanceHistoryAsync(studentId, year, month, ct);
            var student = await _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().GetByIdAsync(studentId, ct);

            if (!history.Any())
            {
                return new StudentAttendanceMonthlySummaryDto
                {
                    StudentId = studentId,
                    StudentNo = student?.StudentNo ?? "",
                    StudentName = student?.FullName ?? "",
                    RollNumber = student?.RollNumber.ToString() ?? "",
                    Year = year,
                    Month = month,
                    TotalDays = 0,
                    AttendancePercentage = 0
                };
            }

            var presentCount = history.Count(h => h.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Present);
            var absentCount = history.Count(h => h.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Absent);
            var lateCount = history.Count(h => h.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Late);
            var leaveCount = history.Count(h => h.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Leave);
            var totalDays = history.Count;
            var attendancePercentage = totalDays > 0 ? Math.Round((double)(presentCount + lateCount) / totalDays * 100, 2) : 0;

            var firstRecord = history.FirstOrDefault();
            return new StudentAttendanceMonthlySummaryDto
            {
                StudentId = studentId,
                StudentNo = student?.StudentNo ?? firstRecord?.StudentNo ?? "",
                StudentName = student?.FullName ?? firstRecord?.StudentName ?? "",
                RollNumber = student?.RollNumber.ToString() ?? firstRecord?.RollNumber ?? "",
                Year = year,
                Month = month,
                TotalDays = totalDays,
                PresentCount = presentCount,
                AbsentCount = absentCount,
                LateCount = lateCount,
                LeaveCount = leaveCount,
                AttendancePercentage = attendancePercentage
            };
        }

        /// <summary>
        /// Get attendance percentage for a student in a specific month - matches EmployeeAttendanceService pattern
        /// </summary>
        public async Task<double> GetAttendancePercentageAsync(int studentId, int year, int month, CancellationToken ct = default)
        {
            var summary = await GetMonthlySummaryAsync(studentId, year, month, ct);
            return summary.AttendancePercentage;
        }
    }
}
