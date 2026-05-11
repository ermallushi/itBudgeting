namespace ITBudgeting.Application.DTOs;

public record CostCenterDto(Guid Id, string Name, string Department);
public record CreateCostCenterDto(string Name, string Department);
public record UpdateCostCenterDto(string Name, string Department);
