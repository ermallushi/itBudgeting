using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Application.DTOs;

public record ProjectDto(Guid Id, string Name, Guid CostCenterId, BudgetCategory Category, bool IsActive);
public record CreateProjectDto(string Name, Guid CostCenterId, BudgetCategory Category);
public record UpdateProjectDto(string Name, BudgetCategory Category, bool IsActive);
