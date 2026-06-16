# MULTI-CLASS EXAM EXECUTION AUDIT

**Date**: 2026-06-16  
**Mode**: Read-only code trace  
**Scenario**: One "Half Yearly Examination 2026" spanning Class 6, 7, 8, 9 (Science/BusinessStudies/Humanities), 10 (Science/BusinessStudies/Humanities)

---

## SECTION A — Multi-Class Supported? **NO**

### Exam Entity Design Limitation

| Field | Type | File/Line | Issue |
|-------|------|-----------|-------|
| `Exam.ClassId` | `int` (required) | `ExamEntities.cs:126` | Single `int`, not `ICollection<int>`. One exam = one class by entity design. |
| `Exam.SectionId` | `int?` | `ExamEntities.cs:129` | Nullable section filter — per-exam, not per-class |
| `Exam.StudentGroupId` | `int?` | `ExamEntities.cs:131` | Nullable group filter — per-exam, not per-class |

**Architecture Flag**: The `Exam` entity is intrinsically single-class. The comment on line 125 reads: `/// <summary>Required: the primary class this exam belongs to.</summary>`. To serve 9 class/group combinations, you would need **9 separate Exam records**, not one.

However, some code paths (`ResultCalculationService`, `AdmitCardService`) were written as if multi-class were supported — a clear architectural contradiction.

---

## SECTION B — Multi-Section Supported? **YES**

`Exam.SectionId = int?` is nullable. When `null`, all sections under the class are included. The result calculation, merit calculation, and mark entry systems all support section-level granularity.

---

## SECTION C — Multi-Group Supported? **NO**

| Barrier | File | Line | Detail |
|---------|------|------|--------|
| `Exam.StudentGroupId` is single `int?` | `ExamEntities.cs` | 131 | One exam = one group. Need separate exam per group. |
| `ExamSubject` unique index excludes group | `SchoolDbContext.cs` | 196 | `IX_ExamSubjects_ExamId_SubjectId` — no `StudentGroupId` column in index |
| Curriculum generation has no group filter on existing check | `ExamService.cs` | 489-493 | `GenerateExamSubjectsFromCurriculumAsync` wipes ALL subjects for exam, regardless of group |

To have "Class 9 Science Mathematics" and "Class 9 BusinessStudies Mathematics" simultaneously under one exam, you'd need separate `ExamSubject` entries with matching `StudentGroupId` — but the unique index prevents it and the generation code doesn't support it.

---

## SECTION D — Result Generation Safe? **YES** (with 1 exception)

### Correct: Multi-class processing
`ResultCalculationService.CalculateExamResultsAsync` at lines 95-100:

```csharp
var classIds = await _examResultRepository.Query()
    .Include(r => r.Student)
    .Where(r => r.ExamId == examId)
    .Select(r => r.Student.ClassId)
    .Distinct()
    .ToListAsync();  // ✅ .Distinct().ToListAsync() — collects ALL classes
```

Then iterates all students (line 102-104), processes each student's exam result (line 116-123), and deletes old results scoped to the current student set (line 127-130).

### Exception: Destructive recalculation
`CalculateExamResultsAsync` at line 130:

```csharp
_examResultRepository.RemoveRange(existingResults);
```

All existing `StudentExamResult` records for the exam are deleted and re-inserted. This loses `PublishedAt` values and any manually set fields. If you recalculate Class 6 results, Class 7-10 results are also deleted and rebuilt — a destructive cascade across all classes.

---

## SECTION E — Merit Calculation Safe? **CRITICALLY NO**

### CRITICAL BUG: FirstOrDefaultAsync truncates to one class

`MeritCalculationService.RecalculateMeritPositionsAsync` at lines 116-120:

```csharp
var classId = await _examResultRepository.Query()
    .Include(r => r.Student)
    .Where(r => r.ExamId == examId)
    .Select(r => r.Student.ClassId)
    .FirstOrDefaultAsync();  // ❌ Gets ONLY the first class in DB order
```

If Class 6, 7, 8, 9, 10 all have students with results under the same ExamId, only the class that appears first in the database (lowest StudentId or DB sort order) gets Class/Section merit positions calculated.

Lines 124-125:
```csharp
await CalculateClassMeritPositionsAsync(examId, classId);     // Only 1 class
await CalculateSectionMeritPositionsAsync(examId, classId);   // Only 1 class
await CalculateGroupMeritPositionsAsync(examId);               // ✅ All groups OK
```

**Impact**: Class 6 gets `Position` and `ClassPosition` populated. Classes 7-10 retain stale/zero values. Report cards, tabulation sheets, and transcript positions for those classes are wrong.

### Secondary: School merit = section position

`MeritCalculationService.GetMeritListAsync` with `MeritCategory.School` at line 198 returns `result.Position` (section position), not a cross-class school rank.

---

## SECTION F — Promotion Safe? **NO**

### Hardcoded sequential class ID assumption

`PromotionService.ProcessClassPromotionAsync` at line 140:

```csharp
var nextClassId = classId + 1;
```

| Class | Current ID | nextClassId (computed) | Correct Next |
|-------|-----------|----------------------|--------------|
| 6 | 6 | 7 | 7 ✅ |
| 7 | 7 | 8 | 8 ✅ |
| 8 | 8 | 9 | 9 ✅ |
| 9 | 9 | 10 | 10 ✅ |
| 9 Science | 9 | 10 | 10 ✅ |
| 10 | 10 | 11 | — (final class) |

✅ Works if class IDs are 6→7→8→9→10. However, this breaks if:
- Class IDs are non-sequential (e.g., 1, 2, 5, 10)
- Multiple groups within a class (Class 9 Science → Class 10 Science, not Class 10 generally)
- A class ID is skipped (e.g., no Class 9 in DB, Class 8 should go to Class 10)

### Student.ClassId never updated

`ProcessClassPromotionAsync` creates `PromotionHistory` records (lines 142-154) but **never updates `Student.ClassId`** — the promotion is recorded but not executed at the entity level.

---

## SECTION G — Database Risks

| # | Risk | Severity | File | Line |
|---|------|----------|------|------|
| G1 | **`Exam.ClassId` single int — cannot store multi-class** | **ARCHITECTURAL** | `ExamEntities.cs` | 126 |
| G2 | **`ExamSubject.ClassId` declared in entity but MISSING from database** | **CRITICAL** | `ExamEntities.cs` vs migration snapshot | 154 |
| G3 | **Unique index `IX_ExamSubjects_ExamId_SubjectId` excludes `ClassId`** — one subject per exam regardless of class | **HIGH** | `SchoolDbContext.cs` | 196 |
| G4 | **Unique index `IX_Marks_ExamId_StudentId_SubjectId` excludes `ClassId`** — same student can't have marks for same subject in different classes | **HIGH** | `SchoolDbContext.cs` | 193 |
| G5 | **Unique index `IX_StudentSubjectResults_ExamId_StudentId_SubjectId` excludes `ClassId`** — same issue as G4 | **HIGH** | `SchoolDbContext.cs` | 194 |
| G6 | **Unique index `IX_StudentExamResults_ExamId_StudentId` excludes `ClassId`** — same issue as G4 | **HIGH** | `SchoolDbContext.cs` | 195 |
| G7 | **`ExamService.CreateExamAsync` does NOT set `ExamSubject.ClassId`** — defaults to 0 | **HIGH** | `ExamService.cs` | 107-114 |
| G8 | **`ExamService.UpdateExamAsync` does NOT set `ExamSubject.ClassId`** — defaults to 0 | **HIGH** | `ExamService.cs` | 159-166 |
| G9 | **`ExamSubjectService.SetupSubjectsAsync` does NOT set `ExamSubject.ClassId`** — defaults to 0 | **HIGH** | `ExamSubjectService.cs` | 178-191 |
| G10 | **`GenerateExamSubjectsFromCurriculumAsync` wipes ALL `ExamSubject` records before inserting** — calling for Class 7 destroys Class 6's subjects | **CRITICAL** | `ExamService.cs` | 489-493 |
| G11 | **No FK from `Exam.AcademicYearId` to `AcademicYears`** | **HIGH** | `SchoolDbContext.cs` (Exam config) | — |
| G12 | **4 denormalized fields on Marks/StudentExamResult/StudentSubjectResult lack FKs** | **MEDIUM** | Model snapshots | — |

---

## SECTION H — Required Fixes Before Production

### Blocking (Must Fix — System Cannot Function for Multi-Class Without)

| # | Fix | File(s) | Lines | Effort | Risk |
|---|-----|---------|-------|--------|------|
| H1 | **Decide multi-class architecture**: either (a) accept one-exam-one-class and remove multi-class code paths, or (b) change `Exam.ClassId` to many-to-many via a join table | `ExamEntities.cs`, `SchoolDbContext.cs` | 126 | 2-3 days | High |
| H2 | **Fix `MeritCalculationService.FirstOrDefaultAsync`** — iterate all classes, not just the first | `MeritCalculationService.cs` | 116-126 | 2 hrs | Low |
| H3 | **Fix subject wipe in `GenerateExamSubjectsFromCurriculumAsync`** — scope delete to current class only | `ExamService.cs` | 489-493 | 1 hr | Low |
| H4 | **Fix `ExamService.CreateExamAsync`** to set `ExamSubject.ClassId` | `ExamService.cs` | 107-114 | 30 min | Low |
| H5 | **Fix `ExamService.UpdateExamAsync`** to set `ExamSubject.ClassId` | `ExamService.cs` | 159-166 | 30 min | Low |
| H6 | **Fix `ExamSubjectService.SetupSubjectsAsync`** to set `ExamSubject.ClassId` | `ExamSubjectService.cs` | 178-191 | 30 min | Low |
| H7 | **Add `ExamSubject.ClassId` colum**n to database via migration | New migration | — | 2 hrs | Medium |
| H8 | **Update unique index `IX_ExamSubjects_ExamId_SubjectId` to include `ClassId`** | `SchoolDbContext.cs` + migration | 196 | 2 hrs | Medium |
| H9 | **Fix `ExamService.UpdateExamAsync` subject removal** — scope delete to current class | `ExamService.cs` | 152-155 | 1 hr | Low |

### High Priority (Correctness)

| # | Fix | File | Lines | Effort |
|---|-----|------|-------|--------|
| H10 | Fix `PromotionService` sequential `classId + 1` — resolve next class from `SortOrder` lookup | `PromotionService.cs` | 140 | 3 hrs |
| H11 | Fix `PromotionService` to update `Student.ClassId` after promotion | `PromotionService.cs` | 140-165 | 2 hrs |
| H12 | Fix destructive recalculation — preserve `PublishedAt` across recalculations | `ResultCalculationService.cs` | 127-134 | 2 hrs |
| H13 | Add class-level scoping to `PublishResultsAsync` if per-class publishing is needed | `ResultPublicationService.cs` | 97-100, 173-175 | 3 hrs |
| H14 | Add class filter to `GetTabulationSheetAsync` subject display | `ResultAnalyticsService.cs` | 117-123 | 1 hr |
| H15 | Add class filter to `GetSubjectAnalysisAsync` | `ResultAnalyticsService.cs` | 161 | 1 hr |

---

## SECTION I — Production Readiness Score

### Single-Exam-Per-Class (current architecture): **82/100**
This is what the system was designed for. The single-class exam workflow works correctly:
- Exam creation ✅
- Subject assignment ✅ (exam serves one class)
- Mark entry ✅
- Result calculation ✅
- Merit calculation ✅ (no multi-class to break)
- Publication ✅
- Admit card ✅
- Tabulation ✅
- Transcript ✅
- Promotion ⚠️ (classId + 1 still broken for non-sequential IDs)

### Multi-Class-Per-Exam (the requested scenario): **28/100**
The system is architecturally incompatible with one-exam-multiple-classes:
- Entity design blocks it 🚫
- Database unique indexes block it 🚫
- Service code partially supports it but with critical bugs 🐛
- Merit calculation has a critical FirstOrDefault bug that corrupts data 🚫

### Verdict
**Do NOT attempt one-exam-multiple-classes** without a major architectural refactor. The system design is **one exam = one class (+ optional group/section filter)**. For the requested 9 class/group combinations, create **9 separate Exam records** sharing the same name "Half Yearly Examination 2026" but with different `ClassId`/`StudentGroupId` values. This is the safe, tested path.

### How to Create One Exam Per Class (Recommended Workaround)

Create **9 exams**, all named "Half Yearly Examination 2026":

| # | Name | ClassId | StudentGroupId | Class |
|---|------|---------|---------------|-------|
| 1 | Half Yearly Examination 2026 | 6 | null | Class 6 |
| 2 | Half Yearly Examination 2026 | 7 | null | Class 7 |
| 3 | Half Yearly Examination 2026 | 8 | null | Class 8 |
| 4 | Half Yearly Examination 2026 | 9 | 1 (Science) | Class 9 Science |
| 5 | Half Yearly Examination 2026 | 9 | 2 (BusinessStudies) | Class 9 BusinessStudies |
| 6 | Half Yearly Examination 2026 | 9 | 3 (Humanities) | Class 9 Humanities |
| 7 | Half Yearly Examination 2026 | 10 | 1 (Science) | Class 10 Science |
| 8 | Half Yearly Examination 2026 | 10 | 2 (BusinessStudies) | Class 10 BusinessStudies |
| 9 | Half Yearly Examination 2026 | 10 | 3 (Humanities) | Class 10 Humanities |

Exceptions to handle:
- **Duplicate name check**: `ExamService.CreateExamAsync` (line 82) checks `Name + AcademicYearId`, not `Name + AcademicYearId + ClassId + StudentGroupId`. Creating 9 exams with the same name will fail on the second one. **Fix**: Add composite uniqueness check or remove the duplicate name guard.
- **Listing/grouping**: Dashboard and list views will show 9 separate exam rows. Need grouping by name to show as one logical exam.
- **Report cards**: Teachers must enter marks per exam record. No cross-class aggregation without custom code.

---

## APPENDIX: CODE PATH SUMMARY

| Pipeline Stage | Multi-Class Safe? | Key Barrier | Fix Required? |
|---------------|-------------------|-------------|---------------|
| Exam CRUD | ❌ No | `Exam.ClassId` = single int | Architecture decision |
| Subject setup | ❌ No | Wipes all subjects; no ClassId set | Yes — H2, H4-9 |
| Exam schedule | ⚠️ Partial | Unique index excludes ClassId | Yes — H8 |
| Mark entry | ❌ No | Unique index excludes ClassId | Yes — H8 |
| Result calculation | ✅ Yes | Distinct().ToListAsync() works | No |
| Merit calculation | ❌ **CRITICAL** | FirstOrDefaultAsync single-class | Yes — H2 |
| Result publication | ❌ No | All-or-nothing exam-level | Yes — H13 |
| Admit card | ✅ Yes | Distinct classIds from schedules | No |
| Tabulation | ⚠️ Partial | Subject display lacks class filter | Yes — H14-15 |
| Transcript | ✅ Yes | Per-student, per-year | No |
| Promotion | ❌ No | ClassId + 1 sequential assumption | Yes — H10-11 |
| Database | ❌ No | Missing ClassId on ExamSubject; unique indexes exclude ClassId | Yes — H1, H3, H7-9 |

---

*End of Multi-Class Exam Execution Audit*
