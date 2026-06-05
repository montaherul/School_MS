using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Guardian;
using SchoolManagementSystem.Models.Entities.Guardian;
using SchoolManagementSystem.Repositories.Guardian;
using System.Data;
using System.Data.Common;

namespace SchoolManagementSystem.Repositories.Implementations.Guardian;

public class GuardianRepository : BaseRepository<SchoolManagementSystem.Models.Entities.Guardian.Guardian>, IGuardianRepository
{
    private readonly SchoolDbContext _context;

    public GuardianRepository(SchoolDbContext context) : base(context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<GuardianListItemDto> Items, int TotalCount)> GetListAsync(string? searchTerm, string? status, int pageNumber, int pageSize)
    {
        var items = new List<GuardianListItemDto>();
        var connection = _context.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetGuardianList]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@SearchTerm", searchTerm);
        AddParameter(command, "@Status", status);
        AddParameter(command, "@PageNumber", pageNumber);
        AddParameter(command, "@PageSize", pageSize);

        var totalCount = 0;
        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            totalCount = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
        }

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                items.Add(new GuardianListItemDto
                {
                    Id = GetInt32(reader, "Id"),
                    GuardianCode = GetString(reader, "GuardianCode"),
                    FullName = GetString(reader, "FullName"),
                    MobileNumber = GetString(reader, "MobileNumber"),
                    Email = GetNullableString(reader, "Email"),
                    RelationType = GetString(reader, "RelationType"),
                    Status = GetString(reader, "Status"),
                    ChildrenCount = GetInt32(reader, "ChildrenCount"),
                    CreatedAt = GetDateTime(reader, "CreatedAt")
                });
            }
        }

        return (items, totalCount);
    }

    public async Task<GuardianDetailsDto?> GetDetailsAsync(int guardianId)
    {
        GuardianDetailsDto? guardian = null;
        var connection = _context.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetGuardianDetails]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@GuardianId", guardianId);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            guardian = new GuardianDetailsDto
            {
                Id = GetInt32(reader, "Id"),
                GuardianCode = GetString(reader, "GuardianCode"),
                FirstName = GetString(reader, "FirstName"),
                LastName = GetString(reader, "LastName"),
                FullName = GetString(reader, "FullName"),
                Gender = GetString(reader, "Gender"),
                DateOfBirth = GetNullableDateTime(reader, "DateOfBirth"),
                RelationType = GetString(reader, "RelationType"),
                NationalId = GetNullableString(reader, "NationalId"),
                PassportNumber = GetNullableString(reader, "PassportNumber"),
                Occupation = GetNullableString(reader, "Occupation"),
                EmployerName = GetNullableString(reader, "EmployerName"),
                MonthlyIncome = GetNullableDecimal(reader, "MonthlyIncome"),
                MobileNumber = GetString(reader, "MobileNumber"),
                AlternativeMobileNumber = GetNullableString(reader, "AlternativeMobileNumber"),
                Email = GetNullableString(reader, "Email"),
                PresentAddress = GetNullableString(reader, "PresentAddress"),
                PermanentAddress = GetNullableString(reader, "PermanentAddress"),
                PhotoPath = GetNullableString(reader, "PhotoPath"),
                EmergencyContactName = GetNullableString(reader, "EmergencyContactName"),
                EmergencyContactNumber = GetNullableString(reader, "EmergencyContactNumber"),
                PortalAccessEnabled = GetBoolean(reader, "PortalAccessEnabled"),
                Status = GetString(reader, "Status"),
                Remarks = GetNullableString(reader, "Remarks")
            };
        }

        if (guardian != null && await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                guardian.Children.Add(new GuardianChildDto
                {
                    StudentId = GetInt32(reader, "StudentId"),
                    StudentNo = GetString(reader, "StudentNo"),
                    FullName = GetString(reader, "FullName"),
                    ClassName = GetNullableString(reader, "ClassName") ?? "N/A",
                    SectionName = GetNullableString(reader, "SectionName") ?? "N/A",
                    RollNumber = GetString(reader, "RollNumber")
                });
            }
        }

        return guardian;
    }

    public async Task<GuardianDashboardDataDto> GetDashboardDataAsync(int guardianId)
    {
        var dashboard = new GuardianDashboardDataDto();
        var connection = _context.Database.GetDbConnection();
        await using var _ = await OpenConnectionAsync(connection);
        await using var command = connection.CreateCommand();
        command.CommandText = "[dbo].[sp_GetGuardianDashboard]";
        command.CommandType = CommandType.StoredProcedure;
        AddParameter(command, "@GuardianId", guardianId);

        await using var reader = await command.ExecuteReaderAsync();
        if (await reader.ReadAsync())
        {
            dashboard.TotalChildren = GetInt32(reader, "TotalChildren");
            dashboard.TotalOutstandingFees = GetDecimal(reader, "TotalOutstandingFees");
            dashboard.UnreadNotifications = GetInt32(reader, "UnreadNotifications");
        }

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                dashboard.ChildrenAttendance.Add(new GuardianChildAttendanceSummaryDto
                {
                    StudentId = GetInt32(reader, "StudentId"),
                    FullName = GetString(reader, "FullName"),
                    PresentCount = GetInt32(reader, "PresentCount"),
                    AbsentCount = GetInt32(reader, "AbsentCount"),
                    TotalDays = GetInt32(reader, "TotalDays")
                });
            }
        }

        if (await reader.NextResultAsync())
        {
            while (await reader.ReadAsync())
            {
                dashboard.RecentNotices.Add(new GuardianRecentNoticeDto
                {
                    Id = GetInt32(reader, "Id"),
                    Title = GetString(reader, "Title"),
                    Date = GetDateTime(reader, "PublishedAt"),
                    Category = GetString(reader, "Category"),
                    Excerpt = string.Empty
                });
            }
        }

        return dashboard;
    }

    public async Task<bool> IsCodeUniqueAsync(string code)
    {
        return !await _context.Guardians.AnyAsync(g => g.GuardianCode == code);
    }

    public async Task<bool> IsMobileUniqueAsync(string mobile)
    {
        return !await _context.Guardians.AnyAsync(g => g.MobileNumber == mobile);
    }

    public async Task<int> GetStudentCountAsync(int guardianId)
    {
        return await _context.StudentGuardians.CountAsync(sg => sg.GuardianId == guardianId);
    }

    public async Task LinkStudentAsync(int guardianId, int studentId, string relation)
    {
        var mapping = new StudentGuardian
        {
            StudentId = studentId,
            GuardianId = guardianId,
            Relationship = Enum.Parse<GuardianRelationshipType>(relation),
            IsPrimaryGuardian = !await _context.StudentGuardians.AnyAsync(sg => sg.StudentId == studentId && sg.IsPrimaryGuardian)
        };
        await _context.StudentGuardians.AddAsync(mapping);
    }

    private static void AddParameter(DbCommand command, string name, object? value)
    {
        var parameter = command.CreateParameter();
        parameter.ParameterName = name;
        parameter.Value = value ?? DBNull.Value;
        command.Parameters.Add(parameter);
    }

    private static async Task<IAsyncDisposable> OpenConnectionAsync(DbConnection connection)
    {
        var wasClosed = connection.State == ConnectionState.Closed;
        if (wasClosed) await connection.OpenAsync();
        return new ConnectionLease(connection, wasClosed);
    }

    private sealed class ConnectionLease : IAsyncDisposable
    {
        private readonly DbConnection _connection;
        private readonly bool _closeOnDispose;

        public ConnectionLease(DbConnection connection, bool closeOnDispose)
        {
            _connection = connection;
            _closeOnDispose = closeOnDispose;
        }

        public async ValueTask DisposeAsync()
        {
            if (_closeOnDispose) await _connection.CloseAsync();
        }
    }

    private static int GetOrdinal(DbDataReader reader, string name) => reader.GetOrdinal(name);
    private static string GetString(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? string.Empty : Convert.ToString(reader[name]) ?? string.Empty;
    private static string? GetNullableString(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? null : Convert.ToString(reader[name]);
    private static int GetInt32(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? 0 : Convert.ToInt32(reader[name]);
    private static decimal GetDecimal(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? 0m : Convert.ToDecimal(reader[name]);
    private static decimal? GetNullableDecimal(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? null : Convert.ToDecimal(reader[name]);
    private static bool GetBoolean(DbDataReader reader, string name) => !reader.IsDBNull(GetOrdinal(reader, name)) && Convert.ToBoolean(reader[name]);
    private static DateTime GetDateTime(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? DateTime.MinValue : Convert.ToDateTime(reader[name]);
    private static DateTime? GetNullableDateTime(DbDataReader reader, string name) => reader.IsDBNull(GetOrdinal(reader, name)) ? null : Convert.ToDateTime(reader[name]);
}
