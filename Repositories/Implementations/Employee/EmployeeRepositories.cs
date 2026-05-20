using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Data;
using SchoolManagementSystem.Models.Entities.Employee;
using SchoolManagementSystem.Models.DTOs.Employee;
using SchoolManagementSystem.Repositories.Interfaces.Employee;

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
