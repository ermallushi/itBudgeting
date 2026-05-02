# IT Budgeting & Forecasting System

Enterprise budget lifecycle and commitment control platform that replaces Excel-based IT budgeting. Modelled after SAP CO + IM + Procurement integration concepts.

---

## Architecture

```
frontend/          React 18 + TypeScript + AG Grid (Excel-like UI)
    ↓
src/ITBudgeting.API            ASP.NET Core 8 Web API
    ↓
src/ITBudgeting.Application    Application Services, DTOs, Interfaces
    ↓
src/ITBudgeting.Domain         Entities, Enums, Business Rules, Domain Exceptions
    ↓
src/ITBudgeting.Infrastructure EF Core 8 DbContext, Repositories, Migrations
    ↓
SQL Server
```

---

## Features

- **Budget Planning** — CapEx / OpEx monthly planning per cost center and project
- **Budget Lifecycle** — Draft → Submitted → Approved → Locked status machine
- **Revision System** — Clone approved version into a forecast/revision; only open months editable
- **Period Management** — Full year (months 1–12) with configurable open/lock windows
- **PR / PO Tracking** — Purchase Requests linked to SharePoint approvals, Purchase Orders consume committed budget
- **Budget Transfers** — Reallocate funds between projects/cost centers with validation and audit trail
- **Commitment Control** — Remaining = Approved − Committed (PO) − Actual; over-budget flagged
- **Audit Trail** — Every change recorded (entity, field, old/new value, user, timestamp)
- **Reports** — Summary and variance views per version
- **Role-Based Access** — BudgetUser, Manager, FinanceController, Admin

---

## Solution Structure

```
ITBudgeting.slnx
├── src/
│   ├── ITBudgeting.Domain          Entities, Enums, Exceptions
│   ├── ITBudgeting.Application     Services, DTOs, Interfaces
│   ├── ITBudgeting.Infrastructure  EF Core DbContext, Repositories, UnitOfWork
│   └── ITBudgeting.API             Controllers, Middleware, Program.cs
├── tests/
│   └── ITBudgeting.Tests           26 xUnit unit tests (all passing)
└── frontend/                       React + Vite + AG Grid frontend
```

---

## Domain Model

| Entity | Key Fields |
|---|---|
| `BudgetVersion` | Name, Year, Type (Draft/Approved/Forecast/Revision), Status, ParentVersionId |
| `BudgetLine` | VersionId, CostCenterId, ProjectId, Category, Period (YYYY-MM), Planned/Approved/Committed/Actual |
| `CostCenter` | Name, Department |
| `Project` | Name, CostCenterId, Category (CapEx/OpEx) |
| `PurchaseRequest` | ProjectId, Amount, Status, SharePointUrl |
| `PurchaseOrder` | PRId, AmountApproved, AmountUsed, RemainingAmount, Status |
| `BudgetTransfer` | FromProjectId, ToProjectId, Amount, Category, Period, Reason |
| `BudgetAudit` | EntityType, EntityId, Field, OldValue, NewValue, ChangedBy |
| `BudgetPeriod` | VersionId, Month, Year, IsOpen, LockType |

---

## Business Rules

1. **Period Lock** — Cannot edit budget lines when `BudgetPeriod.IsOpen == false` or `LockType == FullLock`
2. **Revision Cloning** — Creating a Revision version clones all budget lines from the parent; only specified months are opened
3. **PO Consumption** — `BudgetLine.CommittedAmount` = sum of all approved PO amounts for that project/period
4. **Transfer Validation** — Transfer only allowed when source has sufficient available budget AND the period is open
5. **Status Transitions** — Draft → Submitted → Approved → Locked; approved versions cannot be edited, only cloned
6. **Over-Budget Flag** — Flagged when `CommittedAmount > ApprovedAmount`

---

## Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download)
- [Node.js 18+](https://nodejs.org/)
- SQL Server (or SQL Server Express / Azure SQL)

---

## Backend Setup

```bash
# 1. Restore & build
dotnet restore
dotnet build

# 2. Update connection string in:
#    src/ITBudgeting.API/appsettings.json
#    src/ITBudgeting.Infrastructure/BudgetingDbContextFactory.cs

# 3. Apply EF migrations (creates the database)
dotnet ef database update --project src/ITBudgeting.Infrastructure --startup-project src/ITBudgeting.API

# 4. Run the API (http://localhost:5000, Swagger at /swagger)
dotnet run --project src/ITBudgeting.API

# 5. Run tests
dotnet test
```

---

## Frontend Setup

```bash
cd frontend
npm install
npm run dev      # Development server at http://localhost:5173
npm run build    # Production build → dist/
```

### Demo Login

The frontend uses a mock login. Enter any password and one of these usernames to get the corresponding role:

| Username | Role |
|---|---|
| `admin` | Admin |
| `manager` | Manager |
| `financecontroller` | FinanceController |
| anything else | BudgetUser |

---

## API Reference (Swagger)

With the API running, visit **http://localhost:5000/swagger** for the full interactive API documentation.

Key endpoint groups:

| Group | Base path |
|---|---|
| Budget Versions | `GET/POST /api/budget-versions` |
| Budget Lines | `GET/POST /api/budget-lines` |
| Cost Centers | `GET/POST /api/cost-centers` |
| Projects | `GET/POST /api/projects` |
| Purchase Requests | `GET/POST /api/purchase-requests` |
| Purchase Orders | `GET/POST /api/purchase-orders` |
| Budget Transfers | `GET/POST /api/budget-transfers` |
| Budget Periods | `GET/POST /api/budget-periods` |
| Audit | `GET /api/audit` |
| Reports | `GET /api/reports/summary`, `GET /api/reports/variance` |

---

## Frontend Pages

| Page | Route | Description |
|---|---|---|
| Login | `/login` | Role-based mock auth |
| Dashboard | `/` | Summary cards + quick links |
| Budget Versions | `/budget-versions` | Manage versions; submit/approve/lock/clone |
| Budget Lines | `/budget-versions/:id/lines` | AG Grid spreadsheet view with monthly columns |
| Cost Centers | `/cost-centers` | CRUD with inline editing |
| Projects | `/projects` | CRUD with CapEx/OpEx filter |
| Purchase Requests | `/purchase-requests` | PR tracking + approval |
| Purchase Orders | `/purchase-orders` | PO tracking + usage recording |
| Budget Transfers | `/budget-transfers` | Fund reallocation between projects |
| Budget Periods | `/budget-periods/:versionId` | Open/close months per version |
| Reports | `/reports` | Summary and variance analysis |
| Audit Log | `/audit` | Full change history |

---

## Color Coding

| Status / State | Color |
|---|---|
| Approved row | Green `#d4edda` |
| Draft row | Yellow `#fff3cd` |
| Submitted row | Blue `#d1ecf1` |
| Locked row | Red-pink `#f8d7da` |
| Over-budget cell | Red background |
| Near limit (>80%) cell | Yellow background |
| Under budget cell | Green background |

---

## Security

JWT Bearer authentication is configured. Roles:

- **BudgetUser** — view and edit draft budget lines
- **Manager** — approve PRs, open/close periods
- **FinanceController** — approve budget versions
- **Admin** — full access including lock and user management

Configure the JWT secret in `appsettings.json` under `Jwt:Key`.

