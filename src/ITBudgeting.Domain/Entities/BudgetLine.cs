using ITBudgeting.Domain.Enums;

namespace ITBudgeting.Domain.Entities;

public class BudgetLine
{
    public Guid Id { get; private set; }
    public Guid VersionId { get; private set; }
    public Guid CostCenterId { get; private set; }
    public Guid ProjectId { get; private set; }
    public BudgetCategory Category { get; private set; }
    public string Period { get; private set; } = string.Empty; // YYYY-MM
    public decimal PlannedAmount { get; private set; }
    public decimal ApprovedAmount { get; private set; }
    public decimal CommittedAmount { get; private set; }
    public decimal ActualAmount { get; private set; }

    public BudgetVersion? Version { get; private set; }
    public CostCenter? CostCenter { get; private set; }
    public Project? Project { get; private set; }

    // Computed
    public decimal RemainingAmount => ApprovedAmount - CommittedAmount - ActualAmount;
    public bool IsOverBudget => CommittedAmount > ApprovedAmount;

    private BudgetLine() { }

    public BudgetLine(Guid versionId, Guid costCenterId, Guid projectId,
        BudgetCategory category, string period, decimal plannedAmount)
    {
        Id = Guid.NewGuid();
        VersionId = versionId;
        CostCenterId = costCenterId;
        ProjectId = projectId;
        Category = category;
        Period = period;
        PlannedAmount = plannedAmount;
        ApprovedAmount = 0;
        CommittedAmount = 0;
        ActualAmount = 0;
    }

    public void UpdateAmounts(decimal plannedAmount, decimal approvedAmount, decimal actualAmount)
    {
        PlannedAmount = plannedAmount;
        ApprovedAmount = approvedAmount;
        ActualAmount = actualAmount;
    }

    public void UpdateCommittedAmount(decimal committedAmount)
    {
        CommittedAmount = committedAmount;
    }

    public BudgetLine Clone(Guid newVersionId)
    {
        return new BudgetLine
        {
            Id = Guid.NewGuid(),
            VersionId = newVersionId,
            CostCenterId = CostCenterId,
            ProjectId = ProjectId,
            Category = Category,
            Period = Period,
            PlannedAmount = PlannedAmount,
            ApprovedAmount = ApprovedAmount,
            CommittedAmount = 0,
            ActualAmount = 0
        };
    }
}
