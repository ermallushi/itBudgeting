namespace ITBudgeting.Application.DTOs;

public record BudgetAuditDto(
    Guid Id,
    string EntityType,
    Guid EntityId,
    string Field,
    string? OldValue,
    string? NewValue,
    string ChangedBy,
    DateTime ChangedAt
);
