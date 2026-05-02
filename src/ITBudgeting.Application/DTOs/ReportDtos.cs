using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Application.DTOs;

public record CloneRevisionDto(
    string Name,
    string CreatedBy,
    List<int> OpenMonths
);

public record BudgetSummaryLineDto(
    Guid ProjectId,
    string ProjectName,
    Guid CostCenterId,
    string CostCenterName,
    BudgetCategory Category,
    string Period,
    decimal PlannedAmount,
    decimal ApprovedAmount,
    decimal CommittedAmount,
    decimal ActualAmount,
    decimal RemainingAmount,
    bool IsOverBudget
);

public record BudgetVarianceLineDto(
    Guid ProjectId,
    string ProjectName,
    string Period,
    decimal ApprovedAmount,
    decimal ActualAmount,
    decimal Variance,
    decimal VariancePercent
);
