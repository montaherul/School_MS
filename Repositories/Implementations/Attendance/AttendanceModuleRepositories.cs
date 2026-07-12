using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Attendance;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.Entities.Attendance;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Repositories.Interfaces.Attendance;
using System.Data;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            var items = new List<StudentAttendanceDto>();
            int totalRecords = 0;

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "sp_GetStudentAttendanceList";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@PageNumber", page));
                command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
                command.Parameters.Add(new SqlParameter("@SearchTerm", (object?)filter.SearchTerm ?? DBNull.Value));
                command.Parameters.Add(new SqlParameter("@ClassId", filter.ClassId ?? 0));
                command.Parameters.Add(new SqlParameter("@SectionId", filter.SectionId ?? 0));
                command.Parameters.Add(new SqlParameter("@StudentGroupId", filter.StudentGroupId ?? 0));
                command.Parameters.Add(new SqlParameter("@AttendanceDate", filter.AttendanceDate.Date));
                command.Parameters.Add(new SqlParameter("@Status", filter.Status ?? 0));

                if (command.Connection!.State != ConnectionState.Open)
                    await _db.Database.OpenConnectionAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var dto = new StudentAttendanceDto
                        {
                            Id = reader.GetInt32(0),
                            StudentId = reader.GetInt32(1),
                            StudentNo = reader.IsDBNull(2) ? "" : reader.GetValue(2).ToString(),
                            StudentName = reader.IsDBNull(3) ? "" : reader.GetValue(3).ToString(),
                            RollNumber = reader.IsDBNull(4) ? "" : reader.GetValue(4).ToString(),
                            ClassId = reader.GetInt32(5),
                            ClassName = reader.IsDBNull(6) ? "" : reader.GetString(6),
                            SectionId = reader.GetInt32(7),
                            SectionName = reader.IsDBNull(8) ? "" : reader.GetString(8),
                            StudentGroupId = reader.IsDBNull(9) ? null : (int?)reader.GetInt32(9),
                            StudentGroupName = reader.IsDBNull(10) ? "" : reader.GetString(10),
                            AttendanceDate = reader.GetDateTime(11),
                            Status = (AttendanceStatus)reader.GetInt32(12),
                            Remarks = reader.IsDBNull(13) ? "" : reader.GetString(13)
                        };
                        dto.StatusName = dto.Status.ToString();
                        items.Add(dto);
                        totalRecords = reader.GetInt32(reader.FieldCount - 1); // TotalRecords is the last column
                    }
                }
            }

            return (items, totalRecords);
        }

        public async Task<StudentAttendanceSummaryDto> GetAttendanceSummaryAsync(
            StudentAttendanceFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            var summary = new StudentAttendanceSummaryDto();

            using (var command = _db.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "sp_GetAttendanceSummary";
                command.CommandType = CommandType.StoredProcedure;

                command.Parameters.Add(new SqlParameter("@StudentId", filter.StudentId ?? 0));
                command.Parameters.Add(new SqlParameter("@EmployeeId", 0));
                command.Parameters.Add(new SqlParameter("@ClassId", filter.ClassId ?? 0));
                command.Parameters.Add(new SqlParameter("@SectionId", filter.SectionId ?? 0));
                command.Parameters.Add(new SqlParameter("@StudentGroupId", filter.StudentGroupId ?? 0));
                command.Parameters.Add(new SqlParameter("@AttendanceDate", filter.AttendanceDate.Date));
                command.Parameters.Add(new SqlParameter("@Year", filter.AttendanceDate.Year));
                command.Parameters.Add(new SqlParameter("@Month", filter.AttendanceDate.Month));

                if (command.Connection!.State != ConnectionState.Open)
                    await _db.Database.OpenConnectionAsync(cancellationToken);

                using (var reader = await command.ExecuteReaderAsync(cancellationToken))
                {
                    while (await reader.ReadAsync(cancellationToken))
                    {
                        var statusId = reader.GetInt32(0);
                        var count = reader.GetInt32(1);

                        switch (statusId)
                        {
                            case (int)AttendanceStatus.Present: summary.Present = count; break;
                            case (int)AttendanceStatus.Absent: summary.Absent = count; break;
                            case (int)AttendanceStatus.Late: summary.Late = count; break;
                            case (int)AttendanceStatus.Leave: summary.Leave = count; break;
                        }
                    }
                }
            }

            summary.TotalStudents = summary.Present + summary.Absent + summary.Late + summary.Leave;
            return summary;
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
                    StudentNo = a.Student != null ? a.Student.StudentNo.ToString() : string.Empty,
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

        public async Task<(List<StudentAttendanceDto> Students, int Total)> GetStudentsForAttendanceBySpAsync(
            int classId, int sectionId, int? studentGroupId, DateTime attendanceDate,
            int page = 1, int pageSize = 50, CancellationToken cancellationToken = default)
        {
            var items = new List<StudentAttendanceDto>();
            var connection = _db.Database.GetDbConnection();
            await using var _ = await OpenConnectionAsync(connection, cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = "sp_GetStudentsForAttendance";
            command.CommandType = CommandType.StoredProcedure;
            AddParameter(command, "@ClassId", classId);
            AddParameter(command, "@SectionId", sectionId);
            AddParameter(command, "@StudentGroupId", studentGroupId);
            AddParameter(command, "@AttendanceDate", attendanceDate.Date);
            AddParameter(command, "@PageNumber", page);
            AddParameter(command, "@PageSize", pageSize);

            var total = 0;
            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            if (await reader.ReadAsync(cancellationToken))
            {
                total = GetInt32(reader, "TotalRecords");
            }

            if (await reader.NextResultAsync(cancellationToken))
            {
                while (await reader.ReadAsync(cancellationToken))
                {
                    items.Add(new StudentAttendanceDto
                    {
                        Id = GetInt32(reader, "Id"),
                        StudentId = GetInt32(reader, "StudentId"),
                        StudentNo = GetString(reader, "StudentNo"),
                        StudentName = GetString(reader, "StudentName"),
                        RollNumber = GetString(reader, "RollNumber"),
                        ClassId = GetInt32(reader, "ClassId"),
                        ClassName = GetString(reader, "ClassName"),
                        SectionId = GetInt32(reader, "SectionId"),
                        SectionName = GetString(reader, "SectionName"),
                        StudentGroupId = GetNullableInt32(reader, "StudentGroupId"),
                        StudentGroupName = GetString(reader, "StudentGroupName"),
                        AttendanceDate = GetDateTime(reader, "AttendanceDate"),
                        Status = (SchoolManagementSystem.Models.Enums.AttendanceStatus)GetInt32(reader, "Status"),
                        StatusName = GetString(reader, "StatusName"),
                        Remarks = GetString(reader, "Remarks")
                    });
                }
            }

            return (items, total);
        }

        public async Task<List<AttendanceForPromotionDto>> GetAttendanceForPromotionAsync(
            int academicYearId, int? classId = null, int? sectionId = null,
            decimal? minPercentage = null, CancellationToken cancellationToken = default)
        {
            var results = new List<AttendanceForPromotionDto>();
            var connection = _db.Database.GetDbConnection();
            await using var _ = await OpenConnectionAsync(connection, cancellationToken);
            await using var command = connection.CreateCommand();
            command.CommandText = "sp_GetAttendanceForPromotion";
            command.CommandType = CommandType.StoredProcedure;
            AddParameter(command, "@AcademicYearId", academicYearId);
            AddParameter(command, "@ClassId", classId);
            AddParameter(command, "@SectionId", sectionId);
            AddParameter(command, "@MinPercentage", minPercentage);

            await using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                results.Add(new AttendanceForPromotionDto
                {
                    StudentId = GetInt32(reader, "StudentId"),
                    StudentNo = GetString(reader, "StudentNo"),
                    FullName = GetString(reader, "FullName"),
                    RollNumber = GetInt32(reader, "RollNumber"),
                    ClassName = GetString(reader, "ClassName"),
                    SectionName = GetString(reader, "SectionName"),
                    TotalSchoolDays = GetInt32(reader, "TotalSchoolDays"),
                    PresentDays = GetInt32(reader, "PresentDays"),
                    AbsentDays = GetInt32(reader, "AbsentDays"),
                    LateDays = GetInt32(reader, "LateDays"),
                    LeaveDays = GetInt32(reader, "LeaveDays"),
                    AttendancePercentage = GetDecimal(reader, "AttendancePercentage"),
                    EligibilityStatus = GetString(reader, "EligibilityStatus")
                });
            }
            return results;
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
            var items = new List<EmployeeAttendanceDto>();
            int totalRecords = 0;

            using var connection = new SqlConnection(_db.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand("sp_GetEmployeeAttendanceList", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 60;

            command.Parameters.AddWithValue("@PageNumber", page);
            command.Parameters.AddWithValue("@PageSize", pageSize);
            command.Parameters.AddWithValue("@SearchTerm", (object?)filter.SearchTerm ?? DBNull.Value);
            command.Parameters.AddWithValue("@DepartmentId", filter.DepartmentId ?? 0);
            command.Parameters.AddWithValue("@DesignationId", filter.DesignationId ?? 0);
            command.Parameters.AddWithValue("@EmployeeType", (object?)filter.EmployeeType ?? DBNull.Value);
            command.Parameters.AddWithValue("@IsTeachingStaff", (object?)filter.IsTeachingStaff ?? DBNull.Value);
            command.Parameters.Add(new SqlParameter("@AttendanceDate", System.Data.SqlDbType.Date)
            { Value = filter.AttendanceDate.Date });
            command.Parameters.AddWithValue("@Status", filter.Status ?? 0);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var dto = new EmployeeAttendanceDto
                {
                    Id = reader.GetInt32(0),
                    EmployeeId = reader.GetInt32(1),
                    EmployeeCode = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    EmployeeName = reader.IsDBNull(3) ? "" : reader.GetString(3),
                    Department = reader.IsDBNull(5) ? "" : reader.GetString(5),
                    Designation = reader.IsDBNull(7) ? "" : reader.GetString(7),
                    EmployeeType = reader.IsDBNull(8) ? "" : reader.GetString(8),
                    IsTeachingStaff = !reader.IsDBNull(9) && reader.GetBoolean(9),
                    AttendanceDate = reader.GetDateTime(10),
                    CheckInTime = reader.IsDBNull(11) ? null : reader.GetFieldValue<TimeSpan>(11),
                    CheckOutTime = reader.IsDBNull(12) ? null : reader.GetFieldValue<TimeSpan>(12),
                    Status = (AttendanceStatus)reader.GetInt32(13),
                    Remarks = reader.IsDBNull(15) ? "" : reader.GetString(15),
                };
                dto.StatusName = reader.IsDBNull(14) ? dto.Status.ToString() : reader.GetString(14);
                items.Add(dto);
                totalRecords = reader.GetInt32(17);
            }

            return (items, totalRecords);
        }

        public async Task<EmployeeAttendanceSummaryDto> GetAttendanceSummaryAsync(
            EmployeeAttendanceFilterDto filter,
            CancellationToken cancellationToken = default)
        {
            var summary = new EmployeeAttendanceSummaryDto();

            using var connection = new SqlConnection(_db.Database.GetConnectionString());
            await connection.OpenAsync(cancellationToken);

            using var command = new SqlCommand("sp_GetAttendanceSummary", connection);
            command.CommandType = CommandType.StoredProcedure;
            command.CommandTimeout = 60;

            command.Parameters.AddWithValue("@StudentId", 0);
            command.Parameters.AddWithValue("@EmployeeId", filter.EmployeeId ?? -1);
            command.Parameters.AddWithValue("@ClassId", 0);
            command.Parameters.AddWithValue("@SectionId", 0);
            command.Parameters.AddWithValue("@StudentGroupId", 0);
            command.Parameters.AddWithValue("@AttendanceDate", filter.AttendanceDate.Date);
            command.Parameters.AddWithValue("@Year", filter.AttendanceDate.Year);
            command.Parameters.AddWithValue("@Month", filter.AttendanceDate.Month);

            using var reader = await command.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                var status = (AttendanceStatus)reader.GetInt32(0);
                var count = reader.GetInt32(1);
                switch (status)
                {
                    case AttendanceStatus.Present: summary.Present = count; break;
                    case AttendanceStatus.Absent: summary.Absent = count; break;
                    case AttendanceStatus.Late: summary.Late = count; break;
                    case AttendanceStatus.Leave: summary.Leave = count; break;
                }
            }

            summary.TotalEmployees = summary.Present + summary.Absent + summary.Late + summary.Leave;
            return summary;
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
        public async Task<bool> HasOverlappingLeaveAsync(
int employeeId,
DateTime fromDate,
DateTime toDate,
CancellationToken ct = default)
        {
            return await _db.LeaveApplications
                .AnyAsync(x =>
                    x.EmployeeId == employeeId &&
                    x.ApprovalStatus != LeaveStatus.Rejected &&
                    fromDate <= x.ToDate &&
                    toDate >= x.FromDate,
                    ct);
        }
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
