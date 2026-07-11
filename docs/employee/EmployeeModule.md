# Employee Management Module

## Purpose and Scope

The Employee Management module serves as the master HR identity layer for all school personnel. Every person employed by the institution — teaching staff, administrative staff, support staff — is represented by a single **Employee** record. This record serves as the root identity from which system user accounts, teacher profiles, payroll records, and HR workflows are derived.

The module manages the complete employee lifecycle:

- **Onboarding** — invitation-driven self-service registration
- **Employment** — core profile, documents, qualifications, experience
- **HR Operations** — promotions, transfers, training, awards, disciplinary actions
- **Payroll** — salary structure definition with allowances and deductions
- **ID Card Management** — QR-coded employee cards with verification
- **Separation** — resignation, retirement, or deletion with cascading deactivation

## Architecture

The module follows the project's Clean Architecture with strict layering:

```
┌─────────────────────────────────────────────────────────┐
│  Controllers (4)                                        │
│  EmployeeController, EmployeeHRController,               │
│  EmployeePayrollController, EmployeeInvitationController │
│  OnboardingController (anonymous access)                 │
├─────────────────────────────────────────────────────────┤
│  Services (4 interfaces + 4 implementations)             │
│  IEmployeeService       → EmployeeService                │
│  IEmployeeHrService     → EmployeeHrService              │
│  IEmployeePayrollService → EmployeePayrollService        │
│  IEmployeeInvitationService → EmployeeInvitationService  │
│  IDepartmentService, IDesignationService                 │
│  IUserProvisionService                                   │
├─────────────────────────────────────────────────────────┤
│  Repositories (12)                                       │
│  EmployeeRepository, DepartmentRepository,               │
│  DesignationRepository, EmployeeQualificationRepository, │
│  EmployeeDocumentRepository, EmployeeExperienceRepository│
│  EmployeeInvitationRepository, EmployeeBankAccountRepo   │
│  EmployeePromotionRepository, EmployeeTransferRepository │
│  EmployeeTrainingRepository, EmployeeAwardRepository     │
│  EmployeeDisciplinaryActionRepository                    │
├─────────────────────────────────────────────────────────┤
│  Stored Procedures (7)                                   │
│  sp_GetEmployeeDashboard, sp_GetEmployeesPaged,          │
│  sp_GetEmployeeDetails, sp_GetEmployeeInvitationList,    │
│  sp_GetAttendanceSummary, sp_GetLeaveSummary,            │
│  sp_GetPayrollSummary                                    │
├─────────────────────────────────────────────────────────┤
│  Entities (14 tables)                                    │
└─────────────────────────────────────────────────────────┘
```

### Dependency Injection Registration

All services are registered in `Extensions/ServiceRegistration.cs` (lines 233–242):

| Interface | Implementation |
|---|---|
| `IEmployeeService` | `EmployeeService` |
| `IEmployeeInvitationService` | `EmployeeInvitationService` |
| `IEmployeePayrollService` | `EmployeePayrollService` |
| `IEmployeeHrService` | `EmployeeHrService` |
| `IDepartmentService` | `DepartmentService` |
| `IDesignationService` | `DesignationService` |
| `IUserProvisionService` | `UserProvisionService` |
| `IIdCardService` | `IdCardService` |

## Key Entities and Relationships

### Core Entity: `Employee`

The `Employee` entity (`Models/Entities/Employee/EmployeeEntities.cs:8`) is the central HR record. It extends `BaseEntity` (Id, CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted).

**Identity fields:** EmployeeCode (unique), FullName, BanglaName, FatherName, MotherName, SpouseName
**Personal fields:** Gender, MaritalStatus, DateOfBirth, BloodGroup, Religion, Nationality
**Document fields:** NIDNumber, BirthCertificateNo, PassportNo, TIN, DrivingLicenseNo
**Contact fields:** Phone (unique), AlternateMobile, Email (unique), PresentAddress, PermanentAddress
**Employment fields:** JoiningDate, EmployeeType (Full-Time/Part-Time/Contract), IsTeachingStaff, Status (Active/Inactive/On Leave/Resigned/Retired)
**ID Card fields:** EmployeeCardNumber, CardIssueDate, CardExpiryDate, CardPrintedAt, CardVersion, QRVerificationCode
**Emergency:** EmergencyContactName, EmergencyContactPhone
**System:** UserId → ApplicationUser, ProfilePicturePath, SignaturePath, Remarks

### Lookup Entities

| Entity | Purpose | Key Fields |
|---|---|---|
| `Department` | Organizational unit | Name |
| `Designation` | Job title/role | Name, RoleLevel, IsTeachingRole, IsAdministrativeRole, RequiresLogin, IsActive |
| `DesignationRoleMapping` | Maps Designation → ApplicationRole | DesignationId, RoleId, IsActive |

### Child Entities (all reference Employee via EmployeeId)

| Entity | Purpose | Key Fields |
|---|---|---|
| `EmployeeQualification` | Academic credentials | ExamName, BoardOrUniversity, InstituteName, GroupOrSubject, PassingYear, Result, CGPAOrDivision, CertificateFilePath |
| `EmployeeDocument` | Uploaded documents | DocumentType, DocumentName, FilePath, ExpiryDate |
| `EmployeeExperience` | Prior work history | OrganizationName, Designation, StartDate, EndDate |
| `EmployeeSalary` | Pay structure | BasicSalary, HouseRent, MedicalAllowance, TransportAllowance, OtherAllowance, Deduction, TotalSalary, EffectiveFrom |
| `EmployeeBankAccount` | Banking info | BankName, BranchName, AccountNumber, RoutingNumber, AccountType, IsDefault, IsActive |
| `EmployeePromotion` | Career advancement | PreviousDesignationId, NewDesignationId, PromotionDate, PreviousSalary, NewSalary |
| `EmployeeTransfer` | Department moves | FromDepartmentId, ToDepartmentId, TransferDate |
| `EmployeeTraining` | Professional development | TrainingName, InstitutionName, Duration, StartDate, EndDate |
| `EmployeeAward` | Recognition | AwardName, AwardedBy, AwardDate |
| `EmployeeDisciplinaryAction` | Conduct records | ActionType, Reason, ActionDate, IsResolved, ResolvedAt |
| `EmployeeAcademicAssignment` | Teaching assignments | ClassId, SectionId, SubjectId, AcademicYearId |

### System Integration Entities

| Entity | Relationship | Notes |
|---|---|---|
| `ApplicationUser` | Employee → UserId | User provisioning occurs automatically on employee creation or onboarding |
| `Teacher` | Employee → Teacher.EmployeeId | Teacher profile is synchronized automatically via `ITeacherSynchronizationService` |
| `EmployeeAttendance` | Employee → Attendances | Attendance records reference EmployeeId |
| `LeaveApplication` | Employee → Leaves | Leave applications reference EmployeeId |
| `EmployeeInvitation` | Standalone onboarding record | InvitationCode unique, InvitationToken unique; creates Employee on completion |

### Relationship Summary

```
Employee (1) ──→ EmployeeQualification (many)
Employee (1) ──→ EmployeeDocument (many)
Employee (1) ──→ EmployeeExperience (many)
Employee (1) ──→ EmployeeSalary (many)
Employee (1) ──→ EmployeeBankAccount (many)
Employee (1) ──→ EmployeePromotion (many)
Employee (1) ──→ EmployeeTransfer (many)
Employee (1) ──→ EmployeeTraining (many)
Employee (1) ──→ EmployeeAward (many)
Employee (1) ──→ EmployeeDisciplinaryAction (many)
Employee (1) ──→ EmployeeAcademicAssignment (many)
Employee (1) ──→ EmployeeAttendance (many)
Employee (1) ──→ LeaveApplication (many)
Employee (1) ──→ ApplicationUser (1)  [UserId]
Employee (1) ──→ Teacher (1)          [EmployeeId]
Department (1) ──→ Employee (many)
Designation (1) ──→ Employee (many)
Designation (1) ──→ DesignationRoleMapping (many) ──→ Role (many)
```

## Module Features

| # | Feature | Controller | Permission |
|---|---|---|---|
| 1 | Employee Dashboard | EmployeeController.Dashboard | Employees.View |
| 2 | Employee List (paginated, filterable) | EmployeeController.Index | Employees.View |
| 3 | Create Employee (with auto user provisioning) | EmployeeController.Save | Employees.Create |
| 4 | Edit Employee | EmployeeController.Save | Employees.Edit |
| 5 | View Employee Details (with ID card, attendance, salary, teacher profile) | EmployeeController.Details | Employees.View (or own profile) |
| 6 | Soft-Delete Employee | EmployeeController.Delete | Employees.Delete |
| 7 | Update Employment Status | EmployeeController.UpdateStatus | Employees.Edit |
| 8 | Download Service Book PDF | EmployeeController.DownloadServiceBookPdf | Employees.View |
| 9 | Verify Employee ID Card (QR code) | EmployeeController.Verify | Employees.View (public-facing) |
| 10 | Unique Code/Email/Phone Verification | EmployeeController.VerifyCode/Email/Phone | Employees.View |
| 11 | Bank Account CRUD | EmployeeHRController | Employee.BankAccount.* |
| 12 | Promotion CRUD | EmployeeHRController | Employee.Promotion.* |
| 13 | Transfer CRUD | EmployeeHRController | Employee.Transfer.* |
| 14 | Training CRUD | EmployeeHRController | Employee.Training.* |
| 15 | Award CRUD | EmployeeHRController | Employee.Award.* |
| 16 | Disciplinary Action CRUD + Resolve | EmployeeHRController | Employee.Disciplinary.* |
| 17 | Salary CRUD | EmployeePayrollController | Employee.Salary.* |
| 18 | Invitation (onboarding) CRUD | EmployeeInvitationController | Employees.Invite |
| 19 | Self-Service Onboarding (anonymous) | OnboardingController | None (anonymous) |
| 20 | Employee ID Card Printing | IdCardController | (external) |
| 21 | Public ID Card Verification | VerifyController | None (public) |

### Cross-Cutting Features

- **Automatic User Provisioning:** Creating an employee (or completing onboarding) automatically creates an `ApplicationUser` with generated username/password, sends account credentials via email, and assigns roles based on `DesignationRoleMapping`.
- **Automatic Teacher Synchronization:** When `IsTeachingStaff` is true, a `Teacher` profile is automatically created/updated/deleted via `ITeacherSynchronizationService.SyncEmployeeToTeacherAsync()`.
- **Soft Delete:** All entities use `IsDeleted` flag; deletes cascade status changes to the linked `ApplicationUser`.
- **Audit Logging:** Every Create, Update, Delete, Resolve, and StatusChange operation logs to `AuditLog` with user ID, IP address, and details.
- **File Uploads:** Profile photos, signatures, qualification certificates, documents are uploaded with MIME/extension validation (5 MB max, jpg/png/pdf/doc/docx).
- **ID Card System:** QR-verified employee cards with automatic generation on first profile view.
