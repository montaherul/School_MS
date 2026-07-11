# Employee Module — Permissions

## Permission Code Reference

The following permission codes are checked via `[RequirePermission]` attribute or manual `User.HasClaim()` calls.

| Permission Code | Controller Action | Description |
|---|---|---|
| `Employees.View` | `EmployeeController.Dashboard` | View employee dashboard |
| `Employees.View` | `EmployeeController.Index` | View employee list (Tabulator grid) |
| `Employees.View` | `EmployeeController.Details` | View employee details/profile |
| `Employees.View` | `EmployeeController.DownloadServiceBookPdf` | Download service book PDF |
| `Employees.View` | `EmployeeController.VerifyCode` | Verify unique employee code |
| `Employees.View` | `EmployeeController.VerifyEmail` | Verify unique email |
| `Employees.View` | `EmployeeController.VerifyPhone` | Verify unique phone |
| `Employees.View` | `EmployeeController.Verify` | Public ID card verification |
| `Employees.Create` | `EmployeeController.Save` (Id=0) | Create new employee record |
| `Employees.Edit` | `EmployeeController.Save` (Id>0) | Edit existing employee |
| `Employees.Edit` | `EmployeeController.UpdateStatus` | Change employee employment status |
| `Employees.Delete` | `EmployeeController.Delete` | Soft-delete employee |
| `Employees.Invite` | `EmployeeInvitationController.Index` | View invitation list |
| `Employees.Invite` | `EmployeeInvitationController.Create` (GET) | Show invite form |
| `Employees.Invite` | `EmployeeInvitationController.Create` (POST) | Create and send invitation |
| `Employees.Invite` | `EmployeeInvitationController.Resend` | Resend invitation email |
| `Employees.Invite` | `EmployeeInvitationController.Cancel` | Cancel pending invitation |
| `Employee.BankAccount.View` | `EmployeeHRController.BankAccounts` | View bank accounts |
| `Employee.BankAccount.Edit` | `EmployeeHRController.SaveBankAccount` | Create/update bank account |
| `Employee.BankAccount.Delete` | `EmployeeHRController.DeleteBankAccount` | Delete bank account |
| `Employee.Promotion.View` | `EmployeeHRController.Promotions` | View promotion history |
| `Employee.Promotion.Edit` | `EmployeeHRController.SavePromotion` | Create/update promotion |
| `Employee.Promotion.Delete` | `EmployeeHRController.DeletePromotion` | Delete promotion record |
| `Employee.Transfer.View` | `EmployeeHRController.Transfers` | View transfer history |
| `Employee.Transfer.Edit` | `EmployeeHRController.SaveTransfer` | Create/update transfer |
| `Employee.Transfer.Delete` | `EmployeeHRController.DeleteTransfer` | Delete transfer record |
| `Employee.Training.View` | `EmployeeHRController.Training` | View training records |
| `Employee.Training.Edit` | `EmployeeHRController.SaveTraining` | Create/update training |
| `Employee.Training.Delete` | `EmployeeHRController.DeleteTraining` | Delete training record |
| `Employee.Award.View` | `EmployeeHRController.Awards` | View awards |
| `Employee.Award.Edit` | `EmployeeHRController.SaveAward` | Create/update award |
| `Employee.Award.Delete` | `EmployeeHRController.DeleteAward` | Delete award record |
| `Employee.Disciplinary.View` | `EmployeeHRController.Disciplinary` | View disciplinary actions |
| `Employee.Disciplinary.Edit` | `EmployeeHRController.SaveDisciplinary` | Create/update disciplinary action |
| `Employee.Disciplinary.Edit` | `EmployeeHRController.ResolveDisciplinary` | Resolve disciplinary action |
| `Employee.Disciplinary.Delete` | `EmployeeHRController.DeleteDisciplinary` | Delete disciplinary record |
| `Employee.Salary.View` | `EmployeePayrollController.Index` | View salary history |
| `Employee.Salary.Create` | `EmployeePayrollController.Create` (GET/POST) | Create salary record |
| `Employee.Salary.Edit` | `EmployeePayrollController.Edit` (GET/POST) | Edit salary record |
| `Employee.Salary.Delete` | `EmployeePayrollController.Delete` | Delete salary record |

## Self-Service / Own Profile Access

The `EmployeeController.Details` action has a special security rule:

```
IF id is null or 0 → resolve to current user's employee profile
IF resolved id == current user's employee id → "own profile" mode
    → bypasses Employees.View permission check
    → user can view their own profile
ELSE → requires "Employees.View" or Super Admin role
```

This means any authenticated user can view their own employee profile without the `Employees.View` permission.

## Anonymous Access

The following endpoints have no authorization:

| Controller | Action | Purpose |
|---|---|---|
| `OnboardingController` | `Welcome` | Welcome page for invitees (token-based) |
| `OnboardingController` | `Start` | Onboarding form (token-based) |
| `OnboardingController` | `Submit` | Submit onboarding data (token-based, antiforgery protected) |
| `VerifyController` | `Verify` | Public ID card verification |

These endpoints are `[AllowAnonymous]` and rely on secure tokens rather than authentication.

## Runtime Permission Check

The `EmployeeController.Save` action performs a runtime permission check because the same action handles both Create and Edit:

```csharp
var requiredPerm = dto.Id == 0 ? "Employees.Create" : "Employees.Edit";
if (!User.HasClaim("Permission", requiredPerm) && !User.IsInRole("Super Admin"))
    return Forbid();
```

## Role-to-Permission Mapping via DesignationRoleMapping

Permissions are not directly assigned to users. Instead, the system uses a two-level mapping:

```
Designation (job title)
    │
    └──→ DesignationRoleMapping (many)
            │
            └──→ Role (application security role)
                    │
                    └──→ RoleClaims (permissions)
```

When an employee is created (or onboarding is completed):

1. The employee's `DesignationId` is used to query `DesignationRoleMapping`
2. Each active mapping yields a `RoleId`
3. `UserRole` records are created linking the employee's `ApplicationUser` to each role
4. The role's claims (permissions) are inherited by the user

This is the standard RBAC pattern used throughout the system. The `Designation` entity has flags that guide this:

| Flag | Purpose |
|---|---|
| `IsTeachingRole` | Used by teacher sync to determine auto-provisioning |
| `IsAdministrativeRole` | Identifies admin-level roles |
| `RequiresLogin` | If false, user account may not be created |
| `IsActive` | If false, no role mappings are applied |

## Audit Log Action Codes

Every HR operation logs to the `AuditLog` table with these action codes:

| Module | Action | Logged By |
|---|---|---|
| Employee | Create | EmployeeService |
| Employee | Update | EmployeeService |
| Employee | Delete | EmployeeService |
| Employee | StatusChange | EmployeeService |
| Employee.BankAccount | Create / Update / Delete | EmployeeHrService |
| Employee.Promotion | Create / Update / Delete | EmployeeHrService |
| Employee.Transfer | Create / Update / Delete | EmployeeHrService |
| Employee.Training | Create / Update / Delete | EmployeeHrService |
| Employee.Award | Create / Update / Delete | EmployeeHrService |
| Employee.Disciplinary | Create / Update / Delete / Resolve | EmployeeHrService |
| Employee.Salary | Create / Update / Delete | EmployeePayrollService |
| Employee.Invitation | Resend / Cancel / CompleteOnboarding / Approve | EmployeeInvitationService |

## Audit Log Format

Each audit log entry contains:

- `UserId` — the authenticated user performing the action
- `Module` — "Employee" or "Employee.{SubModule}"
- `Action` — the action code (Create, Update, Delete, StatusChange, Resolve, etc.)
- `EntityId` — the Employee ID (string)
- `Details` — human-readable description (truncated to 1000 chars)
- `IpAddress` — client IP address
- `CreatedAt` — UTC timestamp
- `CreatedBy` — username of the actor
