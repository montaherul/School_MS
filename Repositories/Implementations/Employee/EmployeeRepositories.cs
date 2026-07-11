using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;
using System.Data;
using System.Data.Common;

namespace SchoolManagementSystem.Repositories.Implementations.Employee;

public class EmployeeRepository : BaseRepository<SchoolManagementSystem.Models.Entities.Employee.Employee>, IEmployeeRepository
{
    public EmployeeRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<EmployeeListItemDto> items, int totalRecords)> GetPagedAsync(
        int page, int pageSize, string? search, int? departmentId, int? designationId, bool? isTeachingStaff, string? status, CancellationToken ct)
    {
        var query = _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .AsNoTracking()
            .Where(e => !e.IsDeleted);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(e => e.FullName.Contains(search) || e.EmployeeCode.Contains(search) || e.Phone.Contains(search));
        }

        if (departmentId.HasValue)
        {
            query = query.Where(e => e.DepartmentId == departmentId.Value);
        }

        if (designationId.HasValue)
        {
            query = query.Where(e => e.DesignationId == designationId.Value);
        }

        if (isTeachingStaff.HasValue)
        {
            query = query.Where(e => e.IsTeachingStaff == isTeachingStaff.Value);
        }

        if (!string.IsNullOrEmpty(status))
        {
            query = query.Where(e => e.Status == status);
        }

        var totalCount = await query.CountAsync(ct);
        var items = await query.OrderBy(e => e.FullName)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Select(e => new EmployeeListItemDto
            {
                Id = e.Id,
                EmployeeCode = e.EmployeeCode,
                FullName = e.FullName,
                Designation = e.Designation != null ? e.Designation.Name : string.Empty,
                Department = e.Department != null ? e.Department.Name : string.Empty,
                Phone = e.Phone,
                Email = e.Email,
                Status = e.Status,
                IsTeachingStaff = e.IsTeachingStaff,
                JoiningDate = e.JoiningDate,
                ProfilePicturePath = e.ProfilePicturePath
            }).ToListAsync(ct);

        return (items, totalCount);
    }

    public async Task<EmployeeUpsertDto?> GetForEditAsync(int id, CancellationToken ct)
    {
        var employee = await _db.Employees
            .Include(e => e.Qualifications.Where(q => !q.IsDeleted))
            .Include(e => e.Documents.Where(d => !d.IsDeleted))
            .Include(e => e.Experiences.Where(ex => !ex.IsDeleted))
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);

        if (employee == null) return null;

        return new EmployeeUpsertDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FullName = employee.FullName,
            FatherName = employee.FatherName,
            MotherName = employee.MotherName,
            Gender = employee.Gender,
            DateOfBirth = employee.DateOfBirth,
            BloodGroup = employee.BloodGroup,
            Religion = employee.Religion,
            Nationality = employee.Nationality,
            NIDNumber = employee.NIDNumber,
            BirthCertificateNo = employee.BirthCertificateNo,
            Phone = employee.Phone,
            Email = employee.Email,
            PresentAddress = employee.PresentAddress,
            PermanentAddress = employee.PermanentAddress,
            JoiningDate = employee.JoiningDate,
            DepartmentId = employee.DepartmentId,
            DesignationId = employee.DesignationId,
            EmployeeType = employee.EmployeeType,
            IsTeachingStaff = employee.IsTeachingStaff,
            Status = employee.Status,
            ProfilePicturePath = employee.ProfilePicturePath,
            SignaturePath = employee.SignaturePath,
            EmergencyContactName = employee.EmergencyContactName,
            EmergencyContactPhone = employee.EmergencyContactPhone,
            Remarks = employee.Remarks,
            Qualifications = employee.Qualifications.Select(q => new EmployeeQualificationDto
            {
                Id = q.Id,
                EmployeeId = q.EmployeeId,
                ExamName = q.ExamName,
                BoardOrUniversity = q.BoardOrUniversity,
                InstituteName = q.InstituteName,
                GroupOrSubject = q.GroupOrSubject,
                PassingYear = q.PassingYear,
                Result = q.Result,
                CGPAOrDivision = q.CGPAOrDivision,
                CertificateFilePath = q.CertificateFilePath
            }).ToList(),
            Documents = employee.Documents.Select(d => new EmployeeDocumentDto
            {
                Id = d.Id,
                EmployeeId = d.EmployeeId,
                DocumentType = d.DocumentType,
                DocumentName = d.DocumentName,
                FilePath = d.FilePath,
                ExpiryDate = d.ExpiryDate,
                Remarks = d.Remarks
            }).ToList(),
            Experiences = employee.Experiences.Select(ex => new EmployeeExperienceDto
            {
                Id = ex.Id,
                EmployeeId = ex.EmployeeId,
                OrganizationName = ex.OrganizationName,
                Designation = ex.Designation,
                StartDate = ex.StartDate,
                EndDate = ex.EndDate,
                Remarks = ex.Remarks
            }).ToList()
        };
    }

    public async Task<EmployeeDetailsDto?> GetDetailsAsync(int id, CancellationToken ct)
    {
        var employee = await _db.Employees
            .Include(e => e.Department)
            .Include(e => e.Designation)
            .Include(e => e.User)
            .Include(e => e.Qualifications.Where(q => !q.IsDeleted))
            .Include(e => e.Documents.Where(d => !d.IsDeleted))
            .Include(e => e.Experiences.Where(ex => !ex.IsDeleted))
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted, ct);

        if (employee == null) return null;

        return new EmployeeDetailsDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FullName = employee.FullName,
            FatherName = employee.FatherName,
            MotherName = employee.MotherName,
            Gender = employee.Gender,
            DateOfBirth = employee.DateOfBirth,
            BloodGroup = employee.BloodGroup,
            Religion = employee.Religion,
            Nationality = employee.Nationality,
            NIDNumber = employee.NIDNumber,
            BirthCertificateNo = employee.BirthCertificateNo,
            Phone = employee.Phone,
            Email = employee.Email,
            PresentAddress = employee.PresentAddress,
            PermanentAddress = employee.PermanentAddress,
            JoiningDate = employee.JoiningDate,
            Department = employee.Department != null ? employee.Department.Name : string.Empty,
            Designation = employee.Designation != null ? employee.Designation.Name : string.Empty,
            EmployeeType = employee.EmployeeType,
            IsTeachingStaff = employee.IsTeachingStaff,
            Status = employee.Status,
            ProfilePicturePath = employee.ProfilePicturePath,
            SignaturePath = employee.SignaturePath,
            EmergencyContactName = employee.EmergencyContactName,
            EmergencyContactPhone = employee.EmergencyContactPhone,
            Remarks = employee.Remarks,
            Username = employee.User != null ? employee.User.UserName : null,
            Qualifications = employee.Qualifications.Select(q => new EmployeeQualificationDto
            {
                Id = q.Id,
                EmployeeId = q.EmployeeId,
                ExamName = q.ExamName,
                BoardOrUniversity = q.BoardOrUniversity,
                InstituteName = q.InstituteName,
                GroupOrSubject = q.GroupOrSubject,
                PassingYear = q.PassingYear,
                Result = q.Result,
                CGPAOrDivision = q.CGPAOrDivision,
                CertificateFilePath = q.CertificateFilePath
            }).ToList(),
            Documents = employee.Documents.Select(d => new EmployeeDocumentDto
            {
                Id = d.Id,
                EmployeeId = d.EmployeeId,
                DocumentType = d.DocumentType,
                DocumentName = d.DocumentName,
                FilePath = d.FilePath,
                ExpiryDate = d.ExpiryDate,
                Remarks = d.Remarks
            }).ToList(),
            Experiences = employee.Experiences.Select(ex => new EmployeeExperienceDto
            {
                Id = ex.Id,
                EmployeeId = ex.EmployeeId,
                OrganizationName = ex.OrganizationName,
                Designation = ex.Designation,
                StartDate = ex.StartDate,
                EndDate = ex.EndDate,
                Remarks = ex.Remarks
            }).ToList()
        };
    }

    public async Task<EmployeeUpsertDto?> GetByUserIdAsync(int userId, CancellationToken ct)
    {
        var employee = await _db.Employees
            .Include(e => e.Qualifications.Where(q => !q.IsDeleted))
            .Include(e => e.Documents.Where(d => !d.IsDeleted))
            .Include(e => e.Experiences.Where(ex => !ex.IsDeleted))
            .AsNoTracking()
            .FirstOrDefaultAsync(e => e.UserId == userId && !e.IsDeleted, ct);

        if (employee == null) return null;

        return new EmployeeUpsertDto
        {
            Id = employee.Id,
            EmployeeCode = employee.EmployeeCode,
            FullName = employee.FullName,
            FatherName = employee.FatherName,
            MotherName = employee.MotherName,
            Gender = employee.Gender,
            DateOfBirth = employee.DateOfBirth,
            BloodGroup = employee.BloodGroup,
            Religion = employee.Religion,
            Nationality = employee.Nationality,
            NIDNumber = employee.NIDNumber,
            BirthCertificateNo = employee.BirthCertificateNo,
            Phone = employee.Phone,
            Email = employee.Email,
            PresentAddress = employee.PresentAddress,
            PermanentAddress = employee.PermanentAddress,
            JoiningDate = employee.JoiningDate,
            DepartmentId = employee.DepartmentId,
            DesignationId = employee.DesignationId,
            EmployeeType = employee.EmployeeType,
            IsTeachingStaff = employee.IsTeachingStaff,
            Status = employee.Status,
            ProfilePicturePath = employee.ProfilePicturePath,
            SignaturePath = employee.SignaturePath,
            EmergencyContactName = employee.EmergencyContactName,
            EmergencyContactPhone = employee.EmergencyContactPhone,
            Remarks = employee.Remarks,
            Qualifications = employee.Qualifications.Select(q => new EmployeeQualificationDto
            {
                Id = q.Id,
                EmployeeId = q.EmployeeId,
                ExamName = q.ExamName,
                BoardOrUniversity = q.BoardOrUniversity,
                InstituteName = q.InstituteName,
                GroupOrSubject = q.GroupOrSubject,
                PassingYear = q.PassingYear,
                Result = q.Result,
                CGPAOrDivision = q.CGPAOrDivision,
                CertificateFilePath = q.CertificateFilePath
            }).ToList(),
            Documents = employee.Documents.Select(d => new EmployeeDocumentDto
            {
                Id = d.Id,
                EmployeeId = d.EmployeeId,
                DocumentType = d.DocumentType,
                DocumentName = d.DocumentName,
                FilePath = d.FilePath,
                ExpiryDate = d.ExpiryDate,
                Remarks = d.Remarks
            }).ToList(),
            Experiences = employee.Experiences.Select(ex => new EmployeeExperienceDto
            {
                Id = ex.Id,
                EmployeeId = ex.EmployeeId,
                OrganizationName = ex.OrganizationName,
                Designation = ex.Designation,
                StartDate = ex.StartDate,
                EndDate = ex.EndDate,
                Remarks = ex.Remarks
            }).ToList()
        };
    }

    // ─── Stored Procedure Methods ─────────────────────────────────

    public async Task<(List<EmployeeListItemDto> items, int totalRecords)> GetPagedBySpAsync(
        int page, int pageSize, string? search, int? departmentId, int? designationId, bool? isTeachingStaff, string? status, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetEmployeesPaged";
        command.CommandType = CommandType.StoredProcedure;

        command.Parameters.Add(new SqlParameter("@PageNumber", page));
        command.Parameters.Add(new SqlParameter("@PageSize", pageSize));
        command.Parameters.Add(new SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@DepartmentId", departmentId ?? 0));
        command.Parameters.Add(new SqlParameter("@DesignationId", designationId ?? 0));
        command.Parameters.Add(new SqlParameter("@IsTeachingStaff", (object?)isTeachingStaff ?? DBNull.Value));
        command.Parameters.Add(new SqlParameter("@Status", (object?)status ?? DBNull.Value));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            var items = new List<EmployeeListItemDto>();
            int totalRecords = 0;

            while (await reader.ReadAsync(ct))
            {
                var item = new EmployeeListItemDto
                {
                    Id = reader.GetInt32(reader.GetOrdinal("Id")),
                    EmployeeCode = reader.GetString(reader.GetOrdinal("EmployeeCode")),
                    FullName = reader.GetString(reader.GetOrdinal("FullName")),
                    Department = GetString(reader, "DepartmentName"),
                    Designation = GetString(reader, "DesignationName"),
                    Phone = GetString(reader, "Phone"),
                    Email = reader.IsDBNull(reader.GetOrdinal("Email")) ? null : reader.GetString(reader.GetOrdinal("Email")),
                    Status = reader.GetString(reader.GetOrdinal("Status")),
                    IsTeachingStaff = reader.GetBoolean(reader.GetOrdinal("IsTeachingStaff")),
                    JoiningDate = reader.GetDateTime(reader.GetOrdinal("JoiningDate")),
                    ProfilePicturePath = reader.IsDBNull(reader.GetOrdinal("ProfilePicturePath")) ? null : reader.GetString(reader.GetOrdinal("ProfilePicturePath")),
                    NIDNumber = GetString(reader, "NIDNumber"),
                    EmergencyContactName = GetString(reader, "EmergencyContactName"),
                    EmergencyContactPhone = GetString(reader, "EmergencyContactPhone"),
                    Remarks = GetString(reader, "Remarks")
                };

                if (totalRecords == 0)
                {
                    totalRecords = reader.GetInt32(reader.GetOrdinal("TotalRecords"));
                }

                items.Add(item);
            }

            return (items, totalRecords);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<EmployeeDetailsDto?> GetDetailsBySpAsync(int id, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetEmployeeDetails";
        command.CommandType = CommandType.StoredProcedure;
        command.Parameters.Add(new SqlParameter("@EmployeeId", id));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);

            // Result set 1: Main employee record
            if (!await reader.ReadAsync(ct)) return null;

            var dto = new EmployeeDetailsDto
            {
                Id = reader.GetInt32(reader.GetOrdinal("Id")),
                EmployeeCode = reader.GetString(reader.GetOrdinal("EmployeeCode")),
                FullName = reader.GetString(reader.GetOrdinal("FullName")),
                BanglaName = GetNullableString(reader, "BanglaName"),
                FatherName = GetNullableString(reader, "FatherName"),
                MotherName = GetNullableString(reader, "MotherName"),
                SpouseName = GetNullableString(reader, "SpouseName"),
                Gender = GetString(reader, "Gender"),
                MaritalStatus = GetNullableString(reader, "MaritalStatus"),
                DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                BloodGroup = GetNullableString(reader, "BloodGroup"),
                Religion = GetNullableString(reader, "Religion"),
                Nationality = GetString(reader, "Nationality"),
                NIDNumber = GetNullableString(reader, "NIDNumber"),
                BirthCertificateNo = GetNullableString(reader, "BirthCertificateNo"),
                PassportNo = GetNullableString(reader, "PassportNo"),
                TIN = GetNullableString(reader, "TIN"),
                DrivingLicenseNo = GetNullableString(reader, "DrivingLicenseNo"),
                Phone = GetString(reader, "Phone"),
                AlternateMobile = GetNullableString(reader, "AlternateMobile"),
                Email = GetNullableString(reader, "Email"),
                PresentAddress = GetNullableString(reader, "PresentAddress"),
                PermanentAddress = GetNullableString(reader, "PermanentAddress"),
                JoiningDate = reader.GetDateTime(reader.GetOrdinal("JoiningDate")),
                EmployeeType = GetString(reader, "EmployeeType"),
                IsTeachingStaff = reader.GetBoolean(reader.GetOrdinal("IsTeachingStaff")),
                Status = GetString(reader, "Status"),
                ProfilePicturePath = GetNullableString(reader, "ProfilePicturePath"),
                SignaturePath = GetNullableString(reader, "SignaturePath"),
                EmergencyContactName = GetNullableString(reader, "EmergencyContactName"),
                EmergencyContactPhone = GetNullableString(reader, "EmergencyContactPhone"),
                Remarks = GetNullableString(reader, "Remarks"),
                Department = GetString(reader, "Department"),
                Designation = GetString(reader, "Designation"),
                Username = GetString(reader, "Username"),
                EmployeeCardNumber = GetNullableString(reader, "EmployeeCardNumber"),
                CardIssueDate = GetNullableDateTime(reader, "CardIssueDate"),
                CardExpiryDate = GetNullableDateTime(reader, "CardExpiryDate"),
                CardPrintedAt = GetNullableDateTime(reader, "CardPrintedAt"),
                CardVersion = reader.IsDBNull(reader.GetOrdinal("CardVersion")) ? 0 : reader.GetInt32(reader.GetOrdinal("CardVersion")),
                QRVerificationCode = GetNullableString(reader, "QRVerificationCode")
            };

            // Result set 2: Qualifications
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dto.Qualifications.Add(new EmployeeQualificationDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        ExamName = GetString(reader, "ExamName"),
                        BoardOrUniversity = GetNullableString(reader, "BoardOrUniversity"),
                        InstituteName = GetNullableString(reader, "InstituteName"),
                        GroupOrSubject = GetNullableString(reader, "GroupOrSubject"),
                        PassingYear = GetNullableString(reader, "PassingYear"),
                        Result = GetNullableString(reader, "Result"),
                        CGPAOrDivision = GetNullableString(reader, "CGPAOrDivision"),
                        CertificateFilePath = GetNullableString(reader, "CertificateFilePath")
                    });
                }
            }

            // Result set 3: Documents
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dto.Documents.Add(new EmployeeDocumentDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        DocumentType = GetString(reader, "DocumentType"),
                        DocumentName = GetString(reader, "DocumentName"),
                        FilePath = GetNullableString(reader, "FilePath"),
                        ExpiryDate = GetNullableDateTime(reader, "ExpiryDate"),
                        Remarks = GetNullableString(reader, "Remarks")
                    });
                }
            }

            // Result set 4: Experiences
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dto.Experiences.Add(new EmployeeExperienceDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        OrganizationName = GetString(reader, "OrganizationName"),
                        Designation = GetString(reader, "Designation"),
                        StartDate = reader.GetDateTime(reader.GetOrdinal("StartDate")),
                        EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                        Remarks = GetNullableString(reader, "Remarks")
                    });
                }
            }

            // Result set 5: Bank Accounts
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dto.BankAccounts.Add(new EmployeeBankAccountDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        BankName = GetString(reader, "BankName"),
                        BranchName = GetString(reader, "BranchName"),
                        AccountNumber = GetString(reader, "AccountNumber"),
                        RoutingNumber = GetNullableString(reader, "RoutingNumber"),
                        AccountType = GetNullableString(reader, "AccountType"),
                        IsDefault = reader.GetBoolean(reader.GetOrdinal("IsDefault")),
                        IsActive = reader.GetBoolean(reader.GetOrdinal("IsActive"))
                    });
                }
            }

            // Result set 6: Promotions
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dto.Promotions.Add(new EmployeePromotionDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        PreviousDesignationId = reader.GetInt32(reader.GetOrdinal("PreviousDesignationId")),
                        NewDesignationId = reader.GetInt32(reader.GetOrdinal("NewDesignationId")),
                        Reason = GetNullableString(reader, "Reason"),
                        PromotionDate = reader.GetDateTime(reader.GetOrdinal("PromotionDate")),
                        PreviousSalary = reader.IsDBNull(reader.GetOrdinal("PreviousSalary")) ? null : reader.GetDecimal(reader.GetOrdinal("PreviousSalary")),
                        NewSalary = reader.IsDBNull(reader.GetOrdinal("NewSalary")) ? null : reader.GetDecimal(reader.GetOrdinal("NewSalary")),
                        Remarks = GetNullableString(reader, "Remarks")
                    });
                }
            }

            // Result set 7: Transfers
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dto.Transfers.Add(new EmployeeTransferDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        FromDepartmentId = reader.GetInt32(reader.GetOrdinal("FromDepartmentId")),
                        ToDepartmentId = reader.GetInt32(reader.GetOrdinal("ToDepartmentId")),
                        Reason = GetNullableString(reader, "Reason"),
                        TransferDate = reader.GetDateTime(reader.GetOrdinal("TransferDate")),
                        Remarks = GetNullableString(reader, "Remarks")
                    });
                }
            }

            // Result set 8: Training
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dto.Trainings.Add(new EmployeeTrainingDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        TrainingName = GetString(reader, "TrainingName"),
                        InstitutionName = GetNullableString(reader, "InstitutionName"),
                        Duration = GetNullableString(reader, "Duration"),
                        StartDate = reader.IsDBNull(reader.GetOrdinal("StartDate")) ? null : reader.GetDateTime(reader.GetOrdinal("StartDate")),
                        EndDate = reader.IsDBNull(reader.GetOrdinal("EndDate")) ? null : reader.GetDateTime(reader.GetOrdinal("EndDate")),
                        CertificatePath = GetNullableString(reader, "CertificatePath"),
                        Remarks = GetNullableString(reader, "Remarks")
                    });
                }
            }

            // Result set 9: Awards
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dto.Awards.Add(new EmployeeAwardDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        AwardName = GetString(reader, "AwardName"),
                        AwardedBy = GetNullableString(reader, "AwardedBy"),
                        AwardDate = reader.GetDateTime(reader.GetOrdinal("AwardDate")),
                        Description = GetNullableString(reader, "Description"),
                        CertificatePath = GetNullableString(reader, "CertificatePath")
                    });
                }
            }

            // Result set 10: Disciplinary Actions
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dto.DisciplinaryActions.Add(new EmployeeDisciplinaryActionDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        EmployeeId = reader.GetInt32(reader.GetOrdinal("EmployeeId")),
                        ActionType = GetString(reader, "ActionType"),
                        Reason = GetNullableString(reader, "Reason"),
                        ActionDate = reader.GetDateTime(reader.GetOrdinal("ActionDate")),
                        Description = GetNullableString(reader, "Description"),
                        DocumentPath = GetNullableString(reader, "DocumentPath"),
                        IsResolved = reader.GetBoolean(reader.GetOrdinal("IsResolved")),
                        ResolvedAt = GetNullableDateTime(reader, "ResolvedAt"),
                        ResolutionRemarks = GetNullableString(reader, "ResolutionRemarks")
                    });
                }
            }

            return dto;
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }

    public async Task<EmployeeDashboardDto?> GetDashboardBySpAsync(CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetEmployeeDashboard";
        command.CommandType = CommandType.StoredProcedure;

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);

            var dashboard = new EmployeeDashboardDto();

            // Result set 1: Aggregated counts
            if (await reader.ReadAsync(ct))
            {
                dashboard.TotalEmployees = reader.GetInt32(reader.GetOrdinal("TotalEmployees"));
                dashboard.TeachingStaff = reader.GetInt32(reader.GetOrdinal("TeachingStaff"));
                dashboard.ActiveEmployees = reader.GetInt32(reader.GetOrdinal("ActiveEmployees"));
                dashboard.InactiveEmployees = reader.GetInt32(reader.GetOrdinal("InactiveEmployees"));
                dashboard.OnLeaveEmployees = reader.GetInt32(reader.GetOrdinal("OnLeaveEmployees"));
                dashboard.ResignedEmployees = reader.GetInt32(reader.GetOrdinal("ResignedEmployees"));
                dashboard.RetiredEmployees = reader.GetInt32(reader.GetOrdinal("RetiredEmployees"));
                dashboard.NewHiresThisYear = reader.GetInt32(reader.GetOrdinal("NewHiresThisYear"));
                dashboard.NonTeachingStaff = dashboard.TotalEmployees - dashboard.TeachingStaff;
            }

            // Result set 2: Department distribution
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dashboard.DepartmentStats.Add(new DepartmentStat
                    {
                        DepartmentName = reader.GetString(reader.GetOrdinal("DepartmentName")),
                        Count = reader.GetInt32(reader.GetOrdinal("Count")),
                        TeachingCount = reader.GetInt32(reader.GetOrdinal("TeachingCount")),
                        NonTeachingCount = reader.GetInt32(reader.GetOrdinal("NonTeachingCount"))
                    });
                }
            }

            // Result set 3: Status distribution
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dashboard.StatusStats.Add(new StatusStat
                    {
                        Status = reader.GetString(reader.GetOrdinal("Status")),
                        Count = reader.GetInt32(reader.GetOrdinal("Count"))
                    });
                }
            }

            // Result set 4: Birthdays this month
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dashboard.UpcomingBirthdays.Add(new BirthdayDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        FullName = reader.GetString(reader.GetOrdinal("FullName")),
                        Designation = reader.GetString(reader.GetOrdinal("Designation")),
                        DateOfBirth = reader.GetDateTime(reader.GetOrdinal("DateOfBirth")),
                        ProfilePicturePath = reader.IsDBNull(reader.GetOrdinal("ProfilePicturePath")) ? null : reader.GetString(reader.GetOrdinal("ProfilePicturePath"))
                    });
                }
                dashboard.BirthdaysThisMonth = dashboard.UpcomingBirthdays.Count;
            }

            // Result set 5: Recent hires
            if (await reader.NextResultAsync(ct))
            {
                while (await reader.ReadAsync(ct))
                {
                    dashboard.RecentHires.Add(new RecentHireDto
                    {
                        Id = reader.GetInt32(reader.GetOrdinal("Id")),
                        FullName = reader.GetString(reader.GetOrdinal("FullName")),
                        Designation = reader.GetString(reader.GetOrdinal("Designation")),
                        Department = reader.GetString(reader.GetOrdinal("Department")),
                        JoiningDate = reader.GetDateTime(reader.GetOrdinal("JoiningDate")),
                        ProfilePicturePath = reader.IsDBNull(reader.GetOrdinal("ProfilePicturePath")) ? null : reader.GetString(reader.GetOrdinal("ProfilePicturePath"))
                    });
                }
            }

            return dashboard;
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }
}

public class DepartmentRepository : BaseRepository<Department>, IDepartmentRepository
{
    public DepartmentRepository(SchoolDbContext db) : base(db) { }
}

public class DesignationRepository : BaseRepository<Designation>, IDesignationRepository
{
    public DesignationRepository(SchoolDbContext db) : base(db) { }
}

public class EmployeeQualificationRepository : BaseRepository<EmployeeQualification>, IEmployeeQualificationRepository
{
    public EmployeeQualificationRepository(SchoolDbContext db) : base(db) { }
}

public class EmployeeDocumentRepository : BaseRepository<EmployeeDocument>, IEmployeeDocumentRepository
{
    public EmployeeDocumentRepository(SchoolDbContext db) : base(db) { }
}

public class EmployeeExperienceRepository : BaseRepository<EmployeeExperience>, IEmployeeExperienceRepository
{
    public EmployeeExperienceRepository(SchoolDbContext db) : base(db) { }
}

public class EmployeeInvitationRepository : BaseRepository<EmployeeInvitation>, IEmployeeInvitationRepository
{
    public EmployeeInvitationRepository(SchoolDbContext db) : base(db) { }

    public async Task<(List<EmployeeInvitationDto> items, int totalRecords)> GetPagedBySpAsync(
        int page, int pageSize, string? search, CancellationToken ct)
    {
        using var command = _db.Database.GetDbConnection().CreateCommand();
        command.CommandText = "sp_GetEmployeeInvitationList";
        command.CommandType = System.Data.CommandType.StoredProcedure;

        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageNumber", page));
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@PageSize", pageSize));
        command.Parameters.Add(new Microsoft.Data.SqlClient.SqlParameter("@SearchTerm", (object?)search ?? DBNull.Value));

        await _db.Database.OpenConnectionAsync(ct);
        try
        {
            using var reader = await command.ExecuteReaderAsync(ct);
            var items = new List<EmployeeInvitationDto>();
            int totalRecords = 0;

            var ordId = reader.GetOrdinal("Id");
            var ordInvitationCode = reader.GetOrdinal("InvitationCode");
            var ordFullName = reader.GetOrdinal("FullName");
            var ordEmail = reader.GetOrdinal("Email");
            var ordMobile = reader.GetOrdinal("Mobile");
            var ordToken = reader.GetOrdinal("InvitationToken");
            var ordDeptId = reader.GetOrdinal("DepartmentId");
            var ordDeptName = reader.GetOrdinal("DepartmentName");
            var ordDesigId = reader.GetOrdinal("DesignationId");
            var ordDesigName = reader.GetOrdinal("DesignationName");
            var ordJoiningDate = reader.GetOrdinal("JoiningDate");
            var ordEmpType = reader.GetOrdinal("EmploymentType");
            var ordStatus = reader.GetOrdinal("Status");
            var ordIsTeaching = reader.GetOrdinal("IsTeachingStaff");
            var ordRemarks = reader.GetOrdinal("Remarks");
            var ordExpiresAt = reader.GetOrdinal("ExpiresAt");
            var ordSentAt = reader.GetOrdinal("SentAt");
            var ordCompletedAt = reader.GetOrdinal("CompletedAt");
            var ordIsUsed = reader.GetOrdinal("IsUsed");
            var ordIsApproved = reader.GetOrdinal("IsApproved");
            var ordInviteStatus = reader.GetOrdinal("InvitationStatus");
            var ordCreatedAt = reader.GetOrdinal("CreatedAt");
            var ordTotal = reader.GetOrdinal("TotalRecords");

            while (await reader.ReadAsync(ct))
            {
                if (totalRecords == 0) totalRecords = reader.GetInt32(ordTotal);

                items.Add(new EmployeeInvitationDto
                {
                    Id = reader.GetInt32(ordId),
                    InvitationCode = reader.GetString(ordInvitationCode),
                    FullName = reader.GetString(ordFullName),
                    Email = reader.GetString(ordEmail),
                    Mobile = reader.GetString(ordMobile),
                    InvitationToken = reader.GetString(ordToken),
                    DepartmentId = reader.GetInt32(ordDeptId),
                    DepartmentName = reader.IsDBNull(ordDeptName) ? null : reader.GetString(ordDeptName),
                    DesignationId = reader.GetInt32(ordDesigId),
                    DesignationName = reader.IsDBNull(ordDesigName) ? null : reader.GetString(ordDesigName),
                    JoiningDate = reader.GetDateTime(ordJoiningDate),
                    EmploymentType = reader.GetString(ordEmpType),
                    Status = reader.GetString(ordStatus),
                    IsTeachingStaff = reader.GetBoolean(ordIsTeaching),
                    Remarks = reader.IsDBNull(ordRemarks) ? null : reader.GetString(ordRemarks),
                    ExpiresAt = reader.GetDateTime(ordExpiresAt),
                    SentAt = reader.IsDBNull(ordSentAt) ? null : reader.GetDateTime(ordSentAt),
                    CompletedAt = reader.IsDBNull(ordCompletedAt) ? null : reader.GetDateTime(ordCompletedAt),
                    IsUsed = reader.GetBoolean(ordIsUsed),
                    IsApproved = reader.GetBoolean(ordIsApproved),
                    InvitationStatus = reader.GetString(ordInviteStatus),
                    CreatedAt = reader.GetDateTime(ordCreatedAt)
                });
            }

            return (items, totalRecords);
        }
        finally
        {
            await _db.Database.CloseConnectionAsync();
        }
    }
}

public class EmployeeBankAccountRepository : BaseRepository<EmployeeBankAccount>, IEmployeeBankAccountRepository
{
    public EmployeeBankAccountRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<EmployeeBankAccount>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct)
    {
        return await _db.Set<EmployeeBankAccount>()
            .Where(b => b.EmployeeId == employeeId && !b.IsDeleted)
            .AsNoTracking()
            .ToListAsync(ct);
    }
}

public class EmployeePromotionRepository : BaseRepository<EmployeePromotion>, IEmployeePromotionRepository
{
    public EmployeePromotionRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<EmployeePromotionDto>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct)
    {
        return await _db.Set<EmployeePromotion>()
            .Include(p => p.Employee)
            .Where(p => p.EmployeeId == employeeId && !p.IsDeleted)
            .OrderByDescending(p => p.PromotionDate)
            .Select(p => new EmployeePromotionDto
            {
                Id = p.Id,
                EmployeeId = p.EmployeeId,
                EmployeeName = p.Employee != null ? p.Employee.FullName : null,
                PreviousDesignationId = p.PreviousDesignationId,
                NewDesignationId = p.NewDesignationId,
                Reason = p.Reason,
                PromotionDate = p.PromotionDate,
                PreviousSalary = p.PreviousSalary,
                NewSalary = p.NewSalary,
                Remarks = p.Remarks
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }
}

public class EmployeeTransferRepository : BaseRepository<EmployeeTransfer>, IEmployeeTransferRepository
{
    public EmployeeTransferRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<EmployeeTransferDto>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct)
    {
        return await _db.Set<EmployeeTransfer>()
            .Include(t => t.Employee)
            .Where(t => t.EmployeeId == employeeId && !t.IsDeleted)
            .OrderByDescending(t => t.TransferDate)
            .Select(t => new EmployeeTransferDto
            {
                Id = t.Id,
                EmployeeId = t.EmployeeId,
                EmployeeName = t.Employee != null ? t.Employee.FullName : null,
                FromDepartmentId = t.FromDepartmentId,
                ToDepartmentId = t.ToDepartmentId,
                Reason = t.Reason,
                TransferDate = t.TransferDate,
                Remarks = t.Remarks
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }
}

public class EmployeeTrainingRepository : BaseRepository<EmployeeTraining>, IEmployeeTrainingRepository
{
    public EmployeeTrainingRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<EmployeeTrainingDto>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct)
    {
        return await _db.Set<EmployeeTraining>()
            .Include(t => t.Employee)
            .Where(t => t.EmployeeId == employeeId && !t.IsDeleted)
            .OrderByDescending(t => t.StartDate)
            .Select(t => new EmployeeTrainingDto
            {
                Id = t.Id,
                EmployeeId = t.EmployeeId,
                EmployeeName = t.Employee != null ? t.Employee.FullName : null,
                TrainingName = t.TrainingName,
                InstitutionName = t.InstitutionName,
                Duration = t.Duration,
                StartDate = t.StartDate,
                EndDate = t.EndDate,
                CertificatePath = t.CertificatePath,
                Remarks = t.Remarks
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }
}

public class EmployeeAwardRepository : BaseRepository<EmployeeAward>, IEmployeeAwardRepository
{
    public EmployeeAwardRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<EmployeeAwardDto>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct)
    {
        return await _db.Set<EmployeeAward>()
            .Include(a => a.Employee)
            .Where(a => a.EmployeeId == employeeId && !a.IsDeleted)
            .OrderByDescending(a => a.AwardDate)
            .Select(a => new EmployeeAwardDto
            {
                Id = a.Id,
                EmployeeId = a.EmployeeId,
                EmployeeName = a.Employee != null ? a.Employee.FullName : null,
                AwardName = a.AwardName,
                AwardedBy = a.AwardedBy,
                AwardDate = a.AwardDate,
                Description = a.Description,
                CertificatePath = a.CertificatePath
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }
}

public class EmployeeDisciplinaryActionRepository : BaseRepository<EmployeeDisciplinaryAction>, IEmployeeDisciplinaryActionRepository
{
    public EmployeeDisciplinaryActionRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<EmployeeDisciplinaryActionDto>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct)
    {
        return await _db.Set<EmployeeDisciplinaryAction>()
            .Include(d => d.Employee)
            .Where(d => d.EmployeeId == employeeId && !d.IsDeleted)
            .OrderByDescending(d => d.ActionDate)
            .Select(d => new EmployeeDisciplinaryActionDto
            {
                Id = d.Id,
                EmployeeId = d.EmployeeId,
                EmployeeName = d.Employee != null ? d.Employee.FullName : null,
                ActionType = d.ActionType,
                Reason = d.Reason,
                ActionDate = d.ActionDate,
                Description = d.Description,
                DocumentPath = d.DocumentPath,
                IsResolved = d.IsResolved,
                ResolvedAt = d.ResolvedAt,
                ResolutionRemarks = d.ResolutionRemarks
            })
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
