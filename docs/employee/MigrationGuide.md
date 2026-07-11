# Employee Module — Migration Guide for Developers

## Architecture Rules

Before modifying the Employee module, review these non-negotiable architecture rules:

1. **Controllers must NOT access DbContext or SQL directly.**
   - OK: Inject `IEmployeeService`, `IEmployeeHrService`, `IEmployeePayrollService`, `IEmployeeInvitationService`
   - NEVER: `_db.Employees.Where(...)`, `ExecuteSqlRaw`, `FromSqlRaw`
   - The existing `EmployeeController.Details` has some direct `_uow.Repository<>()` calls — these are pre-existing technical debt. New code should go through services.

2. **Services must NOT return entities to controllers.**
   - Always map Entity → DTO before returning.
   - The extension methods `EmployeeExtensions` in `EmployeeService.cs` handle Entity → DTO mapping at the query level using `IQueryable.Select()`.

3. **Business logic belongs in Services, not Controllers.**
   - Controllers handle: authorization, validation, calling services, returning View/Json.
   - Services handle: uniqueness checks, file processing, user provisioning, teacher sync, audit logging.

4. **Soft delete is required.** Never hard-delete Employee records. Use `IsDeleted = true`.

5. **All CREATE/UPDATE/DELETE operations must log to AuditLog.**

## Adding a New Employee Sub-Module

To add a new HR sub-module (e.g., "EmployeeCertification"), follow this pattern:

### Step 1: Entity

File: `Models/Entities/Employee/EmployeeEntities.cs`

```csharp
public class EmployeeCertification : BaseEntity
{
    public int EmployeeId { get; set; }
    public Employee? Employee { get; set; }

    [Required, MaxLength(200)]
    public string CertificationName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? IssuedBy { get; set; }

    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    [MaxLength(260)]
    public string? CertificateFilePath { get; set; }
}
```

Add navigation property to `Employee`:
```csharp
public ICollection<EmployeeCertification> Certifications { get; set; } = new List<EmployeeCertification>();
```

### Step 2: DbSet + Entity Configuration

File: `Data/SchoolDbContext.cs`

```csharp
public DbSet<EmployeeCertification> EmployeeCertifications => Set<EmployeeCertification>();
```

No additional configuration needed (all decimal precision and delete behavior are applied globally).

### Step 3: DTO

File: `Models/DTOs/Employee/EmployeeDtos.cs`

```csharp
public class EmployeeCertificationDto
{
    public int Id { get; set; }
    public int EmployeeId { get; set; }

    [Required, MaxLength(200)]
    public string CertificationName { get; set; } = string.Empty;

    [MaxLength(200)]
    public string? IssuedBy { get; set; }

    public DateTime IssueDate { get; set; }
    public DateTime? ExpiryDate { get; set; }

    public string? CertificateFilePath { get; set; }
    public IFormFile? CertificateFile { get; set; }
}
```

Add collections to `EmployeeListItemDto`, `EmployeeUpsertDto`, and `EmployeeDetailsDto` as needed.

### Step 4: Repository

File: `Repositories/Implementations/Employee/EmployeeRepositories.cs`

```csharp
public class EmployeeCertificationRepository : BaseRepository<EmployeeCertification>, IEmployeeCertificationRepository
{
    public EmployeeCertificationRepository(SchoolDbContext db) : base(db) { }

    public async Task<List<EmployeeCertificationDto>> GetByEmployeeIdAsync(int employeeId, CancellationToken ct)
    {
        return await _db.Set<EmployeeCertification>()
            .Where(c => c.EmployeeId == employeeId && !c.IsDeleted)
            .OrderByDescending(c => c.IssueDate)
            .Select(c => new EmployeeCertificationDto { ... })
            .AsNoTracking()
            .ToListAsync(ct);
    }
}
```

### Step 5: Interface

File: `Services/Interfaces/Employee/IEmployeeHrService.cs` (add methods):
```csharp
Task<List<EmployeeCertificationDto>> GetCertificationsAsync(int employeeId, CancellationToken ct);
Task SaveCertificationAsync(EmployeeCertificationDto dto, CancellationToken ct);
Task DeleteCertificationAsync(int id, CancellationToken ct);
```

### Step 6: Service Implementation

File: `Services/Implementations/Employee/EmployeeHrService.cs`

Add a new region following the existing patterns (Bank Accounts, Promotions, etc.):

```csharp
// ── Certifications ──
public async Task<List<EmployeeCertificationDto>> GetCertificationsAsync(int employeeId, CancellationToken ct)
{
    // ... query, map, return
}

public async Task SaveCertificationAsync(EmployeeCertificationDto dto, CancellationToken ct)
{
    // ... if dto.Id > 0: update; else: add
    // ... handle file upload
    // ... audit log
}

public async Task DeleteCertificationAsync(int id, CancellationToken ct)
{
    // ... soft delete
    // ... audit log
}
```

### Step 7: Controller Actions

File: `Controllers/Employee/EmployeeHRController.cs`

```csharp
private const string CertificationViewPermission = "Employee.Certification.View";
private const string CertificationEditPermission = "Employee.Certification.Edit";
private const string CertificationDeletePermission = "Employee.Certification.Delete";

[HttpGet("Certifications/{employeeId}")]
[RequirePermission(CertificationViewPermission)]
public async Task<IActionResult> Certifications(int employeeId, CancellationToken ct)
{
    if (employeeId <= 0) return RedirectToAction("Index", "Employee");
    ViewBag.EmployeeName = await GetEmployeeNameAsync(employeeId, ct) ?? "Unknown";
    ViewBag.EmployeeId = employeeId;
    var items = await _hrService.GetCertificationsAsync(employeeId, ct);
    return View(items);
}

[HttpPost("Certification/Save")]
[ValidateAntiForgeryToken]
[RequirePermission(CertificationEditPermission)]
public async Task<IActionResult> SaveCertification(EmployeeCertificationDto dto, CancellationToken ct)
{
    await _hrService.SaveCertificationAsync(dto, ct);
    TempData["SuccessMessage"] = "Certification saved.";
    return RedirectToAction("Certifications", new { employeeId = dto.EmployeeId });
}

[HttpPost("Certification/Delete")]
[ValidateAntiForgeryToken]
[RequirePermission(CertificationDeletePermission)]
public async Task<IActionResult> DeleteCertification(int id, int employeeId, CancellationToken ct)
{
    await _hrService.DeleteCertificationAsync(id, ct);
    TempData["SuccessMessage"] = "Certification deleted.";
    return RedirectToAction("Certifications", new { employeeId });
}
```

### Step 8: DI Registration

File: `Extensions/ServiceRegistration.cs`

```csharp
services.AddScoped<IEmployeeCertificationRepository, EmployeeCertificationRepository>();
```

(The service `EmployeeHrService` is already registered — no change needed if adding methods to existing interface.)

### Step 9: Stored Procedure (Optional)

If this is an enterprise reporting query, create a stored procedure.

File: `Data/StoredProcedures/Employee/sp_GetCertificationSummary.sql`

### Step 10: Migration

```bash
dotnet ef migrations add AddEmployeeCertifications
dotnet ef database update
```

## Adding a New Employee Field

1. Add property to `Employee` entity
2. Add to `EmployeeUpsertDto` and `EmployeeDetailsDto`
3. Update mapping in `EmployeeExtensions.GetForEditAsync()` and `GetDetailsAsync()`
4. Update `EmployeeService.SaveAsync()` — both Insert and Edit branches
5. Add uniqueness check/index in `SchoolDbContext.OnModelCreating()` if needed
6. Create migration

## Common Pitfalls

### 1. User Provisioning Failure

The auto-provisioning in `SaveAsync()` creates a user and assigns roles. If this fails, the employee is still saved (with a note in Remarks). The failure is logged but not thrown. This is by design — the employee record is always created successfully.

If you need to retry provisioning, call `IUserProvisionService.ProvisionUserForEmployeeAsync()` manually (consult AGENTS.md for guidance).

### 2. Teacher Sync Silent Failure

`ITeacherSynchronizationService.SyncEmployeeToTeacherAsync()` is called on a best-effort basis. Failures are logged but never propagated. The employee save always succeeds.

If teacher profiles are out of sync, verify:
- `Employee.IsTeachingStaff == true`
- `TeacherSynchronizationService` is registered in DI
- The `Teacher` entity exists in `SchoolDbContext`

### 3. File Upload Security

All file uploads go through these checks:
- Max file size: 5 MB (controlled by `MaxFileSize` constant)
- Allowed extensions: `.jpg`, `.jpeg`, `.png`, `.pdf`, `.doc`, `.docx`
- Allowed MIME types: `image/jpeg`, `image/png`, `application/pdf`, `application/msword`, `application/vnd.openxmlformats-officedocument.wordprocessingml.document`
- Files are saved with GUID-based names to prevent path traversal
- Files are stored under `wwwroot/uploads/`

If adding a new file upload field, add the extension/MIME to the existing `HashSet` collections in `EmployeeService.cs`.

### 4. Soft Delete Cascading

When an Employee is soft-deleted:
- `Employee.IsDeleted = true`, `Employee.Status = "Deleted"`
- Linked `ApplicationUser.IsDeleted = true`, `ApplicationUser.Status = AccountStatus.Inactive`
- Linked `Teacher` record is NOT automatically soft-deleted (only synced)

If you add a new child entity, ensure it uses the same `IsDeleted` pattern and is filtered in all queries with `!.IsDeleted`.

### 5. Employee Code Uniqueness

Employee codes are generated as `EMP-{YYYY}-{NNNN}` (e.g., `EMP-2026-0001`). The counter resets each year. If you need a different format:
- Modify `GenerateEmployeeCodeAsync()` in both `EmployeeService.cs` and `EmployeeInvitationService.cs`
- Ensure the prefix matches your new format
- Run a data migration to update existing codes (if needed)

### 6. Invitation Token Security

Invitation tokens are `Guid.NewGuid().ToString("N") + Guid.NewGuid().ToString("N")` (128 hex characters). This is intentionally long and unpredictable. Do not shorten the token.

Invitations expire after 72 hours (`ExpiresAt = DateTime.UtcNow.AddHours(72)`). The expiry is checked on `Welcome`, `Start`, and `Submit`.

## Testing the Module

### Unit Test Coverage

Key areas to test:
- `EmployeeService.SaveAsync()` — Insert vs Edit, uniqueness validation, file handling
- `EmployeeInvitationService.CompleteOnboardingAsync()` — full onboarding flow
- `EmployeeService.GetDashboardAsync()` — aggregation logic
- `EmployeeExtensions.GetPagedAsync()` — filtering logic

### Integration Test Checklist

- [ ] Create employee with all child collections (qualifications, documents, experiences)
- [ ] Verify user account was created with correct roles
- [ ] Verify teacher profile was created for teaching staff
- [ ] Verify email was sent (check logs if email service is mocked)
- [ ] Edit employee — verify updates cascade to user account
- [ ] Change status — verify user account status synced
- [ ] Delete employee — verify user account deactivated
- [ ] Create invitation → complete onboarding — verify full flow
- [ ] Upload files — verify file saved and path stored
- [ ] Verify uniqueness validation rejects duplicate phone/email/NID

## Breaking Changes to Avoid

| Change | Impact | Mitigation |
|---|---|---|
| Rename `EmployeeCode` | Breaks code generation, References in other modules | Add new field, deprecate old |
| Remove `UserId` from Employee | Breaks user provisioning integration | Keep field; set to null as needed |
| Change `TotalSalary` computation | Breaks payroll reports | Keep formula; add new computed field |
| Remove `IsTeachingStaff` | Breaks teacher sync, role assignment | Keep field; use `Designation.IsTeachingRole` |
| Change `BaseEntity` | Affects all 14 Employee tables | Extend `BaseEntity` instead |
| Remove soft-delete pattern | Breaks all existing queries | Keep `IsDeleted` |
