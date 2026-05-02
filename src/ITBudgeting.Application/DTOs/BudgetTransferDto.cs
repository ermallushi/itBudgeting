using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Application.DTOs;

public record BudgetTransferDto(
    Guid Id,
    Guid FromProjectId,
    Guid ToProjectId,
    decimal Amount,
    BudgetCategory Category,
    string Period,
    string Reason,
    string ApprovedBy,
    string CreatedBy,
    DateTime CreatedAt
);

public record CreateBudgetTransferDto(
    Guid VersionId,
    Guid FromProjectId,
    Guid ToProjectId,
    decimal Amount,
    BudgetCategory Category,
    string Period,
    string Reason,
    string ApprovedBy,
    string CreatedBy
);
