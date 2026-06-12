# SECURITY HOTFIX REPORT

**Date:** 2026-06-12
**Build:** 0 errors · 107/107 tests passed · 0 new warnings
**Scope:** Phase Security Hotfix — authorization + CSRF only

---

## CHANGES APPLIED

### 1. GenericCrudController Inheritors — Role/Permission/SystemSettings Lockdown

**Risk:** CRITICAL — any authenticated user could CRUD Roles, Permissions, and System Settings.

| File | Change |
|---|---|
| `Controllers/Admin/RoleController.cs:10` | Added `[Authorize(Roles = "Admin,Super Admin")]` class-level |
| `Controllers/Admin/PermissionController.cs:7` | Added `[Authorize(Roles = "Admin,Super Admin")]` class-level |
| `Controllers/Admin/SystemSettingsController.cs:7` | Added `[Authorize(Roles = "Admin,Super Admin,Principal")]` class-level |

### 2. ExamAdminController — CSRF Protection Restored

**Risk:** CRITICAL — class-level `[IgnoreAntiforgeryToken]` disabled CSRF on all 15+ POST endpoints.

| File | Change |
|---|---|
| `Controllers/Result/ExamAdminController.cs:18` | Removed class-level `[IgnoreAntiforgeryToken]`. Global `AutoValidateAntiforgeryToken` filter (Program.cs:81) now protects all endpoints. No individual endpoint required external API bypass. |

### 3. MarksController — CSRF Protection Restored on 5 Actions

**Risk:** HIGH — mark entry/submission/lock/unlock endpoints were CSRF-vulnerable.

| File | Action | Change |
|---|---|---|
| `Controllers/Result/MarksController.cs:153` | `Save` | Removed `[IgnoreAntiforgeryToken]` |
| `Controllers/Result/MarksController.cs:194` | `SaveRow` | Removed `[IgnoreAntiforgeryToken]` |
| `Controllers/Result/MarksController.cs:237` | `SaveDraft` | Removed `[IgnoreAntiforgeryToken]` |
| `Controllers/Result/MarksController.cs:388` | `Lock` | Removed `[IgnoreAntiforgeryToken]` |
| `Controllers/Result/MarksController.cs:404` | `Unlock` | Removed `[IgnoreAntiforgeryToken]` |

### 4. MarksController AJAX — Antiforgery Token Added to Client Requests

**Risk:** Previously, client-side AJAX calls did not send the `RequestVerificationToken` header, causing 400 errors after CSRF restoration.

| File | Change |
|---|---|
| `Views/Marks/Entry.cshtml:99` | Added `@Html.AntiForgeryToken()` to generate token field |
| `Views/Marks/Entry.cshtml:102` | Added `antiforgeryToken` variable reading from hidden field |
| `Views/Marks/Entry.cshtml:182` | Added `headers: { 'RequestVerificationToken': antiforgeryToken }` to `$.ajax` calls for `Save` and `SaveDraft` |
| `Views/Marks/EntryStatus.cshtml:101` | Added `@Html.AntiForgeryToken()` + `getAntiforgeryToken()` helper |
| `Views/Marks/EntryStatus.cshtml:109,122` | Added `'RequestVerificationToken': getAntiforgeryToken()` header to `fetch()` calls for `Lock` and `Unlock` |

**Note:** `wwwroot/js/exam/marks-entry.js` already sent `RequestVerificationToken` header — no change needed.

### 5. AdminResultController — CSRF Protection Restored on 2 Actions

**Risk:** HIGH — result approval/rejection endpoints were CSRF-vulnerable.

| File | Action | Change |
|---|---|---|
| `Controllers/Result/AdminResultController.cs:371` | `ApproveResults` | Removed `[IgnoreAntiforgeryToken]` |
| `Controllers/Result/AdminResultController.cs:388` | `RejectResults` | Removed `[IgnoreAntiforgeryToken]` |

### 6. ResultManagementController — CSRF Protection Restored

**Risk:** HIGH — re-evaluation processing endpoint was CSRF-vulnerable.

| File | Action | Change |
|---|---|---|
| `Controllers/Result/ResultManagementController.cs:270` | `ProcessReEvaluation` | Removed `[IgnoreAntiforgeryToken]` |

### 7. ReportCardController — Ownership Validation Verified

**Result:** Already compliant. The `Download` and `BangladeshFormat` actions already contain:
- Student role check: verifies `student.Id == studentId` (lines 100-105, 152-157)
- Guardian role check: verifies `StudentGuardian` linkage via `AnyAsync()` (lines 106-111, 158-163)
- Teacher role check: uses `ITeacherScopeService.HasStudentAccessAsync()` (lines 112-116, 164-168)

No changes required.

### 8. TranscriptController — MyTranscript Role Restriction

**Risk:** MEDIUM — `MyTranscript()` had only `[Authorize]` with no role filter.

| File | Change |
|---|---|
| `Controllers/Result/TranscriptController.cs:43` | Changed `[Authorize]` → `[Authorize(Roles = "Student")]` |

---

## VERIFICATION

| Check | Result |
|---|---|
| `dotnet build` | 0 errors, 0 new warnings |
| `dotnet test` | 107/107 passed |
| Route changes | None |
| UI changes | None (antiforgery token hidden fields only) |
| Business logic changes | None |
| New files created | None |

---

## FILES MODIFIED (13 total)

| # | File | Change |
|---|---|---|
| 1 | `Controllers/Admin/RoleController.cs` | Added `[Authorize(Roles = "Admin,Super Admin")]` |
| 2 | `Controllers/Admin/PermissionController.cs` | Added `using` + `[Authorize(Roles = "Admin,Super Admin")]` |
| 3 | `Controllers/Admin/SystemSettingsController.cs` | Added `using` + `[Authorize(Roles = "Admin,Super Admin,Principal")]` |
| 4 | `Controllers/Result/ExamAdminController.cs` | Removed class-level `[IgnoreAntiforgeryToken]` |
| 5 | `Controllers/Result/MarksController.cs` | Removed 5× `[IgnoreAntiforgeryToken]` |
| 6 | `Controllers/Result/AdminResultController.cs` | Removed 2× `[IgnoreAntiforgeryToken]` |
| 7 | `Controllers/Result/ResultManagementController.cs` | Removed `[IgnoreAntiforgeryToken]` |
| 8 | `Controllers/Result/TranscriptController.cs` | `[Authorize]` → `[Authorize(Roles = "Student")]` |
| 9 | `Views/Marks/Entry.cshtml` | Added antiforgery token to page + AJAX headers |
| 10 | `Views/Marks/EntryStatus.cshtml` | Added antiforgery token to page + fetch headers |

---

## SECURITY POSTURE IMPROVEMENT

| Metric | Before | After |
|---|---|---|
| GenericCrudController inheritors unprotected | 3 controllers | 0 controllers |
| Class-level `[IgnoreAntiforgeryToken]` | 1 controller | 0 controllers |
| Action-level `[IgnoreAntiforgeryToken]` | 8 endpoints | 0 endpoints |
| `MyTranscript` unrestricted | Yes (any auth user) | Restricted to Student role |
| ReportCard/Transcript ownership checks | Verified correct | Verified correct |
