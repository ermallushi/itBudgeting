using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Application.DTOs;

public record BudgetLineDto(
    Guid Id,
    Guid VersionId,
    Guid CostCenterId,
    Guid ProjectId,
    BudgetCategory Category,
    string Period,
    decimal PlannedAmount,
    decimal ApprovedAmount,
    decimal CommittedAmount,
    decimal ActualAmount,
    decimal RemainingAmount,
    bool IsOverBudget
);

public record CreateBudgetLineDto(
    Guid VersionId,
    Guid CostCenterId,
    Guid ProjectId,
    BudgetCategory Category,
    string Period,
    decimal PlannedAmount
);

public record UpdateBudgetLineDto(
    decimal PlannedAmount,
    decimal ApprovedAmount,
    decimal ActualAmount
);
