# EXAMINATION, MARKS & RESULT — ROLE-BASED ACCESS AUDIT
## Phase 15: Final Security Report

**Date:** 2026-06-12
**Project:** SchoolMS (.NET 8 MVC, Clean Architecture)
**Build:** 0 Errors
**Scope:** 14 controllers, 3 portal sets, 5 service interfaces, 45 permission strings

---

## EXECUTIVE SUMMARY

A comprehensive authorization audit of the Examination, Marks, and Result subsystem was conducted across 15 phases. The audit examined **14 controllers**, **3 portal controllers (Guardian/Student/Teacher)** , **5 authorization service interfaces**, and **45 unique permission strings** for role-based access control completeness, consistency, and data leakage risks.

**Findings:** **3 CRITICAL**, **8 HIGH**, **6 MEDIUM**, **5 LOW** security issues identified. The most severe finding is that `GenericCrudController<T>` grants any authenticated user full CRUD access to **Roles, Permissions, and System Settings** via inherited controllers with no additional checks. Additionally, the `ExamAdminController` has class-level CSRF protection disabled, and several controllers use inconsistent authorization patterns that create bypass opportunities.

---

## FINDINGS

---

### 🔴 CRITICAL (3)

#### C1. GenericCrudController<T> — Inherited Authorization Bypass

| Attribute | Value |
|---|---|
| **Risk** | Any authenticated user can create/update/delete roles, permissions, and system settings |
| **Files** | `Controllers/Common/GenericCrudController.cs:14`, `Controllers/Admin/RoleController.cs:10`, `Controllers/Admin/PermissionController.cs:7`, `Controllers/Admin/SystemSettingsController.cs:7` |
| **Description** | `GenericCrudController<T>` has `[Authorize]` at class level (no role/permission filter). `RoleController`, `PermissionController`, and `SystemSettingsController` inherit from it without adding any `[RequirePermission]` or `[Authorize(Roles = "...")]`. A standard authenticated user (e.g., a Student, Teacher, or Guardian) can access all CRUD endpoints — including assigning permissions, creating roles, and modifying school-wide settings. |
| **Evidence** | `RoleController.cs:10` — `public class RoleController : GenericCrudController<Role>` (no additional auth attributes). `PermissionController.cs:7` — same pattern. `SystemSettingsController.cs:7` — same pattern. |
| **Remediation** | Add `[Authorize(Roles = "Admin,Super Admin")]` to `RoleController` and `PermissionController`. Add `[Authorize(Roles = "Admin,Super Admin,Principal")]` to `SystemSettingsController`. Alternatively, add `[RequirePermission("Roles.*")]` style permissions. |

#### C2. ExamAdminController — Class-level [IgnoreAntiforgeryToken]

| Attribute | Value |
|---|---|
| **Risk** | All POST/PUT/DELETE endpoints in the controller are vulnerable to CSRF attacks |
| **File** | `Controllers/Result/ExamAdminController.cs:18` |
| **Description** | The entire controller is decorated with `[IgnoreAntiforgeryToken]` at class level. This disables CSRF protection for all 15+ POST endpoints. While `[ApiController]` provides some protection (cannot be called from plain HTML forms without proper content-type), this remains a significant risk, especially if any endpoint accepts `application/x-www-form-urlencoded` data. |
| **Remediation** | Remove class-level `[IgnoreAntiforgeryToken]`. Apply only to individual endpoints that genuinely require it (e.g., webhook callbacks), and add `[AutoValidateAntiforgeryToken]` at class level instead. |

#### C3. PermissionController — No Authorization on Permission CRUD

| Attribute | Value |
|---|---|
| **Risk** | Any authenticated user can view, create, edit, or delete permission definitions |
| **File** | `Controllers/Admin/PermissionController.cs:7` |
| **Description** | `PermissionController` inherits from `GenericCrudController<Permission>` with no additional authorization. Permissions are the lowest-level security primitive — compromising them allows an attacker to grant themselves any permission. |
| **Remediation** | Restrict to `[Authorize(Roles = "Admin,Super Admin")]` — only users who can assign permissions should be able to manage the permission catalog itself. |

---

### 🟠 HIGH (8)

#### H1. Inconsistent Role Lists on MarksController Actions

| Attribute | Value |
|---|---|
| **Risk** | Authorization logic is duplicated and may drift; hard to audit |
| **File** | `Controllers/Result/MarksController.cs:47,79,325,387,403,419,429` |
| **Description** | `MarksController` uses `[Authorize(Roles = "...")]` on individual actions with long role strings (7+ roles listed inline, repeated 8+ times). This creates a maintenance burden — if a new teacher role is added, it must be updated in 8+ places. Also, the role string for `Index`/`Entry`/`SaveMarks` actions includes `"Teacher,Senior Lecturer,Lecturer,Admin,Super Admin,Principal"` but does NOT include `"Exam Controller"` — yet `ExamAdminController` allows `Exam Controller` to manage exam results via other paths. |
| **Remediation** | Introduce a `[RequirePermission("Marks.Entry")]` permission attribute and apply it uniformly. Remove inline role strings. |

#### H2. MarksController — 5 Actions with [IgnoreAntiforgeryToken]

| Attribute | Value |
|---|---|
| **Risk** | CSRF attacks on mark entry/submission/approval operations |
| **File** | `Controllers/Result/MarksController.cs:153,194,237,388,404` |
| **Description** | Five POST/PUT actions (`SaveMarks`, `UpdateMarks`, `SubmitMarks`, `BulkUpdateMarks`, `ApproveAllMarks`) have `[IgnoreAntiforgeryToken]`. If these accept form-encoded data, they are directly exploitable. |
| **Remediation** | Remove `[IgnoreAntiforgeryToken]` and include antiforgery tokens in AJAX requests (via ` antiforgery.getToken()` or `[ValidateAntiForgeryToken]` on server). |

#### H3. AdminResultController — 2 Actions with [IgnoreAntiforgeryToken]

| Attribute | Value |
|---|---|
| **Risk** | CSRF on result recalculation and bulk approval |
| **File** | `Controllers/Result/AdminResultController.cs:371,388` |
| **Description** | `RecalculateResults()` and `BulkApproveResults()` have `[IgnoreAntiforgeryToken]`. These are destructive operations (recalculates all results, approves in bulk). |
| **Remediation** | Same as H2 — add antiforgery validation. |

#### H4. ResultManagementController.ProcessReEvaluation — [IgnoreAntiforgeryToken]

| Attribute | Value |
|---|---|
| **Risk** | CSRF on re-evaluation processing |
| **File** | `Controllers/Result/ResultManagementController.cs:270` |
| **Description** | `ProcessReEvaluation()` is marked `[IgnoreAntiforgeryToken]`. Re-evaluation is a financially sensitive operation. |
| **Remediation** | Add antiforgery validation. |

#### H5. ResultManagementController.GetSubjectsForClass — Unrestricted [Authorize]

| Attribute | Value |
|---|---|
| **Risk** | Any authenticated user can query subject lists for any class |
| **File** | `Controllers/Result/ResultManagementController.cs:213` |
| **Description** | `GetSubjectsForClass()` has only `[Authorize]` with no role or permission filter. Any authenticated user (including Students, Guardians, Teachers with limited scope) can enumerate subjects for any class, potentially leaking curriculum structure data. |
| **Remediation** | Add `[RequirePermission("Exam.View")]` or scope the query to the user's accessible classes. |

#### H6. Guardian Controller Permission Mismatch

| Attribute | Value |
|---|---|
| **Risk** | Guardian management uses Student permissions instead of dedicated Guardian permissions |
| **File** | `Controllers/Admin/GuardianController.cs:21,40,49,56,69,96,111,120,129` |
| **Description** | `GuardianController` uses `[Permission("Students", "View/Create/Update/Delete")]` permissions. Guardian management should be gated by `Guardians.*` permissions. This means a user with `Students.Create` can create guardian records, and a user without any guardian-specific restriction cannot be separately denied. |
| **Remediation** | Introduce `Guardians.View`, `Guardians.Create`, `Guardians.Update`, `Guardians.Delete` permissions and update the controller accordingly. |

#### H7. ExamScheduleController — No Permission Checks on Actions

| Attribute | Value |
|---|---|
| **Risk** | Role alone gates access; no fine-grained permission check at action level |
| **File** | `Controllers/Exam/ExamScheduleController.cs:11` |
| **Description** | The controller has class-level `[Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]` but no `[RequirePermission]` or `[Permission]` on any individual action. This means any user in those roles can access ALL schedule actions regardless of specific permission assignments. |
| **Remediation** | Add `[RequirePermission("Exam.Schedule")]` or `[Permission("Exam", "Schedule")]` to each action. |

#### H8. StudentClassAssignmentController — Student Role Included

| Attribute | Value |
|---|---|
| **Risk** | Students may be able to modify their own class assignments |
| **File** | `Controllers/Student/StudentClassAssignmentController.cs:10` |
| **Description** | Class-level `[Authorize(Roles = "Super Admin,Principal,Assistant Head,Senior Lecturer,Lecturer,Student")]` includes the `Student` role. While `ApplySecurityFiltersAsync` may restrict operations, the presence of `Student` in the role list is unusual for a class assignment controller and may indicate a copy-paste error. |
| **Remediation** | Remove `Student` from the role list unless there is a specific self-service feature. Add an inline check if student self-assignment is intentional. |

---

### 🟡 MEDIUM (6)

#### M1. Dual Authorization Pattern in StudentController

| Attribute | Value |
|---|---|
| **Risk** | Conflicting/confusing authorization — inline checks may override attribute checks |
| **File** | `Controllers/Student/StudentController.cs` |
| **Description** | `StudentController` uses BOTH `[RequirePermission("Student.*")]` attributes on actions AND inline `User.HasClaim("Permission", "...")` checks inside action bodies. These are redundant and may diverge. For example, `Index()` has `[RequirePermission("Student.View")]` but also has an inline claim check at line 136 that allows `Student` role users to self-audit even without the `Student.View` permission. |
| **Remediation** | Consolidate to attribute-only authorization. Remove inline `HasClaim` checks. If student self-service is needed, create a separate `StudentSelfServiceController` with appropriate role-based auth. |

#### M2. Inconsistent Permission Naming — "Exam.Update" vs "Exam.Edit"

| Attribute | Value |
|---|---|
| **Risk** | Permission checks may fail silently or allow unintended access |
| **Files** | `Controllers/Exam/ExamSubjectController.cs:25,41,56,73,88,108` (uses `Exam.Update`), `Controllers/Exam/SubjectMarkStructureController.cs` (uses `Exam.Edit`), `Controllers/Exam/ExamComponentsController.cs` (uses `Exam.Edit`) |
| **Description** | Two semantically equivalent permission strings are used: `Exam.Update` (in `ExamSubjectController`) and `Exam.Edit` (in `SubjectMarkStructureController`/`ExamComponentsController`). If the seed creates only `Exam.Edit` but not `Exam.Update`, the `ExamSubjectController` checks will always fail — or vice versa. |
| **Remediation** | Standardize on one naming convention. Recommend `Exam.Edit` to match other module patterns (e.g., `Student.Edit`, `Teachers.Edit`). |

#### M3. MarksController Uses Role Strings Instead of Permission Attributes

| Attribute | Value |
|---|---|
| **Risk** | Bypasses the flexible RolePermission table mechanism |
| **File** | `Controllers/Result/MarksController.cs:47,79,152-153,193-194,236-237,278,294,325,387-388,403-404,419,429` |
| **Description** | Every action in `MarksController` uses `[Authorize(Roles = "...")]` with hardcoded role strings. This means adding a new role (e.g., "Assistant Teacher") requires modifying source code and recompiling. The `RequirePermissionAttribute` is designed to avoid this but is not used here. |
| **Remediation** | Replace role strings with `[RequirePermission("Marks.Entry")]`, `[RequirePermission("Marks.View")]`, `[RequirePermission("Marks.Approve")]`, etc. |

#### M4. ReportCardController Allows Student/Guardian Direct Access Without Ownership Check

| Attribute | Value |
|---|---|
| **Risk** | Students may access other students' report cards |
| **Files** | `Controllers/Result/ReportCardController.cs:91,138,143` |
| **Description** | `View()`, `Download()`, and `DownloadMultiple()` actions allow `Student` and `Guardian` roles but there is no explicit student ownership check for GET requests. The `ITeacherScopeService.HasStudentAccessAsync()` is used only for teacher-scoped queries. A student who modifies the studentId query parameter could access another student's report card. |
| **Remediation** | Add a student ownership check: verify the requesting user's `StudentId` matches the requested `studentId` before serving the report card for `Student` role users. |

#### M5. TranscriptController.MyTranscript — Unrestricted [Authorize]

| Attribute | Value |
|---|---|
| **Risk** | Any authenticated user can generate their transcript, but role is ambiguous |
| **File** | `Controllers/Result/TranscriptController.cs:43` |
| **Description** | `MyTranscript()` has only `[Authorize]` with no role or permission filter. The method name suggests it should serve the current user's transcript, but there is no guard to ensure only `Student` role users can access it. A Teacher or Admin calling this endpoint may get unexpected behavior or data leakage. |
| **Remediation** | Add `[Authorize(Roles = "Student")]` if this is a student-only feature, or add explicit student identity resolution. |

#### M6. AuditLogController — Permission Named "AuditLogs.View" But Seed Creates "AuditLogs.View"

| Attribute | Value |
|---|---|
| **Risk** | Low — naming is consistent here, but the audit module is a security-critical component |
| **File** | `Controllers/Admin/AuditLogController.cs:19` |
| **Description** | `AuditLogs.View` permission is checked. While the naming is correct, audit logs contain sensitive data about who did what in the system. The current authorization only checks for the permission — there is no additional scoping (e.g., Principal can see school-level logs, but Teacher can only see their own department). |
| **Remediation** | Consider data-level scoping for audit log queries based on the user's role. |

---

### 🔵 LOW (5)

#### L1. Unused Permission Strings in Seed Data

| Attribute | Value |
|---|---|
| **Risk** | Permission strings are created but never enforced |
| **Files** | `Data/DbInitializer.cs` |
| **Description** | Many permission strings generated in `DbInitializer` (e.g., `Dashboard.View`, `Admissions.View`, `Library.Issue`, `Laboratory.Manage`) are never referenced in any controller `[RequirePermission]` attribute. This creates a false sense of security — the permissions exist in the database but are never checked. |
| **Remediation** | Audit all permission strings and either implement checks or remove unused ones. |

#### L2. AuthorizeRoleAttribute Defined But Never Used

| Attribute | Value |
|---|---|
| **Risk** | Dead code that may confuse future developers |
| **File** | `Filters/AuthorizeRoleAttribute.cs` |
| **Description** | A custom `[AuthorizeRole(params string[] roles)]` attribute is defined but no controller uses it. The built-in `[Authorize(Roles = "...")]` is used everywhere instead. |
| **Remediation** | Either remove the unused class or migrate to use it if it provides value over `[Authorize]`. |

#### L3. ModulesController Authorization Uses Roles Instead of Permissions

| Attribute | Value |
|---|---|
| **Risk** | Inconsistent with the permission-based system used elsewhere |
| **File** | `Controllers/Common/ModulesController.cs:26` |
| **Description** | `ModulesController` uses `[Authorize(Roles = "Admin,Super Admin,Principal")]`. Module visibility could be more granularly controlled via permissions. |
| **Remediation** | Consider using `[RequirePermission("Modules.View")]` instead of role strings. |

#### L4. HomeController and ErrorController — Public Access

| Attribute | Value |
|---|---|
| **Risk** | Information disclosure through error pages |
| **File** | `Controllers/Common/HomeController.cs:24`, `Controllers/Common/ErrorController.cs:8` |
| **Description** | Both controllers have no authorization (intentionally public). However, `ErrorController` should ensure it does not leak stack traces or internal paths in production. |
| **Remediation** | Verify `appsettings.Production.json` has `ASPNETCORE_ENVIRONMENT=Production` to suppress detailed errors. |

#### L5. ExamController Inherits GenericCrudController — Minimal Additional Checks

| Attribute | Value |
|---|---|
| **Risk** | `GenericCrudController<T>` provides full CRUD to any authenticated user; saved here by class-level role restriction |
| **File** | `Controllers/Exam/ExamController.cs:20-21` |
| **Description** | `ExamController` adds `[Authorize(Roles = "Admin,Super Admin,Principal,Exam Controller")]` at class level, which prevents the `GenericCrudController` bypass. However, the actions still lack `[RequirePermission]` granularity — any user in those 4 roles can do all CRUD operations. |
| **Remediation** | Add `[RequirePermission("Exam.Create")]`, `[RequirePermission("Exam.Edit")]`, `[RequirePermission("Exam.Delete")]` to individual actions. |

---

## COMPREHENSIVE FINDINGS TABLE

| ID | Severity | Controller/File | Issue | Line(s) |
|---|---|---|---|---|
| C1 | 🔴 CRITICAL | `GenericCrudController<T>` + 3 inheritors | Any authenticated user can CRUD Roles/Permissions/Settings | 14, 10, 7, 7 |
| C2 | 🔴 CRITICAL | `ExamAdminController` | Class-level `[IgnoreAntiforgeryToken]` | 18 |
| C3 | 🔴 CRITICAL | `PermissionController` | No authorization on permission CRUD | 7 |
| H1 | 🟠 HIGH | `MarksController` | Inconsistent duplicate role strings on 8+ actions | 47–429 |
| H2 | 🟠 HIGH | `MarksController` | 5 actions with `[IgnoreAntiforgeryToken]` | 153, 194, 237, 388, 404 |
| H3 | 🟠 HIGH | `AdminResultController` | 2 actions with `[IgnoreAntiforgeryToken]` | 371, 388 |
| H4 | 🟠 HIGH | `ResultManagementController` | `ProcessReEvaluation` with `[IgnoreAntiforgeryToken]` | 270 |
| H5 | 🟠 HIGH | `ResultManagementController` | `GetSubjectsForClass` — unrestricted `[Authorize]` | 213 |
| H6 | 🟠 HIGH | `GuardianController` | Uses `Students.*` permissions instead of `Guardians.*` | 21–129 |
| H7 | 🟠 HIGH | `ExamScheduleController` | Role-level only; no per-action permission checks | 11 |
| H8 | 🟠 HIGH | `StudentClassAssignmentController` | `Student` role included in assignment controller | 10 |
| M1 | 🟡 MEDIUM | `StudentController` | Dual attribute + inline claim checks | multiple |
| M2 | 🟡 MEDIUM | `ExamSubjectController` / others | `Exam.Update` vs `Exam.Edit` inconsistency | multiple |
| M3 | 🟡 MEDIUM | `MarksController` | Role strings instead of permission attributes | multiple |
| M4 | 🟡 MEDIUM | `ReportCardController` | Student/Guardian access without ownership check | 91, 138, 143 |
| M5 | 🟡 MEDIUM | `TranscriptController` | `MyTranscript` — unrestricted `[Authorize]` | 43 |
| M6 | 🟡 MEDIUM | `AuditLogController` | No data-level scoping for audit logs | 19 |
| L1 | 🔵 LOW | `DbInitializer` | Unused permission strings in seed data | multiple |
| L2 | 🔵 LOW | `AuthorizeRoleAttribute` | Defined but never used | entire file |
| L3 | 🔵 LOW | `ModulesController` | Roles instead of permissions | 26 |
| L4 | 🔵 LOW | `HomeController` / `ErrorController` | Public access (informational) | 24, 8 |
| L5 | 🔵 LOW | `ExamController` | Missing per-action permission granularity | 20 |

---

## STRONGEST ACCESS CONTROL PATTERNS (Reference Models)

These existing patterns should be replicated across the codebase during remediation:

| Pattern | Location | Description |
|---|---|---|
| **Role-gated class + per-student ownership check** | `GuardianPortalPagesController` | `[Authorize(Roles = "Guardian")]` at class level + `_guardianService.UserHasAccessToStudentAsync()` on every action |
| **RequirePermission on individual actions** | `ExamSubjectController` | `[RequirePermission("Exam.Update")]` on each action — fine-grained, DB-driven |
| **ITeacherScopeService for data scoping** | `TranscriptController`, `ReportCardController` | `_teacherScopeService.HasStudentAccessAsync()` limits teachers to their assigned students |
| **Super Admin bypass in RequirePermissionAttribute** | `Filters/RequirePermissionAttribute.cs:26-28` | Super Admin role auto-bypasses all permission checks — correct design |

---

## PRIORITIZED REMEDIATION ORDER

### Immediate (Blockers — Fix before any other work)

| Order | ID | Action |
|---|---|---|
| 1 | C1 | Add `[Authorize(Roles = "Admin,Super Admin")]` to `RoleController`, `PermissionController`, and `SystemSettingsController` |
| 2 | C3 | Same as C1 (PermissionController) — also needs class-level role restriction |

### Phase 1 — CSRF & Critical Auth Gaps

| Order | ID | Action |
|---|---|---|
| 3 | C2 | Remove class-level `[IgnoreAntiforgeryToken]` from `ExamAdminController`; add per-action only if necessary |
| 4 | H2 | Remove `[IgnoreAntiforgeryToken]` from 5 `MarksController` actions; add antiforgery tokens to AJAX calls |
| 5 | H3 | Remove `[IgnoreAntiforgeryToken]` from `AdminResultController.RecalculateResults` and `BulkApproveResults` |
| 6 | H4 | Remove `[IgnoreAntiforgeryToken]` from `ResultManagementController.ProcessReEvaluation` |
| 7 | H5 | Add `[RequirePermission("Exam.View")]` to `ResultManagementController.GetSubjectsForClass` |
| 8 | M5 | Add `[Authorize(Roles = "Student")]` to `TranscriptController.MyTranscript` |

### Phase 2 — Permission Model Standardization

| Order | ID | Action |
|---|---|---|
| 9 | M2 | Standardize permission naming: replace `Exam.Update` with `Exam.Edit` across `ExamSubjectController` |
| 10 | M3 | Replace role strings in `MarksController` with `[RequirePermission("Marks.View")]`, `[RequirePermission("Marks.Entry")]`, `[RequirePermission("Marks.Approve")]` |
| 11 | H1 | After M3 is complete, remove duplicated role strings |
| 12 | H6 | Create `Guardians.*` permission strings and update `GuardianController` |
| 13 | M1 | Consolidate `StudentController` to attribute-only; remove inline `HasClaim` checks |

### Phase 3 — Data Scoping & Ownership

| Order | ID | Action |
|---|---|---|
| 14 | M4 | Add student ownership check in `ReportCardController` for Student/Guardian role users |
| 15 | H8 | Remove `Student` role from `StudentClassAssignmentController` or add self-service guard |
| 16 | H7 | Add `[RequirePermission("Exam.Schedule")]` to `ExamScheduleController` actions |
| 17 | M6 | Add data-level scoping to `AuditLogController` queries |

### Phase 4 — Housekeeping

| Order | ID | Action |
|---|---|---|
| 18 | L2 | Remove unused `AuthorizeRoleAttribute.cs` |
| 19 | L1 | Audit/clean up unused permission strings in `DbInitializer` |
| 20 | L3 | Consider migrating `ModulesController` to use `[RequirePermission("Modules.View")]` |
| 21 | L5 | Add per-action permissions to `ExamController` |

---

## COMPLIANCE MATRIX

| Requirement | Status | Notes |
|---|---|---|
| All Exam CRUD actions have authorization | ✅ Partial | ExamController OK (role-gated), ExamSubjectController OK (permission-gated), GenericCrudController inheritors FAIL |
| Mark Entry/Submission/Approval gated by role/permission | ✅ Partial | Role strings used instead of permissions; CSRF disabled on 5 endpoints |
| Result Publication/Unpublication gated by admin role | ✅ | `AdminResultController` and `ExamAdminController` both role-gated |
| Student Portal restricted to Student role | ✅ | `ResultManagementController` Student actions all have `[Authorize(Roles = "Student")]` |
| Guardian Portal restricted to Guardian role + per-student check | ✅ | Strongest pattern in codebase |
| Teacher Portal restricted to Teacher role + subject/class scope | ✅ | `ITeacherScopeService` + `IResultAuthorizationService` |
| Report Card download respects role scoping | ⚠️ Partial | Ownership check missing for Student/Guardian roles |
| Transcript access restricted | ⚠️ Partial | `MyTranscript` has no role filter |
| Admit Card generation/view restricted | ✅ | `AdmitCard.Generate` / `AdmitCard.View` permissions |
| API endpoints have CSRF protection | ❌ FAIL | 8 endpoints across 3 controllers have `[IgnoreAntiforgeryToken]` |
| Permission strings consistent across codebase | ❌ FAIL | `Exam.Update` vs `Exam.Edit` naming conflict |
| GenericCrudController inheritors scoped correctly | ❌ FAIL | 3 of 4 inheritors lack proper authorization |
| Super Admin bypass works as intended | ✅ | `RequirePermissionAttribute` correctly auto-allows Super Admin |

---

## APPENDIX A: CONTROLLER AUTHORIZATION MAP

| Controller | Class Auth | Action Auth | Permission Used | Risk Level |
|---|---|---|---|---|
| `ExamController` | `[Authorize(Roles)]` | None | None | 🟡 MEDIUM |
| `ExamAdminController` | `[Authorize(Roles)]` + `[IgnoreAntiforgeryToken]` | `[Authorize(Roles)]` per action | None | 🔴 CRITICAL |
| `ExamSubjectController` | `[Authorize]` | `[RequirePermission("Exam.Update")]` | ✅ Correct | 🟢 LOW |
| `ExamScheduleController` | `[Authorize(Roles)]` | None | None | 🟠 HIGH |
| `ExamComponentsController` | `[Authorize]` | `[Permission("Exam", "*")]` | ✅ Correct | 🟢 LOW |
| `SubjectMarkStructureController` | `[Authorize]` | `[Permission("Exam", "*")]` | ✅ Correct | 🟢 LOW |
| `AdmitCardController` | `[Authorize]` | `[RequirePermission("AdmitCard.*")]` | ✅ Correct | 🟢 LOW |
| `MarksController` | `[Authorize]` | `[Authorize(Roles)]` (no permissions) | ❌ Missing | 🟠 HIGH |
| `ResultManagementController` | `[Authorize]` | `[Authorize(Roles)]` per action | Mixed | 🟠 HIGH |
| `AdminResultController` | `[Authorize]` | `[Authorize(Roles)]` | Mixed | 🟠 HIGH |
| `ResultController` | `[Authorize]` | None (redirects only) | N/A | 🟢 LOW |
| `ReportCardController` | `[Authorize]` | `[Authorize(Roles)]` | Mixed | 🟡 MEDIUM |
| `TranscriptController` | `[Authorize]` | `[Authorize(Roles)]` | Mixed | 🟡 MEDIUM |
| `MeritListController` | `[Authorize]` | `[Authorize(Roles)]` | Mixed | 🟢 LOW |
| `GenericCrudController<T>` | `[Authorize]` | None | ❌ FAIL | 🔴 CRITICAL |
| `RoleController` (inherits) | (inherited `[Authorize]`) | None | ❌ FAIL | 🔴 CRITICAL |
| `PermissionController` (inherits) | (inherited `[Authorize]`) | None | ❌ FAIL | 🔴 CRITICAL |
| `SystemSettingsController` (inherits) | (inherited `[Authorize]`) | None | ❌ FAIL | 🔴 CRITICAL |
| `GuardianPortalPagesController` | `[Authorize(Roles="Guardian")]` | + per-student check | ✅ Best | 🟢 LOW |
| `GuardianPortalController` | `[Authorize(Roles="Guardian")]` | + per-student check | ✅ Best | 🟢 LOW |
| `GuardianActivationController` | `[AllowAnonymous]` | None | ✅ Intentional | 🟢 LOW |
| `StudentController` | `[Authorize]` | `[RequirePermission("Student.*")]` + inline claims | ⚠️ Redundant | 🟡 MEDIUM |
| `TeacherController` | `[Authorize]` | `[RequirePermission("Teachers.*")]` | ✅ Correct | 🟢 LOW |
| `TeacherAssignmentController` | `[Authorize]` | `[RequirePermission("Teachers.Assign")]` | ✅ Correct | 🟢 LOW |

---

## APPENDIX B: PERMISSION STRING INVENTORY

| Permission String | Used In | Status |
|---|---|---|
| `Exam.View` | SubjectMarkStructureController, ExamComponentsController | ✅ Used |
| `Exam.Create` | ExamComponentsController | ✅ Used |
| `Exam.Edit` | SubjectMarkStructureController, ExamComponentsController | ✅ Used |
| `Exam.Update` | ExamSubjectController | ❌ Inconsistent (rename to Edit) |
| `Exam.Delete` | ExamComponentsController | ✅ Used |
| `Student.View` | StudentController, GuardianController | ✅ Used |
| `Student.Create` | StudentController, GuardianController | ✅ Used |
| `Student.Edit` | StudentController | ✅ Used |
| `Student.Update` | GuardianController | ✅ Used |
| `Student.Delete` | StudentController, GuardianController | ✅ Used |
| `Teachers.View` | TeacherController | ✅ Used |
| `Teachers.Create` | TeacherController | ✅ Used |
| `Teachers.Edit` | TeacherController | ✅ Used |
| `Teachers.Delete` | TeacherController | ✅ Used |
| `Teachers.Assign` | TeacherAssignmentController | ✅ Used |
| `Users.View` | UserController | ✅ Used |
| `Users.Create` | UserController | ✅ Used |
| `Users.Edit` | UserController | ✅ Used |
| `Users.Delete` | UserController | ✅ Used |
| `Users.Assign` | UserController | ✅ Used |
| `AdmitCard.Generate` | AdmitCardController | ✅ Used |
| `AdmitCard.View` | AdmitCardController | ✅ Used |
| `AuditLogs.View` | AuditLogController | ✅ Used |
| `Guardians.*` | — | ❌ Missing — should be created |

---

## APPENDIX C: [IgnoreAntiforgeryToken] INVENTORY

| File | Line | Scope | Method |
|---|---|---|---|
| `Controllers/Result/ExamAdminController.cs` | 18 | **Class-level** | **All POST/PUT/DELETE** |
| `Controllers/Result/MarksController.cs` | 153 | Action | `SaveMarks` (POST) |
| `Controllers/Result/MarksController.cs` | 194 | Action | `UpdateMarks` (PUT) |
| `Controllers/Result/MarksController.cs` | 237 | Action | `SubmitMarks` (POST) |
| `Controllers/Result/MarksController.cs` | 388 | Action | `BulkUpdateMarks` (POST) |
| `Controllers/Result/MarksController.cs` | 404 | Action | `ApproveAllMarks` (POST) |
| `Controllers/Result/AdminResultController.cs` | 371 | Action | `RecalculateResults` (POST) |
| `Controllers/Result/AdminResultController.cs` | 388 | Action | `BulkApproveResults` (POST) |
| `Controllers/Result/ResultManagementController.cs` | 270 | Action | `ProcessReEvaluation` (POST) |
| `Controllers/Auth/AuthController.cs` | 92 | Action | `VerifyOtp` (POST) — acceptable for auth |

---

## CONCLUSION

The Examination, Marks & Result subsystem has a fundamentally sound authorization architecture with two powerful mechanisms — `RequirePermissionAttribute` (database-driven) and service-level scoping (`IResultAuthorizationService`, `ITeacherScopeService`, `IGuardianService.UserHasAccessToStudentAsync`). However, the audit reveals **3 critical gaps**, **8 high-risk issues**, and **6 medium-risk inconsistencies** that need remediation.

The single highest-impact finding is **C1**: the `GenericCrudController<T>` inheritance pattern leaves `RoleController`, `PermissionController`, and `SystemSettingsController` accessible to any authenticated user — a direct privilege escalation path that undermines the entire RBAC system. This should be fixed before any other remediation work begins.

The second-highest priority is **CSRF protection restoration** across 8 endpoints in 3 controllers (C2, H2, H3, H4).

The remaining findings relate to **permission model consistency** and **data-level scoping** — important but lower urgency.

*A detailed remediation plan with code changes is available on request.*
