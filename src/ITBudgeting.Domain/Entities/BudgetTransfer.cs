using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Domain.Entities;

public class BudgetTransfer
{
    public Guid Id { get; private set; }
    public Guid FromProjectId { get; private set; }
    public Guid ToProjectId { get; private set; }
    public decimal Amount { get; private set; }
    public BudgetCategory Category { get; private set; }
    public string Period { get; private set; } = string.Empty; // YYYY-MM
    public string Reason { get; private set; } = string.Empty;
    public string ApprovedBy { get; private set; } = string.Empty;
    public string CreatedBy { get; private set; } = string.Empty;
    public DateTime CreatedAt { get; private set; }

    public Project? FromProject { get; private set; }
    public Project? ToProject { get; private set; }

    private BudgetTransfer() { }

    public BudgetTransfer(Guid fromProjectId, Guid toProjectId, decimal amount,
        BudgetCategory category, string period, string reason, string approvedBy, string createdBy)
    {
        Id = Guid.NewGuid();
        FromProjectId = fromProjectId;
        ToProjectId = toProjectId;
        Amount = amount;
        Category = category;
        Period = period;
        Reason = reason;
        ApprovedBy = approvedBy;
        CreatedBy = createdBy;
        CreatedAt = DateTime.UtcNow;
    }
}
