namespace ITBudgeting.Domain.Entities;

public class BudgetAudit
{
    public Guid Id { get; private set; }
    public string EntityType { get; private set; } = string.Empty;
    public Guid EntityId { get; private set; }
    public string Field { get; private set; } = string.Empty;
    public string? OldValue { get; private set; }
    public string? NewValue { get; private set; }
    public string ChangedBy { get; private set; } = string.Empty;
    public DateTime ChangedAt { get; private set; }

    private BudgetAudit() { }

    public BudgetAudit(string entityType, Guid entityId, string field,
        string? oldValue, string? newValue, string changedBy)
    {
        Id = Guid.NewGuid();
        EntityType = entityType;
        EntityId = entityId;
        Field = field;
        OldValue = oldValue;
        NewValue = newValue;
        ChangedBy = changedBy;
        ChangedAt = DateTime.UtcNow;
    }
}
