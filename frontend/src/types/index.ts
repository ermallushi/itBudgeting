export enum BudgetVersionType {
  Annual = 'Annual',
  Revision = 'Revision',
  Forecast = 'Forecast',
}

export enum BudgetVersionStatus {
  Draft = 'Draft',
  Submitted = 'Submitted',
  Approved = 'Approved',
  Locked = 'Locked',
}

export enum BudgetCategory {
  CapEx = 'CapEx',
  OpEx = 'OpEx',
}

export enum PurchaseRequestStatus {
  Draft = 'Draft',
  Submitted = 'Submitted',
  Approved = 'Approved',
  Rejected = 'Rejected',
}

export enum PurchaseOrderStatus {
  Open = 'Open',
  PartiallyUsed = 'PartiallyUsed',
  FullyUsed = 'FullyUsed',
  Closed = 'Closed',
}

export enum PeriodLockType {
  None = 'None',
  PartialLock = 'PartialLock',
  FullLock = 'FullLock',
}

export type Role = 'BudgetUser' | 'Manager' | 'FinanceController' | 'Admin';

export interface BudgetVersion {
  id: number;
  name: string;
  year: number;
  type: BudgetVersionType;
  status: BudgetVersionStatus;
  createdBy: string;
  createdAt: string;
}

export interface BudgetLine {
  id: number;
  budgetVersionId: number;
  costCenterId: number;
  costCenterName?: string;
  projectId: number;
  projectName?: string;
  category: BudgetCategory;
  jan: number; feb: number; mar: number; apr: number;
  may: number; jun: number; jul: number; aug: number;
  sep: number; oct: number; nov: number; dec: number;
  planned: number;
  approved: number;
  committed: number;
  actual: number;
}

export interface CostCenter {
  id: number;
  name: string;
  code: string;
  description?: string;
}

export interface Project {
  id: number;
  name: string;
  code: string;
  costCenterId: number;
  costCenterName?: string;
  category: BudgetCategory;
  description?: string;
}

export interface PurchaseRequest {
  id: number;
  projectId: number;
  projectName?: string;
  amount: number;
  status: PurchaseRequestStatus;
  sharePointUrl?: string;
  createdBy: string;
  createdAt: string;
}

export interface PurchaseOrder {
  id: number;
  purchaseRequestId: number;
  amountApproved: number;
  amountUsed: number;
  remaining: number;
  status: PurchaseOrderStatus;
}

export interface BudgetTransfer {
  id: number;
  fromProjectId: number;
  fromProjectName?: string;
  toProjectId: number;
  toProjectName?: string;
  amount: number;
  category: BudgetCategory;
  period: string;
  reason: string;
  createdBy: string;
  createdAt: string;
}

export interface BudgetPeriod {
  id: number;
  budgetVersionId: number;
  periodName: string;
  periodNumber: number;
  isOpen: boolean;
  lockType: PeriodLockType;
}

export interface BudgetAudit {
  id: number;
  entityType: string;
  entityId: number;
  field: string;
  oldValue: string;
  newValue: string;
  changedBy: string;
  changedAt: string;
}

export interface ReportSummaryRow {
  projectId: number;
  projectName: string;
  month: string;
  planned: number;
  approved: number;
  committed: number;
  actual: number;
}

export interface ReportVarianceRow {
  projectId: number;
  projectName: string;
  month: string;
  planned: number;
  actual: number;
  variance: number;
}
