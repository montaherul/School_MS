# School Management System — Project Report

---

## 1. Project Description

The **School Management System** is an Enterprise-grade web application built on ASP.NET Core 8 MVC with Entity Framework Core and SQL Server. It serves as a unified platform for managing all aspects of a school's operations — from student admission and academic scheduling to fee management, finance accounting, payments, examinations, result processing, and guardian portal access. The system follows Clean Architecture with SOLID principles, Repository Pattern, UnitOfWork, Service Layer separation, and Dependency Injection throughout.

The application is deployed for **Chattogram Collegiate School & College** and supports the Bangladesh NCTB curriculum across Primary, Secondary, Science, Business Studies, and Humanities streams. It includes a dedicated SchoolPay payment orchestration platform, an AI Chat module for students, and a comprehensive accounting subsystem (Chart of Accounts, Journal Engine, General Ledger, Trial Balance, Financial Statements, Bank Book, and Financial Periods).

---

## 2. Technology Stack

| Layer | Technology |
|-------|-----------|
| **Framework** | ASP.NET Core 8 MVC |
| **ORM** | Entity Framework Core 8 with Stored Procedures |
| **Database** | SQL Server 2022 (MONTAHERUL\SQLEXPRESS) |
| **Authentication** | ASP.NET Core Identity with PBKDF2-SHA256 |
| **Authorization** | Role-Based Access Control (RBAC) with Permission-based `[RequirePermission]` |
| **UI Framework** | Universal CSS (adm-\*, dash-\*, ws-\* classes only — no Bootstrap/Tailwind) |
| **Charts** | Chart.js |
| **Data Tables** | Tabulator.js |
| **Payments** | SchoolPay — SSLCommerz, bKash, Nagad, Rocket, Visa, MasterCard, TouchPhone, USSD |
| **Payment Security** | PaymentGatewaySecurityEvent with 23 event types, 11 audit columns, 5 DB indexes |
| **AI Module** | OpenAI Responses API with IOpenAIClient abstraction, token budgeting, rate limiting |
| **DI Container** | Microsoft.Extensions.DependencyInjection |
| **Background Services** | IHostedService / BackgroundService for automation (invoice generation, usage logging) |
| **Architecture** | Clean Architecture (Presentation → Controller → Service → Repository → Stored Procedure → Database) |
| **Pattern** | Repository Pattern + UnitOfWork + DTO Mapping (manual, no AutoMapper) |
| **Transaction** | TransactionScope for multi-step workflows |
| **Logging** | Serilog (Information → Warning) |

---

## 3. Functional Requirements

### FR-01: Student Management
- Student admission with 8-step wizard (personal info, guardian, documents, class assignment)
- Student profile with full academic history, fee assignments, invoices, and ledgers
- Student promotion with auto fee structure migration
- Student transfer between classes/sections
- Student ID card generation (bulk and individual)
- Student leave applications and attendance tracking

### FR-02: Guardian Management
- Guardian registration and profile management
- Guardian-student relationship mapping (one-to-many)
- Guardian portal with attendance, results, fee, and notice views
- Guardian notification system (email, SMS, in-app)
- Guardian fee payment and receipt access

### FR-03: Academic Management
- Class, Section, Subject management with NCTB curriculum support
- Class subject mapping with group-based and elective subject support
- Teacher assignment to classes and subjects
- Academic year, session, and term management
- Academic calendar with holiday and working day tracking
- Routine generation and conflict detection
- Syllabus, Lesson Plans, and Study Materials CRUD

### FR-04: Examination & Result Management
- Exam creation with configurations, components, and mark structures
- Exam scheduling with class/section/subject assignment
- Marks entry with teacher export sheets
- Result calculation (GPA, grade, position, merit)
- Result publication with locking and audit trail
- Report card generation (bulk and individual)
- Transcript generation
- Promotion processing with eligibility rules

### FR-05: Fee Management
- Fee structure wizard with class-based template creation
- Student fee profile with assignment tracking
- Monthly/quarterly/half-yearly/yearly invoice generation (SP-driven)
- Invoice management with discount, late fee, and waiver support
- Fee collection with cash, bank, and online payment methods
- Payment receipt generation
- Fee refund processing
- Fee waiver approval workflow
- Cash book with opening/closing balances
- Finance posting engine (FeeCollection, FeeWaiver, BankReceipt, BankPayment → Journal → Ledger)

### FR-06: Finance & Accounting (FIN-02)
- **Chart of Accounts** — 4 account types (Asset, Liability, Income, Expense) with hierarchy
- **Journal Engine** — Balanced journal entry creation with auto-posting
- **General Ledger** — Entry-level debit/credit tracking with running balance
- **Trial Balance** — Period-balanced trial balance report
- **Financial Statements** — Income Statement and Balance Sheet generation
- **Bank Book** — Daily bank transaction ledger with reconciliation
- **Financial Periods** — Open → Locked → Closed state machine
- **Posting Engine** — Auto-creates financial periods, validates balanced entries
- **Account Mappings** — 12 transaction type mappings (FeeCollection, FeeWaiver, FeeDiscount, FeeRefund, LateFee, Fine, BankReceipt, BankPayment, AdmissionFee, AdmissionRefund)

### FR-07: Payment Gateway (SchoolPay)
- Multi-provider support: SSLCommerz, bKash, Nagad, Rocket, Visa, MasterCard, TouchPhone, USSD
- Checkout orchestration with payment routing
- Webhook processing with idempotency
- Settlement and refund management
- Payment reconciliation with auto-match and manual match
- Sandbox testing environment
- DLQ (Dead Letter Queue) for failed webhooks
- Failover with circuit breaker pattern
- Security audit with 23 event types and 11 audit columns (CorrelationId, RequestId, UserAgent, Severity, etc.)
- 5 database indexes on PaymentGatewaySecurityEvents
- 14 integration tests with 10/10 pass rate

### FR-08: AI Chat Module (AI-01)
- Student-only AI chat with Bangladesh NCTB curriculum context
- Conversation management (Active, Archived, Deleted status)
- SSE streaming responses
- Token budget management (4000-token limit)
- Rate limiting (30 req/min, 500 req/day per student)
- Conversation auto-titling (first user message truncated to 55 chars)
- Background usage logging with bounded channel
- PII masking and prompt injection detection (InputGuard)
- OpenAI client abstraction (swappable provider)

### FR-09: Employee & Staff Management
- Employee workforce onboarding with synchronization from Users
- Teacher profile with subject specialization and class assignments
- Employee attendance tracking
- Employee salary management
- Leave management for staff

### FR-10: Reporting & Analytics
- Dashboard metrics for all modules
- Admission trend analysis and conversion funnel
- Class, section, and subject-wise academic reports
- Attendance analytics (daily, monthly, class-wise)
- Cash flow statement
- Finance analytics dashboard
- Monthly/quarterly/yearly collection reports

### FR-11: System Administration
- RBAC with 18 roles (Super Admin, Principal, Assistant Head, Senior Lecturer, Lecturer, Office Staff, Accountant, Librarian, Lab Assistant, Transport Staff, Support Staff, Guardian, Student, Admin, Exam Controller, etc.)
- 955 permissions across 30+ modules with CRUD + custom actions
- 38 system users with full role assignment
- Audit logging for all important actions
- User session tracking
- System settings and school profile management

### FR-12: Automation Engine
- Auto-generate monthly/quarterly/half-yearly/yearly invoices
- Auto-assign fee structure on admission (by class match)
- Auto-migrate fee structure on promotion
- Auto-copy fee structures for new academic year
- Background scheduler for recurring billing and guardian notifications

---

## 4. Data Flow Diagrams (DFD)

### DFD-01: Student Admission & Fee Assignment Flow

```
┌─────────────┐     ┌──────────────┐     ┌─────────────┐     ┌─────────────────┐
│   Guardian   │────▶│  Admission   │────▶│   Student   │────▶│  Fee Structure  │
│   Portal     │     │   Wizard     │     │   Record    │     │   Auto-Assign   │
└─────────────┘     └──────────────┘     └─────────────┘     └────────┬────────┘
                                                                       │
                                                                       ▼
┌─────────────┐     ┌──────────────┐     ┌─────────────┐     ┌─────────────────┐
│   Payment    │◀────│  Fee Invoice │◀────│  Student    │◀────│  Invoice        │
│   Gateway    │     │  Generation  │     │  Fee Assign │     │  (Monthly)      │
│ (SSLCommerz) │     └──────────────┘     └─────────────┘     └─────────────────┘
└─────────────┘
       │
       ▼
┌─────────────┐     ┌──────────────┐     ┌─────────────────┐
│  Payment     │────▶│  Fee Ledger  │────▶│  Cash Book     │
│  Receipt     │     │  (Debit/Cr)  │     │  (Daily Agg.)   │
└─────────────┘     └──────────────┘     └─────────────────┘
```

**Description:** This DFD traces the flow from guardian admission application through student record creation, automatic fee structure assignment, invoice generation, online/offline payment processing, fee ledger posting, and daily cash book aggregation.

---

### DFD-02: Finance Posting Engine Flow (Fee → Accounting)

```
┌─────────────┐     ┌──────────────┐     ┌─────────────┐     ┌─────────────────┐
│  Fee         │────▶│  Finance     │────▶│  Journal    │────▶│  General        │
│  Collection  │     │  Posting     │     │  Entry      │     │  Ledger         │
│  (Cashier)   │     │  Engine      │     │  (sp_Post)  │     │  (sp_GetGL)     │
└─────────────┘     └──────────────┘     └─────────────┘     └────────┬────────┘
                                                                       │
                                                                       ▼
┌─────────────┐     ┌──────────────┐     ┌─────────────────┐
│  Trial       │◀────│  Financial   │◀────│  Financial      │
│  Balance     │     │  Periods     │     │  Period (Open→  │
│  (sp_GetTB)  │     │  (Open/Lock/ │     │   Locked/Closed)│
└─────────────┘     │   Closed)    │     └─────────────────┘
                     └──────────────┘

┌─────────────┐     ┌──────────────┐     ┌─────────────────┐
│  Income      │◀────│  Financial   │◀────│  Chart of        │
│  Statement   │     │  Statements  │     │  Accounts (COA)  │
│  (sp_IS)     │     │  (sp_BS)     │     │  4 seed accounts │
└─────────────┘     └──────────────┘     └─────────────────┘
```

**Description:** This DFD maps the end-to-end finance automation flow: cashier collection triggers the FinancePostingService, which creates journal entries via `sp_PostJournalEntry`, posts to General Ledger via `sp_GetGeneralLedger`, runs Trial Balance via `sp_GetTrialBalance`, generates Financial Statements (Income Statement via `sp_GetIncomeStatement`, Balance Sheet via `sp_GetBalanceSheet`), all within locked financial periods managed by `sp_CloseFinancialPeriod`.

---

### DFD-03: SchoolPay Payment & Security Audit Flow

```
┌─────────────┐     ┌──────────────┐     ┌─────────────┐     ┌─────────────────┐
│  Student/    │────▶│  SchoolPay   │────▶│  Payment    │────▶│  Payment         │
│  Guardian    │     │  Checkout    │     │  Provider   │     │  Gateway         │
│  Portal      │     │  (Orchestr.) │     │  (SSLCommerz│     │  Transaction     │
└─────────────┘     └──────────────┘     │   bKash etc) │     │  Record          │
                                          └─────────────┘     └────────┬────────┘
                                                                       │
                                              ┌──────────────────────┤
                                              ▼                      ▼
                                      ┌─────────────┐     ┌─────────────────┐
                                      │  Webhook     │────▶│  PaymentGateway  │
                                      │  Processing  │     │  Security Events │
                                      │  (Idempotent)│     │  (23 event types,│
                                      └─────────────┘     │  11 audit cols,  │
                                                           │  5 indexes)      │
                                                           └─────────────────┘

                                                      ┌─────────────────┐
                                                      │  Settlement     │
                                                      │  Reconciliation │
                                                      │  DLQ + Failover │
                                                      └─────────────────┘
```

**Description:** This DFD traces the SchoolPay payment orchestration platform flow: user initiates payment via SchoolPay checkout, which routes to appropriate provider (SSLCommerz, bKash, etc.), processes the gateway transaction, handles webhook callbacks with idempotency and DLQ, records security audit events with full correlation metadata, performs settlement reconciliation with auto-match and manual match, and implements circuit breaker failover for resilience.

---

## 5. Non-Functional Requirements

### NAS-01: Security Requirements
- **Authentication:** ASP.NET Core Identity with PBKDF2-SHA256 password hashing (100,000 iterations)
- **Authorization:** Role-Based Access Control (RBAC) with 18 roles and 955 granular permissions
- **Permission Enforcement:** `[RequirePermission("Module.Action")]` attribute on every controller action
- **Anti-Forgery:** `@Html.AntiForgeryToken()` on all forms; `[ValidateAntiForgeryToken]` on POST endpoints
- **Input Validation:** Client-side + Server-side + Database-level validation; no trust of browser input
- **SQL Injection Prevention:** All queries through parameterized stored procedures or EF Core parameterized queries; no raw SQL in controllers
- **IDOR Prevention:** Ownership validation on all data access; users can only access their own data
- **Sensitive Data:** No sensitive exception leakage to client; structured error responses only
- **Payment Security:** 23 event types in `PaymentSecurityEventType` enum; 11 audit columns (CorrelationId, RequestId, UserAgent, Severity, EventSource, MachineName, SessionId, GatewayTransactionId); 5 database indexes on `PaymentGatewaySecurityEvents`
- **Secure Configuration:** Connection strings encrypted; production secrets via environment variables; HTTPS enforced

### NAS-02: Performance Requirements
- **Query Optimization:** `AsNoTracking()` for read-only queries; projection to DTOs; no unnecessary `Include()`
- **Stored Procedures:** All enterprise queries (dashboard, reports, analytics, tabulator, search, paging, bulk operations) use stored procedures — never raw SQL in controllers
- **Pagination:** All list endpoints use server-side paging (SP-driven) with `Tabulator.js` frontend
- **Caching:** EF Second-Level Cache for reference data; output caching for dashboard metrics
- **Database Indexes:** 5 indexes on `PaymentGatewaySecurityEvents`; proper indexes on all FK columns; clustered indexes on all PKs
- **Bulk Operations:** Batch processing for invoice generation, fee assignment, promotion, and ID card generation
- **Background Processing:** `IHostedService` / `BackgroundService` for invoice automation, usage logging, and health checks — never blocking HTTP requests
- **Target Response Times:** Page loads < 2 seconds; API responses < 500ms for paged queries; < 2s for reports

### NAS-03: Scalability & Availability Requirements
- **Stateless Architecture:** Controllers are stateless; all session data in database or distributed cache
- **Database Connection Pooling:** `MultipleActiveResultSets=true` in connection string; connection pooling enabled
- **Horizontal Scalability:** Application can be deployed behind load balancer; session affinity not required
- **High Availability:** SQL Server Always-On availability groups supported; application can run in multi-instance mode
- **Auto-Recovery:** Background services auto-restart on failure; payment webhook DLQ with retry mechanism
- **Circuit Breaker:** SchoolPay payment provider failover with automatic circuit breaker and fallback routing
- **99.9% Uptime Target:** System designed for continuous school operations; maintenance windows during holidays

### NAS-04: Maintainability & Code Quality Requirements
- **Clean Architecture:** Strict layering (Presentation → Controller → Service → Repository → Stored Procedure → Database); no layer skipping
- **SOLID Principles:** Single Responsibility, Open/Closed, Liskov Substitution, Interface Segregation, Dependency Inversion
- **DRY (Don't Repeat Yourself):** No duplicated entities, services, repositories, or stored procedures
- **KISS (Keep It Simple):** Readable names; small methods; no magic numbers; no overly complex abstractions
- **No AutoMapper:** Manual DTO mapping preferred for explicit control and testability
- **Async APIs:** All repository and service methods are async; controllers use async actions
- **Code Review Standards:** Every PR reviewed; no merge without peer review; architecture rules enforced by AGENTS.md constitution
- **Zero Build Warnings:** Build must produce 0 errors and 0 warnings
- **Test Coverage:** Unit tests for services, repositories, and security logic; integration tests for payment gateway

### NAS-05: Usability & Accessibility Requirements
- **Responsive Design:** Full support for 320px, 360px, 375px, 390px, 412px, 480px, 576px, 768px, 992px, 1200px, 1400px, 1600px, 1920px viewports
- **No Horizontal Scrolling:** Layout adapts to all screen sizes without horizontal overflow
- **Keyboard Navigation:** All interactive elements reachable via keyboard; focus states visible
- **ARIA Labels:** All form controls, navigation, tables, and dynamic content have proper ARIA attributes
- **Screen Reader Support:** Semantic HTML; proper heading hierarchy; live regions for dynamic updates
- **Color Contrast:** All text meets WCAG 2.1 AA contrast ratio requirements (minimum 4.5:1 for normal text, 3:1 for large text)
- **Universal CSS Only:** Only `adm-*`, `dash-*`, and `ws-*` CSS classes used; no Bootstrap, Tailwind, Material UI, or AdminLTE
- **Bangladesh NCTB Curriculum Support:** All UI labels, reports, and print outputs support Bengali text rendering
- **Loading States:** All async operations show loading indicators; no blank screens during data loading
- **Empty States:** All list views show meaningful empty state when no data exists
- **Error Handling:** User-friendly error messages; no raw exception details exposed to users

### DFD-01 (Data Flow Diagram for Admission & Fee Assignment)
See Section 4, DFD-01 above.

### DFD-02 (Data Flow Diagram for Finance Posting Engine)
See Section 4, DFD-02 above.

### DFD-03 (Data Flow Diagram for SchoolPay Payment & Security)
See Section 4, DFD-03 above.

---

## Appendix A: Architecture Rules (from AGENTS.md)

| Rule | Description |
|------|-------------|
| **No Controller SQL** | Controllers must NEVER access SQL, DbContext, or contain business logic |
| **No Layer Skipping** | Views → Controllers → Services → Repositories → Stored Procedures → Database |
| **Repository Pattern** | Every repository has Interface + Implementation + DI + Async APIs + Transaction support |
| **Service Layer** | Services handle business logic, workflows, validation, transactions, audit, notifications, permissions |
| **DTO Mapping** | Views never receive Entities; Controllers never expose Entities |
| **Stored Procedures** | All enterprise queries use SPs; simple CRUD may use EF Repository |
| **Transaction Support** | Multi-step workflows (Admission, Promotion, Transfer, Finance, Result Publish) execute inside transactions |
| **Audit Logging** | Every Create, Update, Delete, Approve, Reject, Convert, Promote, Transfer, Finance, Login, Permission action creates audit log |
| **Permission Enforcement** | Every controller decorated with `[RequirePermission("Module.Action")]`; seeded in RBAC seeder |
| **Zero Bootstrap/Tailwind** | Only Universal CSS (`adm-*`, `dash-*`, `ws-*`) allowed; no inline CSS |
| **Validation** | Client + Server + Database validation; never trust browser input |

---

*Report Version: 1.0*
*Project: School Management System (Enterprise)*
*School: Chattogram Collegiate School & College*
*Framework: ASP.NET Core 8 MVC + EF Core + SQL Server*
*Status: Production Ready*