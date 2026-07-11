# Employee Module — Workflows

## 1. Employee Create Workflow (with Auto User Provisioning)

**Trigger:** HR user submits the Create/Edit form (`EmployeeController.Save` POST)

**Actors:** HR Admin (requires `Employees.Create` or `Employees.Edit`)

**Steps:**

```
User submits EmployeeUpsertDto
    │
    ├─ Runtime permission check: dto.Id == 0 → "Employees.Create", else "Employees.Edit"
    │
    ├─ ModelState validation (server-side)
    │
    ├─ EmployeeService.SaveAsync()
    │   │
    │   ├─ UNIQUENESS CHECKS:
    │   │   ├─ Phone exists? → throw InvalidOperationException
    │   │   ├─ Email exists? → throw InvalidOperationException
    │   │   └─ NIDNumber exists? → throw InvalidOperationException
    │   │
    │   ├─ INSERT MODE (dto.Id == 0):
    │   │   ├─ Generate EmployeeCode (EMP-{YYYY}-{NNNN})
    │   │   ├─ Map DTO → Employee entity
    │   │   ├─ Handle file uploads (profile photo, signature)
    │   │   ├─ Save employee → get ID
    │   │   ├─ AUTO USER PROVISIONING:
    │   │   │   ├─ Call IUserProvisionService.ProvisionUserForEmployeeAsync()
    │   │   │   ├─ Creates ApplicationUser with:
    │   │   │   │   - Username (derived from EmployeeCode)
    │   │   │   │   - Auto-generated password
    │   │   │   │   - Status = Active, MustChangePassword = true
    │   │   │   ├─ Assigns roles via DesignationRoleMapping
    │   │   │   ├─ Sets employee.UserId = user.Id
    │   │   │   └─ Sends email with credentials (on best-effort basis)
    │   │   └─ If provisioning fails: logged, employee saved with note in Remarks
    │   │
    │   ├─ EDIT MODE (dto.Id > 0):
    │   │   ├─ Load existing employee with Qualifications/Documents/Experiences
    │   │   ├─ Update all mapped fields
    │   │   ├─ Update profile photo/signature if new files provided
    │   │   └─ Update linked ApplicationUser email/phone if changed
    │   │
    │   ├─ PROCESS CHILD COLLECTIONS:
    │   │   ├─ Qualifications (add/update/soft-delete)
    │   │   ├─ Documents (add/update/soft-delete with file management)
    │   │   └─ Experiences (add/update/soft-delete)
    │   │
    │   ├─ SaveChanges
    │   │
    │   └─ TEACHER SYNCHRONIZATION:
    │       └─ ITeacherSynchronizationService.SyncEmployeeToTeacherAsync(id)
    │           - If IsTeachingStaff → creates/updates Teacher record
    │           - If !IsTeachingStaff → no-op (does not delete existing Teacher)
    │
    └─ AUDIT LOG: "Employee.Create" or "Employee.Update"
```

**On success:** Redirect to Details page with success message.

**Error handling:** Model errors return to CreateEdit view with populated lookup lists. `InvalidOperationException` shows business validation message. Unexpected exceptions show generic error.

---

## 2. Employee → Teacher Synchronization Workflow

**Trigger:** Called automatically after:
- Employee Create/Edit (`EmployeeService.SaveAsync`)
- Employee Status Change (`EmployeeService.UpdateStatusAsync`)
- Employee Delete (`EmployeeService.DeleteAsync`)
- Onboarding completion (`EmployeeInvitationService.CompleteOnboardingAsync`)

**Actor:** `ITeacherSynchronizationService` (implementation: `TeacherSynchronizationService`)

**Logic:**

```
SyncEmployeeToTeacherAsync(employeeId)
    │
    ├─ Load Employee (with IsTeachingStaff flag)
    │
    ├─ IF IsTeachingStaff == true AND no existing Teacher record:
    │   ├─ Create Teacher entity
    │   ├─ Copy: FullName, Phone, Email, ProfilePicturePath
    │   ├─ Set: TeacherCode (derived), EmployeeId = employee.Id
    │   ├─ Mark: IsActive = (Employee.Status == "Active")
    │   └─ Save
    │
    ├─ IF IsTeachingStaff == false AND Teacher exists:
    │   └─ No automatic deletion (teacher retains history)
    │
    └─ IF Employee is deleted/soft-deleted AND Teacher exists:
        └─ Set Teacher.IsDeleted = true
```

**Error handling:** Failures are logged but never thrown — employee save always succeeds regardless of teacher sync outcome.

---

## 3. Leave Workflow

**Trigger:** Employee/HR submits leave application via `LeaveController` (cross-module)

**Employee Module Integration:**

The Employee module provides:
- Employee identity for leave validation
- Leave history querying via `sp_GetLeaveSummary` stored procedure
- Employee status auto-update (when leave is approved, employee may show "On Leave")

**Relationships:** `LeaveApplication.EmployeeId` → `Employees.Id`

**Leave Types** (stored in `LeaveType` column): Sick Leave, Casual Leave, Annual Leave, Maternity Leave, Leave Without Pay, etc.

---

## 4. Payroll Workflow

**Trigger:** HR user manages salary records via `EmployeePayrollController`

**Actors:** Finance/HR (requires `Employee.Salary.*` permissions)

### Create/Edit Salary

```
User opens Create/Edit form
    │
    ├─ Select employee → populate ViewBag.EmployeeName
    ├─ Enter salary components:
    │   ├─ BasicSalary
    │   ├─ HouseRent
    │   ├─ MedicalAllowance
    │   ├─ TransportAllowance
    │   ├─ OtherAllowance
    │   └─ Deduction
    │
    └─ On save (EmployeePayrollService.SaveSalaryAsync()):
        ├─ TotalSalary = BasicSalary + HouseRent + MedicalAllowance
        │                + TransportAllowance + OtherAllowance - Deduction
        ├─ Set EffectiveFrom date
        └─ Audit Log: "Employee.Salary.Create" / "Employee.Salary.Update"
```

### View Salary History

```
EmployeePayrollController.Index(employeeId)
    └─ EmployeePayrollService.GetSalariesByEmployeeIdAsync()
        └─ Returns all salary records ordered by EffectiveFrom DESC
```

### Delete Salary

```
Soft-deletes the EmployeeSalary record with audit log.
```

### Payroll Summary (Stored Procedure)

`sp_GetPayrollSummary(@EmployeeId)` returns:
- All salary records with employee name
- Aggregate stats: TotalSalaryRecords, CurrentSalary, AvgSalary, MinSalary, LastUpdated

---

## 5. Promotion Workflow

**Trigger:** HR user manages promotions via `EmployeeHRController.Promotions`

**Actors:** HR Admin (requires `Employee.Promotion.*`)

**Steps:**

```
HR opens Promotions tab for employee
    │
    ├─ View existing promotions (ordered by PromotionDate DESC)
    │
    ├─ Create new promotion:
    │   ├─ Select PreviousDesignationId (auto-filled from current)
    │   ├─ Select NewDesignationId
    │   ├─ Set PromotionDate
    │   ├─ Enter PreviousSalary, NewSalary (optional)
    │   └─ Reason and Remarks
    │
    └─ On save:
        ├─ EmployeePromotion entity created
        ├─ Audit log entry
        └─ Note: Employee.DesignationId is NOT auto-updated
            (this is intentional — HR must update designation separately)
```

---

## 6. Transfer Workflow

**Trigger:** HR user manages transfers via `EmployeeHRController.Transfers`

**Actors:** HR Admin (requires `Employee.Transfer.*`)

**Steps:**

```
HR opens Transfers tab for employee
    │
    ├─ View existing transfers (ordered by TransferDate DESC)
    │
    ├─ Create new transfer:
    │   ├─ Select FromDepartmentId (auto-filled from current)
    │   ├─ Select ToDepartmentId
    │   ├─ Set TransferDate
    │   └─ Reason and Remarks
    │
    └─ On save:
        ├─ EmployeeTransfer entity created
        ├─ Audit log entry
        └─ Note: Employee.DepartmentId is NOT auto-updated
            (intentional — HR must update department separately)
```

---

## 7. Invitation/Onboarding Workflow

**Trigger:** HR creates invitation, invitee completes self-service onboarding

**Actors:** HR Admin (creates invitation), Invitee (completes onboarding, anonymous)

### Phase 1: HR Creates Invitation

```
HR Admin opens EmployeeInvitationController.Create
    │
    ├─ Fill EmployeeInvitationUpsertDto:
    │   ├─ FullName, Email, Mobile
    │   ├─ Department, Designation
    │   ├─ JoiningDate
    │   ├─ EmploymentType, IsTeachingStaff
    │   └─ Remarks
    │
    └─ EmployeeInvitationService.CreateInvitationAsync()
        ├─ Check: email not already in active invitation
        ├─ Check: email not already in Employee table
        ├─ Generate InvitationCode (INV-{YYYY}-{NNNN})
        ├─ Generate InvitationToken (GUID-based, 64 hex chars)
        ├─ Set ExpiresAt = UTCNow + 72 hours
        ├─ Set InvitationStatus = "Pending"
        ├─ Save invitation
        ├─ Send email via IEmailService.SendEmployeeInvitationAsync()
        ├─ Update status to "Sent", set SentAt
        └─ Return invitation ID
```

### Phase 2: Invitee Receives Email

Email contains link: `https://school.example.com/Onboarding/Welcome?token={InvitationToken}`

### Phase 3: Invitee Opens Link

```
OnboardingController.Welcome(token)
    │
    ├─ Validate: token exists and not expired
    ├─ IF invalid → show InvalidToken/Expired view
    ├─ IF already used → show AlreadyUsed view
    └─ Redirect to OnboardingController.Start(token)
```

### Phase 4: Invitee Completes Onboarding

```
OnboardingController.Start(token)
    │
    ├─ Load invitation metadata
    ├─ Mark invitation as Opened (InvitationStatus = "Opened")
    ├─ Pre-fill form: FullName, Email, Phone, Department, Designation, JoiningDate
    ├─ Show full EmployeeUpsertDto form (with validation)
    │
    OnboardingController.Submit(model, token, password, confirmPassword)
    │
    └─ EmployeeInvitationService.CompleteOnboardingAsync()
        │
        ├─ Validate token
        │
        ├─ CREATE EMPLOYEE:
        │   ├─ Generate EmployeeCode
        │   ├─ Map DTO → Employee entity
        │   ├─ Set admin-locked fields from invitation:
        │   │   - JoiningDate, DepartmentId, DesignationId
        │   │   - IsTeachingStaff
        │   ├─ Handle file uploads
        │   └─ Save employee
        │
        ├─ ADD QUALIFICATIONS + EXPERIENCES
        │
        ├─ CREATE USER ACCOUNT:
        │   ├─ Username = EmployeeCode (lowercase, no hyphens)
        │   ├─ Handle duplicate username by appending suffix
        │   ├─ Hash password via IPasswordHashService
        │   ├─ Set MustChangePassword = true
        │   └─ Create ApplicationUser
        │
        ├─ RBAC MAPPING:
        │   └─ Query DesignationRoleMapping → create UserRole records
        │
        ├─ TEACHER SYNC:
        │   └─ If IsTeachingStaff → SyncEmployeeToTeacherAsync
        │
        ├─ SEND EMAILS:
        │   ├─ SendEmployeeAccountAsync (credentials)
        │   └─ If teaching → SendTeacherAccountAsync
        │
        ├─ MARK INVITATION COMPLETED:
        │   ├─ IsUsed = true
        │   ├─ InvitationStatus = "Completed"
        │   ├─ CompletedAt = UTCNow
        │   └─ CreatedEmployeeId = employee.Id
        │
        └─ Audit Log: "Employee.Invitation.CompleteOnboarding"
```

### Phase 5: HR Approval (Optional)

```
EmployeeInvitationController.Cancel(id) — cancels pending invitation
EmployeeInvitationController.Resend(id) — regenerates token, resends email
```

---

## 8. Disciplinary Action Workflow

**Trigger:** HR user manages disciplinary actions via `EmployeeHRController.Disciplinary`

**Actors:** HR Admin (requires `Employee.Disciplinary.*`)

### Create Action

```
Select employee → open Disciplinary tab
    │
    ├─ Enter ActionType (e.g., Warning, Suspension, Termination)
    ├─ Set ActionDate, Reason, Description
    ├─ Upload supporting document (optional)
    │
    └─ On save:
        ├─ EmployeeDisciplinaryAction created
        ├─ IsResolved = false
        └─ Audit log entry
```

### Resolve Action

```
Mark disciplinary action as resolved
    │
    ├─ Set IsResolved = true
    ├─ Set ResolvedAt = UTCNow
    ├─ Enter ResolutionRemarks
    └─ Audit log: "Employee.Disciplinary.Resolve"
```

### Delete Action

```
Soft-deletes the disciplinary action record with audit log.
```

---

## 9. Employee Status Change Workflow

**Trigger:** HR user clicks "Update Status" on employee details page

```
EmployeeController.UpdateStatus(id, newStatus)
    │
    ├─ EmployeeService.UpdateStatusAsync()
    │   ├─ Load employee
    │   ├─ Set Status = newStatus
    │   ├─ IF UserId exists → sync ApplicationUser.Status:
    │   │   - "Active" → AccountStatus.Active
    │   │   - otherwise → AccountStatus.Inactive
    │   └─ SaveChanges
    │
    ├─ Teacher Sync (if teaching staff)
    ├─ Audit Log: "Employee.StatusChange"
    └─ Redirect to Details with success message
```

---

## 10. Employee Delete (Soft Delete) Workflow

**Trigger:** HR user deletes employee record

```
EmployeeController.Delete(id)
    │
    ├─ EmployeeService.DeleteAsync()
    │   ├─ Load employee
    │   ├─ Set IsDeleted = true, Status = "Deleted"
    │   ├─ IF UserId exists:
    │   │   ├─ Set User.IsDeleted = true
    │   │   └─ Set User.Status = AccountStatus.Inactive
    │   ├─ SaveChanges
    │   └─ Teacher Sync (deactivate teacher profile)
    │
    ├─ Audit Log: "Employee.Delete"
    └─ Redirect to Index with success message
```

---

## 11. ID Card Generation and Verification Workflow

### Auto-Generation

Triggered on first Details view for an employee without a card number:

```
EmployeeController.Details(id)
    │
    ├─ IF EmployeeCardNumber is null/empty:
    │   ├─ Generate CARD-{YYYY}-{Id:D6}
    │   ├─ Set CardIssueDate = today
    │   ├─ Set CardExpiryDate = Dec 31, 2 years from now
    │   ├─ Set CardVersion = 1
    │   ├─ Generate QRVerificationCode (GUID-based, 10 chars uppercase)
    │   └─ Save
    │
    └─ Generate QR code base64 image for view display
```

### Public Verification

```
GET /Employee/Verify/{id}
    │
    ├─ Load employee details
    ├─ Check: Status == "Active" AND CardExpiryDate >= today
    ├─ Set ViewBag.IsValid
    ├─ If teaching staff, load Teacher details (TeacherCode, specialization, etc.)
    └─ Show Verify view with QR code + validity indicator
```
