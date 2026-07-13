using System.Data;
using System.Data.Common;
using System.Text.Json;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Repositories.Interfaces.Result;

namespace SchoolManagementSystem.Repositories.Implementations.Result;

public class ExamWizardRepository : IExamWizardRepository
{
    private readonly SchoolDbContext _db;
    private readonly string _connectionString;

    public ExamWizardRepository(SchoolDbContext db)
    {
        _db = db;
        _connectionString = db.Database.GetConnectionString()!;
    }

    // DateOnly/TimeOnly extension methods for SqlDataReader
    private static DateOnly GetDateOnly(SqlDataReader reader, int ordinal)
    {
        var dateTime = reader.GetDateTime(ordinal);
        return DateOnly.FromDateTime(dateTime);
    }

    private static TimeOnly GetTimeOnly(SqlDataReader reader, int ordinal)
    {
        var timeSpan = reader.GetTimeSpan(ordinal);
        return TimeOnly.FromTimeSpan(timeSpan);
    }

    private static DateOnly GetDateOnly(DbDataReader reader, int ordinal)
    {
        var dateTime = reader.GetDateTime(ordinal);
        return DateOnly.FromDateTime(dateTime);
    }

    private static TimeOnly GetTimeOnly(DbDataReader reader, int ordinal)
    {
        var timeSpan = reader.GetFieldValue<TimeSpan>(ordinal);
        return TimeOnly.FromTimeSpan(timeSpan);
    }

    public async Task<ExamCreationPreviewDto> GetExamCreationPreviewAsync(int academicYearId, List<int> classIds, CancellationToken ct = default)
    {
        var classIdsJson = JsonSerializer.Serialize(classIds);
        var result = new ExamCreationPreviewDto();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_GetExamCreationPreview", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 120
        };
        command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
        command.Parameters.AddWithValue("@SelectedClassIds", classIdsJson);

        await using var reader = await command.ExecuteReaderAsync(ct);

        // Result Set 1: Class Hierarchy
        var classHierarchy = new List<ExamClassHierarchyItemDto>();
        while (await reader.ReadAsync(ct))
        {
            classHierarchy.Add(MapClassHierarchyItem(reader));
        }
        result.ClassHierarchy = classHierarchy;

        // Result Set 2: Components
        if (await reader.NextResultAsync(ct))
        {
            var components = new List<ExamSubjectComponentDto>();
            while (await reader.ReadAsync(ct))
            {
                components.Add(MapSubjectComponent(reader));
            }
            result.Components = components;
        }

        // Result Set 3: Teacher Assignments
        if (await reader.NextResultAsync(ct))
        {
            var teachers = new List<ExamTeacherAssignmentDto>();
            while (await reader.ReadAsync(ct))
            {
                teachers.Add(MapTeacherAssignment(reader));
            }
            result.TeacherAssignments = teachers;
        }

        // Result Set 4: Validation Summary
        if (await reader.NextResultAsync(ct))
        {
            var validation = new List<ExamClassValidationDto>();
            while (await reader.ReadAsync(ct))
            {
                validation.Add(MapClassValidation(reader));
            }
            result.ClassValidations = validation;
        }

        // Result Set 5: Statistics
        if (await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
        {
            result.Statistics = new ExamStatisticsDto
            {
                TotalClasses = reader.GetInt32(reader.GetOrdinal("TotalClasses")),
                TotalSections = reader.GetInt32(reader.GetOrdinal("TotalSections")),
                TotalSubjects = reader.GetInt32(reader.GetOrdinal("TotalSubjects")),
                TotalComponents = reader.GetInt32(reader.GetOrdinal("TotalComponents")),
                TotalTeachersAssigned = reader.GetInt32(reader.GetOrdinal("TotalTeachersAssigned"))
            };
        }

        return result;
    }

    public async Task<ExamClassHierarchyDto> GetExamClassHierarchyAsync(int academicYearId, List<int> classIds, CancellationToken ct = default)
    {
        var classIdsJson = JsonSerializer.Serialize(classIds);
        var result = new ExamClassHierarchyDto { Items = new List<ExamClassHierarchyItemDto>() };

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_GetExamClassHierarchy", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 120
        };
        command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
        command.Parameters.AddWithValue("@ClassIds", classIdsJson);

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            result.Items.Add(MapClassHierarchyItem(reader));
        }

        return result;
    }

    public async Task<List<ExamTeacherAssignmentDto>> GetExamTeacherAssignmentsAsync(int academicYearId, List<int> classIds, CancellationToken ct = default)
    {
        var classIdsJson = JsonSerializer.Serialize(classIds);
        var result = new List<ExamTeacherAssignmentDto>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_GetExamTeacherAssignments", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };
        command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
        command.Parameters.AddWithValue("@ClassIds", classIdsJson);

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            result.Add(MapTeacherAssignment(reader));
        }

        return result;
    }

    public async Task<ExamValidationResultDto> GetExamValidationAsync(int academicYearId, string examName, int examTerm, List<int> classIds, DateOnly startDate, DateOnly endDate, CancellationToken ct = default)
    {
        var classIdsJson = JsonSerializer.Serialize(classIds);
        var result = new ExamValidationResultDto { Messages = new List<ExamValidationMessageDto>() };

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_GetExamValidation", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };
        command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
        command.Parameters.AddWithValue("@ExamName", examName);
        command.Parameters.AddWithValue("@ExamTerm", examTerm);
        command.Parameters.AddWithValue("@ClassIds", classIdsJson);
        command.Parameters.AddWithValue("@StartDate", startDate);
        command.Parameters.AddWithValue("@EndDate", endDate);

        await using var reader = await command.ExecuteReaderAsync(ct);

        // Result Set 1: Validation Messages
        while (await reader.ReadAsync(ct))
        {
            result.Messages.Add(new ExamValidationMessageDto
            {
                Severity = reader.GetString(reader.GetOrdinal("Severity")),
                Category = reader.GetString(reader.GetOrdinal("Category")),
                Message = reader.GetString(reader.GetOrdinal("Message")),
                ClassId = reader.IsDBNull(reader.GetOrdinal("ClassId")) ? null : reader.GetInt32(reader.GetOrdinal("ClassId")),
                SectionId = reader.IsDBNull(reader.GetOrdinal("SectionId")) ? null : reader.GetInt32(reader.GetOrdinal("SectionId")),
                SubjectId = reader.IsDBNull(reader.GetOrdinal("SubjectId")) ? null : reader.GetInt32(reader.GetOrdinal("SubjectId")),
                FixAction = reader.IsDBNull(reader.GetOrdinal("FixAction")) ? null : reader.GetString(reader.GetOrdinal("FixAction"))
            });
        }

        // Result Set 2: Readiness Score
        if (await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
        {
            result.TotalClasses = reader.GetInt32(reader.GetOrdinal("TotalClasses"));
            result.ReadyClasses = reader.GetInt32(reader.GetOrdinal("ReadyClasses"));
            result.NotReadyClasses = reader.GetInt32(reader.GetOrdinal("NotReadyClasses"));
            result.ReadinessPercentage = reader.GetDecimal(reader.GetOrdinal("ReadinessPercentage"));
            result.ErrorCount = reader.GetInt32(reader.GetOrdinal("ErrorCount"));
            result.WarningCount = reader.GetInt32(reader.GetOrdinal("WarningCount"));
            result.Is100PercentReady = reader.GetInt32(reader.GetOrdinal("Is100PercentReady")) == 1;
        }

        return result;
    }

    public async Task<ExamCreateResultDto> CreateExamHierarchyAsync(SchoolManagementSystem.Models.DTOs.Exam.ExamCreateHierarchyRequest request, CancellationToken ct = default)
    {
        var classIdsJson = JsonSerializer.Serialize(request.ClassIds);

        var result = new ExamCreateResultDto { CreatedExamIds = new List<int>(), CreatedExamNames = new List<string>() };

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_CreateExamHierarchy", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 180
        };
        command.Parameters.AddWithValue("@AcademicYearId", request.AcademicYearId);
        command.Parameters.AddWithValue("@ExamName", request.ExamName);
        command.Parameters.AddWithValue("@ExamTerm", request.ExamTerm);
        command.Parameters.AddWithValue("@ExamType", request.ExamType);
        command.Parameters.AddWithValue("@StartDate", request.StartDate);
        command.Parameters.AddWithValue("@EndDate", request.EndDate);
        command.Parameters.AddWithValue("@ClassIds", classIdsJson);
        command.Parameters.AddWithValue("@Subjects", request.SubjectsJson);
        command.Parameters.AddWithValue("@UserId", request.UserId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            result.Success = true;
            result.Message = reader.GetString(reader.GetOrdinal("Status"));
            result.ExamId = reader.GetInt32(reader.GetOrdinal("ExamId"));
            result.CreatedExamIds.Add(reader.GetInt32(reader.GetOrdinal("ExamId")));
            result.CreatedExamNames.Add(reader.GetString(reader.GetOrdinal("ExamName")));
        }
        else
        {
            result.Success = false;
            result.Message = "Failed to create exam hierarchy.";
        }

        return result;
    }

    public async Task<ExamCreationReadinessDto> GetExamReadinessAsync(int academicYearId, List<int> classIds, CancellationToken ct = default)
    {
        var classIdsJson = JsonSerializer.Serialize(classIds);
        var result = new ExamCreationReadinessDto();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_GetExamReadiness", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };
        command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
        command.Parameters.AddWithValue("@ClassIds", classIdsJson);

        await using var reader = await command.ExecuteReaderAsync(ct);

        // Result Set 1: Class readiness details (skip for now)
        while (await reader.ReadAsync(ct))
        {
            // Could collect per-class readiness if needed
        }

        // Result Set 2: Overall readiness
        if (await reader.NextResultAsync(ct) && await reader.ReadAsync(ct))
        {
            result.TotalClasses = reader.GetInt32(reader.GetOrdinal("TotalClasses"));
            result.TotalSections = reader.GetInt32(reader.GetOrdinal("TotalSections"));
            result.TotalSubjects = reader.GetInt32(reader.GetOrdinal("TotalSubjects"));
            result.TotalComponents = reader.GetInt32(reader.GetOrdinal("TotalComponents"));
            result.TeachersAssigned = reader.GetInt32(reader.GetOrdinal("TeachersAssigned"));
            result.TeachersMissing = reader.GetInt32(reader.GetOrdinal("TeachersMissing"));
            result.ReadinessPercentage = reader.GetDecimal(reader.GetOrdinal("ReadinessPercentage"));
        }

        return result;
    }

    public async Task<ExamStatisticsDto> GetExamStatisticsAsync(int academicYearId, List<int> classIds, CancellationToken ct = default)
    {
        var classIdsJson = JsonSerializer.Serialize(classIds);
        var result = new ExamStatisticsDto();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_GetExamStatistics", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };
        command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
        command.Parameters.AddWithValue("@ClassIds", classIdsJson);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            result.StudentCount = reader.GetInt32(reader.GetOrdinal("StudentCount"));
            result.SectionCount = reader.GetInt32(reader.GetOrdinal("SectionCount"));
            result.SubjectCount = reader.GetInt32(reader.GetOrdinal("SubjectCount"));
            result.ComponentCount = reader.GetInt32(reader.GetOrdinal("ComponentCount"));
            result.TeacherCount = reader.GetInt32(reader.GetOrdinal("TeacherCount"));
        }

        return result;
    }

    public async Task<ExamScheduleResultDto> GenerateExamScheduleAsync(int examId, DateOnly startDate, DateOnly endDate, string userId, CancellationToken ct = default)
    {
        var result = new ExamScheduleResultDto();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_GenerateExamSchedule", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 120
        };
        command.Parameters.AddWithValue("@ExamId", examId);
        command.Parameters.AddWithValue("@StartDate", startDate);
        command.Parameters.AddWithValue("@EndDate", endDate);
        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            result.Success = reader.GetString(reader.GetOrdinal("Status")) == "SUCCESS";
            result.Message = reader.GetString(reader.GetOrdinal("Message"));
            result.ScheduledCount = reader.GetInt32(reader.GetOrdinal("ScheduledCount"));
        }

        return result;
    }

    public async Task<List<ExamConflictDto>> GetExamConflictsAsync(int examId, CancellationToken ct = default)
    {
        var result = new List<ExamConflictDto>();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_GetExamConflicts", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };
        command.Parameters.AddWithValue("@ExamId", examId);

        await using var reader = await command.ExecuteReaderAsync(ct);

        // Result Set 1: Teacher Conflicts
        while (await reader.ReadAsync(ct))
        {
            result.Add(new ExamConflictDto
            {
                ConflictType = reader.GetString(reader.GetOrdinal("ConflictType")),
                Description = $"Teacher conflict: {reader.GetString(reader.GetOrdinal("TeacherName"))} scheduled for {reader.GetString(reader.GetOrdinal("Subject1"))} and {reader.GetString(reader.GetOrdinal("Subject2"))} at same time",
                Date = GetDateOnly(reader, reader.GetOrdinal("ExamDate")),
                StartTime = GetTimeOnly(reader, reader.GetOrdinal("StartsAt")),
                EndTime = GetTimeOnly(reader, reader.GetOrdinal("EndsAt"))
            });
        }

        // Result Set 2: Room Conflicts
        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                result.Add(new ExamConflictDto
                {
                    ConflictType = reader.GetString(reader.GetOrdinal("ConflictType")),
                    Description = $"Room conflict: Room {reader.GetString(reader.GetOrdinal("RoomNo"))} double-booked for {reader.GetString(reader.GetOrdinal("Subject1"))} and {reader.GetString(reader.GetOrdinal("Subject2"))}",
                    Date = GetDateOnly(reader, reader.GetOrdinal("ExamDate")),
                    StartTime = GetTimeOnly(reader, reader.GetOrdinal("StartsAt")),
                    EndTime = GetTimeOnly(reader, reader.GetOrdinal("EndsAt"))
                });
            }
        }

        // Result Set 3: Group Conflicts
        if (await reader.NextResultAsync(ct))
        {
            while (await reader.ReadAsync(ct))
            {
                result.Add(new ExamConflictDto
                {
                    ConflictType = reader.GetString(reader.GetOrdinal("ConflictType")),
                    Description = $"Student group conflict: {reader.GetString(reader.GetOrdinal("StudentGroup"))} has {reader.GetString(reader.GetOrdinal("Subject1"))} and {reader.GetString(reader.GetOrdinal("Subject2"))} at same time",
                    Date = GetDateOnly(reader, reader.GetOrdinal("ExamDate")),
                    StartTime = GetTimeOnly(reader, reader.GetOrdinal("StartsAt")),
                    EndTime = GetTimeOnly(reader, reader.GetOrdinal("EndsAt"))
                });
            }
        }

        return result;
    }

    // ──────────────────────────────────────────────
    // Fix Issues Methods
    // ──────────────────────────────────────────────

    public async Task<ExamFixResultDto> AssignTeacherToExamSubjectAsync(int academicYearId, int subjectId, int classId, int? sectionId, int? studentGroupId, int teacherId, string userId, CancellationToken ct = default)
    {
        var result = new ExamFixResultDto();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_AssignTeacherToExamSubject", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };
        command.Parameters.AddWithValue("@AcademicYearId", academicYearId);
        command.Parameters.AddWithValue("@SubjectId", subjectId);
        command.Parameters.AddWithValue("@ClassId", classId);
        command.Parameters.AddWithValue("@SectionId", sectionId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@StudentGroupId", studentGroupId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@TeacherId", teacherId);
        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            result.Success = true;
            result.Message = reader.GetString(reader.GetOrdinal("Status"));
            result.AssignmentId = reader.GetInt32(reader.GetOrdinal("AssignmentId"));
        }

        return result;
    }

    public async Task<ExamFixResultDto> ConfigureExamSubjectComponentsAsync(int examSubjectId, string componentsJson, string userId, CancellationToken ct = default)
    {
        var result = new ExamFixResultDto();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_ConfigureExamSubjectComponents", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };
        command.Parameters.AddWithValue("@ExamSubjectId", examSubjectId);
        command.Parameters.AddWithValue("@Components", componentsJson);
        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            result.Success = reader.GetString(reader.GetOrdinal("Status")) == "SUCCESS";
            result.Message = reader.GetString(reader.GetOrdinal("Message"));
        }

        return result;
    }

    public async Task<ExamFixResultDto> AddSectionsToClassAsync(int classId, string sectionNamesJson, int? studentGroupId, string userId, CancellationToken ct = default)
    {
        var result = new ExamFixResultDto();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_AddSectionsToClass", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };
        command.Parameters.AddWithValue("@ClassId", classId);
        command.Parameters.AddWithValue("@SectionNames", sectionNamesJson);
        command.Parameters.AddWithValue("@StudentGroupId", studentGroupId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            result.Success = reader.GetString(reader.GetOrdinal("Status")) == "SUCCESS";
            result.Message = $"Created {reader.GetInt32(reader.GetOrdinal("CreatedCount"))} sections";
            result.CreatedCount = reader.GetInt32(reader.GetOrdinal("CreatedCount"));
        }

        return result;
    }

    public async Task<ExamFixResultDto> MapSubjectToClassAsync(int subjectId, int classId, int? studentGroupId, decimal fullMarks, decimal passMarks, bool isOptional, int displayOrder, string userId, CancellationToken ct = default)
    {
        var result = new ExamFixResultDto();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_MapSubjectToClass", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };
        command.Parameters.AddWithValue("@SubjectId", subjectId);
        command.Parameters.AddWithValue("@ClassId", classId);
        command.Parameters.AddWithValue("@StudentGroupId", studentGroupId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@FullMarks", fullMarks);
        command.Parameters.AddWithValue("@PassMarks", passMarks);
        command.Parameters.AddWithValue("@IsOptional", isOptional);
        command.Parameters.AddWithValue("@DisplayOrder", displayOrder);
        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            result.Success = reader.GetString(reader.GetOrdinal("Status")) != "EXISTS";
            result.Message = reader.GetString(reader.GetOrdinal("Status"));
            if (!reader.IsDBNull(reader.GetOrdinal("ClassSubjectId")))
            {
                result.CreatedCount = 1;
            }
        }

        return result;
    }

    public async Task<ExamFixResultDto> ConfigureSubjectMarkStructureAsync(int subjectId, int? classId, int? studentGroupId, string componentsJson, string userId, CancellationToken ct = default)
    {
        var result = new ExamFixResultDto();

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_ConfigureSubjectMarkStructure", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };
        command.Parameters.AddWithValue("@SubjectId", subjectId);
        command.Parameters.AddWithValue("@ClassId", classId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@StudentGroupId", studentGroupId ?? (object)DBNull.Value);
        command.Parameters.AddWithValue("@Components", componentsJson);
        command.Parameters.AddWithValue("@UserId", userId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        if (await reader.ReadAsync(ct))
        {
            result.Success = reader.GetString(reader.GetOrdinal("Status")) == "SUCCESS";
            result.Message = reader.GetString(reader.GetOrdinal("Message"));
            result.CreatedCount = reader.GetInt32(reader.GetOrdinal("ConfiguredCount"));
        }

        return result;
    }

    public async Task<ExamPublishReadinessDto> CheckExamPublishReadinessAsync(int examId, CancellationToken ct = default)
    {
        var result = new ExamPublishReadinessDto { IsReady = false, Blockers = new List<string>() };

        await using var connection = new SqlConnection(_connectionString);
        await connection.OpenAsync(ct);
        await using var command = new SqlCommand("sp_CheckExamPublishReadiness", connection)
        {
            CommandType = CommandType.StoredProcedure,
            CommandTimeout = 60
        };
        command.Parameters.AddWithValue("@ExamId", examId);

        await using var reader = await command.ExecuteReaderAsync(ct);
        while (await reader.ReadAsync(ct))
        {
            result.IsReady = reader.GetBoolean(reader.GetOrdinal("IsReady"));
            if (!result.IsReady)
            {
                result.Blockers.Add(reader.GetString(reader.GetOrdinal("Blocker")));
            }
            else
            {
                result.Blockers.Add(reader.GetString(reader.GetOrdinal("Blocker")));
            }
        }

        return result;
    }

    // ──────────────────────────────────────────────
    // Mapping Helpers
    // ──────────────────────────────────────────────

    private static ExamClassHierarchyItemDto MapClassHierarchyItem(DbDataReader reader)
    {
        return new ExamClassHierarchyItemDto
        {
            ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
            ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
            IsGroupBased = reader.GetBoolean(reader.GetOrdinal("IsGroupBased")),
            ClassSortOrder = reader.GetInt32(reader.GetOrdinal("ClassSortOrder")),
            SectionId = reader.IsDBNull(reader.GetOrdinal("SectionId")) ? null : reader.GetInt32(reader.GetOrdinal("SectionId")),
            SectionName = reader.IsDBNull(reader.GetOrdinal("SectionName")) ? null : reader.GetString(reader.GetOrdinal("SectionName")),
            ParentSectionId = reader.IsDBNull(reader.GetOrdinal("ParentSectionId")) ? null : reader.GetInt32(reader.GetOrdinal("ParentSectionId")),
            StudentGroupId = reader.IsDBNull(reader.GetOrdinal("StudentGroupId")) ? null : reader.GetInt32(reader.GetOrdinal("StudentGroupId")),
            StudentGroupName = reader.IsDBNull(reader.GetOrdinal("StudentGroupName")) ? null : reader.GetString(reader.GetOrdinal("StudentGroupName")),
            StudentGroupCode = reader.IsDBNull(reader.GetOrdinal("StudentGroupCode")) ? null : reader.GetString(reader.GetOrdinal("StudentGroupCode")),
            ClassSubjectId = reader.IsDBNull(reader.GetOrdinal("ClassSubjectId")) ? null : reader.GetInt32(reader.GetOrdinal("ClassSubjectId")),
            SubjectId = reader.IsDBNull(reader.GetOrdinal("SubjectId")) ? null : reader.GetInt32(reader.GetOrdinal("SubjectId")),
            SubjectCode = reader.IsDBNull(reader.GetOrdinal("SubjectCode")) ? null : reader.GetString(reader.GetOrdinal("SubjectCode")),
            SubjectName = reader.IsDBNull(reader.GetOrdinal("SubjectName")) ? null : reader.GetString(reader.GetOrdinal("SubjectName")),
            SubjectNameBn = reader.IsDBNull(reader.GetOrdinal("SubjectNameBn")) ? null : reader.GetString(reader.GetOrdinal("SubjectNameBn")),
            SubjectCategory = reader.IsDBNull(reader.GetOrdinal("SubjectCategory")) ? null : reader.GetString(reader.GetOrdinal("SubjectCategory")),
            SubjectGroupName = reader.IsDBNull(reader.GetOrdinal("SubjectGroupName")) ? null : reader.GetString(reader.GetOrdinal("SubjectGroupName")),
            IsMandatory = reader.GetBoolean(reader.GetOrdinal("IsMandatory")),
            IsOptional = reader.GetBoolean(reader.GetOrdinal("IsOptional")),
            IsReligionSubject = reader.GetBoolean(reader.GetOrdinal("IsReligionSubject")),
            IsPractical = reader.GetBoolean(reader.GetOrdinal("IsPractical")),
            ReligionType = reader.IsDBNull(reader.GetOrdinal("ReligionType")) ? null : reader.GetString(reader.GetOrdinal("ReligionType")),
            DefaultFullMarks = reader.GetDecimal(reader.GetOrdinal("DefaultFullMarks")),
            DefaultPassMarks = reader.GetDecimal(reader.GetOrdinal("DefaultPassMarks")),
            TheoryMarks = reader.GetDecimal(reader.GetOrdinal("TheoryMarks")),
            PracticalMarks = reader.GetDecimal(reader.GetOrdinal("PracticalMarks")),
            Credit = reader.GetDecimal(reader.GetOrdinal("Credit")),
            NctbCode = reader.IsDBNull(reader.GetOrdinal("NctbCode")) ? null : reader.GetString(reader.GetOrdinal("NctbCode")),
            ClassSubjectFullMarks = reader.IsDBNull(reader.GetOrdinal("ClassSubjectFullMarks")) ? null : reader.GetDecimal(reader.GetOrdinal("ClassSubjectFullMarks")),
            ClassSubjectPassMarks = reader.IsDBNull(reader.GetOrdinal("ClassSubjectPassMarks")) ? null : reader.GetDecimal(reader.GetOrdinal("ClassSubjectPassMarks")),
            ClassSubjectIsOptional = reader.GetBoolean(reader.GetOrdinal("ClassSubjectIsOptional"))
        };
    }

    private static ExamSubjectComponentDto MapSubjectComponent(DbDataReader reader)
    {
        return new ExamSubjectComponentDto
        {
            SubjectMarkStructureId = reader.GetInt32(reader.GetOrdinal("SubjectMarkStructureId")),
            SubjectId = reader.GetInt32(reader.GetOrdinal("SubjectId")),
            ClassId = reader.IsDBNull(reader.GetOrdinal("ClassId")) ? null : reader.GetInt32(reader.GetOrdinal("ClassId")),
            StudentGroupId = reader.IsDBNull(reader.GetOrdinal("StudentGroupId")) ? null : reader.GetInt32(reader.GetOrdinal("StudentGroupId")),
            ComponentId = reader.GetInt32(reader.GetOrdinal("ComponentId")),
            ComponentCode = reader.GetString(reader.GetOrdinal("ComponentCode")),
            ComponentName = reader.GetString(reader.GetOrdinal("ComponentName")),
            ComponentDescription = reader.IsDBNull(reader.GetOrdinal("ComponentDescription")) ? null : reader.GetString(reader.GetOrdinal("ComponentDescription")),
            IsPractical = reader.GetBoolean(reader.GetOrdinal("IsPractical")),
            ComponentIsOptional = reader.GetBoolean(reader.GetOrdinal("ComponentIsOptional")),
            ComponentDisplayOrder = reader.GetInt32(reader.GetOrdinal("ComponentDisplayOrder")),
            DefaultFullMarks = reader.GetDecimal(reader.GetOrdinal("DefaultFullMarks")),
            DefaultPassMarks = reader.GetDecimal(reader.GetOrdinal("DefaultPassMarks")),
            FullMarks = reader.GetDecimal(reader.GetOrdinal("FullMarks")),
            PassMarks = reader.GetDecimal(reader.GetOrdinal("PassMarks")),
            StructureDisplayOrder = reader.GetInt32(reader.GetOrdinal("ComponentDisplayOrder")),
            StructureIsActive = reader.GetBoolean(reader.GetOrdinal("StructureIsActive"))
        };
    }

    private static ExamTeacherAssignmentDto MapTeacherAssignment(DbDataReader reader)
    {
        return new ExamTeacherAssignmentDto
        {
            TeacherId = reader.GetInt32(reader.GetOrdinal("TeacherId")),
            SubjectId = reader.GetInt32(reader.GetOrdinal("SubjectId")),
            ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
            SectionId = reader.IsDBNull(reader.GetOrdinal("SectionId")) ? null : reader.GetInt32(reader.GetOrdinal("SectionId")),
            StudentGroupId = reader.IsDBNull(reader.GetOrdinal("StudentGroupId")) ? null : reader.GetInt32(reader.GetOrdinal("StudentGroupId")),
            AcademicYearId = reader.GetInt32(reader.GetOrdinal("AcademicYearId")),
            EmployeeId = reader.IsDBNull(reader.GetOrdinal("EmployeeId")) ? null : reader.GetInt32(reader.GetOrdinal("EmployeeId")),
            EmployeeCode = reader.IsDBNull(reader.GetOrdinal("EmployeeCode")) ? null : reader.GetString(reader.GetOrdinal("EmployeeCode")),
            TeacherName = reader.IsDBNull(reader.GetOrdinal("TeacherName")) ? null : reader.GetString(reader.GetOrdinal("TeacherName")),
            TeacherEmail = reader.IsDBNull(reader.GetOrdinal("TeacherEmail")) ? null : reader.GetString(reader.GetOrdinal("TeacherEmail")),
            IsMissingTeacher = reader.GetBoolean(reader.GetOrdinal("IsMissingTeacher"))
        };
    }

    private static ExamClassValidationDto MapClassValidation(DbDataReader reader)
    {
        return new ExamClassValidationDto
        {
            ClassId = reader.GetInt32(reader.GetOrdinal("ClassId")),
            ClassName = reader.GetString(reader.GetOrdinal("ClassName")),
            SectionCount = reader.GetInt32(reader.GetOrdinal("SectionCount")),
            SubjectCount = reader.GetInt32(reader.GetOrdinal("SubjectCount")),
            ComponentCount = reader.GetInt32(reader.GetOrdinal("ComponentCount")),
            TeacherCount = reader.GetInt32(reader.GetOrdinal("TeacherCount")),
            MissingTeacherCount = reader.GetInt32(reader.GetOrdinal("MissingTeacherCount")),
            ValidationStatus = reader.GetString(reader.GetOrdinal("ValidationStatus")),
            IsReady = reader.GetBoolean(reader.GetOrdinal("IsReady"))
        };
    }

    private static ExamConflictDto MapConflict(DbDataReader reader)
    {
        return new ExamConflictDto
        {
            ConflictType = reader.GetString(reader.GetOrdinal("ConflictType")),
            Description = reader.GetString(reader.GetOrdinal("Description")),
            Date = GetDateOnly(reader, reader.GetOrdinal("ConflictDate")),
            StartTime = GetTimeOnly(reader, reader.GetOrdinal("StartsAt")),
            EndTime = GetTimeOnly(reader, reader.GetOrdinal("EndsAt"))
        };
    }

}