# Employee Module — Database Schema

## Table: Employees

The central HR identity table.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeCode | nvarchar(50) | NOT NULL, UNIQUE | Auto-generated (`EMP-{YYYY}-{NNNN}`) |
| FullName | nvarchar(120) | NOT NULL | Employee full name |
| BanglaName | nvarchar(120) | NULL | Name in Bengali script |
| FatherName | nvarchar(120) | NULL | Father's name |
| MotherName | nvarchar(120) | NULL | Mother's name |
| SpouseName | nvarchar(120) | NULL | Spouse's name |
| Gender | nvarchar(20) | NOT NULL | Male / Female / Other |
| MaritalStatus | nvarchar(20) | NULL | Single / Married / Divorced / Widowed |
| DateOfBirth | datetime2 | NOT NULL | Date of birth |
| BloodGroup | nvarchar(10) | NULL | A+, A-, B+, B-, AB+, AB-, O+, O- |
| Religion | nvarchar(50) | NULL | Religion |
| Nationality | nvarchar(50) | NOT NULL, Default: 'Bangladeshi' | Nationality |
| NIDNumber | nvarchar(50) | NULL, UNIQUE FILTERED | National ID card number |
| BirthCertificateNo | nvarchar(50) | NULL, UNIQUE FILTERED | Birth certificate number |
| PassportNo | nvarchar(50) | NULL, UNIQUE FILTERED | Passport number |
| TIN | nvarchar(50) | NULL, UNIQUE FILTERED | Tax Identification Number |
| DrivingLicenseNo | nvarchar(50) | NULL, UNIQUE FILTERED | Driving license number |
| Phone | nvarchar(30) | NOT NULL, UNIQUE | Primary contact number |
| AlternateMobile | nvarchar(30) | NULL | Secondary contact number |
| Email | nvarchar(160) | NULL, UNIQUE | Email address |
| PresentAddress | nvarchar(500) | NULL | Current residential address |
| PermanentAddress | nvarchar(500) | NULL | Permanent address |
| JoiningDate | datetime2 | NOT NULL | Employment start date |
| DepartmentId | int | NOT NULL, FK → Departments.Id | Department assignment |
| DesignationId | int | NOT NULL, FK → Designations.Id | Designation/title |
| EmployeeType | nvarchar(50) | NOT NULL, Default: 'Full-Time' | Full-Time, Part-Time, Contract |
| IsTeachingStaff | bit | NOT NULL | Whether employee is a teacher |
| Status | nvarchar(20) | NOT NULL, Default: 'Active' | Active, Inactive, On Leave, Resigned, Retired, Deleted |
| UserId | int | NULL, FK → Users.Id, UNIQUE FILTERED | Linked system user account |
| ProfilePicturePath | nvarchar(260) | NULL | Relative path to profile photo |
| SignaturePath | nvarchar(260) | NULL | Relative path to signature image |
| EmergencyContactName | nvarchar(120) | NULL | Emergency contact person |
| EmergencyContactPhone | nvarchar(30) | NULL | Emergency contact phone |
| Remarks | nvarchar(500) | NULL | General remarks |
| EmployeeCardNumber | nvarchar(50) | NULL, UNIQUE FILTERED | ID card number |
| CardIssueDate | datetime2 | NULL | ID card issue date |
| CardExpiryDate | datetime2 | NULL | ID card expiry date |
| CardPrintedAt | datetime2 | NULL | Timestamp of last card print |
| CardVersion | int | NOT NULL, Default: 1 | Card version counter |
| QRVerificationCode | nvarchar(100) | NULL, UNIQUE FILTERED | QR code verification token |
| CreatedBy | nvarchar(64) | NOT NULL, Default: 'system' | Creator username |
| CreatedAt | datetime2 | NOT NULL, Default: GETUTCDATE() | Creation timestamp |
| UpdatedBy | nvarchar(64) | NULL | Last updater username |
| UpdatedAt | datetime2 | NULL | Last update timestamp |
| IsDeleted | bit | NOT NULL, Default: 0 | Soft delete flag |

### Indexes

| Index | Type | Columns | Filter |
|---|---|---|---|
| PK_Employees | Clustered | Id | — |
| IX_Employees_EmployeeCode | Unique | EmployeeCode | — |
| IX_Employees_Phone | Unique | Phone | — |
| IX_Employees_Email | Unique | Email | — |
| IX_Employees_NIDNumber | Unique | NIDNumber | NIDNumber IS NOT NULL |
| IX_Employees_EmployeeCardNumber | Unique | EmployeeCardNumber | EmployeeCardNumber IS NOT NULL |
| IX_Employees_QRVerificationCode | Unique | QRVerificationCode | QRVerificationCode IS NOT NULL |
| IX_Employees_BirthCertificateNo | Unique | BirthCertificateNo | BirthCertificateNo IS NOT NULL |
| IX_Employees_PassportNo | Unique | PassportNo | PassportNo IS NOT NULL |
| IX_Employees_TIN | Unique | TIN | TIN IS NOT NULL |
| IX_Employees_DrivingLicenseNo | Unique | DrivingLicenseNo | DrivingLicenseNo IS NOT NULL |
| IX_Employees_Status | Non-unique | Status | IsDeleted = 0 |
| IX_Employees_DepartmentId | Non-unique | DepartmentId | IsDeleted = 0 |
| IX_Employees_DesignationId | Non-unique | DesignationId | IsDeleted = 0 |
| IX_Employees_UserId | Non-unique | UserId | UserId IS NOT NULL AND IsDeleted = 0 |
| IX_Employees_IsTeachingStaff | Non-unique | IsTeachingStaff | IsDeleted = 0 |
| IX_Employees_JoiningDate | Non-unique | JoiningDate | IsDeleted = 0 |

---

## Table: Departments

Organizational units/departments.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| Name | nvarchar(100) | NOT NULL | Department name (e.g., Science, English, Administration) |
| CreatedBy | nvarchar(64) | NOT NULL | — |
| CreatedAt | datetime2 | NOT NULL | — |
| UpdatedBy | nvarchar(64) | NULL | — |
| UpdatedAt | datetime2 | NULL | — |
| IsDeleted | bit | NOT NULL | — |

---

## Table: Designations

Job titles/roles within the institution.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| Name | nvarchar(100) | NOT NULL | Designation name (e.g., Senior Teacher, Principal, Accountant) |
| RoleLevel | int | NOT NULL, Default: 0 | Hierarchical level for reporting |
| IsTeachingRole | bit | NOT NULL, Default: 0 | Whether this designation is a teaching position |
| IsAdministrativeRole | bit | NOT NULL, Default: 0 | Whether this role is administrative |
| RequiresLogin | bit | NOT NULL, Default: 1 | Whether this role needs system access |
| IsActive | bit | NOT NULL, Default: 1 | Whether this designation is currently active |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: DesignationRoleMappings

Maps designations to application security roles for automatic RBAC assignment.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| DesignationId | int | NOT NULL, FK → Designations.Id | Designation to map |
| RoleId | int | NOT NULL, FK → Roles.Id | Application role to assign |
| IsActive | bit | NOT NULL, Default: 1 | Whether mapping is active |

**Unique Index:** `(DesignationId, RoleId)`

---

## Table: EmployeeQualifications

Academic qualifications attained by an employee.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Owning employee |
| ExamName | nvarchar(100) | NOT NULL | e.g., SSC, HSC, B.Sc., M.Sc. |
| BoardOrUniversity | nvarchar(150) | NULL | e.g., Dhaka Board, University of Dhaka |
| InstituteName | nvarchar(150) | NULL | Institution where studied |
| GroupOrSubject | nvarchar(100) | NULL | e.g., Science, Business Studies, Mathematics |
| PassingYear | nvarchar(10) | NULL | Year of passing |
| Result | nvarchar(50) | NULL | e.g., First Division, A+ |
| CGPAOrDivision | nvarchar(50) | NULL | e.g., 3.50, 4.00 |
| CertificateFilePath | nvarchar(260) | NULL | Uploaded certificate file path |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: EmployeeDocuments

Uploaded documents attached to an employee record.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Owning employee |
| DocumentType | nvarchar(50) | NOT NULL | e.g., CV, Appointment Letter, NID Copy |
| DocumentName | nvarchar(150) | NOT NULL | Display name for the document |
| FilePath | nvarchar(260) | NOT NULL | Relative file path |
| ExpiryDate | datetime2 | NULL | Document expiry (e.g., for contracts) |
| Remarks | nvarchar(255) | NULL | Additional notes |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: EmployeeExperiences

Prior work experience records.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Owning employee |
| OrganizationName | nvarchar(150) | NOT NULL | Previous employer |
| Designation | nvarchar(100) | NOT NULL | Job title held |
| StartDate | datetime2 | NOT NULL | Period start |
| EndDate | datetime2 | NULL | Period end (NULL = current) |
| Remarks | nvarchar(500) | NULL | Description/notes |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: EmployeeSalaries

Salary/pay structure records with historical tracking.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Owning employee |
| BasicSalary | decimal(18,2) | NOT NULL | Base pay |
| HouseRent | decimal(18,2) | NOT NULL | Housing allowance |
| MedicalAllowance | decimal(18,2) | NOT NULL | Medical allowance |
| TransportAllowance | decimal(18,2) | NOT NULL | Transport allowance |
| OtherAllowance | decimal(18,2) | NOT NULL | Other allowances |
| Deduction | decimal(18,2) | NOT NULL | Total deductions |
| TotalSalary | decimal(18,2) | NOT NULL | Computed: BasicSalary + HouseRent + MedicalAllowance + TransportAllowance + OtherAllowance - Deduction |
| EffectiveFrom | datetime2 | NOT NULL | Date this salary structure takes effect |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: EmployeeBankAccounts

Bank account information for salary disbursement.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Owning employee |
| BankName | nvarchar(100) | NOT NULL | Bank name |
| BranchName | nvarchar(50) | NOT NULL | Branch name |
| AccountNumber | nvarchar(50) | NOT NULL | Account number |
| RoutingNumber | nvarchar(50) | NULL | Bank routing number |
| AccountType | nvarchar(50) | NULL | Savings, Current, etc. |
| IsDefault | bit | NOT NULL | Default account for salary |
| IsActive | bit | NOT NULL, Default: 1 | Whether account is active |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: EmployeePromotions

Career progression history.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Owning employee |
| PreviousDesignationId | int | NOT NULL | Prior designation |
| NewDesignationId | int | NOT NULL | Promoted-to designation |
| Reason | nvarchar(200) | NULL | Reason for promotion |
| PromotionDate | datetime2 | NOT NULL | Effective date |
| PreviousSalary | decimal(18,2) | NULL | Salary before promotion |
| NewSalary | decimal(18,2) | NULL | Salary after promotion |
| ApprovedByUserId | int | NULL | Approver user ID |
| ApprovedAt | datetime2 | NULL | Approval timestamp |
| Remarks | nvarchar(500) | NULL | Additional notes |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: EmployeeTransfers

Inter-department transfer history.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Owning employee |
| FromDepartmentId | int | NOT NULL | Source department |
| ToDepartmentId | int | NOT NULL | Destination department |
| Reason | nvarchar(200) | NULL | Reason for transfer |
| TransferDate | datetime2 | NOT NULL | Effective date |
| ApprovedByUserId | int | NULL | Approver user ID |
| ApprovedAt | datetime2 | NULL | Approval timestamp |
| Remarks | nvarchar(500) | NULL | Additional notes |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: EmployeeTrainings

Professional development and training records.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Owning employee |
| TrainingName | nvarchar(200) | NOT NULL | Name/course title |
| InstitutionName | nvarchar(200) | NULL | Training provider |
| Duration | nvarchar(100) | NULL | Duration description (e.g., "3 days", "6 months") |
| StartDate | datetime2 | NULL | Training start |
| EndDate | datetime2 | NULL | Training end |
| CertificatePath | nvarchar(50) | NULL | Certificate file path |
| Remarks | nvarchar(500) | NULL | Additional notes |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: EmployeeAwards

Awards and recognition records.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Owning employee |
| AwardName | nvarchar(200) | NOT NULL | Name of award |
| AwardedBy | nvarchar(200) | NULL | Issuing authority |
| AwardDate | datetime2 | NOT NULL | Date awarded |
| Description | nvarchar(500) | NULL | Details about the award |
| CertificatePath | nvarchar(50) | NULL | Certificate file path |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: EmployeeDisciplinaryActions

Disciplinary action records.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Owning employee |
| ActionType | nvarchar(200) | NOT NULL | e.g., Warning, Suspension, Termination |
| Reason | nvarchar(500) | NULL | Reason for action |
| ActionDate | datetime2 | NOT NULL | Date of action |
| Description | nvarchar(500) | NULL | Detailed description |
| ApprovedByUserId | int | NULL | Approver user ID |
| ApprovedAt | datetime2 | NULL | Approval timestamp |
| DocumentPath | nvarchar(50) | NULL | Supporting document |
| IsResolved | bit | NOT NULL | Whether action is resolved |
| ResolvedAt | datetime2 | NULL | Resolution timestamp |
| ResolutionRemarks | nvarchar(500) | NULL | Resolution notes |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: EmployeeAcademicAssignments

Teaching assignments linking employees to classes, sections, and subjects.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Owning employee |
| ClassId | int | NOT NULL | Assigned class |
| SectionId | int | NOT NULL | Assigned section |
| SubjectId | int | NOT NULL | Assigned subject |
| AcademicYearId | int | NOT NULL | Academic year |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

---

## Table: EmployeeInvitations

Onboarding invitation records. This is a standalone entity (not a child of Employee) that serves as a pre-employment registration mechanism.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK, Identity | Primary key |
| FullName | nvarchar(120) | NOT NULL | Invitee full name |
| InvitationCode | nvarchar(30) | NOT NULL, UNIQUE | Auto-generated (`INV-{YYYY}-{NNNN}`) |
| Email | nvarchar(160) | NOT NULL | Invitee email (unique across active invites) |
| Mobile | nvarchar(30) | NOT NULL | Invitee phone number |
| InvitationToken | nvarchar(100) | NOT NULL, UNIQUE | Secure token for onboarding link |
| DepartmentId | int | NOT NULL, FK → Departments.Id | Target department |
| DesignationId | int | NOT NULL, FK → Designations.Id | Target designation |
| JoiningDate | datetime2 | NOT NULL | Expected joining date |
| EmploymentType | nvarchar(50) | NOT NULL, Default: 'Full-Time' | Employment type |
| Status | nvarchar(20) | NOT NULL, Default: 'Active' | Employment status |
| IsTeachingStaff | bit | NOT NULL | Whether invitee is teaching staff |
| Remarks | nvarchar(500) | NULL | Internal remarks |
| ExpiresAt | datetime2 | NOT NULL | Invitation expiry (72 hours from creation) |
| SentAt | datetime2 | NULL | When email was sent |
| OpenedAt | datetime2 | NULL | When invitee first opened link |
| CompletedAt | datetime2 | NULL | When onboarding was completed |
| OnboardedAt | datetime2 | NULL | Alternative completion timestamp |
| CreatedEmployeeId | int | NULL | Employee record created from this invitation |
| InvitationStatus | nvarchar(50) | NOT NULL, Default: 'Started' | Started, Sent, Opened, Completed, Approved, Expired, Cancelled |
| IsUsed | bit | NOT NULL | Whether invitation has been used |
| IsApproved | bit | NOT NULL | Whether invitation has been approved |
| BaseEntity fields | — | — | CreatedBy, CreatedAt, UpdatedBy, UpdatedAt, IsDeleted |

**Indexes:**

| Index | Type | Columns | Filter |
|---|---|---|---|
| IX_EmployeeInvitations_InvitationCode | Unique | InvitationCode | — |
| IX_EmployeeInvitations_InvitationToken | Unique | InvitationToken | — |
| IX_EmployeeInvitations_Email | Non-unique | Email | IsDeleted = 0 |

---

## Table: EmployeeAttendance (cross-module)

Referenced from the Attendance module. See Attendance module documentation for full schema.

| Column | Type | Constraints | Description |
|---|---|---|---|
| Id | int | PK | Primary key |
| EmployeeId | int | NOT NULL, FK → Employees.Id | Employee |
| AttendanceDate | datetime2 | NOT NULL | Date |
| Status | int | NOT NULL | 0=Present, 1=Absent, 2=Leave, 3=Late |
| ... | — | — | (additional Attendance fields) |

**Unique Index:** `(EmployeeId, AttendanceDate)`

---

## Foreign Key Relationships

| FK | Child Table | Parent Table | Column(s) |
|---|---|---|---|
| FK_Employee_Department | Employees | Departments | DepartmentId |
| FK_Employee_Designation | Employees | Designations | DesignationId |
| FK_Employee_User | Employees | Users | UserId |
| FK_Qualification_Employee | EmployeeQualifications | Employees | EmployeeId |
| FK_Document_Employee | EmployeeDocuments | Employees | EmployeeId |
| FK_Experience_Employee | EmployeeExperiences | Employees | EmployeeId |
| FK_Salary_Employee | EmployeeSalaries | Employees | EmployeeId |
| FK_BankAccount_Employee | EmployeeBankAccounts | Employees | EmployeeId |
| FK_Promotion_Employee | EmployeePromotions | Employees | EmployeeId |
| FK_Transfer_Employee | EmployeeTransfers | Employees | EmployeeId |
| FK_Training_Employee | EmployeeTrainings | Employees | EmployeeId |
| FK_Award_Employee | EmployeeAwards | Employees | EmployeeId |
| FK_Disciplinary_Employee | EmployeeDisciplinaryActions | Employees | EmployeeId |
| FK_AcademicAssignment_Employee | EmployeeAcademicAssignments | Employees | EmployeeId |
| FK_DesignationRole_Designation | DesignationRoleMappings | Designations | DesignationId |
| FK_DesignationRole_Role | DesignationRoleMappings | Roles | RoleId |
| FK_Invitation_Department | EmployeeInvitations | Departments | DepartmentId |
| FK_Invitation_Designation | EmployeeInvitations | Designations | DesignationId |
| FK_Attendance_Employee | EmployeeAttendance | Employees | EmployeeId |

All foreign keys use `DeleteBehavior.Restrict` (no cascade deletes).

## Stored Procedures

Located in `Data/StoredProcedures/Employee/`:

| Procedure | Parameters | Purpose |
|---|---|---|
| `sp_GetEmployeeDashboard` | (none) | Returns 4 result sets: aggregate counts, department distribution, status distribution, birthdays this month, recent hires |
| `sp_GetEmployeesPaged` | @PageNumber, @PageSize, @SearchTerm, @DepartmentId, @DesignationId, @IsTeachingStaff, @Status | Paginated employee list with search/filter for Tabulator grid |
| `sp_GetEmployeeDetails` | @EmployeeId | Full employee profile across 9 result sets: main record, qualifications, documents, experience, bank accounts, promotions, transfers, training, awards, disciplinary actions |
| `sp_GetEmployeeInvitationList` | @PageNumber, @PageSize, @SearchTerm | Paginated invitation list with department/designation names |
| `sp_GetAttendanceSummary` | @EmployeeId, @Year | Attendance summary: totals and monthly breakdown |
| `sp_GetLeaveSummary` | @EmployeeId, @Year | Leave summary: totals and breakdown by leave type |
| `sp_GetPayrollSummary` | @EmployeeId | Salary history list and aggregate stats (total records, current/max/avg/min salary) |

The `sp_GetEmployeeInvitationList` is actively called from `EmployeeInvitationRepository.GetPagedBySpAsync()`. The other procedures are available for reporting but the service layer also uses LINQ-based queries.
