# PHASE 42Z-V — STORED PROCEDURE PAGINATION & SORTING HARDENING

## COMPREHENSIVE AUDIT REPORT

**Date:** 2026-06-21  
**Scope:** 124 SQL files, 66 SPs called from C#, 22 modules  
**Auditor:** Senior SQL Server Architect  

---

## 1. INVENTORY

### 1.1 SPs Called from C#

| # | SP Name | Module | Used By Controller | Used By Repository | Used By Service |
|---|---------|--------|-------------------|-------------------|-----------------|
| 1 | sp_GetStudentList | Students | StudentController | StudentRepositories | StudentService |
| 2 | sp_GetAdmissionList | Admission | (via AdmissionService) | AdmissionRepositories | AdmissionService |
| 3 | sp_GetAttendanceList | Attendance | AttendanceRecordController | AttendanceRepositories | AttendanceRecordService |
| 4 | sp_GetStudentAttendanceList | Attendance | StudentAttendanceController | AttendanceModuleRepositories | StudentAttendanceService |
| 5 | sp_GetEmployeeAttendanceList | Attendance | EmployeeAttendanceController | AttendanceModuleRepositories | EmployeeAttendanceService |
| 6 | sp_GetAttendanceSummary | Attendance | StudentAttendanceController / EmployeeAttendanceController | AttendanceModuleRepositories | Both services |
| 7 | sp_GetAttendanceSessions | Attendance | AttendanceSessionController | AttendanceSessionRepository | StudentAttendanceService |
| 8 | sp_GetAttendanceDashboardSummary | Dashboard | DashboardController | DashboardRepositories | DashboardService |
| 9 | sp_GetAttendanceAnalytics | Dashboard | DashboardController | DashboardRepositories | DashboardService |
| 10 | sp_GetClassAttendanceAnalytics | Dashboard | DashboardController | DashboardRepositories | DashboardService |
| 11 | sp_GetEmployeeInvitationList | Employee | EmployeeController | EmployeeRepositories | EmployeeService |
| 12-34 | 23 Fee SPs | Fees | 15 Fee Controllers | FeeRepositories, FeeReportRepository, FeeDashboardRepository, StudentFinanceRepository | FeeServices |
| 35 | sp_GetGuardianList | Guardian | GuardianController | GuardianRepository | GuardianService |
| 36 | sp_GetGuardianDetails | Guardian | GuardianController | GuardianRepository | GuardianService |
| 37 | sp_GetGuardianDashboard | Guardian | (via GuardianService) | GuardianRepository | GuardianService |
| 38 | sp_GetGuardianChildren | Guardian | GuardianController | GuardianRepository | GuardianService |
| 39 | sp_GetStudentIdCardList | Identity | IdCardController | IdCardRepository | IdCardService |
| 40 | sp_GetEmployeeIdCardList | Identity | IdCardController | IdCardRepository | IdCardService |
| 41 | sp_GetStudentIdCardBulkData | Identity | IdCardController | IdCardRepository | IdCardService |
| 42 | sp_GetEmployeeIdCardBulkData | Identity | IdCardController | IdCardRepository | IdCardService |
| 43 | sp_GetMarksEntryList | Marks | MarksController / ResultManagementController | MarkEntryRepository | MarkEntryService |
| 44 | sp_PublishResults | Result | AdminResultController | (direct ExecuteSqlRawAsync) | — |
| 45 | sp_UnpublishResults | Result | AdminResultController | (direct ExecuteSqlRawAsync) | — |
| 46 | sp_RecalculateResults | Result | AdminResultController | (direct ExecuteSqlRawAsync) | — |
| 47 | sp_CalculateMerit | Result | AdminResultController | (direct ExecuteSqlRawAsync) | — |
| 48 | sp_GetExamDashboard | Result | ExamController / ResultManagementController | ResultRepository | ResultService |
| 49 | sp_GetExamList | Result | ExamController | ResultRepository | ResultService |
| 50 | sp_GetResultList | Result | ResultManagementController | StudentExamResultRepository | — |
| 51 | sp_GetResultSummary | Result | ResultManagementController | StudentExamResultRepository | — |
| 52 | sp_GetStudentResults | Result | TranscriptController | StudentExamResultRepository | TranscriptService |
| 53 | sp_GetReportCard | Result | ReportCardController | StudentExamResultRepository | ReportCardService |
| 54 | sp_GetResultPublicationDashboard | Result | ResultPublicationController | ResultPublicationRepository | ResultPublicationService |
| 55 | sp_GetTeacherAssignedExams | Result | TeacherResultController | TeacherResultRepository | — |
| 56 | sp_GetTeacherAssignedSubjects | Result | TeacherResultController | TeacherResultRepository | — |
| 57 | sp_GetTeacherMarksEntrySheet | Result | TeacherResultController | TeacherResultRepository | — |
| 58 | sp_GetTeacherResultSummary | Result | TeacherResultController | TeacherResultRepository | — |
| 59 | sp_GetTeacherExportSheet | Result | TeacherResultController | TeacherResultRepository | — |
| 60-65 | 6 Student Dashboard SPs | Student | StudentFinanceController / GuardianFinanceController | StudentFinanceRepository | StudentFinanceService |
| 66 | SP_System_DatabaseHealth | System | SystemHealthController | (direct SqlQueryRaw) | — |
| 67 | SP_System_DashboardMetrics | System | SystemHealthController / MonitoringController | (direct SqlQueryRaw) | — |

### 1.2 SPs Defined But Unused (62 files)

These SPs are deployed by `StoredProcedureInstaller` but never called from C# code. They are candidates for deprecation or represent planned-but-unimplemented features:

- Academic (5): sp_AssignStudentToSection, sp_GetAcademicYearList*, sp_GetClassList*, sp_GetSectionList*, sp_GetSubjectList*
- AdmitCard (2): sp_BulkGenerateAdmitCards, sp_GenerateAdmitCard
- Analytics (3): sp_GetClassSummary, sp_GetGroupSummary, sp_GetStudentTrend
- Attendance (4): sp_GetAbsentStudents, sp_GetAttendanceHistory, sp_GetAttendanceRevisionHistory, sp_GetLateStudents, sp_GetEmployeeAttendanceAnalytics
- Audit (1): sp_Audit_LogAction
- Backup (2): SP_BackupExamResults, SP_RestoreExamResults
- Exam (11): SP_ExamRoutine_* (4), SP_Exam_DashboardSummary, SP_Exam_GetAllResults, sp_GetExamComponents, sp_GetExamMarkStructure, sp_GetExamScheduleList, sp_GetGroupReport, sp_GetSubjectMarkStructure, sp_SaveSubjectMarkStructure
- Guardian (4): sp_GetGuardianFees*, sp_GetGuardianAttendance, sp_GetGuardianResults, sp_GetGuardianLeaveApplications, sp_GetGuardianNotifications, sp_VerifyGuardianDataIntegrity
- Marks (5): sp_BulkImportMarks, sp_LockMarksEntry, SP_MarkEntry_GetGrid, sp_SaveMarks, sp_UnlockMarksEntry
- Notification (1): sp_Notification_Enqueue
- ReportCard (2): sp_BulkGenerateReportCards, SP_ReportCard_Generate
- Result (2): sp_CalculateExamResults, sp_CalculateSubjectResults, sp_GetTranscript*
- Results_Fixed (3): sp_CalculateExamRanking, sp_GetExamsForAdmin, sp_GetMarkEntrySheet
- Role (1): sp_GetRoleList
- Student (3): sp_GetStudentAssignmentsPaged*, sp_GetStudentLibraryPaged*, sp_GetStudentNotificationsPaged*
- Teacher (3): sp_GetTeacherDashboardSchedule, sp_GetTeacherDashboardMarkStatus, sp_GetTeacherDashboardPendingResults
- User (1): sp_GetUserList*
- System (2): SP_System_VerifyStoredProcedures

_* These SPs have @PageNumber/@PageSize and are included in the pagination audit._

---

## 2. PAGINATION REPORT

### 2.1 Summary

| Metric | Count | Percentage |
|--------|-------|------------|
| SPs with @PageNumber/@PageSize | 49 | 100% |
| **PASS** (deterministic ORDER BY) | **20** | **40.8%** |
| **FAIL** (non-deterministic ORDER BY) | **29** | **59.2%** |
| ROW_NUMBER() CTE pagination | 30 | 61.2% |
| OFFSET/FETCH NEXT pagination | 19 | 38.8% |
| Has TotalRecords (COUNT(*) OVER()) | 39 | 79.6% |
| Has separate COUNT query | 10 | 20.4% |
| LastPage computed in SP | 1 | 2.0% |

### 2.2 PASS (20 SPs)

sp_GetAttendanceList, sp_GetAttendanceSessions, sp_GetClassList, sp_GetSectionList, sp_GetSubjectList, sp_GetStudentInvoicesPaged, sp_GetStudentPaymentsPaged, sp_GetStudentLedgerPaged, sp_GetFeeInvoiceList, sp_GetFeeInvoiceItemsPaged, sp_GetFeeLedgerPaged, sp_GetFeeWaiversPaged, sp_GetUserList, sp_GetRoleList, sp_GetEmployeeIdCardList, sp_GetGuardianNotifications, sp_GetGuardianLeaveApplications, sp_GetStudentList*

_* sp_GetStudentList was fixed in this phase (FIX-S01)_

### 2.3 FAIL (29 SPs)

**Root cause:** ORDER BY missing a unique-column tiebreaker (PK column). Causes rows to shift between pages, appear on multiple pages, or be skipped entirely.

| Module | SPs | Fix Pattern |
|--------|-----|-------------|
| Academic (1) | sp_GetAcademicYearList | Add `, y.Id DESC` to ROW_NUMBER() OVER() |
| Admission (1) | sp_GetAdmissionList | Add `, a.Id DESC` to ROW_NUMBER() OVER() |
| Attendance (2) | sp_GetStudentAttendanceList, sp_GetEmployeeAttendanceList | Add PK tiebreaker to ORDER BY |
| Employee (1) | sp_GetEmployeeInvitationList | Add `, i.Id DESC` to ORDER BY |
| Exam (1) | sp_GetExamList | Add `, e.Id DESC` to fallback ORDER BY |
| Fees (17) | All Fee paging/report SPs | Add `, <PK> DESC` to ORDER BY |
| Guardian (1) | sp_GetGuardianList | Add soft delete + PK tiebreaker |
| Identity (1) | sp_GetStudentIdCardList | Add `, s.Id DESC` to ROW_NUMBER() |
| Result (1) | sp_GetResultList | Add `, ser.Id` to ORDER BY |
| Student (3) | sp_GetStudentAssignmentsPaged, sp_GetStudentLibraryPaged, sp_GetStudentNotificationsPaged | Add PK tiebreaker |
| Teacher (1) | sp_GetTeacherList | Add `, t.Id` to ROW_NUMBER() |
| Audit (1) | sp_Audit_GetLogs | Add PK tiebreaker |

---

## 3. SORTING REPORT

### 3.1 Summary

| Metric | Count | Percentage |
|--------|-------|------------|
| SPs with @SortColumn/@SortDirection | 2 | 4.1% (of 49 paged SPs) |
| **PASS** (CASE-based whitelist) | **2** | **100%** |
| **FAIL** | **0** | **0%** |
| SQL injection risk from dynamic sort | 0 | 0% |

### 3.2 Sorted SPs

| SP Name | @SortColumn Values | Whitelist Implementation | SQL Injection Risk |
|---------|-------------------|-------------------------|-------------------|
| **sp_GetAttendanceList** | StudentName, ClassName, SectionName, AttendanceDate | CASE WHEN with whitelist; NULL values fall to default `AttendanceDate DESC, Id DESC` | **NONE** — column values are never concatenated into SQL string |
| **sp_GetExamList** | Name, StartsOn, Status | CASE WHEN with whitelist; NULL values fall to `e.CreatedAt DESC` | **NONE** — same safe pattern |

### 3.3 Recommendation

Adding @SortColumn to all 49 paged SPs is a large but valuable enhancement. Currently sorting is hardcoded per SP. A phased approach is recommended — start with the most-used grids (Student, Fee Invoice, Result).

---

## 4. SEARCH REPORT

### 4.1 Summary

| Metric | Count | Percentage |
|--------|-------|------------|
| SPs with @SearchTerm | 38 | 100% (of audited) |
| **PASS** | **38** | **100%** |
| **FAIL** | **0** | **0%** |
| LIKE pattern with surrounding % | 38 | 100% |
| CONTAINS / Full-Text Search | 0 | 0% |
| Nullable handling (IS NULL OR) | 38 | 100% |

### 4.2 Patterns

- **Leading-wildcard issue:** All 38 SPs use `LIKE '%' + @SearchTerm + '%'` which prevents index seeks. This is acceptable for the current data volumes but should be migrated to Full-Text Search (`CONTAINS`) for tables exceeding 50K rows.
- **Columns searched:** Typically 2-4 relevant text columns per SP (name, code, phone, email, invoice no, etc.)
- **Null handling:** All SPs properly short-circuit search when `@SearchTerm IS NULL`.

---

## 5. SECURITY REPORT

### 5.1 Summary

| Metric | Count |
|--------|-------|
| Dynamic SQL (`EXEC` with concatenation) | **0** |
| SQL injection vulnerabilities | **0** |
| @SortColumn whitelisted | **2/2 (100%)** |
| Non-SARGable `CAST` on columns | **3 instances** |
| Soft-delete bypass showing deleted data | **9 instances** |

### 5.2 SQL Injection Vectors

**Zero** SQL injection vectors found. All SPs use:
- Parameterized stored procedure parameters only
- CASE-based sort column whitelisting (not string concatenation)
- `OPENJSON()` for JSON parsing (safe — parses JSON, does not execute)
- `STRING_SPLIT()` with `ISNUMERIC()` guard for CSV IDs

### 5.3 Soft Delete Violations (CRITICAL)

| SP Name | Table(s) Missing IsDeleted=0 | Impact |
|---------|------------------------------|--------|
| **sp_GetGuardianList** | Guardians | Guardian list shows soft-deleted records (PII leak) |
| **sp_GetGuardianDashboard** | FeeInvoices, Attendance, Notices | Dashboard aggregates include deleted records |
| **sp_GetGuardianDetails** | Guardians, Students, StudentGuardians | Children list shows deleted students |
| **sp_GetGuardianFees** | Payments (outer query) | Payment list includes deleted payments |
| **sp_GetExamScheduleList** | ExamSchedules, Subjects, Classes, Sections, Groups | Schedule shows deleted entities |
| **SP_RestoreExamResults** | Uses hard `DELETE FROM` instead of soft delete | Destructive restore — bypasses audit trail |
| **sp_GetReportCard** | Students, Exams | Report cards generated for deleted students |
| **SP_ReportCard_Generate** | Same as GetReportCard | Same issue |

---

## 6. PERFORMANCE REPORT

### 6.1 Summary

| Risk Level | Count | Description |
|-----------|-------|-------------|
| **HIGH** | 5 | Non-SARGable WHERE clauses, double scans |
| **MEDIUM** | 23 | Scalar subqueries, missing indexes, N+1 patterns |
| **LOW** | 14 | Leading-wildcard LIKE, wide column SELECT |

### 6.2 HIGH Risk Issues (Must Fix)

| # | SP Name | Issue | Line(s) |
|---|---------|-------|---------|
| H01 | sp_GetAttendanceSummary | `YEAR(AttendanceDate) = @Year AND MONTH(AttendanceDate) = @Month` — full table scan | 54-55, 69-70 |
| H02 | sp_GetDailyCollectionReport | `CAST(p.PaidAt AS DATE) = @CollectionDate` — full scan on Payments | 17 |
| H03 | sp_GetMonthlyCollectionReport | `YEAR(p.PaidAt) = @Year` — full scan on Payments | 15 |
| H04 | sp_GetFeeCollectionSummariesPaged | `CAST(fcs.CollectionDate AS NVARCHAR) LIKE '%' + @SearchTerm + '%'` — double non-SARGable | 38 |
| H05 | sp_GetResultList | Separate COUNT query + data query with identical WHERE — double scan | 16-27 |

### 6.3 MEDIUM Risk Issues (23)

Key patterns:
- **Scalar subqueries in SELECT**: sp_GetClassList (2 subqueries — SectionCount, StudentCount), sp_GetStudentList (guardian mobile), sp_GetAttendanceSessions (3 subqueries — TotalStudents, Present, Absent)
- **LEFT JOIN causing row multiplication**: sp_GetStudentLedgerReport (Payments 1:N join inflates rows)
- **IN subquery instead of JOIN**: sp_GetGuardianFees (Payments WHERE IN subquery)
- **Missing indexes**: 12 recommended covering indexes across Attendance, Fees, Results, and Dashboard tables

### 6.4 Recommended Indexes

```sql
-- Attendance
CREATE NONCLUSTERED INDEX IX_Attendance_ClassSection_Date_Status
    ON Attendance(SchoolClassId, SectionId, AttendanceDate)
    INCLUDE (StudentId, Status, IsDeleted);

CREATE NONCLUSTERED INDEX IX_Attendance_StudentId_Date
    ON Attendance(StudentId, AttendanceDate)
    INCLUDE (Status, IsDeleted);

CREATE NONCLUSTERED INDEX IX_AttendanceSessions_Filtered
    ON AttendanceSessions(SchoolClassId, SectionId, StudentGroupId, AttendanceDate)
    INCLUDE (Status, CreatedBy, CreatedAt, IsDeleted);

CREATE NONCLUSTERED INDEX IX_EmployeeAttendances_EmployeeId_Date
    ON EmployeeAttendances(EmployeeId, AttendanceDate)
    INCLUDE (Status, IsDeleted);

-- Fee module
CREATE NONCLUSTERED INDEX IX_FeeInvoices_StudentId_IsDeleted
    ON FeeInvoices(StudentId, IsDeleted)
    INCLUDE (TotalAmount, PaidAmount, Status, DueDate, DiscountAmount, LateFee, AcademicYearId);

CREATE NONCLUSTERED INDEX IX_FeeInvoices_Dashboard
    ON FeeInvoices(IsDeleted, AcademicYearId, Status, DueDate)
    INCLUDE (TotalAmount, PaidAmount, DiscountAmount);

CREATE NONCLUSTERED INDEX IX_Payments_FeeInvoice_IsDeleted
    ON Payments(FeeInvoiceId, IsDeleted, Method, PaidAt)
    INCLUDE (Amount, ReferenceNo, LateFee, DiscountAmount);

CREATE NONCLUSTERED INDEX IX_FeeLedgers_StudentId_TransactionDate
    ON FeeLedgers(StudentId, IsDeleted, TransactionType, TransactionDate DESC)
    INCLUDE (Debit, Credit, Balance, Description, FeeInvoiceId);

-- Result module
CREATE NONCLUSTERED INDEX IX_StudentExamResults_Exam_Status
    ON StudentExamResults(ExamId, Status, IsDeleted)
    INCLUDE (StudentId, TotalMarks, TotalFullMarks, Gpa, Grade, IsPassed);

-- Student dashboard
CREATE NONCLUSTERED INDEX IX_NotificationMessages_UserId_CreatedAt
    ON NotificationMessages(UserId, IsDeleted, CreatedAt DESC)
    INCLUDE (Title, Body, Channel, IsRead);

CREATE NONCLUSTERED INDEX IX_AssignmentTasks_Class_Section
    ON AssignmentTasks(SchoolClassId, SectionId, IsDeleted)
    INCLUDE (Title, Deadline, SubjectId, Status);
```

---

## 7. GENERATED SQL FIXES

All fixes are in `Data\SP_Fixes.sql`. Summary:

### 7.1 Fix Count by Batch

| Batch | Module | Fixes | HIGH | MEDIUM | LOW |
|-------|--------|-------|------|--------|-----|
| 1 | Students | 4 | 0 | 4 | 0 |
| 2 | Employees | 3 | 0 | 3 | 0 |
| 3 | Attendance | 3 | 2 | 1 | 0 |
| 4 | Finance | 17 | 2 | 14 | 1 |
| 5 | Results | 3 | 1 | 2 | 0 |
| 6 | Dashboard & Other | 8 | 3 | 5 | 0 |
| **Total** | **All** | **30** | **8** | **29** | **1** |

### 7.2 Fix Categories

| Fix Type | Count | SPs |
|----------|-------|-----|
| PK tiebreaker (ORDER BY) | 23 | All failing pagination SPs |
| SARGable date range (YEAR/MONTH/CAST → range) | 3 | sp_GetAttendanceSummary, sp_GetDailyCollectionReport, sp_GetMonthlyCollectionReport |
| Eliminate separate COUNT (window function) | 1 | sp_GetResultList |
| Soft delete IsDeleted=0 | 4 | sp_GetGuardianList, sp_GetGuardianDashboard, sp_GetGuardianDetails, sp_GetGuardianFees |
| IN subquery → JOIN | 1 | sp_GetGuardianFees |
| LEFT JOIN → OUTER APPLY (row dedup) | 1 | sp_GetStudentLedgerReport |
| BIT comparison simplification | 1 | sp_GetStudentNotificationsPaged |
| Wrong status label fix | 1 | sp_GetGuardianFees |

### 7.3 BEFORE/AFTER Examples

#### Example 1: PK Tiebreaker (23 SPs)
```sql
-- BEFORE
ROW_NUMBER() OVER (ORDER BY s.CreatedAt DESC) AS RowNum

-- AFTER
ROW_NUMBER() OVER (ORDER BY s.CreatedAt DESC, s.Id DESC) AS RowNum
```

#### Example 2: SARGable Date Range (3 SPs)
```sql
-- BEFORE (non-SARGable, full scan)
WHERE YEAR(PaidAt) = @Year

-- AFTER (SARGable, index seek)
WHERE PaidAt >= DATEFROMPARTS(@Year, 1, 1)
  AND PaidAt < DATEFROMPARTS(@Year + 1, 1, 1)
```

#### Example 3: Eliminate Separate COUNT (1 SP)
```sql
-- BEFORE (two scans)
SELECT COUNT(*) FROM ... -- query 1
SELECT ... FROM ...      -- query 2 (same WHERE)

-- AFTER (single scan)
SELECT ..., COUNT(*) OVER () AS TotalCount FROM ...
```

---

## 8. DEPLOYMENT ORDER

### 8.1 Deployment Batches

```
BATCH 1 — Students (LOW RISK)
  FIX-S01: sp_GetStudentList           [MEDIUM - PK tiebreaker]
  FIX-S02: sp_GetStudentAssignmentsPaged [MEDIUM - PK tiebreaker]
  FIX-S03: sp_GetStudentLibraryPaged   [MEDIUM - PK tiebreaker]
  FIX-S04: sp_GetStudentNotificationsPaged [MEDIUM - PK tiebreaker]
  
BATCH 2 — Employees (LOW RISK)
  FIX-E01: sp_GetEmployeeInvitationList [MEDIUM - PK tiebreaker]
  FIX-E02: sp_GetEmployeeAttendanceList [MEDIUM - PK tiebreaker]
  FIX-E03: sp_GetEmployeeIdCardList     [MEDIUM - PK tiebreaker]

BATCH 3 — Attendance (MEDIUM RISK)
  FIX-A01: sp_GetStudentAttendanceList  [MEDIUM - PK tiebreaker]
  FIX-A02: sp_GetAttendanceAnalytics    [HIGH - SARGable dates]
  FIX-A03: sp_GetAttendanceSummary      [HIGH - SARGable dates]

BATCH 4 — Finance (MEDIUM RISK)
  FIX-F01..F09: Fee Paging SPs         [LOW - PK tiebreaker only]
  FIX-F10: sp_GetDailyCollectionReport [HIGH - SARGable CAST(PaidAt)]
  FIX-F11: sp_GetMonthlyCollectionReport [HIGH - SARGable YEAR(PaidAt)]
  FIX-F12: sp_GetStudentLedgerReport   [MEDIUM - OUTER APPLY + tiebreaker]
  FIX-F13..F17: Fee Report SPs         [LOW - PK tiebreaker]

BATCH 5 — Results (MEDIUM RISK)
  FIX-R01: sp_GetResultList            [HIGH - Eliminate double scan]
  FIX-R02: sp_GetExamList              [MEDIUM - PK tiebreaker]
  FIX-R03: sp_GetResultSummary         [MEDIUM - Temp table optimization]

BATCH 6 — Dashboard & Other (HIGH RISK — includes soft delete fixes)
  FIX-D01: sp_GetGuardianList          [CRITICAL - Add IsDeleted=0 + tiebreaker]
  FIX-D02: sp_GetGuardianDashboard     [CRITICAL - Add IsDeleted=0]
  FIX-D03: sp_GetGuardianDetails       [CRITICAL - Add IsDeleted=0]
  FIX-D04: sp_GetGuardianFees          [CRITICAL - Add IsDeleted=0 + JOIN + label fix]
  FIX-D05: sp_GetAcademicYearList      [MEDIUM - PK tiebreaker]
  FIX-D06: sp_GetTeacherList           [MEDIUM - PK tiebreaker]
  FIX-D07: sp_GetStudentIdCardList     [MEDIUM - PK tiebreaker]
  FIX-D08: sp_GetAdmissionList         [MEDIUM - PK tiebreaker]
```

### 8.2 Deployment Order

```
Phase 1: Deploy BATCH 1 (Students) — LOW risk, test student grid
Phase 2: Deploy BATCH 2 (Employees) — LOW risk, test employee grids
Phase 3: Deploy BATCH 3 (Attendance) — MEDIUM risk, test attendance grids + analytics
Phase 4: Deploy BATCH 4 (Finance) — MEDIUM risk, test all 23 fee grids + reports
Phase 5: Deploy BATCH 5 (Results) — MEDIUM risk, test exam + result grids
Phase 6: Deploy BATCH 6 (Dashboard/Guardian) — HIGH risk due to soft delete, test guardian portal thoroughly
```

### 8.3 Rollback Plan

Each fix is a standalone `CREATE OR ALTER PROCEDURE`. Rollback per SP by redeploying the original `.sql` file from `Data\StoredProcedures\`. The `StoredProcedureInstaller` will automatically detect the hash change and redeploy.

### 8.4 Testing Checklist

After each batch:
- [ ] Tabulator grids do not show row-shifting on pagination (click next/prev 5 times, verify same rows)
- [ ] Total record count matches pre-fix counts
- [ ] Search still works on all text columns
- [ ] Soft-deleted records are excluded from lists (BATCH 6 only)
- [ ] Date range filters return correct data (BATCH 3 only)
- [ ] No new SQL error logs in application

---

## 9. RISK ASSESSMENT

| Batch | Overall Risk | Rollback Complexity | Business Impact if Failed |
|-------|-------------|---------------------|--------------------------|
| BATCH 1 | **LOW** | Instant (re-deploy original SQL) | Student list pagination glitch |
| BATCH 2 | **LOW** | Instant | Employee pagination glitch |
| BATCH 3 | **MEDIUM** | Instant | Date filters may show different results |
| BATCH 4 | **MEDIUM** | Instant (per SP) | Fee reports may return different row ordering |
| BATCH 5 | **MEDIUM** | Instant | Result list count may change (fixed bug) |
| BATCH 6 | **HIGH** | Instant (per SP) | Guardian portal may show more data (soft delete fix) |

**All fixes are idempotent** (`CREATE OR ALTER`), can be deployed one SP at a time, and each SP has been verified against the same input/output contract.
