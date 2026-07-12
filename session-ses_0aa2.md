# Enterprise school system roadmap adjustment

**Session ID:** ses_0aa2e13f9ffeJYfY4IN1jIvMEz
**Created:** 7/12/2026, 4:14:08 PM
**Updated:** 7/12/2026, 5:15:40 PM

---

## User

This audit is well structured and, overall, I agree with the direction. Based on your architecture, project size, and your goal of building an **enterprise-grade Bangladesh School Management System**, I would adjust the roadmap slightly.

Your project already has a mature layered architecture (Controllers → Services → Repositories → UnitOfWork → EF Core → SQL Server), around 140+ entities, 100+ services, and dedicated modules for Exams, Results, Promotions, Attendance, Employees, and RBAC.  The feature inventory and Result/Promotion architecture also show that many of the core capabilities already exist and should be extended rather than replaced.  

---

# Overall Assessment

I would rate the audit:

| Area                       | Rating                  |
| -------------------------- | ----------------------- |
| Existing Feature Analysis  | 10/10                   |
| Architecture Understanding | 10/10                   |
| Gap Identification         | 9.8/10                  |
| Enterprise Direction       | 10/10                   |
| Priority Order             | 7/10 (needs adjustment) |

The only thing I would change is the execution order.

---

# I would NOT do Phase A first.

Instead I recommend:

```
Phase 0
↓

Phase B
↓

Phase A

↓

Phase C
```

not

```
Phase A

↓

Phase B
```

---

# Why?

Architecture compliance does **not** add value for users.

The users don't care if

```
ViewBag

↓

DTO
```

They care about

* Exam Wizard

* Promotion Wizard

* Teacher Dashboard

* Automation

* One-click Publish

* Bulk Report Cards

These are what make the product feel enterprise.

---

# My Recommended Roadmap

---

# Phase 0 — Protect the Existing System

Duration:
2–3 days

Do only critical fixes.

* Fix ResultPublicationService compilation errors.
* Ensure all stored procedures compile.
* Fix orphaned SP wiring if they block functionality.
* Resolve any build-breaking issues.
* Add missing tests around Exam → Result → Promotion.

Do **not** spend time replacing Bootstrap or ViewBag at this stage.

---

# Phase 1 — Enterprise Workflow (Highest ROI)

This is the biggest transformation.

Implement:

## 1. Exam Wizard

This becomes

```
The Entry Point
```

Everything starts here.

---

### Step 1

Academic Year

↓

Exam Type

↓

Classes

↓

Term

---

### Step 2

Automatically load

* Subjects
* Components
* Pass Marks
* Full Marks
* Teacher Assignment
* Grade Policy

Buttons

```
Use Previous Exam

Load NCTB Template

Save Template
```

---

### Step 3

Automatically create

* Exam

* Exam Subjects

* Exam Components

* Mark Sessions

* Teacher Assignments

* Schedule (optional)

---

This replaces many manual screens.

---

## 2. Teacher Dashboard

Every teacher logs in and immediately sees

```
Pending Marks

↓

Today's Classes

↓

Assigned Exams

↓

Recent Notifications

↓

Quick Actions
```

No searching.

---

## 3. Result Validation Engine

This is one of the biggest missing enterprise features.

Before Publish:

Run

```
ResultValidationService
```

Checks

✓ Missing Marks

✓ Duplicate Marks

✓ GPA mismatch

✓ Missing Components

✓ Subject not completed

✓ Failed calculations

✓ Practical missing

✓ Attendance missing (optional)

Show

```
PASS

or

BLOCK PUBLISH
```

---

## 4. Publishing Wizard

```
Choose Exam

↓

Validation

↓

Preview

↓

Publish
```

Simple.

---

## 5. Bulk Report Cards

Generate

```
Entire Class

Entire Section

Entire School

ZIP

PDF

Print Queue
```

---

## 6. Promotion Wizard

One wizard.

Internally calls existing services.

Do **not** rewrite.

Simply orchestrate.

---

# Important

Don't rewrite

PromotionService

RollGenerator

SubjectAssignment

AttendanceReset

Instead

Wizard

↓

calls

PromotionService

↓

calls

RollGenerator

↓

calls

SubjectService

↓

calls

ArchiveService

↓

calls

AttendanceService

↓

Done.

---

That keeps the architecture clean.

---

# Phase 2 — Automation

After Wizard.

Implement

### Auto Teacher Assignment

```
TeacherClassSubject

↓

Exam Subject

↓

Teacher
```

---

### Auto Schedule

I would NOT merge this into Exam Wizard.

Reason

Schedule generation is a different bounded context.

Keep

```
Exam Wizard

↓

Exam Created

↓

Generate Schedule
```

Separate.

Much cleaner.

---

Schedule Generator becomes

```
ScheduleGenerationService
```

with

Teacher conflicts

Room conflicts

Holiday conflicts

Friday

Working hours

Exam duration

Gap between exams

---

Much easier to maintain.

---

### Notifications

I would postpone.

Enterprise notification systems become huge.

Need

Email

SMS

Push

In-App

Queue

Retry

Dead-letter

Templates

Localization

Delivery Logs

etc.

Don't let that delay the academic workflow.

---

# Phase 3 — Architecture Compliance

Now users already have

Enterprise UX.

Now fix

Bootstrap

↓

DTO

↓

ViewBag

↓

CSS

↓

JS

↓

Refactoring

No user impact.

---

# Phase 4 — Enterprise Hardening

Performance.

Caching.

Load Tests.

10K students.

Concurrency.

Audit.

Security Review.

Disaster Recovery.

Monitoring.

---

# Answers to Your Questions

## 1. Phase A or Phase B?

**Start with Phase B.**

Only fix architecture issues that block development.

Everything else later.

---

## 2. Should Schedule Generator be inside Exam Wizard?

**No.**

Keep

```
Exam Wizard

↓

Create Exam

↓

Generate Schedule
```

Separate service.

Reason

Scheduling becomes a full engine.

It deserves its own module.

---

## 3. Promotion Wizard

Integrate.

Do **not** duplicate logic.

Wizard becomes

```
UI

↓

Promotion Orchestrator

↓

PromotionPolicyService

↓

Roll Generator

↓

Student Promotion

↓

Subject Assignment

↓

Attendance

↓

Archive
```

Exactly how enterprise ERPs work.

---

## 4. Notifications

Postpone.

Academic workflow first.

Integrate notification infrastructure afterward.

---

## 5. Timeline

Don't deliver one massive feature set.

Use four milestones:

### Milestone 1 (2 weeks)

* Exam Wizard
* Result Validation
* Publishing Wizard
* Bulk Report Cards

### Milestone 2 (2 weeks)

* Promotion Wizard
* Teacher Portal
* Auto Teacher Assignment
* Workflow Dashboard

### Milestone 3 (1 week)

* Architecture cleanup
* DTO conversion
* Remove Bootstrap
* ViewModel cleanup
* Stored procedure wiring

### Milestone 4 (1 week)

* Notifications
* Performance tuning
* Bulk exports
* Load testing
* Final QA

---

# Additional Enterprise Features I Would Add

Since you're targeting a true enterprise-level product, I'd also add:

1. **Workflow State Machine**

   * Draft → Ready → Mark Entry → Submitted → Approved → Published → Archived.

2. **Job Queue**

   * Run long operations (bulk report cards, merit calculation, promotions) asynchronously.

3. **Idempotent Operations**

   * Prevent duplicate result publication or promotion if users click twice.

4. **Feature Flags**

   * Allow schools to enable/disable modules (e.g., Merit List, Auto Promotion, SMS).

5. **Configurable Policies**

   * Make pass marks, grading scales, promotion criteria, attendance thresholds, and report card layouts fully configurable per academic year instead of hardcoding them.

These additions complement your existing architecture and make the system easier to adapt for different schools without code changes.  here 1st one is completeed others do in parallal

---

## Assistant (Build · DeepSeek V4 Flash Free · 9.7s)

_Thinking:_

The user is presenting a comprehensive audit and roadmap recommendation for their School Management System. They've indicated that "Phase 0 — Protect the Existing System" (the first item) is completed, and they want me to proceed with the other phases in parallel.

Let me understand what they're asking:

1. Phase 0 is done (fix ResultPublicationService compilation errors, ensure SPs compile, fix orphaned SP wiring, resolve build-breaking issues, add missing tests)
2. They want me to proceed with the other phases in parallel

Let me look at the current state of the project to understand what's been done and what needs to be done. Let me start by examining the current build status, the ResultPublicationService, and the overall project structure.

Let me first check the build status and understand what's currently broken.

**Tool: todowrite**

**Input:**
```json
{
  "todos": [
    {
      "content": "Phase 0 — Protect the Existing System (already completed per user)",
      "status": "completed",
      "priority": "high"
    },
    {
      "content": "Phase 1 — Enterprise Workflow: Exam Wizard, Teacher Dashboard, Result Validation, Publishing Wizard, Bulk Report Cards",
      "status": "pending",
      "priority": "high"
    },
    {
      "content": "Phase 1 — Promotion Wizard",
      "status": "pending",
      "priority": "high"
    },
    {
      "content": "Phase 2 — Automation: Auto Teacher Assignment, Auto Schedule Generator",
      "status": "pending",
      "priority": "medium"
    },
    {
      "content": "Phase 3 — Architecture Compliance: DTO, Bootstrap removal, ViewBag cleanup, SP wiring",
      "status": "pending",
      "priority": "medium"
    },
    {
      "content": "Phase 4 — Enterprise Hardening: Performance, caching, notifications, security, monitoring",
      "status": "pending",
      "priority": "low"
    }
  ]
}
```

**Output:**
```
[
  {
    "content": "Phase 0 — Protect the Existing System (already completed per user)",
    "status": "completed",
    "priority": "high"
  },
  {
    "content": "Phase 1 — Enterprise Workflow: Exam Wizard, Teacher Dashboard, Result Validation, Publishing Wizard, Bulk Report Cards",
    "status": "pending",
    "priority": "high"
  },
  {
    "content": "Phase 1 — Promotion Wizard",
    "status": "pending",
    "priority": "high"
  },
  {
    "content": "Phase 2 — Automation: Auto Teacher Assignment, Auto Schedule Generator",
    "status": "pending",
    "priority": "medium"
  },
  {
    "content": "Phase 3 — Architecture Compliance: DTO, Bootstrap removal, ViewBag cleanup, SP wiring",
    "status": "pending",
    "priority": "medium"
  },
  {
    "content": "Phase 4 — Enterprise Hardening: Performance, caching, notifications, security, monitoring",
    "status": "pending",
    "priority": "low"
  }
]
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 72.8s)

_Thinking:_

Let me start by understanding the current state of the project. I need to check:

1. Build status
2. ResultPublicationService compilation errors
3. Existing Exam, Result, Promotion architecture
4. What already exists for wizards, teacher dashboard, etc.

Let me launch multiple parallel agents to explore the codebase and understand what exists.

Let me first assess the current state of the project to understand what exists and what needs to be built.

**Tool: task**

**Input:**
```json
{
  "description": "Check build status",
  "prompt": "Run the build command for this .NET project and report back:\n1. The exact build command used\n2. The full build output (success/failure, errors, warnings count)\n3. If there are errors, list the first 20 error messages with file paths and line numbers\n\nThe project is at G:\\PROJECT .NET\\SchoolMS\\clonelatest. Try `dotnet build` first. If that doesn't work, look for a .sln file and build that.",
  "subagent_type": "general"
}
```

**Output:**
```
<task id="ses_0aa2dd5e4ffegGc37E19eCvCjy" state="completed">
<task_result>
**1. Build command:**
```
dotnet build
```

**2. Result: BUILD FAILED** — 10 Errors, 135 Warnings, Time: 00:00:50.34

**3. All 10 errors** (all in one file):

| # | File | Line | Col | Code | Message |
|---|------|------|-----|------|---------|
| 1 | `Services/Implementations/Result/TeacherResultService.cs` | 52 | 14 | CS1061 | `'ITeacherSubjectAssignmentRepository'` has no definition for `GetQueryable` |
| 2 | `Services/Implementations/Result/TeacherResultService.cs` | 54 | 29 | CS1061 | `'TEntity'` has no definition for `Subject` |
| 3 | `Services/Implementations/Result/TeacherResultService.cs` | 76 | 14 | CS1061 | `'IExamRepository'` has no definition for `GetQueryable` |
| 4 | `Services/Implementations/Result/TeacherResultService.cs` | 78 | 27 | CS1061 | `'TEntity'` has no definition for `AcademicYearId` |
| 5 | `Services/Implementations/Result/TeacherResultService.cs` | 78 | 83 | CS1061 | `'TEntity'` has no definition for `ClassId` |
| 6 | `Services/Implementations/Result/TeacherResultService.cs` | 78 | 97 | CS1061 | `'TEntity'` has no definition for `IsLocked` |
| 7 | `Services/Implementations/Result/TeacherResultService.cs` | 84 | 14 | CS1061 | `'IMarkEntryRepository'` has no definition for `GetQueryable` |
| 8 | `Services/Implementations/Result/TeacherResultService.cs` | 86 | 44 | CS1061 | `'TEntity'` has no definition for `ExamId` |
| 9 | `Services/Implementations/Result/TeacherResultService.cs` | 86 | 77 | CS1061 | `'TEntity'` has no definition for `SubjectId` |
| 10 | `Services/Implementations/Result/TeacherResultService.cs` | 86 | 93 | CS1061 | `'TEntity'` has no definition for `EnteredByTeacherId` |

**Root cause**: `TeacherResultService.cs` is a generic class (uses `TEntity`) trying to call `GetQueryable()` which doesn't exist on those repository interfaces, and accessing entity properties (`Subject`, `AcademicYearId`, etc.) on a generic type parameter without a type constraint. This file has **44 pre-existing syntax errors** noted in the AGENTS.md technical debt list.
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 122.5s)

_Thinking:_

So there are 10 build errors, all in `TeacherResultService.cs`. Let me now explore the existing codebase to understand:

1. The ResultPublicationService (mentioned as having 44 syntax errors)
2. The existing Exam, Result, Promotion architecture
3. What repositories, services, controllers, and views already exist

Let me launch multiple parallel agents to explore the codebase.

**Tool: task**

**Input:**
```json
{
  "description": "Explore Exam module",
  "subagent_type": "explore",
  "prompt": "Very thorough exploration of the Exam module in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nI need to understand the complete Exam module architecture. Please find and report:\n\n1. All Exam-related entities (look in Entities folders, search for \"Exam\" in entity files)\n2. All Exam-related DTOs\n3. All Exam-related repository interfaces and implementations\n4. All Exam-related service interfaces and implementations\n5. All Exam-related controllers\n6. All Exam-related views (list the view files)\n7. All Exam-related stored procedures\n8. The Exam module folder structure\n\nAlso look for:\n- ExamType, ExamSubject, ExamComponent, ExamSchedule entities\n- MarkEntry entities and related\n- Any existing wizard-like functionality\n\nSearch in: G:\\PROJECT .NET\\SchoolMS\\clonelatest\n\nReturn the full list of files found and a summary of the architecture."
}
```

**Output:**
```
<task id="ses_0aa2cb2deffe1EL1CqKViGigqZ" state="completed">
<task_result>
Now I have a comprehensive view. Here is the complete report.

---

# EXAM MODULE ARCHITECTURE REPORT

## 1. FOLDER STRUCTURE

```
Controllers/
  Exam/                           (7 controllers)
    AdmitCardController.cs
    ExamComponentsController.cs
    ExamController.cs
    ExamRoutineController.cs
    ExamScheduleController.cs
    ExamSubjectController.cs
    SubjectMarkStructureController.cs
  Result/                         (13 controllers — Exam+Result combined)
    AdminResultController.cs
    ExamAdminController.cs
    ExamWizardController.cs
    MarksController.cs
    MeritListController.cs
    PromotionController.cs
    PromotionWizardController.cs
    PromotioSessionController.cs
    ReportCardController.cs
    ResultController.cs
    ResultManagementController.cs
    TeacherResultController.cs
    TranscriptController.cs

Data/
  ExamControllerRbacSeeder.cs
  SchoolDbContext.cs              (DbSets for all Exam/Result entities)
  StoredProcedures/
    AdmitCard/
      sp_BulkGenerateAdmitCards.sql
      sp_GenerateAdmitCard.sql
    Exam/                         (16 stored procedures)
      SP_Exam_DashboardSummary.sql
      SP_Exam_GetAllResults.sql
      SP_ExamRoutine_GetClassRoutine.sql
      SP_ExamRoutine_GetGuardianRoutine.sql
      SP_ExamRoutine_GetStudentRoutine.sql
      SP_ExamRoutine_GetTeacherRoutine.sql
      sp_GetExamComponents.sql
      sp_GetExamDashboard.sql
      sp_GetExamList.sql
      sp_GetExamMarkStructure.sql
      sp_GetExamReadinessReport.sql
      sp_GetExamScheduleList.sql
      sp_GetGroupReport.sql
      sp_GetMarksEntryList.sql
      sp_GetSubjectMarkStructure.sql
      sp_SaveSubjectMarkStructure.sql
    Result/                       (13 stored procedures + 5 promotion SPs)
      sp_CalculateExamResults.sql
      sp_CalculateFinalPositions.sql
      sp_CalculateMerit.sql
      sp_CalculateSubjectResults.sql
      sp_GetReportCard.sql
      sp_GetResultList.sql
      sp_GetResultPublicationDashboard.sql
      sp_GetResultSummary.sql
      sp_GetStudentResults.sql
      sp_GetTranscript.sql
      sp_PublishResults.sql
      sp_RecalculateResults.sql
      sp_UnpublishResults.sql
      Promotion/
        sp_BulkPromoteStudents.sql
        sp_GetPromotionCandidates.sql
        sp_GetPromotionDashboard.sql
        sp_GetPromotionHistory.sql
        sp_GetPromotionSessionsPaged.sql

Migrations/                       (12+ Exam/Result-related migrations)
  20260607184702_CompleteExamResultEnhancements.cs
  20260608090926_AddDenormalizedFieldsToResultEntities.cs
  20260608130001_AddMarkEntryComponentValues.cs
  20260609102234_RemoveSubjectComponentAndExamId.cs
  20260611160144_AddExamScheduleClassGroupSection.cs
  20260611163649_UpdateAdmitCardColumns.cs
  20260611164334_UpdateExamSubjectColumns.cs
  20260612062759_AddExamClassSectionGroupFields.cs
  20260615123333_DropLegacyExamSubjectMarkColumns.cs
  20260618184416_AddAllowResultWithDueToSchoolSettings.cs
  20260623180046_AddPhase5PromotionEngine.cs

Models/
  Entities/
    Exam/
      ExamEntities.cs             (7 entity classes)
    Result/
      ResultEntities.cs           (20+ entity classes)
  DTOs/
    Exam/                          (8 DTO files)
    Result/                        (15 DTO files — some Exam-related)
  ViewModels/
    Exam/                          (5 ViewModel files)
    Dashboard/
      ExamControllerDashboardViewModel.cs

Repositories/
  Interfaces/
    Result/                        (13 repository interfaces)
  Implementations/
    Result/                        (13 repository implementations)

Services/
  Interfaces/
    Exam/                          IExamRoutineService, IExamSubjectService, IAdmitCardService
    Result/                        (27 service interfaces — Exam+Result combined)
  Implementations/
    Exam/                          ExamSubjectService, ExamRoutineService, AdmitCardService
    Result/                        (29 service implementations — Exam+Result combined)

Views/
  Exam/                            (7 views)
  ExamComponents/                  (2 views)
  ExamRoutine/                     (7 views: 4 partials + 3 role-specific)
  ExamSchedule/                    (4 views)
  ExamSubject/                     (4 views)
  ExamWizard/                      (1 view)
  AdmitCard/                       (4 views)
  SubjectMarkStructure/            (4 views)
  Result/                          (4 views)
  ResultManagement/                (8 views)
  Dashboard/
    ExamControllerIndex.cshtml
  GuardianPortalPages/
    ExamComparison.cshtml

wwwroot/
  js/
    exam-result-ux.js

Tests/
  Services/
    ExamIntegrationTests.cs
```

---

## 2. EXAM ENTITIES (8 entity classes in `ExamEntities.cs` + ~20 in `ResultEntities.cs`)

### In `Models/Entities/Exam/ExamEntities.cs`:

| Entity | Purpose |
|---|---|
| **ExamType** | Master catalog of exam types (Half Yearly, Annual, Pre-Test, Test, First Terminal, Second Terminal, etc.) with Name, Code, DisplayOrder |
| **ExamConfiguration** | Maps ExamTypes to SchoolClasses with weightage percentages. Defines which exams belong to which class group (e.g., Class 1-2 has First Terminal + Second Terminal + Final) |
| **ExamComponent** | System-wide master catalog of component types (Written, MCQ, Practical, Viva, etc.). Dynamic replacement for hardcoded component columns. Has DefaultFullMarks, DefaultPassMarks, IsPractical, IsOptional |
| **SubjectMarkStructure** | Maps ExamComponent instances to subjects/classes/groups with FullMarks/PassMarks. Dynamic mark distribution engine |
| **Exam** | Main exam entity. Has Name, Term (ExamTerm enum), Status (ResultWorkflowStatus), AcademicYearId, ClassId, SectionId, StudentGroupId, StartsOn, EndsOn, IsLocked. Supports ExamGroupKey for grouping same-named exams across classes |
| **ExamSubject** | Maps subjects to exams with FullMarks/PassMarks/IsOptional, TeacherId, ExamDate, ExamStartTime, ExamDuration, RoomNumber |
| **ExamSchedule** | Date/time/room assignment per exam+subject. Supports ClassId, StudentGroupId, SectionId for group-based scheduling (SSC routine). Has ExamDate, StartsAt, EndsAt, RoomNo, Instructions |
| **AdmitCard** | Generated per student per exam. CardNo, AdmitCardNumber, RollNumber, SeatNumber, IsIssued, IsGenerated, PrintedAt |

### In `Models/Entities/Result/ResultEntities.cs`:

| Entity | Purpose |
|---|---|
| **MarkEntry** | Teacher enters component-wise marks per student/subject/exam. Has WrittenMarks, MCQMarks, CQMarks, PracticalMarks, VivaMarks, LabMarks, OralMarks, AssignmentMarks, ContinuousAssessmentMarks, CompetencyMarks, BehaviourMarks, ParticipationMarks + JSON `ComponentValues` column for unlimited dynamic components. Supports Draft/Submitted workflow |
| **GradingRule** | Bangladesh grading system: Grade, MinMarks, MaxMarks, GradePoint |
| **ResultPublication** | Controls when exam results are published. Tracks PublishedAt, ApprovedByUserId, IsLocked |
| **ResultLock** | Prevents modifications to published results |
| **StudentSubjectResult** | Per-subject result calculated from MarkEntry after submission. MarksObtained, FullMarks, PassMarks, Grade, GradePoint, IsPassed, IsOptionalSubject, IsReligionSubject |
| **StudentExamResult** | Overall exam result (aggregate of all subjects). TotalMarks, TotalFullMarks, Gpa, Grade, Position, ClassPosition, GroupPosition, IsPassed, FailedSubjectCount, PassedSubjectCount |
| **FinalResult** | Aggregate result for entire academic year. FinalGpa, WeightedTotalMarks, FinalPosition, FinalClassPosition, FinalSectionPosition, FinalGroupPosition, FinalGrade, PromotionStatus, AttendancePercentage, GeneratedRollNumber |
| **ResultAuditLog** | Audit trail for result modifications: OldMarks, NewMarks, ChangeType |
| **ReEvaluationRequest** | Student/guardian mark re-evaluation requests with approval workflow |
| **ResultSetting** | Configurable rules: OptionalSubjectMode, FailSubjectMode, OptionalBonusMaxGPA, MinimumPromotionGPA, AutoCalculateComponentTotal, GpaRoundingPrecision, etc. |
| **ClassPromotionRule** | Per-class promotion rules: MinimumGPA, MaximumFailedSubjects, AllowConditionalPromotion |
| **ResultPolicy** | Academic year result aggregation weights: defines how exam types contribute to weighted GPA |
| **ResultPolicyExamWeight** | Individual exam type weight within a ResultPolicy |
| **RankingRule** | Configurable ranking order with tie-breaker JSON |
| **PromotionPolicy** | Admin-configurable promotion rules per class (GpaBased/PositionBased/Combined) |
| **PromotionPolicyRule** | Individual rule criteria within a PromotionPolicy (AND/OR logic) |
| **PromotionExecution** | Records actual promotion execution with summary counts |
| **RollGenerationConfig** | Per-class roll number assignment strategy |
| **GroupPromotionConfig** | How students are assigned to groups for next class |
| **PromotionHistory** | Tracks student promotions across years |
| **PromotioSession** | Promotion session (referenced by FinalResult & PromotionHistory) |

---

## 3. EXAM-RELATED DTOs

### `Models/DTOs/Exam/` (8 files):
- `ExamComponentDto.cs` — ExamComponentListDto, ExamComponentUpsertDto
- `ExamDashboardDto.cs` — ExamDashboardDto, ExamStatusDistributionDto, ExamPassRateDto
- `ExamDetailsDto.cs` — ExamDetailsDto
- `ExamListDto.cs` — ExamListDto
- `ExamReadinessReportDto.cs` — ExamReadinessReportDto, ExamReadinessIssueDto
- `ExamRoutineDto.cs` — ExamRoutineDto, ExamRoutineViewModel
- `ExamScheduleDto.cs` — ExamScheduleDto
- `ExamSubjectConfigDto.cs` — ExamSubjectConfigDto

### `Models/DTOs/Result/` (15 files — those with Exam/Result relevance):
- `ResultDtos.cs` — ExamUpsertDto, StudentExamResultDto
- `ExamWizardDtos.cs` — ExamWizardStateDto, ExamWizardSubjectDto, ExamWizardComponentDto, ExamWizardCreateRequest, ExamWizardResultDto
- `MarkEntrySheetDto.cs` — MarkEntrySheetDto
- `StudentResultDto.cs` — StudentResultExamDto
- `TeacherResultDtos.cs` — TeacherAssignedExamDto, TeacherPendingExamDto
- `PublicationDashboardDto.cs` — PublicationDashboardExamDto
- `ComponentMarksDto.cs`
- `MarksEntryStudentDto.cs`
- `ReportCardDto.cs`
- `ResultListItemDto.cs`
- `ResultValidationDtos.cs`
- `ServiceResultDtos.cs`
- `PromotioSessionDtos.cs`
- `PromotioReportDtos.cs`
- `PromotionWizardDtos.cs`

### Other DTOs referencing Exams:
- `CalendarWidgetDto.cs` — UpcomingExamDto
- `AcademicCalendarDtos.cs` — CalendarExamScheduleDto
- `AcademicDashboardDtos.cs` — UpcomingExamItem, ExamDistributionItem

---

## 4. EXAM-RELATED VIEWMODELS

### `Models/ViewModels/Exam/` (5 files):
- `ExamViewModels.cs` — ExamFilterOptionViewModel, ExamGroupPerformanceViewModel, ExamListViewModel, ExamGroupViewModel, ExamDashboardViewModel, ExamGroupReportViewModel, ExamGroupDashboardViewModel, ExamCreateEditViewModel, ExamDetailsViewModel
- `ExamViewModelMapper.cs` — Static mapper class
- `ExamSubjectSetupViewModel.cs` — ExamSubjectSetupViewModel
- `SubjectMarkStructureViewModels.cs`
- `AdmitCardViewModels.cs`

### `Models/ViewModels/Dashboard/`:
- `ExamControllerDashboardViewModel.cs`

### `Models/ViewModels/Result/`:
- `ResultViewModels.cs` — ExamIndexViewModel

---

## 5. REPOSITORY INTERFACES AND IMPLEMENTATIONS

### Interfaces (`Repositories/Interfaces/Result/` — 13 interfaces):
- `IExamRepository.cs`
- `IMarkEntryRepository.cs`
- `IGradingRuleRepository.cs`
- `IStudentExamResultRepository.cs`
- `IStudentSubjectResultRepository.cs`
- `IFinalResultRepository.cs`
- `IResultPublicationRepository.cs`
- `IResultAuditLogRepository.cs`
- `IReEvaluationRequestRepository.cs`
- `ITeacherResultRepository.cs`
- `IPromotionHistoryRepository.cs`
- `IPromotioSessionRepository.cs`
- `IClassProgressionRuleRepository.cs`

### Implementations (`Repositories/Implementations/Result/` — 13):
- `ExamRepository.cs` (via `ResultRepository.cs`)
- `MarkEntryRepository.cs`
- `GradingRuleRepository.cs`
- `StudentExamResultRepository.cs`
- `StudentSubjectResultRepository.cs`
- `FinalResultRepository.cs`
- `ResultPublicationRepository.cs`
- `ResultAuditLogRepository.cs`
- `ReEvaluationRequestRepository.cs`
- `TeacherResultRepository.cs`
- `PromotionHistoryRepository.cs`
- `PromotioSessionRepository.cs`
- `ClassProgressionRuleRepository.cs`

**Note:** There is no separate `Repositories/Exam/` folder. Exam repositories live under `Repositories/Result/`. The `IExamRepository` is the primary interface for Exam CRUD operations.

---

## 6. SERVICE INTERFACES AND IMPLEMENTATIONS

### Service Interfaces (`Services/Interfaces/`):

**Under `Exam/`:**
- `IExamRoutineService.cs`
- `IExamSubjectService.cs`
- `IAdmitCardService.cs`

**Under `Result/` (27 interfaces):**
- `IExamService.cs`
- `IExamValidationService.cs`
- `IExamComponentService.cs`
- `IExamWizardService.cs`
- `IMarkEntryService.cs`
- `ITeacherResultService.cs`
- `ISubjectMarkStructureService.cs`
- `IResultPublicationService.cs`
- `IResultCalculationService.cs`
- `IResultPolicyService.cs`
- `IResultAnalyticsService.cs`
- `IResultValidationService.cs`
- `IResultAuthorizationService.cs`
- `IReportCardService.cs`
- `ITranscriptService.cs`
- `IMeritCalculationService.cs`
- `IGradeCalculator.cs`
- `IPassFailPolicy.cs`
- `IComponentAggregator.cs`
- `IAuditLogger.cs`
- `IReEvaluationService.cs`
- `IStudentSubjectFilterService.cs`
- `IRollGenerationService.cs`
- `IPromotionService.cs`
- `IPromotionPolicyService.cs`
- `IPromotionWizardService.cs`
- `IPromotioSessionService.cs`

### Service Implementations (`Services/Implementations/`):

**Under `Exam/`:**
- `ExamSubjectService.cs`
- `ExamRoutineService.cs`
- `AdmitCardService.cs`

**Under `Result/` (29 implementations):**
- `ExamService.cs`
- `ExamValidationService.cs`
- `ExamComponentService.cs`
- `ExamWizardService.cs`
- `MarkEntryService.cs`
- `TeacherResultService.cs`
- `SubjectMarkStructureService.cs` + `SubjectMarkStructureSeeder.cs`
- `ResultPublicationService.cs`
- `ResultCalculationService.cs`
- `ResultPolicyService.cs`
- `ResultAnalyticsService.cs`
- `ResultValidationService.cs`
- `ResultAuthorizationService.cs`
- `ReportCardService.cs`
- `TranscriptService.cs`
- `MeritCalculationService.cs`
- `GradeCalculator.cs`
- `PassFailPolicy.cs`
- `ComponentAggregator.cs`
- `ComponentFieldMapper.cs`
- `AuditLogger.cs`
- `ReEvaluationService.cs`
- `StudentSubjectFilterService.cs`
- `RollGenerationService.cs`
- `PromotionService.cs`
- `PromotionPolicyService.cs`
- `PromotionWizardService.cs`
- `PromotioSessionService.cs`

---

## 7. CONTROLLERS (20 controllers total)

### `Controllers/Exam/` (7):
1. **`ExamController.cs`** — Main exam CRUD, dashboard, group report
2. **`ExamSubjectController.cs`** — Exam subject setup, configuration, schedule
3. **`ExamScheduleController.cs`** — Exam schedule CRUD
4. **`ExamRoutineController.cs`** — Exam routine display for Admin/Teacher/Student/Guardian roles
5. **`ExamComponentsController.cs`** — Exam component catalog CRUD
6. **`AdmitCardController.cs`** — Admit card generation and display
7. **`SubjectMarkStructureController.cs`** — Subject mark structure CRUD and bulk edit

### `Controllers/Result/` (13):
8. **`ExamWizardController.cs`** — Wizard-based exam creation flow
9. **`ExamAdminController.cs`** — Admin exam management dashboard
10. **`AdminResultController.cs`** — Admin result management (publishing, locking, etc.)
11. **`ResultManagementController.cs`** — Teacher-facing result entry dashboard
12. **`TeacherResultController.cs`** — Teacher result entry and submission
13. **`MarksController.cs`** — Marks entry and management
14. **`ResultController.cs`** — Result viewing (student/guardian)
15. **`TranscriptController.cs`** — Transcript generation
16. **`ReportCardController.cs`** — Report card generation
17. **`MeritListController.cs`** — Merit list generation
18. **`PromotionController.cs`** — Student promotion execution
19. **`PromotionWizardController.cs`** — Wizard-based promotion flow
20. **`PromotioSessionController.cs`** — Promotion session management

---

## 8. VIEWS (55+ view files)

| Views Folder | Files | Count |
|---|---|---|
| `Views/Exam/` | Create, Dashboard, Delete, Details, Edit, GroupReport, Index | 7 |
| `Views/ExamComponents/` | Create, Index | 2 |
| `Views/ExamRoutine/` | _RoutineActions, _RoutineDisplay, _RoutinePrint, Admin, Guardian, Student, Teacher | 7 |
| `Views/ExamSchedule/` | Create, Details, Edit, Index | 4 |
| `Views/ExamSubject/` | Edit, Index, Schedule, Setup | 4 |
| `Views/ExamWizard/` | Index | 1 |
| `Views/AdmitCard/` | AdmitCardPdf, Index, MyAdmitCard, View | 4 |
| `Views/SubjectMarkStructure/` | BulkEdit, Create, Details, Index | 4 |
| `Views/Result/` | CreateEdit, Delete, Details, Index | 4 |
| `Views/ResultManagement/` | AdminIndex, MarkEntry, ReEvaluationDashboard, StudentDashboard, StudentIndex, StudentReportCards, StudentTranscript, TeacherEntry | 8 |
| `Views/Dashboard/` | ExamControllerIndex | 1 |
| `Views/GuardianPortalPages/` | ExamComparison | 1 |
| **Total** | | **47** |

---

## 9. STORED PROCEDURES (36 total related to Exam/Result)

### AdmitCard (2):
- `sp_GenerateAdmitCard.sql`
- `sp_BulkGenerateAdmitCards.sql`

### Exam (16):
- `sp_GetExamList.sql` — Paged/filtered exam listing
- `sp_GetExamDashboard.sql` — Dashboard statistics
- `SP_Exam_DashboardSummary.sql` — Summary metrics
- `sp_GetExamComponents.sql` — Component catalog
- `sp_GetExamScheduleList.sql` — Schedule listing
- `sp_GetExamMarkStructure.sql` — Mark structure retrieval
- `sp_GetSubjectMarkStructure.sql` — Per-subject mark structure
- `sp_SaveSubjectMarkStructure.sql` — Save mark structure
- `sp_GetExamReadinessReport.sql` — Readiness check
- `sp_GetGroupReport.sql` — Group performance report
- `sp_GetMarksEntryList.sql` — Marks entry sheet data
- `SP_Exam_GetAllResults.sql` — All results query
- `SP_ExamRoutine_GetClassRoutine.sql` — Class routine
- `SP_ExamRoutine_GetTeacherRoutine.sql` — Teacher routine
- `SP_ExamRoutine_GetStudentRoutine.sql` — Student routine
- `SP_ExamRoutine_GetGuardianRoutine.sql` — Guardian routine

### Result (13):
- `sp_CalculateExamResults.sql`
- `sp_CalculateSubjectResults.sql`
- `sp_CalculateFinalPositions.sql`
- `sp_CalculateMerit.sql`
- `sp_PublishResults.sql`
- `sp_UnpublishResults.sql`
- `sp_RecalculateResults.sql`
- `sp_GetResultList.sql`
- `sp_GetResultSummary.sql`
- `sp_GetStudentResults.sql`
- `sp_GetReportCard.sql`
- `sp_GetTranscript.sql`
- `sp_GetResultPublicationDashboard.sql`

### Promotion (5):
- `sp_GetPromotionCandidates.sql`
- `sp_BulkPromoteStudents.sql`
- `sp_GetPromotionDashboard.sql`
- `sp_GetPromotionHistory.sql`
- `sp_GetPromotionSessionsPaged.sql`

---

## 10. WIZARD FUNCTIONALITY

The Exam module has **two** wizard implementations:

### 1. **ExamWizard** (`Controllers/Result/ExamWizardController.cs` + `ExamWizardService.cs`)
- Located under the `Result/` namespace
- Single wizard view: `Views/ExamWizard/Index.cshtml`
- DTOs: `ExamWizardDtos.cs` — ExamWizardStateDto, ExamWizardSubjectDto, ExamWizardComponentDto, ExamWizardCreateRequest, ExamWizardResultDto
- Purpose: Multi-step exam creation wizard (likely step-by-step exam + subjects + components + schedule creation)

### 2. **PromotionWizard** (`Controllers/Result/PromotionWizardController.cs` + `PromotionWizardService.cs`)
- DTOs: `PromotionWizardDtos.cs`
- Purpose: Wizard-based student promotion flow

---

## 11. JAVASCRIPT

- `wwwroot/js/exam-result-ux.js` — Shared UX enhancements for exam/result pages

---

## 12. RBAC SEEDING

- `Data/ExamControllerRbacSeeder.cs` — Seeds permissions for exam controllers

---

## 13. ARCHITECTURE SUMMARY

### Layered Architecture (Clean Architecture)

```
Views (47 files)
    ↓
Controllers (20 — 7 in Exam/, 13 in Result/)
    ↓
Services (32 implementations — 3 in Exam/, 29 in Result/)
    ↓
Repositories (13 implementations, all under Result/)
    ↓
Stored Procedures (36 — 16 Exam, 13 Result, 5 Promotion, 2 AdmitCard)
    ↓
SQL Server
```

### Entity Groupings:

**Exam Core (ExamEntities.cs):**
ExamType → ExamConfiguration → Exam → ExamSubject → ExamSchedule → AdmitCard
ExamComponent → SubjectMarkStructure

**Result System (ResultEntities.cs):**
MarkEntry → StudentSubjectResult → StudentExamResult → FinalResult
GradingRule, ResultSetting, ResultPolicy, ResultPolicyExamWeight
Publication: ResultPublication, ResultLock, ResultAuditLog
Promotion: PromotionPolicy, PromotionPolicyRule, ClassPromotionRule, PromotionHistory, PromotionExecution, PromotioSession, RollGenerationConfig, GroupPromotionConfig
ReEvaluation: ReEvaluationRequest

### Key Design Decisions:

1. **Dynamic Component System**: `SubjectMarkStructure` (mapping `ExamComponent` + Subject/Class/Group + FullMarks/PassMarks) replaces hardcoded component columns. The `MarkEntry.ComponentValues` JSON column supports unlimited dynamic component values beyond the 12 standard fields.

2. **Multi-Result-Set Architecture**: Dashboard and detail queries use stored procedures returning multiple result sets for performance (eliminating N+1).

3. **Wizard Flows**: Two wizard services — `ExamWizardService` for multi-step exam creation and `PromotionWizardService` for promotion workflow.

4. **Bangladesh NCTB Compliance**: Grading rules follow Bangladesh standard (A+ = 80-100 = 5.00, etc.). Supports Science/Business Studies/Humanities groups. Supports SSC/JSC exam workflows.

5. **Comprehensive Workflow**: Marks are entered in Draft status → Submitted → Calculated → Published → Locked. Full audit trail via `ResultAuditLog`.

6. **Promotion Engine**: Supports GpaBased, PositionBased, and Combined methods. Per-class rules with optional critical subject enforcement and group-based assignment.

7. **Role-Specific Views**: ExamRoutine has separate views for Admin, Teacher, Student, and Guardian roles.
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 66.8s)

_Thinking:_

Excellent! The codebase is incredibly mature. There's already:
- ExamWizardController + ExamWizardService
- PromotionWizardController + PromotionWizardService
- ResultValidationService
- ResultPublicationService
- ReportCardService
- TeacherResultService
- 36 stored procedures for Exam/Result
- 20 controllers
- 32 service implementations
- 47 views

The main build issue is `TeacherResultService.cs` with 10 errors. Let me now look at the specific files that need fixing and understand the existing wizard implementations better.

Let me now:
1. Fix the build errors in TeacherResultService.cs
2. Explore the existing wizard implementations to understand what needs enhancement
3. Look at the Teacher Dashboard, Result Validation, Publishing, and Report Card functionality

**Tool: task**

**Input:**
```json
{
  "description": "Explore Result/Promotion services",
  "subagent_type": "explore",
  "prompt": "Very thorough exploration of the Result and Promotion services in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nI need to understand the existing implementations. Please read and report the full contents or key sections of:\n\n1. `Services/Implementations/Result/TeacherResultService.cs` — especially the constructor, any methods using GetQueryable, and the class declaration\n2. `Services/Implementations/Result/ExamWizardService.cs` — the full file\n3. `Services/Implementations/Result/PromotionWizardService.cs` — the full file\n4. `Services/Implementations/Result/ResultValidationService.cs` — the full file\n5. `Services/Implementations/Result/ResultPublicationService.cs` — the full file\n6. `Services/Implementations/Result/ReportCardService.cs` — the full file\n7. `Services/Implementations/Result/PromotionService.cs` — the full file\n8. `Services/Implementations/Result/RollGenerationService.cs` — the full file\n9. `Services/Implementations/Result/MeritCalculationService.cs` — the full file\n10. `Services/Implementations/Result/ExamWizardService.cs` — the full file\n\nAlso check:\n11. `Services/Interfaces/Result/IExamWizardService.cs`\n12. `Services/Interfaces/Result/IPromotionWizardService.cs`\n13. `Services/Interfaces/Result/IResultValidationService.cs`\n14. `Services/Interfaces/Result/IResultPublicationService.cs`\n15. `Services/Interfaces/Result/IReportCardService.cs`\n16. `Services/Interfaces/Result/ITeacherResultService.cs`\n\nReturn the full contents of each file."
}
```

**Output:**
```
<task id="ses_0aa2ac39fffemnpTG42IeW0c0Z" state="completed">
<task_result>
All 15 files have been read successfully. Here is the full report:

---

## 1. `TeacherResultService.cs` — `Services/Implementations/Result/TeacherResultService.cs` (159 lines)

**Class declaration:** `public class TeacherResultService : ITeacherResultService`

**Namespace:** `SchoolManagementSystem.Services.Implementations.Result`

**Constructor (lines 25-41):** Injects 7 dependencies:
- `IUnitOfWork _uow`
- `ITeacherSubjectAssignmentRepository _subjectAssignmentRepo`
- `IExamRepository _examRepository`
- `IMarkEntryRepository _markEntryRepository`
- `ISubjectRepository _subjectRepository`
- `ISchoolClassRepository _classRepository`
- `ISectionRepository _sectionRepository`

**Single method — `GetDashboardAsync` (lines 43-158):**
- Returns `Task<TeacherResultDashboardDto>`
- Uses `GetQueryable()` on **three** repositories:
  - `_subjectAssignmentRepo.GetQueryable()` — fetches teacher's subject assignments with `.Include(a => a.Subject).Include(a => a.Class).Include(a => a.Section).Include(a => a.Group).Include(a => a.Teacher).ThenInclude(t => t.Employee)`
  - `_examRepository.GetQueryable()` — fetches exams for the academic year with class filter
  - `_markEntryRepository.GetQueryable()` — fetches mark entries by exam IDs, subject IDs, and teacher ID
- All three queries use `.AsNoTracking()`
- Builds a `TeacherResultDashboardDto` with:
  - `TeacherId`, `TeacherName`, `TotalAssignedSubjects`, `TotalAssignedExams`
  - `PendingMarkEntries`, `SubmittedMarkEntries`, `TotalMarkEntries`, `CompletionPercentage`
  - `PendingExams` (List of `TeacherPendingExamDto`) — iterates assignments and exams to compute completion stats
  - `RecentActivity` (last 20 submitted/saved entries)

---

## 2. `ExamWizardService.cs` — `Services/Implementations/Result/ExamWizardService.cs` (301 lines)

**Class declaration:** `public class ExamWizardService : IExamWizardService`

**Namespace:** `SchoolManagementSystem.Services.Implementations.Result`

**Using aliases:**
- `using ClassSubject = SchoolManagementSystem.Models.Entities.Academic.ClassSubject;`
- `using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;`
- `using ExamSubjectEntity = SchoolManagementSystem.Models.Entities.Exam.ExamSubject;`

**Constructor (lines 26-40):** Injects 6 dependencies:
- `IUnitOfWork _uow`
- `IExamService _examService`
- `IExamValidationService _examValidation`
- `ISubjectMarkStructureService _markStructureService`
- `IExamComponentService _componentService`
- `ITeacherAssignmentService _teacherAssignmentService`

**Methods:**

1. **`LoadSubjectsAsync`** (lines 42-97): Loads class subjects with their mark structure components and teacher assignments for the wizard UI.
   - Uses `_uow.Repository<ClassSubject>().Query()` with `.Include(cs => cs.Subject)`.

2. **`LoadPreviousExamTemplateAsync`** (lines 99-112): Loads a previous exam from a prior academic year as a template.

3. **`LoadExamByIdAsync`** (lines 114-176): Loads a full exam by ID with ExamSubjects, Subjects, Teachers, Classes, and schedules.
   - Uses `_uow.Repository<ExamEntity>().Query()` with multiple `.Include()` chains.

4. **`CreateExamsFromWizardAsync`** (lines 178-260): Creates exams from wizard data — iterates class IDs, creates one exam per class with subjects, calls `_examService.CreateExamAsync()`, then assigns teachers to ExamSubject records.

5. **`GetComponentsForSubjectAsync`** (lines 262-284): Gets mark structure components for a subject.

6. **`GetTeacherAssignmentsAsync`** (lines 286-300): Gets teacher-to-subject assignment mapping for the wizard.

---

## 3. `PromotionWizardService.cs` — `Services/Implementations/Result/PromotionWizardService.cs` (132 lines)

**Class declaration:** `public class PromotionWizardService : IPromotionWizardService`

**Namespace:** `SchoolManagementSystem.Services.Implementations.Result`

**Constructor (lines 15-25):** Injects 4 dependencies (all interfaces, no repositories directly):
- `IPromotionPolicyService _promotionPolicyService`
- `IRollGenerationService _rollGenerationService`
- `IClassSubjectMappingService _classSubjectMappingService`
- `IAcademicYearService _academicYearService`

**Methods:**

1. **`GetPreviewAsync`** (lines 27-72): Calls `_promotionPolicyService.EvaluateClassPromotionAsync()` to get evaluations, then builds a `PromotionWizardPreviewDto` with counts of Eligible/Conditional/Failed/Pending students.

2. **`ExecuteAsync`** (lines 74-131): Calls `_promotionPolicyService.ExecutePromotionAsync()`, then optionally runs `_rollGenerationService.GenerateRollsAsync()` (if `AutoGenerateRoll` is true) and `_classSubjectMappingService.SeedMappingsAsync()` (if `AutoAssignSubjects` is true). Returns `PromotionWizardExecuteResult` with promoted/repeat/failed counts, warnings, and roll count.

---

## 4. `ResultValidationService.cs` — `Services/Implementations/Result/ResultValidationService.cs` (287 lines)

**Class declaration:** `public class ResultValidationService : IResultValidationService`

**Namespace:** `SchoolManagementSystem.Services.Implementations.Result`

**Constructor (lines 23-35):** Injects 5 dependencies:
- `IUnitOfWork _uow`
- `IExamService _examService`
- `IComponentAggregator _componentAggregator`
- `IGradeCalculator _gradeCalculator`
- `ILogger<ResultValidationService> _logger`

**Methods:**

1. **`ValidateAsync`** (lines 37-273): Main validation method. Uses `QueryNoTracking()` extensively:
   - `_uow.Repository<ExamEntity>().QueryNoTracking()` — to get exam
   - `_uow.Repository<ExamSubject>().QueryNoTracking()` — exam subjects with `.Include(es => es.Subject)`
   - `_uow.Repository<Student>().QueryNoTracking()` — active students
   - `_uow.Repository<MarkEntry>().QueryNoTracking()` — all mark entries
   - `_uow.Repository<StudentSubjectResult>().QueryNoTracking()` — existing subject results
   - `_uow.Repository<GradingRule>().QueryNoTracking()` — grading rules
   - `_uow.Repository<StudentExamResult>().QueryNoTracking()` — exam results
   
   Performs 5 validation checks based on request flags:
   - **Missing Marks** (per student per subject)
   - **Duplicate Marks** (more than 1 entry per student+subject)
   - **GPA Mismatch** (recalculates using `_componentAggregator.AggregateAll()` + `_gradeCalculator.CalculateGrade()`)
   - **Incomplete Components** (component marks don't match total)
   - **Missing Subjects** (no mark entries for a subject)
   
   Returns `ResultValidationResultDto` with issues, summary per class, counts.

2. **`ValidatePrePublicationAsync`** (lines 275-286): Convenience method that calls `ValidateAsync` with all checks enabled.

---

## 5. `ResultPublicationService.cs` — `Services/Implementations/Result/ResultPublicationService.cs` (503 lines)

**Class declaration:** `public class ResultPublicationService : IResultPublicationService`

**Namespace:** `SchoolManagementSystem.Services.Implementations.Result`

**Constructor (lines 29-55):** Injects 12 dependencies (!):
- `IUnitOfWork _uow`
- `IMeritCalculationService _meritCalculationService`
- `IExamRepository _examRepository`
- `IMarkEntryRepository _markEntryRepository`
- `IResultPublicationRepository _resultPublicationRepository`
- `IStudentSubjectResultRepository _subjectResultRepository`
- `IStudentExamResultRepository _examResultRepository`
- `IGradingRuleRepository _gradingRuleRepository`
- `IGradeCalculator _gradeCalculator`
- `IComponentAggregator _componentAggregator`
- `IPassFailPolicy _passFailPolicy`
- `IResultAuditLogRepository _auditLogRepository`

**Methods:**

1. **`SubmitExamResultsAsync`** (lines 57-72): Updates mark entries from Draft → Submitted for an exam+class.

2. **`ApproveExamResultsAsync`** (lines 74-87): Updates marks from Submitted/Reviewed → Approved.

3. **`PublishResultsAsync`** (lines 89-239): **Core publishing logic.** Validates workflow (all marks must be Approved/Locked), then:
   - Loads marks with `.Include(x => x.Subject)`
   - Loads grading rules, exam subjects, class subjects
   - Pre-loads existing subject results into a `HashSet<(int StudentId, int SubjectId)>` to avoid N+1
   - For each mark: computes total via `_componentAggregator.AggregateAll()`, calculates grade via `_gradeCalculator.CalculateGrade()`, creates `StudentSubjectResult`
   - Calls `_meritCalculationService.RecalculateMeritPositionsAsync()`
   - Updates `StudentExamResult` records with `PublishedAt` and `Published` status
   - Updates exam status to Published
   - Creates/updates `ResultPublication` record

4. **`ReviewExamResultsAsync`** (lines 241-252): Submitted → Reviewed.

5. **`ApproveReviewedResultsAsync`** (lines 254-265): Reviewed → Approved.

6. **`UnpublishResultsAsync`** (lines 267-286): Published/Unpublished → Unpublished.

7. **`RepublishResultsAsync`** (lines 288-295): Calls `PublishResultsAsync` again.

8. **`GetPublicationStatusAsync`** (lines 297-309): Returns publication status DTO.

9. **`GetResultPublicationsAsync`** (lines 311-329): Lists publications for the active academic year.

10. **`GetStudentResultsAsync`** (lines 331-397): Returns full student portal result with exam and subject results.

11. **`GetAllResultsAsync`** (lines 399-455): Filterable list with `.Include(r => r.Student).ThenInclude(s => s.Class/Section/StudentGroup)`.

12. **`RejectResultsAsync`** (lines 457-464): Uses `ExecuteSqlRawAsync` to update `StudentExamResults` status to Draft.

13. **`GetPublicationDashboardAsync`** (lines 466-470): Delegates to `_resultPublicationRepository.GetPublicationDashboardAsync()`.

14. **`GetAuditLogsAsync`** (lines 472-484): Queries audit logs with filters.

15. **`GetPublicationHistoryAsync`** (lines 487-502): Returns publication history with Projection to DTO.

---

## 6. `ReportCardService.cs` — `Services/Implementations/Result/ReportCardService.cs` (174 lines)

**Class declaration:** `public class ReportCardService : IReportCardService`

**Namespace:** `SchoolManagementSystem.Services.Implementations.Result`

**Constructor (lines 26-43):** Injects 7 dependencies:
- `IUnitOfWork _uow`
- `IPdfGenerator _pdfGenerator`
- `IStudentExamResultRepository _examResultRepository`
- `IMarkEntryRepository _markEntryRepository`
- `ISchoolSettingRepository _schoolSettingRepository`
- `IStudentSubjectFilterService _subjectFilter`
- `IViewRendererService _viewRenderer`

**Methods:**

1. **`IsResultBlockedForStudentAsync`** (lines 45-50): Checks if results are globally blocked AND student has fee dues.

2. **`GenerateReportCardPdfAsync` (single student)** (lines 52-53, 55-95): Overloaded — checks fee block, loads exam result, student, valid subject IDs via `_subjectFilter.GetValidSubjectIdsForStudentAsync()`, loads marks filtered to valid subjects, calls `_pdfGenerator.GenerateSchoolReportCard()`.

3. **`HasFeeDueAsync`** (lines 97-102): Checks for unpaid fee invoices.

4. **`IsResultBlockedAsync`** (lines 104-109): Checks `SchoolSetting.AllowResultWithDue`.

5. **`GetReportCardCountAsync`** (lines 111-120): Counts distinct students with results for an exam.

6. **`GenerateBulkReportCardsAsync`** (lines 122-164): Generates bulk PDFs by rendering each student's report card via `_viewRenderer.RenderToStringAsync()` using `"~/Views/ReportCard/BangladeshFormat.cshtml"` template, combines HTML with page breaks, converts via `_pdfGenerator.GenerateFromHtml()`.

7. **`GenerateReportCardHash`** (lines 166-172): Private — generates a SHA256-based hash for report card verification.

---

## 7. `PromotionService.cs` — `Services/Implementations/Result/PromotionService.cs` (626 lines)

**Class declaration:** `public class PromotionService : IPromotionService`

**Namespace:** `SchoolManagementSystem.Services.Implementations.Result`

**Constructor (lines 29-39):** Injects 4 dependencies:
- `IUnitOfWork _uow`
- `IFinalResultRepository _finalResultRepository`
- `IPromotionHistoryRepository _promotionHistoryRepository`
- `IStudentRepository _studentRepository`

**Methods:**

1. **`CalculatePromotionEligibilityAsync`** (lines 41-50): Fetches student and final result, delegates to internal method.

2. **`CalculatePromotionEligibilityInternal`** (lines 52-112): Core promotion logic:
   - Checks `rules.RequireAllSubjectsPass` → if failed subjects exist, "Repeat"
   - Checks failed subjects vs `rules.MaximumFailedSubjects` → "Repeat"
   - Checks GPA vs `rules.MinimumGPA` → "Repeat"
   - Checks conditional promotion thresholds → "Conditional Promotion"
   - Otherwise → "Promote"

3. **`ProcessClassPromotionAsync`** (lines 114-245): Class-level promotion:
   - Loads all students in class, their final results, and promotion rules
   - Resolves next class via `SchoolClass.SortOrder`
   - Inside transaction: for each student, calculates eligibility, creates `PromotionHistory`, updates student class, handles group requirements (using cached `SchoolSetting`/`SchoolClass` to avoid N+1), calls `RebuildStudentCascadeAsync()`
   - Returns `PromotionResult` with counts

4. **`BulkPromotionAsync`** (lines 247-348): Bulk promotion with override option. Similar logic but targets a specific `ToClassId`.

5. **`GetPromotionRulesAsync`** (lines 350-404): Returns stored rules from `ClassPromotionRule` or computed defaults based on class sort order (Primary: P-5, Junior: 6-8, Secondary: 9+).

6. **`UpdatePromotionRulesAsync`** (lines 406-444): Creates/updates `ClassPromotionRule` entity.

7. **`GetStudentPromotionHistoryAsync`** (lines 446-466): Returns student promotion history with Projection to `PromotionRecord`.

8. **`RebuildStudentCascadeAsync`** (lines 468-537): Batch-updates `AttendanceRecord`, `StudentExamResult`, `StudentSubjectResult`, `FinalResult` via `ExecuteUpdateAsync()`, manages `StudentGroupAssignment`.

9. **`GetPromotionHistoryAsync`** (lines 539-561): Filtered promotion history with Includes.

10. **`GetClassStudentsJsonAsync`** (lines 563-571): Returns student list as JSON-compatible objects.

11. **`ReversePromotionAsync`** (lines 573-625): Reverses a promotion — creates reversal `PromotionHistory` record, restores student class, cascades updates.

---

## 8. `RollGenerationService.cs` — `Services/Implementations/Result/RollGenerationService.cs` (160 lines)

**Class declaration:** `public class RollGenerationService : IRollGenerationService`

**Namespace:** `SchoolManagementSystem.Services.Implementations.Result`

**Constructor (lines 21-24):** Injects single dependency:
- `IUnitOfWork _uow`

**Methods:**

1. **`GetConfigAsync`** (lines 26-32): Retrieves `RollGenerationConfig` by academic year and class.

2. **`SaveConfigAsync`** (lines 34-57): Creates or updates roll generation config.

3. **`GenerateRollsAsync`** (lines 59-159): Main roll generation with 4 strategies:
   - **MeritBased** (default): Orders by `FinalGpa` desc → `WeightedTotalMarks` desc → `FullName` asc
   - **Alphabetical**: Orders by `FullName` asc
   - **PreviousRoll**: Keeps existing roll numbers
   - **Manual**: No changes (returns current rolls without saving)
   
   For MeritBased and Alphabetical, saves changes via `_uow.SaveChangesAsync()`.

---

## 9. `MeritCalculationService.cs` — `Services/Implementations/Result/MeritCalculationService.cs` (215 lines)

**Class declaration:** `public class MeritCalculationService : IMeritCalculationService`

**Namespace:** `SchoolManagementSystem.Services.Implementations.Result`

**Constructor (lines 27-39):** Injects 5 dependencies:
- `IUnitOfWork _uow`
- `IStudentExamResultRepository _examResultRepository`
- `IExamRepository _examRepository`
- `IResultPolicyService _resultPolicyService`
- `IFinalResultRepository _finalResultRepository`

**Methods:**

1. **`CalculateClassMeritPositionsAsync`** (lines 41-51): Sorts results by GPA → Total → Roll, assigns class positions.

2. **`CalculateSectionMeritPositionsAsync`** (lines 53-67): Groups by SectionId, assigns positions within each section.

3. **`CalculateGroupMeritPositionsAsync`** (lines 69-89): Groups by `StudentGroupId`, assigns group positions using `ApplyConfiguredSorting()`.

4. **`RecalculateMeritPositionsAsync`** (lines 91-101): Calls `_examResultRepository.CalculateMeritBySpAsync(exam.Name)` — **uses stored procedure**.

5. **`CalculateFinalResultPositionsAsync`** (lines 108-111): Calls `_finalResultRepository.CalculateFinalPositionsBySpAsync()` — **uses stored procedure**.

6. **`GetMeritListAsync`** (lines 113-153): Returns merit list filtered by category (Class/Section/Group/School) with sorting.

7. **`GetTopPerformersAsync`** (lines 155-175): Returns top N performers by GPA.

8. **Helper methods:**
   - `GetSortedResultsAsync` — loads and sorts results
   - `ApplyConfiguredSorting` — GPA desc → Total desc → Roll asc
   - `ApplyConfiguredFinalSorting` — FinalGPA desc → WeightedTotal desc → SectionId asc
   - `GetPositionForCategory` — maps category to position field

---

## 10. `IExamWizardService.cs` — `Services/Interfaces/Result/IExamWizardService.cs` (14 lines)

```csharp
namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IExamWizardService
{
    Task<List<ExamWizardSubjectDto>> LoadSubjectsAsync(int academicYearId, List<int> classIds, ExamTerm term, CancellationToken ct = default);
    Task<ExamWizardStateDto?> LoadPreviousExamTemplateAsync(int academicYearId, ExamTerm term, CancellationToken ct = default);
    Task<ExamWizardStateDto?> LoadExamByIdAsync(int examId, CancellationToken ct = default);
    Task<ExamWizardResultDto> CreateExamsFromWizardAsync(ExamWizardCreateRequest request, string userId, CancellationToken ct = default);
    Task<List<ExamWizardComponentDto>> GetComponentsForSubjectAsync(int subjectId, int? classId, CancellationToken ct = default);
    Task<Dictionary<int, int?>> GetTeacherAssignmentsAsync(int academicYearId, List<int> classIds, List<int> subjectIds, CancellationToken ct = default);
}
```

---

## 11. `IPromotionWizardService.cs` — `Services/Interfaces/Result/IPromotionWizardService.cs` (9 lines)

```csharp
namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IPromotionWizardService
{
    Task<PromotionWizardPreviewDto> GetPreviewAsync(int fromAcademicYearId, int fromClassId, int? examId, CancellationToken ct = default);
    Task<PromotionWizardExecuteResult> ExecuteAsync(PromotionWizardExecuteRequest request, int userId, CancellationToken ct = default);
}
```

---

## 12. `IResultValidationService.cs` — `Services/Interfaces/Result/IResultValidationService.cs` (9 lines)

```csharp
namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IResultValidationService
{
    Task<ResultValidationResultDto> ValidateAsync(ResultValidationRequest request, CancellationToken ct = default);
    Task<ResultValidationResultDto> ValidatePrePublicationAsync(int examId, CancellationToken ct = default);
}
```

---

## 13. `IResultPublicationService.cs` — `Services/Interfaces/Result/IResultPublicationService.cs` (25 lines)

```csharp
namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IResultPublicationService
{
    Task SubmitExamResultsAsync(int examId, int classId);
    Task ApproveExamResultsAsync(int examId);
    Task PublishResultsAsync(ResultPublishDto dto);
    Task ReviewExamResultsAsync(int examId, int reviewerUserId);
    Task ApproveReviewedResultsAsync(int examId, int approverUserId);
    Task UnpublishResultsAsync(int examId);
    Task RepublishResultsAsync(int examId);
    Task<ResultPublicationDto> GetPublicationStatusAsync(int examId);
    Task<IEnumerable<ResultPublicationDto>> GetResultPublicationsAsync();
    Task<StudentPortalResultDto> GetStudentResultsAsync(int studentId);
    Task<IEnumerable<StudentExamResultDto>> GetAllResultsAsync(int? examId, int? classId, string? status);
    Task<int> RejectResultsAsync(int examId, string updatedBy);
    Task<List<PublicationHistoryEntryDto>> GetPublicationHistoryAsync(int yearId, CancellationToken ct = default);
    Task<(List<PublicationDashboardExamDto> Exams, PublicationDashboardSummaryDto? Summary)> GetPublicationDashboardAsync(int academicYearId, CancellationToken ct = default);
    Task<List<SchoolManagementSystem.Models.Entities.Result.ResultAuditLog>> GetAuditLogsAsync(int? examId, int? studentId, CancellationToken ct = default);
}
```

---

## 14. `IReportCardService.cs` — `Services/Interfaces/Result/IReportCardService.cs` (11 lines)

```csharp
namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface IReportCardService
{
    Task<byte[]?> GenerateReportCardPdfAsync(int examId, int studentId, CancellationToken ct = default);
    Task<byte[]?> GenerateReportCardPdfAsync(int examId, int studentId, bool isAdmin, CancellationToken ct = default);
    Task<bool> IsResultBlockedForStudentAsync(int studentId, CancellationToken cancellationToken = default);
    Task<byte[]> GenerateBulkReportCardsAsync(int examId, int? classId, CancellationToken ct = default);
    Task<int> GetReportCardCountAsync(int examId, int? classId, CancellationToken ct = default);
}
```

---

## 15. `ITeacherResultService.cs` — `Services/Interfaces/Result/ITeacherResultService.cs` (8 lines)

```csharp
namespace SchoolManagementSystem.Services.Interfaces.Result;

public interface ITeacherResultService
{
    Task<TeacherResultDashboardDto> GetDashboardAsync(int teacherId, int academicYearId, CancellationToken ct = default);
}
```

---

## Summary of Key Findings

### Use of `GetQueryable()` / `Query()` patterns:
- **`TeacherResultService`** — uses `GetQueryable()` on 3 repositories (`_subjectAssignmentRepo`, `_examRepository`, `_markEntryRepository`) with `.AsNoTracking()` and `.Include()` chains
- **`ExamWizardService`** — uses `_uow.Repository<T>().Query()` pattern (not repository-specific `GetQueryable()`), with `.AsNoTracking()`
- **`ResultValidationService`** — uses `QueryNoTracking()` exclusively on `_uow.Repository<T>()`
- **`ResultPublicationService`** — uses a mix: `.Query()` on repository instances and `QueryNoTracking()` on `_uow.Repository<T>()`
- **`PromotionService`** — uses `_uow.Repository<T>().Query()` and also uses `_uow.ExecuteInTransactionAsync()` for transactional operations, plus `ExecuteUpdateAsync()` for batch updates
- **`MeritCalculationService`** — uses `QueryNoTracking()` and `Query()` on repositories; delegates to stored procedures via `_examResultRepository.CalculateMeritBySpAsync()` and `_finalResultRepository.CalculateFinalPositionsBySpAsync()`
- **`RollGenerationService`** — uses `_uow.Repository<T>().Query()`
- **`ReportCardService`** — uses `QueryNoTracking()` and `Query()` on repositories

### Notable Architectural Observations:
- **`ResultPublicationService`** is the largest at 503 lines with 12 injected dependencies and the most complex workflow (Submit → Review → Approve → Publish workflow)
- **`PromotionService`** (626 lines) is the largest file with complex transactional logic, caching patterns to avoid N+1, and `ExecuteUpdateAsync()` batch operations
- **`MeritCalculationService`** bridges LINQ-based calculations with stored procedure calls
- **`RollGenerationService`** has the simplest dependency set (single `IUnitOfWork`)
- **`PromotionWizardService`** is a pure orchestration layer — it only depends on other service interfaces, no repositories directly
- **`ExamWizardService`** accesses `IExamService` and other services alongside `IUnitOfWork` for direct repository access
- **`ResultValidationService`** is the only service using `ILogger<T>` injection
- **`ReportCardService`** is the only service integrating PDF generation via `IPdfGenerator` and `IViewRendererService`
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 11.1s)

_Thinking:_

Now I have a comprehensive understanding of the codebase. Let me now look at the specific build errors and fix them. The main issue is `TeacherResultService.cs` which uses `GetQueryable()` on repositories that don't have that method. Let me look at the repository interfaces to understand what methods they actually have, and then fix the service.

Let me also look at the existing controllers and views to understand what needs to be enhanced for the wizards, teacher dashboard, etc.

Let me now:
1. Read the TeacherResultService.cs to understand the exact errors
2. Read the repository interfaces to understand what methods exist
3. Fix the build errors
4. Then proceed with the enterprise workflow enhancements

Let me start by reading the key files.

**Tool: read**

**Input:**
```json
{
  "filePath": "G:\\PROJECT .NET\\SchoolMS\\clonelatest\\Services\\Implementations\\Result\\TeacherResultService.cs"
}
```

**Output:**
```
<path>G:\PROJECT .NET\SchoolMS\clonelatest\Services\Implementations\Result\TeacherResultService.cs</path>
<type>file</type>
<content>
1: using Microsoft.EntityFrameworkCore;
2: using SchoolManagementSystem.Models.DTOs.Result;
3: using SchoolManagementSystem.Models.Entities.Result;
4: using SchoolManagementSystem.Models.Entities.Teachers;
5: using SchoolManagementSystem.Models.Entities.Exam;
6: using SchoolManagementSystem.Models.Enums;
7: using SchoolManagementSystem.Repositories.Interfaces.Result;
8: using SchoolManagementSystem.Repositories.Interfaces.Teachers;
9: using SchoolManagementSystem.Repositories.Interfaces.Academic;
10: using SchoolManagementSystem.Services.Interfaces.Result;
11: using SchoolManagementSystem.UnitOfWork.Interfaces;
12: 
13: namespace SchoolManagementSystem.Services.Implementations.Result;
14: 
15: public class TeacherResultService : ITeacherResultService
16: {
17:     private readonly IUnitOfWork _uow;
18:     private readonly ITeacherSubjectAssignmentRepository _subjectAssignmentRepo;
19:     private readonly IExamRepository _examRepository;
20:     private readonly IMarkEntryRepository _markEntryRepository;
21:     private readonly ISubjectRepository _subjectRepository;
22:     private readonly ISchoolClassRepository _classRepository;
23:     private readonly ISectionRepository _sectionRepository;
24: 
25:     public TeacherResultService(
26:         IUnitOfWork uow,
27:         ITeacherSubjectAssignmentRepository subjectAssignmentRepo,
28:         IExamRepository examRepository,
29:         IMarkEntryRepository markEntryRepository,
30:         ISubjectRepository subjectRepository,
31:         ISchoolClassRepository classRepository,
32:         ISectionRepository sectionRepository)
33:     {
34:         _uow = uow;
35:         _subjectAssignmentRepo = subjectAssignmentRepo;
36:         _examRepository = examRepository;
37:         _markEntryRepository = markEntryRepository;
38:         _subjectRepository = subjectRepository;
39:         _classRepository = classRepository;
40:         _sectionRepository = sectionRepository;
41:     }
42: 
43:     public async Task<TeacherResultDashboardDto> GetDashboardAsync(int teacherId, int academicYearId, CancellationToken ct = default)
44:     {
45:         var dashboard = new TeacherResultDashboardDto
46:         {
47:             TeacherId = teacherId,
48:             TeacherName = ""
49:         };
50: 
51:         var assignments = await _subjectAssignmentRepo
52:             .GetQueryable()
53:             .AsNoTracking()
54:             .Include(a => a.Subject)
55:             .Include(a => a.Class)
56:             .Include(a => a.Section)
57:             .Include(a => a.Group)
58:             .Include(a => a.Teacher)
59:                 .ThenInclude(t => t!.Employee)
60:             .Where(a => a.TeacherId == teacherId && a.AcademicYearId == academicYearId && a.IsActive)
61:             .ToListAsync(ct);
62: 
63:         if (assignments.Count == 0)
64:             return dashboard;
65: 
66:         dashboard.TeacherName = assignments[0].Teacher?.Employee?.FullName ?? "";
67: 
68:         var subjectIds = assignments.Select(a => a.SubjectId).Distinct().ToList();
69:         dashboard.TotalAssignedSubjects = subjectIds.Count;
70: 
71:         var classIds = assignments.Select(a => a.ClassId).Distinct().ToList();
72:         var sectionIds = assignments.Select(a => a.SectionId).Distinct().ToList();
73:         var groupIds = assignments.Where(a => a.GroupId.HasValue).Select(a => a.GroupId!.Value).Distinct().ToList();
74: 
75:         var exams = await _examRepository
76:             .GetQueryable()
77:             .AsNoTracking()
78:             .Where(e => e.AcademicYearId == academicYearId && classIds.Contains(e.ClassId) && e.IsLocked == false)
79:             .ToListAsync(ct);
80: 
81:         var examIds = exams.Select(e => e.Id).ToList();
82: 
83:         var markEntries = await _markEntryRepository
84:             .GetQueryable()
85:             .AsNoTracking()
86:             .Where(m => examIds.Contains(m.ExamId) && subjectIds.Contains(m.SubjectId) && m.EnteredByTeacherId == teacherId)
87:             .ToListAsync(ct);
88: 
89:         var pendingExams = new List<TeacherPendingExamDto>();
90: 
91:         foreach (var assignment in assignments)
92:         {
93:             var matchingExams = exams.Where(e => e.ClassId == assignment.ClassId).ToList();
94: 
95:             foreach (var exam in matchingExams)
96:             {
97:                 var entries = markEntries
98:                     .Where(m => m.ExamId == exam.Id && m.SubjectId == assignment.SubjectId && m.ClassId == assignment.ClassId && m.SectionId == assignment.SectionId)
99:                     .ToList();
100: 
101:                 var completedCount = entries.Count(m => m.Status >= ResultWorkflowStatus.Submitted);
102:                 var totalCount = entries.Count;
103:                 var pendingCount = entries.Count(m => m.Status == ResultWorkflowStatus.Draft);
104: 
105:                 var status = exam.Status switch
106:                 {
107:                     ResultWorkflowStatus.Published => "Published",
108:                     ResultWorkflowStatus.Locked => "Locked",
109:                     ResultWorkflowStatus.Approved => "Approved",
110:                     ResultWorkflowStatus.Submitted => "Submitted",
111:                     _ => "Draft"
112:                 };
113: 
114:                 pendingExams.Add(new TeacherPendingExamDto
115:                 {
116:                     ExamId = exam.Id,
117:                     ExamName = exam.Name,
118:                     SubjectId = assignment.SubjectId,
119:                     SubjectName = assignment.Subject?.Name ?? "",
120:                     ClassId = assignment.ClassId,
121:                     ClassName = assignment.Class?.Name ?? "",
122:                     SectionId = assignment.SectionId,
123:                     SectionName = assignment.Section?.Name ?? "",
124:                     GroupId = assignment.GroupId,
125:                     GroupName = assignment.Group?.Name ?? "",
126:                     StudentCount = totalCount,
127:                     CompletedCount = completedCount,
128:                     Status = status,
129:                     IsReadOnly = exam.Status == ResultWorkflowStatus.Published || exam.Status == ResultWorkflowStatus.Locked
130:                 });
131:             }
132:         }
133: 
134:         dashboard.PendingExams = pendingExams;
135:         dashboard.TotalAssignedExams = pendingExams.Select(p => p.ExamId).Distinct().Count();
136:         dashboard.PendingMarkEntries = pendingExams.Sum(p => p.StudentCount - p.CompletedCount);
137:         dashboard.SubmittedMarkEntries = pendingExams.Sum(p => p.CompletedCount);
138:         dashboard.TotalMarkEntries = pendingExams.Sum(p => p.StudentCount);
139:         dashboard.CompletionPercentage = dashboard.TotalMarkEntries > 0
140:             ? Math.Round((double)dashboard.SubmittedMarkEntries / dashboard.TotalMarkEntries * 100, 1)
141:             : 0;
142: 
143:         var recentActivity = markEntries
144:             .Where(m => m.Status >= ResultWorkflowStatus.Submitted)
145:             .OrderByDescending(m => m.UpdatedAt ?? m.CreatedAt)
146:             .Take(20)
147:             .Select(m => new TeacherRecentActivityDto
148:             {
149:                 Action = m.Status == ResultWorkflowStatus.Submitted ? "Submitted" : "Saved",
150:                 Detail = $"Exam: {exams.FirstOrDefault(e => e.Id == m.ExamId)?.Name ?? "N/A"}, Subject: {assignments.FirstOrDefault(a => a.SubjectId == m.SubjectId)?.Subject?.Name ?? "N/A"}",
151:                 Timestamp = m.UpdatedAt ?? m.CreatedAt
152:             })
153:             .ToList();
154: 
155:         dashboard.RecentActivity = recentActivity;
156: 
157:         return dashboard;
158:     }
159: }

(End of file - total 159 lines)
</content>
```

**Tool: read**

**Input:**
```json
{
  "filePath": "G:\\PROJECT .NET\\SchoolMS\\clonelatest\\Repositories\\Interfaces\\Result\\ITeacherSubjectAssignmentRepository.cs"
}
```

**Error:**
```
File not found: G:\PROJECT .NET\SchoolMS\clonelatest\Repositories\Interfaces\Result\ITeacherSubjectAssignmentRepository.cs
```

**Tool: read**

**Input:**
```json
{
  "filePath": "G:\\PROJECT .NET\\SchoolMS\\clonelatest\\Repositories\\Interfaces\\Result\\IExamRepository.cs"
}
```

**Output:**
```
<path>G:\PROJECT .NET\SchoolMS\clonelatest\Repositories\Interfaces\Result\IExamRepository.cs</path>
<type>file</type>
<content>
1: using SchoolManagementSystem.Models.Entities.Exam;
2: using SchoolManagementSystem.Models.DTOs.Result;
3: using SchoolManagementSystem.Models.DTOs.Exam;
4: 
5: namespace SchoolManagementSystem.Repositories.Interfaces.Result;
6: 
7: public interface IExamRepository : IBaseRepository<Exam>
8: {
9:     Task<IEnumerable<ExamListDto>> GetExamsForAdminAsync(int academicYearId, CancellationToken ct);
10:     Task<ExamDashboardDto> GetDashboardDataAsync(int academicYearId, CancellationToken ct);
11:     Task<List<ExamStatusDistributionDto>> GetStatusDistributionAsync(int academicYearId, CancellationToken ct);
12:     Task<List<ExamPassRateDto>> GetExamPassRatesAsync(int academicYearId, CancellationToken ct);
13:     Task<(IEnumerable<ExamListDto> Items, int TotalCount)> GetExamListAsync(int academicYearId, string? searchTerm, int? status, int pageNumber, int pageSize, string sortColumn, string sortDirection, CancellationToken ct);
14:     Task<ExamDetailsDto?> GetExamDetailsAsync(int examId, CancellationToken ct);
15:     Task<ExamReadinessReportDto> GetExamReadinessReportAsync(int academicYearId, CancellationToken ct);
16: }

(End of file - total 16 lines)
</content>
```

**Tool: read**

**Input:**
```json
{
  "filePath": "G:\\PROJECT .NET\\SchoolMS\\clonelatest\\Repositories\\Interfaces\\Result\\IMarkEntryRepository.cs"
}
```

**Output:**
```
<path>G:\PROJECT .NET\SchoolMS\clonelatest\Repositories\Interfaces\Result\IMarkEntryRepository.cs</path>
<type>file</type>
<content>
1: using SchoolManagementSystem.Models.Entities.Result;
2: using SchoolManagementSystem.Models.DTOs.Result;
3: 
4: namespace SchoolManagementSystem.Repositories.Interfaces.Result;
5: 
6: public interface IMarkEntryRepository : IBaseRepository<MarkEntry>
7: {
8:     Task<List<MarkEntrySheetDto>> GetMarkEntrySheetAsync(int examId, int classId, int sectionId, int subjectId, CancellationToken ct);
9:     Task<List<MarksEntryStudentDto>> GetMarksEntryListAsync(int examId, int classId, int sectionId, int subjectId, CancellationToken ct, int? optionalSubjectId = null);
10: }

(End of file - total 10 lines)
</content>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 10.8s)

_Thinking:_

Now I see the issue. The `IExamRepository` and `IMarkEntryRepository` extend `IBaseRepository<T>`, which likely has `GetQueryable()`. But `ITeacherSubjectAssignmentRepository` is in a different namespace (`Repositories.Interfaces.Teachers`), not `Repositories.Interfaces.Result`. Let me find it.

Also, the `TeacherResultService` uses `GetQueryable()` on all three repositories. The `IExamRepository` and `IMarkEntryRepository` extend `IBaseRepository<T>`, so they should have `GetQueryable()` from the base. But `ITeacherSubjectAssignmentRepository` might not extend `IBaseRepository<T>`.

Let me find the TeacherSubjectAssignment repository interface.

**Tool: glob**

**Input:**
```json
{
  "pattern": "**/ITeacherSubjectAssignmentRepository*"
}
```

**Output:**
```
G:\PROJECT .NET\SchoolMS\clonelatest\Repositories\Interfaces\Teachers\ITeacherSubjectAssignmentRepository.cs
```

**Tool: glob**

**Input:**
```json
{
  "pattern": "**/IBaseRepository*"
}
```

**Output:**
```
No files found
```

**Tool: glob**

**Input:**
```json
{
  "pattern": "**/TeacherSubjectAssignment*"
}
```

**Output:**
```
No files found
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 3.9s)

_Thinking:_

Let me read the ITeacherSubjectAssignmentRepository and find IBaseRepository.

**Tool: read**

**Input:**
```json
{
  "filePath": "G:\\PROJECT .NET\\SchoolMS\\clonelatest\\Repositories\\Interfaces\\Teachers\\ITeacherSubjectAssignmentRepository.cs"
}
```

**Output:**
```
<path>G:\PROJECT .NET\SchoolMS\clonelatest\Repositories\Interfaces\Teachers\ITeacherSubjectAssignmentRepository.cs</path>
<type>file</type>
<content>
1: using SchoolManagementSystem.Models.Entities.Teachers;
2: 
3: namespace SchoolManagementSystem.Repositories.Interfaces.Teachers;
4: 
5: public interface ITeacherSubjectAssignmentRepository : IBaseRepository<TeacherSubjectAssignment> { }

(End of file - total 5 lines)
</content>
```

**Tool: grep**

**Input:**
```json
{
  "pattern": "interface IBaseRepository",
  "include": "*.cs"
}
```

**Output:**
```
Found 1 matches
G:\PROJECT .NET\SchoolMS\clonelatest\Repositories\Interfaces\IGenericRepository.cs:
  Line 5: public interface IBaseRepository<T> where T : class

```

---

## Assistant (Build · DeepSeek V4 Flash Free · 4.4s)

_Thinking:_

Let me read the IBaseRepository (IGenericRepository.cs) to see if it has GetQueryable.

**Tool: read**

**Input:**
```json
{
  "filePath": "G:\\PROJECT .NET\\SchoolMS\\clonelatest\\Repositories\\Interfaces\\IGenericRepository.cs"
}
```

**Output:**
```
<path>G:\PROJECT .NET\SchoolMS\clonelatest\Repositories\Interfaces\IGenericRepository.cs</path>
<type>file</type>
<content>
1: using System.Linq.Expressions;
2: 
3: namespace SchoolManagementSystem.Repositories.Interfaces;
4: 
5: public interface IBaseRepository<T> where T : class
6: {
7:     IQueryable<T> Query();
8:     IQueryable<T> QueryNoTracking();
9:     Task<T?> GetByIdAsync(int id, CancellationToken cancellationToken = default);
10:     Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
11:     Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
12:     Task<bool> AnyAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken = default);
13:     Task<int> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);
14:     Task AddAsync(T entity, CancellationToken cancellationToken = default);
15:     Task AddRangeAsync(IEnumerable<T> entities, CancellationToken cancellationToken = default);
16:     void Update(T entity);
17:     void Remove(T entity);
18:     void RemoveRange(IEnumerable<T> entities);
19:     Task<List<TResult>> ExecuteStoredProcAsync<TResult>(string spName, params object[] parameters) where TResult : class, new();
20: }

(End of file - total 20 lines)
</content>
```

**Tool: grep**

**Input:**
```json
{
  "include": "*.cs",
  "pattern": "GetQueryable"
}
```

**Output:**
```
Found 3 matches
G:\PROJECT .NET\SchoolMS\clonelatest\Services\Implementations\Result\TeacherResultService.cs:
  Line 52:             .GetQueryable()

  Line 76:             .GetQueryable()

  Line 84:             .GetQueryable()

```

---

## Assistant (Build · DeepSeek V4 Flash Free · 26.5s)

_Thinking:_

Now I see the issue clearly. The `IBaseRepository<T>` has `Query()` and `QueryNoTracking()`, but NOT `GetQueryable()`. The `TeacherResultService.cs` is calling `GetQueryable()` which doesn't exist on any of these repository interfaces.

The fix is simple: replace `.GetQueryable()` with `.Query()` (or `.QueryNoTracking()` since they all use `.AsNoTracking()` right after).

Let me also check if there's a `TeacherSubjectAssignment` entity to understand the navigation properties used.

Actually, looking at the code more carefully:
- Line 52: `_subjectAssignmentRepo.GetQueryable()` → should be `_subjectAssignmentRepo.Query()`
- Line 76: `_examRepository.GetQueryable()` → should be `_examRepository.Query()`
- Line 84: `_markEntryRepository.GetQueryable()` → should be `_markEntryRepository.Query()`

All three repositories extend `IBaseRepository<T>` which has `Query()` and `QueryNoTracking()`. The fix is straightforward.

Let me also check if there are any other build errors. The agent said 10 errors all in TeacherResultService.cs. Let me fix them all.

Actually, wait - the build output said 10 errors, all in TeacherResultService.cs. Let me check if there are other issues. Let me also look at the TeacherSubjectAssignment entity to understand the navigation properties.

Actually, the errors are:
1. `'ITeacherSubjectAssignmentRepository'` has no definition for `GetQueryable` — fix: use `Query()` instead
2. `'TEntity'` has no definition for `Subject` — this is about the generic type parameter
3. `'IExamRepository'` has no definition for `GetQueryable` — fix: use `Query()` instead
4. `'TEntity'` has no definition for `AcademicYearId` — generic issue
5. `'TEntity'` has no definition for `ClassId` — generic issue
6. `'TEntity'` has no definition for `IsLocked` — generic issue
7. `'IMarkEntryRepository'` has no definition for `GetQueryable` — fix: use `Query()` instead
8. `'TEntity'` has no definition for `ExamId` — generic issue
9. `'TEntity'` has no definition for `SubjectId` — generic issue
10. `'TEntity'` has no definition for `EnteredByTeacherId` — generic issue

Wait, the error messages mention `'TEntity'` has no definition for `Subject`, `AcademicYearId`, etc. But looking at the code, the file doesn't use `TEntity` as a generic type parameter. The errors are about the entity types returned by `GetQueryable()`.

Actually, looking at the code again, the errors are about the entity types. When you call `_subjectAssignmentRepo.GetQueryable()`, the return type is `IQueryable<TeacherSubjectAssignment>`. The `TeacherSubjectAssignment` entity should have `Subject`, `Class`, `Section`, `Group`, `Teacher` navigation properties.

Wait, the error messages say `'TEntity'` has no definition for `Subject`. This is strange because the code uses concrete types, not generics. Let me re-read the error messages:

1. `'ITeacherSubjectAssignmentRepository'` has no definition for `GetQueryable` — this is because `GetQueryable()` doesn't exist on the interface. The method is called `Query()`.
2. `'TEntity'` has no definition for `Subject` — this is weird because the code uses `a.Subject` where `a` is a `TeacherSubjectAssignment`. Maybe the entity doesn't have a `Subject` navigation property?
3. `'IExamRepository'` has no definition for `GetQueryable` — same issue, should be `Query()`
4-6. Same TEntity issues for AcademicYearId, ClassId, IsLocked on Exam
7. `'IMarkEntryRepository'` has no definition for `GetQueryable` — same issue
8-10. Same TEntity issues for ExamId, SubjectId, EnteredByTeacherId on MarkEntry

Wait, the error messages say `'TEntity'` has no definition for `Subject`. But the code uses `a.Subject` where `a` is a `TeacherSubjectAssignment`. The compiler error says `'TEntity'` which suggests the compiler is interpreting the type as a generic parameter. This could happen if `GetQueryable()` returns `IQueryable<TEntity>` where `TEntity` is unconstrained.

Actually, looking more carefully at the error messages from the build:
- `'ITeacherSubjectAssignmentRepository'` has no definition for `GetQueryable` — this is the first error
- `'TEntity'` has no definition for `Subject` — this is the second error

The second error is likely a cascading error. Since `GetQueryable()` doesn't exist, the compiler doesn't know what type the queryable is, so it treats the result as `IQueryable<TEntity>` where `TEntity` is unconstrained, and then `Subject` doesn't exist on it.

So the root cause is just that `GetQueryable()` doesn't exist. The fix is to replace `GetQueryable()` with `Query()` (or `QueryNoTracking()` since they all use `.AsNoTracking()` right after).

Let me fix this.

Now I understand the issue. `IBaseRepository<T>` has `Query()` and `QueryNoTracking()`, not `GetQueryable()`. Let me fix the build errors.

**Tool: edit**

**Input:**
```json
{
  "filePath": "G:\\PROJECT .NET\\SchoolMS\\clonelatest\\Services\\Implementations\\Result\\TeacherResultService.cs",
  "oldString": "        var assignments = await _subjectAssignmentRepo\n            .GetQueryable()\n            .AsNoTracking()",
  "newString": "        var assignments = await _subjectAssignmentRepo\n            .QueryNoTracking()"
}
```

**Output:**
```
Edit applied successfully.
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 7.6s)

_Thinking:_

Now fix the other two GetQueryable calls.

**Tool: edit**

**Input:**
```json
{
  "filePath": "G:\\PROJECT .NET\\SchoolMS\\clonelatest\\Services\\Implementations\\Result\\TeacherResultService.cs",
  "oldString": "        var exams = await _examRepository\n            .GetQueryable()\n            .AsNoTracking()",
  "newString": "        var exams = await _examRepository\n            .QueryNoTracking()"
}
```

**Output:**
```
Edit applied successfully.
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 3.2s)

**Tool: edit**

**Input:**
```json
{
  "filePath": "G:\\PROJECT .NET\\SchoolMS\\clonelatest\\Services\\Implementations\\Result\\TeacherResultService.cs",
  "oldString": "        var markEntries = await _markEntryRepository\n            .GetQueryable()\n            .AsNoTracking()",
  "newString": "        var markEntries = await _markEntryRepository\n            .QueryNoTracking()"
}
```

**Output:**
```
Edit applied successfully.
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 92.7s)

_Thinking:_

Now let me build to see if the errors are fixed.

**Tool: bash**

**Input:**
```json
{
  "command": "dotnet build 2>&1 | Select-String -Pattern \"error|Error|Build\"",
  "description": "Build to check if errors fixed",
  "workdir": "G:\\PROJECT .NET\\SchoolMS\\clonelatest",
  "timeout": 120000
}
```

**Output:**
```

Build succeeded.
    0 Error(s)



```

---

## Assistant (Build · DeepSeek V4 Flash Free · 9.5s)

_Thinking:_

Build succeeded with 0 errors. Now let me check the warning count.

**Tool: bash**

**Input:**
```json
{
  "description": "Check warning count",
  "command": "dotnet build 2>&1 | Select-String -Pattern \"warning\"",
  "timeout": 120000,
  "workdir": "G:\\PROJECT .NET\\SchoolMS\\clonelatest"
}
```

**Output:**
```

    0 Warning(s)



```

---

## Assistant (Build · DeepSeek V4 Flash Free · 128.2s)

_Thinking:_

Build: 0 errors, 0 warnings. Phase 0 is complete.

Now let me proceed with Phase 1 — Enterprise Workflow. The user said "1st one is completed others do in parallel." So Phase 0 is done, and I should work on the remaining phases in parallel.

Let me now plan the work:

**Phase 1 — Enterprise Workflow:**
1. Exam Wizard — already exists (ExamWizardController + ExamWizardService), needs enhancement
2. Teacher Dashboard — already exists (TeacherResultService + TeacherResultController), needs enhancement
3. Result Validation Engine — already exists (ResultValidationService), needs enhancement
4. Publishing Wizard — already exists (ResultPublicationService), needs enhancement
5. Bulk Report Cards — already exists (ReportCardService.GenerateBulkReportCardsAsync)
6. Promotion Wizard — already exists (PromotionWizardController + PromotionWizardService)

So the core infrastructure already exists! The work is about:
1. Enhancing the existing wizards with better UX
2. Creating/improving the Teacher Dashboard views
3. Creating a proper Publishing Wizard flow
4. Enhancing the Result Validation with a proper UI
5. Making Bulk Report Cards work end-to-end

Let me now explore the existing controllers and views to understand what needs to be enhanced. Let me look at the controllers and views for the wizard, teacher dashboard, etc.

Build: **0 errors, 0 warnings**. Phase 0 complete.

Now let me explore the existing controllers and views to understand what needs enhancement for Phase 1.

**Tool: task**

**Input:**
```json
{
  "description": "Explore wizard controllers/views",
  "subagent_type": "explore",
  "prompt": "Very thorough exploration of the wizard controllers and views in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nPlease read and return the FULL contents of:\n\n1. `Controllers/Result/ExamWizardController.cs`\n2. `Controllers/Result/PromotionWizardController.cs`\n3. `Controllers/Result/TeacherResultController.cs`\n4. `Controllers/Result/AdminResultController.cs`\n5. `Controllers/Result/ResultManagementController.cs`\n6. `Controllers/Result/ReportCardController.cs`\n7. `Controllers/Result/MeritListController.cs`\n8. `Controllers/Result/PromotionController.cs`\n\nAlso read:\n9. `Views/ExamWizard/Index.cshtml`\n10. `Views/PromotionWizard/` (list all files in this directory)\n11. `Views/ResultManagement/` (list all files)\n12. `Views/TeacherResult/` (list all files, if exists)\n\nReturn the full contents of each file."
}
```

**Output:**
```
<task id="ses_0aa272a20ffe1jK5x9roeDLfeR" state="completed">
<task_result>
Here is the complete inventory of all requested files with their full contents.

---

## 1. `G:\PROJECT .NET\SchoolMS\clonelatest\Controllers\Result\ExamWizardController.cs` (146 lines)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Result;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
[RequirePermission("Exam.Manage")]
public class ExamWizardController : Controller
{
    private readonly IExamWizardService _wizardService;
    private readonly IAcademicYearService _academicYearService;
    private readonly ISchoolClassService _classService;
    private readonly IExamService _examService;
    private readonly ILogger<ExamWizardController> _logger;

    public ExamWizardController(
        IExamWizardService wizardService,
        IAcademicYearService academicYearService,
        ISchoolClassService classService,
        IExamService examService,
        ILogger<ExamWizardController> logger)
    {
        _wizardService = wizardService;
        _academicYearService = academicYearService;
        _classService = classService;
        _examService = examService;
        _logger = logger;
    }

    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        ViewBag.AcademicYears = await _academicYearService.GetAllYearsAsync(ct);
        ViewBag.Classes = await _classService.GetAllSchoolClassesAsync(ct);
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> LoadSubjects([FromBody] ExamWizardStep1Request request, CancellationToken ct = default)
    {
        if (request.SelectedClassIds == null || request.SelectedClassIds.Count == 0)
            return Json(new { success = false, message = "Please select at least one class." });

        try
        {
            var subjects = await _wizardService.LoadSubjectsAsync(
                request.AcademicYearId, request.SelectedClassIds, request.Term, ct);

            return Json(new { success = true, subjects, subjectCount = subjects.Count });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load subjects for exam wizard");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    public async Task<IActionResult> LoadPreviousTemplate([FromBody] ExamWizardTemplateRequest request, CancellationToken ct = default)
    {
        try
        {
            var template = await _wizardService.LoadPreviousExamTemplateAsync(request.AcademicYearId, request.Term, ct);
            if (template == null)
                return Json(new { success = false, message = "No previous exam found for this term." });

            return Json(new { success = true, template });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to load previous exam template");
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create([FromBody] ExamWizardCreateRequest request, CancellationToken ct = default)
    {
        if (request.SelectedClassIds == null || request.SelectedClassIds.Count == 0)
            return Json(new { success = false, message = "No classes selected." });

        if (request.Subjects == null || request.Subjects.Count == 0)
            return Json(new { success = false, message = "No subjects configured." });

        try
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "System";
            var result = await _wizardService.CreateExamsFromWizardAsync(request, userId, ct);
            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exam wizard creation failed");
            return Json(new ExamWizardResultDto
            {
                Success = false,
                Message = $"Failed to create exam(s): {ex.Message}"
            });
        }
    }

    [HttpPost]
    public async Task<IActionResult> GetExamSources([FromBody] ExamWizardSourceRequest request, CancellationToken ct = default)
    {
        try
        {
            var exams = await _examService.GetExamsAsync(request.AcademicYearId, ct);
            var filtered = exams
                .Where(e => e.Term == request.Term)
                .Select(e => new { e.Id, e.Name, e.Term, e.Status, AcademicYearId = request.AcademicYearId })
                .ToList();

            return Json(new { success = true, exams = filtered });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get exam sources");
            return Json(new { success = false, message = ex.Message });
        }
    }
}

public class ExamWizardStep1Request
{
    public int AcademicYearId { get; set; }
    public ExamTerm Term { get; set; }
    public List<int> SelectedClassIds { get; set; } = [];
}

public class ExamWizardTemplateRequest
{
    public int AcademicYearId { get; set; }
    public ExamTerm Term { get; set; }
}

public class ExamWizardSourceRequest
{
    public int AcademicYearId { get; set; }
    public ExamTerm Term { get; set; }
}
```

---

## 2. `G:\PROJECT .NET\SchoolMS\clonelatest\Controllers\Result\PromotionWizardController.cs` (75 lines)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Result;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
[RequirePermission("Promotion.Manage")]
public class PromotionWizardController : Controller
{
    private readonly IPromotionWizardService _wizardService;
    private readonly IAcademicYearService _academicYearService;
    private readonly ISchoolClassService _schoolClassService;

    public PromotionWizardController(
        IPromotionWizardService wizardService,
        IAcademicYearService academicYearService,
        ISchoolClassService schoolClassService)
    {
        _wizardService = wizardService;
        _academicYearService = academicYearService;
        _schoolClassService = schoolClassService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct = default)
    {
        var years = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = years.FirstOrDefault(y => y.IsActive);
        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);
        var sortedClasses = classes.Where(c => !c.IsDeleted && c.IsActive).OrderBy(c => c.SortOrder).ToList();

        ViewBag.AcademicYears = years;
        ViewBag.ActiveYear = activeYear;
        ViewBag.Classes = sortedClasses;

        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> GetPreview([FromBody] PromotionWizardExecuteRequest request, CancellationToken ct = default)
    {
        try
        {
            var preview = await _wizardService.GetPreviewAsync(
                request.FromAcademicYearId, request.FromClassId, request.ExamId, ct);
            return Json(new { success = true, data = preview });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Execute([FromBody] PromotionWizardExecuteRequest request, CancellationToken ct = default)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var result = await _wizardService.ExecuteAsync(request, userId, ct);
            return Json(new { success = result.Success, data = result });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }
}
```

---

## 3. `G:\PROJECT .NET\SchoolMS\clonelatest\Controllers\Result\TeacherResultController.cs` (47 lines)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
[RequirePermission("Marks.View")]
public class TeacherResultController : Controller
{
    private readonly ITeacherResultService _teacherResultService;
    private readonly ITeacherService _teacherService;
    private readonly IAcademicYearService _academicYearService;

    public TeacherResultController(
        ITeacherResultService teacherResultService,
        ITeacherService teacherService,
        IAcademicYearService academicYearService)
    {
        _teacherResultService = teacherResultService;
        _teacherService = teacherService;
        _academicYearService = academicYearService;
    }

    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        if (currentUserId == 0)
            return Unauthorized();

        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null)
            return Forbid();

        var activeYear = await _academicYearService.GetActiveYearAsync(ct);
        var activeYearId = activeYear?.Id ?? 0;

        var dashboard = await _teacherResultService.GetDashboardAsync(teacher.Id, activeYearId, ct);

        return View("Dashboard", dashboard);
    }
}
```

---

## 4. `G:\PROJECT .NET\SchoolMS\clonelatest\Controllers\Result\AdminResultController.cs` (567 lines)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Models.Enums;
using ExamEntity = SchoolManagementSystem.Models.Entities.Exam.Exam;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class AdminResultController : Controller
{
    private readonly IResultAnalyticsService _analyticsService;
    private readonly IResultPublicationService _publicationService;
    private readonly IResultCalculationService _resultCalculationService;
    private readonly IExamService _examService;
    private readonly IMeritCalculationService _meritCalculationService;
    private readonly ISubjectService _subjectService;
    private readonly IStudentExamResultRepository _studentExamResultRepository;
    private readonly IAcademicYearService _academicYearService;
    private readonly IStudentGroupService _studentGroupService;
    private readonly ISchoolClassService _schoolClassService;
    private readonly IResultValidationService _validationService;
    private readonly ILogger<AdminResultController> _logger;

    public AdminResultController(
        IResultAnalyticsService analyticsService,
        IResultPublicationService publicationService,
        IResultCalculationService resultCalculationService,
        IExamService examService,
        IMeritCalculationService meritCalculationService,
        ISubjectService subjectService,
        IStudentExamResultRepository studentExamResultRepository,
        IAcademicYearService academicYearService,
        IStudentGroupService studentGroupService,
        ISchoolClassService schoolClassService,
        IResultValidationService validationService,
        ILogger<AdminResultController> logger)
    {
        _analyticsService = analyticsService;
        _publicationService = publicationService;
        _resultCalculationService = resultCalculationService;
        _examService = examService;
        _meritCalculationService = meritCalculationService;
        _subjectService = subjectService;
        _studentExamResultRepository = studentExamResultRepository;
        _academicYearService = academicYearService;
        _studentGroupService = studentGroupService;
        _schoolClassService = schoolClassService;
        _validationService = validationService;
        _logger = logger;
    }

    [HttpGet]
    [RequirePermission("Result.Dashboard")]
    public async Task<IActionResult> Dashboard(
        int? academicYearId,
        int? examId,
        int? classId,
        int? sectionId,
        int? groupId,
        CancellationToken ct = default)
    {
        var academicYears = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        var dashboardDto = await _analyticsService.GetAdminDashboardAsync();
        if (yearId > 0 && activeYear?.Id != yearId)
        {
            var year = await _academicYearService.GetByIdAsync(yearId, ct);
            if (year != null)
            {
                dashboardDto.ActiveYear = year;
            }
        }

        var yearExams = (await _examService.GetExamsAsync(yearId, ct)).ToList();

        // Use repository (which calls sp_GetResultSummary) for server-side aggregations
        List<dynamic> classPerf = new();
        List<dynamic> sectionPerf = new();
        List<dynamic> gradeDist = new();
        List<dynamic> passTrend = new();
        List<dynamic> topStudents = new();
        int passCount = 0, failCount = 0;

        if (examId.HasValue)
        {
            var classWise = await _studentExamResultRepository.GetClassWiseResultsAsync(examId.Value, ct);
            classPerf = classWise.Select(c => (dynamic)new { Label = c.ClassName, PassRate = c.TotalStudents > 0 ? Math.Round(100m * c.PassedCount / c.TotalStudents, 1) : 0m }).ToList();

            var gradeDistData = await _studentExamResultRepository.GetGradeDistributionAsync(examId.Value, ct);
            gradeDist = gradeDistData.Select(g => (dynamic)new { Grade = g.Grade, Count = g.Count }).ToList();

            var top = await _studentExamResultRepository.GetTopStudentsAsync(examId.Value, ct);
            topStudents = top.Select(t => (dynamic)new { Name = t.StudentName, Gpa = t.Gpa }).ToList();

            var summary = await _studentExamResultRepository.GetResultSummaryStatsAsync(examId.Value, ct);
            if (summary != null)
            {
                passCount = summary.PassedCount;
                failCount = summary.FailedCount;
            }
        }

        // Section + group level aggregations via EF (SP has group data but section-level is per-exam/filter)
        var results = await _studentExamResultRepository.GetFilteredResultsAsync(yearId, examId, classId, sectionId, groupId, ct);

        if (!classPerf.Any())
        {
            classPerf = results.GroupBy(r => r.Student.Class?.Name ?? "Unknown")
                .Select(g => (dynamic)new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
                .OrderBy(x => x.Label).ToList();
        }

        sectionPerf = results.GroupBy(r => r.Student.Section?.Name ?? "Unknown")
            .Select(g => (dynamic)new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
            .OrderBy(x => x.Label).ToList();

        if (!gradeDist.Any())
        {
            gradeDist = results.GroupBy(r => string.IsNullOrEmpty(r.Grade) ? "N/A" : r.Grade)
                .Select(g => (dynamic)new { Grade = g.Key, Count = g.Count() })
                .OrderBy(x => x.Grade).ToList();
        }

        passTrend = results.GroupBy(r => r.Exam?.Name ?? "Exam")
            .Select(g => (dynamic)new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
            .OrderBy(x => x.Label).ToList();

        if (!topStudents.Any())
        {
            topStudents = results.OrderByDescending(r => r.Gpa).Take(10)
                .Select(r => (dynamic)new { Name = r.Student.FullName, Gpa = r.Gpa }).ToList();
        }

        if (!examId.HasValue || passCount == 0 && failCount == 0)
        {
            passCount = results.Count(r => r.IsPassed);
            failCount = results.Count(r => !r.IsPassed);
        }

        var groupPerf = results.Where(r => r.Student.StudentGroup != null)
            .GroupBy(r => r.Student.StudentGroup!.Name)
            .Select(g => (dynamic)new { Label = g.Key, PassRate = g.Any() ? Math.Round(100m * g.Count(x => x.IsPassed) / g.Count(), 1) : 0m })
            .OrderBy(x => x.Label).ToList();

        var subjectPerf = new List<dynamic>();

        var groups = await _studentGroupService.GetAllAsync(ct);
        var chartDataJson = JsonSerializer.Serialize(new
        {
            classPerf,
            sectionPerf,
            groupPerf,
            subjectPerf,
            gradeDist,
            passTrend,
            topStudents,
            passCount,
            failCount
        });

        var vm = new ResultDashboardViewModel
        {
            ActiveYear = dashboardDto.ActiveYear,
            Exams = dashboardDto.Exams,
            ResultStats = dashboardDto.ResultStats,
            AcademicYears = academicYears.ToList(),
            FilterExams = yearExams,
            Groups = groups,
            SelectedAcademicYearId = yearId,
            SelectedExamId = examId,
            SelectedGroupId = groupId,
            ChartDataJson = chartDataJson
        };

        return View(vm);
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> AllSubjects(CancellationToken ct)
    {
        var groupedSubjects = await _subjectService.GetGroupedSubjectsAsync(ct);
        var dict = new Dictionary<string, List<SchoolManagementSystem.Models.DTOs.Academic.SubjectListItemDto>>();
        foreach (var kvp in groupedSubjects)
            dict[kvp.Key] = kvp.Value.ToList();
        var allSubjects = dict.SelectMany(g => g.Value).ToList();
        var model = new AllSubjectsPageViewModel
        {
            Subjects = allSubjects,
            GroupedSubjects = dict
        };
        return View(model);
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> AllResults(int? examId, int? classId, string? status, CancellationToken ct)
    {
        var rawClasses = await _examService.GetClassesAsync(ct);
        var classes = rawClasses.Select(c => new IdNamePairDto
        {
            Id = (int)c.GetType().GetProperty("Id")!.GetValue(c)!,
            Name = (string)c.GetType().GetProperty("Name")!.GetValue(c)!
        }).ToList();
        var model = new AllResultsPageViewModel
        {
            Exams = await _examService.GetExamsAsync(0),
            Classes = classes,
            SelectedExamId = examId,
            SelectedClassId = classId,
            SelectedStatus = status
        };
        return View(model);
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> GetResultListJson(
        int page = 1,
        int size = 20,
        int? examId = null,
        int? classId = null,
        int? sectionId = null,
        int? studentGroupId = null,
        int? status = null,
        string? searchTerm = null,
        int? academicYearId = null,
        CancellationToken ct = default)
    {
        var (items, totalCount) = await _studentExamResultRepository.GetResultListAsync(
            examId, classId, sectionId, studentGroupId, status,
            searchTerm, page, size, ct, academicYearId);

        return Json(new
        {
            data = items,
            last_page = size > 0 ? (int)Math.Ceiling((double)totalCount / size) : 1,
            total_records = totalCount
        });
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> TabulationSheet(int examId, int? classId, int? sectionId, CancellationToken ct)
    {
        var tabulationSheet = await _analyticsService.GetTabulationSheetAsync(examId, classId, sectionId);
        if (tabulationSheet == null) return NotFound("Exam not found");

        ViewBag.Classes = await _examService.GetClassesAsync(ct);
        ViewBag.Sections = await _examService.GetSectionsAsync(classId, ct);

        return View(tabulationSheet);
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public IActionResult MeritLists(int examId)
    {
        return RedirectToAction("Index", "MeritList", new { examId });
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> SubjectAnalysis(int examId, CancellationToken ct)
    {
        var exam = await _examService.GetExamByIdAsync(examId, ct) as ExamEntity;
        if (exam == null) return NotFound();
        var subjectAnalysis = await _analyticsService.GetSubjectAnalysisAsync(examId);
        var model = new SubjectAnalysisPageViewModel
        {
            Subjects = subjectAnalysis,
            Exam = exam
        };
        return View(model);
    }

    [HttpGet]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> ResultPublishing(int? academicYearId, CancellationToken ct)
    {
        var academicYears = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        PublicationDashboardSummaryDto? summary = null;
        List<PublicationDashboardExamDto> exams;
        List<PublicationHistoryEntryDto> history = new();

        if (yearId > 0)
        {
            (exams, summary) = await _publicationService.GetPublicationDashboardAsync(yearId, ct);

            history = await _publicationService.GetPublicationHistoryAsync(yearId, ct);
        }
        else
        {
            exams = new List<PublicationDashboardExamDto>();
        }

        var model = new ResultPublishingPageViewModel
        {
            Exams = exams,
            Summary = summary,
            History = history,
            AcademicYears = academicYears.ToList(),
            SelectedYearId = yearId,
            ActiveYear = yearId > 0 ? academicYears.FirstOrDefault(y => y.Id == yearId) : activeYear
        };

        return View(model);
    }

    [HttpGet]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> PublishingWizard(CancellationToken ct)
    {
        var academicYears = await _academicYearService.GetAllYearsAsync(ct);
        var activeYear = academicYears.FirstOrDefault(x => x.IsActive);
        var yearId = activeYear?.Id ?? 0;

        ViewBag.Exams = await _examService.GetExamsAsync(yearId, ct);
        ViewBag.Classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);
        ViewBag.AcademicYears = academicYears;
        ViewBag.ActiveYearId = yearId;

        return View(new PublishingWizardViewModel());
    }

    [HttpGet]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> GetPublishingValidationJson(int examId, string? classIds, CancellationToken ct)
    {
        try
        {
            var validation = await _validationService.ValidatePrePublicationAsync(examId, ct);
            if (validation == null)
                return Json(new { success = false, message = "Validation returned no data." });

            var issues = validation.Issues.Select(i => new
            {
                severity = i.Severity.ToLower(),
                category = i.Category,
                message = i.Message,
                studentName = i.StudentName,
                subjectName = i.SubjectName
            }).ToList();

            return Json(new
            {
                success = true,
                data = new
                {
                    examName = validation.ExamName,
                    totalStudents = validation.TotalStudents,
                    completedCount = validation.PassedCount + validation.FailedCount,
                    incompleteCount = validation.IncompleteCount,
                    passCount = validation.PassedCount,
                    failCount = validation.FailedCount,
                    averageGpa = 0m,
                    passPercentage = validation.TotalStudents > 0
                        ? Math.Round(100m * validation.PassedCount / validation.TotalStudents, 1)
                        : 0m,
                    issues,
                    isValid = validation.IsValid,
                    totalIssues = validation.TotalIssues
                }
            });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpGet]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> ReviewResults(int examId, int? classId, int? sectionId, int? groupId, CancellationToken ct)
    {
        var exam = await _examService.GetExamByIdAsync(examId, ct) as ExamEntity;

        if (exam == null) return NotFound();

        var academicYear = await _academicYearService.GetByIdAsync(exam.AcademicYearId, ct);

        var (results, _) = await _studentExamResultRepository.GetResultListAsync(
            examId, classId, sectionId, groupId, (int)ResultWorkflowStatus.Submitted, null, 1, 2000, ct);

        var model = new ReviewResultsPageViewModel
        {
            ExamName = exam.Name,
            ExamId = examId,
            AcademicYearName = academicYear?.Name ?? "",
            ClassId = classId,
            SectionId = sectionId,
            GroupId = groupId,
            Results = results
        };

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> PublishResults([FromBody] PublishRequest request)
    {
        try
        {
            var dto = new ResultPublishDto
            {
                ExamId = request.ExamId,
                LockResults = request.LockResults,
                PublicationNotes = request.Remarks ?? "",
                ApprovedByUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0")
            };
            await _publicationService.PublishResultsAsync(dto);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> UnpublishResults([FromBody] PublishRequest request)
    {
        try
        {
            await _publicationService.UnpublishResultsAsync(request.ExamId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> RepublishResults([FromBody] PublishRequest request)
    {
        try
        {
            await _publicationService.RepublishResultsAsync(request.ExamId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Approve")]
    public async Task<IActionResult> ApproveResults([FromBody] PublishRequest request)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            await _publicationService.ApproveReviewedResultsAsync(request.ExamId, userId);
            return Json(new { success = true });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Publish")]
    public async Task<IActionResult> RejectResults([FromBody] RejectRequest request)
    {
        try
        {
            var rows = await _publicationService.RejectResultsAsync(request.ExamId, User.Identity?.Name ?? "admin");
            return Json(new { success = true, affected = rows });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = ex.Message });
        }
    }

    public class PublishRequest
    {
        public int ExamId { get; set; }
        public bool LockResults { get; set; } = true;
        public string? Remarks { get; set; }
    }

    public class RejectRequest
    {
        public int ExamId { get; set; }
        public List<int>? StudentIds { get; set; }
        public string? Notes { get; set; }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> ValidateResults([FromBody] ResultValidationRequest request, CancellationToken ct = default)
    {
        try
        {
            var result = await _validationService.ValidateAsync(request, ct);
            return Json(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Result validation failed for exam {ExamId}", request.ExamId);
            return Json(new { IsValid = false, TotalIssues = 0, Issues = new List<ResultValidationIssueDto>(), Error = ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Recalculate")]
    public async Task<IActionResult> RecalculateResults(int examId, CancellationToken ct = default)
    {
        try
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
            var exam = await _examService.GetExamByIdAsync(examId, ct) as ExamEntity;
            var academicYearId = exam?.AcademicYearId ?? 0;
            await _resultCalculationService.RecalculateAllResultsAsync(examId, academicYearId, userId, "Recalculation triggered from admin dashboard");
            return Json(new { success = true, message = "Results recalculated successfully." });
        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = $"Error: {ex.Message}" });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.Recalculate")]
    public async Task<IActionResult> RecalculateMeritPositions(int examId)
    {
        try
        {
            await _meritCalculationService.RecalculateMeritPositionsAsync(examId);
            TempData["Success"] = "Merit positions recalculated successfully.";
        }
        catch (Exception ex)
        {
            TempData["Error"] = $"Error recalculating merit positions: {ex.Message}";
        }
        return RedirectToAction("Dashboard");
    }
}
```

---

## 5. `G:\PROJECT .NET\SchoolMS\clonelatest\Controllers\Result\ResultManagementController.cs` (318 lines)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.DTOs.Result;
using SchoolManagementSystem.Models.DTOs.Exam;
using SchoolManagementSystem.Models.ViewModels.Result;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Models.Entities.Exam;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Academic;
using System.Security.Claims;
using System.Text.Json;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class ResultManagementController : Controller
{
    private readonly IExamService _examService;
    private readonly IResultPublicationService _publicationService;
    private readonly IMarkEntryService _markEntryService;
    private readonly IReEvaluationService _reEvaluationService;
    private readonly IReportCardService _reportCardService;
    private readonly ITeacherService _teacherService;
    private readonly IStudentService _studentService;
    private readonly IAcademicYearService _academicYearService;
    private readonly ISectionService _sectionService;
    private readonly ITeacherAssignmentService _assignmentService;
    private readonly ITranscriptService _transcriptService;
    private readonly IResultAuthorizationService _resultAuthService;
    private readonly ISubjectMarkStructureService _markStructureService;

    public ResultManagementController(
        IExamService examService,
        IResultPublicationService publicationService,
        IMarkEntryService markEntryService,
        IReEvaluationService reEvaluationService,
        IReportCardService reportCardService,
        ITeacherService teacherService,
        IStudentService studentService,
        IAcademicYearService academicYearService,
        ISectionService sectionService,
        ITeacherAssignmentService assignmentService,
        ITranscriptService transcriptService,
        IResultAuthorizationService resultAuthService,
        ISubjectMarkStructureService markStructureService)
    {
        _examService = examService;
        _publicationService = publicationService;
        _markEntryService = markEntryService;
        _reEvaluationService = reEvaluationService;
        _reportCardService = reportCardService;
        _teacherService = teacherService;
        _studentService = studentService;
        _academicYearService = academicYearService;
        _sectionService = sectionService;
        _assignmentService = assignmentService;
        _transcriptService = transcriptService;
        _resultAuthService = resultAuthService;
        _markStructureService = markStructureService;
    }

    [HttpGet]
    [RequirePermission("Result.Manage")]
    public async Task<IActionResult> AdminIndex(CancellationToken ct)
    {
        var exams = (await _examService.GetExamsAsync(0, ct)).ToList();
        var model = new SchoolManagementSystem.Models.ViewModels.Exam.ExamListViewModel
        {
            Exams = exams
        };
        return View(model);
    }

    [HttpGet]
    [RequirePermission("Result.TeacherEntry")]
    public IActionResult TeacherEntry()
        => RedirectToAction("Index", "Marks");

    [HttpGet]
    [RequirePermission("Result.TeacherEntry")]
    public async Task<IActionResult> GetSubjectsForTeacher(int classId, int? groupId, int? sectionId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return Json(new List<object>());

        var subjects = await _assignmentService.GetTeacherAssignedSubjectsAsync(teacher.Id, classId, groupId, sectionId, ct);
        return Json(subjects.Select(s => new { subjectId = s.Id, subjectName = s.Name }));
    }

    [HttpGet]
    [RequirePermission("Result.TeacherEntry")]
    public async Task<IActionResult> GetSectionsForClass(int classId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var teacher = await _teacherService.GetByUserIdAsync(currentUserId, ct);
        if (teacher == null) return Json(new List<object>());

        var assignments = await _assignmentService.GetTeacherClassAssignmentsAsync(teacher.Id, ct);
        var sections = assignments.Where(a => a.ClassId == classId).Select(a => new { id = a.SectionId, name = a.SectionName }).Distinct();
        return Json(sections);
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> StudentIndex(CancellationToken ct)
    {
        return RedirectToAction(nameof(StudentDashboard));
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> StudentDashboard(CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
        if (student == null)
        {
            TempData["Error"] = "Student profile not found.";
            return RedirectToAction("Index", "Home");
        }

        var dto = await _publicationService.GetStudentResultsAsync(student.Id);
        if (dto == null)
        {
            TempData["Error"] = "Result data not found.";
            return RedirectToAction("Index", "Home");
        }

        return View(ToViewModel(dto));
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> StudentReportCards(int? academicYearId, int? examId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
        if (student == null) return RedirectToAction("StudentDashboard");

        var dto = await _publicationService.GetStudentResultsAsync(student.Id);
        if (dto == null) return RedirectToAction("StudentDashboard");

        var academicYears = await _academicYearService.GetPagedAsync(1, 100, string.Empty, ct);
        ViewBag.AcademicYears = academicYears.Items;
        ViewBag.SelectedAcademicYearId = academicYearId ?? 0;
        ViewBag.SelectedExamId = examId ?? 0;

            if (academicYearId.HasValue && academicYearId > 0)
            {
                var yearExams = (await _examService.GetExamsAsync(academicYearId.Value, ct)).ToList();
                ViewBag.Exams = yearExams;
                var yearExamIds = yearExams.Select(e => e.Id).ToHashSet();
                dto.ExamResults = dto.ExamResults.Where(e => yearExamIds.Contains(e.ExamId)).ToList();
            }
            else
            {
                ViewBag.Exams = (await _examService.GetExamsAsync(0, ct)).ToList();
            }

            if (examId.HasValue && examId > 0)
                dto.ExamResults = dto.ExamResults.Where(e => e.ExamId == examId).ToList();

            return View(ToViewModel(dto));
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> StudentTranscript(int? academicYearId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
        if (student == null) return RedirectToAction("StudentDashboard");

        var academicYears = await _academicYearService.GetPagedAsync(1, 100, string.Empty, ct);
        var activeYear = academicYears.Items.FirstOrDefault(y => y.IsActive);
        var yearId = academicYearId ?? activeYear?.Id ?? 0;

        if (yearId == 0)
        {
            TempData["Error"] = "No academic year selected.";
            return RedirectToAction("StudentDashboard");
        }

        var transcript = await _transcriptService.GetStudentTranscriptAsync(student.Id, yearId);
        if (transcript == null)
        {
            TempData["Error"] = "Transcript not found for the selected academic year.";
            return RedirectToAction("StudentDashboard");
        }

        ViewBag.AcademicYears = academicYears.Items;
        ViewBag.SelectedAcademicYearId = yearId;
        return View(transcript);
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> GetExams(CancellationToken ct)
    {
        var exams = await _examService.GetExamsAsync(0);
        return Json(exams);
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public async Task<IActionResult> GetSubjectsForClass(int classId, int? groupId, int? sectionId, CancellationToken ct)
    {
        var subjects = await _examService.GetSubjectsByClassIdAsync(classId, groupId, sectionId, ct);
        return Json(subjects);
    }

    [HttpGet]
    [RequirePermission("Result.MarkEntry")]
    public IActionResult MarkEntry(int examId, int classId, int sectionId, int subjectId, int? groupId)
    {
        return RedirectToAction("Entry", "Marks", new { examId, classId, sectionId, subjectId, groupId });
    }

    [HttpGet]
    [RequirePermission("Result.ReEvaluation")]
    public async Task<IActionResult> ReEvaluationDashboard()
    {
        var dto = await _reEvaluationService.GetReEvaluationDashboardAsync();
        var model = new ReEvaluationDashboardViewModel
        {
            PendingRequests = dto.PendingRequests.Select(r => new ReEvaluationRequestViewModel
            {
                Id = r.Id,
                StudentId = r.StudentId,
                SubjectId = r.SubjectId,
                ExamId = r.ExamId,
                ExamName = r.ExamName,
                StudentName = r.StudentName,
                SubjectName = r.SubjectName,
                OldMarks = r.OldMarks,
                NewMarks = r.NewMarks,
                Status = r.Status,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt
            }).ToList(),
            CompletedRequests = dto.CompletedRequests.Select(r => new ReEvaluationRequestViewModel
            {
                Id = r.Id,
                StudentId = r.StudentId,
                SubjectId = r.SubjectId,
                ExamId = r.ExamId,
                ExamName = r.ExamName,
                StudentName = r.StudentName,
                SubjectName = r.SubjectName,
                OldMarks = r.OldMarks,
                NewMarks = r.NewMarks,
                Status = r.Status,
                Notes = r.Notes,
                CreatedAt = r.CreatedAt
            }).ToList()
        };
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.ReEvaluation")]
    public async Task<IActionResult> ProcessReEvaluation([FromBody] ReEvaluationProcessDto dto)
    {
        var adminId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        await _reEvaluationService.ProcessReEvaluationAsync(dto, adminId);
        return Json(new { success = true });
    }

    [HttpGet]
    [RequirePermission("Result.RequestReEvaluation")]
    public async Task<IActionResult> RequestReEvaluation(int examId, int subjectId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
        if (student == null) return RedirectToAction("StudentIndex");

        var dto = new ReEvaluationRequestDto { ExamId = examId, SubjectId = subjectId, StudentId = student.Id };
        return View(dto);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("Result.RequestReEvaluation")]
    public async Task<IActionResult> RequestReEvaluation(ReEvaluationRequestDto dto, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
        var student = await _studentService.GetByUserIdAsync(currentUserId, ct);
        if (student == null || student.Id != dto.StudentId)
            return RedirectToAction("StudentIndex");
        await _reEvaluationService.RequestReEvaluationAsync(dto, currentUserId);
        return RedirectToAction("StudentIndex");
    }

    [HttpGet]
    [RequirePermission("Result.View")]
    public IActionResult DownloadReportCard(int examId, int studentId)
        => RedirectToAction("Download", "ReportCard", new { examId, studentId });

    private static StudentPortalResultViewModel ToViewModel(StudentPortalResultDto dto) => new()
    {
        StudentId = dto.StudentId,
        StudentName = dto.StudentName,
        ClassName = dto.ClassName,
        SectionName = dto.SectionName,
        RollNumber = dto.RollNumber,
        ExamResults = dto.ExamResults,
        FinalResult = dto.FinalResult,
        Transcript = dto.Transcript
    };
}
```

---

## 6. `G:\PROJECT .NET\SchoolMS\clonelatest\Controllers\Result\ReportCardController.cs` (278 lines)

```csharp
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolManagementSystem.Filters;
using SchoolManagementSystem.Models.Entities.Result;
using SchoolManagementSystem.Models.Enums;
using SchoolManagementSystem.Services.Interfaces.Result;
using SchoolManagementSystem.Services.Interfaces.Academic;
using SchoolManagementSystem.Services.Interfaces.Students;
using SchoolManagementSystem.Services.Interfaces.Teachers;
using SchoolManagementSystem.Models.Entities.Academic;
using SchoolManagementSystem.Repositories.Interfaces.Result;
using System.Security.Claims;

namespace SchoolManagementSystem.Controllers.Result;

[Authorize]
public class ReportCardController : Controller
{
    private readonly IReportCardService _reportCardService;
    private readonly IStudentService _studentService;
    private readonly IStudentExamResultRepository _studentExamResultRepository;
    private readonly ITeacherScopeService _teacherScopeService;
    private readonly ITranscriptService _transcriptService;
    private readonly IExamService _examService;
    private readonly ISchoolClassService _schoolClassService;
    private readonly ISectionService _sectionService;
    private readonly IAcademicYearService _academicYearService;

    public ReportCardController(
        IReportCardService reportCardService,
        IStudentService studentService,
        IStudentExamResultRepository studentExamResultRepository,
        ITeacherScopeService teacherScopeService,
        ITranscriptService transcriptService,
        IExamService examService,
        ISchoolClassService schoolClassService,
        ISectionService sectionService,
        IAcademicYearService academicYearService)
    {
        _reportCardService = reportCardService;
        _studentService = studentService;
        _studentExamResultRepository = studentExamResultRepository;
        _teacherScopeService = teacherScopeService;
        _transcriptService = transcriptService;
        _examService = examService;
        _schoolClassService = schoolClassService;
        _sectionService = sectionService;
        _academicYearService = academicYearService;
    }

    [HttpGet]
    [RequirePermission("ReportCard.View")]
    public async Task<IActionResult> Index(int? examId, int? classId, int? sectionId, CancellationToken ct)
    {
        var exams = await _examService.GetAllExamsAsync(ct);
        var classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);

        ViewBag.Exams = exams;
        ViewBag.Classes = classes;
        ViewBag.SelectedExamId = examId;
        ViewBag.SelectedClassId = classId;
        ViewBag.SelectedSectionId = sectionId;

        if (classId.HasValue)
        {
            var isTeacher = User.IsInRole("Teacher") || User.IsInRole("Senior Lecturer") || User.IsInRole("Lecturer");
            var isAdmin = User.IsInRole("Admin") || User.IsInRole("Super Admin") || User.IsInRole("Principal") || User.IsInRole("Exam Controller");

            IEnumerable<int> allowedSectionIds;
            if (isTeacher && !isAdmin)
            {
                var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");
                allowedSectionIds = await _teacherScopeService.GetAssignedSectionIdsAsync(currentUserId, classId.Value, ct);
            }
            else
            {
                allowedSectionIds = Enumerable.Empty<int>(); // admins see all
            }

            var sections = (await _sectionService.GetByClassIdAsync(classId.Value, null, ct)).ToList();
            if (isTeacher && !isAdmin)
                sections = sections.Where(s => allowedSectionIds.Contains(s.Id)).ToList();
            ViewBag.Sections = sections;
        }

        var studentResults = new List<StudentExamResult>();

        if (examId.HasValue && classId.HasValue)
        {
            var query = _studentExamResultRepository.Query().AsNoTracking()
                .Include(r => r.Student)
                .Include(r => r.Exam)
                .Where(r => r.ExamId == examId.Value && r.Student.ClassId == classId.Value && !r.IsDeleted);

            if (sectionId.HasValue && sectionId > 0)
            {
                query = query.Where(r => r.Student.SectionId == sectionId.Value);
            }

            studentResults = await query.OrderBy(r => r.Student.RollNumber).ToListAsync(ct);
        }

        return View(studentResults);
    }

    [HttpGet]
    [RequirePermission("ReportCard.View")]
    public async Task<IActionResult> Bulk(CancellationToken ct)
    {
        var activeYear = await _academicYearService.GetActiveYearAsync(ct);
        if (activeYear != null)
            ViewBag.Exams = await _examService.GetExamsAsync(activeYear.Id, ct);
        else
            ViewBag.Exams = Array.Empty<object>();

        ViewBag.Classes = await _schoolClassService.GetAllSchoolClassesAsync(ct);
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequirePermission("ReportCard.View")]
    public async Task<IActionResult> GenerateBulk(int examId, int? classId, CancellationToken ct)
    {
        var pdf = await _reportCardService.GenerateBulkReportCardsAsync(examId, classId, ct);
        if (pdf.Length == 0)
        {
            TempData["ErrorMessage"] = "No report cards found for the selected criteria.";
            return RedirectToAction(nameof(Bulk));
        }
        return File(pdf, "application/pdf", $"ReportCards_Exam{examId}.pdf");
    }

    [HttpGet]
    [RequirePermission("ReportCard.Download")]
    public async Task<IActionResult> Download(int examId, int studentId, CancellationToken ct)
    {
        var currentUserId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier) ?? "0");

        bool isAdmin = User.IsInRole("Admin") || User.IsInRole("Super Admin") || User.IsInRole("Principal") || User.IsInRole("Exam Controller");

        if (!isAdmin)
        {
            if (User.IsInRole("Student"))
            {
                var student = await _studentService.GetByUserIdAsync(currentUserId, ct);

...27228 bytes truncated...

The tool call succeeded but the output was truncated. Full output saved to: C:\Users\islam\.local\share\opencode\tool-output\tool_f55da9c46001xyAPJQXhaJtE87
Use the Task tool to have explore agent process this file with Grep and Read (with offset/limit). Do NOT read the full file yourself - delegate to save context.
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 9.6s)

_Thinking:_

The output was truncated. Let me now look at the existing views to understand what needs to be enhanced. Let me also check the existing views for the wizard, teacher dashboard, publishing wizard, etc.

Let me now focus on what needs to be done:

1. **Exam Wizard** - Already exists with ExamWizardController + ExamWizardService + Index view. Needs enhancement for better UX.
2. **Teacher Dashboard** - Already exists with TeacherResultController + TeacherResultService. Needs a proper view.
3. **Result Validation** - Already exists with ResultValidationService + validation endpoint in AdminResultController. Needs a proper UI.
4. **Publishing Wizard** - Already exists with PublishingWizard view in AdminResultController.
5. **Bulk Report Cards** - Already exists with ReportCardController.Bulk + GenerateBulk.
6. **Promotion Wizard** - Already exists with PromotionWizardController + PromotionWizardService.

Let me now look at the existing views to understand what needs to be enhanced. Let me check the views directory structure.

**Tool: todowrite**

**Input:**
```json
{
  "todos": [
    {
      "priority": "high",
      "content": "Phase 0 — Protect the Existing System (already completed per user)",
      "status": "completed"
    },
    {
      "priority": "high",
      "content": "Phase 1 — Enterprise Workflow: Exam Wizard, Teacher Dashboard, Result Validation, Publishing Wizard, Bulk Report Cards",
      "status": "in_progress"
    },
    {
      "priority": "high",
      "content": "Phase 1 — Promotion Wizard",
      "status": "pending"
    },
    {
      "priority": "medium",
      "content": "Phase 2 — Automation: Auto Teacher Assignment, Auto Schedule Generator",
      "status": "pending"
    },
    {
      "priority": "medium",
      "content": "Phase 3 — Architecture Compliance: DTO, Bootstrap removal, ViewBag cleanup, SP wiring",
      "status": "pending"
    },
    {
      "priority": "low",
      "content": "Phase 4 — Enterprise Hardening: Performance, caching, notifications, security, monitoring",
      "status": "pending"
    }
  ]
}
```

**Output:**
```
[
  {
    "content": "Phase 0 — Protect the Existing System (already completed per user)",
    "status": "completed",
    "priority": "high"
  },
  {
    "content": "Phase 1 — Enterprise Workflow: Exam Wizard, Teacher Dashboard, Result Validation, Publishing Wizard, Bulk Report Cards",
    "status": "in_progress",
    "priority": "high"
  },
  {
    "content": "Phase 1 — Promotion Wizard",
    "status": "pending",
    "priority": "high"
  },
  {
    "content": "Phase 2 — Automation: Auto Teacher Assignment, Auto Schedule Generator",
    "status": "pending",
    "priority": "medium"
  },
  {
    "content": "Phase 3 — Architecture Compliance: DTO, Bootstrap removal, ViewBag cleanup, SP wiring",
    "status": "pending",
    "priority": "medium"
  },
  {
    "content": "Phase 4 — Enterprise Hardening: Performance, caching, notifications, security, monitoring",
    "status": "pending",
    "priority": "low"
  }
]
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 62.1s)

_Thinking:_

Let me now explore the existing views to understand what needs to be enhanced. I need to look at the views for the wizard, teacher dashboard, publishing wizard, etc.

Let me explore the existing views and understand what needs to be built/enhanced.

**Tool: task**

**Input:**
```json
{
  "description": "Explore existing wizard views",
  "subagent_type": "explore",
  "prompt": "Very thorough exploration of the existing wizard and result management views in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nPlease read and return the FULL contents of:\n\n1. `Views/ExamWizard/Index.cshtml`\n2. `Views/PromotionWizard/Index.cshtml` (if exists)\n3. `Views/AdminResult/PublishingWizard.cshtml` (if exists)\n4. `Views/AdminResult/ResultPublishing.cshtml` (if exists)\n5. `Views/TeacherResult/Dashboard.cshtml` (if exists)\n6. `Views/ReportCard/Bulk.cshtml` (if exists)\n7. `Views/ReportCard/Index.cshtml` (if exists)\n8. `Views/MeritList/Index.cshtml` (if exists)\n\nAlso list all files in:\n9. `Views/ExamWizard/`\n10. `Views/PromotionWizard/`\n11. `Views/TeacherResult/`\n12. `Views/AdminResult/`\n13. `Views/ReportCard/`\n14. `Views/MeritList/`\n\nReturn the full contents of each file found."
}
```

**Output:**
```
<task id="ses_0aa2515c1ffeLf6pjpo4dxDgwX" state="completed">
<task_result>
Here is the complete and thorough exploration of all wizard and result management views in the system.

---

## 1. `Views/ExamWizard/Index.cshtml` (492 lines)

**File:** `G:\PROJECT .NET\SchoolMS\clonelatest\Views\ExamWizard\Index.cshtml`

This is a 3-step Exam Creation Wizard that:
- **Step 1** — Selects Academic Year, Term, Exam Type, Start/End Dates, and Applicable Classes (with a "Load Previous Exam" template button).
- **Step 2** — Loads subjects with their Full Marks, Pass Marks, Optional flag, Teacher assignment, and Component breakouts via AJAX POST to `ExamWizard.LoadSubjects`. Shows stat counters (Total Subjects, Auto-Assigned Teachers, Total Classes).
- **Step 3** — Displays a summary card with all collected data and a "Create Exam(s)" button that POSTs to `ExamWizard.Create`.

The view uses `_AdminLayout`, has inline embedded styles for the wizard progress bar/panels/class-grid/summary, and vanilla JS wizard navigation. Anti-forgery token is sent with all AJAX requests. Supports loading previous exam templates.

---

## 2. `Views/PromotionWizard/Index.cshtml` (556 lines)

**File:** `G:\PROJECT .NET\SchoolMS\clonelatest\Views\PromotionWizard\Index.cshtml`

A 4-step Promotion Wizard:
- **Step 1** — Select From/To Academic Year and From/To Class.
- **Step 2** — Select Annual Exam (with auto-detect option).
- **Step 3** — Preview with stats (Eligible/Conditional/Failed/Total counts) and a student detail table showing Student Name, Roll, GPA, Attendance %, Status, and Reason. Data fetched via AJAX POST to `PromotionWizard.GetPreview`.
- **Step 4** — Execution with checkboxes (Auto-generate roll numbers, Auto-assign subjects, Auto-create attendance records), a warning alert, and an "Execute Promotion" button that POSTs to `PromotionWizard.Execute`.

Uses `_AdminLayout`, typed with entity references via `ViewBag`. Has inline styles for the wizard progress bar, panels, status colors, and a loading overlay (`er-skeleton-overlay`). Uses `showErAlert()` for toast notifications.

---

## 3. `Views/AdminResult/PublishingWizard.cshtml` (445 lines)

**File:** `G:\PROJECT .NET\SchoolMS\clonelatest\Views\AdminResult\PublishingWizard.cshtml`

Typed with `PublishingWizardViewModel`. A 3-step Result Publishing Wizard:
- **Step 1** — Select Exam + checkboxes for Target Classes (with "Select All").
- **Step 2** — Validation summary: loads stats (Total Students, Completed, Incomplete, Pass %) and issues list (with Error/Warning/Info severity icons). Data via `AdminResult.GetPublishingValidationJson`. Blocking errors disable Next.
- **Step 3** — Summary (Exam, Total Students, Pass/Fail, Pass %, Average GPA), lock checkbox, notes textarea. "Publish Results" button POSTs to `AdminResult.PublishResults`.

Uses `_AdminLayout` with inline styles for the wizard steps, class grid, summary layout, and fade animation.

---

## 4. `Views/AdminResult/ResultPublishing.cshtml` (533 lines)

**File:** `G:\PROJECT .NET\SchoolMS\clonelatest\Views\AdminResult\ResultPublishing.cshtml`

Typed with `ResultPublishingPageViewModel`. Main publication management dashboard:
- **Stats bar**: Total Exams, Published, Pending Action, Total Results (Pub/Total).
- **Exams table**: expandable rows (+/−) showing detail breakdowns (Total Results, Published, Approved, Reviewed, Submitted, Draft). Action buttons per exam: Publish / Republish / Unpublish (contextual based on status).
- **Publication History**: collapsible section with audit trail table (Date/Time, Action, Performed By, Notes).
- **Confirm Modal**: reusable action modal with lock checkbox + notes textarea.

Uses toastr notifications and anti-forgery tokens in AJAX POSTs to `AdminResult.PublishResults`, `AdminResult.UnpublishResults`, `AdminResult.RepublishResults`.

---

## 5. `Views/TeacherResult/Dashboard.cshtml` (264 lines)

**File:** `G:\PROJECT .NET\SchoolMS\clonelatest\Views\TeacherResult\Dashboard.cshtml`

Typed with `TeacherResultDashboardDto`. A teacher's personal result dashboard:
- **Stats cards**: Assigned Exams, Pending Marks, Submitted, Completion %.
- **Progress bars**: Mark Entry Progress, Assigned Subjects, Assigned Exams.
- **Pending Exams table**: Exam, Subject, Class, Section, Students, Completed, Status badges (Published/Locked/Approved/Submitted/Draft), and an "Open" link to `Marks.Entry` for non-read-only items.
- **Recent Activity**: timeline of actions with dots, descriptions, and timestamps.

Uses `_AdminLayout` with inline styles for the trend-row cards, activity list, and responsive breakpoints.

---

## 6. `Views/ReportCard/Bulk.cshtml` (233 lines)

**File:** `G:\PROJECT .NET\SchoolMS\clonelatest\Views\ReportCard\Bulk.cshtml`

Bulk report card generation page:
- **Form**: Exam dropdown (required), Class dropdown (optional — "All Classes").
- **Submit**: triggers AJAX POST to `ReportCard.GenerateBulk` with `responseType: 'blob'`. Handles blob download with `content-disposition` filename parsing. Shows progress spinner + status bar during generation.

Uses `_AdminLayout` with inline styles for the bulk container, form, status bar, and result alerts.

---

## 7. `Views/ReportCard/Index.cshtml` (168 lines)

**File:** `G:\PROJECT .NET\SchoolMS\clonelatest\Views\ReportCard\Index.cshtml`

Typed with `IEnumerable<StudentExamResult>`. Student report cards listing:
- **Filter panel**: Exam, Class, Section (Section loads via AJAX on class change), Search button.
- **Results table**: Roll, Student ID, Student Name, Total Marks, GPA, Letter Grade, Class Position, Pass/Fail badge, "Download PDF" action link per student.

Uses `_AdminLayout` with Bootstrap classes (`row g-3`, `col-md-*`, `form-select`, `form-label`, `d-flex`, etc.) mixed with universal CSS (`er-*`, `adm-*`).

---

## 8. `Views/MeritList/Index.cshtml` (161 lines)

**File:** `G:\PROJECT .NET\SchoolMS\clonelatest\Views\MeritList\Index.cshtml`

Typed with `IEnumerable<MeritListItem>`. Academic merit rankings:
- **Filter panel**: Exam, Ranking Category (Class-wise, Section-wise, Group-wise, School-wide).
- **Admin action**: "Recalculate Merit Positions" button (visible to Admin/Super Admin/Principal/Exam Controller roles) — POSTs to `MeritList.Recalculate`.
- **Results table**: Rank (with special styling for 1st/Gold, 2nd/Silver, 3rd/Bronze), Student ID, Student Name, Roll Number, Section, Group, Total Marks, GPA, Letter Grade.

Uses `_AdminLayout` with mixed Bootstrap (`row g-3`, `col-md-*`, `form-select`) and universal CSS (`er-*`, `adm-*`).

---

## 9. Directory Listings

### `Views/ExamWizard/`
```
1. Index.cshtml
```

### `Views/PromotionWizard/`
```
1. Index.cshtml
```

### `Views/TeacherResult/`
```
1. Dashboard.cshtml
```

### `Views/AdminResult/`
```
1. _ValidationSummary.cshtml      (partial view — validation AJAX logic + stats + issues table)
2. AllResults.cshtml              (Tabulator grid of all student results with filters)
3. AllSubjects.cshtml             (tabbed subject overview grouped by category)
4. ClassResults.cshtml            (performance summary grouped by class)
5. Dashboard.cshtml               (result dashboard: stats + charts + recent exams + bulk ops modals)
6. MeritList.cshtml               (student ranking by performance + filters)
7. PublishingWizard.cshtml        (3-step publish wizard)
8. ResultAnalytics.cshtml         (analytics: KPIs + grade distribution chart + class comparison + top/bottom students)
9. ResultDetails.cshtml           (single student result detail with subject-wise breakdown)
10. ResultPublishing.cshtml       (publication management: publish/unpublish/re-publish + history)
11. ReviewResults.cshtml          (result review workflow: approve/reject with filterable table + review modal)
12. StudentResults.cshtml         (searchable student results listing)
13. SubjectAnalysis.cshtml        (subject performance breakdown: avg marks, pass %, high/low)
14. TabulationSheet.cshtml        (tabulation sheet: student ranking + subject summary + subject-wise breakdown)
```

### `Views/ReportCard/`
```
1. BangladeshFormat.cshtml        (formal Bangladesh-style academic report card — A4 print layout)
2. Bulk.cshtml                    (bulk PDF generation form)
3. Index.cshtml                   (report cards listing with download links)
```

### `Views/MeritList/`
```
1. Index.cshtml                   (academic merit rankings)
```

---

## Summary of Key Findings

| View | Type | Layout | Key Features |
|------|------|--------|-------------|
| **ExamWizard/Index** | 3-step wizard | `_AdminLayout` | AJAX subject loading, previous template loading, exam creation |
| **PromotionWizard/Index** | 4-step wizard | `_AdminLayout` | Preview with eligibility stats, execute with roll/subject/attendance auto-ops |
| **AdminResult/PublishingWizard** | 3-step wizard | `_AdminLayout` | Validation checks, issue filtering, publish with lock/notes |
| **AdminResult/ResultPublishing** | Management page | `_AdminLayout` | Publish/unpublish/republish, expandable exam rows, audit trail modal |
| **TeacherResult/Dashboard** | Personal dashboard | `_AdminLayout` | Stats, progress bars, pending exams table, recent activity |
| **ReportCard/Bulk** | Form page | `_AdminLayout` | AJAX blob download with progress, exam/class filters |
| **ReportCard/Index** | List page | `_AdminLayout` | Bootstrap+universal CSS mixed, per-student PDF download |
| **ReportCard/BangladeshFormat** | Print view | `_Layout` | Full A4 report card: school header, marks table, summary, signatures, footer, QR placeholder |
| **MeritList/Index** | List page | `_AdminLayout` | Bootstrap+universal CSS mixed, recalculate action, gold/silver/bronze rank styling |
| **AdminResult/Dashboard** | Full dashboard | `_AdminLayout` | Chart.js (4 charts), stats, trend cards, quick actions, bulk ops modal, recent exams |
| **AdminResult/AllResults** | Tabulator grid | `_AdminLayout` | Remote pagination, export (Excel/PDF), filters |
| **AdminResult/ReviewResults** | Review workflow | `_AdminLayout` | Approve/reject all, filterable table, review modal with notes |
| **AdminResult/SubjectAnalysis** | Analysis page | `_AdminLayout` | Best/worst subject cards, tabular breakdown |
| **AdminResult/TabulationSheet** | Tabulation page | `_AdminLayout` | Student ranking + subject summary + sticky subject-wise breakdown |
| **AdminResult/ResultDetails** | Detail page | `_AdminLayout` | Student info card, KPI grid, subject-wise table, print button |
| **AdminResult/ResultAnalytics** | Analytics page | `_AdminLayout` | Grade distribution (Chart.js bar), top/bottom students, class comparison |
| **AdminResult/ClassResults** | Summary page | `_AdminLayout` | Class-wise pass rate, avg GPA, high/low GPA |
| **AdminResult/StudentResults** | Search page | `_AdminLayout` | Search by name/roll, exam/class filters, export |
| **AdminResult/AllSubjects** | Overview page | `_AdminLayout` | Tabbed by category, mandatory/optional/religion stats |
| **AdminResult/_ValidationSummary** | Partial | none | AJAX validation, per-class summary, issue filtering |
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 233.7s)

_Thinking:_

Excellent! The codebase already has a very mature set of views and controllers for all the Phase 1 features. Let me now assess what needs to be done:

1. **Exam Wizard** — Already exists with 3-step wizard. The user wants a more comprehensive wizard. Let me check what's missing.
2. **Teacher Dashboard** — Already exists with stats, progress bars, pending exams table, recent activity.
3. **Result Validation** — Already exists with validation service + PublishingWizard view that includes validation step.
4. **Publishing Wizard** — Already exists with 3-step wizard.
5. **Bulk Report Cards** — Already exists with Bulk.cshtml + GenerateBulk action.
6. **Promotion Wizard** — Already exists with 4-step wizard.

The user's roadmap says:
- **Exam Wizard** should be a comprehensive multi-step wizard with:
  - Step 1: Academic Year, Exam Type, Classes, Term
  - Step 2: Auto-load subjects, components, pass marks, full marks, teacher assignment, grade policy
  - Step 3: Auto-create exam, exam subjects, exam components, mark sessions, teacher assignments, schedule
  - Buttons: Use Previous Exam, Load NCTB Template, Save Template

- **Teacher Dashboard** should show: Pending Marks, Today's Classes, Assigned Exams, Recent Notifications, Quick Actions

- **Result Validation Engine** should check: Missing Marks, Duplicate Marks, GPA mismatch, Missing Components, Subject not completed, Failed calculations, Practical missing, Attendance missing

- **Publishing Wizard**: Choose Exam → Validation → Preview → Publish

- **Bulk Report Cards**: Entire Class, Entire Section, Entire School, ZIP, PDF, Print Queue

- **Promotion Wizard**: One wizard that internally calls existing services

The existing codebase already has most of this. The work is about:
1. Enhancing the existing wizards with better UX
2. Making sure the views use universal CSS (not Bootstrap)
3. Adding missing features

Let me now focus on what actually needs to be done. Let me check the current state of the views for Bootstrap usage and inline styles, and then enhance them.

Actually, looking at the user's message more carefully, they said "here 1st one is completed others do in parallal" - meaning Phase 0 is done, and they want me to work on the other phases in parallel.

Let me now focus on the most impactful work:

1. **Phase 1 — Enterprise Workflow**: The core infrastructure already exists. I need to:
   - Enhance the Exam Wizard with better UX (NCTB template loading, better component management)
   - Enhance the Teacher Dashboard with more features
   - Create a proper Result Validation UI
   - Enhance the Publishing Wizard
   - Enhance Bulk Report Cards
   - Enhance the Promotion Wizard

2. **Phase 2 — Automation**: Auto Teacher Assignment, Auto Schedule Generator
3. **Phase 3 — Architecture Compliance**: Fix Bootstrap, DTO, ViewBag, CSS, JS
4. **Phase 4 — Enterprise Hardening**: Performance, caching, notifications, security

Let me now look at what specific enhancements are needed. Let me check the existing views for Bootstrap usage and inline styles, and then start enhancing them.

Actually, let me take a step back. The user said "here 1st one is completed others do in parallal" - meaning Phase 0 is done, and they want me to work on the other phases in parallel. But these are large phases with many sub-tasks.

Let me focus on the most impactful work:

1. **Phase 1 — Enterprise Workflow**: The core infrastructure already exists. I need to:
   - Enhance the Exam Wizard with NCTB template loading, better UX
   - Enhance the Teacher Dashboard
   - Create a proper Result Validation UI
   - Enhance the Publishing Wizard
   - Enhance Bulk Report Cards
   - Enhance the Promotion Wizard

2. **Phase 2 — Automation**: Auto Teacher Assignment, Auto Schedule Generator
3. **Phase 3 — Architecture Compliance**: Fix Bootstrap, DTO, ViewBag, CSS, JS
4. **Phase 4 — Enterprise Hardening**: Performance, caching, notifications, security

Let me now look at what specific enhancements are needed. Let me check the existing views for Bootstrap usage and inline styles, and then start enhancing them.

Actually, let me take a more focused approach. The user said "do in parallel" - meaning I should work on multiple phases simultaneously. Let me launch multiple agents to work on different aspects in parallel.

Let me focus on the most impactful work:

1. **Phase 1 — Enterprise Workflow**: The existing wizards and dashboards are already quite mature. I need to:
   - Enhance the Exam Wizard with NCTB template support and better UX
   - Enhance the Teacher Dashboard
   - Create a proper Result Validation UI
   - Enhance the Publishing Wizard
   - Enhance Bulk Report Cards
   - Enhance the Promotion Wizard

2. **Phase 2 — Automation**: Auto Teacher Assignment, Auto Schedule Generator
3. **Phase 3 — Architecture Compliance**: Fix Bootstrap, DTO, ViewBag, CSS, JS
4. **Phase 4 — Enterprise Hardening**: Performance, caching, notifications, security

Let me now look at what specific enhancements are needed. Let me check the existing views for Bootstrap usage and inline styles, and then start enhancing them.

Actually, let me take a more practical approach. The user wants me to work on multiple phases in parallel. Let me launch multiple agents to work on different aspects simultaneously.

Let me start with the most impactful work:
1. Enhance the Exam Wizard with NCTB template support
2. Enhance the Teacher Dashboard
3. Create a proper Result Validation UI
4. Enhance the Publishing Wizard
5. Enhance Bulk Report Cards
6. Enhance the Promotion Wizard

Let me look at the existing code more carefully to understand what specific enhancements are needed.

The codebase already has mature implementations for all Phase 1 features. Let me now assess what specific enhancements are needed and work on them in parallel.

**Tool: task**

**Input:**
```json
{
  "description": "Assess Bootstrap usage in views",
  "subagent_type": "explore",
  "prompt": "Quick assessment of Bootstrap usage in key views at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nSearch for Bootstrap classes in these specific view files. Count how many Bootstrap classes (col-md, col-sm, row, form-control, form-select, form-label, btn btn-, table table-, card, card-body, card-header, modal, modal-content, modal-header, modal-body, modal-footer, alert alert-, badge bg-, text-, bg-, d-flex, justify-content, align-items, g-3, g-2, mb-3, mt-3, p-3, etc.) are used in each file:\n\n1. `Views/ExamWizard/Index.cshtml`\n2. `Views/PromotionWizard/Index.cshtml`\n3. `Views/AdminResult/PublishingWizard.cshtml`\n4. `Views/AdminResult/ResultPublishing.cshtml`\n5. `Views/TeacherResult/Dashboard.cshtml`\n6. `Views/ReportCard/Bulk.cshtml`\n7. `Views/ReportCard/Index.cshtml`\n8. `Views/MeritList/Index.cshtml`\n9. `Views/AdminResult/Dashboard.cshtml`\n\nFor each file, report:\n- Total lines\n- Number of Bootstrap classes used\n- Number of inline styles\n- Number of universal CSS classes (adm-*, dash-*, ws-*, er-*)\n- Whether it uses _AdminLayout\n\nReturn a table with this data."
}
```

**Output:**
```
<task id="ses_0aa23cce4ffedIMve12WmpLkeY" state="completed">
<task_result>
Now I have all verified counts. Here is the comprehensive assessment:

---

## Bootstrap Usage Assessment in 9 Key Result/Exam Views

### Summary Table

| # | File | Lines | Bootstrap Classes | Inline Styles | Universal CSS (`adm-*`, `dash-*`, `ws-*`, `er-*`) | Uses `_AdminLayout` |
|---|------|-------|-----------------|---------------|----------------------------------------------------|---------------------|
| 1 | `Views/ExamWizard/Index.cshtml` | 492 | **0** | 11 | ~102 | **Yes** (explicit) |
| 2 | `Views/PromotionWizard/Index.cshtml` | 556 | **0** | 0 | ~105 | No (uses `_Layout`) |
| 3 | `Views/AdminResult/PublishingWizard.cshtml` | 445 | **1** (`mb-4`) | 18 | ~99 | No (uses `_Layout`) |
| 4 | `Views/AdminResult/ResultPublishing.cshtml` | 533 | **10** (`mb-4`, `text-end`, `py-1 x3`, `px-3 x3`, `mt-4`, `table-responsive`) | 54 | ~74 | No (uses `_Layout`) |
| 5 | `Views/TeacherResult/Dashboard.cshtml` | 264 | **0** | 6 | ~74 | **Yes** (explicit) |
| 6 | `Views/ReportCard/Bulk.cshtml` | 233 | **0** | 1 | ~16 | **Yes** (explicit) |
| 7 | `Views/ReportCard/Index.cshtml` | 168 | **31** (`row`, `g-3`, `col-md-4/3/2`, `form-label x3`, `form-select x3`, `d-flex`, `align-items-end`, `align-items-center`, `w-100`, `text-center x4`, `font-weight-bold x4`, `text-primary`, `py-1`, `px-3`, `me-2`, `table-responsive`) | 3 | ~21 | No (uses `_Layout`) |
| 8 | `Views/MeritList/Index.cshtml` | 161 | **25** (`me-2`, `row`, `g-3`, `col-md-5 x2`, `col-md-2`, `form-label x2`, `form-select x2`, `d-flex`, `align-items-end`, `w-100`, `table-responsive`, `text-center x5`, `font-weight-bold x4`, `text-success`, `text-primary`) | 4 | ~21 | No (uses `_Layout`) |
| 9 | `Views/AdminResult/Dashboard.cshtml` | 812 | **2** (`mb-4`, `table-responsive`) | 28 | ~165 | No (uses `_Layout`) |
| | **TOTALS** | **3,664** | **69** | **125** | **~677** | **3 of 9** |

---

### Key Findings

1. **Cleanest files (0 Bootstrap):**
   - `ExamWizard/Index.cshtml` -- fully universal CSS, explicitly uses `_AdminLayout`
   - `PromotionWizard/Index.cshtml` -- fully universal CSS, zero inline styles
   - `TeacherResult/Dashboard.cshtml` -- fully universal CSS, explicitly uses `_AdminLayout`
   - `ReportCard/Bulk.cshtml` -- fully universal CSS, explicitly uses `_AdminLayout`

2. **Heaviest Bootstrap usage:**
   - `ReportCard/Index.cshtml` -- **31 Bootstrap class instances** (predominantly `col-md-*`, `form-label`, `form-select`, `text-center`, `font-weight-bold` from its filter panel and table rows)
   - `MeritList/Index.cshtml` -- **25 Bootstrap class instances** (same pattern: filter form with `col-md-*`, `form-label`, `form-select`, and table cells with `text-center`, `font-weight-bold`)
   - `ResultPublishing.cshtml` -- **10 Bootstrap class instances** (mostly `py-1 px-3` on action buttons, plus `mb-4`, `mt-4`, `text-end`, `table-responsive`)

3. **Inline style hotspots:**
   - `ResultPublishing.cshtml` (54) -- heavy use of inline styles in the exam detail expandable rows and audit trail section, many using `var(--adm-*)` CSS variables (acceptable pattern)
   - `AdminResult/Dashboard.cshtml` (28) -- inline styles for chart cards, action cards, and modals
   - `PublishingWizard.cshtml` (18) -- inline styles for validation panels and summary

4. **Layout usage:**
   - Only **3 of 9** files explicitly declare `Layout = "_AdminLayout"`. The remaining 6 rely on `_ViewStart.cshtml` which defaults to `"_Layout"` (not `"_AdminLayout"`).

5. **Universal CSS adoption is strong** (~677 occurrences across the 9 files), but 3 files still carry significant Bootstrap carryover (`ReportCard/Index.cshtml`, `MeritList/Index.cshtml`, `ResultPublishing.cshtml`).
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 392.2s)

_Thinking:_

Now I have a clear picture. Let me now work on the phases in parallel. The user said "do in parallel" so I should launch multiple agents to work on different aspects simultaneously.

Let me plan the parallel work:

**Phase 1 — Enterprise Workflow Enhancements:**
1. **Exam Wizard Enhancement** — Add NCTB template loading, better component management, Save Template functionality
2. **Teacher Dashboard Enhancement** — Already exists, needs minor improvements
3. **Result Validation UI** — Already exists in PublishingWizard, needs enhancement
4. **Publishing Wizard Enhancement** — Already exists, needs minor improvements
5. **Bulk Report Cards Enhancement** — Already exists, needs minor improvements
6. **Promotion Wizard Enhancement** — Already exists, needs minor improvements

**Phase 2 — Automation:**
- Auto Teacher Assignment
- Auto Schedule Generator

**Phase 3 — Architecture Compliance:**
- Fix Bootstrap in ReportCard/Index.cshtml, MeritList/Index.cshtml, ResultPublishing.cshtml
- Fix inline styles
- Fix layout references

**Phase 4 — Enterprise Hardening:**
- Performance, caching, notifications, security

Let me now work on these in parallel. Let me launch multiple agents to work on different aspects simultaneously.

Actually, let me think about what's the most practical approach. The user said "do in parallel" - I should work on multiple phases simultaneously. Let me launch multiple agents to work on different aspects.

Let me focus on the most impactful work first:

1. **Phase 1 — Enterprise Workflow**: Enhance the existing wizards and dashboards
2. **Phase 3 — Architecture Compliance**: Fix Bootstrap in the 3 worst files

Let me launch multiple agents to work on these in parallel.

Now I have a clear picture. Let me work on multiple phases in parallel. The codebase already has mature infrastructure — the work is about enhancement and cleanup.

**Tool: task**

**Input:**
```json
{
  "description": "Enhance Exam Wizard with NCTB",
  "subagent_type": "general",
  "prompt": "Enhance the Exam Wizard in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nThe Exam Wizard already exists with a 3-step flow. I need to enhance it with:\n\n1. **NCTB Template Loading** — Add a \"Load NCTB Template\" button that loads standard Bangladesh NCTB subject/component/mark configurations based on class and group (Science/Business Studies/Humanities).\n\n2. **Save Template** — Add ability to save exam configurations as reusable templates.\n\n3. **Better Component Management** — Show component breakouts (Written, MCQ, Practical, Viva) with their full marks and pass marks in the wizard.\n\nHere's what to do:\n\n### Step 1: Read the existing files\nRead these files fully:\n- `Services/Implementations/Result/ExamWizardService.cs`\n- `Services/Interfaces/Result/IExamWizardService.cs`\n- `Models/DTOs/Result/ExamWizardDtos.cs`\n- `Controllers/Result/ExamWizardController.cs`\n- `Views/ExamWizard/Index.cshtml`\n\n### Step 2: Enhance the Exam Wizard Service\nAdd these methods to `IExamWizardService` and `ExamWizardService`:\n\n1. **`LoadNctbTemplateAsync(int academicYearId, int classId, ExamTerm term)`** — Returns an `ExamWizardStateDto` with standard NCTB subject/component/mark configurations based on class and group (Science/Business Studies/Humanities). For Science group: Physics, Chemistry, Biology, Higher Math, etc. For Business Studies: Accounting, Finance, etc. For Humanities: History, Geography, etc.\n\n2. **`SaveTemplateAsync(ExamWizardCreateRequest request, string templateName, string userId)`** — Saves the exam configuration as a reusable template.\n\n3. **`LoadTemplateAsync(int templateId)`** — Loads a saved template.\n\n### Step 3: Enhance the Exam Wizard View\nUpdate `Views/ExamWizard/Index.cshtml` to:\n- Add \"Load NCTB Template\" button alongside \"Load Previous Exam\"\n- Add \"Save as Template\" button in Step 3\n- Add NCTB subject configurations for Science/Business Studies/Humanities groups\n- Ensure all Bootstrap classes are removed (currently 0 Bootstrap, keep it that way)\n- Use `_AdminLayout` explicitly\n\n### Step 4: Create NCTB Template DTOs\nAdd to `Models/DTOs/Result/ExamWizardDtos.cs`:\n- `NctbTemplateDto` — contains standard NCTB subject/component/mark configurations\n- `ExamTemplateDto` — for saved templates\n\n### Step 5: Register new services\nEnsure `IExamWizardService` and `ExamWizardService` are registered in DI.\n\nReturn the full contents of all files you read and all files you create/modify."
}
```

**Output:**
```
<task id="ses_0aa206414ffeIORr6RnSmJAWrb" state="completed">
<task_result>
**Build: 0 errors, 0 warnings**. All changes are complete. Here's a summary of what was implemented:

### Files Modified/Created

| File | Change |
|------|--------|
| `Models/Entities/Exam/ExamEntities.cs` | Added `ExamTemplate` entity (persistence for saved templates) |
| `Data/SchoolDbContext.cs` | Added `DbSet<ExamTemplate>` |
| `Models/DTOs/Result/ExamWizardDtos.cs` | Added 8 new DTOs: `NctbTemplateDto`, `NctbSubjectTemplateDto`, `NctbComponentTemplateDto`, `ExamTemplateDto`, `ExamTemplateListItemDto`, `SaveTemplateRequest`, `LoadNctbTemplateRequest`, `TemplateListRequest` |
| `Services/Interfaces/Result/IExamWizardService.cs` | Added 5 new methods: `LoadNctbTemplateAsync`, `SaveTemplateAsync`, `LoadTemplateAsync`, `ListTemplatesAsync`, `DeleteTemplateAsync` |
| `Services/Implementations/Result/ExamWizardService.cs` | Full implementation — hardcoded NCTB Science/Business Studies/Humanities/General templates with Written/MCQ/Practical component breakouts, template CRUD with JSON serialization |
| `Controllers/Result/ExamWizardController.cs` | Added 5 endpoints: `LoadNctbTemplate`, `SaveTemplate`, `LoadTemplate`, `ListTemplates`, `DeleteTemplate` |
| `Views/ExamWizard/Index.cshtml` | Full rewrite — NCTB button + class/group selector modal, component breakout table in Step 3, Save as Template modal with name/description, Saved Templates modal with load/delete |

### NCTB Templates Provided

- **Science** (class 9-10): Physics, Chemistry, Biology, Higher Math (optional), ICT — each with Written/MCQ/Practical components where applicable
- **Business Studies** (class 9-10): Accounting, Finance & Banking, Business Entrepreneurship, Economics — all with Written + MCQ components
- **Humanities** (class 9-10): History, Geography, Civics, Economics — all with Written + MCQ components
- **General** (class 1-8): Bangla, English, Math, Science, ICT, Arts & Crafts — with Written/MCQ/Practical where applicable

All NCTB subjects include Bengali names (`SubjectNameBn`) and standard NCTB codes (e.g., Physics = `136`, Chemistry = `137`). Component breakouts show full marks and pass marks for each component (Written, MCQ, Practical, Viva).
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 372.4s)

_Thinking:_

Good, the Exam Wizard enhancement is done. Now let me work on the other phases in parallel. Let me launch multiple agents for:

1. Phase 1 — Teacher Dashboard Enhancement
2. Phase 1 — Result Validation UI Enhancement
3. Phase 1 — Publishing Wizard Enhancement
4. Phase 1 — Bulk Report Cards Enhancement
5. Phase 1 — Promotion Wizard Enhancement
6. Phase 3 — Architecture Compliance (Bootstrap removal)

Let me launch these in parallel.

**Tool: task**

**Input:**
```json
{
  "description": "Enhance Teacher Dashboard",
  "subagent_type": "general",
  "prompt": "Enhance the Teacher Dashboard in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nThe Teacher Dashboard already exists at `Views/TeacherResult/Dashboard.cshtml` with `TeacherResultController` and `TeacherResultService`. I need to enhance it with:\n\n1. **Today's Classes** — Show today's scheduled classes from the routine\n2. **Quick Actions** — Direct links to mark entry, view schedule, etc.\n3. **Notifications** — Show recent notifications/alerts\n\nHere's what to do:\n\n### Step 1: Read existing files\nRead these files fully:\n- `Controllers/Result/TeacherResultController.cs`\n- `Services/Implementations/Result/TeacherResultService.cs`\n- `Services/Interfaces/Result/ITeacherResultService.cs`\n- `Models/DTOs/Result/TeacherResultDtos.cs`\n- `Views/TeacherResult/Dashboard.cshtml`\n\n### Step 2: Enhance the Teacher Dashboard DTO\nRead and update `Models/DTOs/Result/TeacherResultDtos.cs` to add:\n- `TodaySchedule` — list of today's classes with time, subject, class, section\n- `QuickActions` — list of quick action links\n- `Notifications` — list of recent notifications\n\n### Step 3: Enhance the Teacher Result Service\nUpdate `TeacherResultService.GetDashboardAsync` to also fetch:\n- Today's schedule from the routine (look for `IExamRoutineService` or `IRoutineService`)\n- Quick actions based on pending work\n\n### Step 4: Enhance the Teacher Dashboard View\nUpdate `Views/TeacherResult/Dashboard.cshtml` to add:\n- \"Today's Classes\" section showing scheduled classes with time, subject, class, section\n- \"Quick Actions\" section with direct links to mark entry, view schedule, etc.\n- \"Notifications\" section\n\n### Step 5: Enhance the Teacher Result Controller\nUpdate `TeacherResultController.cs` to pass additional data to the dashboard view.\n\nReturn the full contents of all files you read and all files you modify."
}
```

**Output:**
```
<task id="ses_0aa1a813dffendRd1GB9JucHCk" state="completed">
<task_result>
All changes are complete. **Build: 0 errors, 0 warnings.**

Here's a summary of what was modified:

### Files Modified

**1. `Models/DTOs/Result/TeacherResultDtos.cs`**
- Added 3 new DTOs: `TodayScheduleItemDto`, `QuickActionItemDto`, `NotificationItemDto`
- Added 3 new properties to `TeacherResultDashboardDto`: `TodaySchedule`, `QuickActions`, `Notifications`

**2. `Services/Implementations/Result/TeacherResultService.cs`**
- Injected `IRoutineEntryRepository` and `IRoutinePeriodRepository`
- **Today's Schedule**: Converts BD timezone (UTC+6) to BD day numbering (Sat=1..Fri=7), fetches teacher's routine entries via `sp_GetTeacherRoutineGrid`, filters by today's day number, joins with period data for start/end times
- **Quick Actions**: Builds 4 default actions (Mark Entry, View Schedule, My Subjects, View Reports) + prepends a "Pending Marks" alert if there are pending entries
- **Notifications**: Fetches unread `NotificationMessage` records (where `UserId == null` for system-wide) via `IUnitOfWork.Repository<NotificationMessage>()`

**Controller** (`TeacherResultController.cs`): No changes needed — it already passes the DTO to the view.

**Build: 0 errors, 0 warnings.**
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 305.3s)

_Thinking:_

Good. Now let me work on the remaining phases in parallel. Let me launch multiple agents for:

1. Phase 1 — Result Validation UI Enhancement
2. Phase 1 — Publishing Wizard Enhancement
3. Phase 1 — Bulk Report Cards Enhancement
4. Phase 1 — Promotion Wizard Enhancement
5. Phase 3 — Architecture Compliance (Bootstrap removal)

Let me launch these in parallel.

**Tool: task**

**Input:**
```json
{
  "description": "Enhance Publishing Wizard",
  "subagent_type": "general",
  "prompt": "Enhance the Result Publishing Wizard in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nThe Publishing Wizard already exists at `Views/AdminResult/PublishingWizard.cshtml` with a 3-step flow. I need to enhance it with:\n\n1. **Better Validation UI** — Show validation results with severity icons, per-class summaries, and blocking errors\n2. **Preview Step** — Show a preview of what will be published (pass/fail counts, GPA distribution)\n3. **Publish Confirmation** — Better confirmation with summary\n\nHere's what to do:\n\n### Step 1: Read existing files\nRead these files fully:\n- `Views/AdminResult/PublishingWizard.cshtml`\n- `Views/AdminResult/_ValidationSummary.cshtml`\n- `Controllers/Result/AdminResultController.cs`\n- `Services/Implementations/Result/ResultValidationService.cs`\n- `Services/Interfaces/Result/IResultValidationService.cs`\n- `Models/DTOs/Result/ResultValidationDtos.cs`\n\n### Step 2: Enhance the Publishing Wizard View\nUpdate `Views/AdminResult/PublishingWizard.cshtml` to:\n- Add a proper validation summary with per-class breakdown\n- Add a preview step showing pass/fail counts, GPA distribution\n- Add better publish confirmation with summary\n- Remove all inline styles (currently 18) — use universal CSS classes (`adm-*`, `dash-*`, `er-*`)\n- Remove the 1 Bootstrap class (`mb-4`)\n- Add explicit `_AdminLayout`\n- Add proper ARIA attributes\n\n### Step 3: Enhance the Validation Summary Partial\nUpdate `Views/AdminResult/_ValidationSummary.cshtml` to:\n- Show per-class validation results\n- Show severity-filtered issues\n- Use universal CSS classes\n\nReturn the full contents of all files you read and all files you modify."
}
```

**Output:**
```
<task id="ses_0aa14e74fffe1QjvnRx7KAAcjC" state="completed">
<task_result>
**Build: 0 errors, 0 warnings.** Both files enhanced and compiled successfully.

### Summary of changes

**`Views/AdminResult/PublishingWizard.cshtml`** — Full rewrite:
| Before | After |
|--------|-------|
| 3-step wizard | **4-step wizard** (added Preview step between Validation and Publish) |
| 15 inline `style=""` attributes | Zero inline styles — all moved to `.ws-wizard-*` CSS classes |
| 1 Bootstrap `mb-4` class | Removed — uses `.ws-wizard__no-mb` |
| Default layout | Explicit `Layout = "~/Views/Shared/_AdminLayout.cshtml"` |
| No ARIA | Full ARIA: `role="tablist/tab/tabpanel/region/alert/progressbar/group/status"`, `aria-selected/posinset/setsize/live/label/valuenow/valuemin/valuemax/pressed/hidden/required/describedby` |
| Step 2: flat validation stats | Per-class **breakdown cards** + severity filter buttons (All/Errors/Warnings/Info) |
| No preview step | **Step 3 Preview**: pass/fail progress bars, large GPA display with visual bar, GPA distribution grade bars (A+–F), per-class summary table with totals row |
| Step 3: simple 5-field summary | **Step 4 Confirm**: 6-card grid (Exam highlight, Students, Pass/Fail, Pass %, GPA, Issues) + lock checkbox + notes |

**`Views/AdminResult/_ValidationSummary.cshtml`** — Cleaned up:
| Before | After |
|--------|-------|
| Multiple inline `style=""` | All moved to `.er-validation-*` CSS classes |
| Bootstrap `mt-4`, `mb-4` | Removed — uses `er-validation-*` spacing |
| No ARIA | `role="region"`, `aria-label`, `aria-live="polite"`, `role="status"`, `aria-hidden` |
| Severity text only | Severity **icons** (SVG circle-X for errors, alert-triangle for warnings) inline in the badge |
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 493.3s)

_Thinking:_

Good. Now let me work on the remaining items in parallel:
1. Phase 1 — Bulk Report Cards Enhancement
2. Phase 1 — Promotion Wizard Enhancement
3. Phase 3 — Architecture Compliance (Bootstrap removal from ReportCard/Index.cshtml, MeritList/Index.cshtml, ResultPublishing.cshtml)

Let me launch these in parallel.

**Tool: task**

**Input:**
```json
{
  "description": "Enhance Bulk Report Cards",
  "subagent_type": "general",
  "prompt": "Enhance the Bulk Report Cards feature in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nThe Bulk Report Cards feature already exists at `Views/ReportCard/Bulk.cshtml` with `ReportCardController.GenerateBulk`. I need to enhance it with:\n\n1. **Section filter** — Add section dropdown alongside exam and class\n2. **Progress indicator** — Better progress during generation\n3. **Download options** — ZIP for multiple, single PDF for one class\n4. **Print Queue** — Add a \"Send to Print Queue\" option\n\nHere's what to do:\n\n### Step 1: Read existing files\nRead these files fully:\n- `Views/ReportCard/Bulk.cshtml`\n- `Controllers/Result/ReportCardController.cs`\n- `Services/Implementations/Result/ReportCardService.cs`\n- `Services/Interfaces/Result/IReportCardService.cs`\n\n### Step 2: Enhance the Bulk Report Cards View\nUpdate `Views/ReportCard/Bulk.cshtml` to:\n- Add section dropdown (loads via AJAX when class changes)\n- Add download format options (PDF, ZIP)\n- Add \"Send to Print Queue\" option\n- Add better progress indicator\n- Remove all inline styles (currently 1)\n- Ensure 0 Bootstrap classes\n- Add explicit `_AdminLayout`\n\n### Step 3: Enhance the Report Card Controller\nUpdate `ReportCardController.cs` to:\n- Add section filter to Bulk action\n- Add ZIP download option for multiple classes\n\nReturn the full contents of all files you read and all files you modify."
}
```

**Output:**
```
<task id="ses_0aa10476bffe6E65hV3TnHM9NS" state="completed">
<task_result>
All done. Here's a summary of the 6 files modified:

### Files Modified

**1. `Models/Entities/Result/ResultEntities.cs`** — Added `ReportCardPrintQueueItem` entity (ExamId, ClassId, SectionId, RequestedBy, RequestedAt, Status, TotalItems, CompletedItems, CompletedAt, ErrorMessage) + `ReportCardPrintStatus` enum (Pending/Processing/Completed/Failed)

**2. `Data/SchoolDbContext.cs`** — Added `DbSet<ReportCardPrintQueueItem> ReportCardPrintQueueItems`

**3. `Services/Interfaces/Result/IReportCardService.cs`** — Updated signatures:
- `GenerateBulkReportCardsAsync` now takes `int? sectionId, string format`
- `GetReportCardCountAsync` now takes `int? sectionId`
- New `AddToPrintQueueAsync(int examId, int? classId, int? sectionId, int totalItems, string requestedBy, CancellationToken)`

**4. `Services/Implementations/Result/ReportCardService.cs`** — Key changes:
- `GetReportCardCountAsync` / `GenerateBulkReportCardsAsync` filter by `sectionId`
- `format` param: `"pdf"` → combined PDF (existing behavior), `"zip"` → individual PDFs in ZIP archive via `System.IO.Compression.ZipArchive`
- Extracted `GetStudentIdsAsync`, `GenerateCombinedPdfAsync`, `GenerateZipAsync` private methods
- `AddToPrintQueueAsync` persists `ReportCardPrintQueueItem` via `IUnitOfWork`

**5. `Controllers/Result/ReportCardController.cs`** — Changes:
- `Bulk` GET: accepts `int? classId`, passes `ViewBag.SelectedClassId` + loads sections if classId given
- Added `GetSectionsByClass` GET endpoint (returns JSON `{id, name}` for AJAX)
- `GenerateBulk` POST: accepts `int? sectionId, string format`, builds contextual filename (includes Class/Section IDs), sets proper MIME type (application/zip vs application/pdf)
- Added `SendToPrintQueue` POST: counts report cards, creates print queue item, returns JSON result

**6. `Views/ReportCard/Bulk.cshtml`** — Complete rewrite:
- **Section dropdown** — loads via AJAX when class changes (jQuery `$.getJSON` to `GetSectionsByClass`)
- **Download format** — radio buttons: "PDF (Combined)" / "ZIP (Individual)"
- **Send to Print Queue** — separate button with its own AJAX POST + anti-forgery token
- **Progress indicator** — stages: "Counting..." (10%) → "Generating..." (30%) → "Download starting..." (90%) with animated SVG spinner
- **Result alerts** — success/error messages using `er-alert` CSS classes
- **Cleanup** — removed `<style>` block, removed inline Bootstrap classes, uses `adm-*`/`er-*` Universal CSS only

### Build Status
**0 errors, 135 warnings** (all pre-existing, none introduced)
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 36.0s)

_Thinking:_

Good. Now let me work on the remaining items in parallel:
1. Phase 1 — Promotion Wizard Enhancement
2. Phase 3 — Architecture Compliance (Bootstrap removal from ReportCard/Index.cshtml, MeritList/Index.cshtml, ResultPublishing.cshtml)

Let me launch these in parallel.

**Tool: task**

**Input:**
```json
{
  "description": "Enhance Promotion Wizard",
  "subagent_type": "general",
  "prompt": "Enhance the Promotion Wizard in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nThe Promotion Wizard already exists at `Views/PromotionWizard/Index.cshtml` with a 4-step flow. I need to enhance it with:\n\n1. **Better Preview** — Show student-level details with filtering (eligible/conditional/failed)\n2. **Bulk Operations** — Select/deselect individual students for promotion\n3. **Roll Generation Preview** — Show what roll numbers will be assigned\n4. **Subject Assignment Preview** — Show what subjects will be assigned\n5. **Confirmation Step** — Better confirmation with summary\n\nHere's what to do:\n\n### Step 1: Read existing files\nRead these files fully:\n- `Views/PromotionWizard/Index.cshtml`\n- `Controllers/Result/PromotionWizardController.cs`\n- `Services/Implementations/Result/PromotionWizardService.cs`\n- `Services/Interfaces/Result/IPromotionWizardService.cs`\n- `Models/DTOs/Result/PromotionWizardDtos.cs`\n\n### Step 2: Enhance the Promotion Wizard View\nUpdate `Views/PromotionWizard/Index.cshtml` to:\n- Add student-level filtering (eligible/conditional/failed tabs)\n- Add roll number preview\n- Add subject assignment preview\n- Add better confirmation step\n- Remove all inline styles (currently 0, keep it that way)\n- Add explicit `_AdminLayout`\n- Add proper ARIA attributes\n\n### Step 3: Enhance the Promotion Wizard DTOs\nUpdate `Models/DTOs/Result/PromotionWizardDtos.cs` to add:\n- Roll number preview data\n- Subject assignment preview data\n\nReturn the full contents of all files you read and all files you modify."
}
```

**Output:**
```
<task id="ses_0aa08bd75ffejfiJAM2Oi6A3Oy" state="completed">
<task_result>
Let me also examine the roll generation service and subject mapping service to understand their existing DTOs:
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 359.0s)

_Thinking:_

The agent seems to be still working. Let me check if it completed. Let me also launch the architecture compliance work in parallel.

**Tool: task**

**Input:**
```json
{
  "description": "Fix Bootstrap in result views",
  "subagent_type": "general",
  "prompt": "Fix Bootstrap usage in 3 key views in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nThe following views have Bootstrap classes that need to be replaced with universal CSS (`adm-*`, `dash-*`, `ws-*`, `er-*`):\n\n1. **`Views/ReportCard/Index.cshtml`** — 31 Bootstrap classes, 3 inline styles\n2. **`Views/MeritList/Index.cshtml`** — 25 Bootstrap classes, 4 inline styles\n3. **`Views/AdminResult/ResultPublishing.cshtml`** — 10 Bootstrap classes, 54 inline styles\n\n### Step 1: Read each file fully\nRead these files:\n- `Views/ReportCard/Index.cshtml`\n- `Views/MeritList/Index.cshtml`\n- `Views/AdminResult/ResultPublishing.cshtml`\n\n### Step 2: Fix each file\n\nFor each file:\n1. Replace all Bootstrap classes with universal CSS equivalents:\n   - `row` → `adm-form-grid` or `dash-stats-row`\n   - `col-md-*` → `adm-form-group` or `dash-stat-cell`\n   - `form-label` → `adm-label`\n   - `form-select` → `adm-select`\n   - `d-flex` → `er-flex`\n   - `align-items-end` → `er-align-end`\n   - `align-items-center` → `er-align-center`\n   - `w-100` → `er-w-full`\n   - `text-center` → `er-text-center`\n   - `font-weight-bold` → `er-font-bold`\n   - `text-primary` → `er-text-primary`\n   - `text-success` → `er-text-success`\n   - `py-1` → `er-py-1`\n   - `px-3` → `er-px-3`\n   - `me-2` → `er-mr-2`\n   - `table-responsive` → `er-table-scroll`\n   - `mb-4` → `er-mb-4`\n   - `mt-4` → `er-mt-4`\n   - `text-end` → `er-text-right`\n   - `row` → `adm-form-grid`\n   - `g-3` → `adm-form-grid--gap`\n   - `col-md-*` → `adm-form-group`\n   - `form-label` → `adm-label`\n   - `form-select` → `adm-select`\n   - `d-flex` → `er-flex`\n   - `align-items-end` → `er-align-end`\n   - `align-items-center` → `er-align-center`\n   - `w-100` → `er-w-full`\n   - `text-center` → `er-text-center`\n   - `font-weight-bold` → `er-font-bold`\n   - `text-primary` → `er-text-primary`\n   - `text-success` → `er-text-success`\n   - `py-1` → `er-py-1`\n   - `px-3` → `er-px-3`\n   - `me-2` → `er-mr-2`\n   - `table-responsive` → `er-table-scroll`\n\nReturn the full contents of all files you read and all files you modify."
}
```

**Output:**
```
<task id="ses_0aa081f19ffeDzVVg7Ho9AFj2s" state="completed">
<task_result>
Build succeeded — 0 errors. Here's a summary of all replacements:

### `Views/ReportCard/Index.cshtml` — 14 replacements
| Bootstrap | Replaced with |
|-----------|--------------|
| `row g-3` | `adm-form-grid adm-form-grid--gap` |
| `col-md-4`, `col-md-3` (×3) | `adm-form-group` |
| `col-md-2 d-flex align-items-end` | `adm-form-group er-flex er-align-end` |
| `form-label` (×3) | `adm-label` |
| `form-select` (×3) | `adm-select` |
| `w-100` | `er-w-full` |
| `table-responsive` | `er-table-scroll` |
| `py-1 px-3 d-flex align-items-center` | `er-py-1 er-px-3 er-flex er-align-center` |
| `me-2` | `er-mr-2` |
| `text-center font-weight-bold` (×3) | `er-text-center er-font-bold` |
| `text-center text-primary font-weight-bold` | `er-text-center er-text-primary er-font-bold` |
| `text-center` (×1) | `er-text-center` |
| inline `style="text-align:right"` | `class="er-text-right"` |

### `Views/MeritList/Index.cshtml` — 11 replacements
| Bootstrap | Replaced with |
|-----------|--------------|
| `row g-3` | `adm-form-grid adm-form-grid--gap` |
| `col-md-5` (×2) | `adm-form-group` |
| `col-md-2 d-flex align-items-end` | `adm-form-group er-flex er-align-end` |
| `form-label` (×2) | `adm-label` |
| `form-select` (×2) | `adm-select` |
| `w-100` | `er-w-full` |
| `table-responsive` | `er-table-scroll` |
| `me-2` | `er-mr-2` |
| `text-center font-weight-bold` (×2) | `er-text-center er-font-bold` |
| `text-center font-weight-bold text-success` | `er-text-center er-font-bold er-text-success` |
| `text-center text-primary font-weight-bold` | `er-text-center er-text-primary er-font-bold` |
| `text-center` (×1) | `er-text-center` |

### `Views/AdminResult/ResultPublishing.cshtml` — 7 replacements
| Bootstrap | Replaced with |
|-----------|--------------|
| `mb-4` | `er-mb-4` |
| `mt-4` | `er-mt-4` |
| `text-end` | `er-text-right` |
| `table-responsive` | `er-table-scroll` |
| `py-1 px-3` (×3) | `er-py-1 er-px-3` |
| inline `style="text-align:center"` | `class="er-text-center"` |

**Result**: 0 Bootstrap classes remain in any of the 3 files. Complex inline styles using `var(--adm-*)` CSS custom properties and medal badge gradients are retained as they have no utility class equivalent. Build: 0 errors.
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 9.7s)

_Thinking:_

Good progress. Let me now check the build status and then work on the remaining items. Let me also check on the Promotion Wizard enhancement that was still running.

Let me now:
1. Check build status
2. Check on the Promotion Wizard task
3. Work on Phase 2 (Automation) and Phase 4 (Enterprise Hardening)

**Tool: bash**

**Input:**
```json
{
  "description": "Check build status",
  "command": "dotnet build 2>&1 | Select-String -Pattern \"error|Error|Build|warning|Warning\"",
  "timeout": 120000,
  "workdir": "G:\\PROJECT .NET\\SchoolMS\\clonelatest"
}
```

**Output:**
```

Build succeeded.
    0 Warning(s)
    0 Error(s)



```

---

## Assistant (Build · DeepSeek V4 Flash Free · 171.8s)

_Thinking:_

Build: 0 errors, 0 warnings. Let me now work on the remaining phases. Let me launch multiple agents for:

1. Phase 2 — Automation: Auto Teacher Assignment, Auto Schedule Generator
2. Phase 4 — Enterprise Hardening: Performance, caching, notifications, security

Let me also check on the Promotion Wizard task.

Build: **0 errors, 0 warnings**. Let me now work on Phase 2 (Automation) and Phase 4 (Enterprise Hardening) in parallel.

**Tool: task**

**Input:**
```json
{
  "description": "Auto Teacher Assignment service",
  "subagent_type": "general",
  "prompt": "Implement Auto Teacher Assignment in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nThe system already has `TeacherSubjectAssignment` entities and `ITeacherAssignmentService`. I need to create an auto-assignment feature that automatically assigns teachers to exam subjects based on their existing class-subject assignments.\n\nHere's what to do:\n\n### Step 1: Read existing files\nRead these files fully:\n- `Services/Interfaces/Result/IExamWizardService.cs`\n- `Services/Implementations/Result/ExamWizardService.cs`\n- `Services/Interfaces/Teachers/ITeacherAssignmentService.cs`\n- `Services/Implementations/Teachers/TeacherAssignmentService.cs` (if exists)\n- `Models/Entities/Teachers/TeacherSubjectAssignment.cs`\n- `Models/Entities/Exam/ExamEntities.cs` (ExamSubject entity)\n\n### Step 2: Create Auto Teacher Assignment Service\nCreate a new service `IAutoTeacherAssignmentService` and `AutoTeacherAssignmentService` that:\n- Takes an exam ID\n- For each ExamSubject in the exam, finds the teacher assigned to that subject+class+section+group in `TeacherSubjectAssignment`\n- Auto-assigns the teacher to the ExamSubject record\n- Returns a result with counts of assigned/skipped\n\n### Step 3: Create the controller endpoint\nAdd an endpoint to `ExamController.cs` or create a new controller for auto-assignment.\n\n### Step 4: Register the service in DI\nAdd the service registration to `ServiceRegistration.cs` or `Program.cs`.\n\nReturn the full contents of all files you read and all files you create/modify."
}
```

**Output:**
```
<task id="ses_0aa0293f5ffekCEYqZoFmrYUP9" state="completed">
<task_result>
**Build: 0 errors, 135 warnings (all pre-existing).**

Here's a summary of what was created/modified:

### Files Created

| File | Purpose |
|------|---------|
| `Models/DTOs/Exam/AutoTeacherAssignmentResultDto.cs` | Result DTO with counts + per-subject details |
| `Services/Interfaces/Teachers/IAutoTeacherAssignmentService.cs` | Interface with `AutoAssignTeachersAsync(examId)` |
| `Services/Implementations/Teachers/AutoTeacherAssignmentService.cs` | Implementation — matches each ExamSubject to `TeacherSubjectAssignment` by `AcademicYearId`, `ClassId`, `SubjectId`, `SectionId`, `GroupId` |

### Files Modified

| File | Change |
|------|--------|
| `Controllers/Exam/ExamController.cs` | Added `IAutoTeacherAssignmentService` injection + `AutoAssignTeachers` POST endpoint (returns JSON for AJAX, redirect for form) |
| `Extensions/ServiceRegistration.cs` | Added `services.AddScoped<IAutoTeacherAssignmentService, AutoTeacherAssignmentService>()` |

### How It Works

1. **POST** `/Exam/AutoAssignTeachers?examId=N` with anti-forgery token
2. Loads the exam + its active `ExamSubject` records
3. For each subject, queries `TeacherSubjectAssignment` matching `AcademicYearId`, `ClassId`, `SubjectId`, `SectionId` (from exam), `GroupId` (from exam)
4. Writes `TeacherId` back to `ExamSubject` for every match
5. Returns `AutoTeacherAssignmentResultDto` with `Assigned`/`Skipped` counts + per-subject detail
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 321.2s)

_Thinking:_

Good. Now let me work on the Schedule Generator and Phase 4 in parallel.

**Tool: task**

**Input:**
```json
{
  "description": "Auto Schedule Generator service",
  "subagent_type": "general",
  "prompt": "Implement Auto Schedule Generator in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nThe system already has `ExamSchedule` entities and `IExamScheduleService`. I need to create an auto-schedule generation feature that automatically generates exam schedules/routines.\n\nHere's what to do:\n\n### Step 1: Read existing files\nRead these files fully:\n- `Models/Entities/Exam/ExamEntities.cs` (Exam, ExamSubject, ExamSchedule entities)\n- `Services/Interfaces/Result/IExamService.cs`\n- `Services/Implementations/Result/ExamService.cs`\n- `Repositories/Interfaces/Result/IExamRepository.cs`\n- `Repositories/Implementations/Result/ExamRepository.cs` (or ResultRepository.cs)\n- `Controllers/Exam/ExamScheduleController.cs`\n- `Services/Interfaces/Exam/IExamScheduleService.cs` (if exists)\n- `Services/Implementations/Exam/ExamScheduleService.cs` (if exists)\n\n### Step 2: Create Schedule Generation Service\nCreate a new service `IScheduleGenerationService` and `ScheduleGenerationService` that:\n- Takes an exam ID\n- For each ExamSubject in the exam, generates an ExamSchedule entry\n- Handles:\n  - Teacher conflicts (same teacher can't be in two places at once)\n  - Room conflicts (same room can't have two exams at once)\n  - Holiday conflicts (skip Fridays and holidays)\n  - Working hours (8 AM - 4 PM)\n  - Exam duration (from ExamSubject)\n  - Gap between exams (30 minutes minimum)\n- Returns a result with counts of scheduled/skipped\n\n### Step 3: Create the controller endpoint\nAdd an endpoint to `ExamScheduleController.cs` for auto-schedule generation.\n\n### Step 4: Register the service in DI\nAdd the service registration to `ServiceRegistration.cs`.\n\nReturn the full contents of all files you read and all files you create/modify."
}
```

**Output:**
```
<task id="ses_0a9ffe3eeffes10eYrP301Ta74" state="completed">
<task_result>
Build: **0 errors, 0 new warnings** (all 136 warnings pre-existing).

Here's a summary of all files created/modified:

## Files Created

### 1. `Models/DTOs/Exam/AutoScheduleResultDto.cs`
Result DTO with `AutoScheduleResultDto` (exam info, scheduled/skipped counts, warnings) and `AutoScheduleItemDto` (per-subject result with date/time/room).

### 2. `Services/Interfaces/Exam/IAutoScheduleService.cs`
Interface with a single method `GenerateScheduleAsync(int examId)`.

### 3. `Services/Implementations/Exam/AutoScheduleService.cs`
Full auto-schedule algorithm:
- **Blocked dates**: Fridays (Bangladesh weekend) + `HolidayMaster` entries (both exact and recurring)
- **Room pool**: `Room` entities from the Routine module; falls back to subject-configured rooms or default "Room 10x"
- **Working hours**: 8:00 AM – 4:00 PM
- **Min gap**: 30 minutes between exams
- **Duration**: `ExamSubject.ExamDuration` or 180 min default
- **Conflict detection**: room (same room/date/time overlap) + teacher (same teacher/date/time overlap via pre-loaded lookup)
- **Sorting**: non-optional first, then preferred date, then subject name
- **Existing schedules**: preserved (skipped); teacher assignments pre-loaded via dict to avoid N+1

## Files Modified

### 4. `Controllers/Exam/ExamScheduleController.cs`
- Injected `IAutoScheduleService`
- Added `AutoGenerate` POST endpoint with `[ValidateAntiForgeryToken]`

### 5. `Extensions/ServiceRegistration.cs`
- Added `services.AddScoped<IAutoScheduleService, AutoScheduleService>()`
</task_result>
</task>
```

---

## Assistant (Build · DeepSeek V4 Flash Free · 11.6s)

_Thinking:_

Good. Now let me work on Phase 4 — Enterprise Hardening. Let me launch agents for:
1. Performance optimization (caching, AsNoTracking, etc.)
2. Security hardening (anti-forgery, IDOR, XSS, CSRF, file upload, audit logging)
3. Notifications infrastructure

Let me also check the build status first.

**Tool: bash**

**Input:**
```json
{
  "description": "Check build status",
  "command": "dotnet build 2>&1 | Select-String -Pattern \"error|Error|Build|warning|Warning\"",
  "timeout": 120000,
  "workdir": "G:\\PROJECT .NET\\SchoolMS\\clonelatest"
}
```

**Output:**
```

Build succeeded.
    0 Warning(s)
    0 Error(s)



```

---

## Assistant (Build · DeepSeek V4 Flash Free · 323.4s)

_Thinking:_

Build: 0 errors, 0 warnings. Let me now work on Phase 4 — Enterprise Hardening. Let me launch multiple agents for:
1. Performance optimization
2. Security hardening
3. Notifications infrastructure

Build: **0 errors, 0 warnings**. Now Phase 4 — Enterprise Hardening in parallel.

**Tool: task**

**Input:**
```json
{
  "description": "Performance optimization pass",
  "subagent_type": "general",
  "prompt": "Performance optimization pass for the Result/Exam modules in this School Management System at G:\\PROJECT .NET\\SchoolMS\\clonelatest.\n\nThe audit found these performance issues:\n1. Methods missing `AsNoTracking()` in SchoolClassService, HolidayMasterService, CalendarDashboardService\n2. N+1 query patterns in SectionService, CalendarGenerationService\n3. Sequential DB round-trips in AcademicCalendarController\n4. No caching for infrequent data (academic years, holiday lists)\n\nHere's what to do:\n\n### Step 1: Read and fix files with missing AsNoTracking\nSearch for files that use `.Query()` or `.QueryNoTracking()` in the Services folder. Fix any that use `.Query()` without `.AsNoTracking()` for read-only operations.\n\nRead and fix these files:\n- `Services/Implementations/Academic/SchoolClassService.cs`\n- `Services/Implementations/Academic/HolidayMasterService.cs`\n- `Services/Implementations/Academic/CalendarDashboardService.cs`\n\nFor each file, ensure read-only queries use `QueryNoTracking()` instead of `Query()`.\n\n### Step 2: Add caching for infrequent data\nAdd `IMemoryCache` caching to:\n- `AcademicYearService.GetAllYearsAsync` — cache for 30 minutes\n- `HolidayMasterService` — cache holiday lists for 1 hour\n\n### Step 3: Fix N+1 query patterns\nCheck `SectionService.GetByClassIdAsync` for N+1 patterns and fix them.\n\nReturn the full contents of all files you read and all files you modify."
}
```

**Output:**
```
<task id="ses_0a9fadfd1ffe8vyv3bm4b2Uh4a" state="completed">
<task_result>

</task_result>
</task>
```

---

## User

continue

---

## Assistant (Build · Big Pickle · 7.3s)

---

## User

continue

---

## Assistant (Build · MiMo V2.5 Free)

---

