# Employee Module — Controller & Service API

## Controllers

### EmployeeController

**Route:** `/{area?}/Employee` (default area)
**Authorization:** `[Authorize]` (all actions require authentication)

#### Dashboard
```
GET /Employee/Dashboard
Permission: Employees.View
Returns: ViewResult (EmployeeDashboardDto)
```

Returns the HR dashboard with aggregate statistics: total employees, teaching/non-teaching breakdown, status distribution, department stats, recent hires, upcoming birthdays (this month).

#### Index (Employee List)
```
GET /Employee/Index?page=1&size=10&search=&departmentId=&designationId=&isTeachingStaff=&status=
Permission: Employees.View
Returns: ViewResult or JsonResult (Tabulator grid)
```

If `X-Requested-With: XMLHttpRequest` header is present, returns JSON:
```json
{
  "data": [
    {
      "id": 1,
      "employeeCode": "EMP-2026-0001",
      "fullName": "...",
      "designation": "Senior Teacher",
      "department": "Science",
      "phone": "017...",
      "email": "...",
      "status": "Active",
      "isTeachingStaff": true,
      "joiningDate": "2026-01-01T00:00:00",
      "profilePicturePath": "/uploads/employees/photos/..."
    }
  ],
  "last_page": 5,
  "total_records": 42
}
```

Parameters:
| Param | Type | Default | Description |
|---|---|---|---|
| page | int | 1 | Page number |
| size | int | 10 | Page size |
| search | string | null | Search in FullName, EmployeeCode, Phone |
| departmentId | int? | null | Filter by department |
| designationId | int? | null | Filter by designation |
| isTeachingStaff | bool? | null | Filter by teaching status |
| status | string | null | Filter by employment status |

#### Create (GET)
```
GET /Employee/Create
Permission: Employees.Create
Returns: ViewResult (CreateEdit.cshtml with EmployeeUpsertDto)
```

Pre-populates: JoiningDate = today, DateOfBirth = 25 years ago, Status = "Active", IsTeachingStaff = false.

#### Edit (GET)
```
GET /Employee/Edit/{id}
Permission: Employees.Edit
Returns: ViewResult (CreateEdit.cshtml with EmployeeUpsertDto) or NotFound
```

#### Save (POST)
```
POST /Employee/Save
Permission: Employees.Create (Id=0) or Employees.Edit (Id>0) — runtime check
Anti-forgery: Required
Body: EmployeeUpsertDto
Returns: Redirect to Details or View with errors
```

The runtime permission check evaluates the `Id` field: 0 → Create, non-zero → Edit.

#### Details
```
GET /Employee/Details/{id?}
Route: /Employee/Details/{id?}
Permission: Employees.View (bypassed for own profile)
Returns: ViewResult (Details.cshtml with EmployeeDetailsViewModel) or NotFound
```

Security: If `id` is null/0, resolves to current user's employee. Own profile bypasses permission check.

On first view, auto-generates ID card number, QR code, and card dates.

Loads additional ViewBag data:
- `ViewBag.SchoolSetting` — school info for display
- `ViewBag.TotalPresent/Absent/Leave/Late` — attendance stats for current year
- `ViewBag.TotalLeaveDays` — total leave days this year
- `ViewBag.CurrentSalary` — latest salary total
- `ViewBag.TeacherProfile` — linked Teacher entity (if teaching staff)
- `ViewBag.QRCodeBase64` — QR code image (if card exists)
- `ViewBag.AuditLogs` — last 200 audit log entries for linked user

#### Delete (POST)
```
POST /Employee/Delete/{id}
Permission: Employees.Delete
Anti-forgery: Required
Returns: Redirect to Index
```

Soft-deletes employee and deactivates linked ApplicationUser.

#### UpdateStatus (POST)
```
POST /Employee/UpdateStatus?id={id}&status={status}
Permission: Employees.Edit
Anti-forgery: Required
Returns: Redirect to Details
```

Valid status values: Active, Inactive, On Leave, Resigned, Retired.

#### DownloadServiceBookPdf
```
GET /Employee/DownloadServiceBookPdf/{id}
Permission: Employees.View
Returns: FileResult (application/pdf)
```

Generates a PDF service book using `IViewRendererService` and `IPdfGenerator`.

#### VerifyCode / VerifyEmail / VerifyPhone (GET)
```
GET /Employee/VerifyCode?code={code}&id={id?}
GET /Employee/VerifyEmail?email={email}&id={id?}
GET /Employee/VerifyPhone?phone={phone}&id={id?}
Permission: Employees.View
Returns: JsonResult (true = available, false = exists)
```

Used for client-side uniqueness validation.

#### Verify (Public ID Card Verification)
```
GET /Employee/Verify/{id}
Route: /Employee/Verify/{id}
Permission: Employees.View
Returns: ViewResult (Verify.cshtml)
```

Loads employee details, validates card status. If teaching staff, additionally loads teacher specialization.

---

### EmployeeHRController

**Route:** `/EmployeeHR`
**Authorization:** `[Authorize]`

All HR sub-modules follow a consistent RESTful pattern:

| Action | Method | Route | Permission | Description |
|---|---|---|---|---|
| BankAccounts | GET | `/EmployeeHR/BankAccounts/{employeeId}` | Employee.BankAccount.View | List bank accounts |
| SaveBankAccount | POST | `/EmployeeHR/BankAccount/Save` | Employee.BankAccount.Edit | Create/update bank account |
| DeleteBankAccount | POST | `/EmployeeHR/BankAccount/Delete?id={id}&employeeId={eid}` | Employee.BankAccount.Delete | Delete bank account |
| Promotions | GET | `/EmployeeHR/Promotions/{employeeId}` | Employee.Promotion.View | List promotions |
| SavePromotion | POST | `/EmployeeHR/Promotion/Save` | Employee.Promotion.Edit | Create/update promotion |
| DeletePromotion | POST | `/EmployeeHR/Promotion/Delete` | Employee.Promotion.Delete | Delete promotion |
| Transfers | GET | `/EmployeeHR/Transfers/{employeeId}` | Employee.Transfer.View | List transfers |
| SaveTransfer | POST | `/EmployeeHR/Transfer/Save` | Employee.Transfer.Edit | Create/update transfer |
| DeleteTransfer | POST | `/EmployeeHR/Transfer/Delete` | Employee.Transfer.Delete | Delete transfer |
| Training | GET | `/EmployeeHR/Training/{employeeId}` | Employee.Training.View | List training records |
| SaveTraining | POST | `/EmployeeHR/Training/Save` | Employee.Training.Edit | Create/update training |
| DeleteTraining | POST | `/EmployeeHR/Training/Delete` | Employee.Training.Delete | Delete training |
| Awards | GET | `/EmployeeHR/Awards/{employeeId}` | Employee.Award.View | List awards |
| SaveAward | POST | `/EmployeeHR/Award/Save` | Employee.Award.Edit | Create/update award |
| DeleteAward | POST | `/EmployeeHR/Award/Delete` | Employee.Award.Delete | Delete award |
| Disciplinary | GET | `/EmployeeHR/Disciplinary/{employeeId}` | Employee.Disciplinary.View | List disciplinary actions |
| SaveDisciplinary | POST | `/EmployeeHR/Disciplinary/Save` | Employee.Disciplinary.Edit | Create/update action |
| DeleteDisciplinary | POST | `/EmployeeHR/Disciplinary/Delete` | Employee.Disciplinary.Delete | Delete action |
| ResolveDisciplinary | POST | `/EmployeeHR/Disciplinary/Resolve?id={id}&employeeId={eid}&resolutionRemarks={text}` | Employee.Disciplinary.Edit | Mark action resolved |

All POST actions require `[ValidateAntiForgeryToken]`.

All list actions return `ViewResult` with their respective DTO lists. All Save/Create actions redirect back to the list view.

---

### EmployeePayrollController

**Route:** `/EmployeePayroll`
**Authorization:** `[Authorize]`

| Action | Method | Route | Permission |
|---|---|---|---|
| Index | GET | `/EmployeePayroll/Index?employeeId={id}` | Employee.Salary.View |
| Create | GET | `/EmployeePayroll/Create?employeeId={id}` | Employee.Salary.Create |
| Create | POST | `/EmployeePayroll/Create` | Employee.Salary.Create |
| Edit | GET | `/EmployeePayroll/Edit/{id}` | Employee.Salary.Edit |
| Edit | POST | `/EmployeePayroll/Edit` | Employee.Salary.Edit |
| Delete | POST | `/EmployeePayroll/Delete?id={id}&employeeId={eid}` | Employee.Salary.Delete |

All POST actions require `[ValidateAntiForgeryToken]`.

`EmployeeSalaryDto` fields:
```json
{
  "id": 0,
  "employeeId": 1,
  "employeeName": null,
  "basicSalary": 25000.00,
  "houseRent": 10000.00,
  "medicalAllowance": 2000.00,
  "transportAllowance": 1500.00,
  "otherAllowance": 1000.00,
  "deduction": 3000.00,
  "totalSalary": 36500.00,
  "effectiveFrom": "2026-01-01T00:00:00"
}
```

The `TotalSalary` is computed server-side as: `BasicSalary + HouseRent + MedicalAllowance + TransportAllowance + OtherAllowance - Deduction`.

---

### EmployeeInvitationController

**Route:** `/EmployeeInvitation`
**Authorization:** `[Authorize]`

| Action | Method | Route | Permission |
|---|---|---|---|
| Index | GET | `/EmployeeInvitation/Index?page=1&size=10&search=` | Employees.Invite |
| Create | GET | `/EmployeeInvitation/Create` | Employees.Invite |
| Create | POST | `/EmployeeInvitation/Create` | Employees.Invite |
| Resend | POST | `/EmployeeInvitation/Resend` (body: int id) | Employees.Invite |
| Cancel | POST | `/EmployeeInvitation/Cancel` (body: int id) | Employees.Invite |

JSON endpoints return:
```json
{
  "success": true,
  "message": "Invitation resent successfully."
}
```

---

### OnboardingController

**Route:** `/Onboarding`
**Authorization:** `[AllowAnonymous]` (public)

| Action | Method | Route | Purpose |
|---|---|---|---|
| Welcome | GET | `/Onboarding/Welcome?token={token}` | Validate token, redirect to Start |
| Start | GET | `/Onboarding/Start?token={token}` | Show onboarding form |
| Submit | POST | `/Onboarding/Submit?token={token}&password={pwd}&confirmPassword={pwd}` | Complete onboarding |

On Submit, validates password confirmation and calls `CompleteOnboardingAsync`.

---

## Service Interfaces

### IEmployeeService

```csharp
public interface IEmployeeService
{
    Task<(List<EmployeeListItemDto> items, int totalRecords)> GetPagedAsync(
        int page, int pageSize, string? search, int? departmentId,
        int? designationId, bool? isTeachingStaff, string? status, CancellationToken ct);

    Task<EmployeeUpsertDto?> GetForEditAsync(int id, CancellationToken ct);
    Task<EmployeeDetailsDto?> GetDetailsAsync(int id, CancellationToken ct);
    Task<EmployeeUpsertDto?> GetByUserIdAsync(int userId, CancellationToken ct);

    Task<int> SaveAsync(EmployeeUpsertDto dto, CancellationToken ct);
    Task<bool> DeleteAsync(int id, CancellationToken ct);
    Task<bool> UpdateStatusAsync(int id, string status, CancellationToken ct);

    Task<bool> IsCodeExistsAsync(string code, int? excludeId, CancellationToken ct);
    Task<bool> IsEmailExistsAsync(string email, int? excludeId, CancellationToken ct);
    Task<bool> IsPhoneExistsAsync(string phone, int? excludeId, CancellationToken ct);

    Task<EmployeeDashboardDto> GetDashboardAsync(CancellationToken ct);
}
```

### IEmployeeHrService

```csharp
public interface IEmployeeHrService
{
    // Bank Accounts
    Task<List<EmployeeBankAccountDto>> GetBankAccountsAsync(int employeeId, CancellationToken ct);
    Task SaveBankAccountAsync(EmployeeBankAccountDto dto, CancellationToken ct);
    Task DeleteBankAccountAsync(int id, CancellationToken ct);

    // Promotions
    Task<List<EmployeePromotionDto>> GetPromotionsAsync(int employeeId, CancellationToken ct);
    Task SavePromotionAsync(EmployeePromotionDto dto, CancellationToken ct);
    Task DeletePromotionAsync(int id, CancellationToken ct);

    // Transfers
    Task<List<EmployeeTransferDto>> GetTransfersAsync(int employeeId, CancellationToken ct);
    Task SaveTransferAsync(EmployeeTransferDto dto, CancellationToken ct);
    Task DeleteTransferAsync(int id, CancellationToken ct);

    // Training
    Task<List<EmployeeTrainingDto>> GetTrainingsAsync(int employeeId, CancellationToken ct);
    Task SaveTrainingAsync(EmployeeTrainingDto dto, CancellationToken ct);
    Task DeleteTrainingAsync(int id, CancellationToken ct);

    // Awards
    Task<List<EmployeeAwardDto>> GetAwardsAsync(int employeeId, CancellationToken ct);
    Task SaveAwardAsync(EmployeeAwardDto dto, CancellationToken ct);
    Task DeleteAwardAsync(int id, CancellationToken ct);

    // Disciplinary Actions
    Task<List<EmployeeDisciplinaryActionDto>> GetDisciplinaryActionsAsync(int employeeId, CancellationToken ct);
    Task SaveDisciplinaryActionAsync(EmployeeDisciplinaryActionDto dto, CancellationToken ct);
    Task DeleteDisciplinaryActionAsync(int id, CancellationToken ct);
    Task ResolveDisciplinaryActionAsync(int id, string resolutionRemarks, CancellationToken ct);
}
```

### IEmployeePayrollService

```csharp
public interface IEmployeePayrollService
{
    Task<List<EmployeeSalaryDto>> GetSalariesByEmployeeIdAsync(int employeeId, CancellationToken ct);
    Task<EmployeeSalaryDto?> GetSalaryByIdAsync(int id, CancellationToken ct);
    Task SaveSalaryAsync(EmployeeSalaryDto dto, CancellationToken ct);
    Task DeleteSalaryAsync(int id, CancellationToken ct);
}
```

### IEmployeeInvitationService

```csharp
public interface IEmployeeInvitationService
{
    Task<(List<EmployeeInvitationDto> items, int totalRecords)> GetPagedInvitationsAsync(
        int page, int pageSize, string? search, CancellationToken ct);

    Task<EmployeeInvitationDto?> GetInvitationByIdAsync(int id, CancellationToken ct);
    Task<EmployeeInvitationDto?> GetInvitationByTokenAsync(string token, CancellationToken ct);

    Task<int> CreateInvitationAsync(EmployeeInvitationUpsertDto dto, int createdByUserId, CancellationToken ct);
    Task<bool> ResendInvitationAsync(int id, CancellationToken ct);
    Task<bool> CancelInvitationAsync(int id, CancellationToken ct);

    Task<bool> ValidateTokenAsync(string token, CancellationToken ct);
    Task<bool> MarkInvitationOpenedAsync(string token, CancellationToken ct);

    Task<(bool success, string message)> CompleteOnboardingAsync(
        EmployeeUpsertDto model, string token, string password, CancellationToken ct);

    Task<bool> ApproveOnboardingAsync(int id, int approvedByUserId, CancellationToken ct);
}
```

### Supporting Service Interfaces

```csharp
public interface IDepartmentService
{
    Task<IReadOnlyList<DepartmentDto>> GetAllAsync(CancellationToken ct);
}

public interface IDesignationService
{
    Task<IReadOnlyList<DesignationDto>> GetAllAsync(CancellationToken ct);
}

public interface IUserProvisionService
{
    Task<(int userId, string username, string password)> ProvisionUserForEmployeeAsync(
        Models.Entities.Employee.Employee employee, CancellationToken ct);
}
```

## DTO Reference

| DTO | Purpose | Source File |
|---|---|---|
| `EmployeeListItemDto` | Grid list items | EmployeeDtos.cs:8 |
| `EmployeeUpsertDto` | Create/Edit form | EmployeeDtos.cs:45 |
| `EmployeeDetailsDto` | Full profile details | EmployeeDtos.cs:158 |
| `EmployeeQualificationDto` | Qualification CRUD | EmployeeDtos.cs:215 |
| `EmployeeDocumentDto` | Document CRUD | EmployeeDtos.cs:245 |
| `EmployeeExperienceDto` | Experience CRUD | EmployeeDtos.cs:266 |
| `EmployeeInvitationDto` | Invitation list items | EmployeeDtos.cs:302 |
| `EmployeeInvitationUpsertDto` | Invitation create form | EmployeeDtos.cs:329 |
| `DesignationDto` | Designation lookup | EmployeeDtos.cs:287 |
| `DepartmentDto` | Department lookup | EmployeeDtos.cs:296 |
| `EmployeeSalaryDto` | Salary CRUD | EmployeeSalaryDto.cs:5 |
| `EmployeeBankAccountDto` | Bank account CRUD | EmployeeHrDtos.cs:6 |
| `EmployeePromotionDto` | Promotion CRUD | EmployeeHrDtos.cs:32 |
| `EmployeeTransferDto` | Transfer CRUD | EmployeeHrDtos.cs:60 |
| `EmployeeTrainingDto` | Training CRUD | EmployeeHrDtos.cs:82 |
| `EmployeeAwardDto` | Award CRUD | EmployeeHrDtos.cs:108 |
| `EmployeeDisciplinaryActionDto` | Disciplinary CRUD | EmployeeHrDtos.cs:180 |
| `EmployeeDashboardDto` | Dashboard aggregates | EmployeeHrDtos.cs:129 |
| `DepartmentStat` | Per-department stats | EmployeeHrDtos.cs:147 |
| `StatusStat` | Per-status stats | EmployeeHrDtos.cs:155 |
| `RecentHireDto` | Recent hire display | EmployeeHrDtos.cs:161 |
| `BirthdayDto` | Birthday display | EmployeeHrDtos.cs:171 |
