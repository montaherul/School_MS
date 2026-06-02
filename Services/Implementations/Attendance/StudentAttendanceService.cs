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

namespace SchoolManagementSystem.Services.Implementations.Attendance
{
    public class StudentAttendanceService : IStudentAttendanceService
    {
        private readonly IUnitOfWork _uow;
        private readonly IStudentAttendanceRepository _repo;
        private readonly IAttendanceLogRepository _auditLog;
        private readonly IAttendanceNotificationService _notificationService;

        public StudentAttendanceService(
            IUnitOfWork uow, 
            IStudentAttendanceRepository repo, 
            IAttendanceLogRepository auditLog,
            IAttendanceNotificationService notificationService)
        {
            _uow = uow;
            _repo = repo;
            _auditLog = auditLog;
            _notificationService = notificationService;
        }

        public async Task<int> MarkAttendanceAsync(StudentAttendanceItemDto dto, int classId, int sectionId, DateTime date, string recordedBy, CancellationToken ct = default)
        {
            var dateOnly = DateOnly.FromDateTime(date);
            var repo = _uow.Repository<AttendanceRecord>();

            var existing = await repo.Query().FirstOrDefaultAsync(a => a.StudentId == dto.StudentId && a.AttendanceDate == dateOnly && !a.IsDeleted, ct);
            
            if (existing != null)
            {
                existing.Status = dto.Status;
                existing.Remarks = dto.Remarks;
                existing.UpdatedBy = recordedBy;
                existing.UpdatedAt = DateTime.UtcNow;
                repo.Update(existing);
                await _auditLog.AddAsync(new AttendanceLog { UserId = recordedBy, Action = "Updated Student Attendance", EntityName = "AttendanceRecord", EntityId = existing.Id }, ct);
                await _uow.SaveChangesAsync(ct);
                
                return existing.Id;
            }
            else
            {
                var entity = new AttendanceRecord
                {
                    StudentId = dto.StudentId,
                    SchoolClassId = classId,
                    SectionId = sectionId,
                    AttendanceDate = dateOnly,
                    Status = dto.Status,
                    Remarks = dto.Remarks,
                    CreatedBy = recordedBy,
                    CreatedAt = DateTime.UtcNow
                };

                await repo.AddAsync(entity, ct);
                await _uow.SaveChangesAsync(ct);
                
                await _auditLog.AddAsync(new AttendanceLog { UserId = recordedBy, Action = "Marked Student Attendance", EntityName = "AttendanceRecord", EntityId = entity.Id }, ct);
                await _uow.SaveChangesAsync(ct);
                
                return entity.Id;
            }
        }

        public async Task<bool> BulkMarkAsync(StudentAttendanceBulkDto dto, string recordedBy, CancellationToken ct = default)
        {
            var date = DateOnly.FromDateTime(dto.AttendanceDate);
            var entities = new List<AttendanceRecord>();
            var repo = _uow.Repository<AttendanceRecord>();
            
            // Check existing
            var existingRecords = await repo.Query()
                .Where(a => a.SchoolClassId == dto.ClassId && a.SectionId == dto.SectionId && a.AttendanceDate == date && !a.IsDeleted)
                .ToListAsync(ct);

            var existingDict = existingRecords.ToDictionary(a => a.StudentId);

            foreach (var item in dto.Attendances)
            {
                if (existingDict.TryGetValue(item.StudentId, out var att))
                {
                    att.Status = item.Status;
                    att.Remarks = item.Remarks;
                    att.UpdatedBy = recordedBy;
                    att.UpdatedAt = DateTime.UtcNow;
                    repo.Update(att);
                }
                else
                {
                    entities.Add(new AttendanceRecord
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

            if (entities.Any())
            {
                await repo.AddRangeAsync(entities, ct);
            }
            
            await _auditLog.AddAsync(new AttendanceLog { UserId = recordedBy, Action = $"Bulk Marked {entities.Count} Student Attendances", EntityName = "AttendanceRecord", EntityId = 0 }, ct);
            
            await _uow.SaveChangesAsync(ct);
            
            return true;
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
            if (classId.HasValue && sectionId.HasValue)
            {
                var studentQuery = _uow.Repository<SchoolManagementSystem.Models.Entities.Student.Student>().Query()
                    .Include(s => s.Class)
                    .Include(s => s.Section)
                    .Where(s => s.ClassId == classId.Value && s.SectionId == sectionId.Value && s.Status == SchoolManagementSystem.Models.Enums.StudentStatus.Active && !s.IsDeleted);

                // Apply student group filter if provided
                if (studentGroupId.HasValue)
                {
                    studentQuery = studentQuery.Where(s => s.StudentGroupId == studentGroupId.Value);
                }

                var allStudents = await studentQuery.ToListAsync(ct);

                var searchDate = date ?? DateTime.Today;

                var dateOnly = DateOnly.FromDateTime(searchDate);

                var existingAttendances = await _uow.Repository<AttendanceRecord>().Query()
                    .Where(a => a.SchoolClassId == classId.Value && a.SectionId == sectionId.Value && a.AttendanceDate == dateOnly && !a.IsDeleted)
                    .ToListAsync(ct);

                var list = new List<StudentAttendanceDto>();
                foreach (var std in allStudents)
                {
                    var att = existingAttendances.FirstOrDefault(a => a.StudentId == std.Id);
                    if (att != null)
                    {
                        list.Add(new StudentAttendanceDto
                        {
                            Id = att.Id,
                            StudentId = std.Id,
                            StudentNo = std.StudentNo,
                            StudentName = std.FullName,
                            RollNumber = std.RollNumber.ToString(),
                            ClassId = classId.Value,
                            ClassName = std.Class != null ? std.Class.Name : "",
                            SectionId = sectionId.Value,
                            SectionName = std.Section != null ? std.Section.Name : "",
                            AttendanceDate = searchDate.Date,
                            Status = att.Status,
                            Remarks = att.Remarks
                        });
                    }
                    else
                    {
                        list.Add(new StudentAttendanceDto
                        {
                            Id = 0,
                            StudentId = std.Id,
                            StudentNo = std.StudentNo,
                            StudentName = std.FullName,
                            RollNumber = std.RollNumber.ToString(),
                            ClassId = classId.Value,
                            ClassName = std.Class != null ? std.Class.Name : "",
                            SectionId = sectionId.Value,
                            SectionName = std.Section != null ? std.Section.Name : "",
                            AttendanceDate = searchDate.Date,
                            Status = SchoolManagementSystem.Models.Enums.AttendanceStatus.Present,
                            Remarks = string.Empty
                        });
                    }
                }

                var totalCount = list.Count;
                var pagedData = list.OrderBy(s => int.TryParse(s.RollNumber, out var r) ? r : 999)
                                    .Skip((page - 1) * size).Take(size).ToList();

                return (pagedData, totalCount);
            }

            var query = _repo.Query().Include(a => a.Student).Include(a => a.Class).Include(a => a.Section).AsQueryable();

            if (classId.HasValue) query = query.Where(a => a.ClassId == classId.Value);
            if (sectionId.HasValue) query = query.Where(a => a.SectionId == sectionId.Value);
            if (date.HasValue) query = query.Where(a => a.AttendanceDate == date.Value.Date);

            var total = await query.CountAsync(ct);
            var items = await query.OrderByDescending(a => a.AttendanceDate)
                                   .Skip((page - 1) * size).Take(size)
                                   .Select(a => new StudentAttendanceDto
                                   {
                                       Id = a.Id,
                                       StudentId = a.StudentId,
                                       StudentNo = a.Student.StudentNo,
                                       StudentName = a.Student!.FullName,
                                       RollNumber = a.Student.RollNumber.ToString(),
                                       ClassId = a.ClassId,
                                       ClassName = a.Class!.Name,
                                       SectionId = a.SectionId,
                                       SectionName = a.Section!.Name,
                                       AttendanceDate = a.AttendanceDate,
                                       Status = a.Status,
                                       Remarks = a.Remarks
                                   })
                                   .ToListAsync(ct);

            return (items, total);
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

            // Ensure an attendance session exists or is checked for locking
            var existingSession = await sessionRepo.Query()
                .Where(s => s.SchoolClassId == dto.ClassId 
                    && s.SectionId == dto.SectionId 
                    && s.StudentGroupId == dto.StudentGroupId 
                    && s.AttendanceDate == date 
                    && !s.IsDeleted)
                .FirstOrDefaultAsync(ct);

            if (existingSession != null && existingSession.Status == AttendanceSessionStatus.Locked)
            {
                response.Success = false;
                response.Message = "Attendance already submitted for this class and date.";
                return response;
            }

            try
            {
                // Validate input
                if (dto.Attendances == null || !dto.Attendances.Any())
                {
                    response.Success = false;
                    response.Message = "No attendance records to save.";
                    return response;
                }

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

                // Get existing records for this date/class/section
                var existingRecords = await repo.Query()
                    .Where(a => a.SchoolClassId == dto.ClassId 
                        && a.SectionId == dto.SectionId 
                        && a.AttendanceDate == date 
                        && !a.IsDeleted)
                    .Include(a => a.Student)
                    .ToListAsync(ct);

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
                await _uow.BeginTransactionAsync(ct);
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
                            CreatedBy = recordedBy,
                            CreatedAt = DateTime.UtcNow
                        };
                        await sessionRepo.AddAsync(existingSession, ct);
                    }
                    else
                    {
                        existingSession.Status = AttendanceSessionStatus.Submitted;
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

                    // Create attendance revision logs for changed statuses
                    var revisionRepo = _uow.Repository<AttendanceRevision>();
                    foreach (var sc in statusChanges)
                    {
                        var rev = new AttendanceRevision
                        {
                            AttendanceRecordId = existingDict.ContainsKey(sc.StudentId) ? existingDict[sc.StudentId].Id : 0,
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
                }

                response.Message = $"Successfully saved {response.RecordsSaved} attendance records." + 
                    (response.NotificationsSent > 0 ? $" {response.NotificationsSent} notification(s) sent." : "");
            }
            catch (Exception ex)
            {
                response.Success = false;
                response.Message = "An error occurred while saving attendance records.";
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
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
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
                    StudentNo = s.StudentNo,
                    StudentName = s.FullName,
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

        public async Task UnlockAttendanceSessionAsync(int classId, int sectionId, DateTime attendanceDate, string unlockedBy, string reason, CancellationToken ct = default)
        {
            var dateOnly = DateOnly.FromDateTime(attendanceDate.Date);
            var sessionRepo = _uow.Repository<AttendanceSession>();
            var session = await sessionRepo.FirstOrDefaultAsync(s => s.SchoolClassId == classId && s.SectionId == sectionId && s.AttendanceDate == dateOnly && !s.IsDeleted, ct)
                ?? throw new KeyNotFoundException("Attendance session not found.");

            if (session.Status != AttendanceSessionStatus.Locked)
                throw new InvalidOperationException("Only locked sessions can be unlocked.");

            session.Status = AttendanceSessionStatus.Revised;
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

        public async Task ReviseAttendanceSessionAsync(int classId, int sectionId, DateTime attendanceDate, string revisedBy, string? notes, CancellationToken ct = default)
        {
            var dateOnly = DateOnly.FromDateTime(attendanceDate.Date);
            var sessionRepo = _uow.Repository<AttendanceSession>();
            var session = await sessionRepo.FirstOrDefaultAsync(s => s.SchoolClassId == classId && s.SectionId == sectionId && s.AttendanceDate == dateOnly && !s.IsDeleted, ct)
                ?? throw new KeyNotFoundException("Attendance session not found.");

            session.Status = AttendanceSessionStatus.Revised;
            session.UpdatedBy = revisedBy;
            session.UpdatedAt = DateTime.UtcNow;
            session.Notes = notes;
            sessionRepo.Update(session);
            await _uow.SaveChangesAsync(ct);

            await _auditLog.AddAsync(new AttendanceLog { UserId = revisedBy, Action = "Revised Attendance Session", EntityName = "AttendanceSession", EntityId = session.Id }, ct);
            await _uow.SaveChangesAsync(ct);
        }

        public async Task ApproveAttendanceSessionAsync(int classId, int sectionId, DateTime attendanceDate, string approvedBy, CancellationToken ct = default)
        {
            var dateOnly = DateOnly.FromDateTime(attendanceDate.Date);
            var sessionRepo = _uow.Repository<AttendanceSession>();
            var session = await sessionRepo.FirstOrDefaultAsync(s => s.SchoolClassId == classId && s.SectionId == sectionId && s.AttendanceDate == dateOnly && !s.IsDeleted, ct)
                ?? throw new KeyNotFoundException("Attendance session not found.");

            session.Status = AttendanceSessionStatus.Approved;
            session.UpdatedBy = approvedBy;
            session.UpdatedAt = DateTime.UtcNow;
            sessionRepo.Update(session);
            await _uow.SaveChangesAsync(ct);

            await _auditLog.AddAsync(new AttendanceLog { UserId = approvedBy, Action = "Approved Attendance Session", EntityName = "AttendanceSession", EntityId = session.Id }, ct);
            await _uow.SaveChangesAsync(ct);
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
            // Use GetPagedAsync to retrieve paginated data
            var (data, totalRecords) = await GetPagedAsync(page, size, filter.ClassId, filter.SectionId, filter.StudentGroupId, filter.AttendanceDate, ct);

            // Calculate summary for the current filter
            var summary = new StudentAttendanceSummaryDto
            {
                TotalStudents = data.Count,
                Present = data.Count(d => d.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Present),
                Absent = data.Count(d => d.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Absent),
                Late = data.Count(d => d.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Late),
                Leave = data.Count(d => d.Status == SchoolManagementSystem.Models.Enums.AttendanceStatus.Leave)
            };

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
