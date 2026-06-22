# Session Summary

## Goal
Migrate ID card PDF rendering from DinkToPdf/wkhtmltopdf to Microsoft Playwright Chromium for modern CSS support (flexbox, grid, gradients). Keep portrait CR80 orientation (53.98mm × 85.60mm).

## Constraints & Preferences
- Do NOT create new controllers, modify routes, download endpoints, QR generation, ViewModels, or business logic.
- Keep existing workflow: IdCardController → ViewRendererService → Render Razor HTML → PDF Generator → Download PDF.
- Only replace the PDF engine; keep PlainPdfGenerator's iText methods for report cards/transcripts.
- Only modify: PlainPdfGenerator.cs, idcard-print.css, 4 card partials, 2 PrintIdCard views, Program.cs/ServiceRegistration.cs.
- Card dimensions: CR80 portrait (53.98mm × 85.60mm).
- QR must contain only StudentNo or EmployeeCode — no name/phone/address/DB id.
- 0 build errors, all 541 tests must pass.
- Microsoft.Playwright v1.60.0 installed; Chromium browser must be installed on target machine.

## Progress
### Done
- **PlaywrightPdfEngine** (`Helpers/Pdf/PlaywrightPdfEngine.cs`): Singleton browser pool — single Chromium instance with thread-safe lazy initialization; `Convert(html, isBulk)` sync wrapper returns `byte[]` PDF. Page dimensions: single=53.98×85.60mm, bulk=297×210mm.
- **PlainPdfGenerator** refactored: Injects `PlaywrightPdfEngine` (singleton via DI); `GenerateIdCardPdf()` and `GenerateFromHtml()` delegate to Playwright; iText methods (report card, transcript, admit card) unchanged. DinkToPdf dependency fully removed.
- **`PrepareHtmlForPdf()`** updated: Keeps `<base>` tag (Chromium resolves relative paths properly) instead of removing it.
- **idcard-print.css** full rewrite: Portrait CR80 (53.98mm×85.60mm); flexbox layout; modern gradient (#0F172A→#1E40AF); gold accent (#D4AF37); photo→name→badge→data-grid stack; back side with left info + right QR; bulk 3-column grid auto-flow.
- **4 card partials** redesigned for portrait stacked layout (photo centered, name+badge below, data grid below, footer at bottom).
- **2 PrintIdCard views** cleaned up: Removed CDN CSS (fonts.googleapis, font-awesome); removed extraneous body classes; `card-group`→`card-pair` for CSS grid bulk layout.
- **ServiceRegistration.cs**: Added `services.AddSingleton<PlaywrightPdfEngine>()`.
- **Build: 0 errors, 0 warnings. Tests: 541/541 passing**.

### Blocked
- (none)

## Key Decisions
- Security pattern: centralized `IFeeSecurityService` injected into all controllers.
- Waiver/Refund workflow: ledger writes deferred exclusively to `ApproveAsync`.
- Late fee rule precedence: 4-tier (class+category > class > category > global), capped at `MaxFee`.
- Accountant dashboard: reuse `FeeDashboardDto` rather than new ViewModel.
- Finance status filter: use typed `PaymentStatus.Paid` enum instead of magic number.
- Academic year filter: passive — null means all-time data (backward compatible).
- Invoice number dedup: query max existing seq for year, offset `ROW_NUMBER()` by it.
- Guardian security: use `IGuardianService.UserHasAccessToStudentAsync` for IDOR (no new security service).
- Guardian finance reuse: `GuardianFinanceController` delegates to existing `IStudentFinanceService` — no new service/repo/SP.
- Widget reuse: guardian tabs reuse `IDashboardRepository` widget methods directly from `DashboardService`.
- Teacher dashboard: widget queries use LINQ/EF, not SPs (small data volumes; SP pattern reserved for paged grids).
- Parent Portal: mapped as Guardian alias — no new role, entity, or service duplication.

## Build & Test Status
- **Build: 0 errors, 0 warnings**.
- **Tests: 541/541 passing** (all phases).

## Completed (this session)
- **Phase 42H.1-42H.7 — Optional Guardian Portal Architecture**: Added 4 toggle fields (`EnableGuardianPortal`, `EnableGuardianActivation`, `RequireGuardianForAdmission`, `EnableGuardianNotifications`) to `SchoolSetting` (default `false`). All guardian controllers check `ISchoolSettingRepository.GetCurrentSettingsAsync().EnableGuardianPortal` on entry and redirect when disabled. `_Layout.cshtml` conditionally renders guardian sidebar. `GuardianService` notification methods guard behind `EnableGuardianNotifications`. `AdmissionService.ApproveAndConvertAsync` guards guardian creation behind portal/activation toggles. `GuardianFinanceControllerTests` (12) + `Phase37B_AdmissionSecurityFixTests` updated for new `ISchoolSettingRepository` constructor parameter.

## Next Steps
1. Run `Data\FinanceInitialization.sql` against production SQL Server to initialize finance data.
2. (Future) Implement Phase 42H.6 — email-based guardian activation flow (activation controller already scaffolded).

## Relevant Files
- `Models/Entities/Website/WebsiteEntities.cs`: Added 4 guardian portal toggle fields.
- `Services/Implementations/Website/WebsiteServices.cs`: Persist new settings fields.
- `Views/WebsiteAdmin/Settings.cshtml`: Guardian Portal Settings section with 4 checkboxes.
- `Controllers/Guardian/GuardianPortalController.cs`: Feature check on Dashboard action.
- `Controllers/Guardian/GuardianPortalPagesController.cs`: Feature check on all 13 actions.
- `Controllers/Guardian/GuardianActivationController.cs`: Feature check on activate GET/POST.
- `Controllers/Dashboard/GuardianFinanceController.cs`: Feature check on all 5 endpoints.
- `Controllers/Dashboard/DashboardController.cs`: Guardian/Parent routing feature check.
- `Services/Implementations/Admissions/AdmissionService.cs`: Guardian creation gated behind portal/activation toggles.
- `Services/Implementations/Guardian/GuardianService.cs`: Notification methods guarded by `EnableGuardianNotifications`.
- `Views/Shared/_Layout.cshtml`: Conditional guardian sidebar rendering based on `EnableGuardianPortal`.
- `SchoolManagementSystem.Tests/Services/GuardianFinanceControllerTests.cs`: 12 tests updated for new `ISchoolSettingRepository` param.
- `SchoolManagementSystem.Tests/Services/Phase37B_AdmissionSecurityFixTests.cs`: Updated for new `ISchoolSettingRepository` param.
- `Data/FinanceInitialization.sql`: **New** — enterprise data initialization script.
- `Controllers/Dashboard/DashboardController.cs`: Role-based routing (now includes Accountant).
- `Controllers/Dashboard/StudentFinanceController.cs`: 5 JSON endpoints for student finance center.
- `Controllers/Dashboard/GuardianFinanceController.cs`: **New** — 5 JSON endpoints for guardian child finance center.
- `Controllers/Fees/Fee*Controller.cs` (15 files): All use `IFeeSecurityService`.
- `Services/Implementations/Fees/FeeSecurityService.cs`: Centralized IDOR/ownership security.
- `Services/Implementations/Fees/FeeInvoiceService.cs`: `ApplyLateFeesAsync` engine, ledger writes.
- `Services/Implementations/Fees/FeePaymentService.cs`: Ledger writes, invoice recalculation.
- `Services/Implementations/Fees/FeeReceiptService.cs`: PDF receipt with QR code.
- `Services/Implementations/Fees/StudentFinanceService.cs`: **New** — student finance grid orchestration.
- `Repositories/Implementations/Dashboard/DashboardRepositories.cs`: Aggregation + widget queries.
- `Repositories/Implementations/Fees/StudentFinanceRepository.cs`: **New** — SP-calling finance repository.
- `Service/Implementations/Dashboard/DashboardService.cs`: Dashboard orchestrator (Student + Guardian).
- `Models/ViewModels/Dashboard/StudentDashboardViewModel.cs`: Widget properties, IsResultBlocked.
- `Models/ViewModels/Dashboard/GuardianDashboardViewModel.cs`: **Extended** — 12 widget properties.
- `Views/Dashboard/StudentIndex.cshtml`: 6-tab student dashboard with Tabulator + widgets.
- `Views/Dashboard/GuardianIndex.cshtml`: **Extended** — 6-tab guardian dashboard with Tabulator + widgets.
- `Models/Entities/Fees/FeesEntities.cs`: All finance entity definitions.
- `Models/Enums/SchoolEnums.cs`: All enums (PaymentStatus, FeeLedgerType, etc.).
- `Data/StoredProcedures/Student/`: 6 stored procedures for student paged grids.

### Guardian Features in Student Dashboard
- Extended `StudentDashboardViewModel` with: `PresentCount`, `AbsentCount`, `LateCount`, `LeaveCount`, `AttendanceHistory`, `GuardianName`, `GuardianCode`, `Alerts`, `LeaveApplicationCount`, `PendingLeaveCount`
- Updated `IDashboardRepository`/`IDashboardQueryRepository`/`DashboardRepository.GetStudentDashboardDataAsync` to return individual attendance status counts instead of combined `presentAttendance`
- Updated `DashboardService.GetStudentDashboardAsync` to populate guardian info, attendance breakdown, alerts, attendance history, and leave counts
- Enhanced `StudentIndex.cshtml` with: alerts section, guardian name in hero, attendance breakdown stat, leave stat, Chart.js doughnut chart, recent attendance history table
- **Build: 0 errors. Tests: 541/541 passing.**
