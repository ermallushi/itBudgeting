using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Application.DTOs;

public record BudgetVersionDto(
    Guid Id,
    string Name,
    int Year,
    BudgetVersionType Type,
    BudgetVersionStatus Status,
    Guid? ParentVersionId,
    string CreatedBy,
    DateTime CreatedAt
);

public record CreateBudgetVersionDto(
    string Name,
    int Year,
    BudgetVersionType Type,
    string CreatedBy,
    Guid? ParentVersionId = null
);

public record UpdateBudgetVersionDto(
    string Name
);
