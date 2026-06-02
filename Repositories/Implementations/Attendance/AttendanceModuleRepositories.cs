using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using System.Linq;

namespace SchoolManagementSystem.Repositories.Implementations.Attendance
{
    public class StudentAttendanceRepository : BaseRepository<StudentAttendance>, IStudentAttendanceRepository
    {
        public StudentAttendanceRepository(SchoolDbContext context) : base(context) { }

        public async Task<bool> IsAttendanceExistsAsync(int studentId, System.DateTime date, CancellationToken cancellationToken = default)
        {
            return await _set.AnyAsync(a => a.StudentId == studentId && a.AttendanceDate.Date == date.Date, cancellationToken);
        }

        public async Task<(List<StudentAttendanceDto> Items, int TotalRecords)> GetAttendanceGridAsync(
            StudentAttendanceFilterDto filter,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var date = DateOnly.FromDateTime(filter.AttendanceDate.Date);
            var students = BuildFilteredStudents(filter);
            var totalRecords = await students.CountAsync(cancellationToken);

            var pagedStudents = await students
                .OrderBy(s => s.RollNumber)
                .ThenBy(s => s.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(s => new
                {
                    s.Id,
                    s.StudentNo,
                    s.FullName,
                    s.RollNumber,
                    s.ClassId,
                    ClassName = s.Class != null ? s.Class.Name : string.Empty,
                    s.SectionId,
                    SectionName = s.Section != null ? s.Section.Name : string.Empty,
                    s.StudentGroupId,
                    StudentGroupName = s.StudentGroup != null ? s.StudentGroup.Name : string.Empty
                })
                .ToListAsync(cancellationToken);

            var studentIds = pagedStudents.Select(s => s.Id).ToArray();
            var attendanceByStudent = await _db.Attendance
                .AsNoTracking()
                .Where(a => a.AttendanceDate == date && studentIds.Contains(a.StudentId) && !a.IsDeleted)
                .ToDictionaryAsync(a => a.StudentId, cancellationToken);

            var rows = pagedStudents.Select(student =>
            {
                attendanceByStudent.TryGetValue(student.Id, out var attendance);
                var status = attendance?.Status ?? AttendanceStatus.Present;

                return new StudentAttendanceDto
                {
                    Id = attendance?.Id ?? 0,
                    StudentId = student.Id,
                    StudentNo = student.StudentNo,
                    StudentName = student.FullName,
                    RollNumber = student.RollNumber.ToString(),
                    ClassId = student.ClassId,
                    ClassName = student.ClassName,
                    SectionId = student.SectionId,
                    SectionName = student.SectionName,
                    StudentGroupId = student.StudentGroupId,
                    StudentGroupName = student.StudentGroupName,
                    AttendanceDate = filter.AttendanceDate.Date,
                    Status = status,
                    StatusName = status.ToString(),
                    Remarks = attendance?.Remarks
                };
            }).ToList();

            return (rows, totalRecords);
        }

        public async Task<StudentAttendanceSummaryDto> GetAttendanceSummaryAsync(
            StudentAttendanceFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            var date = DateOnly.FromDateTime(filter.AttendanceDate.Date);
            var studentIds = await BuildFilteredStudents(filter)
                .Select(s => s.Id)
                .ToArrayAsync(cancellationToken);

            var records = await _db.Attendance
                .AsNoTracking()
                .Where(a => a.AttendanceDate == date && studentIds.Contains(a.StudentId) && !a.IsDeleted)
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var present = records.FirstOrDefault(x => x.Status == AttendanceStatus.Present)?.Count ?? 0;
            var absent = records.FirstOrDefault(x => x.Status == AttendanceStatus.Absent)?.Count ?? 0;
            var late = records.FirstOrDefault(x => x.Status == AttendanceStatus.Late)?.Count ?? 0;
            var leave = records.FirstOrDefault(x => x.Status == AttendanceStatus.Leave)?.Count ?? 0;
            var unmarked = Math.Max(studentIds.Length - records.Sum(x => x.Count), 0);

            return new StudentAttendanceSummaryDto
            {
                TotalStudents = studentIds.Length,
                Present = present + unmarked,
                Absent = absent,
                Late = late,
                Leave = leave
            };
        }

        public async Task<List<StudentAttendanceDto>> GetStudentHistoryAsync(
            int studentId,
            int year,
            int month,
            CancellationToken cancellationToken = default)
        {
            return await _db.Attendance
                .AsNoTracking()
                .Include(a => a.Student).ThenInclude(s => s!.Class)
                .Include(a => a.Student).ThenInclude(s => s!.Section)
                .Include(a => a.Student).ThenInclude(s => s!.StudentGroup)
                .Where(a => a.StudentId == studentId &&
                            a.AttendanceDate.Year == year &&
                            a.AttendanceDate.Month == month &&
                            !a.IsDeleted)
                .OrderByDescending(a => a.AttendanceDate)
                .Select(a => new StudentAttendanceDto
                {
                    Id = a.Id,
                    StudentId = a.StudentId,
                    StudentNo = a.Student != null ? a.Student.StudentNo : string.Empty,
                    StudentName = a.Student != null ? a.Student.FullName : string.Empty,
                    RollNumber = a.Student != null ? a.Student.RollNumber.ToString() : string.Empty,
                    ClassId = a.SchoolClassId,
                    ClassName = a.Student != null && a.Student.Class != null ? a.Student.Class.Name : string.Empty,
                    SectionId = a.SectionId,
                    SectionName = a.Student != null && a.Student.Section != null ? a.Student.Section.Name : string.Empty,
                    StudentGroupId = a.Student != null ? a.Student.StudentGroupId : null,
                    StudentGroupName = a.Student != null && a.Student.StudentGroup != null ? a.Student.StudentGroup.Name : string.Empty,
                    AttendanceDate = a.AttendanceDate.ToDateTime(TimeOnly.MinValue),
                    Status = a.Status,
                    StatusName = a.Status.ToString(),
                    Remarks = a.Remarks
                })
                .ToListAsync(cancellationToken);
        }

        private IQueryable<SchoolManagementSystem.Models.Entities.Student.Student> BuildFilteredStudents(StudentAttendanceFilterDto filter)
        {
            var query = _db.Students
                .AsNoTracking()
                .Include(s => s.Class)
                .Include(s => s.Section)
                .Include(s => s.StudentGroup)
                .Where(s => s.Status == StudentStatus.Active && !s.IsDeleted);

            if (filter.ClassId.HasValue)
            {
                query = query.Where(s => s.ClassId == filter.ClassId.Value);
            }

            if (filter.SectionId.HasValue)
            {
                query = query.Where(s => s.SectionId == filter.SectionId.Value);
            }

            if (filter.StudentGroupId.HasValue)
            {
                query = query.Where(s => s.StudentGroupId == filter.StudentGroupId.Value);
            }

            return query;
        }
    }

    public class EmployeeAttendanceRepository : BaseRepository<EmployeeAttendance>, IEmployeeAttendanceRepository
    {
        public EmployeeAttendanceRepository(SchoolDbContext context) : base(context) { }

        public async Task<bool> IsAttendanceExistsAsync(int employeeId, System.DateTime date, CancellationToken cancellationToken = default)
        {
            return await _set.AnyAsync(a => a.EmployeeId == employeeId && a.AttendanceDate.Date == date.Date && !a.IsDeleted, cancellationToken);
        }

        public async Task<(List<EmployeeAttendanceDto> Items, int TotalRecords)> GetAttendanceGridAsync(
            EmployeeAttendanceFilterDto filter,
            int page,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            var date = filter.AttendanceDate.Date;
            var employees = BuildFilteredEmployees(filter);
            var totalRecords = await employees.CountAsync(cancellationToken);

            var pagedEmployees = await employees
                .OrderBy(e => e.FullName)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(e => new
                {
                    e.Id,
                    e.EmployeeCode,
                    e.FullName,
                    e.EmployeeType,
                    e.IsTeachingStaff,
                    Department = e.Department != null ? e.Department.Name : string.Empty,
                    Designation = e.Designation != null ? e.Designation.Name : string.Empty
                })
                .ToListAsync(cancellationToken);

            var employeeIds = pagedEmployees.Select(e => e.Id).ToArray();
            var attendanceByEmployee = await _set
                .AsNoTracking()
                .Where(a => a.AttendanceDate.Date == date && employeeIds.Contains(a.EmployeeId))
                .ToDictionaryAsync(a => a.EmployeeId, cancellationToken);

            var rows = pagedEmployees.Select(employee =>
            {
                attendanceByEmployee.TryGetValue(employee.Id, out var attendance);
                var status = attendance?.Status ?? AttendanceStatus.Present;

                return new EmployeeAttendanceDto
                {
                    Id = attendance?.Id ?? 0,
                    EmployeeId = employee.Id,
                    EmployeeCode = employee.EmployeeCode,
                    EmployeeName = employee.FullName,
                    Department = employee.Department,
                    Designation = employee.Designation,
                    EmployeeType = employee.EmployeeType,
                    IsTeachingStaff = employee.IsTeachingStaff,
                    AttendanceDate = date,
                    CheckInTime = attendance?.CheckInTime,
                    CheckOutTime = attendance?.CheckOutTime,
                    Status = status,
                    StatusName = status.ToString(),
                    Remarks = attendance?.Remarks
                };
            }).ToList();

            return (rows, totalRecords);
        }

        public async Task<EmployeeAttendanceSummaryDto> GetAttendanceSummaryAsync(
            EmployeeAttendanceFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            var date = filter.AttendanceDate.Date;
            var employeeIds = await BuildFilteredEmployees(filter)
                .Select(e => e.Id)
                .ToArrayAsync(cancellationToken);

            var records = await _set
                .AsNoTracking()
                .Where(a => a.AttendanceDate.Date == date && employeeIds.Contains(a.EmployeeId))
                .GroupBy(a => a.Status)
                .Select(g => new { Status = g.Key, Count = g.Count() })
                .ToListAsync(cancellationToken);

            var present = records.FirstOrDefault(x => x.Status == AttendanceStatus.Present)?.Count ?? 0;
            var absent = records.FirstOrDefault(x => x.Status == AttendanceStatus.Absent)?.Count ?? 0;
            var late = records.FirstOrDefault(x => x.Status == AttendanceStatus.Late)?.Count ?? 0;
            var leave = records.FirstOrDefault(x => x.Status == AttendanceStatus.Leave)?.Count ?? 0;
            var unmarked = Math.Max(employeeIds.Length - records.Sum(x => x.Count), 0);

            return new EmployeeAttendanceSummaryDto
            {
                TotalEmployees = employeeIds.Length,
                Present = present + unmarked,
                Absent = absent,
                Late = late,
                Leave = leave
            };
        }

        public async Task<List<EmployeeAttendanceDto>> GetEmployeeHistoryAsync(
            int employeeId,
            int year,
            int month,
            CancellationToken cancellationToken = default)
        {
            return await _set
                .AsNoTracking()
                .Include(a => a.Employee).ThenInclude(e => e!.Department)
                .Include(a => a.Employee).ThenInclude(e => e!.Designation)
                .Where(a => a.EmployeeId == employeeId &&
                            a.AttendanceDate.Year == year &&
                            a.AttendanceDate.Month == month)
                .OrderByDescending(a => a.AttendanceDate)
                .Select(a => new EmployeeAttendanceDto
                {
                    Id = a.Id,
                    EmployeeId = a.EmployeeId,
                    EmployeeCode = a.Employee != null ? a.Employee.EmployeeCode : string.Empty,
                    EmployeeName = a.Employee != null ? a.Employee.FullName : string.Empty,
                    Department = a.Employee != null && a.Employee.Department != null ? a.Employee.Department.Name : string.Empty,
                    Designation = a.Employee != null && a.Employee.Designation != null ? a.Employee.Designation.Name : string.Empty,
                    EmployeeType = a.Employee != null ? a.Employee.EmployeeType : string.Empty,
                    IsTeachingStaff = a.Employee != null && a.Employee.IsTeachingStaff,
                    AttendanceDate = a.AttendanceDate,
                    CheckInTime = a.CheckInTime,
                    CheckOutTime = a.CheckOutTime,
                    Status = a.Status,
                    StatusName = a.Status.ToString(),
                    Remarks = a.Remarks
                })
                .ToListAsync(cancellationToken);
        }

        private IQueryable<SchoolManagementSystem.Models.Entities.Employee.Employee> BuildFilteredEmployees(EmployeeAttendanceFilterDto filter)
        {
            var query = _db.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .Include(e => e.Designation)
                .Where(e => e.Status == "Active" && !e.IsDeleted);

            if (filter.DepartmentId.HasValue)
            {
                query = query.Where(e => e.DepartmentId == filter.DepartmentId.Value);
            }

            if (filter.DesignationId.HasValue)
            {
                query = query.Where(e => e.DesignationId == filter.DesignationId.Value);
            }

            if (!string.IsNullOrWhiteSpace(filter.EmployeeType))
            {
                query = query.Where(e => e.EmployeeType == filter.EmployeeType);
            }

            if (filter.IsTeachingStaff.HasValue)
            {
                query = query.Where(e => e.IsTeachingStaff == filter.IsTeachingStaff.Value);
            }

            return query;
        }
    }

    public class LeaveTypeRepository : BaseRepository<LeaveType>, ILeaveTypeRepository
    {
        public LeaveTypeRepository(SchoolDbContext context) : base(context) { }
    }

    public class LeaveApplicationRepository : BaseRepository<LeaveApplication>, ILeaveApplicationRepository
    {
        public LeaveApplicationRepository(SchoolDbContext context) : base(context) { }
    }

    public class AttendanceSettingRepository : BaseRepository<AttendanceSetting>, IAttendanceSettingRepository
    {
        public AttendanceSettingRepository(SchoolDbContext context) : base(context) { }

        public async Task<AttendanceSetting?> GetCurrentSettingsAsync(CancellationToken cancellationToken = default)
        {
            return await _set.FirstOrDefaultAsync(cancellationToken);
        }
    }

    public class AttendanceLogRepository : BaseRepository<AttendanceLog>, IAttendanceLogRepository
    {
        public AttendanceLogRepository(SchoolDbContext context) : base(context) { }
    }

    public class AttendanceRevisionRepository : BaseRepository<AttendanceRevision>
    {
        public AttendanceRevisionRepository(SchoolDbContext context) : base(context) { }
    }
}
